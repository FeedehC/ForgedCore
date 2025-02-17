﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Framework.Configuration;
using Framework.Constants;
using Framework.Database;
using Framework.Realm;
using Framework.Threading;
using Game.BattlePets;
using Game.Chat;
using Game.Collision;
using Game.DataStorage;
using Game.Entities;
using Game.Maps;
using Game.Networking;
using Game.Networking.Packets;
using Game.Scripting;
using Game.Scripting.Interfaces.IServer;
using Game.Scripting.Interfaces.IWorld;
using Game.Spells;

namespace Game;

public class WorldManager : Singleton<WorldManager>
{
	public const string NextCurrencyResetTimeVarId = "NextCurrencyResetTime";
	public const string NextWeeklyQuestResetTimeVarId = "NextWeeklyQuestResetTime";
	public const string NextBGRandomDailyResetTimeVarId = "NextBGRandomDailyResetTime";
	public const string CharacterDatabaseCleaningFlagsVarId = "PersistentCharacterCleanFlags";
	public const string NextGuildDailyResetTimeVarId = "NextGuildDailyResetTime";
	public const string NextMonthlyQuestResetTimeVarId = "NextMonthlyQuestResetTime";
	public const string NextDailyQuestResetTimeVarId = "NextDailyQuestResetTime";
	public const string NextOldCalendarEventDeletionTimeVarId = "NextOldCalendarEventDeletionTime";
	public const string NextGuildWeeklyResetTimeVarId = "NextGuildWeeklyResetTime";
	public bool IsStopped;
	readonly Dictionary<byte, Autobroadcast> _autobroadcasts = new();
	readonly Dictionary<WorldTimers, IntervalTimer> _timers = new();
	readonly ConcurrentDictionary<uint, WorldSession> _sessions = new();
	readonly MultiMap<ObjectGuid, WorldSession> _sessionsByBnetGuid = new();
	readonly Dictionary<uint, long> _disconnects = new();
	readonly Dictionary<string, int> _worldVariables = new();
	readonly List<string> _motd = new();
	readonly List<WorldSession> _queuedPlayer = new();
	readonly ConcurrentQueue<WorldSession> _addSessQueue = new();
	readonly ConcurrentQueue<Tuple<WorldSocket, ulong>> _linkSocketQueue = new();
	readonly AsyncCallbackProcessor<QueryCallback> _queryProcessor = new();
	readonly Realm _realm;
	readonly WorldUpdateTime _worldUpdateTime;
	readonly object _guidAlertLock = new();
	readonly LimitedThreadTaskManager _taskManager = new(10);

	uint _shutdownTimer;
	ShutdownMask _shutdownMask;
	ShutdownExitCode _exitCode;

	CleaningFlags _cleaningFlags;

	float _maxVisibleDistanceOnContinents = SharedConst.DefaultVisibilityDistance;
	float _maxVisibleDistanceInInstances = SharedConst.DefaultVisibilityInstance;
	float _maxVisibleDistanceInBg = SharedConst.DefaultVisibilityBGAreans;
	float _maxVisibleDistanceInArenas = SharedConst.DefaultVisibilityBGAreans;

	int _visibilityNotifyPeriodOnContinents = SharedConst.DefaultVisibilityNotifyPeriod;
	int _visibilityNotifyPeriodInInstances = SharedConst.DefaultVisibilityNotifyPeriod;
	int _visibilityNotifyPeriodInBg = SharedConst.DefaultVisibilityNotifyPeriod;
	int _visibilityNotifyPeriodInArenas = SharedConst.DefaultVisibilityNotifyPeriod;

	bool _isClosed;
	long _mailTimer;
	long _timerExpires;
	long _blackmarketTimer;
	uint _maxActiveSessionCount;
	uint _maxQueuedSessionCount;
	uint _playerCount;
	uint _maxPlayerCount;
	uint _playerLimit;
	AccountTypes _allowedSecurityLevel;
	Locale _defaultDbcLocale;       // from config for one from loaded DBC locales
	BitSet _availableDbcLocaleMask; // by loaded DBC

	// scheduled reset times
	long _nextDailyQuestReset;
	long _nextWeeklyQuestReset;
	long _nextMonthlyQuestReset;
	long _nextRandomBgReset;
	long _nextCalendarOldEventsDeletionTime;
	long _nextGuildReset;
	long _nextCurrencyReset;

	string _dataPath;

	string _guidWarningMsg;
	string _alertRestartReason;

	bool _guidWarn;
	bool _guidAlert;
	uint _warnDiff;
	long _warnShutdownTime;

	uint _maxSkill = 0;

	public bool IsClosed => _isClosed;

	public List<string> Motd => _motd;

	public List<WorldSession> AllSessions => _sessions.Values.ToList();

	public int ActiveAndQueuedSessionCount => _sessions.Count;

	public int ActiveSessionCount => _sessions.Count - _queuedPlayer.Count;

	public int QueuedSessionCount => _queuedPlayer.Count;

	// Get the maximum number of parallel sessions on the server since last reboot
	public uint MaxQueuedSessionCount => _maxQueuedSessionCount;

	public uint MaxActiveSessionCount => _maxActiveSessionCount;

	public uint PlayerCount => _playerCount;

	public uint MaxPlayerCount => _maxPlayerCount;

	public AccountTypes PlayerSecurityLimit
	{
		get => _allowedSecurityLevel;
		set
		{
			var sec = value < AccountTypes.Console ? value : AccountTypes.Player;
			var update = sec > _allowedSecurityLevel;
			_allowedSecurityLevel = sec;

			if (update)
				KickAllLess(_allowedSecurityLevel);
		}
	}

	public uint PlayerAmountLimit
	{
		get => _playerLimit;
		set => _playerLimit = value;
	}

	/// Get the path where data (dbc, maps) are stored on disk
	public string DataPath
	{
		get => _dataPath;
		set => _dataPath = value;
	}

	public long NextDailyQuestsResetTime
	{
		get => _nextDailyQuestReset;
		set => _nextDailyQuestReset = value;
	}

	public long NextWeeklyQuestsResetTime
	{
		get => _nextWeeklyQuestReset;
		set => _nextWeeklyQuestReset = value;
	}

	public long NextMonthlyQuestsResetTime
	{
		get => _nextMonthlyQuestReset;
		set => _nextMonthlyQuestReset = value;
	}

	public uint ConfigMaxSkillValue
	{
		get
		{
			if (_maxSkill == 0)
			{
				var lvl = WorldConfig.GetIntValue(WorldCfg.MaxPlayerLevel);

				_maxSkill = (uint)(lvl > 60 ? 300 + ((lvl - 60) * 75) / 10 : lvl * 5);
			}

			return _maxSkill;
		}
	}

	public bool IsShuttingDown => _shutdownTimer > 0;

	public uint ShutDownTimeLeft => _shutdownTimer;

	public int ExitCode => (int)_exitCode;

	public bool IsPvPRealm
	{
		get
		{
			var realmtype = (RealmType)WorldConfig.GetIntValue(WorldCfg.GameType);

			return (realmtype == RealmType.PVP || realmtype == RealmType.RPPVP || realmtype == RealmType.FFAPVP);
		}
	}

	public bool IsFFAPvPRealm => WorldConfig.GetIntValue(WorldCfg.GameType) == (int)RealmType.FFAPVP;

	public Locale DefaultDbcLocale => _defaultDbcLocale;

	public Realm Realm => _realm;

	public RealmId RealmId => _realm.Id;

	public uint VirtualRealmAddress => _realm.Id.GetAddress();

	public float MaxVisibleDistanceOnContinents => _maxVisibleDistanceOnContinents;

	public float MaxVisibleDistanceInInstances => _maxVisibleDistanceInInstances;

	public float MaxVisibleDistanceInBG => _maxVisibleDistanceInBg;

	public float MaxVisibleDistanceInArenas => _maxVisibleDistanceInArenas;

	public int VisibilityNotifyPeriodOnContinents => _visibilityNotifyPeriodOnContinents;

	public int VisibilityNotifyPeriodInInstances => _visibilityNotifyPeriodInInstances;

	public int VisibilityNotifyPeriodInBG => _visibilityNotifyPeriodInBg;

	public int VisibilityNotifyPeriodInArenas => _visibilityNotifyPeriodInArenas;

	public CleaningFlags CleaningFlags
	{
		get => _cleaningFlags;
		set => _cleaningFlags = value;
	}

	public bool IsGuidWarning => _guidWarn;

	public bool IsGuidAlert => _guidAlert;

	public WorldUpdateTime WorldUpdateTime => _worldUpdateTime;

	WorldManager()
	{
		foreach (WorldTimers timer in Enum.GetValues(typeof(WorldTimers)))
			_timers[timer] = new IntervalTimer();

		_allowedSecurityLevel = AccountTypes.Player;

		_realm = new Realm();

		_worldUpdateTime = new WorldUpdateTime();
		_warnShutdownTime = GameTime.GetGameTime();
	}

	public Player FindPlayerInZone(uint zone)
	{
		foreach (var session in _sessions)
		{
			var player = session.Value.Player;

			if (player == null)
				continue;

			if (player.IsInWorld && player.Zone == zone)
				// Used by the weather system. We return the player to broadcast the change weather message to him and all players in the zone.
				return player;
		}

		return null;
	}

	public void SetClosed(bool val)
	{
		_isClosed = val;
		Global.ScriptMgr.ForEach<IWorldOnOpenStateChange>(p => p.OnOpenStateChange(!val));
	}

	public void LoadDBAllowedSecurityLevel()
	{
		var stmt = DB.Login.GetPreparedStatement(LoginStatements.SEL_REALMLIST_SECURITY_LEVEL);
		stmt.AddValue(0, (int)_realm.Id.Index);
		var result = DB.Login.Query(stmt);

		if (!result.IsEmpty())
			PlayerSecurityLimit = (AccountTypes)result.Read<byte>(0);
	}

	public void SetMotd(string motd)
	{
		Global.ScriptMgr.ForEach<IWorldOnMotdChange>(p => p.OnMotdChange(motd));

		_motd.Clear();
		_motd.AddRange(motd.Split('@'));
	}

	public void TriggerGuidWarning()
	{
		// Lock this only to prevent multiple maps triggering at the same time
		lock (_guidAlertLock)
		{
			var gameTime = GameTime.GetGameTime();
			var today = (gameTime / Time.Day) * Time.Day;

			// Check if our window to restart today has passed. 5 mins until quiet time
			while (gameTime >= Time.GetLocalHourTimestamp(today, WorldConfig.GetUIntValue(WorldCfg.RespawnRestartQuietTime)) - 1810)
				today += Time.Day;

			// Schedule restart for 30 minutes before quiet time, or as long as we have
			_warnShutdownTime = Time.GetLocalHourTimestamp(today, WorldConfig.GetUIntValue(WorldCfg.RespawnRestartQuietTime)) - 1800;

			_guidWarn = true;
			SendGuidWarning();
		}
	}

	public void TriggerGuidAlert()
	{
		// Lock this only to prevent multiple maps triggering at the same time
		lock (_guidAlertLock)
		{
			DoGuidAlertRestart();
			_guidAlert = true;
			_guidWarn = false;
		}
	}

	public WorldSession FindSession(uint id)
	{
		return _sessions.LookupByKey(id);
	}

	public void AddSession(WorldSession s)
	{
		_addSessQueue.Enqueue(s);
	}

	public void AddInstanceSocket(WorldSocket sock, ulong connectToKey)
	{
		_linkSocketQueue.Enqueue(Tuple.Create(sock, connectToKey));
	}

	public void SetInitialWorldSettings()
	{
		LoadRealmInfo();

		Log.SetRealmId(_realm.Id.Index);

		LoadConfigSettings();

		// Initialize Allowed Security Level
		LoadDBAllowedSecurityLevel();

		Global.ObjectMgr.SetHighestGuids();

		if (!TerrainManager.ExistMapAndVMap(0, -6240.32f, 331.033f) || !TerrainManager.ExistMapAndVMap(0, -8949.95f, -132.493f) || !TerrainManager.ExistMapAndVMap(1, -618.518f, -4251.67f) || !TerrainManager.ExistMapAndVMap(0, 1676.35f, 1677.45f) || !TerrainManager.ExistMapAndVMap(1, 10311.3f, 832.463f) || !TerrainManager.ExistMapAndVMap(1, -2917.58f, -257.98f) || (WorldConfig.GetIntValue(WorldCfg.Expansion) != 0 && (!TerrainManager.ExistMapAndVMap(530, 10349.6f, -6357.29f) || !TerrainManager.ExistMapAndVMap(530, -3961.64f, -13931.2f))))
		{
			Log.outError(LogFilter.ServerLoading, "Unable to load map and vmap data for starting zones - server shutting down!");
			Environment.Exit(1);
		}

		// Initialize pool manager
		Global.PoolMgr.Initialize();

		// Initialize game event manager
		Global.GameEventMgr.Initialize();

		Log.outInfo(LogFilter.ServerLoading, "Loading Cypher Strings...");

		if (!Global.ObjectMgr.LoadCypherStrings())
			Environment.Exit(1);

		// not send custom type REALM_FFA_PVP to realm list
		var server_type = IsFFAPvPRealm ? RealmType.PVP : (RealmType)WorldConfig.GetIntValue(WorldCfg.GameType);
		var realm_zone = WorldConfig.GetUIntValue(WorldCfg.RealmZone);

		DB.Login.Execute("UPDATE realmlist SET icon = {0}, timezone = {1} WHERE id = '{2}'", (byte)server_type, realm_zone, _realm.Id.Index); // One-time query

		Log.outInfo(LogFilter.ServerLoading, "Initialize DataStorage...");
		// Load DB2s
		_availableDbcLocaleMask = CliDB.LoadStores(_dataPath, _defaultDbcLocale);

		if (_availableDbcLocaleMask == null || !_availableDbcLocaleMask[(int)_defaultDbcLocale])
		{
			Log.outFatal(LogFilter.ServerLoading, $"Unable to load db2 files for {_defaultDbcLocale} locale specified in DBC.Locale config!");
			Environment.Exit(1);
		}

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObject models...");

		if (!GameObjectModel.LoadGameObjectModelList())
		{
			Log.outFatal(LogFilter.ServerLoading, "Unable to load gameobject models (part of vmaps), objects using WMO models will crash the client - server shutting down!");
			Environment.Exit(1);
		}

		Log.outInfo(LogFilter.ServerLoading, "Loading hotfix blobs...");
		Global.DB2Mgr.LoadHotfixBlob(_availableDbcLocaleMask);

		Log.outInfo(LogFilter.ServerLoading, "Loading hotfix info...");
		Global.DB2Mgr.LoadHotfixData();

		Log.outInfo(LogFilter.ServerLoading, "Loading hotfix optional data...");
		Global.DB2Mgr.LoadHotfixOptionalData(_availableDbcLocaleMask);

		//- Load M2 fly by cameras
		M2Storage.LoadM2Cameras(_dataPath);

		//- Load GameTables
		CliDB.LoadGameTables(_dataPath);

		//Load weighted graph on taxi nodes path
		TaxiPathGraph.Initialize();

		MultiMap<uint, uint> mapData = new();

		foreach (var mapEntry in CliDB.MapStorage.Values)
			if (mapEntry.ParentMapID != -1)
			{
				mapData.Add((uint)mapEntry.ParentMapID, mapEntry.Id);
			}
			else if (mapEntry.CosmeticParentMapID != -1)
			{
				mapData.Add((uint)mapEntry.CosmeticParentMapID, mapEntry.Id);
			}

		Global.TerrainMgr.InitializeParentMapData(mapData);

		Global.VMapMgr.Initialize(mapData);
		Global.MMapMgr.Initialize(mapData);

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo Storage...");
		Global.SpellMgr.LoadSpellInfoStore();

		Log.outInfo(LogFilter.ServerLoading, "Loading serverside spells...");
		Global.SpellMgr.LoadSpellInfoServerside();

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo corrections...");
		Global.SpellMgr.LoadSpellInfoCorrections();

		Log.outInfo(LogFilter.ServerLoading, "Loading SkillLineAbility Data...");
		Global.SpellMgr.LoadSkillLineAbilityMap();

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo custom attributes...");
		Global.SpellMgr.LoadSpellInfoCustomAttributes();

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo diminishing infos...");
		Global.SpellMgr.LoadSpellInfoDiminishing();

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo immunity infos...");
		Global.SpellMgr.LoadSpellInfoImmunities();

		Log.outInfo(LogFilter.ServerLoading, "Loading PetFamilySpellsStore Data...");
		Global.SpellMgr.LoadPetFamilySpellsStore();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Totem models...");
		Global.SpellMgr.LoadSpellTotemModel();

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo late corrections...");
		Global.SpellMgr.LoadSpellInfosLateFix();

		Log.outInfo(LogFilter.ServerLoading, "Loading Traits...");
		TraitMgr.Load();

		Log.outInfo(LogFilter.ServerLoading, "Loading languages...");
		Global.LanguageMgr.LoadLanguages();

		Log.outInfo(LogFilter.ServerLoading, "Loading languages words...");
		Global.LanguageMgr.LoadLanguagesWords();

		Log.outInfo(LogFilter.ServerLoading, "Loading Instance Template...");
		Global.ObjectMgr.LoadInstanceTemplate();

		// Must be called before `respawn` data
		Log.outInfo(LogFilter.ServerLoading, "Loading instances...");

		Global.MapMgr.InitInstanceIds();
		Global.InstanceLockMgr.Load();

		Log.outInfo(LogFilter.ServerLoading, "Loading Localization strings...");
		var oldMSTime = Time.MSTime;
		Global.ObjectMgr.LoadCreatureLocales();
		Global.ObjectMgr.LoadGameObjectLocales();
		Global.ObjectMgr.LoadQuestTemplateLocale();
		Global.ObjectMgr.LoadQuestOfferRewardLocale();
		Global.ObjectMgr.LoadQuestRequestItemsLocale();
		Global.ObjectMgr.LoadQuestObjectivesLocale();
		Global.ObjectMgr.LoadPageTextLocales();
		Global.ObjectMgr.LoadGossipMenuItemsLocales();
		Global.ObjectMgr.LoadPointOfInterestLocales();
		Log.outInfo(LogFilter.ServerLoading, "Localization strings loaded in {0} ms", Time.GetMSTimeDiffToNow(oldMSTime));

		Log.outInfo(LogFilter.ServerLoading, "Loading Account Roles and Permissions...");
		Global.AccountMgr.LoadRBAC();

		Log.outInfo(LogFilter.ServerLoading, "Loading Page Texts...");
		Global.ObjectMgr.LoadPageTexts();

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObject Template...");
		Global.ObjectMgr.LoadGameObjectTemplate();

		Log.outInfo(LogFilter.ServerLoading, "Loading Game Object template addons...");
		Global.ObjectMgr.LoadGameObjectTemplateAddons();

		Log.outInfo(LogFilter.ServerLoading, "Loading Transport Templates...");
		Global.TransportMgr.LoadTransportTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Transport animations and rotations...");
		Global.TransportMgr.LoadTransportAnimationAndRotation();

		Log.outInfo(LogFilter.ServerLoading, "Loading Transport spawns...");
		Global.TransportMgr.LoadTransportSpawns();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Rank Data...");
		Global.SpellMgr.LoadSpellRanks();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Required Data...");
		Global.SpellMgr.LoadSpellRequired();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Group types...");
		Global.SpellMgr.LoadSpellGroups();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Learn Skills...");
		Global.SpellMgr.LoadSpellLearnSkills();

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellInfo SpellSpecific and AuraState...");
		Global.SpellMgr.LoadSpellInfoSpellSpecificAndAuraState();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Learn Spells...");
		Global.SpellMgr.LoadSpellLearnSpells();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Proc conditions and data...");
		Global.SpellMgr.LoadSpellProcs();

		Log.outInfo(LogFilter.ServerLoading, "Loading Aggro Spells Definitions...");
		Global.SpellMgr.LoadSpellThreats();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell Group Stack Rules...");
		Global.SpellMgr.LoadSpellGroupStackRules();

		Log.outInfo(LogFilter.ServerLoading, "Loading NPC Texts...");
		Global.ObjectMgr.LoadNPCText();

		Log.outInfo(LogFilter.ServerLoading, "Loading Enchant Spells Proc datas...");
		Global.SpellMgr.LoadSpellEnchantProcData();

		Log.outInfo(LogFilter.ServerLoading, "Loading Random item bonus list definitions...");
		ItemEnchantmentManager.LoadItemRandomBonusListTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Disables"); // must be before loading quests and items
		Global.DisableMgr.LoadDisables();

		Log.outInfo(LogFilter.ServerLoading, "Loading Items..."); // must be after LoadRandomEnchantmentsTable and LoadPageTexts
		Global.ObjectMgr.LoadItemTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Item set names..."); // must be after LoadItemPrototypes
		Global.ObjectMgr.LoadItemTemplateAddon();

		Log.outInfo(LogFilter.ServerLoading, "Loading Item Scripts..."); // must be after LoadItemPrototypes
		Global.ObjectMgr.LoadItemScriptNames();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Model Based Info Data...");
		Global.ObjectMgr.LoadCreatureModelInfo();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature templates...");
		Global.ObjectMgr.LoadCreatureTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Equipment templates...");
		Global.ObjectMgr.LoadEquipmentTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature template addons...");
		Global.ObjectMgr.LoadCreatureTemplateAddons();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature template scaling...");
		Global.ObjectMgr.LoadCreatureScalingData();

		Log.outInfo(LogFilter.ServerLoading, "Loading Reputation Reward Rates...");
		Global.ObjectMgr.LoadReputationRewardRate();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Reputation OnKill Data...");
		Global.ObjectMgr.LoadReputationOnKill();

		Log.outInfo(LogFilter.ServerLoading, "Loading Reputation Spillover Data...");
		Global.ObjectMgr.LoadReputationSpilloverTemplate();

		Log.outInfo(LogFilter.ServerLoading, "Loading Points Of Interest Data...");
		Global.ObjectMgr.LoadPointsOfInterest();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Base Stats...");
		Global.ObjectMgr.LoadCreatureClassLevelStats();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spawn Group Templates...");
		Global.ObjectMgr.LoadSpawnGroupTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Data...");
		Global.ObjectMgr.LoadCreatures();

		Log.outInfo(LogFilter.ServerLoading, "Loading Temporary Summon Data...");
		Global.ObjectMgr.LoadTempSummons(); // must be after LoadCreatureTemplates() and LoadGameObjectTemplates()

		Log.outInfo(LogFilter.ServerLoading, "Loading pet levelup spells...");
		Global.SpellMgr.LoadPetLevelupSpellMap();

		Log.outInfo(LogFilter.ServerLoading, "Loading pet default spells additional to levelup spells...");
		Global.SpellMgr.LoadPetDefaultSpells();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Addon Data...");
		Global.ObjectMgr.LoadCreatureAddons();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Movement Overrides...");
		Global.ObjectMgr.LoadCreatureMovementOverrides(); // must be after LoadCreatures()

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObjects...");
		Global.ObjectMgr.LoadGameObjects();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spawn Group Data...");
		Global.ObjectMgr.LoadSpawnGroups();

		Log.outInfo(LogFilter.ServerLoading, "Loading instance spawn groups...");
		Global.ObjectMgr.LoadInstanceSpawnGroups();

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObject Addon Data...");
		Global.ObjectMgr.LoadGameObjectAddons(); // must be after LoadGameObjects()

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObject faction and flags overrides...");
		Global.ObjectMgr.LoadGameObjectOverrides(); // must be after LoadGameObjects()

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObject Quest Items...");
		Global.ObjectMgr.LoadGameObjectQuestItems();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Quest Items...");
		Global.ObjectMgr.LoadCreatureQuestItems();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Linked Respawn...");
		Global.ObjectMgr.LoadLinkedRespawn(); // must be after LoadCreatures(), LoadGameObjects()

		Log.outInfo(LogFilter.ServerLoading, "Loading Weather Data...");
		Global.WeatherMgr.LoadWeatherData();

		Log.outInfo(LogFilter.ServerLoading, "Loading Quests...");
		Global.ObjectMgr.LoadQuests();

		Log.outInfo(LogFilter.ServerLoading, "Checking Quest Disables");
		Global.DisableMgr.CheckQuestDisables(); // must be after loading quests

		Log.outInfo(LogFilter.ServerLoading, "Loading Quest POI");
		Global.ObjectMgr.LoadQuestPOI();

		Log.outInfo(LogFilter.ServerLoading, "Loading Quests Starters and Enders...");
		Global.ObjectMgr.LoadQuestStartersAndEnders(); // must be after quest load

		Log.outInfo(LogFilter.ServerLoading, "Loading Quest Greetings...");
		Global.ObjectMgr.LoadQuestGreetings();
		Global.ObjectMgr.LoadQuestGreetingLocales();

		Log.outInfo(LogFilter.ServerLoading, "Loading Objects Pooling Data...");
		Global.PoolMgr.LoadFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Loading Quest Pooling Data...");
		Global.QuestPoolMgr.LoadFromDB(); // must be after quest templates

		Log.outInfo(LogFilter.ServerLoading, "Loading Game Event Data..."); // must be after loading pools fully
		Global.GameEventMgr.LoadFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Loading NPCSpellClick Data..."); // must be after LoadQuests
		Global.ObjectMgr.LoadNPCSpellClickSpells();

		Log.outInfo(LogFilter.ServerLoading, "Loading Vehicle Templates...");
		Global.ObjectMgr.LoadVehicleTemplate(); // must be after LoadCreatureTemplates()

		Log.outInfo(LogFilter.ServerLoading, "Loading Vehicle Template Accessories...");
		Global.ObjectMgr.LoadVehicleTemplateAccessories(); // must be after LoadCreatureTemplates() and LoadNPCSpellClickSpells()

		Log.outInfo(LogFilter.ServerLoading, "Loading Vehicle Accessories...");
		Global.ObjectMgr.LoadVehicleAccessories(); // must be after LoadCreatureTemplates() and LoadNPCSpellClickSpells()

		Log.outInfo(LogFilter.ServerLoading, "Loading Vehicle Seat Addon Data...");
		Global.ObjectMgr.LoadVehicleSeatAddon(); // must be after loading DBC

		Log.outInfo(LogFilter.ServerLoading, "Loading SpellArea Data..."); // must be after quest load
		Global.SpellMgr.LoadSpellAreas();

		Log.outInfo(LogFilter.ServerLoading, "Loading World locations...");
		Global.ObjectMgr.LoadWorldSafeLocs(); // must be before LoadAreaTriggerTeleports and LoadGraveyardZones

		Log.outInfo(LogFilter.ServerLoading, "Loading AreaTrigger definitions...");
		Global.ObjectMgr.LoadAreaTriggerTeleports();

		Log.outInfo(LogFilter.ServerLoading, "Loading Access Requirements...");
		Global.ObjectMgr.LoadAccessRequirements(); // must be after item template load

		Log.outInfo(LogFilter.ServerLoading, "Loading Quest Area Triggers...");
		Global.ObjectMgr.LoadQuestAreaTriggers(); // must be after LoadQuests

		Log.outInfo(LogFilter.ServerLoading, "Loading Tavern Area Triggers...");
		Global.ObjectMgr.LoadTavernAreaTriggers();

		Log.outInfo(LogFilter.ServerLoading, "Loading AreaTrigger script names...");
		Global.ObjectMgr.LoadAreaTriggerScripts();

		Log.outInfo(LogFilter.ServerLoading, "Loading LFG entrance positions..."); // Must be after areatriggers
		Global.LFGMgr.LoadLFGDungeons();

		Log.outInfo(LogFilter.ServerLoading, "Loading Dungeon boss data...");
		Global.ObjectMgr.LoadInstanceEncounters();

		Log.outInfo(LogFilter.ServerLoading, "Loading LFG rewards...");
		Global.LFGMgr.LoadRewards();

		Log.outInfo(LogFilter.ServerLoading, "Loading Graveyard-zone links...");
		Global.ObjectMgr.LoadGraveyardZones();

		Log.outInfo(LogFilter.ServerLoading, "Loading spell pet auras...");
		Global.SpellMgr.LoadSpellPetAuras();

		Log.outInfo(LogFilter.ServerLoading, "Loading Spell target coordinates...");
		Global.SpellMgr.LoadSpellTargetPositions();

		Log.outInfo(LogFilter.ServerLoading, "Loading linked spells...");
		Global.SpellMgr.LoadSpellLinked();

		Log.outInfo(LogFilter.ServerLoading, "Loading Scenes Templates..."); // must be before LoadPlayerInfo
		Global.ObjectMgr.LoadSceneTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Player Create Data...");
		Global.ObjectMgr.LoadPlayerInfo();

		Log.outInfo(LogFilter.ServerLoading, "Loading Exploration BaseXP Data...");
		Global.ObjectMgr.LoadExplorationBaseXP();

		Log.outInfo(LogFilter.ServerLoading, "Loading Pet Name Parts...");
		Global.ObjectMgr.LoadPetNames();

		Log.outInfo(LogFilter.ServerLoading, "Loading AreaTrigger Templates...");
		Global.AreaTriggerDataStorage.LoadAreaTriggerTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading AreaTrigger Spawns...");
		Global.AreaTriggerDataStorage.LoadAreaTriggerSpawns();

		Log.outInfo(LogFilter.ServerLoading, "Loading Conversation Templates...");
		Global.ConversationDataStorage.LoadConversationTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading Player Choices...");
		Global.ObjectMgr.LoadPlayerChoices();

		Log.outInfo(LogFilter.ServerLoading, "Loading Player Choices Locales...");
		Global.ObjectMgr.LoadPlayerChoicesLocale();

		Log.outInfo(LogFilter.ServerLoading, "Loading Jump Charge Params...");
		Global.ObjectMgr.LoadJumpChargeParams();

		CharacterDatabaseCleaner.CleanDatabase();

		Log.outInfo(LogFilter.ServerLoading, "Loading the max pet number...");
		Global.ObjectMgr.LoadPetNumber();

		Log.outInfo(LogFilter.ServerLoading, "Loading pet level stats...");
		Global.ObjectMgr.LoadPetLevelInfo();

		Log.outInfo(LogFilter.ServerLoading, "Loading Player level dependent mail rewards...");
		Global.ObjectMgr.LoadMailLevelRewards();

		// Loot tables
		Loots.LootManager.LoadLootTables();

		Log.outInfo(LogFilter.ServerLoading, "Loading Skill Discovery Table...");
		SkillDiscovery.LoadSkillDiscoveryTable();

		Log.outInfo(LogFilter.ServerLoading, "Loading Skill Extra Item Table...");
		SkillExtraItems.LoadSkillExtraItemTable();

		Log.outInfo(LogFilter.ServerLoading, "Loading Skill Perfection Data Table...");
		SkillPerfectItems.LoadSkillPerfectItemTable();

		Log.outInfo(LogFilter.ServerLoading, "Loading Skill Fishing base level requirements...");
		Global.ObjectMgr.LoadFishingBaseSkillLevel();

		Log.outInfo(LogFilter.ServerLoading, "Loading skill tier info...");
		Global.ObjectMgr.LoadSkillTiers();

		Log.outInfo(LogFilter.ServerLoading, "Loading Criteria Modifier trees...");
		Global.CriteriaMgr.LoadCriteriaModifiersTree();
		Log.outInfo(LogFilter.ServerLoading, "Loading Criteria Lists...");
		Global.CriteriaMgr.LoadCriteriaList();
		Log.outInfo(LogFilter.ServerLoading, "Loading Criteria Data...");
		Global.CriteriaMgr.LoadCriteriaData();
		Log.outInfo(LogFilter.ServerLoading, "Loading Achievements...");
		Global.AchievementMgr.LoadAchievementReferenceList();
		Log.outInfo(LogFilter.ServerLoading, "Loading Achievements Scripts...");
		Global.AchievementMgr.LoadAchievementScripts();
		Log.outInfo(LogFilter.ServerLoading, "Loading Achievement Rewards...");
		Global.AchievementMgr.LoadRewards();
		Log.outInfo(LogFilter.ServerLoading, "Loading Achievement Reward Locales...");
		Global.AchievementMgr.LoadRewardLocales();
		Log.outInfo(LogFilter.ServerLoading, "Loading Completed Achievements...");
		Global.AchievementMgr.LoadCompletedAchievements();

		// Load before guilds and arena teams
		Log.outInfo(LogFilter.ServerLoading, "Loading character cache store...");
		Global.CharacterCacheStorage.LoadCharacterCacheStorage();

		// Load dynamic data tables from the database
		Log.outInfo(LogFilter.ServerLoading, "Loading Auctions...");
		Global.AuctionHouseMgr.LoadAuctions();

		if (WorldConfig.GetBoolValue(WorldCfg.BlackmarketEnabled))
		{
			Log.outInfo(LogFilter.ServerLoading, "Loading Black Market Templates...");
			Global.BlackMarketMgr.LoadTemplates();

			Log.outInfo(LogFilter.ServerLoading, "Loading Black Market Auctions...");
			Global.BlackMarketMgr.LoadAuctions();
		}

		Log.outInfo(LogFilter.ServerLoading, "Loading Guild rewards...");
		Global.GuildMgr.LoadGuildRewards();

		Log.outInfo(LogFilter.ServerLoading, "Loading Guilds...");
		Global.GuildMgr.LoadGuilds();


		Log.outInfo(LogFilter.ServerLoading, "Loading ArenaTeams...");
		Global.ArenaTeamMgr.LoadArenaTeams();

		Log.outInfo(LogFilter.ServerLoading, "Loading Groups...");
		Global.GroupMgr.LoadGroups();

		Log.outInfo(LogFilter.ServerLoading, "Loading ReservedNames...");
		Global.ObjectMgr.LoadReservedPlayersNames();

		Log.outInfo(LogFilter.ServerLoading, "Loading GameObjects for quests...");
		Global.ObjectMgr.LoadGameObjectForQuests();

		Log.outInfo(LogFilter.ServerLoading, "Loading BattleMasters...");
		Global.BattlegroundMgr.LoadBattleMastersEntry();

		Log.outInfo(LogFilter.ServerLoading, "Loading GameTeleports...");
		Global.ObjectMgr.LoadGameTele();

		Log.outInfo(LogFilter.ServerLoading, "Loading Trainers...");
		Global.ObjectMgr.LoadTrainers(); // must be after load CreatureTemplate

		Log.outInfo(LogFilter.ServerLoading, "Loading Gossip menu...");
		Global.ObjectMgr.LoadGossipMenu();

		Log.outInfo(LogFilter.ServerLoading, "Loading Gossip menu options...");
		Global.ObjectMgr.LoadGossipMenuItems();

		Log.outInfo(LogFilter.ServerLoading, "Loading Gossip menu addon...");
		Global.ObjectMgr.LoadGossipMenuAddon();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature trainers...");
		Global.ObjectMgr.LoadCreatureTrainers(); // must be after LoadGossipMenuItems

		Log.outInfo(LogFilter.ServerLoading, "Loading Vendors...");
		Global.ObjectMgr.LoadVendors(); // must be after load CreatureTemplate and ItemTemplate

		Log.outInfo(LogFilter.ServerLoading, "Loading Waypoints...");
		Global.WaypointMgr.Load();

		Log.outInfo(LogFilter.ServerLoading, "Loading SmartAI Waypoints...");
		Global.SmartAIMgr.LoadWaypointFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Formations...");
		FormationMgr.LoadCreatureFormations();

		Log.outInfo(LogFilter.ServerLoading, "Loading World State templates...");
		Global.WorldStateMgr.LoadFromDB(); // must be loaded before battleground, outdoor PvP and conditions

		Log.outInfo(LogFilter.ServerLoading, "Loading Persistend World Variables..."); // must be loaded before Battleground, outdoor PvP and conditions
		LoadPersistentWorldVariables();

		Global.WorldStateMgr.SetValue(WorldStates.CurrentPvpSeasonId, WorldConfig.GetBoolValue(WorldCfg.ArenaSeasonInProgress) ? WorldConfig.GetIntValue(WorldCfg.ArenaSeasonId) : 0, false, null);
		Global.WorldStateMgr.SetValue(WorldStates.PreviousPvpSeasonId, WorldConfig.GetIntValue(WorldCfg.ArenaSeasonId) - (WorldConfig.GetBoolValue(WorldCfg.ArenaSeasonInProgress) ? 1 : 0), false, null);

		Global.ObjectMgr.LoadPhases();

		Log.outInfo(LogFilter.ServerLoading, "Loading Conditions...");
		Global.ConditionMgr.LoadConditions();

		Log.outInfo(LogFilter.ServerLoading, "Loading faction change achievement pairs...");
		Global.ObjectMgr.LoadFactionChangeAchievements();

		Log.outInfo(LogFilter.ServerLoading, "Loading faction change spell pairs...");
		Global.ObjectMgr.LoadFactionChangeSpells();

		Log.outInfo(LogFilter.ServerLoading, "Loading faction change item pairs...");
		Global.ObjectMgr.LoadFactionChangeItems();

		Log.outInfo(LogFilter.ServerLoading, "Loading faction change quest pairs...");
		Global.ObjectMgr.LoadFactionChangeQuests();

		Log.outInfo(LogFilter.ServerLoading, "Loading faction change reputation pairs...");
		Global.ObjectMgr.LoadFactionChangeReputations();

		Log.outInfo(LogFilter.ServerLoading, "Loading faction change title pairs...");
		Global.ObjectMgr.LoadFactionChangeTitles();

		Log.outInfo(LogFilter.ServerLoading, "Loading mount definitions...");
		CollectionMgr.LoadMountDefinitions();

		Log.outInfo(LogFilter.ServerLoading, "Loading GM bugs...");
		Global.SupportMgr.LoadBugTickets();

		Log.outInfo(LogFilter.ServerLoading, "Loading GM complaints...");
		Global.SupportMgr.LoadComplaintTickets();

		Log.outInfo(LogFilter.ServerLoading, "Loading GM suggestions...");
		Global.SupportMgr.LoadSuggestionTickets();

		//Log.outInfo(LogFilter.ServerLoading, "Loading GM surveys...");
		//Global.SupportMgr.LoadSurveys();

		Log.outInfo(LogFilter.ServerLoading, "Loading garrison info...");
		Global.GarrisonMgr.Initialize();

		// Handle outdated emails (delete/return)
		Log.outInfo(LogFilter.ServerLoading, "Returning old mails...");
		Global.ObjectMgr.ReturnOrDeleteOldMails(false);

		Log.outInfo(LogFilter.ServerLoading, "Loading Autobroadcasts...");
		LoadAutobroadcasts();

		// Load and initialize scripts
		Global.ObjectMgr.LoadSpellScripts(); // must be after load Creature/Gameobject(Template/Data)
		Global.ObjectMgr.LoadEventScripts(); // must be after load Creature/Gameobject(Template/Data)
		Global.ObjectMgr.LoadWaypointScripts();

		Log.outInfo(LogFilter.ServerLoading, "Loading spell script names...");
		Global.ObjectMgr.LoadSpellScriptNames();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Texts...");
		Global.CreatureTextMgr.LoadCreatureTexts();

		Log.outInfo(LogFilter.ServerLoading, "Loading Creature Text Locales...");
		Global.CreatureTextMgr.LoadCreatureTextLocales();

		Log.outInfo(LogFilter.ServerLoading, "Initializing Scripts...");
		Global.ScriptMgr.Initialize();
		Global.ScriptMgr.ForEach<IWorldOnConfigLoad>(p => p.OnConfigLoad(false)); // must be done after the ScriptMgr has been properly initialized

		Log.outInfo(LogFilter.ServerLoading, "Validating spell scripts...");
		Global.ObjectMgr.ValidateSpellScripts();

		Log.outInfo(LogFilter.ServerLoading, "Loading SmartAI scripts...");
		Global.SmartAIMgr.LoadFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Loading Calendar data...");
		Global.CalendarMgr.LoadFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Loading Petitions...");
		Global.PetitionMgr.LoadPetitions();

		Log.outInfo(LogFilter.ServerLoading, "Loading Signatures...");
		Global.PetitionMgr.LoadSignatures();

		Log.outInfo(LogFilter.ServerLoading, "Loading Item loot...");
		Global.LootItemStorage.LoadStorageFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Initialize query data...");
		Global.ObjectMgr.InitializeQueriesData(QueryDataGroup.All);

		// Initialize game time and timers
		Log.outInfo(LogFilter.ServerLoading, "Initialize game time and timers");
		GameTime.UpdateGameTimers();

		DB.Login.Execute("INSERT INTO uptime (realmid, starttime, uptime, revision) VALUES({0}, {1}, 0, '{2}')", _realm.Id.Index, GameTime.GetStartTime(), ""); // One-time query

		_timers[WorldTimers.Auctions].Interval = Time.Minute * Time.InMilliseconds;
		_timers[WorldTimers.AuctionsPending].Interval = 250;

		//Update "uptime" table based on configuration entry in minutes.
		_timers[WorldTimers.UpTime]
			.
			//Update "uptime" table based on configuration entry in minutes.
			Interval = 10 * Time.Minute * Time.InMilliseconds;

		//erase corpses every 20 minutes
		_timers[WorldTimers.Corpses]
			. //erase corpses every 20 minutes
			Interval = 20 * Time.Minute * Time.InMilliseconds;

		_timers[WorldTimers.CleanDB].Interval = WorldConfig.GetIntValue(WorldCfg.LogdbClearinterval) * Time.Minute * Time.InMilliseconds;
		_timers[WorldTimers.AutoBroadcast].Interval = WorldConfig.GetIntValue(WorldCfg.AutoBroadcastInterval);

		// check for chars to delete every day
		_timers[WorldTimers.DeleteChars]
			. // check for chars to delete every day
			Interval = Time.Day * Time.InMilliseconds;

		// for AhBot
		_timers[WorldTimers.AhBot]
			.                                                                                       // for AhBot
			Interval = WorldConfig.GetIntValue(WorldCfg.AhbotUpdateInterval) * Time.InMilliseconds; // every 20 sec

		_timers[WorldTimers.GuildSave].Interval = WorldConfig.GetIntValue(WorldCfg.GuildSaveInterval) * Time.Minute * Time.InMilliseconds;

		_timers[WorldTimers.Blackmarket].Interval = 10 * Time.InMilliseconds;

		_blackmarketTimer = 0;

		_timers[WorldTimers.WhoList].Interval = 5 * Time.InMilliseconds; // update who list cache every 5 seconds

		_timers[WorldTimers.ChannelSave].Interval = WorldConfig.GetIntValue(WorldCfg.PreserveCustomChannelInterval) * Time.Minute * Time.InMilliseconds;

		//to set mailtimer to return mails every day between 4 and 5 am
		//mailtimer is increased when updating auctions
		//one second is 1000 -(tested on win system)
		// @todo Get rid of magic numbers
		var localTime = Time.UnixTimeToDateTime(GameTime.GetGameTime()).ToLocalTime();
		var CleanOldMailsTime = WorldConfig.GetIntValue(WorldCfg.CleanOldMailTime);
		_mailTimer = ((((localTime.Hour + (24 - CleanOldMailsTime)) % 24) * Time.Hour * Time.InMilliseconds) / _timers[WorldTimers.Auctions].Interval);
		//1440
		_timerExpires = ((Time.Day * Time.InMilliseconds) / (_timers[(int)WorldTimers.Auctions].Interval));
		Log.outInfo(LogFilter.ServerLoading, "Mail timer set to: {0}, mail return is called every {1} minutes", _mailTimer, _timerExpires);

		//- Initialize MapManager
		Log.outInfo(LogFilter.ServerLoading, "Starting Map System");
		Global.MapMgr.Initialize();

		Log.outInfo(LogFilter.ServerLoading, "Starting Game Event system...");
		var nextGameEvent = Global.GameEventMgr.StartSystem();
		_timers[WorldTimers.Events].Interval = nextGameEvent; //depend on next event

		// Delete all characters which have been deleted X days before
		Player.DeleteOldCharacters();

		Log.outInfo(LogFilter.ServerLoading, "Initializing chat channels...");
		ChannelManager.LoadFromDB();

		Log.outInfo(LogFilter.ServerLoading, "Initializing Opcodes...");
		PacketManager.Initialize();

		Log.outInfo(LogFilter.ServerLoading, "Starting Arena Season...");
		Global.GameEventMgr.StartArenaSeason();

		Global.SupportMgr.Initialize();

		// Initialize Battlegrounds
		Log.outInfo(LogFilter.ServerLoading, "Starting BattlegroundSystem");
		Global.BattlegroundMgr.LoadBattlegroundTemplates();

		// Initialize outdoor pvp
		Log.outInfo(LogFilter.ServerLoading, "Starting Outdoor PvP System");
		Global.OutdoorPvPMgr.InitOutdoorPvP();

		// Initialize Battlefield
		Log.outInfo(LogFilter.ServerLoading, "Starting Battlefield System");
		Global.BattleFieldMgr.InitBattlefield();

		// Initialize Warden
		Log.outInfo(LogFilter.ServerLoading, "Loading Warden Checks...");
		Global.WardenCheckMgr.LoadWardenChecks();

		Log.outInfo(LogFilter.ServerLoading, "Loading Warden Action Overrides...");
		Global.WardenCheckMgr.LoadWardenOverrides();

		Log.outInfo(LogFilter.ServerLoading, "Deleting expired bans...");
		DB.Login.Execute("DELETE FROM ip_banned WHERE unbandate <= UNIX_TIMESTAMP() AND unbandate<>bandate"); // One-time query

		Log.outInfo(LogFilter.ServerLoading, "Initializing quest reset times...");
		InitQuestResetTimes();
		CheckScheduledResetTimes();

		Log.outInfo(LogFilter.ServerLoading, "Calculate random battleground reset time...");
		InitRandomBGResetTime();

		Log.outInfo(LogFilter.ServerLoading, "Calculate deletion of old calendar events time...");
		InitCalendarOldEventsDeletionTime();

		Log.outInfo(LogFilter.ServerLoading, "Calculate Guild cap reset time...");
		InitGuildResetTime();

		Log.outInfo(LogFilter.ServerLoading, "Calculate next currency reset time...");
		InitCurrencyResetTime();

		Log.outInfo(LogFilter.ServerLoading, "Loading race and class expansion requirements...");
		Global.ObjectMgr.LoadRaceAndClassExpansionRequirements();

		Log.outInfo(LogFilter.ServerLoading, "Loading character templates...");
		Global.CharacterTemplateDataStorage.LoadCharacterTemplates();

		Log.outInfo(LogFilter.ServerLoading, "Loading realm names...");
		Global.ObjectMgr.LoadRealmNames();

		Log.outInfo(LogFilter.ServerLoading, "Loading battle pets info...");
		BattlePetMgr.Initialize();

		Log.outInfo(LogFilter.ServerLoading, "Loading scenarios");
		Global.ScenarioMgr.LoadDB2Data();
		Global.ScenarioMgr.LoadDBData();

		Log.outInfo(LogFilter.ServerLoading, "Loading scenario poi data");
		Global.ScenarioMgr.LoadScenarioPOI();

		Log.outInfo(LogFilter.ServerLoading, "Loading phase names...");
		Global.ObjectMgr.LoadPhaseNames();

		ScriptManager.Instance.ForEach<IServerLoadComplete>(s => s.LoadComplete());
	}

	public void LoadConfigSettings(bool reload = false)
	{
		WorldConfig.Load(reload);

		_defaultDbcLocale = (Locale)ConfigMgr.GetDefaultValue("DBC.Locale", 0);

		if (_defaultDbcLocale >= Locale.Total || _defaultDbcLocale == Locale.None)
		{
			Log.outError(LogFilter.ServerLoading, "Incorrect DBC.Locale! Must be >= 0 and < {0} and not {1} (set to 0)", Locale.Total, Locale.None);
			_defaultDbcLocale = Locale.enUS;
		}

		Log.outInfo(LogFilter.ServerLoading, "Using {0} DBC Locale", _defaultDbcLocale);

		// load update time related configs
		_worldUpdateTime.LoadFromConfig();

		PlayerAmountLimit = (uint)ConfigMgr.GetDefaultValue("PlayerLimit", 100);
		SetMotd(ConfigMgr.GetDefaultValue("Motd", "Welcome to a Cypher Core Server."));

		if (reload)
		{
			Global.SupportMgr.SetSupportSystemStatus(WorldConfig.GetBoolValue(WorldCfg.SupportEnabled));
			Global.SupportMgr.SetTicketSystemStatus(WorldConfig.GetBoolValue(WorldCfg.SupportTicketsEnabled));
			Global.SupportMgr.SetBugSystemStatus(WorldConfig.GetBoolValue(WorldCfg.SupportBugsEnabled));
			Global.SupportMgr.SetComplaintSystemStatus(WorldConfig.GetBoolValue(WorldCfg.SupportComplaintsEnabled));
			Global.SupportMgr.SetSuggestionSystemStatus(WorldConfig.GetBoolValue(WorldCfg.SupportSuggestionsEnabled));

			Global.MapMgr.SetMapUpdateInterval(WorldConfig.GetIntValue(WorldCfg.IntervalMapupdate));
			Global.MapMgr.SetGridCleanUpDelay(WorldConfig.GetUIntValue(WorldCfg.IntervalGridclean));

			_timers[WorldTimers.UpTime].Interval = WorldConfig.GetIntValue(WorldCfg.UptimeUpdate) * Time.Minute * Time.InMilliseconds;
			_timers[WorldTimers.UpTime].Reset();

			_timers[WorldTimers.CleanDB].Interval = WorldConfig.GetIntValue(WorldCfg.LogdbClearinterval) * Time.Minute * Time.InMilliseconds;
			_timers[WorldTimers.CleanDB].Reset();


			_timers[WorldTimers.AutoBroadcast].Interval = WorldConfig.GetIntValue(WorldCfg.AutoBroadcastInterval);
			_timers[WorldTimers.AutoBroadcast].Reset();
		}

		for (byte i = 0; i < (int)UnitMoveType.Max; ++i)
			SharedConst.playerBaseMoveSpeed[i] = SharedConst.baseMoveSpeed[i] * WorldConfig.GetFloatValue(WorldCfg.RateMovespeed);

		var rateCreatureAggro = WorldConfig.GetFloatValue(WorldCfg.RateCreatureAggro);
		//visibility on continents
		_maxVisibleDistanceOnContinents = ConfigMgr.GetDefaultValue("Visibility.Distance.Continents", SharedConst.DefaultVisibilityDistance);

		if (_maxVisibleDistanceOnContinents < 45 * rateCreatureAggro)
		{
			Log.outError(LogFilter.ServerLoading, "Visibility.Distance.Continents can't be less max aggro radius {0}", 45 * rateCreatureAggro);
			_maxVisibleDistanceOnContinents = 45 * rateCreatureAggro;
		}
		else if (_maxVisibleDistanceOnContinents > SharedConst.MaxVisibilityDistance)
		{
			Log.outError(LogFilter.ServerLoading, "Visibility.Distance.Continents can't be greater {0}", SharedConst.MaxVisibilityDistance);
			_maxVisibleDistanceOnContinents = SharedConst.MaxVisibilityDistance;
		}

		//visibility in instances
		_maxVisibleDistanceInInstances = ConfigMgr.GetDefaultValue("Visibility.Distance.Instances", SharedConst.DefaultVisibilityInstance);

		if (_maxVisibleDistanceInInstances < 45 * rateCreatureAggro)
		{
			Log.outError(LogFilter.ServerLoading, "Visibility.Distance.Instances can't be less max aggro radius {0}", 45 * rateCreatureAggro);
			_maxVisibleDistanceInInstances = 45 * rateCreatureAggro;
		}
		else if (_maxVisibleDistanceInInstances > SharedConst.MaxVisibilityDistance)
		{
			Log.outError(LogFilter.ServerLoading, "Visibility.Distance.Instances can't be greater {0}", SharedConst.MaxVisibilityDistance);
			_maxVisibleDistanceInInstances = SharedConst.MaxVisibilityDistance;
		}

		//visibility in BG
		_maxVisibleDistanceInBg = ConfigMgr.GetDefaultValue("Visibility.Distance.BG", SharedConst.DefaultVisibilityBGAreans);

		if (_maxVisibleDistanceInBg < 45 * rateCreatureAggro)
		{
			Log.outError(LogFilter.ServerLoading, $"Visibility.Distance.BG can't be less max aggro radius {45 * rateCreatureAggro}");
			_maxVisibleDistanceInBg = 45 * rateCreatureAggro;
		}
		else if (_maxVisibleDistanceInBg > SharedConst.MaxVisibilityDistance)
		{
			Log.outError(LogFilter.ServerLoading, $"Visibility.Distance.BG can't be greater {SharedConst.MaxVisibilityDistance}");
			_maxVisibleDistanceInBg = SharedConst.MaxVisibilityDistance;
		}

		// Visibility in Arenas
		_maxVisibleDistanceInArenas = ConfigMgr.GetDefaultValue("Visibility.Distance.Arenas", SharedConst.DefaultVisibilityBGAreans);

		if (_maxVisibleDistanceInArenas < 45 * rateCreatureAggro)
		{
			Log.outError(LogFilter.ServerLoading, $"Visibility.Distance.Arenas can't be less max aggro radius {45 * rateCreatureAggro}");
			_maxVisibleDistanceInArenas = 45 * rateCreatureAggro;
		}
		else if (_maxVisibleDistanceInArenas > SharedConst.MaxVisibilityDistance)
		{
			Log.outError(LogFilter.ServerLoading, $"Visibility.Distance.Arenas can't be greater {SharedConst.MaxVisibilityDistance}");
			_maxVisibleDistanceInArenas = SharedConst.MaxVisibilityDistance;
		}

		_visibilityNotifyPeriodOnContinents = ConfigMgr.GetDefaultValue("Visibility.Notify.Period.OnContinents", SharedConst.DefaultVisibilityNotifyPeriod);
		_visibilityNotifyPeriodInInstances = ConfigMgr.GetDefaultValue("Visibility.Notify.Period.InInstances", SharedConst.DefaultVisibilityNotifyPeriod);
		_visibilityNotifyPeriodInBg = ConfigMgr.GetDefaultValue("Visibility.Notify.Period.InBG", SharedConst.DefaultVisibilityNotifyPeriod);
		_visibilityNotifyPeriodInArenas = ConfigMgr.GetDefaultValue("Visibility.Notify.Period.InArenas", SharedConst.DefaultVisibilityNotifyPeriod);

		_guidWarningMsg = ConfigMgr.GetDefaultValue("Respawn.WarningMessage", "There will be an unscheduled server restart at 03:00. The server will be available again shortly after.");
		_alertRestartReason = ConfigMgr.GetDefaultValue("Respawn.AlertRestartReason", "Urgent Maintenance");

		var dataPath = ConfigMgr.GetDefaultValue("DataDir", "./");

		if (reload)
		{
			if (dataPath != _dataPath)
				Log.outError(LogFilter.ServerLoading, "DataDir option can't be changed at worldserver.conf reload, using current value ({0}).", _dataPath);
		}
		else
		{
			_dataPath = dataPath;
			Log.outInfo(LogFilter.ServerLoading, "Using DataDir {0}", _dataPath);
		}

		Log.outInfo(LogFilter.ServerLoading, @"WORLD: MMap data directory is: {0}\mmaps", _dataPath);

		var EnableIndoor = ConfigMgr.GetDefaultValue("vmap.EnableIndoorCheck", true);
		var EnableLOS = ConfigMgr.GetDefaultValue("vmap.EnableLOS", true);
		var EnableHeight = ConfigMgr.GetDefaultValue("vmap.EnableHeight", true);

		if (!EnableHeight)
			Log.outError(LogFilter.ServerLoading, "VMap height checking Disabled! Creatures movements and other various things WILL be broken! Expect no support.");

		Global.VMapMgr.SetEnableLineOfSightCalc(EnableLOS);
		Global.VMapMgr.SetEnableHeightCalc(EnableHeight);

		Log.outInfo(LogFilter.ServerLoading, "VMap support included. LineOfSight: {0}, getHeight: {1}, indoorCheck: {2}", EnableLOS, EnableHeight, EnableIndoor);
		Log.outInfo(LogFilter.ServerLoading, @"VMap data directory is: {0}\vmaps", DataPath);
	}

	public void SetForcedWarModeFactionBalanceState(int team, int reward = 0)
	{
		Global.WorldStateMgr.SetValueAndSaveInDb(WorldStates.WarModeHordeBuffValue, 10 + (team == TeamIds.Alliance ? reward : 0), false, null);
		Global.WorldStateMgr.SetValueAndSaveInDb(WorldStates.WarModeAllianceBuffValue, 10 + (team == TeamIds.Horde ? reward : 0), false, null);
	}

	public void DisableForcedWarModeFactionBalanceState()
	{
		UpdateWarModeRewardValues();
	}

	public void LoadAutobroadcasts()
	{
		var oldMSTime = Time.MSTime;

		_autobroadcasts.Clear();

		var stmt = DB.Login.GetPreparedStatement(LoginStatements.SEL_AUTOBROADCAST);
		stmt.AddValue(0, _realm.Id.Index);

		var result = DB.Login.Query(stmt);

		if (result.IsEmpty())
		{
			Log.outInfo(LogFilter.ServerLoading, "Loaded 0 autobroadcasts definitions. DB table `autobroadcast` is empty for this realm!");

			return;
		}

		do
		{
			var id = result.Read<byte>(0);

			_autobroadcasts[id] = new Autobroadcast(result.Read<string>(2), result.Read<byte>(1));
		} while (result.NextRow());

		Log.outInfo(LogFilter.ServerLoading, "Loaded {0} autobroadcast definitions in {1} ms", _autobroadcasts.Count, Time.GetMSTimeDiffToNow(oldMSTime));
	}

	public void Update(uint diff)
	{
		///- Update the game time and check for shutdown time
		UpdateGameTime();
		var currentGameTime = GameTime.GetGameTime();

		_worldUpdateTime.UpdateWithDiff(diff);

		// Record update if recording set in log and diff is greater then minimum set in log
		_worldUpdateTime.RecordUpdateTime(GameTime.GetGameTimeMS(), diff, (uint)ActiveSessionCount);
		Realm.PopulationLevel = ActiveSessionCount;

		// Update the different timers
		for (WorldTimers i = 0; i < WorldTimers.Max; ++i)
			if (_timers[i].Current >= 0)
				_timers[i].Update(diff);
			else
				_timers[i].Current = 0;

		// Update Who List Storage
		if (_timers[WorldTimers.WhoList].Passed)
		{
			_timers[WorldTimers.WhoList].Reset();
			_taskManager.Schedule(Global.WhoListStorageMgr.Update);
		}

		if (IsStopped || _timers[WorldTimers.ChannelSave].Passed)
		{
			_timers[WorldTimers.ChannelSave].Reset();

			if (WorldConfig.GetBoolValue(WorldCfg.PreserveCustomChannels))
				_taskManager.Schedule(() =>
				{
					var mgr1 = ChannelManager.ForTeam(TeamFaction.Alliance);
					mgr1.SaveToDB();
					var mgr2 = ChannelManager.ForTeam(TeamFaction.Horde);

					if (mgr1 != mgr2)
						mgr2.SaveToDB();
				});
		}

		CheckScheduledResetTimes();

		if (currentGameTime > _nextRandomBgReset)
			_taskManager.Schedule(ResetRandomBG);

		if (currentGameTime > _nextCalendarOldEventsDeletionTime)
			_taskManager.Schedule(CalendarDeleteOldEvents);

		if (currentGameTime > _nextGuildReset)
			_taskManager.Schedule(ResetGuildCap);

		if (currentGameTime > _nextCurrencyReset)
			_taskManager.Schedule(ResetCurrencyWeekCap);

		//Handle auctions when the timer has passed
		if (_timers[WorldTimers.Auctions].Passed)
		{
			_timers[WorldTimers.Auctions].Reset();

			// Update mails (return old mails with item, or delete them)
			if (++_mailTimer > _timerExpires)
			{
				_mailTimer = 0;
				_taskManager.Schedule(() => Global.ObjectMgr.ReturnOrDeleteOldMails(true));
			}

			// Handle expired auctions
			_taskManager.Schedule(Global.AuctionHouseMgr.Update);
		}

		if (_timers[WorldTimers.AuctionsPending].Passed)
		{
			_timers[WorldTimers.AuctionsPending].Reset();

			_taskManager.Schedule(Global.AuctionHouseMgr.UpdatePendingAuctions);
		}

		if (_timers[WorldTimers.Blackmarket].Passed)
		{
			_timers[WorldTimers.Blackmarket].Reset();

            DB.Login.DirectExecute("UPDATE realmlist SET population = {0} WHERE id = '{1}'", ActiveSessionCount, Global.WorldMgr.Realm.Id.Index);

            //- Update blackmarket, refresh auctions if necessary
            if ((_blackmarketTimer * _timers[WorldTimers.Blackmarket].Interval >= WorldConfig.GetIntValue(WorldCfg.BlackmarketUpdatePeriod) * Time.Hour * Time.InMilliseconds) || _blackmarketTimer == 0)
			{
				_taskManager.Schedule(Global.BlackMarketMgr.RefreshAuctions);
				_blackmarketTimer = 1; // timer is 0 on startup
			}
			else
			{
				++_blackmarketTimer;
				_taskManager.Schedule(() => Global.BlackMarketMgr.Update());
			}
		}

		//Handle session updates when the timer has passed
		_worldUpdateTime.RecordUpdateTimeReset();
		UpdateSessions(diff);
		_worldUpdateTime.RecordUpdateTimeDuration("UpdateSessions");

		// <li> Update uptime table
		if (_timers[WorldTimers.UpTime].Passed)
		{
			var tmpDiff = GameTime.GetUptime();
			var maxOnlinePlayers = MaxPlayerCount;

			_timers[WorldTimers.UpTime].Reset();

			_taskManager.Schedule(() =>
			{
				var stmt = DB.Login.GetPreparedStatement(LoginStatements.UPD_UPTIME_PLAYERS);

				stmt.AddValue(0, tmpDiff);
				stmt.AddValue(1, maxOnlinePlayers);
				stmt.AddValue(2, _realm.Id.Index);
				stmt.AddValue(3, (uint)GameTime.GetStartTime());

				DB.Login.Execute(stmt);
			});
		}

		// <li> Clean logs table
		if (WorldConfig.GetIntValue(WorldCfg.LogdbCleartime) > 0) // if not enabled, ignore the timer
			if (_timers[WorldTimers.CleanDB].Passed)
			{
				_timers[WorldTimers.CleanDB].Reset();

				_taskManager.Schedule(() =>
				{
					var stmt = DB.Login.GetPreparedStatement(LoginStatements.DEL_OLD_LOGS);
					stmt.AddValue(0, WorldConfig.GetIntValue(WorldCfg.LogdbCleartime));
					stmt.AddValue(1, 0);
					stmt.AddValue(2, Realm.Id.Index);

					DB.Login.Execute(stmt);
				});
			}

		_taskManager.Wait();
		_worldUpdateTime.RecordUpdateTimeReset();
		Global.MapMgr.Update(diff);
		_worldUpdateTime.RecordUpdateTimeDuration("UpdateMapMgr");

		Global.TerrainMgr.Update(diff); // TPL blocks inside

		if (WorldConfig.GetBoolValue(WorldCfg.AutoBroadcast))
			if (_timers[WorldTimers.AutoBroadcast].Passed)
			{
				_timers[WorldTimers.AutoBroadcast].Reset();
				_taskManager.Schedule(SendAutoBroadcast);
			}

		Global.BattlegroundMgr.Update(diff); // TPL Blocks inside
		_worldUpdateTime.RecordUpdateTimeDuration("UpdateBattlegroundMgr");

		Global.OutdoorPvPMgr.Update(diff); // TPL Blocks inside
		_worldUpdateTime.RecordUpdateTimeDuration("UpdateOutdoorPvPMgr");

		Global.BattleFieldMgr.Update(diff); // TPL Blocks inside
		_worldUpdateTime.RecordUpdateTimeDuration("BattlefieldMgr");

		//- Delete all characters which have been deleted X days before
		if (_timers[WorldTimers.DeleteChars].Passed)
		{
			_timers[WorldTimers.DeleteChars].Reset();
			_taskManager.Schedule(Player.DeleteOldCharacters);
		}

		_taskManager.Schedule(() => Global.LFGMgr.Update(diff));
		_worldUpdateTime.RecordUpdateTimeDuration("UpdateLFGMgr");

		_taskManager.Schedule(() => Global.GroupMgr.Update(diff));
		_worldUpdateTime.RecordUpdateTimeDuration("GroupMgr");

		// execute callbacks from sql queries that were queued recently
		_taskManager.Schedule(ProcessQueryCallbacks);
		_worldUpdateTime.RecordUpdateTimeDuration("ProcessQueryCallbacks");

		// Erase corpses once every 20 minutes
		if (_timers[WorldTimers.Corpses].Passed)
		{
			_timers[WorldTimers.Corpses].Reset();
			_taskManager.Schedule(() => Global.MapMgr.DoForAllMaps(map => map.RemoveOldCorpses()));
		}

		// Process Game events when necessary
		if (_timers[WorldTimers.Events].Passed)
		{
			_timers[WorldTimers.Events].Reset(); // to give time for Update() to be processed
			var nextGameEvent = Global.GameEventMgr.Update();
			_timers[WorldTimers.Events].Interval = nextGameEvent;
			_timers[WorldTimers.Events].Reset();
		}

		if (_timers[WorldTimers.GuildSave].Passed)
		{
			_timers[WorldTimers.GuildSave].Reset();
			_taskManager.Schedule(Global.GuildMgr.SaveGuilds);
		}

		// Check for shutdown warning
		if (_guidWarn && !_guidAlert)
		{
			_warnDiff += diff;

			if (GameTime.GetGameTime() >= _warnShutdownTime)
				DoGuidWarningRestart();
			else if (_warnDiff > WorldConfig.GetIntValue(WorldCfg.RespawnGuidWarningFrequency) * Time.InMilliseconds)
				SendGuidWarning();
		}

		Global.ScriptMgr.ForEach<IWorldOnUpdate>(p => p.OnUpdate(diff));
		_taskManager.Wait(); // wait for all blocks to complete.
	}

	public void ForceGameEventUpdate()
	{
		_timers[WorldTimers.Events].Reset(); // to give time for Update() to be processed
		var nextGameEvent = Global.GameEventMgr.Update();
		_timers[WorldTimers.Events].Interval = nextGameEvent;
		_timers[WorldTimers.Events].Reset();
	}

	public void SendGlobalMessage(ServerPacket packet, WorldSession self = null, TeamFaction team = 0)
	{
		foreach (var session in _sessions.Values)
			if (session.Player != null &&
				session.Player.IsInWorld &&
				session != self &&
				(team == 0 || session.Player.Team == team))
				session.SendPacket(packet);
	}

	public void SendGlobalGMMessage(ServerPacket packet, WorldSession self = null, TeamFaction team = 0)
	{
		foreach (var session in _sessions.Values)
		{
			// check if session and can receive global GM Messages and its not self
			if (session == null || session == self || !session.HasPermission(RBACPermissions.ReceiveGlobalGmTextmessage))
				continue;

			// Player should be in world
			var player = session.Player;

			if (player == null || !player.IsInWorld)
				continue;

			// Send only to same team, if team is given
			if (team == 0 || player.Team == team)
				session.SendPacket(packet);
		}
	}

	// Send a System Message to all players (except self if mentioned)
	public void SendWorldText(CypherStrings string_id, params object[] args)
	{
		WorldWorldTextBuilder wt_builder = new((uint)string_id, args);
		var wt_do = new LocalizedDo(wt_builder);

		foreach (var session in _sessions.Values)
		{
			if (session == null || !session.Player || !session.Player.IsInWorld)
				continue;

			wt_do.Invoke(session.Player);
		}
	}

	// Send a System Message to all GMs (except self if mentioned)
	public void SendGMText(CypherStrings string_id, params object[] args)
	{
		var wt_builder = new WorldWorldTextBuilder((uint)string_id, args);
		var wt_do = new LocalizedDo(wt_builder);

		foreach (var session in _sessions.Values)
		{
			// Session should have permissions to receive global gm messages
			if (session == null || !session.HasPermission(RBACPermissions.ReceiveGlobalGmTextmessage))
				continue;

			// Player should be in world
			var player = session.Player;

			if (!player || !player.IsInWorld)
				continue;

			wt_do.Invoke(player);
		}
	}

	// Send a packet to all players (or players selected team) in the zone (except self if mentioned)
	public bool SendZoneMessage(uint zone, ServerPacket packet, WorldSession self = null, uint team = 0)
	{
		var foundPlayerToSend = false;

		foreach (var session in _sessions.Values)
			if (session != null &&
				session.Player &&
				session.Player.IsInWorld &&
				session.Player.Zone == zone &&
				session != self &&
				(team == 0 || (uint)session.Player.Team == team))
			{
				session.SendPacket(packet);
				foundPlayerToSend = true;
			}

		return foundPlayerToSend;
	}

	// Send a System Message to all players in the zone (except self if mentioned)
	public void SendZoneText(uint zone, string text, WorldSession self = null, uint team = 0)
	{
		ChatPkt data = new();
		data.Initialize(ChatMsg.System, Language.Universal, null, null, text);
		SendZoneMessage(zone, data, self, team);
	}

	public void KickAll()
	{
		_queuedPlayer.Clear(); // prevent send queue update packet and login queued sessions

		// session not removed at kick and will removed in next update tick
		foreach (var session in _sessions.Values)
			session.KickPlayer("World::KickAll");
	}

	/// Ban an account or ban an IP address, duration will be parsed using TimeStringToSecs if it is positive, otherwise permban
	public BanReturn BanAccount(BanMode mode, string nameOrIP, string duration, string reason, string author)
	{
		var duration_secs = Time.TimeStringToSecs(duration);

		return BanAccount(mode, nameOrIP, duration_secs, reason, author);
	}

	/// Ban an account or ban an IP address, duration is in seconds if positive, otherwise permban
	public BanReturn BanAccount(BanMode mode, string nameOrIP, uint duration_secs, string reason, string author)
	{
		// Prevent banning an already banned account
		if (mode == BanMode.Account && Global.AccountMgr.IsBannedAccount(nameOrIP))
			return BanReturn.Exists;

		SQLResult resultAccounts;
		PreparedStatement stmt;

		// Update the database with ban information
		switch (mode)
		{
			case BanMode.IP:
				// No SQL injection with prepared statements
				stmt = DB.Login.GetPreparedStatement(LoginStatements.SEL_ACCOUNT_BY_IP);
				stmt.AddValue(0, nameOrIP);
				resultAccounts = DB.Login.Query(stmt);
				stmt = DB.Login.GetPreparedStatement(LoginStatements.INS_IP_BANNED);
				stmt.AddValue(0, nameOrIP);
				stmt.AddValue(1, duration_secs);
				stmt.AddValue(2, author);
				stmt.AddValue(3, reason);
				DB.Login.Execute(stmt);

				break;
			case BanMode.Account:
				// No SQL injection with prepared statements
				stmt = DB.Login.GetPreparedStatement(LoginStatements.SEL_ACCOUNT_ID_BY_NAME);
				stmt.AddValue(0, nameOrIP);
				resultAccounts = DB.Login.Query(stmt);

				break;
			case BanMode.Character:
				// No SQL injection with prepared statements
				stmt = DB.Characters.GetPreparedStatement(CharStatements.SEL_ACCOUNT_BY_NAME);
				stmt.AddValue(0, nameOrIP);
				resultAccounts = DB.Characters.Query(stmt);

				break;
			default:
				return BanReturn.SyntaxError;
		}

		if (resultAccounts == null)
		{
			if (mode == BanMode.IP)
				return BanReturn.Success; // ip correctly banned but nobody affected (yet)
			else
				return BanReturn.Notfound; // Nobody to ban
		}

		// Disconnect all affected players (for IP it can be several)
		SQLTransaction trans = new();

		do
		{
			var account = resultAccounts.Read<uint>(0);

			if (mode != BanMode.IP)
			{
				// make sure there is only one active ban
				stmt = DB.Login.GetPreparedStatement(LoginStatements.UPD_ACCOUNT_NOT_BANNED);
				stmt.AddValue(0, account);
				trans.Append(stmt);
				// No SQL injection with prepared statements
				stmt = DB.Login.GetPreparedStatement(LoginStatements.INS_ACCOUNT_BANNED);
				stmt.AddValue(0, account);
				stmt.AddValue(1, duration_secs);
				stmt.AddValue(2, author);
				stmt.AddValue(3, reason);
				trans.Append(stmt);
			}

			var sess = FindSession(account);

			if (sess)
				if (sess.PlayerName != author)
					sess.KickPlayer("World::BanAccount Banning account");
		} while (resultAccounts.NextRow());

		DB.Login.CommitTransaction(trans);

		return BanReturn.Success;
	}

	/// Remove a ban from an account or IP address
	public bool RemoveBanAccount(BanMode mode, string nameOrIP)
	{
		PreparedStatement stmt;

		if (mode == BanMode.IP)
		{
			stmt = DB.Login.GetPreparedStatement(LoginStatements.DEL_IP_NOT_BANNED);
			stmt.AddValue(0, nameOrIP);
			DB.Login.Execute(stmt);
		}
		else
		{
			uint account = 0;

			if (mode == BanMode.Account)
				account = Global.AccountMgr.GetId(nameOrIP);
			else if (mode == BanMode.Character)
				account = Global.CharacterCacheStorage.GetCharacterAccountIdByName(nameOrIP);

			if (account == 0)
				return false;

			//NO SQL injection as account is uint32
			stmt = DB.Login.GetPreparedStatement(LoginStatements.UPD_ACCOUNT_NOT_BANNED);
			stmt.AddValue(0, account);
			DB.Login.Execute(stmt);
		}

		return true;
	}

	/// Ban an account or ban an IP address, duration will be parsed using TimeStringToSecs if it is positive, otherwise permban
	public BanReturn BanCharacter(string name, string duration, string reason, string author)
	{
		var durationSecs = Time.TimeStringToSecs(duration);

		return BanAccount(BanMode.Character, name, durationSecs, reason, author);
	}

	public BanReturn BanCharacter(string name, uint durationSecs, string reason, string author)
	{
		var pBanned = Global.ObjAccessor.FindConnectedPlayerByName(name);
		ObjectGuid guid;

		// Pick a player to ban if not online
		if (!pBanned)
		{
			guid = Global.CharacterCacheStorage.GetCharacterGuidByName(name);

			if (guid.IsEmpty)
				return BanReturn.Notfound; // Nobody to ban
		}
		else
		{
			guid = pBanned.GUID;
		}

		//Use transaction in order to ensure the order of the queries
		SQLTransaction trans = new();

		// make sure there is only one active ban
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_CHARACTER_BAN);
		stmt.AddValue(0, guid.Counter);
		trans.Append(stmt);

		stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_CHARACTER_BAN);
		stmt.AddValue(0, guid.Counter);
		stmt.AddValue(1, (long)durationSecs);
		stmt.AddValue(2, author);
		stmt.AddValue(3, reason);
		trans.Append(stmt);
		DB.Characters.CommitTransaction(trans);

		if (pBanned)
			pBanned.Session.KickPlayer("World::BanCharacter Banning character");

		return BanReturn.Success;
	}

	// Remove a ban from a character
	public bool RemoveBanCharacter(string name)
	{
		var pBanned = Global.ObjAccessor.FindConnectedPlayerByName(name);
		ObjectGuid guid;

		// Pick a player to ban if not online
		if (!pBanned)
		{
			guid = Global.CharacterCacheStorage.GetCharacterGuidByName(name);

			if (guid.IsEmpty)
				return false; // Nobody to ban
		}
		else
		{
			guid = pBanned.GUID;
		}

		var stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_CHARACTER_BAN);
		stmt.AddValue(0, guid.Counter);
		DB.Characters.Execute(stmt);

		return true;
	}

	public void ShutdownServ(uint time, ShutdownMask options, ShutdownExitCode exitcode, string reason = "")
	{
		// ignore if server shutdown at next tick
		if (IsStopped)
			return;

		_shutdownMask = options;
		_exitCode = exitcode;

		// If the shutdown time is 0, evaluate shutdown on next tick (no message)
		if (time == 0)
		{
			_shutdownTimer = 1;
		}
		// Else set the shutdown timer and warn users
		else
		{
			_shutdownTimer = time;
			ShutdownMsg(true, null, reason);
		}

		Global.ScriptMgr.ForEach<IWorldOnShutdownInitiate>(p => p.OnShutdownInitiate(exitcode, options));
	}

	public void ShutdownMsg(bool show = false, Player player = null, string reason = "")
	{
		// not show messages for idle shutdown mode
		if (_shutdownMask.HasAnyFlag(ShutdownMask.Idle))
			return;

		// Display a message every 12 hours, hours, 5 minutes, minute, 5 seconds and finally seconds
		if (show ||
			(_shutdownTimer < 5 * Time.Minute && (_shutdownTimer % 15) == 0) ||                 // < 5 min; every 15 sec
			(_shutdownTimer < 15 * Time.Minute && (_shutdownTimer % Time.Minute) == 0) ||       // < 15 min ; every 1 min
			(_shutdownTimer < 30 * Time.Minute && (_shutdownTimer % (5 * Time.Minute)) == 0) || // < 30 min ; every 5 min
			(_shutdownTimer < 12 * Time.Hour && (_shutdownTimer % Time.Hour) == 0) ||           // < 12 h ; every 1 h
			(_shutdownTimer > 12 * Time.Hour && (_shutdownTimer % (12 * Time.Hour)) == 0))      // > 12 h ; every 12 h
		{
			var str = Time.secsToTimeString(_shutdownTimer, TimeFormat.Numeric);

			if (!reason.IsEmpty())
				str += " - " + reason;

			var msgid = _shutdownMask.HasAnyFlag(ShutdownMask.Restart) ? ServerMessageType.RestartTime : ServerMessageType.ShutdownTime;

			SendServerMessage(msgid, str, player);
			Log.outDebug(LogFilter.Server, "Server is {0} in {1}", (_shutdownMask.HasAnyFlag(ShutdownMask.Restart) ? "restart" : "shuttingdown"), str);
		}
	}

	public uint ShutdownCancel()
	{
		// nothing cancel or too late
		if (_shutdownTimer == 0 || IsStopped)
			return 0;

		var msgid = _shutdownMask.HasAnyFlag(ShutdownMask.Restart) ? ServerMessageType.RestartCancelled : ServerMessageType.ShutdownCancelled;

		var oldTimer = _shutdownTimer;
		_shutdownMask = 0;
		_shutdownTimer = 0;
		_exitCode = (byte)ShutdownExitCode.Shutdown; // to default value
		SendServerMessage(msgid);

		Log.outDebug(LogFilter.Server, "Server {0} cancelled.", (_shutdownMask.HasAnyFlag(ShutdownMask.Restart) ? "restart" : "shutdown"));

		Global.ScriptMgr.ForEach<IWorldOnShutdownCancel>(p => p.OnShutdownCancel());

		return oldTimer;
	}

	public void SendServerMessage(ServerMessageType messageID, string stringParam = "", Player player = null)
	{
		ChatServerMessage packet = new();
		packet.MessageID = (int)messageID;

		if (messageID <= ServerMessageType.String)
			packet.StringParam = stringParam;

		if (player)
			player.SendPacket(packet);
		else
			SendGlobalMessage(packet);
	}

	public void UpdateSessions(uint diff)
	{
		while (_linkSocketQueue.TryDequeue(out var linkInfo))
			ProcessLinkInstanceSocket(linkInfo);

		// Add new sessions
		while (_addSessQueue.TryDequeue(out var sess))
			AddSession_(sess);

		// Then send an update signal to remaining ones
		foreach (var pair in _sessions)
		{
			var session = pair.Value;

			if (!session.UpdateWorld(diff)) // As interval = 0
			{
				if (!RemoveQueuedPlayer(session) && session != null && WorldConfig.GetIntValue(WorldCfg.IntervalDisconnectTolerance) != 0)
					_disconnects[session.AccountId] = GameTime.GetGameTime();

				RemoveQueuedPlayer(session);
				_sessions.TryRemove(pair.Key, out _);
				_sessionsByBnetGuid.Remove(session.BattlenetAccountGUID, session);
				session.Dispose();
			}
		}
	}

	public void UpdateRealmCharCount(uint accountId)
	{
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.SEL_CHARACTER_COUNT);
		stmt.AddValue(0, accountId);
		_queryProcessor.AddCallback(DB.Characters.AsyncQuery(stmt).WithCallback(UpdateRealmCharCount));
	}

	public void DailyReset()
	{
		// reset all saved quest status
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_RESET_CHARACTER_QUESTSTATUS_DAILY);
		DB.Characters.Execute(stmt);

		stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_CHARACTER_GARRISON_FOLLOWER_ACTIVATIONS);
		stmt.AddValue(0, 1);
		DB.Characters.Execute(stmt);

		// reset all quest status in memory
		foreach (var itr in _sessions)
		{
			var player = itr.Value.Player;

			if (player != null)
				player.DailyReset();
		}

		// reselect pools
		Global.QuestPoolMgr.ChangeDailyQuests();

		// Update faction balance
		UpdateWarModeRewardValues();

		// store next reset time
		var now = GameTime.GetGameTime();
		var next = GetNextDailyResetTime(now);

		_nextDailyQuestReset = next;
		SetPersistentWorldVariable(NextDailyQuestResetTimeVarId, (int)next);

		Log.outInfo(LogFilter.Misc, "Daily quests for all characters have been reset.");
	}

	public void ResetWeeklyQuests()
	{
		// reset all saved quest status
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_RESET_CHARACTER_QUESTSTATUS_WEEKLY);
		DB.Characters.Execute(stmt);

		// reset all quest status in memory
		foreach (var itr in _sessions)
		{
			var player = itr.Value.Player;

			if (player != null)
				player.ResetWeeklyQuestStatus();
		}

		// reselect pools
		Global.QuestPoolMgr.ChangeWeeklyQuests();

		// store next reset time
		var now = GameTime.GetGameTime();
		var next = GetNextWeeklyResetTime(now);

		_nextWeeklyQuestReset = next;
		SetPersistentWorldVariable(NextWeeklyQuestResetTimeVarId, (int)next);

		Log.outInfo(LogFilter.Misc, "Weekly quests for all characters have been reset.");
	}

	public void ResetMonthlyQuests()
	{
		// reset all saved quest status
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_RESET_CHARACTER_QUESTSTATUS_MONTHLY);
		DB.Characters.Execute(stmt);

		// reset all quest status in memory
		foreach (var itr in _sessions)
		{
			var player = itr.Value.Player;

			if (player != null)
				player.ResetMonthlyQuestStatus();
		}

		// reselect pools
		Global.QuestPoolMgr.ChangeMonthlyQuests();

		// store next reset time
		var now = GameTime.GetGameTime();
		var next = GetNextMonthlyResetTime(now);

		_nextMonthlyQuestReset = next;

		Log.outInfo(LogFilter.Misc, "Monthly quests for all characters have been reset.");
	}

	public void ResetEventSeasonalQuests(ushort event_id, long eventStartTime)
	{
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_RESET_CHARACTER_QUESTSTATUS_SEASONAL_BY_EVENT);
		stmt.AddValue(0, event_id);
		stmt.AddValue(1, eventStartTime);
		DB.Characters.Execute(stmt);

		foreach (var session in _sessions.Values)
			session.Player?.ResetSeasonalQuestStatus(event_id, eventStartTime);
	}

	public string LoadDBVersion()
	{
		var DBVersion = "Unknown world database.";

		var result = DB.World.Query("SELECT db_version, cache_id FROM version LIMIT 1");

		if (!result.IsEmpty())
		{
			DBVersion = result.Read<string>(0);
			// will be overwrite by config values if different and non-0
			WorldConfig.SetValue(WorldCfg.ClientCacheVersion, result.Read<uint>(1));
		}

		return DBVersion;
	}

	public bool IsBattlePetJournalLockAcquired(ObjectGuid battlenetAccountGuid)
	{
		foreach (var sessionForBnet in _sessionsByBnetGuid.LookupByKey(battlenetAccountGuid))
			if (sessionForBnet.BattlePetMgr.HasJournalLock)
				return true;

		return false;
	}

	public int GetPersistentWorldVariable(string var)
	{
		return _worldVariables.LookupByKey(var);
	}

	public void SetPersistentWorldVariable(string var, int value)
	{
		_worldVariables[var] = value;

		var stmt = DB.Characters.GetPreparedStatement(CharStatements.REP_WORLD_VARIABLE);
		stmt.AddValue(0, var);
		stmt.AddValue(1, value);
		DB.Characters.Execute(stmt);
	}

	public void ReloadRBAC()
	{
		// Passive reload, we mark the data as invalidated and next time a permission is checked it will be reloaded
		Log.outInfo(LogFilter.Rbac, "World.ReloadRBAC()");

		foreach (var session in _sessions.Values)
			session.InvalidateRBACData();
	}

	public void IncreasePlayerCount()
	{
		_playerCount++;
		_maxPlayerCount = Math.Max(_maxPlayerCount, _playerCount);
	}

	public void DecreasePlayerCount()
	{
		_playerCount--;
	}

	public void StopNow(ShutdownExitCode exitcode = ShutdownExitCode.Error)
	{
		IsStopped = true;
		_exitCode = exitcode;
	}

	public bool LoadRealmInfo()
	{
		var result = DB.Login.Query("SELECT id, name, address, localAddress, localSubnetMask, port, icon, flag, timezone, allowedSecurityLevel, population, gamebuild, Region, Battlegroup FROM realmlist WHERE id = {0}", _realm.Id.Index);

		if (result.IsEmpty())
			return false;

		_realm.SetName(result.Read<string>(1));
		_realm.ExternalAddress = System.Net.IPAddress.Parse(result.Read<string>(2));
		_realm.LocalAddress = System.Net.IPAddress.Parse(result.Read<string>(3));
		_realm.LocalSubnetMask = System.Net.IPAddress.Parse(result.Read<string>(4));
		_realm.Port = result.Read<ushort>(5);
		_realm.Type = result.Read<byte>(6);
		_realm.Flags = (RealmFlags)result.Read<byte>(7);
		_realm.Timezone = result.Read<byte>(8);
		_realm.AllowedSecurityLevel = (AccountTypes)result.Read<byte>(9);
		_realm.PopulationLevel = result.Read<float>(10);
		_realm.Id.Region = result.Read<byte>(12);
		_realm.Id.Site = result.Read<byte>(13);
		_realm.Build = result.Read<uint>(11);

		return true;
	}

	public void RemoveOldCorpses()
	{
		_timers[WorldTimers.Corpses].Current = _timers[WorldTimers.Corpses].Interval;
	}

	public Locale GetAvailableDbcLocale(Locale locale)
	{
		if (_availableDbcLocaleMask[(int)locale])
			return locale;
		else
			return _defaultDbcLocale;
	}

	void DoGuidWarningRestart()
	{
		if (_shutdownTimer != 0)
			return;

		ShutdownServ(1800, ShutdownMask.Restart, ShutdownExitCode.Restart);
		_warnShutdownTime += Time.Hour;
	}

	void DoGuidAlertRestart()
	{
		if (_shutdownTimer != 0)
			return;

		ShutdownServ(300, ShutdownMask.Restart, ShutdownExitCode.Restart, _alertRestartReason);
	}

	void SendGuidWarning()
	{
		if (_shutdownTimer == 0 && _guidWarn && WorldConfig.GetIntValue(WorldCfg.RespawnGuidWarningFrequency) > 0)
			SendServerMessage(ServerMessageType.String, _guidWarningMsg);

		_warnDiff = 0;
	}

	bool RemoveSession(uint id)
	{
		// Find the session, kick the user, but we can't delete session at this moment to prevent iterator invalidation
		var session = _sessions.LookupByKey(id);

		if (session != null)
		{
			if (session.PlayerLoading)
				return false;

			session.KickPlayer("World::RemoveSession");
		}

		return true;
	}

	void AddSession_(WorldSession s)
	{
		//NOTE - Still there is race condition in WorldSession* being used in the Sockets

		// kick already loaded player with same account (if any) and remove session
		// if player is in loading and want to load again, return
		if (!RemoveSession(s.AccountId))
		{
			s.KickPlayer("World::AddSession_ Couldn't remove the other session while on loading screen");

			return;
		}

		// decrease session counts only at not reconnection case
		var decrease_session = true;

		// if session already exist, prepare to it deleting at next world update
		// NOTE - KickPlayer() should be called on "old" in RemoveSession()
		{
			var old = _sessions.LookupByKey(s.AccountId);

			if (old != null)
			{
				// prevent decrease sessions count if session queued
				if (RemoveQueuedPlayer(old))
					decrease_session = false;

				_sessionsByBnetGuid.Remove(old.BattlenetAccountGUID, old);
				old.Dispose();
			}
		}

		_sessions[s.AccountId] = s;
		_sessionsByBnetGuid.Add(s.BattlenetAccountGUID, s);

		var Sessions = ActiveAndQueuedSessionCount;
		var pLimit = PlayerAmountLimit;
		var QueueSize = QueuedSessionCount; //number of players in the queue

		//so we don't count the user trying to
		//login as a session and queue the socket that we are using
		if (decrease_session)
			--Sessions;

		if (pLimit > 0 && Sessions >= pLimit && !s.HasPermission(RBACPermissions.SkipQueue) && !HasRecentlyDisconnected(s))
		{
			AddQueuedPlayer(s);
			UpdateMaxSessionCounters();
			Log.outInfo(LogFilter.Server, "PlayerQueue: Account id {0} is in Queue Position ({1}).", s.AccountId, ++QueueSize);

			return;
		}

		s.InitializeSession();

		UpdateMaxSessionCounters();

		// Updates the population
		if (pLimit > 0)
		{
			float popu = ActiveSessionCount; // updated number of users on the server
			popu /= pLimit;
			popu *= 2;
			Log.outInfo(LogFilter.Server, "Server Population ({0}).", popu);
		}
	}

	void ProcessLinkInstanceSocket(Tuple<WorldSocket, ulong> linkInfo)
	{
		if (!linkInfo.Item1.IsOpen())
			return;

		ConnectToKey key = new();
		key.Raw = linkInfo.Item2;

		var session = FindSession(key.AccountId);

		if (!session || session.ConnectToInstanceKey != linkInfo.Item2)
		{
			linkInfo.Item1.SendAuthResponseError(BattlenetRpcErrorCode.TimedOut);
			linkInfo.Item1.CloseSocket();

			return;
		}

		linkInfo.Item1.SetWorldSession(session);
		session.AddInstanceConnection(linkInfo.Item1);
		session.HandleContinuePlayerLogin();
	}

	bool HasRecentlyDisconnected(WorldSession session)
	{
		if (session == null)
			return false;

		uint tolerance = 0;

		if (tolerance != 0)
			foreach (var disconnect in _disconnects)
				if ((disconnect.Value - GameTime.GetGameTime()) < tolerance)
				{
					if (disconnect.Key == session.AccountId)
						return true;
				}
				else
				{
					_disconnects.Remove(disconnect.Key);
				}

		return false;
	}

	uint GetQueuePos(WorldSession sess)
	{
		uint position = 1;

		foreach (var iter in _queuedPlayer)
			if (iter != sess)
				++position;
			else
				return position;

		return 0;
	}

	void AddQueuedPlayer(WorldSession sess)
	{
		sess.SetInQueue(true);
		_queuedPlayer.Add(sess);

		// The 1st SMSG_AUTH_RESPONSE needs to contain other info too.
		sess.SendAuthResponse(BattlenetRpcErrorCode.Ok, true, GetQueuePos(sess));
	}

	bool RemoveQueuedPlayer(WorldSession sess)
	{
		// sessions count including queued to remove (if removed_session set)
		var sessions = ActiveSessionCount;

		uint position = 1;

		// search to remove and count skipped positions
		var found = false;

		foreach (var iter in _queuedPlayer)
			if (iter != sess)
			{
				++position;
			}
			else
			{
				sess.SetInQueue(false);
				sess.ResetTimeOutTime(false);
				_queuedPlayer.Remove(iter);
				found = true; // removing queued session

				break;
			}

		// iter point to next socked after removed or end()
		// position store position of removed socket and then new position next socket after removed

		// if session not queued then we need decrease sessions count
		if (!found && sessions != 0)
			--sessions;

		// accept first in queue
		if ((_playerLimit == 0 || sessions < _playerLimit) && !_queuedPlayer.Empty())
		{
			var pop_sess = _queuedPlayer.First();
			pop_sess.InitializeSession();

			_queuedPlayer.RemoveAt(0);

			// update iter to point first queued socket or end() if queue is empty now
			position = 1;
		}

		// update position from iter to end()
		// iter point to first not updated socket, position store new position
		foreach (var iter in _queuedPlayer)
			iter.SendAuthWaitQueue(++position);

		return found;
	}

	void KickAllLess(AccountTypes sec)
	{
		// session not removed at kick and will removed in next update tick
		foreach (var session in _sessions.Values)
			if (session.Security < sec)
				session.KickPlayer("World::KickAllLess");
	}

	void UpdateGameTime()
	{
		// update the time
		var lastGameTime = GameTime.GetGameTime();
		GameTime.UpdateGameTimers();

		var elapsed = (uint)(GameTime.GetGameTime() - lastGameTime);

		//- if there is a shutdown timer
		if (!IsStopped && _shutdownTimer > 0 && elapsed > 0)
		{
			//- ... and it is overdue, stop the world
			if (_shutdownTimer <= elapsed)
			{
				if (!_shutdownMask.HasAnyFlag(ShutdownMask.Idle) || ActiveAndQueuedSessionCount == 0)
					IsStopped = true; // exist code already set
				else
					_shutdownTimer = 1; // minimum timer value to wait idle state
			}
			//- ... else decrease it and if necessary display a shutdown countdown to the users
			else
			{
				_shutdownTimer -= elapsed;

				ShutdownMsg();
			}
		}
	}

	void SendAutoBroadcast()
	{
		if (_autobroadcasts.Empty())
			return;

		var pair = _autobroadcasts.SelectRandomElementByWeight(autoPair => autoPair.Value.Weight);

		var abcenter = WorldConfig.GetUIntValue(WorldCfg.AutoBroadcastCenter);

		if (abcenter == 0)
		{
			SendWorldText(CypherStrings.AutoBroadcast, pair.Value.Message);
		}
		else if (abcenter == 1)
		{
			SendGlobalMessage(new PrintNotification(pair.Value.Message));
		}
		else if (abcenter == 2)
		{
			SendWorldText(CypherStrings.AutoBroadcast, pair.Value.Message);
			SendGlobalMessage(new PrintNotification(pair.Value.Message));
		}

		Log.outDebug(LogFilter.Misc, "AutoBroadcast: '{0}'", pair.Value.Message);
	}

	void UpdateRealmCharCount(SQLResult result)
	{
		if (!result.IsEmpty())
		{
			var Id = result.Read<uint>(0);
			var charCount = result.Read<uint>(1);

			var stmt = DB.Login.GetPreparedStatement(LoginStatements.REP_REALM_CHARACTERS);
			stmt.AddValue(0, charCount);
			stmt.AddValue(1, Id);
			stmt.AddValue(2, _realm.Id.Index);
			DB.Login.DirectExecute(stmt);
		}
	}

	void InitQuestResetTimes()
	{
		_nextDailyQuestReset = GetPersistentWorldVariable(NextDailyQuestResetTimeVarId);
		_nextWeeklyQuestReset = GetPersistentWorldVariable(NextWeeklyQuestResetTimeVarId);
		_nextMonthlyQuestReset = GetPersistentWorldVariable(NextMonthlyQuestResetTimeVarId);
	}

	static long GetNextDailyResetTime(long t)
	{
		return Time.GetLocalHourTimestamp(t, WorldConfig.GetUIntValue(WorldCfg.DailyQuestResetTimeHour), true);
	}

	static long GetNextWeeklyResetTime(long t)
	{
		t = GetNextDailyResetTime(t);
		var time = Time.UnixTimeToDateTime(t);
		var wday = (int)time.DayOfWeek;
		var target = WorldConfig.GetIntValue(WorldCfg.WeeklyQuestResetTimeWDay);

		if (target < wday)
			wday -= 7;

		t += (Time.Day * (target - wday));

		return t;
	}

	static long GetNextMonthlyResetTime(long t)
	{
		t = GetNextDailyResetTime(t);
		var time = Time.UnixTimeToDateTime(t);

		if (time.Day == 1)
			return t;

		var newDate = new DateTime(time.Year, time.Month + 1, 1, 0, 0, 0, time.Kind);

		return Time.DateTimeToUnixTime(newDate);
	}

	void CheckScheduledResetTimes()
	{
		var now = GameTime.GetGameTime();

		if (_nextDailyQuestReset <= now)
			_taskManager.Schedule(DailyReset);

		if (_nextWeeklyQuestReset <= now)
			_taskManager.Schedule(ResetWeeklyQuests);

		if (_nextMonthlyQuestReset <= now)
			_taskManager.Schedule(ResetMonthlyQuests);
	}

	void InitRandomBGResetTime()
	{
		long bgtime = GetPersistentWorldVariable(NextBGRandomDailyResetTimeVarId);

		if (bgtime == 0)
			_nextRandomBgReset = GameTime.GetGameTime(); // game time not yet init

		// generate time by config
		var curTime = GameTime.GetGameTime();

		// current day reset time
		var nextDayResetTime = Time.GetNextResetUnixTime(WorldConfig.GetIntValue(WorldCfg.RandomBgResetHour));

		// next reset time before current moment
		if (curTime >= nextDayResetTime)
			nextDayResetTime += Time.Day;

		// normalize reset time
		_nextRandomBgReset = bgtime < curTime ? nextDayResetTime - Time.Day : nextDayResetTime;

		if (bgtime == 0)
			SetPersistentWorldVariable(NextBGRandomDailyResetTimeVarId, (int)_nextRandomBgReset);
	}

	void InitCalendarOldEventsDeletionTime()
	{
		var now = GameTime.GetGameTime();
		var nextDeletionTime = Time.GetLocalHourTimestamp(now, WorldConfig.GetUIntValue(WorldCfg.CalendarDeleteOldEventsHour));
		long currentDeletionTime = GetPersistentWorldVariable(NextOldCalendarEventDeletionTimeVarId);

		// If the reset time saved in the worldstate is before now it means the server was offline when the reset was supposed to occur.
		// In this case we set the reset time in the past and next world update will do the reset and schedule next one in the future.
		if (currentDeletionTime < now)
			_nextCalendarOldEventsDeletionTime = nextDeletionTime - Time.Day;
		else
			_nextCalendarOldEventsDeletionTime = nextDeletionTime;

		if (currentDeletionTime == 0)
			SetPersistentWorldVariable(NextOldCalendarEventDeletionTimeVarId, (int)_nextCalendarOldEventsDeletionTime);
	}

	void InitGuildResetTime()
	{
		long gtime = GetPersistentWorldVariable(NextGuildDailyResetTimeVarId);

		if (gtime == 0)
			_nextGuildReset = GameTime.GetGameTime(); // game time not yet init

		var curTime = GameTime.GetGameTime();
		var nextDayResetTime = Time.GetNextResetUnixTime(WorldConfig.GetIntValue(WorldCfg.GuildResetHour));

		if (curTime >= nextDayResetTime)
			nextDayResetTime += Time.Day;

		// normalize reset time
		_nextGuildReset = gtime < curTime ? nextDayResetTime - Time.Day : nextDayResetTime;

		if (gtime == 0)
			SetPersistentWorldVariable(NextGuildDailyResetTimeVarId, (int)_nextGuildReset);
	}

	void InitCurrencyResetTime()
	{
		long currencytime = GetPersistentWorldVariable(NextCurrencyResetTimeVarId);

		if (currencytime == 0)
			_nextCurrencyReset = GameTime.GetGameTime(); // game time not yet init

		// generate time by config
		var curTime = GameTime.GetGameTime();

		var nextWeekResetTime = Time.GetNextResetUnixTime(WorldConfig.GetIntValue(WorldCfg.CurrencyResetDay), WorldConfig.GetIntValue(WorldCfg.CurrencyResetHour));

		// next reset time before current moment
		if (curTime >= nextWeekResetTime)
			nextWeekResetTime += WorldConfig.GetIntValue(WorldCfg.CurrencyResetInterval) * Time.Day;

		// normalize reset time
		_nextCurrencyReset = currencytime < curTime ? nextWeekResetTime - WorldConfig.GetIntValue(WorldCfg.CurrencyResetInterval) * Time.Day : nextWeekResetTime;

		if (currencytime == 0)
			SetPersistentWorldVariable(NextCurrencyResetTimeVarId, (int)_nextCurrencyReset);
	}

	void ResetCurrencyWeekCap()
	{
		DB.Characters.Execute("UPDATE `character_currency` SET `WeeklyQuantity` = 0");

		foreach (var session in _sessions.Values)
			if (session.Player != null)
				session.Player.ResetCurrencyWeekCap();

		_nextCurrencyReset += Time.Day * WorldConfig.GetIntValue(WorldCfg.CurrencyResetInterval);
		SetPersistentWorldVariable(NextCurrencyResetTimeVarId, (int)_nextCurrencyReset);
	}

	void ResetRandomBG()
	{
		Log.outInfo(LogFilter.Server, "Random BG status reset for all characters.");

		var stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_BATTLEGROUND_RANDOM_ALL);
		DB.Characters.Execute(stmt);

		foreach (var session in _sessions.Values)
			if (session.Player)
				session.Player.SetRandomWinner(false);

		_nextRandomBgReset += Time.Day;
		SetPersistentWorldVariable(NextBGRandomDailyResetTimeVarId, (int)_nextRandomBgReset);
	}

	void CalendarDeleteOldEvents()
	{
		Log.outInfo(LogFilter.Misc, "Calendar deletion of old events.");

		_nextCalendarOldEventsDeletionTime = _nextCalendarOldEventsDeletionTime + Time.Day;
		SetPersistentWorldVariable(NextOldCalendarEventDeletionTimeVarId, (int)_nextCalendarOldEventsDeletionTime);
		Global.CalendarMgr.DeleteOldEvents();
	}

	void ResetGuildCap()
	{
		_nextGuildReset += Time.Day;
		SetPersistentWorldVariable(NextGuildDailyResetTimeVarId, (int)_nextGuildReset);
		var week = GetPersistentWorldVariable(NextGuildWeeklyResetTimeVarId);
		week = week < 7 ? week + 1 : 1;

		Log.outInfo(LogFilter.Server, "Guild Daily Cap reset. Week: {0}", week == 1);
		SetPersistentWorldVariable(NextGuildWeeklyResetTimeVarId, week);
		Global.GuildMgr.ResetTimes(week == 1);
	}

	void UpdateMaxSessionCounters()
	{
		_maxActiveSessionCount = Math.Max(_maxActiveSessionCount, (uint)(_sessions.Count - _queuedPlayer.Count));
		_maxQueuedSessionCount = Math.Max(_maxQueuedSessionCount, (uint)_queuedPlayer.Count);
	}

	void UpdateAreaDependentAuras()
	{
		foreach (var session in _sessions.Values)
			if (session.Player != null && session.Player.IsInWorld)
			{
				session.Player.UpdateAreaDependentAuras(session.Player.Area);
				session.Player.UpdateZoneDependentAuras(session.Player.Zone);
			}
	}

	void LoadPersistentWorldVariables()
	{
		var oldMSTime = Time.MSTime;

		var result = DB.Characters.Query("SELECT ID, Value FROM world_variable");

		if (!result.IsEmpty())
			do
			{
				_worldVariables[result.Read<string>(0)] = result.Read<int>(1);
			} while (result.NextRow());

		Log.outInfo(LogFilter.ServerLoading, $"Loaded {_worldVariables.Count} world variables in {Time.GetMSTimeDiffToNow(oldMSTime)} ms");
	}

	void ProcessQueryCallbacks()
	{
		_queryProcessor.ProcessReadyCallbacks();
	}

	long GetNextRandomBGResetTime()
	{
		return _nextRandomBgReset;
	}

	void UpdateWarModeRewardValues()
	{
		var warModeEnabledFaction = new long[2];

		// Search for characters that have war mode enabled and played during the last week
		var stmt = DB.Characters.GetPreparedStatement(CharStatements.SEL_WAR_MODE_TUNING);
		stmt.AddValue(0, (uint)PlayerFlags.WarModeDesired);
		stmt.AddValue(1, (uint)PlayerFlags.WarModeDesired);

		var result = DB.Characters.Query(stmt);

		if (!result.IsEmpty())
			do
			{
				var race = result.Read<byte>(0);

				var raceEntry = CliDB.ChrRacesStorage.LookupByKey(race);

				if (raceEntry != null)
				{
					var raceFaction = CliDB.FactionTemplateStorage.LookupByKey(raceEntry.FactionID);

					if (raceFaction != null)
					{
						if ((raceFaction.FactionGroup & (byte)FactionMasks.Alliance) != 0)
							warModeEnabledFaction[TeamIds.Alliance] += result.Read<long>(1);
						else if ((raceFaction.FactionGroup & (byte)FactionMasks.Horde) != 0)
							warModeEnabledFaction[TeamIds.Horde] += result.Read<long>(1);
					}
				}
			} while (result.NextRow());


		var dominantFaction = TeamIds.Alliance;
		var outnumberedFactionReward = 0;

		if (warModeEnabledFaction.Any(val => val != 0))
		{
			var dominantFactionCount = warModeEnabledFaction[TeamIds.Alliance];

			if (warModeEnabledFaction[TeamIds.Alliance] < warModeEnabledFaction[TeamIds.Horde])
			{
				dominantFactionCount = warModeEnabledFaction[TeamIds.Horde];
				dominantFaction = TeamIds.Horde;
			}

			double total = warModeEnabledFaction[TeamIds.Alliance] + warModeEnabledFaction[TeamIds.Horde];
			var pct = dominantFactionCount / total;

			if (pct >= WorldConfig.GetFloatValue(WorldCfg.CallToArms20Pct))
				outnumberedFactionReward = 20;
			else if (pct >= WorldConfig.GetFloatValue(WorldCfg.CallToArms10Pct))
				outnumberedFactionReward = 10;
			else if (pct >= WorldConfig.GetFloatValue(WorldCfg.CallToArms5Pct))
				outnumberedFactionReward = 5;
		}

		Global.WorldStateMgr.SetValueAndSaveInDb(WorldStates.WarModeHordeBuffValue, 10 + (dominantFaction == TeamIds.Alliance ? outnumberedFactionReward : 0), false, null);
		Global.WorldStateMgr.SetValueAndSaveInDb(WorldStates.WarModeAllianceBuffValue, 10 + (dominantFaction == TeamIds.Horde ? outnumberedFactionReward : 0), false, null);
	}
}