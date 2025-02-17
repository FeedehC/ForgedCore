﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Framework.Database;
using Game.Chat;
using Game.DataStorage;
using Game.Entities;
using Game.Mails;
using Game.Maps;
using Game.Networking;
using Game.Networking.Packets;
using Game.Scripting.Interfaces.IAchievement;

namespace Game.Achievements;

public class PlayerAchievementMgr : AchievementManager
{
	readonly Player _owner;

	public PlayerAchievementMgr(Player owner)
	{
		_owner = owner;
	}

	public override void Reset()
	{
		base.Reset();

		foreach (var iter in _completedAchievements)
		{
			AchievementDeleted achievementDeleted = new();
			achievementDeleted.AchievementID = iter.Key;
			SendPacket(achievementDeleted);
		}

		_completedAchievements.Clear();
		_achievementPoints = 0;
		DeleteFromDB(_owner.GUID);

		// re-fill data
		CheckAllAchievementCriteria(_owner);
	}

	public static void DeleteFromDB(ObjectGuid guid)
	{
		SQLTransaction trans = new();

		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_CHAR_ACHIEVEMENT);
		stmt.AddValue(0, guid.Counter);
		DB.Characters.Execute(stmt);

		stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_CHAR_ACHIEVEMENT_PROGRESS);
		stmt.AddValue(0, guid.Counter);
		DB.Characters.Execute(stmt);

		DB.Characters.CommitTransaction(trans);
	}

	public void LoadFromDB(SQLResult achievementResult, SQLResult criteriaResult)
	{
		if (!achievementResult.IsEmpty())
			do
			{
				var achievementid = achievementResult.Read<uint>(0);

				// must not happen: cleanup at server startup in sAchievementMgr.LoadCompletedAchievements()
				var achievement = CliDB.AchievementStorage.LookupByKey(achievementid);

				if (achievement == null)
					continue;

				CompletedAchievementData ca = new();
				ca.Date = achievementResult.Read<long>(1);
				ca.Changed = false;

				_achievementPoints += achievement.Points;

				// title achievement rewards are retroactive
				var reward = Global.AchievementMgr.GetAchievementReward(achievement);

				if (reward != null)
				{
					var titleId = reward.TitleId[Player.TeamForRace(_owner.Race) == TeamFaction.Alliance ? 0 : 1];

					if (titleId != 0)
					{
						var titleEntry = CliDB.CharTitlesStorage.LookupByKey(titleId);

						if (titleEntry != null)
							_owner.SetTitle(titleEntry);
					}
				}

				_completedAchievements[achievementid] = ca;
			} while (achievementResult.NextRow());

		if (!criteriaResult.IsEmpty())
		{
			var now = GameTime.GetGameTime();

			do
			{
				var id = criteriaResult.Read<uint>(0);
				var counter = criteriaResult.Read<ulong>(1);
				var date = criteriaResult.Read<long>(2);

				var criteria = Global.CriteriaMgr.GetCriteria(id);

				if (criteria == null)
				{
					// Removing non-existing criteria data for all characters
					Log.outError(LogFilter.Achievement, "Non-existing achievement criteria {0} data removed from table `character_achievement_progress`.", id);

					var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_INVALID_ACHIEV_PROGRESS_CRITERIA);
					stmt.AddValue(0, id);
					DB.Characters.Execute(stmt);

					continue;
				}

				if (criteria.Entry.StartTimer != 0 && (date + criteria.Entry.StartTimer) < now)
					continue;

				CriteriaProgress progress = new();
				progress.Counter = counter;
				progress.Date = date;
				progress.PlayerGUID = _owner.GUID;
				progress.Changed = false;

				_criteriaProgress[id] = progress;
			} while (criteriaResult.NextRow());
		}
	}

	public void SaveToDB(SQLTransaction trans)
	{
		if (!_completedAchievements.Empty())
			foreach (var pair in _completedAchievements)
			{
				if (!pair.Value.Changed)
					continue;

				var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_CHAR_ACHIEVEMENT_BY_ACHIEVEMENT);
				stmt.AddValue(0, pair.Key);
				stmt.AddValue(1, _owner.GUID.Counter);
				trans.Append(stmt);

				stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_CHAR_ACHIEVEMENT);
				stmt.AddValue(0, _owner.GUID.Counter);
				stmt.AddValue(1, pair.Key);
				stmt.AddValue(2, pair.Value.Date);
				trans.Append(stmt);

				pair.Value.Changed = false;
			}

		if (!_criteriaProgress.Empty())
			foreach (var pair in _criteriaProgress)
			{
				if (!pair.Value.Changed)
					continue;

				var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_CHAR_ACHIEVEMENT_PROGRESS_BY_CRITERIA);
				stmt.AddValue(0, _owner.GUID.Counter);
				stmt.AddValue(1, pair.Key);
				trans.Append(stmt);

				if (pair.Value.Counter != 0)
				{
					stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_CHAR_ACHIEVEMENT_PROGRESS);
					stmt.AddValue(0, _owner.GUID.Counter);
					stmt.AddValue(1, pair.Key);
					stmt.AddValue(2, pair.Value.Counter);
					stmt.AddValue(3, pair.Value.Date);
					trans.Append(stmt);
				}

				pair.Value.Changed = false;
			}
	}

	public void ResetCriteria(CriteriaFailEvent failEvent, uint failAsset, bool evenIfCriteriaComplete)
	{
		Log.outDebug(LogFilter.Achievement, $"ResetAchievementCriteria({failEvent}, {failAsset}, {evenIfCriteriaComplete})");

		// Disable for GameMasters with GM-mode enabled or for players that don't have the related RBAC permission
		if (_owner.IsGameMaster || _owner.Session.HasPermission(RBACPermissions.CannotEarnAchievements))
			return;

		var achievementCriteriaList = Global.CriteriaMgr.GetCriteriaByFailEvent(failEvent, (int)failAsset);

		if (!achievementCriteriaList.Empty())
			foreach (var achievementCriteria in achievementCriteriaList)
			{
				var trees = Global.CriteriaMgr.GetCriteriaTreesByCriteria(achievementCriteria.Id);
				var allComplete = true;

				foreach (var tree in trees)
					// don't update already completed criteria if not forced or achievement already complete
					if (!(IsCompletedCriteriaTree(tree) && !evenIfCriteriaComplete) || !HasAchieved(tree.Achievement.Id))
					{
						allComplete = false;

						break;
					}

				if (allComplete)
					continue;

				RemoveCriteriaProgress(achievementCriteria);
			}
	}

	public override void SendAllData(Player receiver)
	{
		AllAccountCriteria allAccountCriteria = new();
		AllAchievementData achievementData = new();

		foreach (var pair in _completedAchievements)
		{
			var achievement = VisibleAchievementCheck(pair);

			if (achievement == null)
				continue;

			EarnedAchievement earned = new();
			earned.Id = pair.Key;
			earned.Date = pair.Value.Date;

			if (!achievement.Flags.HasAnyFlag(AchievementFlags.Account))
			{
				earned.Owner = _owner.GUID;
				earned.VirtualRealmAddress = earned.NativeRealmAddress = Global.WorldMgr.VirtualRealmAddress;
			}

			achievementData.Data.Earned.Add(earned);
		}

		foreach (var pair in _criteriaProgress)
		{
			var criteria = Global.CriteriaMgr.GetCriteria(pair.Key);

			CriteriaProgressPkt progress = new();
			progress.Id = pair.Key;
			progress.Quantity = pair.Value.Counter;
			progress.Player = pair.Value.PlayerGUID;
			progress.Flags = 0;
			progress.Date = pair.Value.Date;
			progress.TimeFromStart = 0;
			progress.TimeFromCreate = 0;
			achievementData.Data.Progress.Add(progress);

			if (criteria.FlagsCu.HasAnyFlag(CriteriaFlagsCu.Account))
			{
				CriteriaProgressPkt accountProgress = new();
				accountProgress.Id = pair.Key;
				accountProgress.Quantity = pair.Value.Counter;
				accountProgress.Player = _owner.Session.BattlenetAccountGUID;
				accountProgress.Flags = 0;
				accountProgress.Date = pair.Value.Date;
				accountProgress.TimeFromStart = 0;
				accountProgress.TimeFromCreate = 0;
				allAccountCriteria.Progress.Add(accountProgress);
			}
		}

		if (!allAccountCriteria.Progress.Empty())
			SendPacket(allAccountCriteria);

		SendPacket(achievementData);
	}

	public void SendAchievementInfo(Player receiver)
	{
		RespondInspectAchievements inspectedAchievements = new();
		inspectedAchievements.Player = _owner.GUID;

		foreach (var pair in _completedAchievements)
		{
			var achievement = VisibleAchievementCheck(pair);

			if (achievement == null)
				continue;

			EarnedAchievement earned = new();
			earned.Id = pair.Key;
			earned.Date = pair.Value.Date;

			if (!achievement.Flags.HasAnyFlag(AchievementFlags.Account))
			{
				earned.Owner = _owner.GUID;
				earned.VirtualRealmAddress = earned.NativeRealmAddress = Global.WorldMgr.VirtualRealmAddress;
			}

			inspectedAchievements.Data.Earned.Add(earned);
		}

		foreach (var pair in _criteriaProgress)
		{
			CriteriaProgressPkt progress = new();
			progress.Id = pair.Key;
			progress.Quantity = pair.Value.Counter;
			progress.Player = pair.Value.PlayerGUID;
			progress.Flags = 0;
			progress.Date = pair.Value.Date;
			progress.TimeFromStart = 0;
			progress.TimeFromCreate = 0;
			inspectedAchievements.Data.Progress.Add(progress);
		}

		receiver.SendPacket(inspectedAchievements);
	}

	public override void CompletedAchievement(AchievementRecord achievement, Player referencePlayer)
	{
		// Disable for GameMasters with GM-mode enabled or for players that don't have the related RBAC permission
		if (_owner.IsGameMaster || _owner.Session.HasPermission(RBACPermissions.CannotEarnAchievements))
			return;

		if ((achievement.Faction == AchievementFaction.Horde && referencePlayer.Team != TeamFaction.Horde) ||
			(achievement.Faction == AchievementFaction.Alliance && referencePlayer.Team != TeamFaction.Alliance))
			return;

		if (achievement.Flags.HasAnyFlag(AchievementFlags.Counter) || HasAchieved(achievement.Id))
			return;

		if (achievement.Flags.HasAnyFlag(AchievementFlags.ShowInGuildNews))
		{
			var guild = referencePlayer.Guild;

			if (guild)
				guild.AddGuildNews(GuildNews.PlayerAchievement, referencePlayer.GUID, (uint)(achievement.Flags & AchievementFlags.ShowInGuildHeader), achievement.Id);
		}

		if (!_owner.Session.PlayerLoading)
			SendAchievementEarned(achievement);

		Log.outDebug(LogFilter.Achievement, "PlayerAchievementMgr.CompletedAchievement({0}). {1}", achievement.Id, GetOwnerInfo());

		CompletedAchievementData ca = new();
		ca.Date = GameTime.GetGameTime();
		ca.Changed = true;
		_completedAchievements[achievement.Id] = ca;

		if (achievement.Flags.HasAnyFlag(AchievementFlags.RealmFirstReach | AchievementFlags.RealmFirstKill))
			Global.AchievementMgr.SetRealmCompleted(achievement);

		if (!achievement.Flags.HasAnyFlag(AchievementFlags.TrackingFlag))
			_achievementPoints += achievement.Points;

		UpdateCriteria(CriteriaType.EarnAchievement, achievement.Id, 0, 0, null, referencePlayer);
		UpdateCriteria(CriteriaType.EarnAchievementPoints, achievement.Points, 0, 0, null, referencePlayer);

		Global.ScriptMgr.RunScript<IAchievementOnCompleted>(p => p.OnCompleted(referencePlayer, achievement), Global.AchievementMgr.GetAchievementScriptId(achievement.Id));
		// reward items and titles if any
		var reward = Global.AchievementMgr.GetAchievementReward(achievement);

		// no rewards
		if (reward == null)
			return;

		// titles
		//! Currently there's only one achievement that deals with gender-specific titles.
		//! Since no common attributes were found, (not even in titleRewardFlags field)
		//! we explicitly check by ID. Maybe in the future we could move the achievement_reward
		//! condition fields to the condition system.
		var titleId = reward.TitleId[achievement.Id == 1793 ? (int)_owner.NativeGender : (_owner.Team == TeamFaction.Alliance ? 0 : 1)];

		if (titleId != 0)
		{
			var titleEntry = CliDB.CharTitlesStorage.LookupByKey(titleId);

			if (titleEntry != null)
				_owner.SetTitle(titleEntry);
		}

		// mail
		if (reward.SenderCreatureId != 0)
		{
			MailDraft draft = new(reward.MailTemplateId);

			if (reward.MailTemplateId == 0)
			{
				// subject and text
				var subject = reward.Subject;
				var text = reward.Body;

				var localeConstant = _owner.Session.SessionDbLocaleIndex;

				if (localeConstant != Locale.enUS)
				{
					var loc = Global.AchievementMgr.GetAchievementRewardLocale(achievement);

					if (loc != null)
					{
						ObjectManager.GetLocaleString(loc.Subject, localeConstant, ref subject);
						ObjectManager.GetLocaleString(loc.Body, localeConstant, ref text);
					}
				}

				draft = new MailDraft(subject, text);
			}

			SQLTransaction trans = new();

			var item = reward.ItemId != 0 ? Item.CreateItem(reward.ItemId, 1, ItemContext.None, _owner) : null;

			if (item)
			{
				// save new item before send
				item.SaveToDB(trans); // save for prevent lost at next mail load, if send fail then item will deleted

				// item
				draft.AddItem(item);
			}

			draft.SendMailTo(trans, _owner, new MailSender(MailMessageType.Creature, reward.SenderCreatureId));
			DB.Characters.CommitTransaction(trans);
		}
	}

	public bool ModifierTreeSatisfied(uint modifierTreeId)
	{
		var modifierTree = Global.CriteriaMgr.GetModifierTree(modifierTreeId);

		if (modifierTree != null)
			return ModifierTreeSatisfied(modifierTree, 0, 0, null, _owner);

		return false;
	}

	public override void SendCriteriaUpdate(Criteria criteria, CriteriaProgress progress, TimeSpan timeElapsed, bool timedCompleted)
	{
		if (criteria.FlagsCu.HasAnyFlag(CriteriaFlagsCu.Account))
		{
			AccountCriteriaUpdate criteriaUpdate = new();
			criteriaUpdate.Progress.Id = criteria.Id;
			criteriaUpdate.Progress.Quantity = progress.Counter;
			criteriaUpdate.Progress.Player = _owner.Session.BattlenetAccountGUID;
			criteriaUpdate.Progress.Flags = 0;

			if (criteria.Entry.StartTimer != 0)
				criteriaUpdate.Progress.Flags = timedCompleted ? 1 : 0u; // 1 is for keeping the counter at 0 in client

			criteriaUpdate.Progress.Date = progress.Date;
			criteriaUpdate.Progress.TimeFromStart = (uint)timeElapsed.TotalSeconds;
			criteriaUpdate.Progress.TimeFromCreate = 0;
			SendPacket(criteriaUpdate);
		}
		else
		{
			CriteriaUpdate criteriaUpdate = new();

			criteriaUpdate.CriteriaID = criteria.Id;
			criteriaUpdate.Quantity = progress.Counter;
			criteriaUpdate.PlayerGUID = _owner.GUID;
			criteriaUpdate.Flags = 0;

			if (criteria.Entry.StartTimer != 0)
				criteriaUpdate.Flags = timedCompleted ? 1 : 0u; // 1 is for keeping the counter at 0 in client

			criteriaUpdate.CurrentTime = progress.Date;
			criteriaUpdate.ElapsedTime = (uint)timeElapsed.TotalSeconds;
			criteriaUpdate.CreationTime = 0;

			SendPacket(criteriaUpdate);
		}
	}

	public override void SendCriteriaProgressRemoved(uint criteriaId)
	{
		CriteriaDeleted criteriaDeleted = new();
		criteriaDeleted.CriteriaID = criteriaId;
		SendPacket(criteriaDeleted);
	}

	public override void SendPacket(ServerPacket data)
	{
		_owner.SendPacket(data);
	}

	public override List<Criteria> GetCriteriaByType(CriteriaType type, uint asset)
	{
		return Global.CriteriaMgr.GetPlayerCriteriaByType(type, asset);
	}

	public override string GetOwnerInfo()
	{
		return $"{_owner.GUID} {_owner.GetName()}";
	}

	void SendAchievementEarned(AchievementRecord achievement)
	{
		// Don't send for achievements with ACHIEVEMENT_FLAG_HIDDEN
		if (achievement.Flags.HasAnyFlag(AchievementFlags.Hidden))
			return;

		Log.outDebug(LogFilter.Achievement, "PlayerAchievementMgr.SendAchievementEarned({0})", achievement.Id);

		if (!achievement.Flags.HasAnyFlag(AchievementFlags.TrackingFlag))
		{
			var guild = Global.GuildMgr.GetGuildById(_owner.GuildId);

			if (guild)
			{
				BroadcastTextBuilder say_builder = new(_owner, ChatMsg.GuildAchievement, (uint)BroadcastTextIds.AchivementEarned, _owner.NativeGender, _owner, achievement.Id);
				var say_do = new LocalizedDo(say_builder);
				guild.BroadcastWorker(say_do, _owner);
			}

			if (achievement.Flags.HasAnyFlag(AchievementFlags.RealmFirstReach | AchievementFlags.RealmFirstKill))
			{
				// broadcast realm first reached
				BroadcastAchievement serverFirstAchievement = new();
				serverFirstAchievement.Name = _owner.GetName();
				serverFirstAchievement.PlayerGUID = _owner.GUID;
				serverFirstAchievement.AchievementID = achievement.Id;
				Global.WorldMgr.SendGlobalMessage(serverFirstAchievement);
			}
			// if player is in world he can tell his friends about new achievement
			else if (_owner.IsInWorld)
			{
				BroadcastTextBuilder _builder = new(_owner, ChatMsg.Achievement, (uint)BroadcastTextIds.AchivementEarned, _owner.NativeGender, _owner, achievement.Id);
				var _localizer = new LocalizedDo(_builder);
				var _worker = new PlayerDistWorker(_owner, WorldConfig.GetFloatValue(WorldCfg.ListenRangeSay), _localizer, GridType.World);
				Cell.VisitGrid(_owner, _worker, WorldConfig.GetFloatValue(WorldCfg.ListenRangeSay));
			}
		}

		AchievementEarned achievementEarned = new();
		achievementEarned.Sender = _owner.GUID;
		achievementEarned.Earner = _owner.GUID;
		achievementEarned.EarnerNativeRealm = achievementEarned.EarnerVirtualRealm = Global.WorldMgr.VirtualRealmAddress;
		achievementEarned.AchievementID = achievement.Id;
		achievementEarned.Time = GameTime.GetGameTime();

		if (!achievement.Flags.HasAnyFlag(AchievementFlags.TrackingFlag))
			_owner.SendMessageToSetInRange(achievementEarned, WorldConfig.GetFloatValue(WorldCfg.ListenRangeSay), true);
		else
			_owner.SendPacket(achievementEarned);
	}
}