﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using System.Linq;
using Framework.Constants;
using Game.Maps;
using Game.Networking;

namespace Game.Entities;

public class SceneObject : WorldObject
{
	readonly SceneObjectData _sceneObjectData;
	readonly Position _stationaryPosition = new();
	ObjectGuid _createdBySpellCast;

	public SceneObject() : base(false)
	{
		ObjectTypeMask |= TypeMask.SceneObject;
		ObjectTypeId = TypeId.SceneObject;

		_updateFlag.Stationary = true;
		_updateFlag.SceneObject = true;

		_sceneObjectData = new SceneObjectData();
		_stationaryPosition = new Position();
	}

	public override void AddToWorld()
	{
		if (!IsInWorld)
		{
			GetMap().GetObjectsStore().Add(GetGUID(), this);
			base.AddToWorld();
		}
	}

	public override void RemoveFromWorld()
	{
		if (IsInWorld)
		{
			base.RemoveFromWorld();
			GetMap().GetObjectsStore().Remove(GetGUID());
		}
	}

	public override void Update(uint diff)
	{
		base.Update(diff);

		if (ShouldBeRemoved())
			Remove();
	}

	public static SceneObject CreateSceneObject(uint sceneId, Unit creator, Position pos, ObjectGuid privateObjectOwner)
	{
		var sceneTemplate = Global.ObjectMgr.GetSceneTemplate(sceneId);

		if (sceneTemplate == null)
			return null;

		var lowGuid = creator.GetMap().GenerateLowGuid(HighGuid.SceneObject);

		SceneObject sceneObject = new();

		if (!sceneObject.Create(lowGuid, SceneType.Normal, sceneId, sceneTemplate != null ? sceneTemplate.ScenePackageId : 0, creator.GetMap(), creator, pos, privateObjectOwner))
		{
			sceneObject.Dispose();

			return null;
		}

		return sceneObject;
	}

	public override void BuildValuesCreate(WorldPacket data, Player target)
	{
		var flags = GetUpdateFieldFlagsFor(target);
		WorldPacket buffer = new();

		ObjectData.WriteCreate(buffer, flags, this, target);
		_sceneObjectData.WriteCreate(buffer, flags, this, target);

		data.WriteUInt32(buffer.GetSize());
		data.WriteUInt8((byte)flags);
		data.WriteBytes(buffer);
	}

	public override void BuildValuesUpdate(WorldPacket data, Player target)
	{
		var flags = GetUpdateFieldFlagsFor(target);
		WorldPacket buffer = new();

		buffer.WriteUInt32(Values.GetChangedObjectTypeMask());

		if (Values.HasChanged(TypeId.Object))
			ObjectData.WriteUpdate(buffer, flags, this, target);

		if (Values.HasChanged(TypeId.SceneObject))
			_sceneObjectData.WriteUpdate(buffer, flags, this, target);

		data.WriteUInt32(buffer.GetSize());
		data.WriteBytes(buffer);
	}

	public override void ClearUpdateMask(bool remove)
	{
		Values.ClearChangesMask(_sceneObjectData);
		base.ClearUpdateMask(remove);
	}

	public override ObjectGuid GetOwnerGUID()
	{
		return _sceneObjectData.CreatedBy;
	}

	public override uint GetFaction()
	{
		return 0;
	}

	public override float GetStationaryX()
	{
		return _stationaryPosition.X;
	}

	public override float GetStationaryY()
	{
		return _stationaryPosition.Y;
	}

	public override float GetStationaryZ()
	{
		return _stationaryPosition.Z;
	}

	public override float GetStationaryO()
	{
		return _stationaryPosition.Orientation;
	}

	public void SetCreatedBySpellCast(ObjectGuid castId)
	{
		_createdBySpellCast = castId;
	}

	void Remove()
	{
		if (IsInWorld)
			AddObjectToRemoveList();
	}

	bool ShouldBeRemoved()
	{
		var creator = Global.ObjAccessor.GetUnit(this, GetOwnerGUID());

		if (creator == null)
			return true;

		if (!_createdBySpellCast.IsEmpty())
		{
			// search for a dummy aura on creator

			var linkedAura = creator.GetAuraQuery().HasSpellId(_createdBySpellCast.GetEntry()).HasCastId(_createdBySpellCast).GetResults().FirstOrDefault();

			if (linkedAura == null)
				return true;
		}

		return false;
	}

	bool Create(ulong lowGuid, SceneType type, uint sceneId, uint scriptPackageId, Map map, Unit creator, Position pos, ObjectGuid privateObjectOwner)
	{
		SetMap(map);
		Location.Relocate(pos);
		RelocateStationaryPosition(pos);

		SetPrivateObjectOwner(privateObjectOwner);

		Create(ObjectGuid.Create(HighGuid.SceneObject, Location.MapId, sceneId, lowGuid));
		PhasingHandler.InheritPhaseShift(this, creator);

		SetEntry(scriptPackageId);
		SetObjectScale(1.0f);

		SetUpdateFieldValue(Values.ModifyValue(_sceneObjectData).ModifyValue(_sceneObjectData.ScriptPackageID), (int)scriptPackageId);
		SetUpdateFieldValue(Values.ModifyValue(_sceneObjectData).ModifyValue(_sceneObjectData.RndSeedVal), GameTime.GetGameTimeMS());
		SetUpdateFieldValue(Values.ModifyValue(_sceneObjectData).ModifyValue(_sceneObjectData.CreatedBy), creator.GetGUID());
		SetUpdateFieldValue(Values.ModifyValue(_sceneObjectData).ModifyValue(_sceneObjectData.SceneType), (uint)type);

		if (!GetMap().AddToMap(this))
			return false;

		return true;
	}

	void BuildValuesUpdateForPlayerWithMask(UpdateData data, UpdateMask requestedObjectMask, UpdateMask requestedSceneObjectMask, Player target)
	{
		UpdateMask valuesMask = new((int)TypeId.Max);

		if (requestedObjectMask.IsAnySet())
			valuesMask.Set((int)TypeId.Object);

		if (requestedSceneObjectMask.IsAnySet())
			valuesMask.Set((int)TypeId.SceneObject);

		WorldPacket buffer = new();
		buffer.WriteUInt32(valuesMask.GetBlock(0));

		if (valuesMask[(int)TypeId.Object])
			ObjectData.WriteUpdate(buffer, requestedObjectMask, true, this, target);

		if (valuesMask[(int)TypeId.SceneObject])
			_sceneObjectData.WriteUpdate(buffer, requestedSceneObjectMask, true, this, target);

		WorldPacket buffer1 = new();
		buffer1.WriteUInt8((byte)UpdateType.Values);
		buffer1.WritePackedGuid(GetGUID());
		buffer1.WriteUInt32(buffer.GetSize());
		buffer1.WriteBytes(buffer.GetData());

		data.AddUpdateBlock(buffer1);
	}

	void RelocateStationaryPosition(Position pos)
	{
		_stationaryPosition.Relocate(pos);
	}

	class ValuesUpdateForPlayerWithMaskSender : IDoWork<Player>
	{
		readonly SceneObject _owner;
		readonly ObjectFieldData _objectMask = new();
		readonly SceneObjectData _sceneObjectData = new();

		public ValuesUpdateForPlayerWithMaskSender(SceneObject owner)
		{
			_owner = owner;
		}

		public void Invoke(Player player)
		{
			UpdateData udata = new(_owner.Location.MapId);

			_owner.BuildValuesUpdateForPlayerWithMask(udata, _objectMask.GetUpdateMask(), _sceneObjectData.GetUpdateMask(), player);

			udata.BuildPacket(out var packet);
			player.SendPacket(packet);
		}
	}
}