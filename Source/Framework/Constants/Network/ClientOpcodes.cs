﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

namespace Framework.Constants;

public enum ClientOpcodes : uint
{
	AbandonNpeResponse = 0x33eb,
	AcceptGuildInvite = 0x35fc,
	AcceptReturningPlayerPrompt = 0x3395,
	AcceptSocialContract = 0x373c,
	AcceptTrade = 0x315a,
	AcceptWargameInvite = 0x35e0,
	AccountNotificationAcknowledged = 0x3731,
	ActivateSoulbind = 0x33da,
	ActivateTaxi = 0x34b1,
	AddonList = 0x35d8,
	AddAccountCosmetic = 0x32b1,
	AddBattlenetFriend = 0x3658,
	AddFriend = 0x36cc,
	AddIgnore = 0x36d0,
	AddToy = 0x32b0,
	AdventureJournalOpenQuest = 0x3204,
	AdventureJournalUpdateSuggestions = 0x33dd,
	AdventureMapStartQuest = 0x3369,
	AlterAppearance = 0x3503,
	AreaSpiritHealerQuery = 0x34b6,
	AreaSpiritHealerQueue = 0x34b7,
	AreaTrigger = 0x31d8,
	ArtifactAddPower = 0x31ab,
	ArtifactSetAppearance = 0x31ad,
	AssignEquipmentSetSpec = 0x3210,
	AttackStop = 0x3263,
	AttackSwing = 0x3262,
	AuctionableTokenSell = 0x36e2,
	AuctionableTokenSellAtMarketPrice = 0x36e3,
	AuctionBrowseQuery = 0x34d6,
	AuctionCancelCommoditiesPurchase = 0x34de,
	AuctionConfirmCommoditiesPurchase = 0x34dd,
	AuctionGetCommodityQuote = 0x34dc,
	AuctionHelloRequest = 0x34d1,
	AuctionListBiddedItems = 0x34da,
	AuctionListBucketsByBucketKeys = 0x34db,
	AuctionListItemsByBucketKey = 0x34d7,
	AuctionListItemsByItemId = 0x34d8,
	AuctionListOwnedItems = 0x34d9,
	AuctionPlaceBid = 0x34d5,
	AuctionRemoveItem = 0x34d3,
	AuctionReplicateItems = 0x34d4,
	AuctionRequestFavoriteList = 0x3733,
	AuctionSellCommodity = 0x34df,
	AuctionSellItem = 0x34d2,
	AuctionSetFavoriteItem = 0x3732,
	AuthContinuedSession = 0x3766,
	AuthSession = 0x3765,
	AutobankItem = 0x3997,
	AutobankReagent = 0x3999,
	AutostoreBankItem = 0x3996,
	AutostoreBankReagent = 0x3998,
	AutoEquipItem = 0x399a,
	AutoEquipItemSlot = 0x399f,
	AutoGuildBankItem = 0x34bd,
	AutoStoreBagItem = 0x399b,
	AutoStoreGuildBankItem = 0x34c6,
	AzeriteEmpoweredItemSelectPower = 0x3391,
	AzeriteEmpoweredItemViewed = 0x3374,
	AzeriteEssenceActivateEssence = 0x3393,
	AzeriteEssenceUnlockMilestone = 0x3392,
	BankerActivate = 0x34b9,
	BattlefieldLeave = 0x3173,
	BattlefieldList = 0x317f,
	BattlefieldPort = 0x3538,
	BattlemasterHello = 0x32cd,
	BattlemasterJoin = 0x3530,
	BattlemasterJoinArena = 0x3531,
	BattlemasterJoinBrawl = 0x3536,
	BattlemasterJoinRatedSoloShuffle = 0x3532,
	BattlemasterJoinSkirmish = 0x3533,
	BattlenetChallengeResponse = 0x36cf,
	BattlenetRequest = 0x36f1,
	BattlePayAckFailedResponse = 0x36c9,
	BattlePayCancelOpenCheckout = 0x370f,
	BattlePayConfirmPurchaseResponse = 0x36c8,
	BattlePayDistributionAssignToTarget = 0x36bf,
	BattlePayDistributionAssignVas = 0x3736,
	BattlePayGetProductList = 0x36b7,
	BattlePayGetPurchaseList = 0x36b8,
	BattlePayOpenCheckout = 0x3708,
	BattlePayRequestPriceInfo = 0x3704,
	BattlePayStartPurchase = 0x36c7,
	BattlePayStartVasPurchase = 0x36ee,
	BattlePetClearFanfare = 0x312b,
	BattlePetDeletePet = 0x3623,
	BattlePetDeletePetCheat = 0x3624,
	BattlePetModifyName = 0x3626,
	BattlePetRequestJournal = 0x3622,
	BattlePetRequestJournalLock = 0x3621,
	BattlePetSetBattleSlot = 0x362b,
	BattlePetSetFlags = 0x362e,
	BattlePetSummon = 0x3627,
	BattlePetUpdateDisplayNotify = 0x31e2,
	BattlePetUpdateNotify = 0x31e1,
	BeginTrade = 0x3157,
	BinderActivate = 0x34b8,
	BlackMarketBidOnItem = 0x3540,
	BlackMarketOpen = 0x353e,
	BlackMarketRequestItems = 0x353f,
	BonusRoll = 0x3394,
	BugReport = 0x3683,
	BusyTrade = 0x3158,
	BuyBackItem = 0x34aa,
	BuyBankSlot = 0x34ba,
	BuyItem = 0x34a9,
	BuyReagentBank = 0x34bb,
	CageBattlePet = 0x31f4,
	CalendarAddEvent = 0x367b,
	CalendarCommunityInvite = 0x366f,
	CalendarComplain = 0x3677,
	CalendarCopyEvent = 0x3676,
	CalendarEventSignUp = 0x3679,
	CalendarGet = 0x366d,
	CalendarGetEvent = 0x366e,
	CalendarGetNumPending = 0x3678,
	CalendarInvite = 0x3670,
	CalendarModeratorStatus = 0x3674,
	CalendarRemoveEvent = 0x3675,
	CalendarRemoveInvite = 0x3671,
	CalendarRsvp = 0x3672,
	CalendarStatus = 0x3673,
	CalendarUpdateEvent = 0x367c,
	CancelAura = 0x31af,
	CancelAutoRepeatSpell = 0x34f5,
	CancelCast = 0x32b7,
	CancelChannelling = 0x327b,
	CancelGrowthAura = 0x3283,
	CancelMasterLootRoll = 0x321c,
	CancelModSpeedNoControlAuras = 0x31ae,
	CancelMountAura = 0x3296,
	CancelQueuedSpell = 0x3180,
	CancelTempEnchantment = 0x3500,
	CancelTrade = 0x315c,
	CanDuel = 0x3660,
	CanRedeemTokenForBalance = 0x3703,
	CastSpell = 0x32b4,
	ChallengeModeRequestLeaders = 0x308f,
	ChangeBagSlotFlag = 0x334e,
	ChangeBankBagSlotFlag = 0x334f,
	ChangeMonumentAppearance = 0x332f,
	ChangeRealmTicket = 0x36f6,
	ChangeSubGroup = 0x364a,
	CharacterCheckUpgrade = 0x36c2,
	CharacterRenameRequest = 0x36bd,
	CharacterUpgradeManualUnrevokeRequest = 0x36c0,
	CharacterUpgradeStart = 0x36c1,
	CharCustomize = 0x368a,
	CharDelete = 0x369a,
	CharRaceOrFactionChange = 0x3690,
	ChatAddonMessage = 0x37ee,
	ChatAddonMessageTargeted = 0x37ef,
	ChatChannelAnnouncements = 0x37e3,
	ChatChannelBan = 0x37e1,
	ChatChannelDeclineInvite = 0x37e6,
	ChatChannelDisplayList = 0x37d6,
	ChatChannelInvite = 0x37df,
	ChatChannelKick = 0x37e0,
	ChatChannelList = 0x37d5,
	ChatChannelModerator = 0x37db,
	ChatChannelOwner = 0x37d9,
	ChatChannelPassword = 0x37d7,
	ChatChannelSetOwner = 0x37d8,
	ChatChannelSilenceAll = 0x37e4,
	ChatChannelUnban = 0x37e2,
	ChatChannelUnmoderator = 0x37dc,
	ChatChannelUnsilenceAll = 0x37e5,
	ChatJoinChannel = 0x37c8,
	ChatLeaveChannel = 0x37c9,
	ChatMessageAfk = 0x37d3,
	ChatMessageChannel = 0x37cf,
	ChatMessageDnd = 0x37d4,
	ChatMessageEmote = 0x37e8,
	ChatMessageGuild = 0x37d1,
	ChatMessageInstanceChat = 0x37ec,
	ChatMessageOfficer = 0x37d2,
	ChatMessageParty = 0x37ea,
	ChatMessageRaid = 0x37eb,
	ChatMessageRaidWarning = 0x37ed,
	ChatMessageSay = 0x37e7,
	ChatMessageWhisper = 0x37d0,
	ChatMessageYell = 0x37e9,
	ChatRegisterAddonPrefixes = 0x37cd,
	ChatReportFiltered = 0x37cc,
	ChatReportIgnored = 0x37cb,
	ChatUnregisterAllAddonPrefixes = 0x37ce,
	CheckCharacterNameAvailability = 0x3643,
	CheckIsAdventureMapPoiValid = 0x3254,
	ChoiceResponse = 0x32bc,
	ChromieTimeSelectExpansion = 0x33d9,
	ClaimWeeklyReward = 0x33b5,
	ClassTalentsDeleteConfig = 0x3410,
	ClassTalentsNotifyEmptyConfig = 0x3214,
	ClassTalentsNotifyValidationFailed = 0x3412,
	ClassTalentsRenameConfig = 0x340f,
	ClassTalentsRequestNewConfig = 0x340e,
	ClassTalentsSetStarterBuildActive = 0x3413,
	ClassTalentsSetUsesSharedActionBars = 0x3213,
	ClearNewAppearance = 0x312e,
	ClearRaidMarker = 0x31a7,
	ClearTradeItem = 0x315e,
	ClientPortGraveyard = 0x353a,
	CloseInteraction = 0x3499,
	CloseQuestChoice = 0x32bd,
	CloseRuneforgeInteraction = 0x33e1,
	CloseTraitSystemInteraction = 0x3414,
	ClubFinderApplicationResponse = 0x371a,
	ClubFinderGetApplicantsList = 0x3718,
	ClubFinderPost = 0x3715,
	ClubFinderRequestClubsData = 0x371c,
	ClubFinderRequestClubsList = 0x3716,
	ClubFinderRequestMembershipToClub = 0x3717,
	ClubFinderRequestPendingClubsList = 0x371b,
	ClubFinderRequestSubscribedClubPostingIds = 0x371d,
	ClubFinderRespondToApplicant = 0x3719,
	ClubFinderWhisperApplicantRequest = 0x3739,
	ClubPresenceSubscribe = 0x36f3,
	CollectionItemSetFavorite = 0x3631,
	CommentatorEnable = 0x35f0,
	CommentatorEnterInstance = 0x35f4,
	CommentatorExitInstance = 0x35f5,
	CommentatorGetMapInfo = 0x35f1,
	CommentatorGetPlayerCooldowns = 0x35f3,
	CommentatorGetPlayerInfo = 0x35f2,
	CommentatorSpectate = 0x3737,
	CommentatorStartWargame = 0x35ef,
	CommerceTokenGetCount = 0x36e0,
	CommerceTokenGetLog = 0x36ea,
	CommerceTokenGetMarketPrice = 0x36e1,
	Complaint = 0x366a,
	CompleteCinematic = 0x3558,
	CompleteMovie = 0x34eb,
	ConfirmArtifactRespec = 0x31ac,
	ConfirmRespecWipe = 0x3216,
	ConnectToFailed = 0x35d4,
	ConsumableTokenBuy = 0x36e5,
	ConsumableTokenBuyAtMarketPrice = 0x36e6,
	ConsumableTokenCanVeteranBuy = 0x36e4,
	ConsumableTokenRedeem = 0x36e8,
	ConsumableTokenRedeemConfirmation = 0x36e9,
	ContributionContribute = 0x356c,
	ContributionLastUpdateRequest = 0x356d,
	ConversationCinematicReady = 0x355a,
	ConversationLineStarted = 0x3559,
	ConvertRaid = 0x364c,
	CovenantRenownRequestCatchupState = 0x3580,
	CraftingOrderCancel = 0x358b,
	CraftingOrderClaim = 0x3588,
	CraftingOrderCreate = 0x3585,
	CraftingOrderFulfill = 0x358a,
	CraftingOrderListCrafterOrders = 0x3587,
	CraftingOrderListMyOrders = 0x3586,
	CraftingOrderReject = 0x358c,
	CraftingOrderRelease = 0x3589,
	CraftingOrderUpdateIgnoreList = 0x358d,
	CreateCharacter = 0x3642,
	CreateShipment = 0x331a,
	DbQueryBulk = 0x35e4,
	DeclineGuildInvites = 0x352d,
	DeclinePetition = 0x3547,
	DeleteEquipmentSet = 0x3519,
	DelFriend = 0x36cd,
	DelIgnore = 0x36d1,
	DepositReagentBank = 0x3357,
	DestroyItem = 0x32aa,
	DfBootPlayerVote = 0x3616,
	DfConfirmExpandSearch = 0x3608,
	DfGetJoinStatus = 0x3614,
	DfGetSystemInfo = 0x3613,
	DfJoin = 0x3609,
	DfLeave = 0x3612,
	DfProposalResponse = 0x3607,
	DfReadyCheckResponse = 0x3619,
	DfSetRoles = 0x3615,
	DfTeleport = 0x3617,
	DiscardedTimeSyncAcks = 0x3a41,
	DismissCritter = 0x3507,
	DoCountdown = 0x3714,
	DoMasterLootRoll = 0x321b,
	DoReadyCheck = 0x3632,
	DuelResponse = 0x34f0,
	EjectPassenger = 0x3249,
	Emote = 0x3554,
	EnableNagle = 0x376b,
	EnableTaxiNode = 0x34af,
	EngineSurvey = 0x36df,
	EnterEncryptedModeAck = 0x3767,
	EnumCharacters = 0x35e8,
	EnumCharactersDeletedByClient = 0x36d9,
	FarSight = 0x34f6,
	GameEventDebugDisable = 0x31b3,
	GameEventDebugEnable = 0x31b2,
	GameObjReportUse = 0x34fd,
	GameObjUse = 0x34fc,
	GarrisonAddFollowerHealth = 0x3315,
	GarrisonAssignFollowerToBuilding = 0x32fb,
	GarrisonCancelConstruction = 0x32e8,
	GarrisonCheckUpgradeable = 0x334a,
	GarrisonCompleteMission = 0x333c,
	GarrisonFullyHealAllFollowers = 0x3316,
	GarrisonGenerateRecruits = 0x32fe,
	GarrisonGetClassSpecCategoryInfo = 0x330d,
	GarrisonGetMapData = 0x3314,
	GarrisonGetMissionReward = 0x336d,
	GarrisonLearnTalent = 0x3309,
	GarrisonMissionBonusRoll = 0x333e,
	GarrisonPurchaseBuilding = 0x32e4,
	GarrisonRecruitFollower = 0x3300,
	GarrisonRemoveFollower = 0x3333,
	GarrisonRemoveFollowerFromBuilding = 0x32fc,
	GarrisonRenameFollower = 0x32fd,
	GarrisonRequestBlueprintAndSpecializationData = 0x32e3,
	GarrisonRequestShipmentInfo = 0x3318,
	GarrisonResearchTalent = 0x3301,
	GarrisonSetBuildingActive = 0x32e5,
	GarrisonSetFollowerFavorite = 0x32f9,
	GarrisonSetFollowerInactive = 0x32f1,
	GarrisonSetRecruitmentPreferences = 0x32ff,
	GarrisonSocketTalent = 0x33ee,
	GarrisonStartMission = 0x333b,
	GarrisonSwapBuildings = 0x32e9,
	GenerateRandomCharacterName = 0x35e7,
	GetAccountCharacterList = 0x36b2,
	GetAccountNotifications = 0x3730,
	GetGarrisonInfo = 0x32de,
	GetItemPurchaseData = 0x3542,
	GetLandingPageShipments = 0x3319,
	GetMirrorImageData = 0x32ae,
	GetPvpOptionsEnabled = 0x35ee,
	GetRafAccountInfo = 0x371e,
	GetRemainingGameTime = 0x36e7,
	GetTrophyList = 0x332c,
	GetUndeleteCharacterCooldownStatus = 0x36db,
	GetVasAccountCharacterList = 0x36ec,
	GetVasTransferTargetRealmList = 0x36ed,
	GmTicketAcknowledgeSurvey = 0x368e,
	GmTicketGetCaseStatus = 0x368d,
	GmTicketGetSystemStatus = 0x368c,
	GossipRefreshOptions = 0x357f,
	GossipSelectOption = 0x349a,
	GuildAddBattlenetFriend = 0x308d,
	GuildAddRank = 0x3065,
	GuildAssignMemberRank = 0x3060,
	GuildAutoDeclineInvitation = 0x3062,
	GuildBankActivate = 0x34bc,
	GuildBankBuyTab = 0x34ca,
	GuildBankDepositMoney = 0x34cc,
	GuildBankLogQuery = 0x3083,
	GuildBankQueryTab = 0x34c9,
	GuildBankRemainingWithdrawMoneyQuery = 0x3084,
	GuildBankSetTabText = 0x3087,
	GuildBankTextQuery = 0x3088,
	GuildBankUpdateTab = 0x34cb,
	GuildBankWithdrawMoney = 0x34cd,
	GuildChallengeUpdateRequest = 0x307c,
	GuildChangeNameRequest = 0x307f,
	GuildDeclineInvitation = 0x3061,
	GuildDelete = 0x3069,
	GuildDeleteRank = 0x3066,
	GuildDemoteMember = 0x305f,
	GuildEventLogQuery = 0x3086,
	GuildGetAchievementMembers = 0x3072,
	GuildGetRanks = 0x306e,
	GuildGetRoster = 0x3074,
	GuildInviteByName = 0x3606,
	GuildLeave = 0x3063,
	GuildNewsUpdateSticky = 0x306f,
	GuildOfficerRemoveMember = 0x3064,
	GuildPermissionsQuery = 0x3085,
	GuildPromoteMember = 0x305e,
	GuildQueryMembersForRecipe = 0x306c,
	GuildQueryMemberRecipes = 0x306a,
	GuildQueryNews = 0x306d,
	GuildQueryRecipes = 0x306b,
	GuildReplaceGuildMaster = 0x3089,
	GuildSetAchievementTracking = 0x3070,
	GuildSetFocusedAchievement = 0x3071,
	GuildSetGuildMaster = 0x36c4,
	GuildSetMemberNote = 0x3073,
	GuildSetRankPermissions = 0x3068,
	GuildShiftRank = 0x3067,
	GuildUpdateInfoText = 0x3076,
	GuildUpdateMotdText = 0x3075,
	HearthAndResurrect = 0x3515,
	HideQuestChoice = 0x32be,
	HotfixRequest = 0x35e5,
	IgnoreTrade = 0x3159,
	InitiateRolePoll = 0x35da,
	InitiateTrade = 0x3156,
	Inspect = 0x353c,
	InstanceLockResponse = 0x351a,
	IslandQueue = 0x33b1,
	ItemPurchaseRefund = 0x3543,
	ItemTextQuery = 0x334b,
	JoinPetBattleQueue = 0x31df,
	JoinRatedBattleground = 0x3179,
	KeepAlive = 0x367d,
	KeyboundOverride = 0x322e,
	LatencyReport = 0x3771,
	LearnPvpTalents = 0x356b,
	LearnTalents = 0x3569,
	LeaveGroup = 0x3647,
	LeavePetBattleQueue = 0x31e0,
	LfgListApplyToGroup = 0x360d,
	LfgListCancelApplication = 0x360e,
	LfgListDeclineApplicant = 0x360f,
	LfgListGetStatus = 0x360b,
	LfgListInviteApplicant = 0x3610,
	LfgListInviteResponse = 0x3611,
	LfgListJoin = 0x338f,
	LfgListLeave = 0x360a,
	LfgListSearch = 0x360c,
	LfgListUpdateRequest = 0x3390,
	ListInventory = 0x34a7,
	LiveRegionAccountRestore = 0x36b5,
	LiveRegionCharacterCopy = 0x36b4,
	LiveRegionGetAccountCharacterList = 0x36b3,
	LiveRegionKeyBindingsCopy = 0x36b6,
	LoadingScreenNotify = 0x35f8,
	LoadSelectedTrophy = 0x332d,
	LogoutCancel = 0x34e6,
	LogoutInstant = 0x34e7,
	LogoutRequest = 0x34e4,
	LogDisconnect = 0x3769,
	LogStreamingError = 0x376d,
	LootItem = 0x3219,
	LootMoney = 0x3218,
	LootRelease = 0x321d,
	LootRoll = 0x321e,
	LootUnit = 0x3217,
	LowLevelRaid1 = 0x369e,
	LowLevelRaid2 = 0x3521,
	MailCreateTextItem = 0x354e,
	MailDelete = 0x3230,
	MailGetList = 0x3549,
	MailMarkAsRead = 0x354d,
	MailReturnToSender = 0x3653,
	MailTakeItem = 0x354b,
	MailTakeMoney = 0x354a,
	MakeContitionalAppearancePermanent = 0x3232,
	MasterLootItem = 0x321a,
	MergeGuildBankItemWithGuildBankItem = 0x34c7,
	MergeGuildBankItemWithItem = 0x34c4,
	MergeItemWithGuildBankItem = 0x34c2,
	MinimapPing = 0x3649,
	MissileTrajectoryCollision = 0x318b,
	MountClearFanfare = 0x312c,
	MountSetFavorite = 0x3630,
	MountSpecialAnim = 0x3297,
	MoveAddImpulseAck = 0x3a50,
	MoveApplyInertiaAck = 0x3a4e,
	MoveApplyMovementForceAck = 0x3a15,
	MoveChangeTransport = 0x3a2f,
	MoveChangeVehicleSeats = 0x3a34,
	MoveCollisionDisableAck = 0x3a39,
	MoveCollisionEnableAck = 0x3a3a,
	MoveDismissVehicle = 0x3a33,
	MoveDoubleJump = 0x39eb,
	MoveEnableDoubleJumpAck = 0x3a1e,
	MoveEnableSwimToFlyTransAck = 0x3a24,
	MoveFallLand = 0x39fb,
	MoveFallReset = 0x3a19,
	MoveFeatherFallAck = 0x3a1c,
	MoveForceFlightBackSpeedChangeAck = 0x3a2e,
	MoveForceFlightSpeedChangeAck = 0x3a2d,
	MoveForcePitchRateChangeAck = 0x3a32,
	MoveForceRootAck = 0x3a0e,
	MoveForceRunBackSpeedChangeAck = 0x3a0c,
	MoveForceRunSpeedChangeAck = 0x3a0b,
	MoveForceSwimBackSpeedChangeAck = 0x3a22,
	MoveForceSwimSpeedChangeAck = 0x3a0d,
	MoveForceTurnRateChangeAck = 0x3a23,
	MoveForceUnrootAck = 0x3a0f,
	MoveForceWalkSpeedChangeAck = 0x3a21,
	MoveGravityDisableAck = 0x3a35,
	MoveGravityEnableAck = 0x3a36,
	MoveGuildBankItem = 0x34c1,
	MoveHeartbeat = 0x3a10,
	MoveHoverAck = 0x3a13,
	MoveInertiaDisableAck = 0x3a37,
	MoveInertiaEnableAck = 0x3a38,
	MoveInitActiveMoverComplete = 0x3a46,
	MoveJump = 0x39ea,
	MoveKnockBackAck = 0x3a12,
	MoveRemoveInertiaAck = 0x3a4f,
	MoveRemoveMovementForces = 0x3a17,
	MoveRemoveMovementForceAck = 0x3a16,
	MoveSeamlessTransferComplete = 0x3a44,
	MoveSetAdvFly = 0x3a52,
	MoveSetAdvFlyingAddImpulseMaxSpeedAck = 0x3a58,
	MoveSetAdvFlyingAirFrictionAck = 0x3a53,
	MoveSetAdvFlyingBankingRateAck = 0x3a59,
	MoveSetAdvFlyingDoubleJumpVelModAck = 0x3a56,
	MoveSetAdvFlyingGlideStartMinHeightAck = 0x3a57,
	MoveSetAdvFlyingLaunchSpeedCoefficientAck = 0x3a60,
	MoveSetAdvFlyingLiftCoefficientAck = 0x3a55,
	MoveSetAdvFlyingMaxVelAck = 0x3a54,
	MoveSetAdvFlyingOverMaxDecelerationAck = 0x3a5e,
	MoveSetAdvFlyingPitchingRateDownAck = 0x3a5a,
	MoveSetAdvFlyingPitchingRateUpAck = 0x3a5b,
	MoveSetAdvFlyingSurfaceFrictionAck = 0x3a5d,
	MoveSetAdvFlyingTurnVelocityThresholdAck = 0x3a5c,
	MoveSetCanAdvFlyAck = 0x3a51,
	MoveSetCanFlyAck = 0x3a27,
	MoveSetCanTurnWhileFallingAck = 0x3a25,
	MoveSetCollisionHeightAck = 0x3a3b,
	MoveSetFacing = 0x3a09,
	MoveSetFacingHeartbeat = 0x3a5f,
	MoveSetFly = 0x3a28,
	MoveSetIgnoreMovementForcesAck = 0x3a26,
	MoveSetModMovementForceMagnitudeAck = 0x3a42,
	MoveSetPitch = 0x3a0a,
	MoveSetRunMode = 0x39f2,
	MoveSetTurnRateCheat = 0x3a06,
	MoveSetVehicleRecIdAck = 0x3a14,
	MoveSetWalkMode = 0x39f3,
	MoveSplineDone = 0x3a18,
	MoveStartAscend = 0x3a29,
	MoveStartBackward = 0x39e5,
	MoveStartDescend = 0x3a30,
	MoveStartForward = 0x39e4,
	MoveStartPitchDown = 0x39f0,
	MoveStartPitchUp = 0x39ef,
	MoveStartStrafeLeft = 0x39e7,
	MoveStartStrafeRight = 0x39e8,
	MoveStartSwim = 0x39fc,
	MoveStartTurnLeft = 0x39ec,
	MoveStartTurnRight = 0x39ed,
	MoveStop = 0x39e6,
	MoveStopAscend = 0x3a2a,
	MoveStopPitch = 0x39f1,
	MoveStopStrafe = 0x39e9,
	MoveStopSwim = 0x39fd,
	MoveStopTurn = 0x39ee,
	MoveTeleportAck = 0x39fa,
	MoveTimeSkipped = 0x3a1b,
	MoveUpdateFallSpeed = 0x3a1a,
	MoveWaterWalkAck = 0x3a1d,
	MythicPlusRequestMapStats = 0x308e,
	NeutralPlayerSelectFaction = 0x31d5,
	NextCinematicCamera = 0x3557,
	ObjectUpdateFailed = 0x3181,
	ObjectUpdateRescued = 0x3182,
	OfferPetition = 0x33d8,
	OpeningCinematic = 0x3556,
	OpenItem = 0x334c,
	OpenMissionNpc = 0x330f,
	OpenShipmentNpc = 0x3317,
	OpenTradeskillNpc = 0x3322,
	OptOutOfLoot = 0x3504,
	OverrideScreenFlash = 0x352e,
	PartyInvite = 0x3602,
	PartyInviteResponse = 0x3604,
	PartyUninvite = 0x3645,
	PerformItemInteraction = 0x323a,
	PerksProgramRequestPendingRewards = 0x313a,
	PerksProgramRequestPurchase = 0x3400,
	PerksProgramRequestRefund = 0x3401,
	PerksProgramSetFrozenVendorItem = 0x3402,
	PerksProgramStatusRequest = 0x33ff,
	PetitionBuy = 0x34cf,
	PetitionRenameGuild = 0x36c5,
	PetitionShowList = 0x34ce,
	PetitionShowSignatures = 0x34d0,
	PetAbandon = 0x3493,
	PetAction = 0x3491,
	PetBattleFinalNotify = 0x31e4,
	PetBattleInput = 0x363f,
	PetBattleQueueProposeMatchResult = 0x322f,
	PetBattleQuitNotify = 0x31e3,
	PetBattleReplaceFrontPet = 0x3640,
	PetBattleRequestPvp = 0x31dd,
	PetBattleRequestUpdate = 0x31de,
	PetBattleRequestWild = 0x31db,
	PetBattleScriptErrorNotify = 0x31e5,
	PetBattleWildLocationFail = 0x31dc,
	PetCancelAura = 0x3494,
	PetCastSpell = 0x32b3,
	PetRename = 0x3682,
	PetSetAction = 0x3490,
	PetSpellAutocast = 0x3495,
	PetStopAttack = 0x3492,
	Ping = 0x3768,
	PlayerLogin = 0x35ea,
	PushQuestToParty = 0x34a5,
	PvpLogData = 0x317c,
	QueryBattlePetName = 0x328a,
	QueryCorpseLocationFromClient = 0x365e,
	QueryCorpseTransport = 0x365f,
	QueryCountdownTimer = 0x31aa,
	QueryCreature = 0x3284,
	QueryGameObject = 0x3285,
	QueryGarrisonPetName = 0x328b,
	QueryGuildInfo = 0x3688,
	QueryInspectAchievements = 0x350e,
	QueryNextMailTime = 0x354c,
	QueryNpcText = 0x3286,
	QueryPageText = 0x3288,
	QueryPetition = 0x328c,
	QueryPetName = 0x3289,
	QueryPlayerNames = 0x3772,
	QueryPlayerNamesForCommunity = 0x3770,
	QueryPlayerNameByCommunityId = 0x376f,
	QueryQuestCompletionNpcs = 0x3175,
	QueryQuestInfo = 0x3287,
	QueryQuestItemUsability = 0x3176,
	QueryRealmName = 0x3687,
	QueryScenarioPoi = 0x3654,
	QueryTime = 0x34e3,
	QueryTreasurePicker = 0x3370,
	QueryVoidStorage = 0x31a3,
	QuestConfirmAccept = 0x34a4,
	QuestGiverAcceptQuest = 0x349e,
	QuestGiverChooseReward = 0x34a0,
	QuestGiverCloseQuest = 0x355d,
	QuestGiverCompleteQuest = 0x349f,
	QuestGiverHello = 0x349c,
	QuestGiverQueryQuest = 0x349d,
	QuestGiverRequestReward = 0x34a1,
	QuestGiverStatusMultipleQuery = 0x34a3,
	QuestGiverStatusQuery = 0x34a2,
	QuestGiverStatusTrackedQuery = 0x358f,
	QuestLogRemoveQuest = 0x3541,
	QuestPoiQuery = 0x36ac,
	QuestPushResult = 0x34a6,
	QuestSessionBeginResponse = 0x33c9,
	QuestSessionRequestStart = 0x33c8,
	QuestSessionRequestStop = 0x3729,
	QueuedMessagesEnd = 0x376c,
	QuickJoinAutoAcceptRequests = 0x3702,
	QuickJoinRequestInvite = 0x3701,
	QuickJoinRequestInviteWithConfirmation = 0x372e,
	QuickJoinRespondToInvite = 0x3700,
	QuickJoinSignalToastDisplayed = 0x36ff,
	RafClaimActivityReward = 0x3512,
	RafClaimNextReward = 0x371f,
	RafGenerateRecruitmentLink = 0x3721,
	RafUpdateRecruitmentInfo = 0x3720,
	RandomRoll = 0x3652,
	ReadyCheckResponse = 0x3633,
	ReadItem = 0x334d,
	ReclaimCorpse = 0x34e9,
	RemoveNewItem = 0x3373,
	RemoveRafRecruit = 0x3722,
	ReorderCharacters = 0x35e9,
	RepairItem = 0x34fa,
	ReplaceTrophy = 0x332e,
	RepopRequest = 0x3539,
	ReportClientVariables = 0x36fc,
	ReportEnabledAddons = 0x36fb,
	ReportFrozenWhileLoadingMap = 0x36a4,
	ReportKeybindingExecutionCounts = 0x36fd,
	ReportPvpPlayerAfk = 0x3502,
	ReportServerLag = 0x33c1,
	ReportStuckInCombat = 0x33c2,
	RequestAccountData = 0x3692,
	RequestAreaPoiUpdate = 0x3372,
	RequestBattlefieldStatus = 0x35dc,
	RequestCategoryCooldowns = 0x317e,
	RequestCemeteryList = 0x3177,
	RequestCharacterGuildFollowInfo = 0x3689,
	RequestConquestFormulaConstants = 0x32d0,
	RequestCovenantCallings = 0x33b3,
	RequestCrowdControlSpell = 0x353d,
	RequestForcedReactions = 0x320e,
	RequestGarrisonTalentWorldQuestUnlocks = 0x33ed,
	RequestGuildPartyState = 0x31a9,
	RequestGuildRewardsList = 0x31a8,
	RequestLatestSplashScreen = 0x33c3,
	RequestLfgListBlacklist = 0x32bf,
	RequestMythicPlusAffixes = 0x3208,
	RequestMythicPlusSeasonData = 0x3209,
	RequestPartyJoinUpdates = 0x35f7,
	RequestPartyMemberStats = 0x3651,
	RequestPetInfo = 0x3496,
	RequestPlayedTime = 0x328f,
	RequestPvpRewards = 0x3196,
	RequestRaidInfo = 0x36c6,
	RequestRatedPvpInfo = 0x35e3,
	RequestRealmGuildMasterInfo = 0x309a,
	RequestResearchHistory = 0x3167,
	RequestScheduledPvpInfo = 0x3197,
	RequestStabledPets = 0x3497,
	RequestVehicleExit = 0x3244,
	RequestVehicleNextSeat = 0x3246,
	RequestVehiclePrevSeat = 0x3245,
	RequestVehicleSwitchSeat = 0x3247,
	RequestWeeklyRewards = 0x33b6,
	RequestWorldQuestUpdate = 0x3371,
	ResetChallengeMode = 0x3206,
	ResetChallengeModeCheat = 0x3207,
	ResetInstances = 0x3666,
	ResurrectResponse = 0x3681,
	RevertMonumentAppearance = 0x3330,
	RideVehicleInteract = 0x3248,
	SaveCufProfiles = 0x318c,
	SaveEquipmentSet = 0x3518,
	SaveGuildEmblem = 0x32c3,
	ScenePlaybackCanceled = 0x322b,
	ScenePlaybackComplete = 0x322a,
	SceneTriggerEvent = 0x322c,
	SelfRes = 0x3544,
	SellItem = 0x34a8,
	SendCharacterClubInvitation = 0x36f5,
	SendContactList = 0x36cb,
	SendMail = 0x35fa,
	SendTextEmote = 0x348e,
	ServerTimeOffsetRequest = 0x3699,
	SetAchievementsHidden = 0x3231,
	SetActionBarToggles = 0x3545,
	SetActionButton = 0x3634,
	SetActiveMover = 0x3a3c,
	SetAdvancedCombatLogging = 0x32d1,
	SetAssistantLeader = 0x364d,
	SetBackpackAutosortDisabled = 0x3350,
	SetBankAutosortDisabled = 0x3351,
	SetContactNotes = 0x36ce,
	SetCurrencyFlags = 0x3169,
	SetDifficultyId = 0x322d,
	SetDungeonDifficulty = 0x3680,
	SetEmpowerMinHoldStagePercent = 0x327e,
	SetEveryoneIsAssistant = 0x3618,
	SetFactionAtWar = 0x34ec,
	SetFactionInactive = 0x34ee,
	SetFactionNotAtWar = 0x34ed,
	SetGameEventDebugViewState = 0x31ba,
	SetInsertItemsLeftToRight = 0x3353,
	SetLootMethod = 0x3646,
	SetLootSpecialization = 0x3552,
	SetPartyAssignment = 0x364f,
	SetPartyLeader = 0x3648,
	SetPetSlot = 0x3168,
	SetPlayerDeclinedNames = 0x3686,
	SetPreferredCemetery = 0x3178,
	SetPvp = 0x32c7,
	SetRaidDifficulty = 0x36d7,
	SetRole = 0x35d9,
	SetSavedInstanceExtend = 0x3684,
	SetSelection = 0x353b,
	SetSheathed = 0x348f,
	SetSortBagsRightToLeft = 0x3352,
	SetTaxiBenchmarkMode = 0x3501,
	SetTitle = 0x3295,
	SetTradeCurrency = 0x3160,
	SetTradeGold = 0x315f,
	SetTradeItem = 0x315d,
	SetUsingPartyGarrison = 0x3311,
	SetWarMode = 0x32c8,
	SetWatchedFaction = 0x34ef,
	ShowTradeSkill = 0x36be,
	SignPetition = 0x3546,
	SilencePartyTalker = 0x3650,
	SocialContractRequest = 0x373b,
	SocketGems = 0x34f9,
	SortBags = 0x3354,
	SortBankBags = 0x3355,
	SortReagentBankBags = 0x3356,
	SpellClick = 0x349b,
	SpellEmpowerRelease = 0x327c,
	SpellEmpowerRestart = 0x327d,
	SpiritHealerActivate = 0x34b5,
	SplitGuildBankItem = 0x34c8,
	SplitGuildBankItemToInventory = 0x34c5,
	SplitItem = 0x399e,
	SplitItemToGuildBank = 0x34c3,
	StandStateChange = 0x318a,
	StartChallengeMode = 0x355e,
	StartSpectatorWarGame = 0x35df,
	StartWarGame = 0x35de,
	StoreGuildBankItem = 0x34be,
	SubmitUserFeedback = 0x3691,
	SubscriptionInterstitialResponse = 0x33e2,
	SummonResponse = 0x3668,
	SupportTicketSubmitComplaint = 0x3644,
	SurrenderArena = 0x3174,
	SuspendCommsAck = 0x3764,
	SuspendTokenResponse = 0x376a,
	SwapGuildBankItemWithGuildBankItem = 0x34c0,
	SwapInvItem = 0x399d,
	SwapItem = 0x399c,
	SwapItemWithGuildBankItem = 0x34bf,
	SwapSubGroups = 0x364b,
	SwapVoidItem = 0x31a5,
	TabardVendorActivate = 0x32c4,
	TalkToGossip = 0x3498,
	TaxiNodeStatusQuery = 0x34ae,
	TaxiQueryAvailableNodes = 0x34b0,
	TaxiRequestEarlyLanding = 0x34b2,
	TimeAdjustmentResponse = 0x3a40,
	TimeSyncResponse = 0x3a3d,
	TimeSyncResponseDropped = 0x3a3f,
	TimeSyncResponseFailed = 0x3a3e,
	ToggleDifficulty = 0x3655,
	TogglePvp = 0x32c6,
	TotemDestroyed = 0x3506,
	ToyClearFanfare = 0x312d,
	TradeSkillSetFavorite = 0x336f,
	TrainerBuySpell = 0x34b4,
	TrainerList = 0x34b3,
	TraitsCommitConfig = 0x3408,
	TraitsTalentTestUnlearnSpells = 0x3406,
	TransmogrifyItems = 0x3198,
	TurnInPetition = 0x3548,
	Tutorial = 0x36d8,
	TwitterCheckStatus = 0x3129,
	TwitterConnect = 0x3126,
	TwitterDisconnect = 0x312a,
	UiMapQuestLinesRequest = 0x33b2,
	UnacceptTrade = 0x315b,
	UndeleteCharacter = 0x36da,
	UnlearnSkill = 0x34f3,
	UnlearnSpecialization = 0x31a6,
	UnlockVoidStorage = 0x31a2,
	UpdateAadcStatus = 0x3735,
	UpdateAccountData = 0x3693,
	UpdateAreaTriggerVisual = 0x32b6,
	UpdateClientSettings = 0x3662,
	UpdateCraftingNpcRecipes = 0x3323,
	UpdateMissileTrajectory = 0x3a43,
	UpdateRaidTarget = 0x364e,
	UpdateSpellVisual = 0x32b5,
	UpdateVasPurchaseStates = 0x36ef,
	UpgradeGarrison = 0x32d9,
	UpgradeRuneforgeLegendary = 0x33e0,
	UsedFollow = 0x3187,
	UseCritterItem = 0x324e,
	UseEquipmentSet = 0x3995,
	UseItem = 0x32af,
	UseToy = 0x32b2,
	VasCheckTransferOk = 0x3707,
	VasGetQueueMinutes = 0x3706,
	VasGetServiceStatus = 0x3705,
	ViolenceLevel = 0x3185,
	VoiceChannelSttTokenRequest = 0x370b,
	VoiceChatJoinChannel = 0x370c,
	VoiceChatLogin = 0x370a,
	VoidStorageTransfer = 0x31a4,
	Warden3Data = 0x35ec,
	Who = 0x367f,
	WhoIs = 0x367e,
	WorldPortResponse = 0x35f9,
	WrapItem = 0x3994,

	Max = 0x3FFF,
	Unknown = 0xbadd
}