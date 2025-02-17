﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Constants;
using Framework.Database;
using Framework.IO;
using Game.Conditions;
using Game.DataStorage;
using Game.Entities;
using Game.Loots;
using Game.Maps;
using Game.Spells;

namespace Game;

public sealed class ConditionManager : Singleton<ConditionManager>
{
	public string[] StaticSourceTypeData =
	{
		"None", "Creature Loot", "Disenchant Loot", "Fishing Loot", "GameObject Loot", "Item Loot", "Mail Loot", "Milling Loot", "Pickpocketing Loot", "Prospecting Loot", "Reference Loot", "Skinning Loot", "Spell Loot", "Spell Impl. Target", "Gossip Menu", "Gossip Menu Option", "Creature Vehicle", "Spell Expl. Target", "Spell Click Event", "Quest Accept", "Quest Show Mark", "Vehicle Spell", "SmartScript", "Npc Vendor", "Spell Proc", "Terrain Swap", "Phase", "Graveyard", "AreaTrigger", "ConversationLine", "AreaTrigger Client Triggered", "Trainer Spell", "Object Visibility (by ID)", "Spawn Group"
	};

	public ConditionTypeInfo[] StaticConditionTypeData =
	{
		new("None", false, false, false), new("Aura", true, true, true), new("Item Stored", true, true, true), new("Item Equipped", true, false, false), new("Zone", true, false, false), new("Reputation", true, true, false), new("Team", true, false, false), new("Skill", true, true, false), new("Quest Rewarded", true, false, false), new("Quest Taken", true, false, false), new("Drunken", true, false, false), new("WorldState", true, true, false), new("Active Event", true, false, false), new("Instance Info", true, true, true), new("Quest None", true, false, false), new("Class", true, false, false), new("Race", true, false, false), new("Achievement", true, false, false), new("Title", true, false, false), new("SpawnMask", true, false, false), new("Gender", true, false, false), new("Unit State", true, false, false), new("Map", true, false, false), new("Area", true, false, false), new("CreatureType", true, false, false), new("Spell Known", true, false, false), new("Phase", true, false, false), new("Level", true, true, false), new("Quest Completed", true, false, false), new("Near Creature", true, true, true), new("Near GameObject", true, true, false), new("Object Entry or Guid", true, true, true), new("Object TypeMask", true, false, false), new("Relation", true, true, false), new("Reaction", true, true, false), new("Distance", true, true, true), new("Alive", false, false, false), new("Health Value", true, true, false), new("Health Pct", true, true, false), new("Realm Achievement", true, false, false), new("In Water", false, false, false), new("Terrain Swap", true, false, false), new("Sit/stand state", true, true, false), new("Daily Quest Completed", true, false, false), new("Charmed", false, false, false), new("Pet type", true, false, false), new("On Taxi", false, false, false), new("Quest state mask", true, true, false), new("Quest objective progress", true, false, true), new("Map Difficulty", true, false, false), new("Is Gamemaster", true, false, false), new("Object Entry or Guid", true, true, true), new("Object TypeMask", true, false, false), new("BattlePet Species Learned", true, true, true), new("On Scenario Step", true, false, false), new("Scene In Progress", true, false, false), new("Player Condition", true, false, false)
	};

	readonly Dictionary<ConditionSourceType, MultiMap<uint, Condition>> conditionStorage = new();
	readonly MultiMap<uint, Condition> conditionReferenceStorage = new();
	readonly Dictionary<uint, MultiMap<uint, Condition>> vehicleSpellConditionStorage = new();
	readonly Dictionary<uint, MultiMap<uint, Condition>> spellClickEventConditionStorage = new();
	readonly List<uint> spellsUsedInSpellClickConditions = new();
	readonly Dictionary<uint, MultiMap<uint, Condition>> npcVendorConditionContainerStorage = new();
	readonly Dictionary<Tuple<int, uint>, MultiMap<uint, Condition>> smartEventConditionStorage = new();
	readonly MultiMap<Tuple<uint, bool>, Condition> areaTriggerConditionContainerStorage = new();
	readonly Dictionary<uint, MultiMap<uint, Condition>> trainerSpellConditionContainerStorage = new();
	readonly MultiMap<(uint objectType, uint objectId), Condition> objectVisibilityConditionStorage = new();
	ConditionManager() { }

	public GridMapTypeMask GetSearcherTypeMaskForConditionList(List<Condition> conditions)
	{
		if (conditions.Empty())
			return GridMapTypeMask.All;

		//     groupId, typeMask
		Dictionary<uint, GridMapTypeMask> elseGroupSearcherTypeMasks = new();

		foreach (var i in conditions)
		{
			// group not filled yet, fill with widest mask possible
			if (!elseGroupSearcherTypeMasks.ContainsKey(i.ElseGroup))
				elseGroupSearcherTypeMasks[i.ElseGroup] = GridMapTypeMask.All;
			// no point of checking anymore, empty mask
			else if (elseGroupSearcherTypeMasks[i.ElseGroup] == 0)
				continue;

			if (i.ReferenceId != 0) // handle reference
			{
				var refe = conditionReferenceStorage.LookupByKey(i.ReferenceId);
				elseGroupSearcherTypeMasks[i.ElseGroup] &= GetSearcherTypeMaskForConditionList(refe);
			}
			else // handle normal condition
			{
				// object will match conditions in one ElseGroupStore only when it matches all of them
				// so, let's find a smallest possible mask which satisfies all conditions
				elseGroupSearcherTypeMasks[i.ElseGroup] &= i.GetSearcherTypeMaskForCondition();
			}
		}

		// object will match condition when one of the checks in ElseGroupStore is matching
		// so, let's include all possible masks
		GridMapTypeMask mask = 0;

		foreach (var i in elseGroupSearcherTypeMasks)
			mask |= i.Value;

		return mask;
	}

	public bool IsObjectMeetToConditionList(ConditionSourceInfo sourceInfo, List<Condition> conditions)
	{
		//     groupId, groupCheckPassed
		Dictionary<uint, bool> elseGroupStore = new();

		foreach (var condition in conditions)
		{
			Log.outDebug(LogFilter.Condition, "ConditionMgr.IsPlayerMeetToConditionList condType: {0} val1: {1}", condition.ConditionType, condition.ConditionValue1);

			if (condition.IsLoaded())
			{
				//! Find ElseGroup in ElseGroupStore
				//! If not found, add an entry in the store and set to true (placeholder)
				if (!elseGroupStore.ContainsKey(condition.ElseGroup))
					elseGroupStore[condition.ElseGroup] = true;
				else if (!elseGroupStore[condition.ElseGroup]) //! If another condition in this group was unmatched before this, don't bother checking (the group is false anyway)
					continue;

				if (condition.ReferenceId != 0) //handle reference
				{
					var refe = conditionReferenceStorage.LookupByKey(condition.ReferenceId);

					if (!refe.Empty())
					{
						if (!IsObjectMeetToConditionList(sourceInfo, refe))
							elseGroupStore[condition.ElseGroup] = false;
					}
					else
					{
						Log.outDebug(LogFilter.Condition,
									"IsPlayerMeetToConditionList: Reference template -{0} not found",
									condition.ReferenceId); //checked at loading, should never happen
					}
				}
				else //handle normal condition
				{
					if (!condition.Meets(sourceInfo))
						elseGroupStore[condition.ElseGroup] = false;
				}
			}
		}

		foreach (var i in elseGroupStore)
			if (i.Value)
				return true;

		return false;
	}

	public bool IsObjectMeetToConditions(WorldObject obj, List<Condition> conditions)
	{
		ConditionSourceInfo srcInfo = new(obj);

		return IsObjectMeetToConditions(srcInfo, conditions);
	}

	public bool IsObjectMeetToConditions(WorldObject obj1, WorldObject obj2, List<Condition> conditions)
	{
		ConditionSourceInfo srcInfo = new(obj1, obj2);

		return IsObjectMeetToConditions(srcInfo, conditions);
	}

	public bool IsObjectMeetToConditions(ConditionSourceInfo sourceInfo, List<Condition> conditions)
	{
		if (conditions.Empty())
			return true;

		Log.outDebug(LogFilter.Condition, "ConditionMgr.IsObjectMeetToConditions");

		return IsObjectMeetToConditionList(sourceInfo, conditions);
	}

	public bool CanHaveSourceGroupSet(ConditionSourceType sourceType)
	{
		return sourceType == ConditionSourceType.CreatureLootTemplate ||
				sourceType == ConditionSourceType.DisenchantLootTemplate ||
				sourceType == ConditionSourceType.FishingLootTemplate ||
				sourceType == ConditionSourceType.GameobjectLootTemplate ||
				sourceType == ConditionSourceType.ItemLootTemplate ||
				sourceType == ConditionSourceType.MailLootTemplate ||
				sourceType == ConditionSourceType.MillingLootTemplate ||
				sourceType == ConditionSourceType.PickpocketingLootTemplate ||
				sourceType == ConditionSourceType.ProspectingLootTemplate ||
				sourceType == ConditionSourceType.ReferenceLootTemplate ||
				sourceType == ConditionSourceType.SkinningLootTemplate ||
				sourceType == ConditionSourceType.SpellLootTemplate ||
				sourceType == ConditionSourceType.GossipMenu ||
				sourceType == ConditionSourceType.GossipMenuOption ||
				sourceType == ConditionSourceType.VehicleSpell ||
				sourceType == ConditionSourceType.SpellImplicitTarget ||
				sourceType == ConditionSourceType.SpellClickEvent ||
				sourceType == ConditionSourceType.SmartEvent ||
				sourceType == ConditionSourceType.NpcVendor ||
				sourceType == ConditionSourceType.Phase ||
				sourceType == ConditionSourceType.AreaTrigger ||
				sourceType == ConditionSourceType.TrainerSpell ||
				sourceType == ConditionSourceType.ObjectIdVisibility;
	}

	public bool CanHaveSourceIdSet(ConditionSourceType sourceType)
	{
		return (sourceType == ConditionSourceType.SmartEvent);
	}

	public bool IsObjectMeetingNotGroupedConditions(ConditionSourceType sourceType, uint entry, ConditionSourceInfo sourceInfo)
	{
		if (sourceType > ConditionSourceType.None && sourceType < ConditionSourceType.Max)
		{
			var conditions = conditionStorage[sourceType].LookupByKey(entry);

			if (!conditions.Empty())
			{
				Log.outDebug(LogFilter.Condition, "GetConditionsForNotGroupedEntry: found conditions for type {0} and entry {1}", sourceType, entry);

				return IsObjectMeetToConditions(sourceInfo, conditions);
			}
		}

		return true;
	}

	public bool IsObjectMeetingNotGroupedConditions(ConditionSourceType sourceType, uint entry, WorldObject target0, WorldObject target1 = null, WorldObject target2 = null)
	{
		ConditionSourceInfo conditionSource = new(target0, target1, target2);

		return IsObjectMeetingNotGroupedConditions(sourceType, entry, conditionSource);
	}

	public bool IsMapMeetingNotGroupedConditions(ConditionSourceType sourceType, uint entry, Map map)
	{
		ConditionSourceInfo conditionSource = new(map);

		return IsObjectMeetingNotGroupedConditions(sourceType, entry, conditionSource);
	}

	public bool HasConditionsForNotGroupedEntry(ConditionSourceType sourceType, uint entry)
	{
		if (sourceType > ConditionSourceType.None && sourceType < ConditionSourceType.Max)
			if (conditionStorage[sourceType].ContainsKey(entry))
				return true;

		return false;
	}

	public bool IsObjectMeetingSpellClickConditions(uint creatureId, uint spellId, WorldObject clicker, WorldObject target)
	{
		var multiMap = spellClickEventConditionStorage.LookupByKey(creatureId);

		if (multiMap != null)
		{
			var conditions = multiMap.LookupByKey(spellId);

			if (!conditions.Empty())
			{
				Log.outDebug(LogFilter.Condition, "GetConditionsForSpellClickEvent: found conditions for SpellClickEvent entry {0} spell {1}", creatureId, spellId);
				ConditionSourceInfo sourceInfo = new(clicker, target);

				return IsObjectMeetToConditions(sourceInfo, conditions);
			}
		}

		return true;
	}

	public List<Condition> GetConditionsForSpellClickEvent(uint creatureId, uint spellId)
	{
		var multiMap = spellClickEventConditionStorage.LookupByKey(creatureId);

		if (multiMap != null)
		{
			var conditions = multiMap.LookupByKey(spellId);

			if (!conditions.Empty())
			{
				Log.outDebug(LogFilter.Condition, "GetConditionsForSpellClickEvent: found conditions for SpellClickEvent entry {0} spell {1}", creatureId, spellId);

				return conditions;
			}
		}

		return null;
	}

	public bool IsObjectMeetingVehicleSpellConditions(uint creatureId, uint spellId, Player player, Unit vehicle)
	{
		var multiMap = vehicleSpellConditionStorage.LookupByKey(creatureId);

		if (multiMap != null)
		{
			var conditions = multiMap.LookupByKey(spellId);

			if (!conditions.Empty())
			{
				Log.outDebug(LogFilter.Condition, "GetConditionsForVehicleSpell: found conditions for Vehicle entry {0} spell {1}", creatureId, spellId);
				ConditionSourceInfo sourceInfo = new(player, vehicle);

				return IsObjectMeetToConditions(sourceInfo, conditions);
			}
		}

		return true;
	}

	public bool IsObjectMeetingSmartEventConditions(long entryOrGuid, uint eventId, SmartScriptType sourceType, Unit unit, WorldObject baseObject)
	{
		var multiMap = smartEventConditionStorage.LookupByKey(Tuple.Create((int)entryOrGuid, (uint)sourceType));

		if (multiMap != null)
		{
			var conditions = multiMap.LookupByKey(eventId + 1);

			if (!conditions.Empty())
			{
				Log.outDebug(LogFilter.Condition, "GetConditionsForSmartEvent: found conditions for Smart Event entry or guid {0} eventId {1}", entryOrGuid, eventId);
				ConditionSourceInfo sourceInfo = new(unit, baseObject);

				return IsObjectMeetToConditions(sourceInfo, conditions);
			}
		}

		return true;
	}

	public bool IsObjectMeetingVendorItemConditions(uint creatureId, uint itemId, Player player, Creature vendor)
	{
		var multiMap = npcVendorConditionContainerStorage.LookupByKey(creatureId);

		if (multiMap != null)
		{
			var conditions = multiMap.LookupByKey(itemId);

			if (!conditions.Empty())
			{
				Log.outDebug(LogFilter.Condition, "GetConditionsForNpcVendor: found conditions for creature entry {0} item {1}", creatureId, itemId);
				ConditionSourceInfo sourceInfo = new(player, vendor);

				return IsObjectMeetToConditions(sourceInfo, conditions);
			}
		}

		return true;
	}

	public bool IsSpellUsedInSpellClickConditions(uint spellId)
	{
		return spellsUsedInSpellClickConditions.Contains(spellId);
	}

	public List<Condition> GetConditionsForAreaTrigger(uint areaTriggerId, bool isServerSide)
	{
		return areaTriggerConditionContainerStorage.LookupByKey(Tuple.Create(areaTriggerId, isServerSide));
	}

	public bool IsObjectMeetingTrainerSpellConditions(uint trainerId, uint spellId, Player player)
	{
		var multiMap = trainerSpellConditionContainerStorage.LookupByKey(trainerId);

		if (multiMap != null)
		{
			var conditionList = multiMap.LookupByKey(spellId);

			if (!conditionList.Empty())
			{
				Log.outDebug(LogFilter.Condition, $"GetConditionsForTrainerSpell: found conditions for trainer id {trainerId} spell {spellId}");

				return IsObjectMeetToConditions(player, conditionList);
			}
		}

		return true;
	}

	public bool IsObjectMeetingVisibilityByObjectIdConditions(uint objectType, uint entry, WorldObject seer)
	{
		var conditions = objectVisibilityConditionStorage.LookupByKey((objectType, entry));

		if (conditions != null)
		{
			Log.outDebug(LogFilter.Condition, $"IsObjectMeetingVisibilityByObjectIdConditions: found conditions for objectType {objectType} entry {entry}");

			return IsObjectMeetToConditions(seer, conditions);
		}

		return true;
	}

	public void LoadConditions(bool isReload = false)
	{
		var oldMSTime = Time.MSTime;

		Clean();

		//must clear all custom handled cases (groupped types) before reload
		if (isReload)
		{
			Log.outInfo(LogFilter.Server, "Reseting Loot Conditions...");
			LootStorage.Creature.ResetConditions();
			LootStorage.Fishing.ResetConditions();
			LootStorage.Gameobject.ResetConditions();
			LootStorage.Items.ResetConditions();
			LootStorage.Mail.ResetConditions();
			LootStorage.Milling.ResetConditions();
			LootStorage.Pickpocketing.ResetConditions();
			LootStorage.Reference.ResetConditions();
			LootStorage.Skinning.ResetConditions();
			LootStorage.Disenchant.ResetConditions();
			LootStorage.Prospecting.ResetConditions();
			LootStorage.Spell.ResetConditions();

			Log.outInfo(LogFilter.Server, "Re-Loading `gossip_menu` Table for Conditions!");
			Global.ObjectMgr.LoadGossipMenu();

			Log.outInfo(LogFilter.Server, "Re-Loading `gossip_menu_option` Table for Conditions!");
			Global.ObjectMgr.LoadGossipMenuItems();
			Global.SpellMgr.UnloadSpellInfoImplicitTargetConditionLists();

			Global.ObjectMgr.UnloadPhaseConditions();
		}

		var result = DB.World.Query("SELECT SourceTypeOrReferenceId, SourceGroup, SourceEntry, SourceId, ElseGroup, ConditionTypeOrReference, ConditionTarget, " +
									" ConditionValue1, ConditionValue2, ConditionValue3, NegativeCondition, ErrorType, ErrorTextId, ScriptName FROM conditions");

		if (result.IsEmpty())
		{
			Log.outInfo(LogFilter.ServerLoading, "Loaded 0 conditions. DB table `conditions` is empty!");

			return;
		}

		uint count = 0;

		do
		{
			Condition cond = new();
			var iSourceTypeOrReferenceId = result.Read<int>(0);
			cond.SourceGroup = result.Read<uint>(1);
			cond.SourceEntry = result.Read<int>(2);
			cond.SourceId = result.Read<uint>(3);
			cond.ElseGroup = result.Read<uint>(4);
			var iConditionTypeOrReference = result.Read<int>(5);
			cond.ConditionTarget = result.Read<byte>(6);
			cond.ConditionValue1 = result.Read<uint>(7);
			cond.ConditionValue2 = result.Read<uint>(8);
			cond.ConditionValue3 = result.Read<uint>(9);
			cond.NegativeCondition = result.Read<byte>(10) != 0;
			cond.ErrorType = result.Read<uint>(11);
			cond.ErrorTextId = result.Read<uint>(12);
			cond.ScriptId = Global.ObjectMgr.GetScriptId(result.Read<string>(13));

			if (iConditionTypeOrReference >= 0)
				cond.ConditionType = (ConditionTypes)iConditionTypeOrReference;

			if (iSourceTypeOrReferenceId >= 0)
				cond.SourceType = (ConditionSourceType)iSourceTypeOrReferenceId;

			if (iConditionTypeOrReference < 0) //it has a reference
			{
				if (iConditionTypeOrReference == iSourceTypeOrReferenceId) //self referencing, skip
				{
					Log.outDebug(LogFilter.Sql, "Condition reference {1} is referencing self, skipped", iSourceTypeOrReferenceId);

					continue;
				}

				cond.ReferenceId = (uint)Math.Abs(iConditionTypeOrReference);

				var rowType = "reference template";

				if (iSourceTypeOrReferenceId >= 0)
					rowType = "reference";

				//check for useless data
				if (cond.ConditionTarget != 0)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in ConditionTarget ({2})!", rowType, iSourceTypeOrReferenceId, cond.ConditionTarget);

				if (cond.ConditionValue1 != 0)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in value1 ({2})!", rowType, iSourceTypeOrReferenceId, cond.ConditionValue1);

				if (cond.ConditionValue2 != 0)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in value2 ({2})!", rowType, iSourceTypeOrReferenceId, cond.ConditionValue2);

				if (cond.ConditionValue3 != 0)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in value3 ({2})!", rowType, iSourceTypeOrReferenceId, cond.ConditionValue3);

				if (cond.NegativeCondition)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in NegativeCondition ({2})!", rowType, iSourceTypeOrReferenceId, cond.NegativeCondition);

				if (cond.SourceGroup != 0 && iSourceTypeOrReferenceId < 0)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in SourceGroup ({2})!", rowType, iSourceTypeOrReferenceId, cond.SourceGroup);

				if (cond.SourceEntry != 0 && iSourceTypeOrReferenceId < 0)
					Log.outDebug(LogFilter.Sql, "Condition {0} {1} has useless data in SourceEntry ({2})!", rowType, iSourceTypeOrReferenceId, cond.SourceEntry);
			}
			else if (!IsConditionTypeValid(cond)) //doesn't have reference, validate ConditionType
			{
				continue;
			}

			if (iSourceTypeOrReferenceId < 0) //it is a reference template
			{
				conditionReferenceStorage.Add((uint)Math.Abs(iSourceTypeOrReferenceId), cond); //add to reference storage
				count++;

				continue;
			} //end of reference templates

			//if not a reference and SourceType is invalid, skip
			if (iConditionTypeOrReference >= 0 && !IsSourceTypeValid(cond))
				continue;

			//Grouping is only allowed for some types (loot templates, gossip menus, gossip items)
			if (cond.SourceGroup != 0 && !CanHaveSourceGroupSet(cond.SourceType))
			{
				Log.outDebug(LogFilter.Sql, "{0} has not allowed value of SourceGroup = {1}!", cond.ToString(), cond.SourceGroup);

				continue;
			}

			if (cond.SourceId != 0 && !CanHaveSourceIdSet(cond.SourceType))
			{
				Log.outDebug(LogFilter.Sql, "{0} has not allowed value of SourceId = {1}!", cond.ToString(), cond.SourceId);

				continue;
			}

			if (cond.ErrorType != 0 && cond.SourceType != ConditionSourceType.Spell)
			{
				Log.outDebug(LogFilter.Sql, "{0} can't have ErrorType ({1}), set to 0!", cond.ToString(), cond.ErrorType);
				cond.ErrorType = 0;
			}

			if (cond.ErrorTextId != 0 && cond.ErrorType == 0)
			{
				Log.outDebug(LogFilter.Sql, "{0} has any ErrorType, ErrorTextId ({1}) is set, set to 0!", cond.ToString(), cond.ErrorTextId);
				cond.ErrorTextId = 0;
			}

			if (cond.SourceGroup != 0)
			{
				var valid = false;

				// handle grouped conditions
				switch (cond.SourceType)
				{
					case ConditionSourceType.CreatureLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Creature.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.DisenchantLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Disenchant.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.FishingLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Fishing.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.GameobjectLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Gameobject.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.ItemLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Items.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.MailLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Mail.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.MillingLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Milling.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.PickpocketingLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Pickpocketing.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.ProspectingLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Prospecting.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.ReferenceLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Reference.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.SkinningLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Skinning.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.SpellLootTemplate:
						valid = AddToLootTemplate(cond, LootStorage.Spell.GetLootForConditionFill(cond.SourceGroup));

						break;
					case ConditionSourceType.GossipMenu:
						valid = AddToGossipMenus(cond);

						break;
					case ConditionSourceType.GossipMenuOption:
						valid = AddToGossipMenuItems(cond);

						break;
					case ConditionSourceType.SpellClickEvent:
					{
						if (!spellClickEventConditionStorage.ContainsKey(cond.SourceGroup))
							spellClickEventConditionStorage[cond.SourceGroup] = new MultiMap<uint, Condition>();

						spellClickEventConditionStorage[cond.SourceGroup].Add((uint)cond.SourceEntry, cond);

						if (cond.ConditionType == ConditionTypes.Aura)
							spellsUsedInSpellClickConditions.Add(cond.ConditionValue1);

						++count;

						continue; // do not add to m_AllocatedMemory to avoid double deleting
					}
					case ConditionSourceType.SpellImplicitTarget:
						valid = AddToSpellImplicitTargetConditions(cond);

						break;
					case ConditionSourceType.VehicleSpell:
					{
						if (!vehicleSpellConditionStorage.ContainsKey(cond.SourceGroup))
							vehicleSpellConditionStorage[cond.SourceGroup] = new MultiMap<uint, Condition>();

						vehicleSpellConditionStorage[cond.SourceGroup].Add((uint)cond.SourceEntry, cond);
						++count;

						continue; // do not add to m_AllocatedMemory to avoid double deleting
					}
					case ConditionSourceType.SmartEvent:
					{
						//! TODO: PAIR_32 ?
						var key = Tuple.Create(cond.SourceEntry, cond.SourceId);

						if (!smartEventConditionStorage.ContainsKey(key))
							smartEventConditionStorage[key] = new MultiMap<uint, Condition>();

						smartEventConditionStorage[key].Add(cond.SourceGroup, cond);
						++count;

						continue;
					}
					case ConditionSourceType.NpcVendor:
					{
						if (!npcVendorConditionContainerStorage.ContainsKey(cond.SourceGroup))
							npcVendorConditionContainerStorage[cond.SourceGroup] = new MultiMap<uint, Condition>();

						npcVendorConditionContainerStorage[cond.SourceGroup].Add((uint)cond.SourceEntry, cond);
						++count;

						continue;
					}
					case ConditionSourceType.Phase:
						valid = AddToPhases(cond);

						break;
					case ConditionSourceType.AreaTrigger:
						areaTriggerConditionContainerStorage.Add(Tuple.Create(cond.SourceGroup, cond.SourceEntry != 0), cond);
						++count;

						continue;
					case ConditionSourceType.TrainerSpell:
					{
						if (!trainerSpellConditionContainerStorage.ContainsKey(cond.SourceGroup))
							trainerSpellConditionContainerStorage[cond.SourceGroup] = new MultiMap<uint, Condition>();

						trainerSpellConditionContainerStorage[cond.SourceGroup].Add((uint)cond.SourceEntry, cond);
						++count;

						continue;
					}
					case ConditionSourceType.ObjectIdVisibility:
					{
						objectVisibilityConditionStorage.Add((cond.SourceGroup, (uint)cond.SourceEntry), cond);
						valid = true;
						++count;

						continue;
					}
					default:
						break;
				}

				if (!valid)
					Log.outDebug(LogFilter.Sql, "{0} Not handled grouped condition.", cond.ToString());
				else
					++count;

				continue;
			}

			//add new Condition to storage based on Type/Entry
			if (cond.SourceType == ConditionSourceType.SpellClickEvent && cond.ConditionType == ConditionTypes.Aura)
				spellsUsedInSpellClickConditions.Add(cond.ConditionValue1);

			conditionStorage[cond.SourceType].Add((uint)cond.SourceEntry, cond);
			++count;
		} while (result.NextRow());

		Log.outInfo(LogFilter.ServerLoading, "Loaded {0} conditions in {1} ms", count, Time.GetMSTimeDiffToNow(oldMSTime));
	}

	public static uint GetPlayerConditionLfgValue(Player player, PlayerConditionLfgStatus status)
	{
		if (player.Group == null)
			return 0;

		switch (status)
		{
			case PlayerConditionLfgStatus.InLFGDungeon:
				return Global.LFGMgr.InLfgDungeonMap(player.GUID, player.Location.MapId, player.Map.DifficultyID) ? 1 : 0u;
			case PlayerConditionLfgStatus.InLFGRandomDungeon:
				return Global.LFGMgr.InLfgDungeonMap(player.GUID, player.Location.MapId, player.Map.DifficultyID) &&
						Global.LFGMgr.SelectedRandomLfgDungeon(player.GUID)
							? 1
							: 0u;
			case PlayerConditionLfgStatus.InLFGFirstRandomDungeon:
			{
				if (!Global.LFGMgr.InLfgDungeonMap(player.GUID, player.Location.MapId, player.Map.DifficultyID))
					return 0;

				var selectedRandomDungeon = Global.LFGMgr.GetSelectedRandomDungeon(player.GUID);

				if (selectedRandomDungeon == 0)
					return 0;

				var reward = Global.LFGMgr.GetRandomDungeonReward(selectedRandomDungeon, player.Level);

				if (reward != null)
				{
					var quest = Global.ObjectMgr.GetQuestTemplate(reward.firstQuest);

					if (quest != null)
						if (player.CanRewardQuest(quest, false))
							return 1;
				}

				return 0;
			}
			case PlayerConditionLfgStatus.PartialClear:
				break;
			case PlayerConditionLfgStatus.StrangerCount:
				break;
			case PlayerConditionLfgStatus.VoteKickCount:
				break;
			case PlayerConditionLfgStatus.BootCount:
				break;
			case PlayerConditionLfgStatus.GearDiff:
				break;
			default:
				break;
		}

		return 0;
	}

	public static bool IsPlayerMeetingCondition(Player player, PlayerConditionRecord condition)
	{
		var levels = Global.DB2Mgr.GetContentTuningData(condition.ContentTuningID, player.PlayerData.CtrOptions.GetValue().ContentTuningConditionMask);

		if (levels.HasValue)
		{
			var minLevel = (byte)(condition.Flags.HasAnyFlag(0x800) ? levels.Value.MinLevelWithDelta : levels.Value.MinLevel);
			byte maxLevel = 0;

			if (!condition.Flags.HasAnyFlag(0x20))
				maxLevel = (byte)(condition.Flags.HasAnyFlag(0x800) ? levels.Value.MaxLevelWithDelta : levels.Value.MaxLevel);

			if (condition.Flags.HasAnyFlag(0x80))
			{
				if (minLevel != 0 && player.Level >= minLevel && (maxLevel == 0 || player.Level <= maxLevel))
					return false;

				if (maxLevel != 0 && player.Level <= maxLevel && (minLevel == 0 || player.Level >= minLevel))
					return false;
			}
			else
			{
				if (minLevel != 0 && player.Level < minLevel)
					return false;

				if (maxLevel != 0 && player.Level > maxLevel)
					return false;
			}
		}

		if (condition.RaceMask != 0 && !Convert.ToBoolean(SharedConst.GetMaskForRace(player.Race) & condition.RaceMask))
			return false;

		if (condition.ClassMask != 0 && !Convert.ToBoolean(player.ClassMask & condition.ClassMask))
			return false;

		if (condition.Gender >= 0 && (int)player.Gender != condition.Gender)
			return false;

		if (condition.NativeGender >= 0 && player.NativeGender != (Gender)condition.NativeGender)
			return false;

		if (condition.PowerType != -1 && condition.PowerTypeComp != 0)
		{
			var requiredPowerValue = Convert.ToBoolean(condition.Flags & 4) ? player.GetMaxPower((PowerType)condition.PowerType) : condition.PowerTypeValue;

			if (!PlayerConditionCompare(condition.PowerTypeComp, player.GetPower((PowerType)condition.PowerType), requiredPowerValue))
				return false;
		}

		if (condition.ChrSpecializationIndex >= 0 || condition.ChrSpecializationRole >= 0)
		{
			var spec = CliDB.ChrSpecializationStorage.LookupByKey(player.GetPrimarySpecialization());

			if (spec != null)
			{
				if (condition.ChrSpecializationIndex >= 0 && spec.OrderIndex != condition.ChrSpecializationIndex)
					return false;

				if (condition.ChrSpecializationRole >= 0 && spec.Role != condition.ChrSpecializationRole)
					return false;
			}
		}

		bool[] results;

		if (condition.SkillID[0] != 0 || condition.SkillID[1] != 0 || condition.SkillID[2] != 0 || condition.SkillID[3] != 0)
		{
			results = new bool[condition.SkillID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.SkillID.Length; ++i)
				if (condition.SkillID[i] != 0)
				{
					var skillValue = player.GetSkillValue((SkillType)condition.SkillID[i]);
					results[i] = skillValue != 0 && skillValue > condition.MinSkill[i] && skillValue < condition.MaxSkill[i];
				}

			if (!PlayerConditionLogic(condition.SkillLogic, results))
				return false;
		}

		if (condition.LanguageID != 0)
		{
			var languageSkill = 0;

			if (player.HasAuraTypeWithMiscvalue(AuraType.ComprehendLanguage, condition.LanguageID))
				languageSkill = 300;
			else
				foreach (var languageDesc in Global.LanguageMgr.GetLanguageDescById((Language)condition.LanguageID))
					languageSkill = Math.Max(languageSkill, player.GetSkillValue((SkillType)languageDesc.SkillId));

			if (condition.MinLanguage != 0 && languageSkill < condition.MinLanguage)
				return false;

			if (condition.MaxLanguage != 0 && languageSkill > condition.MaxLanguage)
				return false;
		}

		if (condition.MinFactionID[0] != 0 && condition.MinFactionID[1] != 0 && condition.MinFactionID[2] != 0 && condition.MaxFactionID != 0)
		{
			if (condition.MinFactionID[0] == 0 && condition.MinFactionID[1] == 0 && condition.MinFactionID[2] == 0)
			{
				var forcedRank = player.ReputationMgr.GetForcedRankIfAny(condition.MaxFactionID);

				if (forcedRank != 0)
				{
					if ((uint)forcedRank > condition.MaxReputation)
						return false;
				}
				else if (CliDB.FactionStorage.HasRecord(condition.MaxReputation) && (uint)player.GetReputationRank(condition.MaxFactionID) > condition.MaxReputation)
				{
					return false;
				}
			}
			else
			{
				results = new bool[condition.MinFactionID.Length + 1];

				for (var i = 0; i < results.Length; ++i)
					results[i] = true;

				for (var i = 0; i < condition.MinFactionID.Length; ++i)
					if (CliDB.FactionStorage.HasRecord(condition.MinFactionID[i]))
					{
						var forcedRank = player.ReputationMgr.GetForcedRankIfAny(condition.MinFactionID[i]);

						if (forcedRank != 0)
							results[i] = (uint)forcedRank >= condition.MinReputation[i];
						else
							results[i] = (uint)player.GetReputationRank(condition.MinFactionID[i]) >= condition.MinReputation[i];
					}

				var forcedRank1 = player.ReputationMgr.GetForcedRankIfAny(condition.MaxFactionID);

				if (forcedRank1 != 0)
					results[3] = (uint)forcedRank1 <= condition.MaxReputation;
				else if (CliDB.FactionStorage.HasRecord(condition.MaxReputation))
					results[3] = (uint)player.GetReputationRank(condition.MaxFactionID) <= condition.MaxReputation;

				if (!PlayerConditionLogic(condition.ReputationLogic, results))
					return false;
			}
		}

		if (condition.CurrentPvpFaction != 0)
		{
			byte team;

			if (player.Map.IsBattlegroundOrArena)
				team = player.PlayerData.ArenaFaction;
			else
				team = (byte)player.TeamId;

			if (condition.CurrentPvpFaction - 1 != team)
				return false;
		}

		if (condition.PvpMedal != 0 && !Convert.ToBoolean((1 << (condition.PvpMedal - 1)) & player.ActivePlayerData.PvpMedals))
			return false;

		if (condition.LifetimeMaxPVPRank != 0 && player.ActivePlayerData.LifetimeMaxRank != condition.LifetimeMaxPVPRank)
			return false;

		if (condition.MovementFlags[0] != 0 && !Convert.ToBoolean((uint)player.GetUnitMovementFlags() & condition.MovementFlags[0]))
			return false;

		if (condition.MovementFlags[1] != 0 && !Convert.ToBoolean((uint)player.GetUnitMovementFlags2() & condition.MovementFlags[1]))
			return false;

		if (condition.WeaponSubclassMask != 0)
		{
			var mainHand = player.GetItemByPos(InventorySlots.Bag0, EquipmentSlot.MainHand);

			if (!mainHand || !Convert.ToBoolean((1 << (int)mainHand.Template.SubClass) & condition.WeaponSubclassMask))
				return false;
		}

		if (condition.PartyStatus != 0)
		{
			var group = player.Group;

			switch (condition.PartyStatus)
			{
				case 1:
					if (group)
						return false;

					break;
				case 2:
					if (!group)
						return false;

					break;
				case 3:
					if (!group || group.IsRaidGroup)
						return false;

					break;
				case 4:
					if (!group || !group.IsRaidGroup)
						return false;

					break;
				case 5:
					if (group && group.IsRaidGroup)
						return false;

					break;
				default:
					break;
			}
		}

		if (condition.PrevQuestID[0] != 0)
		{
			results = new bool[condition.PrevQuestID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.PrevQuestID.Length; ++i)
			{
				var questBit = Global.DB2Mgr.GetQuestUniqueBitFlag(condition.PrevQuestID[i]);

				if (questBit != 0)
					results[i] = (player.ActivePlayerData.QuestCompleted[((int)questBit - 1) >> 6] & (1ul << (((int)questBit - 1) & 63))) != 0;
			}

			if (!PlayerConditionLogic(condition.PrevQuestLogic, results))
				return false;
		}

		if (condition.CurrQuestID[0] != 0)
		{
			results = new bool[condition.CurrQuestID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.CurrQuestID.Length; ++i)
				if (condition.CurrQuestID[i] != 0)
					results[i] = player.FindQuestSlot(condition.CurrQuestID[i]) != SharedConst.MaxQuestLogSize;

			if (!PlayerConditionLogic(condition.CurrQuestLogic, results))
				return false;
		}

		if (condition.CurrentCompletedQuestID[0] != 0)
		{
			results = new bool[condition.CurrentCompletedQuestID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.CurrentCompletedQuestID.Length; ++i)
				if (condition.CurrentCompletedQuestID[i] != 0)
					results[i] = player.GetQuestStatus(condition.CurrentCompletedQuestID[i]) == QuestStatus.Complete;

			if (!PlayerConditionLogic(condition.CurrentCompletedQuestLogic, results))
				return false;
		}


		if (condition.SpellID[0] != 0)
		{
			results = new bool[condition.SpellID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.SpellID.Length; ++i)
				if (condition.SpellID[i] != 0)
					results[i] = player.HasSpell(condition.SpellID[i]);

			if (!PlayerConditionLogic(condition.SpellLogic, results))
				return false;
		}

		if (condition.ItemID[0] != 0)
		{
			results = new bool[condition.ItemID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.ItemID.Length; ++i)
				if (condition.ItemID[i] != 0)
					results[i] = player.GetItemCount(condition.ItemID[i], condition.ItemFlags != 0) >= condition.ItemCount[i];

			if (!PlayerConditionLogic(condition.ItemLogic, results))
				return false;
		}

		if (condition.CurrencyID[0] != 0)
		{
			results = new bool[condition.CurrencyID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.CurrencyID.Length; ++i)
				if (condition.CurrencyID[i] != 0)
					results[i] = player.GetCurrencyQuantity(condition.CurrencyID[i]) >= condition.CurrencyCount[i];

			if (!PlayerConditionLogic(condition.CurrencyLogic, results))
				return false;
		}

		if (condition.Explored[0] != 0 || condition.Explored[1] != 0)
			for (var i = 0; i < condition.Explored.Length; ++i)
			{
				var area = CliDB.AreaTableStorage.LookupByKey(condition.Explored[i]);

				if (area != null)
					if (area.AreaBit != -1 && !Convert.ToBoolean(player.ActivePlayerData.ExploredZones[area.AreaBit / ActivePlayerData.ExploredZonesBits] & (1ul << ((int)area.AreaBit % ActivePlayerData.ExploredZonesBits))))
						return false;
			}

		if (condition.AuraSpellID[0] != 0)
		{
			results = new bool[condition.AuraSpellID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.AuraSpellID.Length; ++i)
				if (condition.AuraSpellID[i] != 0)
				{
					if (condition.AuraStacks[i] != 0)
						results[i] = player.GetAuraCount(condition.AuraSpellID[i]) >= condition.AuraStacks[i];
					else
						results[i] = player.HasAura(condition.AuraSpellID[i]);
				}

			if (!PlayerConditionLogic(condition.AuraSpellLogic, results))
				return false;
		}

		if (condition.Time[0] != 0)
		{
			var from = Time.GetUnixTimeFromPackedTime(condition.Time[0]);
			var to = Time.GetUnixTimeFromPackedTime(condition.Time[1]);

			if (GameTime.GetGameTime() < from || GameTime.GetGameTime() > to)
				return false;
		}

		if (condition.WorldStateExpressionID != 0)
		{
			var worldStateExpression = CliDB.WorldStateExpressionStorage.LookupByKey(condition.WorldStateExpressionID);

			if (worldStateExpression == null)
				return false;

			if (!IsPlayerMeetingExpression(player, worldStateExpression))
				return false;
		}

		if (condition.WeatherID != 0)
			if (player.Map.GetZoneWeather(player.Zone) != (WeatherState)condition.WeatherID)
				return false;

		if (condition.Achievement[0] != 0)
		{
			results = new bool[condition.Achievement.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.Achievement.Length; ++i)
				if (condition.Achievement[i] != 0)
					// if (condition.Flags & 2) { any character on account completed it } else { current character only }
					// TODO: part of accountwide achievements
					results[i] = player.HasAchieved(condition.Achievement[i]);

			if (!PlayerConditionLogic(condition.AchievementLogic, results))
				return false;
		}

		if (condition.LfgStatus[0] != 0)
		{
			results = new bool[condition.LfgStatus.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.LfgStatus.Length; ++i)
				if (condition.LfgStatus[i] != 0)
					results[i] = PlayerConditionCompare(condition.LfgCompare[i], (int)GetPlayerConditionLfgValue(player, (PlayerConditionLfgStatus)condition.LfgStatus[i]), (int)condition.LfgValue[i]);

			if (!PlayerConditionLogic(condition.LfgLogic, results))
				return false;
		}

		if (condition.AreaID[0] != 0)
		{
			results = new bool[condition.AreaID.Length];

			for (var i = 0; i < results.Length; ++i)
				results[i] = true;

			for (var i = 0; i < condition.AreaID.Length; ++i)
				if (condition.AreaID[i] != 0)
					results[i] = player.Area == condition.AreaID[i] || player.Zone == condition.AreaID[i];

			if (!PlayerConditionLogic(condition.AreaLogic, results))
				return false;
		}

		if (condition.MinExpansionLevel != -1 && (int)player.Session.Expansion < condition.MinExpansionLevel)
			return false;

		if (condition.MaxExpansionLevel != -1 && (int)player.Session.Expansion > condition.MaxExpansionLevel)
			return false;

		if (condition.MinExpansionLevel != -1 && condition.MinExpansionTier != -1 && !player.IsGameMaster && ((condition.MinExpansionLevel == WorldConfig.GetIntValue(WorldCfg.Expansion) && condition.MinExpansionTier > 0) /*TODO: implement tier*/ || condition.MinExpansionLevel > WorldConfig.GetIntValue(WorldCfg.Expansion)))
			return false;

		if (condition.PhaseID != 0 || condition.PhaseGroupID != 0 || condition.PhaseUseFlags != 0)
			if (!PhasingHandler.InDbPhaseShift(player, (PhaseUseFlagsValues)condition.PhaseUseFlags, condition.PhaseID, condition.PhaseGroupID))
				return false;

		if (condition.QuestKillID != 0)
		{
			var quest = Global.ObjectMgr.GetQuestTemplate(condition.QuestKillID);
			var questSlot = player.FindQuestSlot(condition.QuestKillID);

			if (quest != null && player.GetQuestStatus(condition.QuestKillID) != QuestStatus.Complete && questSlot < SharedConst.MaxQuestLogSize)
			{
				results = new bool[condition.QuestKillMonster.Length];

				for (var i = 0; i < results.Length; ++i)
					results[i] = true;

				for (var i = 0; i < condition.QuestKillMonster.Length; ++i)
					if (condition.QuestKillMonster[i] != 0)
					{
						var questObjective = quest.Objectives.Find(objective => objective.Type == QuestObjectiveType.Monster && objective.ObjectID == condition.QuestKillMonster[i]);

						if (questObjective != null)
							results[i] = player.GetQuestSlotObjectiveData(questSlot, questObjective) >= questObjective.Amount;
					}

				if (!PlayerConditionLogic(condition.QuestKillLogic, results))
					return false;
			}
		}

		if (condition.MinAvgItemLevel != 0 && Math.Floor(player.PlayerData.AvgItemLevel[0]) < condition.MinAvgItemLevel)
			return false;

		if (condition.MaxAvgItemLevel != 0 && Math.Floor(player.PlayerData.AvgItemLevel[0]) > condition.MaxAvgItemLevel)
			return false;

		if (condition.MinAvgEquippedItemLevel != 0 && Math.Floor(player.PlayerData.AvgItemLevel[1]) < condition.MinAvgEquippedItemLevel)
			return false;

		if (condition.MaxAvgEquippedItemLevel != 0 && Math.Floor(player.PlayerData.AvgItemLevel[1]) > condition.MaxAvgEquippedItemLevel)
			return false;

		if (condition.ModifierTreeID != 0 && !player.ModifierTreeSatisfied(condition.ModifierTreeID))
			return false;

		if (condition.CovenantID != 0 && player.PlayerData.CovenantID != condition.CovenantID)
			return false;

		if (condition.TraitNodeEntryID.Any(traitNodeEntryId => traitNodeEntryId != 0))
		{
			var getTraitNodeEntryRank = ushort?(int traitNodeEntryId) =>
			{
				foreach (var traitConfig in player.ActivePlayerData.TraitConfigs)
				{
					if ((TraitConfigType)(int)traitConfig.Type == TraitConfigType.Combat)
						if (player.ActivePlayerData.ActiveCombatTraitConfigID != traitConfig.ID || !((TraitCombatConfigFlags)(int)traitConfig.CombatConfigFlags).HasFlag(TraitCombatConfigFlags.ActiveForSpec))
							continue;

					foreach (var traitEntry in traitConfig.Entries)
						if (traitEntry.TraitNodeEntryID == traitNodeEntryId)
							return (ushort)traitEntry.Rank;
				}

				return null;
			};

			results = new bool[condition.TraitNodeEntryID.Length];
			Array.Fill(results, true);

			for (var i = 0; i < condition.TraitNodeEntryID.Count(); ++i)
			{
				if (condition.TraitNodeEntryID[i] == 0)
					continue;

				var rank = getTraitNodeEntryRank(condition.TraitNodeEntryID[i]);

				if (!rank.HasValue)
					results[i] = false;
				else if (condition.TraitNodeEntryMinRank[i] != 0 && rank < condition.TraitNodeEntryMinRank[i])
					results[i] = false;
				else if (condition.TraitNodeEntryMaxRank[i] != 0 && rank > condition.TraitNodeEntryMaxRank[i])
					results[i] = false;
			}

			if (!PlayerConditionLogic(condition.TraitNodeEntryLogic, results))
				return false;
		}

		return true;
	}

	public static bool IsPlayerMeetingExpression(Player player, WorldStateExpressionRecord expression)
	{
		ByteBuffer buffer = new(expression.Expression.ToByteArray());

		if (buffer.GetSize() == 0)
			return false;

		var enabled = buffer.ReadBool();

		if (!enabled)
			return false;

		var finalResult = EvalRelOp(buffer, player);
		var resultLogic = (WorldStateExpressionLogic)buffer.ReadUInt8();

		while (resultLogic != WorldStateExpressionLogic.None)
		{
			var secondResult = EvalRelOp(buffer, player);

			switch (resultLogic)
			{
				case WorldStateExpressionLogic.And:
					finalResult = finalResult && secondResult;

					break;
				case WorldStateExpressionLogic.Or:
					finalResult = finalResult || secondResult;

					break;
				case WorldStateExpressionLogic.Xor:
					finalResult = finalResult != secondResult;

					break;
				default:
					break;
			}

			if (buffer.GetCurrentStream().Position < buffer.GetSize())
				break;

			resultLogic = (WorldStateExpressionLogic)buffer.ReadUInt8();
		}

		return finalResult;
	}

	public static bool IsUnitMeetingCondition(Unit unit, Unit otherUnit, UnitConditionRecord condition)
	{
		for (var i = 0; i < condition.Variable.Length; ++i)
		{
			if (condition.Variable[i] == 0)
				break;

			var unitValue = GetUnitConditionVariable(unit, otherUnit, (UnitConditionVariable)condition.Variable[i], condition.Value[i]);
			var meets = false;

			switch ((UnitConditionOp)condition.Op[i])
			{
				case UnitConditionOp.EqualTo:
					meets = unitValue == condition.Value[i];

					break;
				case UnitConditionOp.NotEqualTo:
					meets = unitValue != condition.Value[i];

					break;
				case UnitConditionOp.LessThan:
					meets = unitValue < condition.Value[i];

					break;
				case UnitConditionOp.LessThanOrEqualTo:
					meets = unitValue <= condition.Value[i];

					break;
				case UnitConditionOp.GreaterThan:
					meets = unitValue > condition.Value[i];

					break;
				case UnitConditionOp.GreaterThanOrEqualTo:
					meets = unitValue >= condition.Value[i];

					break;
				default:
					break;
			}

			if (condition.GetFlags().HasFlag(UnitConditionFlags.LogicOr))
			{
				if (meets)
					return true;
			}
			else if (!meets)
			{
				return false;
			}
		}

		return !condition.GetFlags().HasFlag(UnitConditionFlags.LogicOr);
	}

	bool CanHaveConditionType(ConditionSourceType sourceType, ConditionTypes conditionType)
	{
		switch (sourceType)
		{
			case ConditionSourceType.SpawnGroup:
				switch (conditionType)
				{
					case ConditionTypes.None:
					case ConditionTypes.ActiveEvent:
					case ConditionTypes.InstanceInfo:
					case ConditionTypes.Mapid:
					case ConditionTypes.WorldState:
					case ConditionTypes.RealmAchievement:
					case ConditionTypes.DifficultyId:
					case ConditionTypes.ScenarioStep:
						return true;
					default:
						return false;
				}
			default:
				break;
		}

		return true;
	}

	bool AddToLootTemplate(Condition cond, LootTemplate loot)
	{
		if (loot == null)
		{
			Log.outDebug(LogFilter.Sql, "{0} LootTemplate {1} not found.", cond.ToString(), cond.SourceGroup);

			return false;
		}

		if (loot.AddConditionItem(cond))
			return true;

		Log.outDebug(LogFilter.Sql, "{0} Item {1} not found in LootTemplate {2}.", cond.ToString(), cond.SourceEntry, cond.SourceGroup);

		return false;
	}

	bool AddToGossipMenus(Condition cond)
	{
		var pMenuBounds = Global.ObjectMgr.GetGossipMenusMapBounds(cond.SourceGroup);

		foreach (var menu in pMenuBounds)
			if (menu.MenuId == cond.SourceGroup && menu.TextId == cond.SourceEntry)
			{
				menu.Conditions.Add(cond);

				return true;
			}

		Log.outDebug(LogFilter.Sql, "{0} GossipMenu {1} not found.", cond.ToString(), cond.SourceGroup);

		return false;
	}

	bool AddToGossipMenuItems(Condition cond)
	{
		var pMenuItemBounds = Global.ObjectMgr.GetGossipMenuItemsMapBounds(cond.SourceGroup);

		foreach (var gossipMenuItem in pMenuItemBounds)
			if (gossipMenuItem.MenuId == cond.SourceGroup && gossipMenuItem.OrderIndex == cond.SourceEntry)
			{
				gossipMenuItem.Conditions.Add(cond);

				return true;
			}

		Log.outDebug(LogFilter.Sql, "{0} GossipMenuId {1} Item {2} not found.", cond.ToString(), cond.SourceGroup, cond.SourceEntry);

		return false;
	}

	bool AddToSpellImplicitTargetConditions(Condition cond)
	{
		Global.SpellMgr.ForEachSpellInfoDifficulty((uint)cond.SourceEntry,
													spellInfo =>
													{
														var conditionEffMask = cond.SourceGroup;
														List<uint> sharedMasks = new();

														foreach (var spellEffectInfo in spellInfo.Effects)
														{
															// additional checks by condition type
															if ((conditionEffMask & (1 << spellEffectInfo.EffectIndex)) != 0)
																switch (cond.ConditionType)
																{
																	case ConditionTypes.ObjectEntryGuid:
																	{
																		var implicitTargetMask = SpellInfo.GetTargetFlagMask(spellEffectInfo.TargetA.ObjectType) | SpellInfo.GetTargetFlagMask(spellEffectInfo.TargetB.ObjectType);

																		if (implicitTargetMask.HasFlag(SpellCastTargetFlags.UnitMask) && cond.ConditionValue1 != (uint)TypeId.Unit && cond.ConditionValue1 != (uint)TypeId.Player)
																		{
																			Log.outDebug(LogFilter.Sql, $"{cond} in `condition` table - spell {spellInfo.Id} EFFECT_{spellEffectInfo.EffectIndex} - target requires ConditionValue1 to be either TYPEID_UNIT ({(uint)TypeId.Unit}) or TYPEID_PLAYER ({(uint)TypeId.Player})");

																			return;
																		}

																		if (implicitTargetMask.HasFlag(SpellCastTargetFlags.GameobjectMask) && cond.ConditionValue1 != (uint)TypeId.GameObject)
																		{
																			Log.outDebug(LogFilter.Sql, $"{cond} in `condition` table - spell {spellInfo.Id} EFFECT_{spellEffectInfo.EffectIndex} - target requires ConditionValue1 to be TYPEID_GAMEOBJECT ({(uint)TypeId.GameObject})");

																			return;
																		}

																		if (implicitTargetMask.HasFlag(SpellCastTargetFlags.CorpseMask) && cond.ConditionValue1 != (uint)TypeId.Corpse)
																		{
																			Log.outDebug(LogFilter.Sql, $"{cond} in `condition` table - spell {spellInfo.Id} EFFECT_{spellEffectInfo.EffectIndex} - target requires ConditionValue1 to be TYPEID_CORPSE ({(uint)TypeId.Corpse})");

																			return;
																		}

																		break;
																	}
																	default:
																		break;
																}

															// check if effect is already a part of some shared mask
															if (sharedMasks.Any(mask => !!Convert.ToBoolean(mask & (1 << spellEffectInfo.EffectIndex))))
																continue;

															// build new shared mask with found effect
															var sharedMask = (uint)(1 << spellEffectInfo.EffectIndex);
															var cmp = spellEffectInfo.ImplicitTargetConditions;

															for (var effIndex = spellEffectInfo.EffectIndex + 1; effIndex < spellInfo.Effects.Count; ++effIndex)
																if (spellInfo.GetEffect(effIndex).ImplicitTargetConditions == cmp)
																	sharedMask |= (uint)(1 << effIndex);

															sharedMasks.Add(sharedMask);
														}

														foreach (var effectMask in sharedMasks)
														{
															// some effect indexes should have same data
															var commonMask = (effectMask & conditionEffMask);

															if (commonMask != 0)
															{
																byte firstEffIndex = 0;
																var effectCount = spellInfo.Effects.Count;

																for (; firstEffIndex < effectCount; ++firstEffIndex)
																	if (((1 << firstEffIndex) & effectMask) != 0)
																		break;

																if (firstEffIndex >= effectCount)
																	return;

																// get shared data
																var sharedList = spellInfo.GetEffect(firstEffIndex).ImplicitTargetConditions;

																// there's already data entry for that sharedMask
																if (sharedList != null)
																{
																	// we have overlapping masks in db
																	if (conditionEffMask != effectMask)
																	{
																		Log.outDebug(LogFilter.Sql,
																					"{0} in `condition` table, has incorrect SourceGroup {1} (spell effectMask) set - " +
																					"effect masks are overlapping (all SourceGroup values having given bit set must be equal) - ignoring.",
																					cond.ToString(),
																					cond.SourceGroup);

																		return;
																	}
																}
																// no data for shared mask, we can create new submask
																else
																{
																	// add new list, create new shared mask
																	sharedList = new List<Condition>();
																	var assigned = false;

																	for (int i = firstEffIndex; i < effectCount; ++i)
																		if (((1 << i) & commonMask) != 0)
																		{
																			spellInfo.GetEffect(i).ImplicitTargetConditions = sharedList;
																			assigned = true;
																		}

																	if (!assigned)
																		break;
																}

																sharedList.Add(cond);

																break;
															}
														}
													});

		return true;
	}

	bool AddToPhases(Condition cond)
	{
		if (cond.SourceEntry == 0)
		{
			var phaseInfo = Global.ObjectMgr.GetPhaseInfo(cond.SourceGroup);

			if (phaseInfo != null)
			{
				var found = false;

				foreach (var areaId in phaseInfo.Areas)
				{
					var phases = Global.ObjectMgr.GetPhasesForArea(areaId);

					if (phases != null)
						foreach (var phase in phases)
							if (phase.PhaseInfo.Id == cond.SourceGroup)
							{
								phase.Conditions.Add(cond);
								found = true;
							}
				}

				if (found)
					return true;
			}
		}
		else
		{
			var phases = Global.ObjectMgr.GetPhasesForArea((uint)cond.SourceEntry);

			foreach (var phase in phases)
				if (phase.PhaseInfo.Id == cond.SourceGroup)
				{
					phase.Conditions.Add(cond);

					return true;
				}
		}

		Log.outDebug(LogFilter.Sql, "{0} Area {1} does not have phase {2}.", cond.ToString(), cond.SourceGroup, cond.SourceEntry);

		return false;
	}

	bool IsSourceTypeValid(Condition cond)
	{
		switch (cond.SourceType)
		{
			case ConditionSourceType.CreatureLootTemplate:
			{
				if (!LootStorage.Creature.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `creature_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Creature.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, Item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.DisenchantLootTemplate:
			{
				if (!LootStorage.Disenchant.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `disenchant_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Disenchant.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.FishingLootTemplate:
			{
				if (!LootStorage.Fishing.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `fishing_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Fishing.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.GameobjectLootTemplate:
			{
				if (!LootStorage.Gameobject.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `gameobject_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Gameobject.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.ItemLootTemplate:
			{
				if (!LootStorage.Items.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `item_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Items.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.MailLootTemplate:
			{
				if (!LootStorage.Mail.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `mail_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Mail.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.MillingLootTemplate:
			{
				if (!LootStorage.Milling.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `milling_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Milling.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.PickpocketingLootTemplate:
			{
				if (!LootStorage.Pickpocketing.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `pickpocketing_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Pickpocketing.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.ProspectingLootTemplate:
			{
				if (!LootStorage.Prospecting.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `prospecting_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Prospecting.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.ReferenceLootTemplate:
			{
				if (!LootStorage.Reference.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `reference_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Reference.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.SkinningLootTemplate:
			{
				if (!LootStorage.Skinning.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `skinning_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Skinning.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.SpellLootTemplate:
			{
				if (!LootStorage.Spell.HaveLootFor(cond.SourceGroup))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table, does not exist in `spell_loot_template`, ignoring.", cond.ToString());

					return false;
				}

				var loot = LootStorage.Spell.GetLootForConditionFill(cond.SourceGroup);
				var pItemProto = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (pItemProto == null && !loot.IsReference((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceType, SourceEntry in `condition` table, item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.SpellImplicitTarget:
			{
				var spellInfo = Global.SpellMgr.GetSpellInfo((uint)cond.SourceEntry, Difficulty.None);

				if (spellInfo == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in `spell.db2`, ignoring.", cond.ToString());

					return false;
				}

				if ((cond.SourceGroup > SpellConst.MAX_EFFECT_MASK) || cond.SourceGroup == 0)
				{
					Log.outDebug(LogFilter.Sql, "{0} in `condition` table, has incorrect SourceGroup (spell effectMask) set, ignoring.", cond.ToString());

					return false;
				}

				var origGroup = cond.SourceGroup;

				foreach (var spellEffectInfo in spellInfo.Effects)
				{
					if (((1 << spellEffectInfo.EffectIndex) & cond.SourceGroup) == 0)
						continue;

					if (spellEffectInfo.ChainTargets > 0)
						continue;

					switch (spellEffectInfo.TargetA.SelectionCategory)
					{
						case SpellTargetSelectionCategories.Nearby:
						case SpellTargetSelectionCategories.Cone:
						case SpellTargetSelectionCategories.Area:
						case SpellTargetSelectionCategories.Traj:
						case SpellTargetSelectionCategories.Line:
							continue;
						default:
							break;
					}

					switch (spellEffectInfo.TargetB.SelectionCategory)
					{
						case SpellTargetSelectionCategories.Nearby:
						case SpellTargetSelectionCategories.Cone:
						case SpellTargetSelectionCategories.Area:
						case SpellTargetSelectionCategories.Traj:
						case SpellTargetSelectionCategories.Line:
							continue;
						default:
							break;
					}

					switch (spellEffectInfo.Effect)
					{
						case SpellEffectName.PersistentAreaAura:
						case SpellEffectName.ApplyAreaAuraParty:
						case SpellEffectName.ApplyAreaAuraRaid:
						case SpellEffectName.ApplyAreaAuraFriend:
						case SpellEffectName.ApplyAreaAuraEnemy:
						case SpellEffectName.ApplyAreaAuraPet:
						case SpellEffectName.ApplyAreaAuraOwner:
						case SpellEffectName.ApplyAuraOnPet:
						case SpellEffectName.ApplyAreaAuraSummons:
						case SpellEffectName.ApplyAreaAuraPartyNonrandom:
							continue;
						default:
							break;
					}

					Log.outDebug(LogFilter.Sql, "SourceEntry {0} SourceGroup {1} in `condition` table - spell {2} does not have implicit targets of types: _AREA_, _CONE_, _NEARBY_, _CHAIN_ for effect {3}, SourceGroup needs correction, ignoring.", cond.SourceEntry, origGroup, cond.SourceEntry, spellEffectInfo.EffectIndex);
					cond.SourceGroup &= ~(1u << spellEffectInfo.EffectIndex);
				}

				// all effects were removed, no need to add the condition at all
				if (cond.SourceGroup == 0)
					return false;

				break;
			}
			case ConditionSourceType.CreatureTemplateVehicle:
			{
				if (Global.ObjectMgr.GetCreatureTemplate((uint)cond.SourceEntry) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in `creature_template`, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.Spell:
			case ConditionSourceType.SpellProc:
			{
				var spellProto = Global.SpellMgr.GetSpellInfo((uint)cond.SourceEntry, Difficulty.None);

				if (spellProto == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in `spell.db2`, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.QuestAvailable:
				if (Global.ObjectMgr.GetQuestTemplate((uint)cond.SourceEntry) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry specifies non-existing quest, skipped.", cond.ToString());

					return false;
				}

				break;
			case ConditionSourceType.VehicleSpell:
				if (Global.ObjectMgr.GetCreatureTemplate(cond.SourceGroup) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table does not exist in `creature_template`, ignoring.", cond.ToString());

					return false;
				}

				if (!Global.SpellMgr.HasSpellInfo((uint)cond.SourceEntry, Difficulty.None))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in `spell.db2`, ignoring.", cond.ToString());

					return false;
				}

				break;
			case ConditionSourceType.SpellClickEvent:
				if (Global.ObjectMgr.GetCreatureTemplate(cond.SourceGroup) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table does not exist in `creature_template`, ignoring.", cond.ToString());

					return false;
				}

				if (!Global.SpellMgr.HasSpellInfo((uint)cond.SourceEntry, Difficulty.None))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in `spell.db2`, ignoring.", cond.ToString());

					return false;
				}

				break;
			case ConditionSourceType.NpcVendor:
			{
				if (Global.ObjectMgr.GetCreatureTemplate(cond.SourceGroup) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceGroup in `condition` table does not exist in `creature_template`, ignoring.", cond.ToString());

					return false;
				}

				var itemTemplate = Global.ObjectMgr.GetItemTemplate((uint)cond.SourceEntry);

				if (itemTemplate == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table item does not exist, ignoring.", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionSourceType.TerrainSwap:
				if (!CliDB.MapStorage.ContainsKey((uint)cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in Map.db2, ignoring.", cond.ToString());

					return false;
				}

				break;
			case ConditionSourceType.Phase:
				if (cond.SourceEntry != 0 && !CliDB.AreaTableStorage.ContainsKey(cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, "{0} SourceEntry in `condition` table does not exist in AreaTable.db2, ignoring.", cond.ToString());

					return false;
				}

				break;
			case ConditionSourceType.GossipMenu:
			case ConditionSourceType.GossipMenuOption:
			case ConditionSourceType.SmartEvent:
				break;
			case ConditionSourceType.Graveyard:
				if (Global.ObjectMgr.GetWorldSafeLoc((uint)cond.SourceEntry) == null)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceEntry in `condition` table, does not exist in WorldSafeLocs.db2, ignoring.");

					return false;
				}

				break;
			case ConditionSourceType.AreaTrigger:
				if (cond.SourceEntry != 0 && cond.SourceEntry != 1)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} in `condition` table, unexpected SourceEntry value (expected 0 or 1), ignoring.");

					return false;
				}

				if (Global.AreaTriggerDataStorage.GetAreaTriggerTemplate(new AreaTriggerId(cond.SourceGroup, cond.SourceEntry != 0)) == null)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} in `condition` table, does not exist in `areatrigger_template`, ignoring.");

					return false;
				}

				break;
			case ConditionSourceType.ConversationLine:
				if (Global.ConversationDataStorage.GetConversationLineTemplate((uint)cond.SourceEntry) == null)
				{
					Log.outDebug(LogFilter.Sql, $"{cond} does not exist in `conversation_line_template`, ignoring.");

					return false;
				}

				break;
			case ConditionSourceType.AreatriggerClientTriggered:
				if (!CliDB.AreaTriggerStorage.ContainsKey(cond.SourceEntry))
				{
					Log.outDebug(LogFilter.Sql, $"{cond} SourceEntry in `condition` table, does not exists in AreaTrigger.db2, ignoring.");

					return false;
				}

				break;
			case ConditionSourceType.TrainerSpell:
			{
				if (Global.ObjectMgr.GetTrainer(cond.SourceGroup) == null)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceGroup in `condition` table, does not exist in `trainer`, ignoring.");

					return false;
				}

				if (Global.SpellMgr.GetSpellInfo((uint)cond.SourceEntry, Difficulty.None) == null)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceEntry in `condition` table does not exist in `Spell.db2`, ignoring.");

					return false;
				}

				break;
			}
			case ConditionSourceType.ObjectIdVisibility:
			{
				if (cond.SourceGroup <= 0 || cond.SourceGroup >= (uint)TypeId.Max)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceGroup in `condition` table, is no valid object type, ignoring.");

					return false;
				}

				if (cond.SourceGroup == (uint)TypeId.Unit)
				{
					if (Global.ObjectMgr.GetCreatureTemplate((uint)cond.SourceEntry) == null)
					{
						Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceEntry in `condition` table, does not exist in `creature_template`, ignoring.");

						return false;
					}
				}
				else if (cond.SourceGroup == (uint)TypeId.GameObject)
				{
					if (Global.ObjectMgr.GetGameObjectTemplate((uint)cond.SourceEntry) == null)
					{
						Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceEntry in `condition` table, does not exist in `gameobject_template`, ignoring.");

						return false;
					}
				}
				else
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceGroup in `condition` table, uses unchecked type id, ignoring.");

					return false;
				}

				break;
			}
			case ConditionSourceType.SpawnGroup:
			{
				var spawnGroup = Global.ObjectMgr.GetSpawnGroupData((uint)cond.SourceEntry);

				if (spawnGroup == null)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} SourceEntry in `condition` table, does not exist in `spawn_group_template`, ignoring.");

					return false;
				}

				if (spawnGroup.Flags.HasAnyFlag(SpawnGroupFlags.System))
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString()} in `spawn_group_template` table cannot have SPAWNGROUP_FLAG_SYSTEM or SPAWNGROUP_FLAG_MANUAL_SPAWN flags, ignoring.");

					return false;
				}

				break;
			}
			default:
				Log.outDebug(LogFilter.Sql, $"{cond.ToString()} Invalid ConditionSourceType in `condition` table, ignoring.");

				return false;
		}

		return true;
	}

	bool IsConditionTypeValid(Condition cond)
	{
		switch (cond.ConditionType)
		{
			case ConditionTypes.Aura:
			{
				if (!Global.SpellMgr.HasSpellInfo(cond.ConditionValue1, Difficulty.None))
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing spell (Id: {1}), skipped", cond.ToString(), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Item:
			{
				var proto = Global.ObjectMgr.GetItemTemplate(cond.ConditionValue1);

				if (proto == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} Item ({1}) does not exist, skipped", cond.ToString(), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue2 == 0)
				{
					Log.outDebug(LogFilter.Sql, "{0} Zero item count in ConditionValue2, skipped", cond.ToString());

					return false;
				}

				break;
			}
			case ConditionTypes.ItemEquipped:
			{
				var proto = Global.ObjectMgr.GetItemTemplate(cond.ConditionValue1);

				if (proto == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} Item ({1}) does not exist, skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Zoneid:
			{
				var areaEntry = CliDB.AreaTableStorage.LookupByKey(cond.ConditionValue1);

				if (areaEntry == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} Area ({1}) does not exist, skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (areaEntry.ParentAreaID != 0)
				{
					Log.outDebug(LogFilter.Sql, "{0} requires to be in area ({1}) which is a subzone but zone expected, skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.ReputationRank:
			{
				if (!CliDB.FactionStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing faction ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Team:
			{
				if (cond.ConditionValue1 != (uint)TeamFaction.Alliance && cond.ConditionValue1 != (uint)TeamFaction.Horde)
				{
					Log.outDebug(LogFilter.Sql, "{0} specifies unknown team ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Skill:
			{
				var pSkill = CliDB.SkillLineStorage.LookupByKey(cond.ConditionValue1);

				if (pSkill == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} specifies non-existing skill ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue2 < 1 || cond.ConditionValue2 > Global.WorldMgr.ConfigMaxSkillValue)
				{
					Log.outDebug(LogFilter.Sql, "{0} specifies skill ({1}) with invalid value ({1}), skipped.", cond.ToString(true), cond.ConditionValue1, cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.Queststate:
				if (cond.ConditionValue2 >= (1 << (int)QuestStatus.Max))
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid state mask ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				if (Global.ObjectMgr.GetQuestTemplate(cond.ConditionValue1) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} points to non-existing quest ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			case ConditionTypes.QuestRewarded:
			case ConditionTypes.QuestTaken:
			case ConditionTypes.QuestNone:
			case ConditionTypes.QuestComplete:
			case ConditionTypes.DailyQuestDone:
			{
				if (Global.ObjectMgr.GetQuestTemplate(cond.ConditionValue1) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} points to non-existing quest ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.ActiveEvent:
			{
				var events = Global.GameEventMgr.GetEventMap();

				if (cond.ConditionValue1 >= events.Length || !events[cond.ConditionValue1].IsValid())
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing event id ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Achievement:
			{
				var achievement = CliDB.AchievementStorage.LookupByKey(cond.ConditionValue1);

				if (achievement == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing achivement id ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Class:
			{
				if (Convert.ToBoolean(cond.ConditionValue1 & ~(uint)PlayerClass.ClassMaskAllPlayable))
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing classmask ({1}), skipped.", cond.ToString(true), cond.ConditionValue1 & ~(uint)PlayerClass.ClassMaskAllPlayable);

					return false;
				}

				break;
			}
			case ConditionTypes.Race:
			{
				if (Convert.ToBoolean(cond.ConditionValue1 & ~SharedConst.RaceMaskAllPlayable))
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing racemask ({1}), skipped.", cond.ToString(true), cond.ConditionValue1 & ~SharedConst.RaceMaskAllPlayable);

					return false;
				}

				break;
			}
			case ConditionTypes.Gender:
			{
				if (!Player.IsValidGender((Gender)cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid gender ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Mapid:
			{
				var me = CliDB.MapStorage.LookupByKey(cond.ConditionValue1);

				if (me == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing map ({1}), skipped", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Spell:
			{
				if (!Global.SpellMgr.HasSpellInfo(cond.ConditionValue1, Difficulty.None))
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing spell (Id: {1}), skipped", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Level:
			{
				if (cond.ConditionValue2 >= (uint)ComparisionType.Max)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ComparisionType ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.DrunkenState:
			{
				if (cond.ConditionValue1 > (uint)DrunkenState.Smashed)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid state ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.NearCreature:
			{
				if (Global.ObjectMgr.GetCreatureTemplate(cond.ConditionValue1) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing creature template entry ({1}), skipped", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.NearGameobject:
			{
				if (Global.ObjectMgr.GetGameObjectTemplate(cond.ConditionValue1) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing gameobject template entry ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.ObjectEntryGuid:
			{
				switch ((TypeId)cond.ConditionValue1)
				{
					case TypeId.Unit:
						if (cond.ConditionValue2 != 0 && Global.ObjectMgr.GetCreatureTemplate(cond.ConditionValue2) == null)
						{
							Log.outDebug(LogFilter.Sql, "{0} has non existing creature template entry ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

							return false;
						}

						if (cond.ConditionValue3 != 0)
						{
							var creatureData = Global.ObjectMgr.GetCreatureData(cond.ConditionValue3);

							if (creatureData != null)
							{
								if (cond.ConditionValue2 != 0 && creatureData.Id != cond.ConditionValue2)
								{
									Log.outDebug(LogFilter.Sql, "{0} has guid {1} set but does not match creature entry ({1}), skipped.", cond.ToString(true), cond.ConditionValue3, cond.ConditionValue2);

									return false;
								}
							}
							else
							{
								Log.outDebug(LogFilter.Sql, "{0} has non existing creature guid ({1}), skipped.", cond.ToString(true), cond.ConditionValue3);

								return false;
							}
						}

						break;
					case TypeId.GameObject:
						if (cond.ConditionValue2 != 0 && Global.ObjectMgr.GetGameObjectTemplate(cond.ConditionValue2) == null)
						{
							Log.outDebug(LogFilter.Sql, "{0} has non existing gameobject template entry ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

							return false;
						}

						if (cond.ConditionValue3 != 0)
						{
							var goData = Global.ObjectMgr.GetGameObjectData(cond.ConditionValue3);

							if (goData != null)
							{
								if (cond.ConditionValue2 != 0 && goData.Id != cond.ConditionValue2)
								{
									Log.outDebug(LogFilter.Sql, "{0} has guid {1} set but does not match gameobject entry ({1}), skipped.", cond.ToString(true), cond.ConditionValue3, cond.ConditionValue2);

									return false;
								}
							}
							else
							{
								Log.outDebug(LogFilter.Sql, "{0} has non existing gameobject guid ({1}), skipped.", cond.ToString(true), cond.ConditionValue3);

								return false;
							}
						}

						break;
					case TypeId.Player:
					case TypeId.Corpse:
						if (cond.ConditionValue2 != 0)
							LogUselessConditionValue(cond, 2, cond.ConditionValue2);

						if (cond.ConditionValue3 != 0)
							LogUselessConditionValue(cond, 3, cond.ConditionValue3);

						break;
					default:
						Log.outDebug(LogFilter.Sql, "{0} has wrong typeid set ({1}), skipped", cond.ToString(true), cond.ConditionValue1);

						return false;
				}

				break;
			}
			case ConditionTypes.TypeMask:
			{
				if (cond.ConditionValue1 == 0 || Convert.ToBoolean(cond.ConditionValue1 & ~(uint)(TypeMask.Unit | TypeMask.Player | TypeMask.GameObject | TypeMask.Corpse)))
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid typemask set ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.RelationTo:
			{
				if (cond.ConditionValue1 >= cond.GetMaxAvailableConditionTargets())
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ConditionValue1(ConditionTarget selection) ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue1 == cond.ConditionTarget)
				{
					Log.outDebug(LogFilter.Sql, "{0} has ConditionValue1(ConditionTarget selection) set to self ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue2 >= (uint)RelationType.Max)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ConditionValue2(RelationType) ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.ReactionTo:
			{
				if (cond.ConditionValue1 >= cond.GetMaxAvailableConditionTargets())
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ConditionValue1(ConditionTarget selection) ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue1 == cond.ConditionTarget)
				{
					Log.outDebug(LogFilter.Sql, "{0} has ConditionValue1(ConditionTarget selection) set to self ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue2 == 0)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ConditionValue2(rankMask) ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.DistanceTo:
			{
				if (cond.ConditionValue1 >= cond.GetMaxAvailableConditionTargets())
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ConditionValue1(ConditionTarget selection) ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue1 == cond.ConditionTarget)
				{
					Log.outDebug(LogFilter.Sql, "{0} has ConditionValue1(ConditionTarget selection) set to self ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue3 >= (uint)ComparisionType.Max)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ComparisionType ({1}), skipped.", cond.ToString(true), cond.ConditionValue3);

					return false;
				}

				break;
			}
			case ConditionTypes.HpVal:
			{
				if (cond.ConditionValue2 >= (uint)ComparisionType.Max)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ComparisionType ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.HpPct:
			{
				if (cond.ConditionValue1 > 100)
				{
					Log.outDebug(LogFilter.Sql, "{0} has too big percent value ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				if (cond.ConditionValue2 >= (uint)ComparisionType.Max)
				{
					Log.outDebug(LogFilter.Sql, "{0} has invalid ComparisionType ({1}), skipped.", cond.ToString(true), cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.WorldState:
			{
				if (Global.WorldStateMgr.GetWorldStateTemplate((int)cond.ConditionValue1) == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing world state in value1 ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.PhaseId:
			{
				if (!CliDB.PhaseStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, "{0} has nonexistent phaseid in value1 ({1}), skipped", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.Title:
			{
				var titleEntry = CliDB.CharTitlesStorage.LookupByKey(cond.ConditionValue1);

				if (titleEntry == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing title in value1 ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.SpawnmaskDeprecated:
			{
				Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} using deprecated condition type CONDITION_SPAWNMASK.");

				return false;
			}
			case ConditionTypes.UnitState:
			{
				if (cond.ConditionValue1 > (uint)UnitState.AllStateSupported)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing UnitState in value1 ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.CreatureType:
			{
				if (cond.ConditionValue1 == 0 || cond.ConditionValue1 > (uint)CreatureType.GasCloud)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing CreatureType in value1 ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.RealmAchievement:
			{
				var achievement = CliDB.AchievementStorage.LookupByKey(cond.ConditionValue1);

				if (achievement == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non existing realm first achivement id ({1}), skipped.", cond.ToString(), cond.ConditionValue1);

					return false;
				}

				break;
			}
			case ConditionTypes.StandState:
			{
				bool valid;

				switch (cond.ConditionValue1)
				{
					case 0:
						valid = cond.ConditionValue2 <= (uint)UnitStandStateType.Submerged;

						break;
					case 1:
						valid = cond.ConditionValue2 <= 1;

						break;
					default:
						valid = false;

						break;
				}

				if (!valid)
				{
					Log.outDebug(LogFilter.Sql, "{0} has non-existing stand state ({1},{2}), skipped.", cond.ToString(true), cond.ConditionValue1, cond.ConditionValue2);

					return false;
				}

				break;
			}
			case ConditionTypes.ObjectiveProgress:
			{
				var obj = Global.ObjectMgr.GetQuestObjective(cond.ConditionValue1);

				if (obj == null)
				{
					Log.outDebug(LogFilter.Sql, "{0} points to non-existing quest objective ({1}), skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				var limit = obj.IsStoringFlag() ? 1 : obj.Amount;

				if (cond.ConditionValue3 > limit)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has quest objective count {cond.ConditionValue3} in value3, but quest objective {cond.ConditionValue1} has a maximum objective count of {limit}, skipped.");

					return false;
				}

				break;
			}
			case ConditionTypes.PetType:
				if (cond.ConditionValue1 >= (1 << (int)PetType.Max))
				{
					Log.outDebug(LogFilter.Sql, "{0} has non-existing pet type {1}, skipped.", cond.ToString(true), cond.ConditionValue1);

					return false;
				}

				break;
			case ConditionTypes.Alive:
			case ConditionTypes.Areaid:
			case ConditionTypes.InstanceInfo:
			case ConditionTypes.TerrainSwap:
			case ConditionTypes.InWater:
			case ConditionTypes.Charmed:
			case ConditionTypes.Taxi:
			case ConditionTypes.Gamemaster:
				break;
			case ConditionTypes.DifficultyId:
				if (!CliDB.DifficultyStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has non existing difficulty in value1 ({cond.ConditionValue1}), skipped.");

					return false;
				}

				break;
			case ConditionTypes.BattlePetCount:
				if (!CliDB.BattlePetSpeciesStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has non existing BattlePet SpeciesId in value1 ({cond.ConditionValue1}), skipped.");

					return false;
				}

				if (cond.ConditionValue2 > SharedConst.DefaultMaxBattlePetsPerSpecies)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has invalid (greater than {SharedConst.DefaultMaxBattlePetsPerSpecies}) value2 ({cond.ConditionValue2}), skipped.");

					return false;
				}

				if (cond.ConditionValue3 >= (uint)ComparisionType.Max)
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has invalid ComparisionType ({cond.ConditionValue3}), skipped.");

					return false;
				}

				break;
			case ConditionTypes.ScenarioStep:
			{
				if (!CliDB.ScenarioStepStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has non existing ScenarioStep in value1 ({cond.ConditionValue1}), skipped.");

					return false;
				}

				break;
			}
			case ConditionTypes.SceneInProgress:
			{
				if (!CliDB.SceneScriptPackageStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has non existing SceneScriptPackageId in value1 ({cond.ConditionValue1}), skipped.");

					return false;
				}

				break;
			}
			case ConditionTypes.PlayerCondition:
			{
				if (!CliDB.PlayerConditionStorage.ContainsKey(cond.ConditionValue1))
				{
					Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} has non existing PlayerConditionId in value1 ({cond.ConditionValue1}), skipped.");

					return false;
				}

				break;
			}
			default:
				Log.outDebug(LogFilter.Sql, $"{cond.ToString()} Invalid ConditionType in `condition` table, ignoring.");

				return false;
		}

		if (cond.ConditionTarget >= cond.GetMaxAvailableConditionTargets())
		{
			Log.outDebug(LogFilter.Sql, $"{cond.ToString(true)} in `condition` table, has incorrect ConditionTarget set, ignoring.");

			return false;
		}

		if (cond.ConditionValue1 != 0 && !StaticConditionTypeData[(int)cond.ConditionType].HasConditionValue1)
			LogUselessConditionValue(cond, 1, cond.ConditionValue1);

		if (cond.ConditionValue2 != 0 && !StaticConditionTypeData[(int)cond.ConditionType].HasConditionValue2)
			LogUselessConditionValue(cond, 2, cond.ConditionValue2);

		if (cond.ConditionValue3 != 0 && !StaticConditionTypeData[(int)cond.ConditionType].HasConditionValue3)
			LogUselessConditionValue(cond, 3, cond.ConditionValue3);

		return true;
	}

	void LogUselessConditionValue(Condition cond, byte index, uint value)
	{
		Log.outDebug(LogFilter.Sql, "{0} has useless data in ConditionValue{1} ({2})!", cond.ToString(true), index, value);
	}

	void Clean()
	{
		conditionReferenceStorage.Clear();

		conditionStorage.Clear();

		for (ConditionSourceType i = 0; i < ConditionSourceType.Max; ++i)
			conditionStorage[i] = new MultiMap<uint, Condition>(); //add new empty list for SourceType

		vehicleSpellConditionStorage.Clear();

		smartEventConditionStorage.Clear();

		spellClickEventConditionStorage.Clear();
		spellsUsedInSpellClickConditions.Clear();

		npcVendorConditionContainerStorage.Clear();

		areaTriggerConditionContainerStorage.Clear();

		trainerSpellConditionContainerStorage.Clear();

		objectVisibilityConditionStorage.Clear();
	}

	static bool PlayerConditionCompare(int comparisonType, int value1, int value2)
	{
		switch (comparisonType)
		{
			case 1:
				return value1 == value2;
			case 2:
				return value1 != value2;
			case 3:
				return value1 > value2;
			case 4:
				return value1 >= value2;
			case 5:
				return value1 < value2;
			case 6:
				return value1 <= value2;
			default:
				break;
		}

		return false;
	}

	static bool PlayerConditionLogic(uint logic, bool[] results)
	{
		for (var i = 0; i < results.Length; ++i)
			if (Convert.ToBoolean((logic >> (16 + i)) & 1))
				results[i] ^= true;

		var result = results[0];

		for (var i = 1; i < results.Length; ++i)
			switch ((logic >> (2 * (i - 1))) & 3)
			{
				case 1:
					result = result && results[i];

					break;
				case 2:
					result = result || results[i];

					break;
				default:
					break;
			}

		return result;
	}

	static int GetUnitConditionVariable(Unit unit, Unit otherUnit, UnitConditionVariable variable, int value)
	{
		switch (variable)
		{
			case UnitConditionVariable.Race:
				return (int)unit.Race;
			case UnitConditionVariable.Class:
				return (int)unit.Class;
			case UnitConditionVariable.Level:
				return (int)unit.Level;
			case UnitConditionVariable.IsSelf:
				return unit == otherUnit ? 1 : 0;
			case UnitConditionVariable.IsMyPet:
				return (otherUnit != null && unit.CharmerOrOwnerGUID == otherUnit.GUID) ? 1 : 0;
			case UnitConditionVariable.IsMaster:
				return (otherUnit && otherUnit.CharmerOrOwnerGUID == unit.GUID) ? 1 : 0;
			case UnitConditionVariable.IsTarget:
				return (otherUnit && otherUnit.Target == unit.GUID) ? 1 : 0;
			case UnitConditionVariable.CanAssist:
				return (otherUnit && unit.IsValidAssistTarget(otherUnit)) ? 1 : 0;
			case UnitConditionVariable.CanAttack:
				return (otherUnit && unit.IsValidAttackTarget(otherUnit)) ? 1 : 0;
			case UnitConditionVariable.HasPet:
				return (!unit.CharmedGUID.IsEmpty || !unit.MinionGUID.IsEmpty) ? 1 : 0;
			case UnitConditionVariable.HasWeapon:
				var player = unit.AsPlayer;

				if (player != null)
					return (player.GetWeaponForAttack(WeaponAttackType.BaseAttack) || player.GetWeaponForAttack(WeaponAttackType.OffAttack)) ? 1 : 0;

				return (unit.GetVirtualItemId(0) != 0 || unit.GetVirtualItemId(1) != 0) ? 1 : 0;
			case UnitConditionVariable.HealthPct:
				return (int)unit.HealthPct;
			case UnitConditionVariable.ManaPct:
				return (int)unit.GetPowerPct(PowerType.Mana);
			case UnitConditionVariable.RagePct:
				return (int)unit.GetPowerPct(PowerType.Rage);
			case UnitConditionVariable.EnergyPct:
				return (int)unit.GetPowerPct(PowerType.Energy);
			case UnitConditionVariable.ComboPoints:
				return unit.GetPower(PowerType.ComboPoints);
			case UnitConditionVariable.HasHelpfulAuraSpell:
				return unit.GetAppliedAurasQuery()
							.HasSpellId((uint)value)
							.HasNegitiveFlag(false)
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.HasHelpfulAuraDispelType:
				return unit.GetAppliedAurasQuery()
							.HasDispelType((DispelType)value)
							.HasNegitiveFlag(false)
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.HasHelpfulAuraMechanic:
				return unit.GetAppliedAurasQuery()
							.HasNegitiveFlag(false)
							.AlsoMatches(aurApp => (aurApp.Base.SpellInfo.GetSpellMechanicMaskByEffectMask(aurApp.EffectMask) & (1ul << value)) != 0)
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.HasHarmfulAuraSpell:
				return unit.GetAppliedAurasQuery()
							.HasSpellId((uint)value)
							.HasNegitiveFlag()
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.HasHarmfulAuraDispelType:
				return unit.GetAppliedAurasQuery()
							.HasDispelType((DispelType)value)
							.HasNegitiveFlag()
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.HasHarmfulAuraMechanic:
				return unit.GetAppliedAurasQuery()
							.HasNegitiveFlag()
							.AlsoMatches(aurApp => (aurApp.Base.SpellInfo.GetSpellMechanicMaskByEffectMask(aurApp.EffectMask) & (1ul << value)) != 0)
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.HasHarmfulAuraSchool:
				return unit.GetAppliedAurasQuery()
							.HasNegitiveFlag()
							.AlsoMatches(aurApp => ((int)aurApp.Base.SpellInfo.GetSchoolMask() & (1 << value)) != 0)
							.GetResults()
							.Any()
							? value
							: 0;
			case UnitConditionVariable.DamagePhysicalPct:
				break;
			case UnitConditionVariable.DamageHolyPct:
				break;
			case UnitConditionVariable.DamageFirePct:
				break;
			case UnitConditionVariable.DamageNaturePct:
				break;
			case UnitConditionVariable.DamageFrostPct:
				break;
			case UnitConditionVariable.DamageShadowPct:
				break;
			case UnitConditionVariable.DamageArcanePct:
				break;
			case UnitConditionVariable.InCombat:
				return unit.IsInCombat ? 1 : 0;
			case UnitConditionVariable.IsMoving:
				return unit.HasUnitMovementFlag(MovementFlag.Forward | MovementFlag.Backward | MovementFlag.StrafeLeft | MovementFlag.StrafeRight) ? 1 : 0;
			case UnitConditionVariable.IsCasting:
			case UnitConditionVariable.IsCastingSpell: // this is supposed to return spell id by client code but data always has 0 or 1
				return unit.GetCurrentSpell(CurrentSpellTypes.Generic) != null ? 1 : 0;
			case UnitConditionVariable.IsChanneling:
			case UnitConditionVariable.IsChannelingSpell: // this is supposed to return spell id by client code but data always has 0 or 1
				return unit.ChannelSpellId != 0 ? 1 : 0;
			case UnitConditionVariable.NumberOfMeleeAttackers:
				return unit.Attackers.Count(attacker =>
				{
					var distance = Math.Max(unit.CombatReach + attacker.CombatReach + 1.3333334f, 5.0f);

					if (unit.HasUnitFlag(UnitFlags.PlayerControlled) || attacker.HasUnitFlag(UnitFlags.PlayerControlled))
						distance += 1.0f;

					return unit.Location.GetExactDistSq(attacker.Location) < distance * distance;
				});
			case UnitConditionVariable.IsAttackingMe:
				return (otherUnit != null && unit.Target == otherUnit.GUID) ? 1 : 0;
			case UnitConditionVariable.Range:
				return otherUnit ? (int)unit.Location.GetExactDist(otherUnit.Location) : 0;
			case UnitConditionVariable.InMeleeRange:
				if (otherUnit)
				{
					var distance = Math.Max(unit.CombatReach + otherUnit.CombatReach + 1.3333334f, 5.0f);

					if (unit.HasUnitFlag(UnitFlags.PlayerControlled) || otherUnit.HasUnitFlag(UnitFlags.PlayerControlled))
						distance += 1.0f;

					return (unit.Location.GetExactDistSq(otherUnit.Location) < distance * distance) ? 1 : 0;
				}

				return 0;
			case UnitConditionVariable.PursuitTime:
				break;
			case UnitConditionVariable.HasHarmfulAuraCanceledByDamage:
				return unit.HasNegativeAuraWithInterruptFlag(SpellAuraInterruptFlags.Damage) ? 1 : 0;
			case UnitConditionVariable.HasHarmfulAuraWithPeriodicDamage:
				return unit.HasAuraType(AuraType.PeriodicDamage) ? 1 : 0;
			case UnitConditionVariable.NumberOfEnemies:
				return unit.GetThreatManager().ThreatListSize;
			case UnitConditionVariable.NumberOfFriends:
				break;
			case UnitConditionVariable.ThreatPhysicalPct:
				break;
			case UnitConditionVariable.ThreatHolyPct:
				break;
			case UnitConditionVariable.ThreatFirePct:
				break;
			case UnitConditionVariable.ThreatNaturePct:
				break;
			case UnitConditionVariable.ThreatFrostPct:
				break;
			case UnitConditionVariable.ThreatShadowPct:
				break;
			case UnitConditionVariable.ThreatArcanePct:
				break;
			case UnitConditionVariable.IsInterruptible:
				break;
			case UnitConditionVariable.NumberOfAttackers:
				return unit.Attackers.Count;
			case UnitConditionVariable.NumberOfRangedAttackers:
				return unit.Attackers.Count(attacker =>
				{
					var distance = Math.Max(unit.CombatReach + attacker.CombatReach + 1.3333334f, 5.0f);

					if (unit.HasUnitFlag(UnitFlags.PlayerControlled) || attacker.HasUnitFlag(UnitFlags.PlayerControlled))
						distance += 1.0f;

					return unit.Location.GetExactDistSq(attacker.Location) >= distance * distance;
				});
			case UnitConditionVariable.CreatureType:
				return (int)unit.CreatureType;
			case UnitConditionVariable.IsMeleeAttacking:
			{
				var target = Global.ObjAccessor.GetUnit(unit, unit.Target);

				if (target != null)
				{
					var distance = Math.Max(unit.CombatReach + target.CombatReach + 1.3333334f, 5.0f);

					if (unit.HasUnitFlag(UnitFlags.PlayerControlled) || target.HasUnitFlag(UnitFlags.PlayerControlled))
						distance += 1.0f;

					return (unit.Location.GetExactDistSq(target.Location) < distance * distance) ? 1 : 0;
				}

				return 0;
			}
			case UnitConditionVariable.IsRangedAttacking:
			{
				var target = Global.ObjAccessor.GetUnit(unit, unit.Target);

				if (target != null)
				{
					var distance = Math.Max(unit.CombatReach + target.CombatReach + 1.3333334f, 5.0f);

					if (unit.HasUnitFlag(UnitFlags.PlayerControlled) || target.HasUnitFlag(UnitFlags.PlayerControlled))
						distance += 1.0f;

					return (unit.Location.GetExactDistSq(target.Location) >= distance * distance) ? 1 : 0;
				}

				return 0;
			}
			case UnitConditionVariable.Health:
				return (int)unit.Health;
			case UnitConditionVariable.SpellKnown:
				return unit.HasSpell((uint)value) ? value : 0;
			case UnitConditionVariable.HasHarmfulAuraEffect:
				return (value >= 0 && value < (int)AuraType.Total && unit.GetAuraEffectsByType((AuraType)value).Any(aurEff => aurEff.Base.GetApplicationOfTarget(unit.GUID).Flags.HasFlag(AuraFlags.Negative))) ? 1 : 0;
			case UnitConditionVariable.IsImmuneToAreaOfEffect:
				break;
			case UnitConditionVariable.IsPlayer:
				return unit.IsPlayer ? 1 : 0;
			case UnitConditionVariable.DamageMagicPct:
				break;
			case UnitConditionVariable.DamageTotalPct:
				break;
			case UnitConditionVariable.ThreatMagicPct:
				break;
			case UnitConditionVariable.ThreatTotalPct:
				break;
			case UnitConditionVariable.HasCritter:
				return unit.CritterGUID.IsEmpty ? 0 : 1;
			case UnitConditionVariable.HasTotemInSlot1:
				return unit.SummonSlot[(int)SummonSlot.Totem].IsEmpty ? 0 : 1;
			case UnitConditionVariable.HasTotemInSlot2:
				return unit.SummonSlot[(int)SummonSlot.Totem2].IsEmpty ? 0 : 1;
			case UnitConditionVariable.HasTotemInSlot3:
				return unit.SummonSlot[(int)SummonSlot.Totem3].IsEmpty ? 0 : 1;
			case UnitConditionVariable.HasTotemInSlot4:
				return unit.SummonSlot[(int)SummonSlot.Totem4].IsEmpty ? 0 : 1;
			case UnitConditionVariable.HasTotemInSlot5:
				break;
			case UnitConditionVariable.Creature:
				return (int)unit.Entry;
			case UnitConditionVariable.StringID:
				break;
			case UnitConditionVariable.HasAura:
				return unit.HasAura((uint)value) ? value : 0;
			case UnitConditionVariable.IsEnemy:
				return (otherUnit && unit.GetReactionTo(otherUnit) <= ReputationRank.Hostile) ? 1 : 0;
			case UnitConditionVariable.IsSpecMelee:
				return (unit.IsPlayer && unit.AsPlayer.GetPrimarySpecialization() != 0 && CliDB.ChrSpecializationStorage.LookupByKey(unit.AsPlayer.GetPrimarySpecialization()).Flags.HasFlag(ChrSpecializationFlag.Melee)) ? 1 : 0;
			case UnitConditionVariable.IsSpecTank:
				return (unit.IsPlayer && unit.AsPlayer.GetPrimarySpecialization() != 0 && CliDB.ChrSpecializationStorage.LookupByKey(unit.AsPlayer.GetPrimarySpecialization()).Role == 0) ? 1 : 0;
			case UnitConditionVariable.IsSpecRanged:
				return (unit.IsPlayer && unit.AsPlayer.GetPrimarySpecialization() != 0 && CliDB.ChrSpecializationStorage.LookupByKey(unit.AsPlayer.GetPrimarySpecialization()).Flags.HasFlag(ChrSpecializationFlag.Ranged)) ? 1 : 0;
			case UnitConditionVariable.IsSpecHealer:
				return (unit.IsPlayer && unit.AsPlayer.GetPrimarySpecialization() != 0 && CliDB.ChrSpecializationStorage.LookupByKey(unit.AsPlayer.GetPrimarySpecialization()).Role == 1) ? 1 : 0;
			case UnitConditionVariable.IsPlayerControlledNPC:
				return unit.IsCreature && unit.HasUnitFlag(UnitFlags.PlayerControlled) ? 1 : 0;
			case UnitConditionVariable.IsDying:
				return unit.Health == 0 ? 1 : 0;
			case UnitConditionVariable.PathFailCount:
				break;
			case UnitConditionVariable.IsMounted:
				return unit.MountDisplayId != 0 ? 1 : 0;
			case UnitConditionVariable.Label:
				break;
			case UnitConditionVariable.IsMySummon:
				return (otherUnit && (otherUnit.CharmerGUID == unit.GUID || otherUnit.CreatorGUID == unit.GUID)) ? 1 : 0;
			case UnitConditionVariable.IsSummoner:
				return (otherUnit && (unit.CharmerGUID == otherUnit.GUID || unit.CreatorGUID == otherUnit.GUID)) ? 1 : 0;
			case UnitConditionVariable.IsMyTarget:
				return (otherUnit && unit.Target == otherUnit.GUID) ? 1 : 0;
			case UnitConditionVariable.Sex:
				return (int)unit.Gender;
			case UnitConditionVariable.LevelWithinContentTuning:
				var levelRange = Global.DB2Mgr.GetContentTuningData((uint)value, 0);

				if (levelRange.HasValue)
					return unit.Level >= levelRange.Value.MinLevel && unit.Level <= levelRange.Value.MaxLevel ? value : 0;

				return 0;
			case UnitConditionVariable.IsFlying:
				return unit.IsFlying ? 1 : 0;
			case UnitConditionVariable.IsHovering:
				return unit.IsHovering ? 1 : 0;
			case UnitConditionVariable.HasHelpfulAuraEffect:
				return (value >= 0 && value < (int)AuraType.Total && unit.GetAuraEffectsByType((AuraType)value).Any(aurEff => !aurEff.Base.GetApplicationOfTarget(unit.GUID).Flags.HasFlag(AuraFlags.Negative))) ? 1 : 0;
			case UnitConditionVariable.HasHelpfulAuraSchool:
				return unit.GetAppliedAurasQuery()
							.HasNegitiveFlag()
							.AlsoMatches(aurApp => ((int)aurApp.Base.SpellInfo.GetSchoolMask() & (1 << value)) != 0)
							.GetResults()
							.Any()
							? 1
							: 0;
			default:
				break;
		}

		return 0;
	}

	static int EvalSingleValue(ByteBuffer buffer, Player player)
	{
		var valueType = (WorldStateExpressionValueType)buffer.ReadUInt8();
		var value = 0;

		switch (valueType)
		{
			case WorldStateExpressionValueType.Constant:
			{
				value = buffer.ReadInt32();

				break;
			}
			case WorldStateExpressionValueType.WorldState:
			{
				var worldStateId = buffer.ReadUInt32();
				value = Global.WorldStateMgr.GetValue((int)worldStateId, player.Map);

				break;
			}
			case WorldStateExpressionValueType.Function:
			{
				var functionType = (WorldStateExpressionFunctions)buffer.ReadUInt32();
				var arg1 = EvalSingleValue(buffer, player);
				var arg2 = EvalSingleValue(buffer, player);

				if (functionType >= WorldStateExpressionFunctions.Max)
					return 0;

				value = WorldStateExpressionFunction(functionType, player, arg1, arg2);

				break;
			}
			default:
				break;
		}

		return value;
	}

	static int WorldStateExpressionFunction(WorldStateExpressionFunctions functionType, Player player, int arg1, int arg2)
	{
		switch (functionType)
		{
			case WorldStateExpressionFunctions.Random:
				return (int)RandomHelper.URand(Math.Min(arg1, arg2), Math.Max(arg1, arg2));
			case WorldStateExpressionFunctions.Month:
				return GameTime.GetDateAndTime().Month + 1;
			case WorldStateExpressionFunctions.Day:
				return GameTime.GetDateAndTime().Day + 1;
			case WorldStateExpressionFunctions.TimeOfDay:
				var localTime = GameTime.GetDateAndTime();

				return localTime.Hour * Time.Minute + localTime.Minute;
			case WorldStateExpressionFunctions.Region:
				return Global.WorldMgr.RealmId.Region;
			case WorldStateExpressionFunctions.ClockHour:
				var currentHour = GameTime.GetDateAndTime().Hour + 1;

				return currentHour <= 12 ? (currentHour != 0 ? currentHour : 12) : currentHour - 12;
			case WorldStateExpressionFunctions.OldDifficultyId:
				var difficulty = CliDB.DifficultyStorage.LookupByKey(player.Map.DifficultyID);

				if (difficulty != null)
					return difficulty.OldEnumValue;

				return -1;
			case WorldStateExpressionFunctions.HolidayActive:
				return Global.GameEventMgr.IsHolidayActive((HolidayIds)arg1) ? 1 : 0;
			case WorldStateExpressionFunctions.TimerCurrentTime:
				return (int)GameTime.GetGameTime();
			case WorldStateExpressionFunctions.WeekNumber:
				var now = GameTime.GetGameTime();
				uint raidOrigin = 1135695600;
				var region = CliDB.CfgRegionsStorage.LookupByKey(Global.WorldMgr.RealmId.Region);

				if (region != null)
					raidOrigin = region.Raidorigin;

				return (int)(now - raidOrigin) / Time.Week;
			case WorldStateExpressionFunctions.DifficultyId:
				return (int)player.Map.DifficultyID;
			case WorldStateExpressionFunctions.WarModeActive:
				return player.HasPlayerFlag(PlayerFlags.WarModeActive) ? 1 : 0;
			case WorldStateExpressionFunctions.WorldStateExpression:
				var worldStateExpression = CliDB.WorldStateExpressionStorage.LookupByKey(arg1);

				if (worldStateExpression != null)
					return IsPlayerMeetingExpression(player, worldStateExpression) ? 1 : 0;

				return 0;
			case WorldStateExpressionFunctions.MersenneRandom:
				if (arg1 == 1)
					return 1;

				//todo fix me
				// init with predetermined seed                      
				//std::mt19937 mt(arg2? arg2 : 1);
				//value = mt() % arg1 + 1;
				return 0;
			case WorldStateExpressionFunctions.None:
			case WorldStateExpressionFunctions.HolidayStart:
			case WorldStateExpressionFunctions.HolidayLeft:
			case WorldStateExpressionFunctions.Unk13:
			case WorldStateExpressionFunctions.Unk14:
			case WorldStateExpressionFunctions.Unk17:
			case WorldStateExpressionFunctions.Unk18:
			case WorldStateExpressionFunctions.Unk19:
			case WorldStateExpressionFunctions.Unk20:
			case WorldStateExpressionFunctions.Unk21:
			case WorldStateExpressionFunctions.KeystoneAffix:
			case WorldStateExpressionFunctions.Unk24:
			case WorldStateExpressionFunctions.Unk25:
			case WorldStateExpressionFunctions.Unk26:
			case WorldStateExpressionFunctions.Unk27:
			case WorldStateExpressionFunctions.KeystoneLevel:
			case WorldStateExpressionFunctions.Unk29:
			case WorldStateExpressionFunctions.Unk30:
			case WorldStateExpressionFunctions.Unk31:
			case WorldStateExpressionFunctions.Unk32:
			case WorldStateExpressionFunctions.Unk34:
			case WorldStateExpressionFunctions.Unk35:
			case WorldStateExpressionFunctions.Unk36:
			case WorldStateExpressionFunctions.UiWidgetData:
			case WorldStateExpressionFunctions.TimeEventPassed:
			default:
				return 0;
		}
	}

	static int EvalValue(ByteBuffer buffer, Player player)
	{
		var leftValue = EvalSingleValue(buffer, player);

		var operatorType = (WorldStateExpressionOperatorType)buffer.ReadUInt8();

		if (operatorType == WorldStateExpressionOperatorType.None)
			return leftValue;

		var rightValue = EvalSingleValue(buffer, player);

		switch (operatorType)
		{
			case WorldStateExpressionOperatorType.Sum:
				return leftValue + rightValue;
			case WorldStateExpressionOperatorType.Substraction:
				return leftValue - rightValue;
			case WorldStateExpressionOperatorType.Multiplication:
				return leftValue * rightValue;
			case WorldStateExpressionOperatorType.Division:
				return rightValue == 0 ? 0 : leftValue / rightValue;
			case WorldStateExpressionOperatorType.Remainder:
				return rightValue == 0 ? 0 : leftValue % rightValue;
			default:
				break;
		}

		return leftValue;
	}

	static bool EvalRelOp(ByteBuffer buffer, Player player)
	{
		var leftValue = EvalValue(buffer, player);

		var compareLogic = (WorldStateExpressionComparisonType)buffer.ReadUInt8();

		if (compareLogic == WorldStateExpressionComparisonType.None)
			return leftValue != 0;

		var rightValue = EvalValue(buffer, player);

		switch (compareLogic)
		{
			case WorldStateExpressionComparisonType.Equal:
				return leftValue == rightValue;
			case WorldStateExpressionComparisonType.NotEqual:
				return leftValue != rightValue;
			case WorldStateExpressionComparisonType.Less:
				return leftValue < rightValue;
			case WorldStateExpressionComparisonType.LessOrEqual:
				return leftValue <= rightValue;
			case WorldStateExpressionComparisonType.Greater:
				return leftValue > rightValue;
			case WorldStateExpressionComparisonType.GreaterOrEqual:
				return leftValue >= rightValue;
			default:
				break;
		}

		return false;
	}

	public struct ConditionTypeInfo
	{
		public ConditionTypeInfo(string name, params bool[] args)
		{
			Name = name;
			HasConditionValue1 = args[0];
			HasConditionValue2 = args[1];
			HasConditionValue3 = args[2];
		}

		public string Name;
		public bool HasConditionValue1;
		public bool HasConditionValue2;
		public bool HasConditionValue3;
	}
}