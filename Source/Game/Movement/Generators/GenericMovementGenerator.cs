﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.Entities;

namespace Game.Movement;

class GenericMovementGenerator : MovementGenerator
{
	readonly Action<MoveSplineInit> _splineInit;
	readonly MovementGeneratorType _type;
	readonly uint _pointId;
	readonly TimeTracker _duration;
	readonly uint _arrivalSpellId;
	readonly ObjectGuid _arrivalSpellTargetGuid;

	public GenericMovementGenerator(Action<MoveSplineInit> initializer, MovementGeneratorType type, uint id, uint arrivalSpellId = 0, ObjectGuid arrivalSpellTargetGuid = default)
	{
		_splineInit = initializer;
		_type = type;
		_pointId = id;
		_duration = new TimeTracker();
		_arrivalSpellId = arrivalSpellId;
		_arrivalSpellTargetGuid = arrivalSpellTargetGuid;

		Mode = MovementGeneratorMode.Default;
		Priority = MovementGeneratorPriority.Normal;
		Flags = MovementGeneratorFlags.InitializationPending;
		BaseUnitState = UnitState.Roaming;
	}

	public override void Initialize(Unit owner)
	{
		if (HasFlag(MovementGeneratorFlags.Deactivated) && !HasFlag(MovementGeneratorFlags.InitializationPending)) // Resume spline is not supported
		{
			RemoveFlag(MovementGeneratorFlags.Deactivated);
			AddFlag(MovementGeneratorFlags.Finalized);

			return;
		}

		RemoveFlag(MovementGeneratorFlags.InitializationPending | MovementGeneratorFlags.Deactivated);
		AddFlag(MovementGeneratorFlags.Initialized);

		MoveSplineInit init = new(owner);
		_splineInit(init);
		_duration.Reset((uint)init.Launch());
	}

	public override void Reset(Unit owner)
	{
		Initialize(owner);
	}

	public override bool Update(Unit owner, uint diff)
	{
		if (!owner || HasFlag(MovementGeneratorFlags.Finalized))
			return false;

		// Cyclic splines never expire, so update the duration only if it's not cyclic
		if (!owner.MoveSpline.IsCyclic())
			_duration.Update(diff);

		if (_duration.Passed || owner.MoveSpline.Finalized())
		{
			AddFlag(MovementGeneratorFlags.InformEnabled);

			return false;
		}

		return true;
	}

	public override void Deactivate(Unit owner)
	{
		AddFlag(MovementGeneratorFlags.Deactivated);
	}

	public override void Finalize(Unit owner, bool active, bool movementInform)
	{
		AddFlag(MovementGeneratorFlags.Finalized);

		if (movementInform && HasFlag(MovementGeneratorFlags.InformEnabled))
			MovementInform(owner);
	}

	public override MovementGeneratorType GetMovementGeneratorType()
	{
		return _type;
	}

	void MovementInform(Unit owner)
	{
		if (_arrivalSpellId != 0)
			owner.CastSpell(Global.ObjAccessor.GetUnit(owner, _arrivalSpellTargetGuid), _arrivalSpellId, true);

		var creature = owner.AsCreature;

		if (creature != null && creature.AI != null)
			creature.AI.MovementInform(_type, _pointId);
	}
}