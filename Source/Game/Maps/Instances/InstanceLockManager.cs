﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Framework.Database;
using Game.DataStorage;
using Game.Entities;

namespace Game.Maps;

using InstanceLockKey = Tuple<uint, uint>;

public class InstanceLockManager : Singleton<InstanceLockManager>
{
	readonly object _lockObject = new();
	readonly Dictionary<ObjectGuid, Dictionary<InstanceLockKey, InstanceLock>> _temporaryInstanceLocksByPlayer = new(); // locks stored here before any boss gets killed
	readonly Dictionary<ObjectGuid, Dictionary<InstanceLockKey, InstanceLock>> _instanceLocksByPlayer = new();
	readonly Dictionary<uint, SharedInstanceLockData> _instanceLockDataById = new();
	bool _unloading;

	InstanceLockManager() { }

	public void Load()
	{
		Dictionary<uint, SharedInstanceLockData> instanceLockDataById = new();

		//                                              0           1     2
		var result = DB.Characters.Query("SELECT instanceId, data, completedEncountersMask FROM instance");

		if (!result.IsEmpty())
			do
			{
				var instanceId = result.Read<uint>(0);

				SharedInstanceLockData data = new();
				data.Data = result.Read<string>(1);
				data.CompletedEncountersMask = result.Read<uint>(2);
				data.InstanceId = instanceId;

				instanceLockDataById[instanceId] = data;
			} while (result.NextRow());

		//                                                  0     1      2       3           4           5     6                        7           8
		var lockResult = DB.Characters.Query("SELECT guid, mapId, lockId, instanceId, difficulty, data, completedEncountersMask, expiryTime, extended FROM character_instance_lock");

		if (!result.IsEmpty())
			do
			{
				var playerGuid = ObjectGuid.Create(HighGuid.Player, lockResult.Read<ulong>(0));
				var mapId = lockResult.Read<uint>(1);
				var lockId = lockResult.Read<uint>(2);
				var instanceId = lockResult.Read<uint>(3);
				var difficulty = (Difficulty)lockResult.Read<byte>(4);
				var expiryTime = Time.UnixTimeToDateTime(lockResult.Read<long>(7));

				// Mark instance id as being used
				Global.MapMgr.RegisterInstanceId(instanceId);

				InstanceLock instanceLock;

				if (new MapDb2Entries(mapId, difficulty).IsInstanceIdBound())
				{
					var sharedData = instanceLockDataById.LookupByKey(instanceId);

					if (sharedData == null)
					{
						Log.outError(LogFilter.Instance, $"Missing instance data for instance id based lock (id {instanceId})");
						DB.Characters.Execute($"DELETE FROM character_instance_lock WHERE instanceId = {instanceId}");

						continue;
					}

					instanceLock = new SharedInstanceLock(mapId, difficulty, expiryTime, instanceId, sharedData);
					_instanceLockDataById[instanceId] = sharedData;
				}
				else
				{
					instanceLock = new InstanceLock(mapId, difficulty, expiryTime, instanceId);
				}

				instanceLock.GetData().Data = lockResult.Read<string>(5);
				instanceLock.GetData().CompletedEncountersMask = lockResult.Read<uint>(6);
				instanceLock.SetExtended(lockResult.Read<bool>(8));

				_instanceLocksByPlayer[playerGuid][Tuple.Create(mapId, lockId)] = instanceLock;
			} while (result.NextRow());
	}

	public void Unload()
	{
		_unloading = true;
		_instanceLocksByPlayer.Clear();
		_instanceLockDataById.Clear();
	}

	public TransferAbortReason CanJoinInstanceLock(ObjectGuid playerGuid, MapDb2Entries entries, InstanceLock instanceLock)
	{
		if (!entries.MapDifficulty.HasResetSchedule())
			return TransferAbortReason.None;

		var playerInstanceLock = FindActiveInstanceLock(playerGuid, entries);

		if (playerInstanceLock == null)
			return TransferAbortReason.None;

		if (entries.Map.IsFlexLocking())
		{
			// compare completed encounters - if instance has any encounters unkilled in players lock then cannot enter
			if ((playerInstanceLock.GetData().CompletedEncountersMask & ~instanceLock.GetData().CompletedEncountersMask) != 0)
				return TransferAbortReason.AlreadyCompletedEncounter;

			return TransferAbortReason.None;
		}

		if (!entries.MapDifficulty.IsUsingEncounterLocks() && playerInstanceLock.GetInstanceId() != 0 && playerInstanceLock.GetInstanceId() != instanceLock.GetInstanceId())
			return TransferAbortReason.LockedToDifferentInstance;

		return TransferAbortReason.None;
	}

	public InstanceLock FindInstanceLock(Dictionary<ObjectGuid, Dictionary<InstanceLockKey, InstanceLock>> locks, ObjectGuid playerGuid, MapDb2Entries entries)
	{
		var playerLocks = locks.LookupByKey(playerGuid);

		if (playerLocks == null)
			return null;

		return playerLocks.LookupByKey(entries.GetKey());
	}

	public InstanceLock FindActiveInstanceLock(ObjectGuid playerGuid, MapDb2Entries entries)
	{
		lock (_lockObject)
		{
			return FindActiveInstanceLock(playerGuid, entries, false, true);
		}
	}

	public InstanceLock FindActiveInstanceLock(ObjectGuid playerGuid, MapDb2Entries entries, bool ignoreTemporary, bool ignoreExpired)
	{
		var instanceLock = FindInstanceLock(_instanceLocksByPlayer, playerGuid, entries);

		// Ignore expired and not extended locks
		if (instanceLock != null && (!instanceLock.IsExpired() || instanceLock.IsExtended() || !ignoreExpired))
			return instanceLock;

		if (ignoreTemporary)
			return null;

		return FindInstanceLock(_temporaryInstanceLocksByPlayer, playerGuid, entries);
	}

	public ICollection<InstanceLock> GetInstanceLocksForPlayer(ObjectGuid playerGuid)
	{
		if (_instanceLocksByPlayer.TryGetValue(playerGuid, out var dictionary))
			return dictionary.Values;

		return new List<InstanceLock>();
	}

	public InstanceLock CreateInstanceLockForNewInstance(ObjectGuid playerGuid, MapDb2Entries entries, uint instanceId)
	{
		if (!entries.MapDifficulty.HasResetSchedule())
			return null;

		InstanceLock instanceLock;

		if (entries.IsInstanceIdBound())
		{
			SharedInstanceLockData sharedData = new();
			_instanceLockDataById[instanceId] = sharedData;

			instanceLock = new SharedInstanceLock(entries.MapDifficulty.MapID,
												(Difficulty)entries.MapDifficulty.DifficultyID,
												GetNextResetTime(entries),
												0,
												sharedData);
		}
		else
		{
			instanceLock = new InstanceLock(entries.MapDifficulty.MapID,
											(Difficulty)entries.MapDifficulty.DifficultyID,
											GetNextResetTime(entries),
											0);
		}

		if (!_temporaryInstanceLocksByPlayer.ContainsKey(playerGuid))
			_temporaryInstanceLocksByPlayer[playerGuid] = new Dictionary<InstanceLockKey, InstanceLock>();

		_temporaryInstanceLocksByPlayer[playerGuid][entries.GetKey()] = instanceLock;

		Log.outDebug(LogFilter.Instance,
					$"[{entries.Map.Id}-{entries.Map.MapName[Global.WorldMgr.DefaultDbcLocale]} | " +
					$"{entries.MapDifficulty.DifficultyID}-{CliDB.DifficultyStorage.LookupByKey(entries.MapDifficulty.DifficultyID).Name}] Created new temporary instance lock for {playerGuid} in instance {instanceId}");

		return instanceLock;
	}

	public InstanceLock UpdateInstanceLockForPlayer(SQLTransaction trans, ObjectGuid playerGuid, MapDb2Entries entries, InstanceLockUpdateEvent updateEvent)
	{
		var instanceLock = FindActiveInstanceLock(playerGuid, entries, true, true);

		if (instanceLock == null)
			lock (_lockObject)
			{
				// Move lock from temporary storage if it exists there
				// This is to avoid destroying expired locks before any boss is killed in a fresh lock
				// player can still change his mind, exit instance and reactivate old lock
				var playerLocks = _temporaryInstanceLocksByPlayer.LookupByKey(playerGuid);

				if (playerLocks != null)
				{
					var playerInstanceLock = playerLocks.LookupByKey(entries.GetKey());

					if (playerInstanceLock != null)
					{
						instanceLock = playerInstanceLock;
						_instanceLocksByPlayer[playerGuid][entries.GetKey()] = instanceLock;

						playerLocks.Remove(entries.GetKey());

						if (playerLocks.Empty())
							_temporaryInstanceLocksByPlayer.Remove(playerGuid);

						Log.outDebug(LogFilter.Instance,
									$"[{entries.Map.Id}-{entries.Map.MapName[Global.WorldMgr.DefaultDbcLocale]} | " +
									$"{entries.MapDifficulty.DifficultyID}-{CliDB.DifficultyStorage.LookupByKey(entries.MapDifficulty.DifficultyID).Name}] Promoting temporary lock to permanent for {playerGuid} in instance {updateEvent.InstanceId}");
					}
				}
			}

		if (instanceLock == null)
		{
			if (entries.IsInstanceIdBound())
			{
				var sharedDataItr = _instanceLockDataById.LookupByKey(updateEvent.InstanceId);

				instanceLock = new SharedInstanceLock(entries.MapDifficulty.MapID,
													(Difficulty)entries.MapDifficulty.DifficultyID,
													GetNextResetTime(entries),
													updateEvent.InstanceId,
													sharedDataItr);
			}
			else
			{
				instanceLock = new InstanceLock(entries.MapDifficulty.MapID,
												(Difficulty)entries.MapDifficulty.DifficultyID,
												GetNextResetTime(entries),
												updateEvent.InstanceId);
			}

			lock (_lockObject)
			{
				_instanceLocksByPlayer[playerGuid][entries.GetKey()] = instanceLock;
			}

			Log.outDebug(LogFilter.Instance,
						$"[{entries.Map.Id}-{entries.Map.MapName[Global.WorldMgr.DefaultDbcLocale]} | " +
						$"{entries.MapDifficulty.DifficultyID}-{CliDB.DifficultyStorage.LookupByKey(entries.MapDifficulty.DifficultyID).Name}] Created new instance lock for {playerGuid} in instance {updateEvent.InstanceId}");
		}
		else
		{
			instanceLock.SetInstanceId(updateEvent.InstanceId);
		}

		instanceLock.GetData().Data = updateEvent.NewData;

		if (updateEvent.CompletedEncounter != null)
		{
			instanceLock.GetData().CompletedEncountersMask |= 1u << updateEvent.CompletedEncounter.Bit;

			Log.outDebug(LogFilter.Instance,
						$"[{entries.Map.Id}-{entries.Map.MapName[Global.WorldMgr.DefaultDbcLocale]} | " +
						$"{entries.MapDifficulty.DifficultyID}-{CliDB.DifficultyStorage.LookupByKey(entries.MapDifficulty.DifficultyID).Name}] " +
						$"Instance lock for {playerGuid} in instance {updateEvent.InstanceId} gains completed encounter [{updateEvent.CompletedEncounter.Id}-{updateEvent.CompletedEncounter.Name[Global.WorldMgr.DefaultDbcLocale]}]");
		}

		// Synchronize map completed encounters into players completed encounters for UI
		if (!entries.MapDifficulty.IsUsingEncounterLocks())
			instanceLock.GetData().CompletedEncountersMask |= updateEvent.InstanceCompletedEncountersMask;

		if (updateEvent.EntranceWorldSafeLocId.HasValue)
			instanceLock.GetData().EntranceWorldSafeLocId = updateEvent.EntranceWorldSafeLocId.Value;

		if (instanceLock.IsExpired())
		{
			instanceLock.SetExpiryTime(GetNextResetTime(entries));
			instanceLock.SetExtended(false);

			Log.outDebug(LogFilter.Instance,
						$"[{entries.Map.Id}-{entries.Map.MapName[Global.WorldMgr.DefaultDbcLocale]} | " +
						$"{entries.MapDifficulty.DifficultyID}-{CliDB.DifficultyStorage.LookupByKey(entries.MapDifficulty.DifficultyID).Name}] Expired instance lock for {playerGuid} in instance {updateEvent.InstanceId} is now active");
		}

		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_CHARACTER_INSTANCE_LOCK);
		stmt.AddValue(0, playerGuid.Counter);
		stmt.AddValue(1, entries.MapDifficulty.MapID);
		stmt.AddValue(2, entries.MapDifficulty.LockID);
		trans.Append(stmt);

		stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_CHARACTER_INSTANCE_LOCK);
		stmt.AddValue(0, playerGuid.Counter);
		stmt.AddValue(1, entries.MapDifficulty.MapID);
		stmt.AddValue(2, entries.MapDifficulty.LockID);
		stmt.AddValue(3, instanceLock.GetInstanceId());
		stmt.AddValue(4, entries.MapDifficulty.DifficultyID);
		stmt.AddValue(5, instanceLock.GetData().Data);
		stmt.AddValue(6, instanceLock.GetData().CompletedEncountersMask);
		stmt.AddValue(7, instanceLock.GetData().EntranceWorldSafeLocId);
		stmt.AddValue(8, (ulong)Time.DateTimeToUnixTime(instanceLock.GetExpiryTime()));
		stmt.AddValue(9, instanceLock.IsExtended() ? 1 : 0);
		trans.Append(stmt);

		return instanceLock;
	}

	public void UpdateSharedInstanceLock(SQLTransaction trans, InstanceLockUpdateEvent updateEvent)
	{
		var sharedData = _instanceLockDataById.LookupByKey(updateEvent.InstanceId);
		sharedData.Data = updateEvent.NewData;
		sharedData.InstanceId = updateEvent.InstanceId;

		if (updateEvent.CompletedEncounter != null)
		{
			sharedData.CompletedEncountersMask |= 1u << updateEvent.CompletedEncounter.Bit;
			Log.outDebug(LogFilter.Instance, $"Instance {updateEvent.InstanceId} gains completed encounter [{updateEvent.CompletedEncounter.Id}-{updateEvent.CompletedEncounter.Name[Global.WorldMgr.DefaultDbcLocale]}]");
		}

		if (updateEvent.EntranceWorldSafeLocId.HasValue)
			sharedData.EntranceWorldSafeLocId = updateEvent.EntranceWorldSafeLocId.Value;

		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_INSTANCE);
		stmt.AddValue(0, sharedData.InstanceId);
		trans.Append(stmt);

		stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_INSTANCE);
		stmt.AddValue(0, sharedData.InstanceId);
		stmt.AddValue(1, sharedData.Data);
		stmt.AddValue(2, sharedData.CompletedEncountersMask);
		stmt.AddValue(3, sharedData.EntranceWorldSafeLocId);
		trans.Append(stmt);
	}

	public void OnSharedInstanceLockDataDelete(uint instanceId)
	{
		if (_unloading)
			return;

		_instanceLockDataById.Remove(instanceId);
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_INSTANCE);
		stmt.AddValue(0, instanceId);
		DB.Characters.Execute(stmt);
		Log.outDebug(LogFilter.Instance, $"Deleting instance {instanceId} as it is no longer referenced by any player");
	}

	public Tuple<DateTime, DateTime> UpdateInstanceLockExtensionForPlayer(ObjectGuid playerGuid, MapDb2Entries entries, bool extended)
	{
		var instanceLock = FindActiveInstanceLock(playerGuid, entries, true, false);

		if (instanceLock != null)
		{
			var oldExpiryTime = instanceLock.GetEffectiveExpiryTime();
			instanceLock.SetExtended(extended);
			var stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_CHARACTER_INSTANCE_LOCK_EXTENSION);
			stmt.AddValue(0, extended ? 1 : 0);
			stmt.AddValue(1, playerGuid.Counter);
			stmt.AddValue(2, entries.MapDifficulty.MapID);
			stmt.AddValue(3, entries.MapDifficulty.LockID);
			DB.Characters.Execute(stmt);

			Log.outDebug(LogFilter.Instance,
						$"[{entries.Map.Id}-{entries.Map.MapName[Global.WorldMgr.DefaultDbcLocale]} | " +
						$"{entries.MapDifficulty.DifficultyID}-{CliDB.DifficultyStorage.LookupByKey(entries.MapDifficulty.DifficultyID).Name}] Instance lock for {playerGuid} is {(extended ? "now" : "no longer")} extended");

			return Tuple.Create(oldExpiryTime, instanceLock.GetEffectiveExpiryTime());
		}

		return Tuple.Create(DateTime.MinValue, DateTime.MinValue);
	}

	/// <summary>
	///  Resets instances that match given filter - for use in GM commands
	/// </summary>
	/// <param name="playerGuid"> Guid of player whose locks will be removed </param>
	/// <param name="mapId"> (Optional) Map id of instance locks to reset </param>
	/// <param name="difficulty"> (Optional) Difficulty of instance locks to reset </param>
	/// <param name="locksReset"> All locks that were reset </param>
	/// <param name="locksFailedToReset"> Locks that could not be reset because they are used by existing instance map </param>
	public void ResetInstanceLocksForPlayer(ObjectGuid playerGuid, uint? mapId, Difficulty? difficulty, List<InstanceLock> locksReset, List<InstanceLock> locksFailedToReset)
	{
		var playerLocks = _instanceLocksByPlayer.LookupByKey(playerGuid);

		if (playerLocks == null)
			return;

		foreach (var playerLockPair in playerLocks)
		{
			if (playerLockPair.Value.IsInUse())
			{
				locksFailedToReset.Add(playerLockPair.Value);

				continue;
			}

			if (mapId.HasValue && mapId.Value != playerLockPair.Value.GetMapId())
				continue;

			if (difficulty.HasValue && difficulty.Value != playerLockPair.Value.GetDifficultyId())
				continue;

			locksReset.Add(playerLockPair.Value);
		}

		if (!locksReset.Empty())
		{
			SQLTransaction trans = new();

			foreach (var instanceLock in locksReset)
			{
				MapDb2Entries entries = new(instanceLock.GetMapId(), instanceLock.GetDifficultyId());
				var newExpiryTime = GetNextResetTime(entries) - TimeSpan.FromSeconds(entries.MapDifficulty.GetRaidDuration());
				// set reset time to last reset time
				instanceLock.SetExpiryTime(newExpiryTime);
				instanceLock.SetExtended(false);

				var stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_CHARACTER_INSTANCE_LOCK_FORCE_EXPIRE);
				stmt.AddValue(0, (ulong)Time.DateTimeToUnixTime(newExpiryTime));
				stmt.AddValue(1, playerGuid.Counter);
				stmt.AddValue(2, entries.MapDifficulty.MapID);
				stmt.AddValue(3, entries.MapDifficulty.LockID);
				trans.Append(stmt);
			}

			DB.Characters.CommitTransaction(trans);
		}
	}

	/// <summary>
	///  Retrieves instance lock statistics - for use in GM commands
	/// </summary>
	/// <returns> Statistics info </returns>
	public InstanceLocksStatistics GetStatistics()
	{
		InstanceLocksStatistics statistics;
		statistics.InstanceCount = _instanceLockDataById.Count;
		statistics.PlayerCount = _instanceLocksByPlayer.Count;

		return statistics;
	}

	public DateTime GetNextResetTime(MapDb2Entries entries)
	{
		var dateTime = GameTime.GetDateAndTime();
		var resetHour = WorldConfig.GetIntValue(WorldCfg.ResetScheduleHour);

		var hour = 0;
		var day = 0;

		switch (entries.MapDifficulty.ResetInterval)
		{
			case MapDifficultyResetInterval.Daily:
			{
				if (dateTime.Hour >= resetHour)
					day++;

				hour = resetHour;

				break;
			}
			case MapDifficultyResetInterval.Weekly:
			{
				var resetDay = WorldConfig.GetIntValue(WorldCfg.ResetScheduleWeekDay);
				var daysAdjust = resetDay - dateTime.Day;

				if (dateTime.Day > resetDay || (dateTime.Day == resetDay && dateTime.Hour >= resetHour))
					daysAdjust += 7; // passed it for current week, grab time from next week

				hour = resetHour;
				day += daysAdjust;

				break;
			}
			default:
				break;
		}

		return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day + day, hour, 0, 0);
	}
}