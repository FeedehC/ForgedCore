// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Entities;
using Game.Maps;
using Game.Scripting.BaseScripts;
using Game.Scripting.Interfaces.IMap;

namespace Scripts.EasternKingdoms.Gnomeregan;

internal struct GNOGameObjectIds
{
	public const uint CaveInLeft = 146085;
	public const uint CaveInRight = 146086;
	public const uint RedRocket = 103820;
}

internal struct GNOCreatureIds
{
	public const uint BlastmasterEmiShortfuse = 7998;
	public const uint CaverndeepAmbusher = 6207;
	public const uint Grubbis = 7361;
	public const uint ViciousFallout = 7079;
	public const uint Chomper = 6215;
	public const uint Electrocutioner = 6235;
	public const uint CrowdPummeler = 6229;
	public const uint Mekgineer = 7800;
}

internal struct DataTypes
{
	public const uint BlastmasterEvent = 0;
	public const uint ViciousFallout = 1;
	public const uint Electrocutioner = 2;
	public const uint CrowdPummeler = 3;
	public const uint Thermaplugg = 4;

	public const uint MaxEncounter = 5;

	// Additional Objects
	public const uint GoCaveInLeft = 6;
	public const uint GoCaveInRight = 7;
	public const uint NpcBastmasterEmiShortfuse = 8;
}

internal struct DataTypes64
{
	public const uint GoCaveInLeft = 0;
	public const uint GoCaveInRight = 1;
	public const uint NpcBastmasterEmiShortfuse = 2;
}

internal class instance_gnomeregan : InstanceMapScript, IInstanceMapGetInstanceScript
{
	public instance_gnomeregan() : base(nameof(instance_gnomeregan), 90) { }

	public InstanceScript GetInstanceScript(InstanceMap map)
	{
		return new instance_gnomeregan_InstanceMapScript(map);
	}

	private class instance_gnomeregan_InstanceMapScript : InstanceScript
	{
		private ObjectGuid uiBlastmasterEmiShortfuseGUID;
		private ObjectGuid uiCaveInLeftGUID;
		private ObjectGuid uiCaveInRightGUID;

		public instance_gnomeregan_InstanceMapScript(InstanceMap map) : base(map)
		{
			SetHeaders("GNO");
			SetBossNumber(DataTypes.MaxEncounter);
		}

		public override void OnCreatureCreate(Creature creature)
		{
			switch (creature.Entry)
			{
				case GNOCreatureIds.BlastmasterEmiShortfuse:
					uiBlastmasterEmiShortfuseGUID = creature.GUID;

					break;
			}
		}

		public override void OnGameObjectCreate(GameObject go)
		{
			switch (go.Entry)
			{
				case DataTypes64.GoCaveInLeft:
					uiCaveInLeftGUID = go.GUID;

					break;
				case DataTypes64.GoCaveInRight:
					uiCaveInRightGUID = go.GUID;

					break;
			}
		}

		public override void OnUnitDeath(Unit unit)
		{
			var creature = unit.AsCreature;

			if (creature)
				switch (creature.Entry)
				{
					case GNOCreatureIds.ViciousFallout:
						SetBossState(DataTypes.ViciousFallout, EncounterState.Done);

						break;
					case GNOCreatureIds.Electrocutioner:
						SetBossState(DataTypes.Electrocutioner, EncounterState.Done);

						break;
					case GNOCreatureIds.CrowdPummeler:
						SetBossState(DataTypes.CrowdPummeler, EncounterState.Done);

						break;
					case GNOCreatureIds.Mekgineer:
						SetBossState(DataTypes.Thermaplugg, EncounterState.Done);

						break;
				}
		}

		public override ObjectGuid GetGuidData(uint uiType)
		{
			switch (uiType)
			{
				case DataTypes64.GoCaveInLeft:              return uiCaveInLeftGUID;
				case DataTypes64.GoCaveInRight:             return uiCaveInRightGUID;
				case DataTypes64.NpcBastmasterEmiShortfuse: return uiBlastmasterEmiShortfuseGUID;
			}

			return ObjectGuid.Empty;
		}
	}
}