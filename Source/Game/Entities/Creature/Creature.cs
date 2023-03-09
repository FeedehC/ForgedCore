﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Configuration;
using Framework.Constants;
using Framework.Database;
using Game.AI;
using Game.DataStorage;
using Game.Loots;
using Game.Maps;
using Game.Maps.Grids;
using Game.Networking.Packets;
using Game.Spells;

namespace Game.Entities;

public partial class Creature : Unit
{
	public bool IsReturningHome
	{
		get
		{
			if (MotionMaster.GetCurrentMovementGeneratorType() == MovementGeneratorType.Home)
				return true;

			return false;
		}
	}

	public bool IsFormationLeader
	{
		get
		{
			if (_creatureGroup == null)
				return false;

			return _creatureGroup.IsLeader(this);
		}
	}

	public bool IsFormationLeaderMoveAllowed
	{
		get
		{
			if (_creatureGroup == null)
				return false;

			return _creatureGroup.CanLeaderStartMoving();
		}
	}

	public bool CanGiveExperience => !IsCritter && !IsPet && !IsTotem && !CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.NoXP);

	public override bool IsEngaged
	{
		get
		{
			var ai = AI;

			if (ai != null)
				return ai.IsEngaged;

			return false;
		}
	}

	public bool IsEscorted
	{
		get
		{
			var ai = AI;

			if (ai != null)
				return ai.IsEscorted();

			return false;
		}
	}

	public bool CanGeneratePickPocketLoot => _pickpocketLootRestore <= GameTime.GetGameTime();

	public bool IsFullyLooted
	{
		get
		{
			if (Loot != null && !Loot.IsLooted())
				return false;

			foreach (var (_, loot) in PersonalLoot)
				if (!loot.IsLooted())
					return false;

			return true;
		}
	}

	public HashSet<ObjectGuid> TapList => _tapList;

	public bool HasLootRecipient => !_tapList.Empty();

	public bool IsElite
	{
		get
		{
			if (IsPet)
				return false;

			return CreatureTemplate.Rank != CreatureEliteType.Elite && CreatureTemplate.Rank != CreatureEliteType.RareElite;
		}
	}

	public bool IsWorldBoss
	{
		get
		{
			if (IsPet)
				return false;

			return Convert.ToBoolean(CreatureTemplate.TypeFlags & CreatureTypeFlags.BossMob);
		}
	}

	public long RespawnTimeEx
	{
		get
		{
			var now = GameTime.GetGameTime();

			if (_respawnTime > now)
				return _respawnTime;
			else
				return now;
		}
	}

	public Position RespawnPosition => GetRespawnPosition(out _);

	public CreatureMovementData MovementTemplate
	{
		get
		{
			if (Global.ObjectMgr.TryGetGetCreatureMovementOverride(SpawnId, out var movementOverride))
				return movementOverride;

			return CreatureTemplate.Movement;
		}
	}

	public override bool CanSwim
	{
		get
		{
			if (base.CanSwim)
				return true;

			if (IsPet)
				return true;

			return false;
		}
	}

	public override bool CanEnterWater
	{
		get
		{
			if (CanSwim)
				return true;

			return MovementTemplate.IsSwimAllowed();
		}
	}

	public bool HasCanSwimFlagOutOfCombat => !_isMissingCanSwimFlagOutOfCombat;

	public bool HasScalableLevels => UnitData.ContentTuningID != 0;

	public string[] StringIds => _stringIds;

	public VendorItemData VendorItems => Global.ObjectMgr.GetNpcVendorItemList(Entry);

	public virtual byte PetAutoSpellSize => 4;

    public override float NativeObjectScale => CreatureTemplate.Scale;

	public uint CorpseDelay => _corpseDelay;

	public bool IsRacialLeader => CreatureTemplate.RacialLeader;

	public bool IsCivilian => CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.Civilian);

	public bool IsTrigger => CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.Trigger);

	public bool IsGuard => CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.Guard);

	public bool CanWalk => MovementTemplate.IsGroundAllowed();

	public override bool CanFly => MovementTemplate.IsFlightAllowed() || IsFlying;

	public bool IsDungeonBoss => (CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.DungeonBoss));

	public override bool IsAffectedByDiminishingReturns => base.IsAffectedByDiminishingReturns || CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.AllDiminish);

	public ReactStates ReactState
	{
		get => _reactState;
		set => _reactState = value;
	}

	public bool IsInEvadeMode => HasUnitState(UnitState.Evade);

	public bool IsEvadingAttacks => IsInEvadeMode || CanNotReachTarget;

	public sbyte OriginalEquipmentId => _originalEquipmentId;

	public byte CurrentEquipmentId
	{
		get => _equipmentId;
		set => _equipmentId = value;
	}

	public CreatureTemplate CreatureTemplate => _creatureInfo;

	public CreatureData CreatureData => _creatureData;

	public bool IsReputationGainDisabled => _reputationGain;

	CreatureAddon CreatureAddon
	{
		get
		{
			if (SpawnId != 0)
			{
				var addon = Global.ObjectMgr.GetCreatureAddon(SpawnId);

				if (addon != null)
					return addon;
			}

			// dependent from difficulty mode entry
			return Global.ObjectMgr.GetCreatureTemplateAddon(CreatureTemplate.Entry);
		}
	}

	bool IsSpawnedOnTransport => _creatureData != null && _creatureData.MapId != Location.MapId;

	bool CanNotReachTarget => _cannotReachTarget;

	public bool CanHover => MovementTemplate.Ground == CreatureGroundMovementType.Hover || IsHovering;

	// (secs) interval at which the creature pulses the entire zone into combat (only works in dungeons)
	public uint CombatPulseDelay
	{
		get => _combatPulseDelay;
		set
		{
			_combatPulseDelay = value;

			if (_combatPulseTime == 0 || _combatPulseTime > value)
				_combatPulseTime = value;
		}
	}

	// Part of Evade mechanics
	public long LastDamagedTime
	{
		get => _lastDamagedTime;
		set => _lastDamagedTime = value;
	}

	public bool HasSearchedAssistance => _alreadySearchedAssistance;

	public bool CanIgnoreFeignDeath => CreatureTemplate.FlagsExtra.HasFlag(CreatureFlagsExtra.IgnoreFeighDeath);

	public long RespawnTime => _respawnTime;

	public uint RespawnDelay
	{
		get => _respawnDelay;
		set => _respawnDelay = value;
	}

	public float WanderDistance
	{
		get => _wanderDistance;
		set => _wanderDistance = value;
	}

	public bool CanRegenerateHealth => !_regenerateHealthLock && _regenerateHealth;

	public Position HomePosition
	{
		get => _homePosition;
		set => _homePosition.Relocate(value);
	}

	public Position TransportHomePosition
	{
		get => _transportHomePosition;
		set => _transportHomePosition.Relocate(value);
	}

	public uint WaypointPath => _waypointPathId;

	public (uint nodeId, uint pathId) CurrentWaypointInfo => _currentWaypointNodeInfo;

	public CreatureGroup Formation
	{
		get => _creatureGroup;
		set => _creatureGroup = value;
	}

	public uint OriginalEntry1
	{
		get => OriginalEntry;
		private set => OriginalEntry = value;
	}

	// There's many places not ready for dynamic spawns. This allows them to live on for now.
	public bool RespawnCompatibilityMode
	{
		get => _respawnCompatibilityMode;
		private set => _respawnCompatibilityMode = value;
	}

	public Creature() : this(false) { }

	public Creature(bool worldObject) : base(worldObject)
	{
		_respawnDelay = 300;
		_corpseDelay = 60;
		_boundaryCheckTime = 2500;
		_reactState = ReactStates.Aggressive;
		DefaultMovementType = MovementGeneratorType.Idle;
		_regenerateHealth = true;
		_meleeDamageSchoolMask = SpellSchoolMask.Normal;
		_triggerJustAppeared = true;

		RegenTimer = SharedConst.CreatureRegenInterval;

		SightDistance = SharedConst.SightRangeUnit;

		ResetLootMode(); // restore default loot mode

		_homePosition = new WorldLocation();

		_currentWaypointNodeInfo = new ValueTuple<uint, uint>();
	}

	public override void AddToWorld()
	{
		// Register the creature for guid lookup
		if (!IsInWorld)
		{
			Map.			ObjectsStore.Add(GUID, this);

			if (SpawnId != 0)
				Map.				CreatureBySpawnIdStore.Add(SpawnId, this);

			base.AddToWorld();
			SearchFormation();
			InitializeAI();

			if (IsVehicle)
				VehicleKit1.Install();

			if (ZoneScript != null)
				ZoneScript.OnCreatureCreate(this);
		}
	}

	public override void RemoveFromWorld()
	{
		if (IsInWorld)
		{
			if (ZoneScript != null)
				ZoneScript.OnCreatureRemove(this);

			if (_creatureGroup != null)
				FormationMgr.RemoveCreatureFromGroup(_creatureGroup, this);

			base.RemoveFromWorld();

			if (SpawnId != 0)
				Map.				CreatureBySpawnIdStore.Remove(SpawnId, this);

			Map.
			ObjectsStore.Remove(GUID);
		}
	}

	public void DisappearAndDie()
	{
		ForcedDespawn(0);
	}

	public void SearchFormation()
	{
		if (IsSummon)
			return;

		var lowguid = SpawnId;

		if (lowguid == 0)
			return;

		var formationInfo = FormationMgr.GetFormationInfo(lowguid);

		if (formationInfo != null)
			FormationMgr.AddCreatureToGroup(formationInfo.LeaderSpawnId, this);
	}

	public void SignalFormationMovement()
	{
		if (_creatureGroup == null)
			return;

		if (!_creatureGroup.IsLeader(this))
			return;

		_creatureGroup.LeaderStartedMoving();
	}

	public void RemoveCorpse(bool setSpawnTime = true, bool destroyForNearbyPlayers = true)
	{
		if (DeathState != DeathState.Corpse)
			return;

		if (_respawnCompatibilityMode)
		{
			CorpseRemoveTime = GameTime.GetGameTime();
			SetDeathState(DeathState.Dead);
			RemoveAllAuras();
			//DestroyForNearbyPlayers(); // old UpdateObjectVisibility()
			Loot = null;
			var respawnDelay = _respawnDelay;
			var ai = AI;

			if (ai != null)
				ai.CorpseRemoved(respawnDelay);

			if (destroyForNearbyPlayers)
				UpdateObjectVisibilityOnDestroy();

			// Should get removed later, just keep "compatibility" with scripts
			if (setSpawnTime)
				_respawnTime = Math.Max(GameTime.GetGameTime() + respawnDelay, _respawnTime);

			// if corpse was removed during falling, the falling will continue and override relocation to respawn position
			if (IsFalling)
				StopMoving();

			var respawn = RespawnPosition;

			// We were spawned on transport, calculate real position
			if (IsSpawnedOnTransport)
			{
				MovementInfo.Transport.Pos.Relocate(respawn);

				var transport = DirectTransport;

				if (transport != null)
					transport.CalculatePassengerPosition(respawn);
			}

			respawn.Z = UpdateAllowedPositionZ(respawn.X, respawn.Y, respawn.Z);
			HomePosition = respawn;
			Map.CreatureRelocation(this, respawn);
		}
		else
		{
			var ai = AI;

			if (ai != null)
				ai.CorpseRemoved(_respawnDelay);

			// In case this is called directly and normal respawn timer not set
			// Since this timer will be longer than the already present time it
			// will be ignored if the correct place added a respawn timer
			if (setSpawnTime)
			{
				var respawnDelay = _respawnDelay;
				_respawnTime = Math.Max(GameTime.GetGameTime() + respawnDelay, _respawnTime);

				SaveRespawnTime();
			}

			var summon = ToTempSummon();

			if (summon != null)
				summon.UnSummon();
			else
				AddObjectToRemoveList();
		}
	}

	public bool InitEntry(uint entry, CreatureData data = null)
	{
		var normalInfo = Global.ObjectMgr.GetCreatureTemplate(entry);

		if (normalInfo == null)
		{
			Log.outError(LogFilter.Sql, "Creature.InitEntry creature entry {0} does not exist.", entry);

			return false;
		}

		// get difficulty 1 mode entry
		CreatureTemplate cInfo = null;
		var difficultyEntry = CliDB.DifficultyStorage.LookupByKey(Map.DifficultyID);

		while (cInfo == null && difficultyEntry != null)
		{
			var idx = CreatureTemplate.DifficultyIDToDifficultyEntryIndex(difficultyEntry.Id);

			if (idx == -1)
				break;

			if (normalInfo.DifficultyEntry[idx] != 0)
			{
				cInfo = Global.ObjectMgr.GetCreatureTemplate(normalInfo.DifficultyEntry[idx]);

				break;
			}

			if (difficultyEntry.FallbackDifficultyID == 0)
				break;

			difficultyEntry = CliDB.DifficultyStorage.LookupByKey(difficultyEntry.FallbackDifficultyID);
		}

		if (cInfo == null)
			cInfo = normalInfo;

		Entry = entry;         // normal entry always
		_creatureInfo = cInfo; // map mode related always

		// equal to player Race field, but creature does not have race
		Race = 0;
		Class = (Class)cInfo.UnitClass;

		// Cancel load if no model defined
		if (cInfo.GetFirstValidModel() == null)
		{
			Log.outError(LogFilter.Sql, "Creature (Entry: {0}) has no model defined in table `creature_template`, can't load. ", entry);

			return false;
		}

		var model = ObjectManager.ChooseDisplayId(cInfo, data);
		var minfo = Global.ObjectMgr.GetCreatureModelRandomGender(ref model, cInfo);

		if (minfo == null) // Cancel load if no model defined
		{
			Log.outError(LogFilter.Sql, "Creature (Entry: {0}) has invalid model {1} defined in table `creature_template`, can't load.", entry, model.CreatureDisplayId);

			return false;
		}

		SetDisplayId(model.CreatureDisplayId, model.DisplayScale);
		SetNativeDisplayId(model.CreatureDisplayId, model.DisplayScale);

		// Load creature equipment
		if (data == null)
		{
			LoadEquipment(); // use default equipment (if available) for summons
		}
		else if (data.EquipmentId == 0)
		{
			LoadEquipment(0); // 0 means no equipment for creature table
		}
		else
		{
			_originalEquipmentId = data.EquipmentId;
			LoadEquipment(data.EquipmentId);
		}

		SetName(normalInfo.Name); // at normal entry always

		SetModCastingSpeed(1.0f);
		SetModSpellHaste(1.0f);
		SetModHaste(1.0f);
		SetModRangedHaste(1.0f);
		SetModHasteRegen(1.0f);
		SetModTimeRate(1.0f);

		SetSpeedRate(UnitMoveType.Walk, cInfo.SpeedWalk);
		SetSpeedRate(UnitMoveType.Run, cInfo.SpeedRun);
		SetSpeedRate(UnitMoveType.Swim, 1.0f);   // using 1.0 rate
		SetSpeedRate(UnitMoveType.Flight, 1.0f); // using 1.0 rate

		ObjectScale = NativeObjectScale;

		SetHoverHeight(cInfo.HoverHeight);

		SetCanDualWield(cInfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.UseOffhandAttack));

		// checked at loading
		DefaultMovementType = (MovementGeneratorType)(data != null ? data.MovementType : cInfo.MovementType);

		if (_wanderDistance == 0 && DefaultMovementType == MovementGeneratorType.Random)
			DefaultMovementType = MovementGeneratorType.Idle;

		for (byte i = 0; i < SharedConst.MaxCreatureSpells; ++i)
			Spells[i] = CreatureTemplate.Spells[i];

		return true;
	}

	public bool UpdateEntry(uint entry, CreatureData data = null, bool updateLevel = true)
	{
		if (!InitEntry(entry, data))
			return false;

		var cInfo = CreatureTemplate;

		_regenerateHealth = cInfo.RegenHealth;

		// creatures always have melee weapon ready if any unless specified otherwise
		if (CreatureAddon == null)
			Sheath = SheathState.Melee;

		Faction = cInfo.Faction;

		ObjectManager.ChooseCreatureFlags(cInfo, out var npcFlags, out var unitFlags, out var unitFlags2, out var unitFlags3, out var dynamicFlags, data);

		if (cInfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.Worldevent))
			npcFlags |= Global.GameEventMgr.GetNPCFlag(this);

		ReplaceAllNpcFlags((NPCFlags)(npcFlags & 0xFFFFFFFF));
		ReplaceAllNpcFlags2((NPCFlags2)(npcFlags >> 32));

		// if unit is in combat, keep this flag
		unitFlags &= ~(uint)UnitFlags.InCombat;

		if (IsInCombat)
			unitFlags |= (uint)UnitFlags.InCombat;

		ReplaceAllUnitFlags((UnitFlags)unitFlags);
		ReplaceAllUnitFlags2((UnitFlags2)unitFlags2);
		ReplaceAllUnitFlags3((UnitFlags3)unitFlags3);

		ReplaceAllDynamicFlags((UnitDynFlags)dynamicFlags);

		SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.StateAnimID), Global.DB2Mgr.GetEmptyAnimStateID());

		SetCanDualWield(cInfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.UseOffhandAttack));

		SetBaseAttackTime(WeaponAttackType.BaseAttack, cInfo.BaseAttackTime);
		SetBaseAttackTime(WeaponAttackType.OffAttack, cInfo.BaseAttackTime);
		SetBaseAttackTime(WeaponAttackType.RangedAttack, cInfo.RangeAttackTime);

		if (updateLevel)
		{
			SelectLevel();
		}
		else if (!IsGuardian)
		{
			var previousHealth = Health;
			UpdateLevelDependantStats(); // We still re-initialize level dependant stats on entry update

			if (previousHealth > 0)
				SetHealth(previousHealth);
		}

		// Do not update guardian stats here - they are handled in Guardian::InitStatsForLevel()
		if (!IsGuardian)
		{
			SetMeleeDamageSchool((SpellSchools)cInfo.DmgSchool);
			SetStatFlatModifier(UnitMods.ResistanceHoly, UnitModifierFlatType.Base, cInfo.Resistance[(int)SpellSchools.Holy]);
			SetStatFlatModifier(UnitMods.ResistanceFire, UnitModifierFlatType.Base, cInfo.Resistance[(int)SpellSchools.Fire]);
			SetStatFlatModifier(UnitMods.ResistanceNature, UnitModifierFlatType.Base, cInfo.Resistance[(int)SpellSchools.Nature]);
			SetStatFlatModifier(UnitMods.ResistanceFrost, UnitModifierFlatType.Base, cInfo.Resistance[(int)SpellSchools.Frost]);
			SetStatFlatModifier(UnitMods.ResistanceShadow, UnitModifierFlatType.Base, cInfo.Resistance[(int)SpellSchools.Shadow]);
			SetStatFlatModifier(UnitMods.ResistanceArcane, UnitModifierFlatType.Base, cInfo.Resistance[(int)SpellSchools.Arcane]);

			SetCanModifyStats(true);
			UpdateAllStats();
		}

		// checked and error show at loading templates
		var factionTemplate = CliDB.FactionTemplateStorage.LookupByKey(cInfo.Faction);

		if (factionTemplate != null)
			SetPvP(factionTemplate.Flags.HasAnyFlag((ushort)FactionTemplateFlags.PVP));

		// updates spell bars for vehicles and set player's faction - should be called here, to overwrite faction that is set from the new template
		if (IsVehicle)
		{
			var owner = CharmerOrOwnerPlayerOrPlayerItself;

			if (owner != null) // this check comes in case we don't have a player
			{
				Faction = owner.Faction; // vehicles should have same as owner faction
				owner.VehicleSpellInitialize();
			}
		}

		// trigger creature is always not selectable and can not be attacked
		if (IsTrigger)
			SetUnitFlag(UnitFlags.Uninteractible);

		InitializeReactState();

		if (Convert.ToBoolean(cInfo.FlagsExtra & CreatureFlagsExtra.NoTaunt))
		{
			ApplySpellImmune(0, SpellImmunity.State, AuraType.ModTaunt, true);
			ApplySpellImmune(0, SpellImmunity.Effect, SpellEffectName.AttackMe, true);
		}

		SetIsCombatDisallowed(cInfo.FlagsExtra.HasFlag(CreatureFlagsExtra.CannotEnterCombat));

		LoadTemplateRoot();
		InitializeMovementFlags();

		LoadCreaturesAddon();

		LoadTemplateImmunities();
		GetThreatManager().EvaluateSuppressed();

		//We must update last scriptId or it looks like we reloaded a script, breaking some things such as gossip temporarily
		LastUsedScriptID = GetScriptId();

		_stringIds[0] = cInfo.StringId;

		return true;
	}

	public override void Update(uint diff)
	{
		if (IsAIEnabled && _triggerJustAppeared && DeathState != DeathState.Dead)
		{
			if (_respawnCompatibilityMode && VehicleKit != null)
				VehicleKit.Reset();

			_triggerJustAppeared = false;
			AI.JustAppeared();
		}

		UpdateMovementFlags();

		switch (DeathState)
		{
			case DeathState.JustRespawned:
			case DeathState.JustDied:
				Log.outError(LogFilter.Unit, $"Creature ({GUID}) in wrong state: {DeathState}");

				break;
			case DeathState.Dead:
			{
				if (!_respawnCompatibilityMode)
				{
					Log.outError(LogFilter.Unit, $"Creature (GUID: {GUID.Counter} Entry: {Entry}) in wrong state: DEAD (3)");

					break;
				}

				var now = GameTime.GetGameTime();

				if (_respawnTime <= now)
				{
					// Delay respawn if spawn group is not active
					if (_creatureData != null && !Map.IsSpawnGroupActive(_creatureData.SpawnGroupData.GroupId))
					{
						_respawnTime = now + RandomHelper.LRand(4, 7);

						break; // Will be rechecked on next Update call after delay expires
					}

					var dbtableHighGuid = ObjectGuid.Create(HighGuid.Creature, Location.MapId, Entry, SpawnId);
					var linkedRespawnTime = Map.GetLinkedRespawnTime(dbtableHighGuid);

					if (linkedRespawnTime == 0) // Can respawn
					{
						Respawn();
					}
					else // the master is dead
					{
						var targetGuid = Global.ObjectMgr.GetLinkedRespawnGuid(dbtableHighGuid);

						if (targetGuid == dbtableHighGuid) // if linking self, never respawn (check delayed to next day)
						{
							SetRespawnTime(Time.Week);
						}
						else
						{
							// else copy time from master and add a little
							var baseRespawnTime = Math.Max(linkedRespawnTime, now);
							var offset = RandomHelper.LRand(5, Time.Minute);

							// linked guid can be a boss, uses std::numeric_limits<time_t>::max to never respawn in that instance
							// we shall inherit it instead of adding and causing an overflow
							if (baseRespawnTime <= long.MaxValue - offset)
								_respawnTime = baseRespawnTime + offset;
							else
								_respawnTime = long.MaxValue;
						}

						SaveRespawnTime(); // also save to DB immediately
					}
				}

				break;
			}
			case DeathState.Corpse:
				base.Update(diff);

				if (DeathState != DeathState.Corpse)
					break;

				if (IsEngaged)
					AIUpdateTick(diff);

				Loot?.Update();

				foreach (var (playerOwner, loot) in PersonalLoot)
					loot.Update();

				if (CorpseRemoveTime <= GameTime.GetGameTime())
				{
					RemoveCorpse(false);
					Log.outDebug(LogFilter.Unit, "Removing corpse... {0} ", Entry);
				}

				break;
			case DeathState.Alive:
				base.Update(diff);

				if (!IsAlive)
					break;

				GetThreatManager().Update(diff);

				if (IsFeared)
					return;

				if (_spellFocusInfo.Delay != 0)
				{
					if (_spellFocusInfo.Delay <= diff)
						ReacquireSpellFocusTarget();
					else
						_spellFocusInfo.Delay -= diff;
				}

				// periodic check to see if the creature has passed an evade boundary
				if (IsAIEnabled && !IsInEvadeMode && IsEngaged)
				{
					if (diff >= _boundaryCheckTime)
					{
						AI.CheckInRoom();
						_boundaryCheckTime = 2500;
					}
					else
					{
						_boundaryCheckTime -= diff;
					}
				}

				// if periodic combat pulse is enabled and we are both in combat and in a dungeon, do this now
				if (_combatPulseDelay > 0 && IsEngaged && Map.IsDungeon)
				{
					if (diff > _combatPulseTime)
						_combatPulseTime = 0;
					else
						_combatPulseTime -= diff;

					if (_combatPulseTime == 0)
					{
						var players = Map.Players;

						foreach (var player in players)
						{
							if (player.IsGameMaster)
								continue;

							if (player.IsAlive && IsHostileTo(player))
								EngageWithTarget(player);
						}

						_combatPulseTime = _combatPulseDelay * Time.InMilliseconds;
					}
				}

				AIUpdateTick(diff);

				if (!IsAlive)
					break;

				if (RegenTimer > 0)
				{
					if (diff >= RegenTimer)
						RegenTimer = 0;
					else
						RegenTimer -= diff;
				}

				if (RegenTimer == 0)
				{
					if (!IsInEvadeMode)
					{
						// regenerate health if not in combat or if polymorphed)
						if (!IsEngaged || IsPolymorphed)
						{
							RegenerateHealth();
						}
						else if (CanNotReachTarget)
						{
							// regenerate health if cannot reach the target and the setting is set to do so.
							// this allows to disable the health regen of raid bosses if pathfinding has issues for whatever reason
							if (WorldConfig.GetBoolValue(WorldCfg.RegenHpCannotReachTargetInRaid) || !Map.IsRaid)
							{
								RegenerateHealth();
								Log.outDebug(LogFilter.Unit, $"RegenerateHealth() enabled because Creature cannot reach the target. Detail: {GetDebugInfo()}");
							}
							else
							{
								Log.outDebug(LogFilter.Unit, $"RegenerateHealth() disabled even if the Creature cannot reach the target. Detail: {GetDebugInfo()}");
							}
						}
					}

					if (DisplayPowerType == PowerType.Energy)
						Regenerate(PowerType.Energy);
					else
						Regenerate(PowerType.Mana);

					RegenTimer = SharedConst.CreatureRegenInterval;
				}

				if (CanNotReachTarget && !IsInEvadeMode && !Map.IsRaid)
				{
					_cannotReachTimer += diff;

					if (_cannotReachTimer >= SharedConst.CreatureNoPathEvadeTime)
					{
						var ai = AI;

						if (ai != null)
							ai.EnterEvadeMode(EvadeReason.NoPath);
					}
				}

				break;
		}
	}

	public void Regenerate(PowerType power)
	{
		var curValue = GetPower(power);
		var maxValue = GetMaxPower(power);

		if (!HasUnitFlag2(UnitFlags2.RegeneratePower))
			return;

		if (curValue >= maxValue)
			return;

		double addvalue;

		switch (power)
		{
			case PowerType.Focus:
			{
				// For hunter pets.
				addvalue = 24 * WorldConfig.GetFloatValue(WorldCfg.RatePowerFocus);

				break;
			}
			case PowerType.Energy:
			{
				// For deathknight's ghoul.
				addvalue = 20;

				break;
			}
			case PowerType.Mana:
			{
				// Combat and any controlled creature
				if (IsInCombat || CharmerOrOwnerGUID.IsEmpty)
				{
					var ManaIncreaseRate = WorldConfig.GetFloatValue(WorldCfg.RatePowerMana);
					addvalue = (27.0f / 5.0f + 17.0f) * ManaIncreaseRate;
				}
				else
				{
					addvalue = maxValue / 3;
				}

				break;
			}
			default:
				return;
		}

		// Apply modifiers (if any).
		addvalue *= GetTotalAuraMultiplierByMiscValue(AuraType.ModPowerRegenPercent, (int)power);
		addvalue += GetTotalAuraModifierByMiscValue(AuraType.ModPowerRegen, (int)power) * (IsHunterPet ? SharedConst.PetFocusRegenInterval : SharedConst.CreatureRegenInterval) / (5 * Time.InMilliseconds);

		ModifyPower(power, (int)addvalue);
	}

	public void DoFleeToGetAssistance()
	{
		if (!Victim)
			return;

		if (HasAuraType(AuraType.PreventsFleeing))
			return;

		var radius = WorldConfig.GetFloatValue(WorldCfg.CreatureFamilyFleeAssistanceRadius);

		if (radius > 0)
		{
			var u_check = new NearestAssistCreatureInCreatureRangeCheck(this, Victim, radius);
			var searcher = new CreatureLastSearcher(this, u_check, GridType.Grid);
			Cell.VisitGrid(this, searcher, radius);

			var creature = searcher.GetTarget();

			SetNoSearchAssistance(true);

			if (!creature)
				SetControlled(true, UnitState.Fleeing);
			else
				MotionMaster.MoveSeekAssistance(creature.Location.X, creature.Location.Y, creature.Location.Z);
		}
	}

	public bool InitializeAI(CreatureAI ai = null)
	{
		InitializeMovementAI();

		AI = ai != null ? ai : AISelector.SelectAI(this);

		Ai.InitializeAI();

		// Initialize vehicle
		if (VehicleKit1 != null)
			VehicleKit1.Reset();

		return true;
	}

	public static Creature CreateCreature(uint entry, Map map, Position pos, uint vehId = 0)
	{
		var cInfo = Global.ObjectMgr.GetCreatureTemplate(entry);

		if (cInfo == null)
			return null;

		ulong lowGuid;

		if (vehId != 0 || cInfo.VehicleId != 0)
			lowGuid = map.GenerateLowGuid(HighGuid.Vehicle);
		else
			lowGuid = map.GenerateLowGuid(HighGuid.Creature);

		Creature creature = new();

		if (!creature.Create(lowGuid, map, entry, pos, null, vehId))
			return null;

		return creature;
	}

	public static Creature CreateCreatureFromDB(ulong spawnId, Map map, bool addToMap = true, bool allowDuplicate = false)
	{
		Creature creature = new();

		if (!creature.LoadFromDB(spawnId, map, addToMap, allowDuplicate))
			return null;

		return creature;
	}

	public bool Create(ulong guidlow, Map map, uint entry, Position pos, CreatureData data = null, uint vehId = 0, bool dynamic = false)
	{
		Map = map;

		if (data != null)
		{
			PhasingHandler.InitDbPhaseShift(PhaseShift, data.PhaseUseFlags, data.PhaseId, data.PhaseGroup);
			PhasingHandler.InitDbVisibleMapId(PhaseShift, data.terrainSwapMap);
		}

		// Set if this creature can handle dynamic spawns
		if (!dynamic)
			RespawnCompatibilityMode = true;

		var cinfo = Global.ObjectMgr.GetCreatureTemplate(entry);

		if (cinfo == null)
		{
			Log.outError(LogFilter.Sql, "Creature.Create: creature template (guidlow: {0}, entry: {1}) does not exist.", guidlow, entry);

			return false;
		}

		//! Relocate before CreateFromProto, to initialize coords and allow
		//! returning correct zone id for selecting OutdoorPvP/Battlefield script
		Location.Relocate(pos);

		// Check if the position is valid before calling CreateFromProto(), otherwise we might add Auras to Creatures at
		// invalid position, triggering a crash about Auras not removed in the destructor
		if (!Location.IsPositionValid)
		{
			Log.outError(LogFilter.Unit, $"Creature.Create: given coordinates for creature (guidlow {guidlow}, entry {entry}) are not valid ({pos})");

			return false;
		}

		{
			// area/zone id is needed immediately for ZoneScript::GetCreatureEntry hook before it is known which creature template to load (no model/scale available yet)
			PositionFullTerrainStatus positionData = new();
			Map.GetFullTerrainStatusForPosition(PhaseShift, Location.X, Location.Y, Location.Z, positionData, LiquidHeaderTypeFlags.AllLiquids, MapConst.DefaultCollesionHeight);
			ProcessPositionDataChanged(positionData);
		}

		// Allow players to see those units while dead, do it here (mayby altered by addon auras)
		if (cinfo.TypeFlags.HasAnyFlag(CreatureTypeFlags.VisibleToGhosts))
			ServerSideVisibility.SetValue(ServerSideVisibilityType.Ghost, GhostVisibilityType.Alive | GhostVisibilityType.Ghost);

		if (!CreateFromProto(guidlow, entry, data, vehId))
			return false;

		cinfo = CreatureTemplate; // might be different than initially requested

		if (cinfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.DungeonBoss) && map.IsDungeon)
			_respawnDelay = 0; // special value, prevents respawn for dungeon bosses unless overridden

		switch (cinfo.Rank)
		{
			case CreatureEliteType.Rare:
				_corpseDelay = WorldConfig.GetUIntValue(WorldCfg.CorpseDecayRare);

				break;
			case CreatureEliteType.Elite:
				_corpseDelay = WorldConfig.GetUIntValue(WorldCfg.CorpseDecayElite);

				break;
			case CreatureEliteType.RareElite:
				_corpseDelay = WorldConfig.GetUIntValue(WorldCfg.CorpseDecayRareelite);

				break;
			case CreatureEliteType.WorldBoss:
				_corpseDelay = WorldConfig.GetUIntValue(WorldCfg.CorpseDecayWorldboss);

				break;
			default:
				_corpseDelay = WorldConfig.GetUIntValue(WorldCfg.CorpseDecayNormal);

				break;
		}

		LoadCreaturesAddon();

		//! Need to be called after LoadCreaturesAddon - MOVEMENTFLAG_HOVER is set there
		Location.Z += HoverOffset;

		LastUsedScriptID = GetScriptId();

		if (IsSpiritHealer || IsSpiritGuide || CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.GhostVisibility))
		{
			ServerSideVisibility.SetValue(ServerSideVisibilityType.Ghost, GhostVisibilityType.Ghost);
			ServerSideVisibilityDetect.SetValue(ServerSideVisibilityType.Ghost, GhostVisibilityType.Ghost);
		}

		if (cinfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.IgnorePathfinding))
			AddUnitState(UnitState.IgnorePathfinding);

		if (cinfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.ImmunityKnockback))
		{
			ApplySpellImmune(0, SpellImmunity.Effect, SpellEffectName.KnockBack, true);
			ApplySpellImmune(0, SpellImmunity.Effect, SpellEffectName.KnockBackDest, true);
		}

		GetThreatManager().Initialize();

		return true;
	}

	public Unit SelectVictim()
	{
		Unit target;

		if (CanHaveThreatList)
		{
			target = GetThreatManager().CurrentVictim;
		}
		else if (!HasReactState(ReactStates.Passive))
		{
			// We're a player pet, probably
			target = GetAttackerForHelper();

			if (!target && IsSummon)
			{
				var owner = ToTempSummon().OwnerUnit;

				if (owner != null)
				{
					if (owner.IsInCombat)
						target = owner.GetAttackerForHelper();

					if (!target)
						foreach (var itr in owner.Controlled)
							if (itr.IsInCombat)
							{
								target = itr.GetAttackerForHelper();

								if (target)
									break;
							}
				}
			}
		}
		else
		{
			return null;
		}

		if (target && _IsTargetAcceptable(target) && CanCreatureAttack(target))
		{
			if (!HasSpellFocus())
				SetInFront(target);

			return target;
		}

		/// @todo a vehicle may eat some mob, so mob should not evade
		if (Vehicle1)
			return null;

		var iAuras = GetAuraEffectsByType(AuraType.ModInvisibility);

		if (!iAuras.Empty())
		{
			foreach (var itr in iAuras)
				if (itr.Base.IsPermanent)
				{
					AI.EnterEvadeMode(EvadeReason.Other);

					break;
				}

			return null;
		}

		// enter in evade mode in other case
		AI.EnterEvadeMode(EvadeReason.NoHostiles);

		return null;
	}

	public void InitializeReactState()
	{
		if (IsTotem || IsTrigger || IsCritter || IsSpiritService)
			ReactState = ReactStates.Passive;
		else
			ReactState = ReactStates.Aggressive;
	}

	public bool CanInteractWithBattleMaster(Player player, bool msg)
	{
		if (!IsBattleMaster)
			return false;

		var bgTypeId = Global.BattlegroundMgr.GetBattleMasterBG(Entry);

		if (!msg)
			return player.GetBgAccessByLevel(bgTypeId);

		if (!player.GetBgAccessByLevel(bgTypeId))
		{
			player.PlayerTalkClass.ClearMenus();

			switch (bgTypeId)
			{
				case BattlegroundTypeId.AV:
					player.PlayerTalkClass.SendGossipMenu(7616, GUID);

					break;
				case BattlegroundTypeId.WS:
					player.PlayerTalkClass.SendGossipMenu(7599, GUID);

					break;
				case BattlegroundTypeId.AB:
					player.PlayerTalkClass.SendGossipMenu(7642, GUID);

					break;
				case BattlegroundTypeId.EY:
				case BattlegroundTypeId.NA:
				case BattlegroundTypeId.BE:
				case BattlegroundTypeId.AA:
				case BattlegroundTypeId.RL:
				case BattlegroundTypeId.SA:
				case BattlegroundTypeId.DS:
				case BattlegroundTypeId.RV:
					player.PlayerTalkClass.SendGossipMenu(10024, GUID);

					break;
				default: break;
			}

			return false;
		}

		return true;
	}

	public bool CanResetTalents(Player player)
	{
		return player.Level >= 15 && player.Class == CreatureTemplate.TrainerClass;
	}

	public void SetTextRepeatId(byte textGroup, byte id)
	{
		if (!_textRepeat.ContainsKey(textGroup))
		{
			_textRepeat.Add(textGroup, id);

			return;
		}

		var repeats = _textRepeat[textGroup];

		if (!repeats.Contains(id))
			repeats.Add(id);
		else
			Log.outError(LogFilter.Sql, "CreatureTextMgr: TextGroup {0} for ({1}) {2}, id {3} already added", textGroup, GetName(), GUID.ToString(), id);
	}

	public List<byte> GetTextRepeatGroup(byte textGroup)
	{
		return _textRepeat.LookupByKey(textGroup);
	}

	public void ClearTextRepeatGroup(byte textGroup)
	{
		var groupList = _textRepeat[textGroup];

		if (groupList != null)
			groupList.Clear();
	}

	public override void AtEngage(Unit target)
	{
		base.AtEngage(target);

		if (!CreatureTemplate.TypeFlags.HasAnyFlag(CreatureTypeFlags.AllowMountedCombat))
			Dismount();

		RefreshCanSwimFlag();

		if (IsPet || IsGuardian) // update pets' speed for catchup OOC speed
		{
			UpdateSpeed(UnitMoveType.Run);
			UpdateSpeed(UnitMoveType.Swim);
			UpdateSpeed(UnitMoveType.Flight);
		}

		var movetype = MotionMaster.GetCurrentMovementGeneratorType();

		if (movetype == MovementGeneratorType.Waypoint || movetype == MovementGeneratorType.Point || (IsAIEnabled && AI.IsEscorted()))
		{
			HomePosition = Location;
			// if its a vehicle, set the home positon of every creature passenger at engage
			// so that they are in combat range if hostile
			var vehicle = VehicleKit1;

			if (vehicle != null)
				foreach (var (_, seat) in vehicle.Seats)
				{
					var passenger = Global.ObjAccessor.GetUnit(this, seat.Passenger.Guid);

					if (passenger != null)
					{
						var creature = passenger.AsCreature;

						if (creature != null)
							creature.HomePosition = Location;
					}
				}
		}

		var ai = AI;

		if (ai != null)
			ai.JustEngagedWith(target);

		var formation = Formation;

		if (formation != null)
			formation.MemberEngagingTarget(this, target);
	}

	public override void AtDisengage()
	{
		base.AtDisengage();

		ClearUnitState(UnitState.AttackPlayer);

		if (IsAlive && HasDynamicFlag(UnitDynFlags.Tapped))
			ReplaceAllDynamicFlags((UnitDynFlags)CreatureTemplate.DynamicFlags);

		if (IsPet || IsGuardian) // update pets' speed for catchup OOC speed
		{
			UpdateSpeed(UnitMoveType.Run);
			UpdateSpeed(UnitMoveType.Swim);
			UpdateSpeed(UnitMoveType.Flight);
		}
	}

	public override string GetDebugInfo()
	{
		return $"{base.GetDebugInfo()}\nAIName: {GetAIName()} ScriptName: {GetScriptName()} WaypointPath: {WaypointPath} SpawnId: {SpawnId}";
	}

	public override void ExitVehicle(Position exitPosition = null)
	{
		base.ExitVehicle();

		// if the creature exits a vehicle, set it's home position to the
		// exited position so it won't run away (home) and evade if it's hostile
		HomePosition = Location;
	}

	public override bool IsMovementPreventedByCasting()
	{
		// first check if currently a movement allowed channel is active and we're not casting
		var spell = GetCurrentSpell(CurrentSpellTypes.Channeled);

		if (spell != null)
			if (spell.State != SpellState.Finished && spell.IsChannelActive())
				if (spell.CheckMovement() != SpellCastResult.SpellCastOk)
					return true;

		if (HasSpellFocus())
			return true;

		if (HasUnitState(UnitState.Casting))
			return true;

		return false;
	}

	public void StartPickPocketRefillTimer()
	{
		_pickpocketLootRestore = GameTime.GetGameTime() + WorldConfig.GetIntValue(WorldCfg.CreaturePickpocketRefill);
	}

	public void ResetPickPocketRefillTimer()
	{
		_pickpocketLootRestore = 0;
	}

	public void SetTappedBy(Unit unit, bool withGroup = true)
	{
		// set the player whose group should receive the right
		// to loot the creature after it dies
		// should be set to NULL after the loot disappears

		if (unit == null)
		{
			_tapList.Clear();
			RemoveDynamicFlag(UnitDynFlags.Lootable | UnitDynFlags.Tapped);

			return;
		}

		if (_tapList.Count >= SharedConst.CreatureTappersSoftCap)
			return;

		if (!unit.IsTypeId(TypeId.Player) && !unit.IsVehicle)
			return;

		var player = unit.CharmerOrOwnerPlayerOrPlayerItself;

		if (player == null) // normal creature, no player involved
			return;

		_tapList.Add(player.GUID);

		if (withGroup)
		{
			var group = player.Group;

			if (group != null)
				for (var itr = group.FirstMember; itr != null; itr = itr.Next())
					if (Map.IsRaid || group.SameSubGroup(player, itr.Source))
						_tapList.Add(itr.Source.GUID);
		}

		if (_tapList.Count >= SharedConst.CreatureTappersSoftCap)
			SetDynamicFlag(UnitDynFlags.Tapped);
	}

	public bool IsTappedBy(Player player)
	{
		return _tapList.Contains(player.GUID);
	}

	public override Loot GetLootForPlayer(Player player)
	{
		if (PersonalLoot.Empty())
			return Loot;

		var loot = PersonalLoot.LookupByKey(player.GUID);

		if (loot != null)
			return loot;

		return null;
	}

	public bool IsSkinnedBy(Player player)
	{
		var loot = GetLootForPlayer(player);

		if (loot != null)
			return loot.loot_type == LootType.Skinning;

		return false;
	}

	public void SetTapList(HashSet<ObjectGuid> tapList)
	{
		_tapList = tapList;
	}

	public void SaveToDB()
	{
		// this should only be used when the creature has already been loaded
		// preferably after adding to map, because mapid may not be valid otherwise
		var data = Global.ObjectMgr.GetCreatureData(SpawnId);

		if (data == null)
		{
			Log.outError(LogFilter.Unit, "Creature.SaveToDB failed, cannot get creature data!");

			return;
		}

		var mapId = Location.MapId;
		var transport = Transport;

		if (transport != null)
			if (transport.GetMapIdForSpawning() >= 0)
				mapId = (uint)transport.GetMapIdForSpawning();

		SaveToDB(mapId, data.SpawnDifficulties);
	}

	public virtual void SaveToDB(uint mapid, List<Difficulty> spawnDifficulties)
	{
		// update in loaded data
		if (SpawnId == 0)
			SpawnId = Global.ObjectMgr.GenerateCreatureSpawnId();

		var data = Global.ObjectMgr.NewOrExistCreatureData(SpawnId);

		var displayId = NativeDisplayId;
		var npcflag = ((ulong)UnitData.NpcFlags[1] << 32) | UnitData.NpcFlags[0];
		uint unitFlags = UnitData.Flags;
		uint unitFlags2 = UnitData.Flags2;
		uint unitFlags3 = UnitData.Flags3;
		var dynamicflags = (UnitDynFlags)(uint)ObjectData.DynamicFlags;

		// check if it's a custom model and if not, use 0 for displayId
		var cinfo = CreatureTemplate;

		if (cinfo != null)
		{
			foreach (var model in cinfo.Models)
				if (displayId != 0 && displayId == model.CreatureDisplayId)
					displayId = 0;

			if (npcflag == (uint)cinfo.Npcflag)
				npcflag = 0;

			if (unitFlags == (uint)cinfo.UnitFlags)
				unitFlags = 0;

			if (unitFlags2 == cinfo.UnitFlags2)
				unitFlags2 = 0;

			if (unitFlags3 == cinfo.UnitFlags3)
				unitFlags3 = 0;

			if (dynamicflags == (UnitDynFlags)cinfo.DynamicFlags)
				dynamicflags = 0;
		}

		if (data.SpawnId == 0)
			data.SpawnId = SpawnId;

		Cypher.Assert(data.SpawnId == SpawnId);

		data.Id = Entry;
		data.Displayid = displayId;
		data.EquipmentId = (sbyte)CurrentEquipmentId;

		if (Transport == null)
		{
			data.MapId = Location.MapId;
			data.SpawnPoint.Relocate(Location);
		}
		else
		{
			data.MapId = mapid;
			data.SpawnPoint.Relocate(TransOffsetX, TransOffsetY, TransOffsetZ, TransOffsetO);
		}

		data.spawntimesecs = (int)_respawnDelay;
		// prevent add data integrity problems
		data.WanderDistance = GetDefaultMovementType() == MovementGeneratorType.Idle ? 0.0f : _wanderDistance;
		data.Currentwaypoint = 0;
		data.Curhealth = (uint)Health;
		data.Curmana = (uint)GetPower(PowerType.Mana);

		// prevent add data integrity problems
		data.MovementType = (byte)(_wanderDistance == 0 && GetDefaultMovementType() == MovementGeneratorType.Random
										? MovementGeneratorType.Idle
										: GetDefaultMovementType());

		data.SpawnDifficulties = spawnDifficulties;
		data.Npcflag = npcflag;
		data.UnitFlags = unitFlags;
		data.UnitFlags2 = unitFlags2;
		data.UnitFlags3 = unitFlags3;
		data.Dynamicflags = (uint)dynamicflags;

		if (data.SpawnGroupData == null)
			data.SpawnGroupData = Global.ObjectMgr.GetDefaultSpawnGroup();

		data.PhaseId = DBPhase > 0 ? (uint)DBPhase : data.PhaseId;
		data.PhaseGroup = DBPhase < 0 ? (uint)-DBPhase : data.PhaseGroup;

		// update in DB
		SQLTransaction trans = new();

		var stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_CREATURE);
		stmt.AddValue(0, SpawnId);
		trans.Append(stmt);

		byte index = 0;

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.INS_CREATURE);
		stmt.AddValue(index++, SpawnId);
		stmt.AddValue(index++, Entry);
		stmt.AddValue(index++, mapid);
		stmt.AddValue(index++, data.SpawnDifficulties.Empty() ? "" : string.Join(',', data.SpawnDifficulties));
		stmt.AddValue(index++, data.PhaseId);
		stmt.AddValue(index++, data.PhaseGroup);
		stmt.AddValue(index++, displayId);
		stmt.AddValue(index++, CurrentEquipmentId);
		stmt.AddValue(index++, Location.X);
		stmt.AddValue(index++, Location.Y);
		stmt.AddValue(index++, Location.Z);
		stmt.AddValue(index++, Location.Orientation);
		stmt.AddValue(index++, _respawnDelay);
		stmt.AddValue(index++, _wanderDistance);
		stmt.AddValue(index++, 0);
		stmt.AddValue(index++, Health);
		stmt.AddValue(index++, GetPower(PowerType.Mana));
		stmt.AddValue(index++, (byte)GetDefaultMovementType());
		stmt.AddValue(index++, npcflag);
		stmt.AddValue(index++, unitFlags);
		stmt.AddValue(index++, unitFlags2);
		stmt.AddValue(index++, unitFlags3);
		stmt.AddValue(index++, (uint)dynamicflags);
		trans.Append(stmt);

		DB.World.CommitTransaction(trans);
	}

	public void SelectLevel()
	{
		var cInfo = CreatureTemplate;

		// level
		var minMaxLevels = cInfo.GetMinMaxLevel();
		var minlevel = Math.Min(minMaxLevels[0], minMaxLevels[1]);
		var maxlevel = Math.Max(minMaxLevels[0], minMaxLevels[1]);
		var level = (minlevel == maxlevel ? minlevel : RandomHelper.IRand(minlevel, maxlevel));
		SetLevel((uint)level);

		ApplyLevelScaling();

		UpdateLevelDependantStats();
	}

	public void UpdateLevelDependantStats()
	{
		var cInfo = CreatureTemplate;
		var rank = IsPet ? 0 : cInfo.Rank;
		var level = Level;
		var stats = Global.ObjectMgr.GetCreatureBaseStats(level, cInfo.UnitClass);

		// health
		var healthmod = GetHealthMod(rank);

		var basehp = (uint)GetMaxHealthByLevel(level);
		var health = (uint)(basehp * healthmod);

		SetCreateHealth(health);
		SetMaxHealth(health);
		SetHealth(health);
		ResetPlayerDamageReq();

		// mana
		var mana = stats.GenerateMana(cInfo);
		SetCreateMana(mana);

		switch (Class)
		{
			case Class.Paladin:
			case Class.Mage:
				SetMaxPower(PowerType.Mana, (int)mana);
				SetPower(PowerType.Mana, (int)mana);

				break;
			default: // We don't set max power here, 0 makes power bar hidden
				break;
		}

		SetStatFlatModifier(UnitMods.Health, UnitModifierFlatType.Base, health);

		//Damage
		var basedamage = GetBaseDamageForLevel(level);
		var weaponBaseMinDamage = basedamage;
		var weaponBaseMaxDamage = basedamage * 1.5f;

		SetBaseWeaponDamage(WeaponAttackType.BaseAttack, WeaponDamageRange.MinDamage, weaponBaseMinDamage);
		SetBaseWeaponDamage(WeaponAttackType.BaseAttack, WeaponDamageRange.MaxDamage, weaponBaseMaxDamage);

		SetBaseWeaponDamage(WeaponAttackType.OffAttack, WeaponDamageRange.MinDamage, weaponBaseMinDamage);
		SetBaseWeaponDamage(WeaponAttackType.OffAttack, WeaponDamageRange.MaxDamage, weaponBaseMaxDamage);

		SetBaseWeaponDamage(WeaponAttackType.RangedAttack, WeaponDamageRange.MinDamage, weaponBaseMinDamage);
		SetBaseWeaponDamage(WeaponAttackType.RangedAttack, WeaponDamageRange.MaxDamage, weaponBaseMaxDamage);

		SetStatFlatModifier(UnitMods.AttackPower, UnitModifierFlatType.Base, stats.AttackPower);
		SetStatFlatModifier(UnitMods.AttackPowerRanged, UnitModifierFlatType.Base, stats.RangedAttackPower);

		var armor = GetBaseArmorForLevel(level); /// @todo Why is this treated as uint32 when it's a float?
		SetStatFlatModifier(UnitMods.Armor, UnitModifierFlatType.Base, armor);
	}

	public float GetHealthMod(CreatureEliteType Rank)
	{
		switch (Rank) // define rates for each elite rank
		{
			case CreatureEliteType.Normal:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureNormalHp);
			case CreatureEliteType.Elite:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteEliteHp);
			case CreatureEliteType.RareElite:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareeliteHp);
			case CreatureEliteType.WorldBoss:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteWorldbossHp);
			case CreatureEliteType.Rare:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareHp);
			default:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareeliteHp);
		}
	}

	public void LowerPlayerDamageReq(double unDamage)
	{
		LowerPlayerDamageReq((ulong)unDamage);
	}

	public void LowerPlayerDamageReq(ulong unDamage)
	{
		if (PlayerDamageReq != 0)
		{
			if (PlayerDamageReq > unDamage)
				PlayerDamageReq -= unDamage;
			else
				PlayerDamageReq = 0;
		}
	}

	public static float _GetDamageMod(CreatureEliteType Rank)
	{
		switch (Rank) // define rates for each elite rank
		{
			case CreatureEliteType.Normal:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureNormalDamage);
			case CreatureEliteType.Elite:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteEliteDamage);
			case CreatureEliteType.RareElite:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareeliteDamage);
			case CreatureEliteType.WorldBoss:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteWorldbossDamage);
			case CreatureEliteType.Rare:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareDamage);
			default:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteEliteDamage);
		}
	}

	public float GetSpellDamageMod(CreatureEliteType Rank)
	{
		switch (Rank) // define rates for each elite rank
		{
			case CreatureEliteType.Normal:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureNormalSpelldamage);
			case CreatureEliteType.Elite:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteEliteSpelldamage);
			case CreatureEliteType.RareElite:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareeliteSpelldamage);
			case CreatureEliteType.WorldBoss:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteWorldbossSpelldamage);
			case CreatureEliteType.Rare:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteRareSpelldamage);
			default:
				return WorldConfig.GetFloatValue(WorldCfg.RateCreatureEliteEliteSpelldamage);
		}
	}

	public override void SetCanDualWield(bool value)
	{
		base.SetCanDualWield(value);
		UpdateDamagePhysical(WeaponAttackType.OffAttack);
	}

	public void LoadEquipment(int id = 1, bool force = true)
	{
		if (id == 0)
		{
			if (force)
			{
				for (byte i = 0; i < SharedConst.MaxEquipmentItems; ++i)
					SetVirtualItem(i, 0);

				_equipmentId = 0;
			}

			return;
		}

		var einfo = Global.ObjectMgr.GetEquipmentInfo(Entry, id);

		if (einfo == null)
			return;

		_equipmentId = (byte)id;

		for (byte i = 0; i < SharedConst.MaxEquipmentItems; ++i)
			SetVirtualItem(i, einfo.Items[i].ItemId, einfo.Items[i].AppearanceModId, einfo.Items[i].ItemVisual);
	}

	public void SetSpawnHealth()
	{
		if (_regenerateHealthLock)
			return;

		long curhealth;

		if (_creatureData != null && !_regenerateHealth)
		{
			curhealth = _creatureData.Curhealth;

			if (curhealth != 0)
			{
				curhealth = (long)(curhealth * GetHealthMod(CreatureTemplate.Rank));

				if (curhealth < 1)
					curhealth = 1;
			}

			SetPower(PowerType.Mana, (int)_creatureData.Curmana);
		}
		else
		{
			curhealth = MaxHealth;
			SetFullPower(PowerType.Mana);
		}

		SetHealth((DeathState == DeathState.Alive || DeathState == DeathState.JustRespawned) ? curhealth : 0);
	}

	public override bool HasQuest(uint questId)
	{
		return Global.ObjectMgr.GetCreatureQuestRelations(Entry).HasQuest(questId);
	}

	public override bool HasInvolvedQuest(uint questId)
	{
		return Global.ObjectMgr.GetCreatureQuestInvolvedRelations(Entry).HasQuest(questId);
	}

	public static bool DeleteFromDB(ulong spawnId)
	{
		var data = Global.ObjectMgr.GetCreatureData(spawnId);

		if (data == null)
			return false;

		SQLTransaction trans = new();

		Global.MapMgr.DoForAllMapsWithMapId(data.MapId,
											map =>
											{
												// despawn all active creatures, and remove their respawns
												List<Creature> toUnload = new();

												foreach (var creature in map.CreatureBySpawnIdStore.LookupByKey(spawnId))
													toUnload.Add(creature);

												foreach (var creature in toUnload)
													map.AddObjectToRemoveList(creature);

												map.RemoveRespawnTime(SpawnObjectType.Creature, spawnId, trans);
											});

		// delete data from memory ...
		Global.ObjectMgr.DeleteCreatureData(spawnId);

		DB.Characters.CommitTransaction(trans);

		// ... and the database
		trans = new SQLTransaction();

		var stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_CREATURE);
		stmt.AddValue(0, spawnId);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_SPAWNGROUP_MEMBER);
		stmt.AddValue(0, (byte)SpawnObjectType.Creature);
		stmt.AddValue(1, spawnId);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_CREATURE_ADDON);
		stmt.AddValue(0, spawnId);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_GAME_EVENT_CREATURE);
		stmt.AddValue(0, spawnId);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_GAME_EVENT_MODEL_EQUIP);
		stmt.AddValue(0, spawnId);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_LINKED_RESPAWN);
		stmt.AddValue(0, spawnId);
		stmt.AddValue(1, (uint)CreatureLinkedRespawnType.CreatureToCreature);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_LINKED_RESPAWN);
		stmt.AddValue(0, spawnId);
		stmt.AddValue(1, (uint)CreatureLinkedRespawnType.CreatureToGO);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_LINKED_RESPAWN_MASTER);
		stmt.AddValue(0, spawnId);
		stmt.AddValue(1, (uint)CreatureLinkedRespawnType.CreatureToCreature);
		trans.Append(stmt);

		stmt = WorldDatabase.GetPreparedStatement(WorldStatements.DEL_LINKED_RESPAWN_MASTER);
		stmt.AddValue(0, spawnId);
		stmt.AddValue(1, (uint)CreatureLinkedRespawnType.GOToCreature);
		trans.Append(stmt);

		DB.World.CommitTransaction(trans);

		return true;
	}

	public override bool IsInvisibleDueToDespawn(WorldObject seer)
	{
		if (base.IsInvisibleDueToDespawn(seer))
			return true;

		if (IsAlive || CorpseRemoveTime > GameTime.GetGameTime())
			return false;

		return true;
	}

	public override bool CanAlwaysSee(WorldObject obj)
	{
		if (IsAIEnabled && GetAI<CreatureAI>().CanSeeAlways(obj))
			return true;

		return false;
	}

	public bool CanStartAttack(Unit who, bool force)
	{
		if (IsCivilian)
			return false;

		// This set of checks is should be done only for creatures
		if ((IsImmuneToNPC() && !who.HasUnitFlag(UnitFlags.PlayerControlled)) || (IsImmuneToPC() && who.HasUnitFlag(UnitFlags.PlayerControlled)))
			return false;

		// Do not attack non-combat pets
		if (who.IsTypeId(TypeId.Unit) && who.CreatureType == CreatureType.NonCombatPet)
			return false;

		if (!CanFly && (GetDistanceZ(who) > SharedConst.CreatureAttackRangeZ + CombatDistance))
			return false;

		if (!force)
		{
			if (!_IsTargetAcceptable(who))
				return false;

			if (IsNeutralToAll() || !IsWithinDistInMap(who, (float)GetAttackDistance(who) + CombatDistance))
				return false;
		}

		if (!CanCreatureAttack(who, force))
			return false;

		return IsWithinLOSInMap(who);
	}

	public double GetAttackDistance(Unit player)
	{
		var aggroRate = WorldConfig.GetFloatValue(WorldCfg.RateCreatureAggro);

		if (aggroRate == 0)
			return 0.0f;

		// WoW Wiki: the minimum radius seems to be 5 yards, while the maximum range is 45 yards
		var maxRadius = 45.0f * aggroRate;
		var minRadius = 5.0f * aggroRate;

		var expansionMaxLevel = (int)Global.ObjectMgr.GetMaxLevelForExpansion((Expansion)CreatureTemplate.RequiredExpansion);
		var playerLevel = (int)player.GetLevelForTarget(this);
		var creatureLevel = (int)GetLevelForTarget(player);
		var levelDifference = creatureLevel - playerLevel;

		// The aggro radius for creatures with equal level as the player is 20 yards.
		// The combatreach should not get taken into account for the distance so we drop it from the range (see Supremus as expample)
		var baseAggroDistance = 20.0f - CombatReach;

		// + - 1 yard for each level difference between player and creature
		double aggroRadius = baseAggroDistance + levelDifference;

		// detect range auras
		if ((creatureLevel + 5) <= WorldConfig.GetIntValue(WorldCfg.MaxPlayerLevel))
		{
			aggroRadius += GetTotalAuraModifier(AuraType.ModDetectRange);
			aggroRadius += player.GetTotalAuraModifier(AuraType.ModDetectedRange);
		}

		// The aggro range of creatures with higher levels than the total player level for the expansion should get the maxlevel treatment
		// This makes sure that creatures such as bosses wont have a bigger aggro range than the rest of the npc's
		// The following code is used for blizzlike behaviour such as skippable bosses
		if (creatureLevel > expansionMaxLevel)
			aggroRadius = baseAggroDistance + (float)(expansionMaxLevel - playerLevel);

		// Make sure that we wont go over the total range limits
		if (aggroRadius > maxRadius)
			aggroRadius = maxRadius;
		else if (aggroRadius < minRadius)
			aggroRadius = minRadius;

		return (aggroRadius * aggroRate);
	}

	public override void SetDeathState(DeathState s)
	{
		base.SetDeathState(s);

		if (s == DeathState.JustDied)
		{
			CorpseRemoveTime = GameTime.GetGameTime() + _corpseDelay;
			var respawnDelay = _respawnDelay;
			var scalingMode = WorldConfig.GetUIntValue(WorldCfg.RespawnDynamicMode);

			if (scalingMode != 0)
				Map.ApplyDynamicModeRespawnScaling(this, SpawnId, ref respawnDelay, scalingMode);

			// @todo remove the boss respawn time hack in a dynspawn follow-up once we have creature groups in instances
			if (_respawnCompatibilityMode)
			{
				if (IsDungeonBoss && _respawnDelay == 0)
					_respawnTime = long.MaxValue; // never respawn in this instance
				else
					_respawnTime = GameTime.GetGameTime() + respawnDelay + _corpseDelay;
			}
			else
			{
				if (IsDungeonBoss && _respawnDelay == 0)
					_respawnTime = long.MaxValue; // never respawn in this instance
				else
					_respawnTime = GameTime.GetGameTime() + respawnDelay;
			}

			SaveRespawnTime();

			ReleaseSpellFocus(null, false);   // remove spellcast focus
			DoNotReacquireSpellFocusTarget(); // cancel delayed re-target
			SetTarget(ObjectGuid.Empty);      // drop target - dead mobs shouldn't ever target things

			ReplaceAllNpcFlags(NPCFlags.None);
			ReplaceAllNpcFlags2(NPCFlags2.None);

			MountDisplayId = 0; // if creature is mounted on a virtual mount, remove it at death

			SetActive(false);
			SetNoSearchAssistance(false);

			//Dismiss group if is leader
			if (_creatureGroup != null && _creatureGroup.Leader == this)
				_creatureGroup.FormationReset(true);

			var needsFalling = (IsFlying || IsHovering) && !IsUnderWater;
			SetHover(false, false);
			SetDisableGravity(false, false);

			if (needsFalling)
				MotionMaster.MoveFall();

			base.SetDeathState(DeathState.Corpse);
		}
		else if (s == DeathState.JustRespawned)
		{
			if (IsPet)
				SetFullHealth();
			else
				SetSpawnHealth();

			SetTappedBy(null);
			ResetPlayerDamageReq();

			SetCannotReachTarget(false);
			UpdateMovementFlags();

			ClearUnitState(UnitState.AllErasable);

			if (!IsPet)
			{
				var creatureData = CreatureData;
				var cInfo = CreatureTemplate;

				ObjectManager.ChooseCreatureFlags(cInfo, out var npcFlags, out var unitFlags, out var unitFlags2, out var unitFlags3, out var dynamicFlags, creatureData);

				if (cInfo.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.Worldevent))
					npcFlags |= Global.GameEventMgr.GetNPCFlag(this);

				ReplaceAllNpcFlags((NPCFlags)(npcFlags & 0xFFFFFFFF));
				ReplaceAllNpcFlags2((NPCFlags2)(npcFlags >> 32));

				ReplaceAllUnitFlags((UnitFlags)unitFlags);
				ReplaceAllUnitFlags2((UnitFlags2)unitFlags2);
				ReplaceAllUnitFlags3((UnitFlags3)unitFlags3);
				ReplaceAllDynamicFlags((UnitDynFlags)dynamicFlags);

				RemoveUnitFlag(UnitFlags.InCombat);

				SetMeleeDamageSchool((SpellSchools)cInfo.DmgSchool);
			}

			InitializeMovementAI();
			base.SetDeathState(DeathState.Alive);
			LoadCreaturesAddon();
		}
	}

	public void Respawn(bool force = false)
	{
		if (force)
		{
			if (IsAlive)
				SetDeathState(DeathState.JustDied);
			else if (DeathState != DeathState.Corpse)
				SetDeathState(DeathState.Corpse);
		}

		if (_respawnCompatibilityMode)
		{
			UpdateObjectVisibilityOnDestroy();
			RemoveCorpse(false, false);

			if (DeathState == DeathState.Dead)
			{
				Log.outDebug(LogFilter.Unit, "Respawning creature {0} ({1})", GetName(), GUID.ToString());
				_respawnTime = 0;
				ResetPickPocketRefillTimer();
				Loot = null;

				if (OriginalEntry != Entry)
					UpdateEntry(OriginalEntry);

				SelectLevel();

				SetDeathState(DeathState.JustRespawned);

				CreatureModel display = new(NativeDisplayId, NativeDisplayScale, 1.0f);

				if (Global.ObjectMgr.GetCreatureModelRandomGender(ref display, CreatureTemplate) != null)
				{
					SetDisplayId(display.CreatureDisplayId, display.DisplayScale);
					SetNativeDisplayId(display.CreatureDisplayId, display.DisplayScale);
				}

				MotionMaster.InitializeDefault();

				//Re-initialize reactstate that could be altered by movementgenerators
				InitializeReactState();

				IUnitAI ai = AI;

				if (ai != null) // reset the AI to be sure no dirty or uninitialized values will be used till next tick
					ai.Reset();

				_triggerJustAppeared = true;

				var poolid = CreatureData != null ? CreatureData.poolId : 0;

				if (poolid != 0)
					Global.PoolMgr.UpdatePool<Creature>(Map.PoolData, poolid, SpawnId);
			}

			UpdateObjectVisibility();
		}
		else
		{
			if (SpawnId != 0)
				Map.Respawn(SpawnObjectType.Creature, SpawnId);
		}

		Log.outDebug(LogFilter.Unit, $"Respawning creature {GetName()} ({GUID})");
	}

	public void ForcedDespawn(uint timeMSToDespawn = 0, TimeSpan forceRespawnTimer = default)
	{
		if (timeMSToDespawn != 0)
		{
			Events.AddEvent(new ForcedDespawnDelayEvent(this, forceRespawnTimer), Events.CalculateTime(TimeSpan.FromMilliseconds(timeMSToDespawn)));

			return;
		}

		if (_respawnCompatibilityMode)
		{
			var corpseDelay = CorpseDelay;
			var respawnDelay = RespawnDelay;

			// do it before killing creature
			UpdateObjectVisibilityOnDestroy();

			var overrideRespawnTime = false;

			if (IsAlive)
			{
				if (forceRespawnTimer > TimeSpan.Zero)
				{
					SetCorpseDelay(0);
					RespawnDelay = (uint)forceRespawnTimer.TotalSeconds;
					overrideRespawnTime = false;
				}

				SetDeathState(DeathState.JustDied);
			}

			// Skip corpse decay time
			RemoveCorpse(overrideRespawnTime, false);

			SetCorpseDelay(corpseDelay);
			RespawnDelay = respawnDelay;
		}
		else
		{
			if (forceRespawnTimer > TimeSpan.Zero)
			{
				SaveRespawnTime((uint)forceRespawnTimer.TotalSeconds);
			}
			else
			{
				var respawnDelay = _respawnDelay;
				var scalingMode = WorldConfig.GetUIntValue(WorldCfg.RespawnDynamicMode);

				if (scalingMode != 0)
					Map.ApplyDynamicModeRespawnScaling(this, SpawnId, ref respawnDelay, scalingMode);

				_respawnTime = GameTime.GetGameTime() + respawnDelay;
				SaveRespawnTime();
			}

			AddObjectToRemoveList();
		}
	}

	public void DespawnOrUnsummon(TimeSpan msTimeToDespawn = default, TimeSpan forceRespawnTimer = default)
	{
		var summon = ToTempSummon();

		if (summon != null)
			summon.UnSummon(TimeSpan.FromMilliseconds(msTimeToDespawn.TotalMilliseconds));
		else
			ForcedDespawn((uint)msTimeToDespawn.TotalMilliseconds, forceRespawnTimer);
	}

	public void LoadTemplateImmunities()
	{
		// uint32 max used for "spell id", the immunity system will not perform SpellInfo checks against invalid spells
		// used so we know which immunities were loaded from template
		var placeholderSpellId = uint.MaxValue;

		// unapply template immunities (in case we're updating entry)
		for (uint i = 0; i < (int)Mechanics.Max; ++i)
			ApplySpellImmune(placeholderSpellId, SpellImmunity.Mechanic, i, false);

		for (var i = (int)SpellSchools.Normal; i < (int)SpellSchools.Max; ++i)
			ApplySpellImmune(placeholderSpellId, SpellImmunity.School, 1u << i, false);

		// don't inherit immunities for hunter pets
		if (OwnerGUID.IsPlayer && IsHunterPet)
			return;

		var mechanicMask = CreatureTemplate.MechanicImmuneMask;

		if (mechanicMask != 0)
			for (uint i = 0 + 1; i < (int)Mechanics.Max; ++i)
				if ((mechanicMask & (1ul << ((int)i - 1))) != 0)
					ApplySpellImmune(placeholderSpellId, SpellImmunity.Mechanic, i, true);

		var schoolMask = CreatureTemplate.SpellSchoolImmuneMask;

		if (schoolMask != 0)
			for (var i = (int)SpellSchools.Normal; i <= (int)SpellSchools.Max; ++i)
				if ((schoolMask & (1 << i)) != 0)
					ApplySpellImmune(placeholderSpellId, SpellImmunity.School, 1u << i, true);
	}

	public override bool IsImmunedToSpellEffect(SpellInfo spellInfo, SpellEffectInfo spellEffectInfo, WorldObject caster, bool requireImmunityPurgesEffectAttribute = false)
	{
		if (CreatureTemplate.CreatureType == CreatureType.Mechanical && spellEffectInfo.IsEffect(SpellEffectName.Heal))
			return true;

		return base.IsImmunedToSpellEffect(spellInfo, spellEffectInfo, caster, requireImmunityPurgesEffectAttribute);
	}

	// select nearest hostile unit within the given distance (regardless of threat list).
	public Unit SelectNearestTarget(float dist = 0)
	{
		if (dist == 0.0f)
			dist = SharedConst.MaxVisibilityDistance;

		var u_check = new NearestHostileUnitCheck(this, dist);
		var searcher = new UnitLastSearcher(this, u_check, GridType.All);
		Cell.VisitGrid(this, searcher, dist);

		return searcher.GetTarget();
	}

	// select nearest hostile unit within the given attack distance (i.e. distance is ignored if > than ATTACK_DISTANCE), regardless of threat list.
	public Unit SelectNearestTargetInAttackDistance(float dist = 0)
	{
		if (dist > SharedConst.MaxVisibilityDistance)
		{
			Log.outError(LogFilter.Unit, "Creature ({0}) SelectNearestTargetInAttackDistance called with dist > MAX_VISIBILITY_DISTANCE. Distance set to ATTACK_DISTANCE.", GUID.ToString());
			dist = SharedConst.AttackDistance;
		}

		var u_check = new NearestHostileUnitInAttackDistanceCheck(this, dist);
		var searcher = new UnitLastSearcher(this, u_check, GridType.All);

		Cell.VisitGrid(this, searcher, Math.Max(dist, SharedConst.AttackDistance));

		return searcher.GetTarget();
	}

	public void SendAIReaction(AiReaction reactionType)
	{
		AIReaction packet = new();

		packet.UnitGUID = GUID;
		packet.Reaction = reactionType;

		SendMessageToSet(packet, true);
	}

	public void CallAssistance()
	{
		if (!_alreadyCallAssistance && Victim != null && !IsPet && !IsCharmed)
		{
			SetNoCallAssistance(true);

			var radius = WorldConfig.GetFloatValue(WorldCfg.CreatureFamilyAssistanceRadius);

			if (radius > 0)
			{
				List<Creature> assistList = new();

				var u_check = new AnyAssistCreatureInRangeCheck(this, Victim, radius);
				var searcher = new CreatureListSearcher(this, assistList, u_check, GridType.Grid);
				Cell.VisitGrid(this, searcher, radius);

				if (!assistList.Empty())
				{
					AssistDelayEvent e = new(Victim.GUID, this);

					while (!assistList.Empty())
					{
						// Pushing guids because in delay can happen some creature gets despawned
						e.AddAssistant(assistList.First().GUID);
						assistList.Remove(assistList.First());
					}

					Events.AddEvent(e, Events.CalculateTime(TimeSpan.FromMilliseconds(WorldConfig.GetUIntValue(WorldCfg.CreatureFamilyAssistanceDelay))));
				}
			}
		}
	}

	public void CallForHelp(float radius)
	{
		if (radius <= 0.0f || !IsEngaged || !IsAlive || IsPet || IsCharmed)
			return;

		var target = GetThreatManager().CurrentVictim;

		if (target == null)
			target = GetThreatManager().GetAnyTarget();

		if (target == null)
			target = GetCombatManager().GetAnyTarget();

		if (target == null)
		{
			Log.outError(LogFilter.Unit, $"Creature {Entry} ({GetName()}) trying to call for help without being in combat.");

			return;
		}

		var u_do = new CallOfHelpCreatureInRangeDo(this, target, radius);
		var worker = new CreatureWorker(this, u_do, GridType.Grid);
		Cell.VisitGrid(this, worker, radius);
	}

	public bool CanAssistTo(Unit u, Unit enemy, bool checkfaction = true)
	{
		// is it true?
		if (!HasReactState(ReactStates.Aggressive))
			return false;

		// we don't need help from zombies :)
		if (!IsAlive)
			return false;

		// we cannot assist in evade mode
		if (IsInEvadeMode)
			return false;

		// or if enemy is in evade mode
		if (enemy.TypeId == TypeId.Unit && enemy.AsCreature.IsInEvadeMode)
			return false;

		// we don't need help from non-combatant ;)
		if (IsCivilian)
			return false;

		if (HasUnitFlag(UnitFlags.NonAttackable | UnitFlags.Uninteractible) || IsImmuneToNPC())
			return false;

		// skip fighting creature
		if (IsEngaged)
			return false;

		// only free creature
		if (!CharmerOrOwnerGUID.IsEmpty)
			return false;

		// only from same creature faction
		if (checkfaction)
		{
			if (Faction != u.Faction)
				return false;
		}
		else
		{
			if (!IsFriendlyTo(u))
				return false;
		}

		// skip non hostile to caster enemy creatures
		if (!IsHostileTo(enemy))
			return false;

		return true;
	}

	public bool _IsTargetAcceptable(Unit target)
	{
		// if the target cannot be attacked, the target is not acceptable
		if (IsFriendlyTo(target) || !target.IsTargetableForAttack(false) || (Vehicle != null && (IsOnVehicle(target) || Vehicle.GetBase().IsOnVehicle(target))))
			return false;

		if (target.HasUnitState(UnitState.Died))
		{
			// some creatures can detect fake death
			if (CanIgnoreFeignDeath && target.HasUnitFlag2(UnitFlags2.FeignDeath))
				return true;
			else
				return false;
		}

		// if I'm already fighting target, or I'm hostile towards the target, the target is acceptable
		if (IsEngagedBy(target) || IsHostileTo(target))
			return true;

		// if the target's victim is not friendly, or the target is friendly, the target is not acceptable
		return false;
	}

	public void SaveRespawnTime(uint forceDelay = 0)
	{
		if (IsSummon || SpawnId == 0 || (_creatureData != null && !_creatureData.DbData))
			return;

		if (_respawnCompatibilityMode)
		{
			RespawnInfo ri = new();
			ri.ObjectType = SpawnObjectType.Creature;
			ri.SpawnId = SpawnId;
			ri.RespawnTime = _respawnTime;
			Map.SaveRespawnInfoDB(ri);

			return;
		}

		var thisRespawnTime = forceDelay != 0 ? GameTime.GetGameTime() + forceDelay : _respawnTime;
		Map.SaveRespawnTime(SpawnObjectType.Creature, SpawnId, Entry, thisRespawnTime, GridDefines.ComputeGridCoord(HomePosition.X, HomePosition.Y).GetId());
	}

	public bool CanCreatureAttack(Unit victim, bool force = true)
	{
		if (!victim.IsInMap(this))
			return false;

		if (!IsValidAttackTarget(victim))
			return false;

		if (!victim.IsInAccessiblePlaceFor(this))
			return false;

		var ai = AI;

		if (ai != null)
			if (!ai.CanAIAttack(victim))
				return false;

		// we cannot attack in evade mode
		if (IsInEvadeMode)
			return false;

		// or if enemy is in evade mode
		if (victim.TypeId == TypeId.Unit && victim.AsCreature.IsInEvadeMode)
			return false;

		if (!CharmerOrOwnerGUID.IsPlayer)
		{
			if (Map.IsDungeon)
				return true;

			// don't check distance to home position if recently damaged, this should include taunt auras
			if (!IsWorldBoss && (LastDamagedTime > GameTime.GetGameTime() || HasAuraType(AuraType.ModTaunt)))
				return true;
		}

		// Map visibility range, but no more than 2*cell size
		var dist = Math.Min(Map.VisibilityRange, MapConst.SizeofCells * 2);

		var unit = CharmerOrOwner;

		if (unit != null)
		{
			return victim.IsWithinDist(unit, dist);
		}
		else
		{
			// include sizes for huge npcs
			dist += CombatReach + victim.CombatReach;

			// to prevent creatures in air ignore attacks because distance is already too high...
			if (MovementTemplate.IsFlightAllowed())
				return victim.Location.IsInDist2d(_homePosition, dist);
			else
				return victim.Location.IsInDist(_homePosition, dist);
		}
	}

	public bool LoadCreaturesAddon()
	{
		var creatureAddon = CreatureAddon;

		if (creatureAddon == null)
			return false;

		if (creatureAddon.Mount != 0)
			Mount(creatureAddon.Mount);

		SetStandState((UnitStandStateType)creatureAddon.StandState);
		ReplaceAllVisFlags((UnitVisFlags)creatureAddon.VisFlags);
		SetAnimTier((AnimTier)creatureAddon.AnimTier, false);

		//! Suspected correlation between UNIT_FIELD_BYTES_1, offset 3, value 0x2:
		//! If no inhabittype_fly (if no MovementFlag_DisableGravity or MovementFlag_CanFly flag found in sniffs)
		//! Check using InhabitType as movement flags are assigned dynamically
		//! basing on whether the creature is in air or not
		//! Set MovementFlag_Hover. Otherwise do nothing.
		if (CanHover)
			AddUnitMovementFlag(MovementFlag.Hover);

		Sheath = (SheathState)creatureAddon.SheathState;
		ReplaceAllPvpFlags((UnitPVPStateFlags)creatureAddon.PvpFlags);

		// These fields must only be handled by core internals and must not be modified via scripts/DB dat
		ReplaceAllPetFlags(UnitPetFlags.None);
		ShapeshiftForm = ShapeShiftForm.None;

		if (creatureAddon.Emote != 0)
			EmoteState = (Emote)creatureAddon.Emote;

		SetAIAnimKitId(creatureAddon.AiAnimKit);
		SetMovementAnimKitId(creatureAddon.MovementAnimKit);
		SetMeleeAnimKitId(creatureAddon.MeleeAnimKit);

		// Check if visibility distance different
		if (creatureAddon.VisibilityDistanceType != VisibilityDistanceType.Normal)
			SetVisibilityDistanceOverride(creatureAddon.VisibilityDistanceType);

		//Load Path
		if (creatureAddon.PathId != 0)
			_waypointPathId = creatureAddon.PathId;

		if (creatureAddon.Auras != null)
			foreach (var id in creatureAddon.Auras)
			{
				var AdditionalSpellInfo = Global.SpellMgr.GetSpellInfo(id, Map.DifficultyID);

				if (AdditionalSpellInfo == null)
				{
					Log.outError(LogFilter.Sql, "Creature ({0}) has wrong spell {1} defined in `auras` field.", GUID.ToString(), id);

					continue;
				}

				// skip already applied aura
				if (HasAura(id))
					continue;

				AddAura(id, this);
				Log.outDebug(LogFilter.Unit, "Spell: {0} added to creature ({1})", id, GUID.ToString());
			}

		return true;
	}

	// Send a message to LocalDefense channel for players opposition team in the zone
	public void SendZoneUnderAttackMessage(Player attacker)
	{
		var enemy_team = attacker.Team;

		ZoneUnderAttack packet = new();
		packet.AreaID = (int)Area;
		Global.WorldMgr.SendGlobalMessage(packet, null, (enemy_team == TeamFaction.Alliance ? TeamFaction.Horde : TeamFaction.Alliance));
	}

	public override bool HasSpell(uint spellId)
	{
		return Spells.Contains(spellId);
	}

	public Position GetRespawnPosition(out float dist)
	{
		if (_creatureData != null)
		{
			dist = _creatureData.WanderDistance;

			return _creatureData.SpawnPoint.Copy();
		}
		else
		{
			dist = 0;

			return HomePosition.Copy();
		}
	}

	public void UpdateMovementFlags()
	{
		// Do not update movement flags if creature is controlled by a player (charm/vehicle)
		if (PlayerMovingMe != null)
			return;

		// Creatures with CREATURE_FLAG_EXTRA_NO_MOVE_FLAGS_UPDATE should control MovementFlags in your own scripts
		if (CreatureTemplate.FlagsExtra.HasAnyFlag(CreatureFlagsExtra.NoMoveFlagsUpdate))
			return;

		// Set the movement flags if the creature is in that mode. (Only fly if actually in air, only swim if in water, etc)
		var ground = FloorZ;

		var canHover = CanHover;
		var isInAir = (MathFunctions.fuzzyGt(Location.Z, ground + (canHover ? UnitData.HoverHeight : 0.0f) + MapConst.GroundHeightTolerance) || MathFunctions.fuzzyLt(Location.Z, ground - MapConst.GroundHeightTolerance)); // Can be underground too, prevent the falling

		if (MovementTemplate.IsFlightAllowed() && (isInAir || !MovementTemplate.IsGroundAllowed()) && !IsFalling)
		{
			if (MovementTemplate.Flight == CreatureFlightMovementType.CanFly)
				SetCanFly(true);
			else
				SetDisableGravity(true);

			if (!HasAuraType(AuraType.Hover) && MovementTemplate.Ground != CreatureGroundMovementType.Hover)
				SetHover(false);
		}
		else
		{
			SetCanFly(false);
			SetDisableGravity(false);

			if (IsAlive && (CanHover || HasAuraType(AuraType.Hover)))
				SetHover(true);
		}

		if (!isInAir)
			SetFall(false);

		SetSwim(CanSwim && IsInWater);
	}

	public void RefreshCanSwimFlag(bool recheck = false)
	{
		if (!_isMissingCanSwimFlagOutOfCombat || recheck)
			_isMissingCanSwimFlagOutOfCombat = !HasUnitFlag(UnitFlags.CanSwim);

		// Check if the creature has UNIT_FLAG_CAN_SWIM and add it if it's missing
		// Creatures must be able to chase a target in water if they can enter water
		if (_isMissingCanSwimFlagOutOfCombat && CanEnterWater)
			SetUnitFlag(UnitFlags.CanSwim);
	}

	public void AllLootRemovedFromCorpse()
	{
		var now = GameTime.GetGameTime();

		// Do not reset corpse remove time if corpse is already removed
		if (CorpseRemoveTime <= now)
			return;

		// Scripts can choose to ignore RATE_CORPSE_DECAY_LOOTED by calling SetCorpseDelay(timer, true)
		var decayRate = _ignoreCorpseDecayRatio ? 1.0f : WorldConfig.GetFloatValue(WorldCfg.RateCorpseDecayLooted);

		// corpse skinnable, but without skinning flag, and then skinned, corpse will despawn next update
		bool isFullySkinned()
		{
			if (Loot != null && Loot.loot_type == LootType.Skinning && Loot.IsLooted())
				return true;

			foreach (var (_, loot) in PersonalLoot)
				if (loot.loot_type != LootType.Skinning || !loot.IsLooted())
					return false;

			return true;
		}

		if (isFullySkinned())
			CorpseRemoveTime = now;
		else
			CorpseRemoveTime = now + (uint)(_corpseDelay * decayRate);

		_respawnTime = Math.Max(CorpseRemoveTime + _respawnDelay, _respawnTime);
	}

	public void ApplyLevelScaling()
	{
		var scaling = CreatureTemplate.GetLevelScaling(Map.DifficultyID);
		var levels = Global.DB2Mgr.GetContentTuningData(scaling.ContentTuningId, 0);

		if (levels.HasValue)
		{
			SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.ScalingLevelMin), levels.Value.MinLevel);
			SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.ScalingLevelMax), levels.Value.MaxLevel);
		}
		else if (ConfigMgr.GetDefaultValue("CreatureScaling.DefaultMaxLevel", false))
		{
			SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.ScalingLevelMin), 1);
			SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.ScalingLevelMax), WorldConfig.GetIntValue(WorldCfg.MaxPlayerLevel));
		}

		int mindelta = Math.Min(scaling.DeltaLevelMax, scaling.DeltaLevelMin);
		int maxdelta = Math.Max(scaling.DeltaLevelMax, scaling.DeltaLevelMin);
		var delta = mindelta == maxdelta ? mindelta : RandomHelper.IRand(mindelta, maxdelta);

		SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.ScalingLevelDelta), delta);
		SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.ContentTuningID), scaling.ContentTuningId);
	}

	public override float GetHealthMultiplierForTarget(WorldObject target)
	{
		if (!HasScalableLevels)
			return 1.0f;

		var levelForTarget = GetLevelForTarget(target);

		if (Level < levelForTarget)
			return 1.0f;

		return (float)GetMaxHealthByLevel(levelForTarget) / GetCreateHealth();
	}

	public float GetBaseDamageForLevel(uint level)
	{
		var cInfo = CreatureTemplate;
		var scaling = cInfo.GetLevelScaling(Map.DifficultyID);

		return Global.DB2Mgr.EvaluateExpectedStat(ExpectedStatType.CreatureAutoAttackDps, level, cInfo.GetHealthScalingExpansion(), scaling.ContentTuningId, (Class)cInfo.UnitClass);
	}

	public override float GetDamageMultiplierForTarget(WorldObject target)
	{
		if (!HasScalableLevels)
			return 1.0f;

		var levelForTarget = GetLevelForTarget(target);

		return GetBaseDamageForLevel(levelForTarget) / GetBaseDamageForLevel(Level);
	}

	public override float GetArmorMultiplierForTarget(WorldObject target)
	{
		if (!HasScalableLevels)
			return 1.0f;

		var levelForTarget = GetLevelForTarget(target);

		return GetBaseArmorForLevel(levelForTarget) / GetBaseArmorForLevel(Level);
	}

	public override uint GetLevelForTarget(WorldObject target)
	{
		var unitTarget = target.AsUnit;

		if (unitTarget)
		{
			if (IsWorldBoss)
			{
				var level = (int)(unitTarget.Level + WorldConfig.GetIntValue(WorldCfg.WorldBossLevelDiff));

				return (uint)MathFunctions.RoundToInterval(ref level, 1u, 255u);
			}

			// If this creature should scale level, adapt level depending of target level
			// between UNIT_FIELD_SCALING_LEVEL_MIN and UNIT_FIELD_SCALING_LEVEL_MAX
			if (HasScalableLevels)
			{
				int scalingLevelMin = UnitData.ScalingLevelMin;
				int scalingLevelMax = UnitData.ScalingLevelMax;
				int scalingLevelDelta = UnitData.ScalingLevelDelta;
				int scalingFactionGroup = UnitData.ScalingFactionGroup;
				int targetLevel = unitTarget.UnitData.EffectiveLevel;

				if (targetLevel == 0)
					targetLevel = (int)unitTarget.Level;

				var targetLevelDelta = 0;

				var playerTarget = target.AsPlayer;

				if (playerTarget != null)
				{
					if (scalingFactionGroup != 0 && CliDB.FactionTemplateStorage.LookupByKey(CliDB.ChrRacesStorage.LookupByKey(playerTarget.Race).FactionID).FactionGroup != scalingFactionGroup)
						scalingLevelMin = scalingLevelMax;

					int maxCreatureScalingLevel = playerTarget.ActivePlayerData.MaxCreatureScalingLevel;
					targetLevelDelta = Math.Min(maxCreatureScalingLevel > 0 ? maxCreatureScalingLevel - targetLevel : 0, playerTarget.ActivePlayerData.ScalingPlayerLevelDelta);
				}

				var levelWithDelta = targetLevel + targetLevelDelta;
				var level = MathFunctions.RoundToInterval(ref levelWithDelta, scalingLevelMin, scalingLevelMax) + scalingLevelDelta;

				return (uint)MathFunctions.RoundToInterval(ref level, 1, SharedConst.MaxLevel + 3);
			}
		}

		return base.GetLevelForTarget(target);
	}

	public string GetAIName()
	{
		return Global.ObjectMgr.GetCreatureTemplate(Entry).AIName;
	}

	public string GetScriptName()
	{
		return Global.ObjectMgr.GetScriptName(GetScriptId());
	}

	public uint GetScriptId()
	{
		var creatureData = CreatureData;

		if (creatureData != null)
		{
			var scriptId = creatureData.ScriptId;

			if (scriptId != 0)
				return scriptId;
		}

		if (_creatureInfo.ScriptID != 0)
			return _creatureInfo.ScriptID;

		return Global.ObjectMgr.GetCreatureTemplate(Entry) != null ? Global.ObjectMgr.GetCreatureTemplate(Entry).ScriptID : 0;
	}

	public bool HasStringId(string id)
	{
		return _stringIds.Contains(id);
	}

	public uint GetVendorItemCurrentCount(VendorItem vItem)
	{
		if (vItem.Maxcount == 0)
			return vItem.Maxcount;

		VendorItemCount vCount = null;

		for (var i = 0; i < _vendorItemCounts.Count; i++)
		{
			vCount = _vendorItemCounts[i];

			if (vCount.ItemId == vItem.Item)
				break;
		}

		if (vCount == null)
			return vItem.Maxcount;

		var ptime = GameTime.GetGameTime();

		if (vCount.LastIncrementTime + vItem.Incrtime <= ptime)
		{
			var pProto = Global.ObjectMgr.GetItemTemplate(vItem.Item);

			var diff = (uint)((ptime - vCount.LastIncrementTime) / vItem.Incrtime);

			if ((vCount.Count + diff * pProto.GetBuyCount()) >= vItem.Maxcount)
			{
				_vendorItemCounts.Remove(vCount);

				return vItem.Maxcount;
			}

			vCount.Count += diff * pProto.GetBuyCount();
			vCount.LastIncrementTime = ptime;
		}

		return vCount.Count;
	}

	public uint UpdateVendorItemCurrentCount(VendorItem vItem, uint used_count)
	{
		if (vItem.Maxcount == 0)
			return 0;

		VendorItemCount vCount = null;

		for (var i = 0; i < _vendorItemCounts.Count; i++)
		{
			vCount = _vendorItemCounts[i];

			if (vCount.ItemId == vItem.Item)
				break;
		}

		if (vCount == null)
		{
			var new_count = vItem.Maxcount > used_count ? vItem.Maxcount - used_count : 0;
			_vendorItemCounts.Add(new VendorItemCount(vItem.Item, new_count));

			return new_count;
		}

		var ptime = GameTime.GetGameTime();

		if (vCount.LastIncrementTime + vItem.Incrtime <= ptime)
		{
			var pProto = Global.ObjectMgr.GetItemTemplate(vItem.Item);

			var diff = (uint)((ptime - vCount.LastIncrementTime) / vItem.Incrtime);

			if ((vCount.Count + diff * pProto.GetBuyCount()) < vItem.Maxcount)
				vCount.Count += diff * pProto.GetBuyCount();
			else
				vCount.Count = vItem.Maxcount;
		}

		vCount.Count = vCount.Count > used_count ? vCount.Count - used_count : 0;
		vCount.LastIncrementTime = ptime;

		return vCount.Count;
	}

	public override string GetName(Locale locale = Locale.enUS)
	{
		if (locale != Locale.enUS)
		{
			var cl = Global.ObjectMgr.GetCreatureLocale(Entry);

			if (cl != null)
				if (cl.Name.Length > (int)locale && !cl.Name[(int)locale].IsEmpty())
					return cl.Name[(int)locale];
		}

		return base.GetName(locale);
	}

	public virtual uint GetPetAutoSpellOnPos(byte pos)
	{
		if (pos >= SharedConst.MaxSpellCharm || GetCharmInfo() == null || GetCharmInfo().GetCharmSpell(pos).GetActiveState() != ActiveStates.Enabled)
			return 0;
		else
			return GetCharmInfo().GetCharmSpell(pos).GetAction();
	}

	public float GetPetChaseDistance()
	{
		var range = 0f;

		for (byte i = 0; i < PetAutoSpellSize; ++i)
		{
			var spellID = GetPetAutoSpellOnPos(i);

			if (spellID == 0)
				continue;

			var spellInfo = Global.SpellMgr.GetSpellInfo(spellID, Map.DifficultyID);

			if (spellInfo != null)
				if (spellInfo.RecoveryTime1 == 0 && spellInfo.RangeEntry.Id != 1 /*Self*/ && spellInfo.RangeEntry.Id != 2 /*Combat Range*/ && spellInfo.GetMaxRange() > range)
					range = spellInfo.GetMaxRange();
		}

		return range;
	}

	public void SetCannotReachTarget(bool cannotReach)
	{
		if (cannotReach == _cannotReachTarget)
			return;

		_cannotReachTarget = cannotReach;
		_cannotReachTimer = 0;

		if (cannotReach)
			Log.outDebug(LogFilter.Unit, $"Creature::SetCannotReachTarget() called with true. Details: {GetDebugInfo()}");
	}

	public float GetAggroRange(Unit target)
	{
		// Determines the aggro range for creatures (usually pets), used mainly for aggressive pet target selection.
		// Based on data from wowwiki due to lack of 3.3.5a data

		if (target != null && IsPet)
		{
			uint targetLevel = 0;

			if (target.IsTypeId(TypeId.Player))
				targetLevel = target.GetLevelForTarget(this);
			else if (target.IsTypeId(TypeId.Unit))
				targetLevel = target.AsCreature.GetLevelForTarget(this);

			var myLevel = GetLevelForTarget(target);
			var levelDiff = (int)(targetLevel - myLevel);

			// The maximum Aggro Radius is capped at 45 yards (25 level difference)
			if (levelDiff < -25)
				levelDiff = -25;

			// The base aggro radius for mob of same level
			double aggroRadius = 20;

			// Aggro Radius varies with level difference at a rate of roughly 1 yard/level
			aggroRadius -= levelDiff;

			// detect range auras
			aggroRadius += GetTotalAuraModifier(AuraType.ModDetectRange);

			// detected range auras
			aggroRadius += target.GetTotalAuraModifier(AuraType.ModDetectedRange);

			// Just in case, we don't want pets running all over the map
			if (aggroRadius > SharedConst.MaxAggroRadius)
				aggroRadius = SharedConst.MaxAggroRadius;

			// Minimum Aggro Radius for a mob seems to be combat range (5 yards)
			//  hunter pets seem to ignore minimum aggro radius so we'll default it a little higher
			if (aggroRadius < 10)
				aggroRadius = 10;

			return (float)(aggroRadius);
		}

		// Default
		return 0.0f;
	}

	public Unit SelectNearestHostileUnitInAggroRange(bool useLOS = false, bool ignoreCivilians = false)
	{
		// Selects nearest hostile target within creature's aggro range. Used primarily by
		//  pets set to aggressive. Will not return neutral or friendly targets
		var u_check = new NearestHostileUnitInAggroRangeCheck(this, useLOS, ignoreCivilians);
		var searcher = new UnitSearcher(this, u_check, GridType.Grid);
		Cell.VisitGrid(this, searcher, SharedConst.MaxAggroRadius);

		return searcher.GetTarget();
	}

	public override void SetDisplayId(uint modelId, float displayScale = 1f)
	{
		base.SetDisplayId(modelId, displayScale);

		var minfo = Global.ObjectMgr.GetCreatureModelInfo(modelId);

		if (minfo != null)
		{
			BoundingRadius = (IsPet ? 1.0f : minfo.BoundingRadius) * ObjectScale;
			SetCombatReach((IsPet ? SharedConst.DefaultPlayerCombatReach : minfo.CombatReach) * ObjectScale);
		}
	}

	public void SetDisplayFromModel(int modelIdx)
	{
		var model = CreatureTemplate.GetModelByIdx(modelIdx);

		if (model != null)
			SetDisplayId(model.CreatureDisplayId, model.DisplayScale);
	}

	public override void SetTarget(ObjectGuid guid)
	{
		if (HasSpellFocus())
			_spellFocusInfo.Target = guid;
		else
			SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.Target), guid);
	}

	public void SetSpellFocus(Spell focusSpell, WorldObject target)
	{
		// Pointer validation and checking for a already existing focus
		if (_spellFocusInfo.Spell != null || focusSpell == null)
			return;

		// Prevent dead / feign death creatures from setting a focus target
		if (!IsAlive || HasUnitFlag2(UnitFlags2.FeignDeath) || HasAuraType(AuraType.FeignDeath))
			return;

		// Don't allow stunned creatures to set a focus target
		if (HasUnitFlag(UnitFlags.Stunned))
			return;

		// some spells shouldn't track targets
		if (focusSpell.IsFocusDisabled())
			return;

		var spellInfo = focusSpell.SpellInfo;

		// don't use spell focus for vehicle spells
		if (spellInfo.HasAura(AuraType.ControlVehicle))
			return;

		// instant non-channeled casts and non-target spells don't need facing updates
		if (target == null && (focusSpell.CastTime == 0 && !spellInfo.IsChanneled))
			return;

		// store pre-cast values for target and orientation (used to later restore)
		if (_spellFocusInfo.Delay == 0)
		{
			// only overwrite these fields if we aren't transitioning from one spell focus to another
			_spellFocusInfo.Target = Target;
			_spellFocusInfo.Orientation = Location.Orientation;
		}
		else // don't automatically reacquire target for the previous spellcast
		{
			_spellFocusInfo.Delay = 0;
		}

		_spellFocusInfo.Spell = focusSpell;

		var noTurnDuringCast = spellInfo.HasAttribute(SpellAttr5.AiDoesntFaceTarget);
		var turnDisabled = HasUnitFlag2(UnitFlags2.CannotTurn);
		// set target, then force send update packet to players if it changed to provide appropriate facing
		var newTarget = (target != null && !noTurnDuringCast && !turnDisabled) ? target.GUID : ObjectGuid.Empty;

		if (Target != newTarget)
			SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.Target), newTarget);

		// If we are not allowed to turn during cast but have a focus target, face the target
		if (!turnDisabled && noTurnDuringCast && target)
			SetFacingToObject(target, false);

		if (!noTurnDuringCast)
			AddUnitState(UnitState.Focusing);
	}

	public override bool HasSpellFocus(Spell focusSpell = null)
	{
		if (IsDead) // dead creatures cannot focus
		{
			if (_spellFocusInfo.Spell != null || _spellFocusInfo.Delay != 0)
				Log.outWarn(LogFilter.Unit, $"Creature '{GetName()}' (entry {Entry}) has spell focus (spell id {(_spellFocusInfo.Spell != null ? _spellFocusInfo.Spell.SpellInfo.Id : 0)}, delay {_spellFocusInfo.Delay}ms) despite being dead.");

			return false;
		}

		if (focusSpell)
			return focusSpell == _spellFocusInfo.Spell;
		else
			return _spellFocusInfo.Spell != null || _spellFocusInfo.Delay != 0;
	}

	public void ReleaseSpellFocus(Spell focusSpell = null, bool withDelay = true)
	{
		if (!_spellFocusInfo.Spell)
			return;

		// focused to something else
		if (focusSpell && focusSpell != _spellFocusInfo.Spell)
			return;

		if (_spellFocusInfo.Spell.SpellInfo.HasAttribute(SpellAttr5.AiDoesntFaceTarget))
			ClearUnitState(UnitState.Focusing);

		if (IsPet) // player pets do not use delay system
		{
			if (!HasUnitFlag2(UnitFlags2.CannotTurn))
				ReacquireSpellFocusTarget();
		}
		else // don't allow re-target right away to prevent visual bugs
		{
			_spellFocusInfo.Delay = withDelay ? 1000 : 1u;
		}

		_spellFocusInfo.Spell = null;
	}

	public void DoNotReacquireSpellFocusTarget()
	{
		_spellFocusInfo.Delay = 0;
		_spellFocusInfo.Spell = null;
	}

	public void SetCorpseDelay(uint delay, bool ignoreCorpseDecayRatio = false)
	{
		_corpseDelay = delay;

		if (ignoreCorpseDecayRatio)
			_ignoreCorpseDecayRatio = true;
	}

	public bool HasReactState(ReactStates state)
	{
		return (_reactState == state);
	}

	public override void SetImmuneToAll(bool apply)
	{
		SetImmuneToAll(apply, HasReactState(ReactStates.Passive));
	}

	public override void SetImmuneToPC(bool apply)
	{
		SetImmuneToPC(apply, HasReactState(ReactStates.Passive));
	}

	public override void SetImmuneToNPC(bool apply)
	{
		SetImmuneToNPC(apply, HasReactState(ReactStates.Passive));
	}

    public override CreatureAI AI => (CreatureAI)Ai;

    public T GetAI<T>() where T : CreatureAI
	{
		return (T)Ai;
	}

	public override SpellSchoolMask GetMeleeDamageSchoolMask(WeaponAttackType attackType = WeaponAttackType.BaseAttack)
	{
		return _meleeDamageSchoolMask;
	}

	public void SetMeleeDamageSchool(SpellSchools school)
	{
		_meleeDamageSchoolMask = (SpellSchoolMask)(1 << (int)school);
	}

	public override bool LoadFromDB(ulong spawnId, Map map, bool addToMap, bool allowDuplicate)
	{
		if (!allowDuplicate)
		{
			// If an alive instance of this spawnId is already found, skip creation
			// If only dead instance(s) exist, despawn them and spawn a new (maybe also dead) version
			var creatureBounds = map.CreatureBySpawnIdStore.LookupByKey(spawnId);
			List<Creature> despawnList = new();

			foreach (var creature in creatureBounds)
				if (creature.IsAlive)
				{
					Log.outDebug(LogFilter.Maps, "Would have spawned {0} but {1} already exists", spawnId, creature.GUID.ToString());

					return false;
				}
				else
				{
					despawnList.Add(creature);
					Log.outDebug(LogFilter.Maps, "Despawned dead instance of spawn {0} ({1})", spawnId, creature.GUID.ToString());
				}

			foreach (var despawnCreature in despawnList)
				despawnCreature.AddObjectToRemoveList();
		}

		var data = Global.ObjectMgr.GetCreatureData(spawnId);

		if (data == null)
		{
			Log.outError(LogFilter.Sql, $"Creature (SpawnID: {spawnId}) not found in table `creature`, can't load.");

			return false;
		}

		SpawnId = spawnId;
		_respawnCompatibilityMode = data.SpawnGroupData.Flags.HasAnyFlag(SpawnGroupFlags.CompatibilityMode);
		_creatureData = data;
		_wanderDistance = data.WanderDistance;
		_respawnDelay = (uint)data.spawntimesecs;

		if (!Create(map.GenerateLowGuid(HighGuid.Creature), map, data.Id, data.SpawnPoint, data, 0, !_respawnCompatibilityMode))
			return false;

		//We should set first home position, because then AI calls home movement
		HomePosition = Location;

		DeathState = DeathState.Alive;

		_respawnTime = Map.GetCreatureRespawnTime(SpawnId);

		if (_respawnTime == 0 && !map.IsSpawnGroupActive(data.SpawnGroupData.GroupId))
		{
			if (!_respawnCompatibilityMode)
				// @todo pools need fixing! this is just a temporary thing, but they violate dynspawn principles
				if (data.poolId == 0)
				{
					Log.outError(LogFilter.Unit, $"Creature (SpawnID {spawnId}) trying to load in inactive spawn group '{data.SpawnGroupData.Name}':\n{GetDebugInfo()}");

					return false;
				}

			_respawnTime = GameTime.GetGameTime() + RandomHelper.URand(4, 7);
		}

		if (_respawnTime != 0)
		{
			if (!_respawnCompatibilityMode)
			{
				// @todo same as above
				if (data.poolId == 0)
				{
					Log.outError(LogFilter.Unit, $"Creature (SpawnID {spawnId}) trying to load despite a respawn timer in progress:\n{GetDebugInfo()}");

					return false;
				}
			}
			else
			{
				// compatibility mode creatures will be respawned in ::Update()
				DeathState = DeathState.Dead;
			}

			if (CanFly)
			{
				var tz = map.GetHeight(PhaseShift, data.SpawnPoint, true, MapConst.MaxFallDistance);

				if (data.SpawnPoint.Z - tz > 0.1f && GridDefines.IsValidMapCoord(tz))
					Location.Relocate(data.SpawnPoint.X, data.SpawnPoint.Y, tz);
			}
		}

		SetSpawnHealth();

		SelectWildBattlePetLevel();

		// checked at creature_template loading
		DefaultMovementType = (MovementGeneratorType)data.MovementType;

		_stringIds[1] = data.StringId;

		if (addToMap && !Map.AddToMap(this))
			return false;

		return true;
	}

	public LootModes GetLootMode()
	{
		return _lootMode;
	}

	public bool HasLootMode(LootModes lootMode)
	{
		return Convert.ToBoolean(_lootMode & lootMode);
	}

	public void SetLootMode(LootModes lootMode)
	{
		_lootMode = lootMode;
	}

	public void AddLootMode(LootModes lootMode)
	{
		_lootMode |= lootMode;
	}

	public void RemoveLootMode(LootModes lootMode)
	{
		_lootMode &= ~lootMode;
	}

	public void ResetLootMode()
	{
		_lootMode = LootModes.Default;
	}

	public void SetNoCallAssistance(bool val)
	{
		_alreadyCallAssistance = val;
	}

	public void SetNoSearchAssistance(bool val)
	{
		_alreadySearchedAssistance = val;
	}

	public override MovementGeneratorType GetDefaultMovementType()
	{
		return DefaultMovementType;
	}

	public void SetDefaultMovementType(MovementGeneratorType mgt)
	{
		DefaultMovementType = mgt;
	}

	public void SetRespawnTime(uint respawn)
	{
		_respawnTime = respawn != 0 ? GameTime.GetGameTime() + respawn : 0;
	}

	public void DoImmediateBoundaryCheck() => _boundaryCheckTime = 0;

	public void SetRegenerateHealth(bool value)
	{
		_regenerateHealthLock = !value;
	}

	public void SetHomePosition(float x, float y, float z, float o)
	{
		_homePosition.Relocate(x, y, z, o);
	}

	public void SetTransportHomePosition(float x, float y, float z, float o)
	{
		_transportHomePosition.Relocate(x, y, z, o);
	}

	public void LoadPath(uint pathid)
	{
		_waypointPathId = pathid;
	}

	public void UpdateCurrentWaypointInfo(uint nodeId, uint pathId)
	{
		_currentWaypointNodeInfo = (nodeId, pathId);
	}

	public void ResetPlayerDamageReq()
	{
		PlayerDamageReq = (uint)(Health / 2);
	}

	void RegenerateHealth()
	{
		if (!CanRegenerateHealth)
			return;

		var curValue = Health;
		var maxValue = MaxHealth;

		if (curValue >= maxValue)
			return;

		double addvalue;

		// Not only pet, but any controlled creature (and not polymorphed)
		if (!CharmerOrOwnerGUID.IsEmpty && !IsPolymorphed)
		{
			var HealthIncreaseRate = WorldConfig.GetFloatValue(WorldCfg.RateHealth);
			addvalue = 0.015f * MaxHealth * HealthIncreaseRate;
		}
		else
		{
			addvalue = maxValue / 3;
		}

		// Apply modifiers (if any).
		addvalue *= GetTotalAuraMultiplier(AuraType.ModHealthRegenPercent);
		addvalue += GetTotalAuraModifier(AuraType.ModRegen) * SharedConst.CreatureRegenInterval / (5 * Time.InMilliseconds);

		ModifyHealth(addvalue);
	}

	bool DestoryAI()
	{
		PopAI();
		RefreshAI();

		return true;
	}

	void InitializeMovementAI()
	{
		if (_creatureGroup != null)
		{
			if (_creatureGroup.Leader == this)
			{
				_creatureGroup.FormationReset(false);
			}
			else if (_creatureGroup.IsFormed)
			{
				MotionMaster.MoveIdle(); //wait the order of leader

				return;
			}
		}

		MotionMaster.Initialize();
	}

	void SelectWildBattlePetLevel()
	{
		if (IsWildBattlePet)
		{
			byte wildBattlePetLevel = 1;

			var areaTable = CliDB.AreaTableStorage.LookupByKey(Zone);

			if (areaTable != null)
				if (areaTable.WildBattlePetLevelMin > 0)
					wildBattlePetLevel = (byte)RandomHelper.URand(areaTable.WildBattlePetLevelMin, areaTable.WildBattlePetLevelMax);

			WildBattlePetLevel = wildBattlePetLevel;
		}
	}

	bool CreateFromProto(ulong guidlow, uint entry, CreatureData data = null, uint vehId = 0)
	{
		SetZoneScript();

		if (ZoneScript != null && data != null)
		{
			entry = ZoneScript.GetCreatureEntry(guidlow, data);

			if (entry == 0)
				return false;
		}

		var cinfo = Global.ObjectMgr.GetCreatureTemplate(entry);

		if (cinfo == null)
		{
			Log.outError(LogFilter.Sql, "Creature.CreateFromProto: creature template (guidlow: {0}, entry: {1}) does not exist.", guidlow, entry);

			return false;
		}

		OriginalEntry1 = entry;

		if (vehId != 0 || cinfo.VehicleId != 0)
			Create(ObjectGuid.Create(HighGuid.Vehicle, Location.MapId, entry, guidlow));
		else
			Create(ObjectGuid.Create(HighGuid.Creature, Location.MapId, entry, guidlow));

		if (!UpdateEntry(entry, data))
			return false;

		if (vehId == 0)
		{
			if (CreatureTemplate.VehicleId != 0)
			{
				vehId = CreatureTemplate.VehicleId;
				entry = CreatureTemplate.Entry;
			}
			else
			{
				vehId = cinfo.VehicleId;
			}
		}

		if (vehId != 0)
			if (CreateVehicleKit(vehId, entry, true))
				UpdateDisplayPower();

		return true;
	}

	void LoadTemplateRoot()
	{
		if (MovementTemplate.IsRooted())
			SetControlled(true, UnitState.Root);
	}

	void InitializeMovementFlags()
	{
		// It does the same, for now
		UpdateMovementFlags();
	}

	ulong GetMaxHealthByLevel(uint level)
	{
		var cInfo = CreatureTemplate;
		var scaling = cInfo.GetLevelScaling(Map.DifficultyID);
		var baseHealth = Global.DB2Mgr.EvaluateExpectedStat(ExpectedStatType.CreatureHealth, level, cInfo.GetHealthScalingExpansion(), scaling.ContentTuningId, (Class)cInfo.UnitClass);

		return (ulong)(baseHealth * cInfo.ModHealth * cInfo.ModHealthExtra);
	}

	float GetBaseArmorForLevel(uint level)
	{
		var cInfo = CreatureTemplate;
		var scaling = cInfo.GetLevelScaling(Map.DifficultyID);
		var baseArmor = Global.DB2Mgr.EvaluateExpectedStat(ExpectedStatType.CreatureArmor, level, cInfo.GetHealthScalingExpansion(), scaling.ContentTuningId, (Class)cInfo.UnitClass);

		return baseArmor * cInfo.ModArmor;
	}

	void SetScriptStringId(string id)
	{
		if (!id.IsEmpty())
		{
			_scriptStringId = id;
			_stringIds[2] = _scriptStringId;
		}
		else
		{
			_scriptStringId = null;
			_stringIds[2] = null;
		}
	}

	void ReacquireSpellFocusTarget()
	{
		if (!HasSpellFocus())
		{
			Log.outError(LogFilter.Unit, $"Creature::ReacquireSpellFocusTarget() being called with HasSpellFocus() returning false. {GetDebugInfo()}");

			return;
		}

		SetUpdateFieldValue(Values.ModifyValue(UnitData).ModifyValue(UnitData.Target), _spellFocusInfo.Target);

		if (!HasUnitFlag2(UnitFlags2.CannotTurn))
		{
			if (!_spellFocusInfo.Target.IsEmpty)
			{
				var objTarget = Global.ObjAccessor.GetWorldObject(this, _spellFocusInfo.Target);

				if (objTarget)
					SetFacingToObject(objTarget, false);
			}
			else
			{
				SetFacingTo(_spellFocusInfo.Orientation, false);
			}
		}

		_spellFocusInfo.Delay = 0;
	}

	void SetDisableReputationGain(bool disable)
	{
		_reputationGain = disable;
	}
}