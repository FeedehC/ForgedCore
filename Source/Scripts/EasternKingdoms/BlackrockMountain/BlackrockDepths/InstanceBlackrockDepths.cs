// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.Entities;
using Game.Maps;
using Game.Scripting.BaseScripts;
using Game.Scripting.Interfaces.IMap;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockDepths;

internal struct CreatureIds
{
	public const uint Emperor = 9019;
	public const uint Phalanx = 9502;
	public const uint Angerrel = 9035;
	public const uint Doperel = 9040;
	public const uint Haterel = 9034;
	public const uint Vilerel = 9036;
	public const uint Seethrel = 9038;
	public const uint Gloomrel = 9037;
	public const uint Doomrel = 9039;
	public const uint Magmus = 9938;
	public const uint Moira = 8929;
	public const uint Coren = 23872;
}

internal struct GameObjectIds
{
	public const uint Arena1 = 161525;
	public const uint Arena2 = 161522;
	public const uint Arena3 = 161524;
	public const uint Arena4 = 161523;
	public const uint ShadowLock = 161460;
	public const uint ShadowMechanism = 161461;
	public const uint ShadowGiantDoor = 157923;
	public const uint ShadowDummy = 161516;
	public const uint BarKegShot = 170607;
	public const uint BarKegTrap = 171941;
	public const uint BarDoor = 170571;
	public const uint TombEnter = 170576;
	public const uint TombExit = 170577;
	public const uint Lyceum = 170558;
	public const uint SfN = 174745;        // Shadowforge Brazier North
	public const uint SfS = 174744;        // Shadowforge Brazier South
	public const uint GolemRoomN = 170573; // Magmus door North
	public const uint GolemRoomS = 170574; // Magmus door Soutsh
	public const uint ThroneRoom = 170575; // Throne door
	public const uint SpectralChalice = 164869;
	public const uint ChestSeven = 169243;
}

internal struct DataTypes
{
	public const uint TypeRingOfLaw = 1;
	public const uint TypeVault = 2;
	public const uint TypeBar = 3;
	public const uint TypeTombOfSeven = 4;
	public const uint TypeLyceum = 5;
	public const uint TypeIronHall = 6;

	public const uint DataEmperor = 10;
	public const uint DataPhalanx = 11;

	public const uint DataArena1 = 12;
	public const uint DataArena2 = 13;
	public const uint DataArena3 = 14;
	public const uint DataArena4 = 15;

	public const uint DataGoBarKeg = 16;
	public const uint DataGoBarKegTrap = 17;
	public const uint DataGoBarDoor = 18;
	public const uint DataGoChalice = 19;

	public const uint DataGhostkill = 20;
	public const uint DataEvenstarter = 21;

	public const uint DataGolemDoorN = 22;
	public const uint DataGolemDoorS = 23;

	public const uint DataThroneDoor = 24;

	public const uint DataSfBrazierN = 25;
	public const uint DataSfBrazierS = 26;
	public const uint DataMoira = 27;
	public const uint DataCoren = 28;
}

internal struct MiscConst
{
	public const uint TimerTombOfTheSeven = 15000;
	public const uint MaxEncounter = 6;
	public const uint TombOfSevenBossNum = 7;
}

internal class instance_blackrock_depths : InstanceMapScript, IInstanceMapGetInstanceScript
{
	public instance_blackrock_depths() : base(nameof(instance_blackrock_depths), 230) { }

	public InstanceScript GetInstanceScript(InstanceMap map)
	{
		return new instance_blackrock_depths_InstanceMapScript(map);
	}

	private class instance_blackrock_depths_InstanceMapScript : InstanceScript
	{
		private readonly ObjectGuid[] TombBossGUIDs = new ObjectGuid[MiscConst.TombOfSevenBossNum];
		private uint BarAleCount;
		private ObjectGuid CorenGUID;
		private ObjectGuid EmperorGUID;
		private uint GhostKillCount;

		private ObjectGuid GoArena1GUID;
		private ObjectGuid GoArena2GUID;
		private ObjectGuid GoArena3GUID;
		private ObjectGuid GoArena4GUID;
		private ObjectGuid GoBarDoorGUID;
		private ObjectGuid GoBarKegGUID;
		private ObjectGuid GoBarKegTrapGUID;
		private ObjectGuid GoChestGUID;
		private ObjectGuid GoGolemNGUID;
		private ObjectGuid GoGolemSGUID;
		private ObjectGuid GoLyceumGUID;
		private ObjectGuid GoSFNGUID;
		private ObjectGuid GoSFSGUID;
		private ObjectGuid GoShadowDummyGUID;
		private ObjectGuid GoShadowGiantGUID;
		private ObjectGuid GoShadowLockGUID;
		private ObjectGuid GoShadowMechGUID;
		private ObjectGuid GoSpectralChaliceGUID;
		private ObjectGuid GoThroneGUID;
		private ObjectGuid GoTombEnterGUID;
		private ObjectGuid GoTombExitGUID;
		private ObjectGuid MagmusGUID;
		private ObjectGuid MoiraGUID;
		private ObjectGuid PhalanxGUID;
		private uint TombEventCounter;
		private ObjectGuid TombEventStarterGUID;
		private uint TombTimer;

		public instance_blackrock_depths_InstanceMapScript(InstanceMap map) : base(map)
		{
			SetHeaders("BRD");
			SetBossNumber(MiscConst.MaxEncounter);

			BarAleCount = 0;
			GhostKillCount = 0;
			TombTimer = MiscConst.TimerTombOfTheSeven;
			TombEventCounter = 0;
		}

		public override void OnCreatureCreate(Creature creature)
		{
			switch (creature.Entry)
			{
				case CreatureIds.Emperor:
					EmperorGUID = creature.GUID;

					break;
				case CreatureIds.Phalanx:
					PhalanxGUID = creature.GUID;

					break;
				case CreatureIds.Moira:
					MoiraGUID = creature.GUID;

					break;
				case CreatureIds.Coren:
					CorenGUID = creature.GUID;

					break;
				case CreatureIds.Doomrel:
					TombBossGUIDs[0] = creature.GUID;

					break;
				case CreatureIds.Doperel:
					TombBossGUIDs[1] = creature.GUID;

					break;
				case CreatureIds.Haterel:
					TombBossGUIDs[2] = creature.GUID;

					break;
				case CreatureIds.Vilerel:
					TombBossGUIDs[3] = creature.GUID;

					break;
				case CreatureIds.Seethrel:
					TombBossGUIDs[4] = creature.GUID;

					break;
				case CreatureIds.Gloomrel:
					TombBossGUIDs[5] = creature.GUID;

					break;
				case CreatureIds.Angerrel:
					TombBossGUIDs[6] = creature.GUID;

					break;
				case CreatureIds.Magmus:
					MagmusGUID = creature.GUID;

					if (!creature.IsAlive)
						HandleGameObject(GetGuidData(DataTypes.DataThroneDoor), true); // if Magmus is dead open door to last boss

					break;
			}
		}

		public override void OnGameObjectCreate(GameObject go)
		{
			switch (go.Entry)
			{
				case GameObjectIds.Arena1:
					GoArena1GUID = go.GUID;

					break;
				case GameObjectIds.Arena2:
					GoArena2GUID = go.GUID;

					break;
				case GameObjectIds.Arena3:
					GoArena3GUID = go.GUID;

					break;
				case GameObjectIds.Arena4:
					GoArena4GUID = go.GUID;

					break;
				case GameObjectIds.ShadowLock:
					GoShadowLockGUID = go.GUID;

					break;
				case GameObjectIds.ShadowMechanism:
					GoShadowMechGUID = go.GUID;

					break;
				case GameObjectIds.ShadowGiantDoor:
					GoShadowGiantGUID = go.GUID;

					break;
				case GameObjectIds.ShadowDummy:
					GoShadowDummyGUID = go.GUID;

					break;
				case GameObjectIds.BarKegShot:
					GoBarKegGUID = go.GUID;

					break;
				case GameObjectIds.BarKegTrap:
					GoBarKegTrapGUID = go.GUID;

					break;
				case GameObjectIds.BarDoor:
					GoBarDoorGUID = go.GUID;

					break;
				case GameObjectIds.TombEnter:
					GoTombEnterGUID = go.GUID;

					break;
				case GameObjectIds.TombExit:
					GoTombExitGUID = go.GUID;

					if (GhostKillCount >= MiscConst.TombOfSevenBossNum)
						HandleGameObject(ObjectGuid.Empty, true, go);
					else
						HandleGameObject(ObjectGuid.Empty, false, go);

					break;
				case GameObjectIds.Lyceum:
					GoLyceumGUID = go.GUID;

					break;
				case GameObjectIds.SfS:
					GoSFSGUID = go.GUID;

					break;
				case GameObjectIds.SfN:
					GoSFNGUID = go.GUID;

					break;
				case GameObjectIds.GolemRoomN:
					GoGolemNGUID = go.GUID;

					break;
				case GameObjectIds.GolemRoomS:
					GoGolemSGUID = go.GUID;

					break;
				case GameObjectIds.ThroneRoom:
					GoThroneGUID = go.GUID;

					break;
				case GameObjectIds.ChestSeven:
					GoChestGUID = go.GUID;

					break;
				case GameObjectIds.SpectralChalice:
					GoSpectralChaliceGUID = go.GUID;

					break;
			}
		}

		public override void SetGuidData(uint type, ObjectGuid data)
		{
			switch (type)
			{
				case DataTypes.DataEvenstarter:
					TombEventStarterGUID = data;

					if (TombEventStarterGUID.IsEmpty)
						TombOfSevenReset(); //reset
					else
						TombOfSevenStart(); //start

					break;
			}
		}

		public override void SetData(uint type, uint data)
		{
			switch (type)
			{
				case DataTypes.TypeRingOfLaw:
					SetBossState(0, (EncounterState)data);

					break;
				case DataTypes.TypeVault:
					SetBossState(1, (EncounterState)data);

					break;
				case DataTypes.TypeBar:
					if (data == (uint)EncounterState.Special)
						++BarAleCount;
					else
						SetBossState(2, (EncounterState)data);

					break;
				case DataTypes.TypeTombOfSeven:
					SetBossState(3, (EncounterState)data);

					break;
				case DataTypes.TypeLyceum:
					SetBossState(4, (EncounterState)data);

					break;
				case DataTypes.TypeIronHall:
					SetBossState(5, (EncounterState)data);

					break;
				case DataTypes.DataGhostkill:
					GhostKillCount += data;

					break;
			}
		}

		public override uint GetData(uint type)
		{
			switch (type)
			{
				case DataTypes.TypeRingOfLaw:
					return (uint)GetBossState(0);
				case DataTypes.TypeVault:
					return (uint)GetBossState(1);
				case DataTypes.TypeBar:
					if (GetBossState(2) == EncounterState.InProgress &&
						BarAleCount == 3)
						return (uint)EncounterState.Special;
					else
						return (uint)GetBossState(2);
				case DataTypes.TypeTombOfSeven:
					return (uint)GetBossState(3);
				case DataTypes.TypeLyceum:
					return (uint)GetBossState(4);
				case DataTypes.TypeIronHall:
					return (uint)GetBossState(5);
				case DataTypes.DataGhostkill:
					return GhostKillCount;
			}

			return 0;
		}

		public override ObjectGuid GetGuidData(uint data)
		{
			switch (data)
			{
				case DataTypes.DataEmperor:
					return EmperorGUID;
				case DataTypes.DataPhalanx:
					return PhalanxGUID;
				case DataTypes.DataMoira:
					return MoiraGUID;
				case DataTypes.DataCoren:
					return CorenGUID;
				case DataTypes.DataArena1:
					return GoArena1GUID;
				case DataTypes.DataArena2:
					return GoArena2GUID;
				case DataTypes.DataArena3:
					return GoArena3GUID;
				case DataTypes.DataArena4:
					return GoArena4GUID;
				case DataTypes.DataGoBarKeg:
					return GoBarKegGUID;
				case DataTypes.DataGoBarKegTrap:
					return GoBarKegTrapGUID;
				case DataTypes.DataGoBarDoor:
					return GoBarDoorGUID;
				case DataTypes.DataEvenstarter:
					return TombEventStarterGUID;
				case DataTypes.DataSfBrazierN:
					return GoSFNGUID;
				case DataTypes.DataSfBrazierS:
					return GoSFSGUID;
				case DataTypes.DataThroneDoor:
					return GoThroneGUID;
				case DataTypes.DataGolemDoorN:
					return GoGolemNGUID;
				case DataTypes.DataGolemDoorS:
					return GoGolemSGUID;
				case DataTypes.DataGoChalice:
					return GoSpectralChaliceGUID;
			}

			return ObjectGuid.Empty;
		}

		public override void Update(uint diff)
		{
			if (!TombEventStarterGUID.IsEmpty &&
				GhostKillCount < MiscConst.TombOfSevenBossNum)
			{
				if (TombTimer <= diff)
				{
					TombTimer = MiscConst.TimerTombOfTheSeven;

					if (TombEventCounter < MiscConst.TombOfSevenBossNum)
					{
						TombOfSevenEvent();
						++TombEventCounter;
					}

					// Check Killed bosses
					for (byte i = 0; i < MiscConst.TombOfSevenBossNum; ++i)
					{
						var boss = Instance.GetCreature(TombBossGUIDs[i]);

						if (boss)
							if (!boss.IsAlive)
								GhostKillCount = i + 1u;
					}
				}
				else
				{
					TombTimer -= diff;
				}
			}

			if (GhostKillCount >= MiscConst.TombOfSevenBossNum &&
				!TombEventStarterGUID.IsEmpty)
				TombOfSevenEnd();
		}

		private void TombOfSevenEvent()
		{
			if (GhostKillCount < MiscConst.TombOfSevenBossNum &&
				!TombBossGUIDs[TombEventCounter].IsEmpty)
			{
				var boss = Instance.GetCreature(TombBossGUIDs[TombEventCounter]);

				if (boss)
				{
					boss.Faction = (uint)FactionTemplates.DarkIronDwarves;
					boss.SetImmuneToPC(false);
					var target = boss.SelectNearestTarget(500);

					if (target)
						boss.AI.AttackStart(target);
				}
			}
		}

		private void TombOfSevenReset()
		{
			HandleGameObject(GoTombExitGUID, false); //event reseted, close exit door
			HandleGameObject(GoTombEnterGUID, true); //event reseted, open entrance door

			for (byte i = 0; i < MiscConst.TombOfSevenBossNum; ++i)
			{
				var boss = Instance.GetCreature(TombBossGUIDs[i]);

				if (boss)
				{
					if (!boss.IsAlive)
						boss.Respawn();
					else
						boss.Faction = (uint)FactionTemplates.Friendly;
				}
			}

			GhostKillCount = 0;
			TombEventStarterGUID.Clear();
			TombEventCounter = 0;
			TombTimer = MiscConst.TimerTombOfTheSeven;
			SetData(DataTypes.TypeTombOfSeven, (uint)EncounterState.NotStarted);
		}

		private void TombOfSevenStart()
		{
			HandleGameObject(GoTombExitGUID, false);  //event started, close exit door
			HandleGameObject(GoTombEnterGUID, false); //event started, close entrance door
			SetData(DataTypes.TypeTombOfSeven, (uint)EncounterState.InProgress);
		}

		private void TombOfSevenEnd()
		{
			DoRespawnGameObject(GoChestGUID, TimeSpan.FromHours(24));
			HandleGameObject(GoTombExitGUID, true);  //event done, open exit door
			HandleGameObject(GoTombEnterGUID, true); //event done, open entrance door
			TombEventStarterGUID.Clear();
			SetData(DataTypes.TypeTombOfSeven, (uint)EncounterState.Done);
		}
	}
}