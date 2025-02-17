// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.World.GameObjects;

internal struct SpellIds
{
	//CatFigurine
	public const uint SummonGhostSaber = 5968;

	//EthereumPrison
	public const uint RepLc = 39456;
	public const uint RepShat = 39457;
	public const uint RepCe = 39460;
	public const uint RepCon = 39474;
	public const uint RepKt = 39475;
	public const uint RepSpor = 39476;

	//Southfury
	public const uint Blackjack = 39865; //Stuns Player
	public const uint SummonRizzle = 39866;

	//Felcrystalforge
	public const uint Create1FlaskOfBeast = 40964;
	public const uint Create5FlaskOfBeast = 40965;

	//Bashircrystalforge
	public const uint Create1FlaskOfSorcerer = 40968;
	public const uint Create5FlaskOfSorcerer = 40970;

	//Jotunheimcage
	public const uint SummonBladeKnightH = 56207;
	public const uint SummonBladeKnightNe = 56209;
	public const uint SummonBladeKnightOrc = 56212;
	public const uint SummonBladeKnightTroll = 56214;

	//Amberpineouthouse
	public const uint Indisposed = 53017;
	public const uint IndisposedIii = 48341;
	public const uint CreateAmberseeds = 48330;

	//Thecleansing
	public const uint CleansingSoul = 43351;
	public const uint RecentMeditation = 61720;

	//Midsummerbonfire
	public const uint StampOutBonfireQuestComplete = 45458;

	//MidsummerPoleRibbon
	public static uint[] RibbonPoleSpells =
	{
		29705, 29726, 29727
	};

	//Toy Train Set
	public const uint ToyTrainPulse = 61551;
}

internal struct CreatureIds
{
	//GildedBrazier
	public const uint Stillblade = 17716;

	//EthereumPrison
	public static uint[] PrisonEntry =
	{
		22810, 22811, 22812, 22813, 22814, 22815,       //Good Guys
		20783, 20784, 20785, 20786, 20788, 20789, 20790 //Bad Guys
	};

	//Ethereum Stasis
	public static uint[] StasisEntry =
	{
		22825, 20888, 22827, 22826, 22828
	};

	//ResoniteCask
	public const uint Goggeroc = 11920;

	//Sacredfireoflife
	public const uint Arikara = 10882;

	//Southfury
	public const uint Rizzle = 23002;

	//Bloodfilledorb
	public const uint Zelemar = 17830;

	//Jotunheimcage
	public const uint EbonBladePrisonerHuman = 30186;
	public const uint EbonBladePrisonerNe = 30194;
	public const uint EbonBladePrisonerTroll = 30196;
	public const uint EbonBladePrisonerOrc = 30195;

	//Tadpoles
	public const uint WinterfinTadpole = 25201;

	//Amberpineouthouse
	public const uint OuthouseBunny = 27326;

	//Missingfriends
	public const uint CaptiveChild = 22314;

	//MidsummerPoleRibbon
	public const uint PoleRibbonBunny = 17066;
}

internal struct GameObjectIds
{
	//Bellhourlyobjects
	public const uint HordeBell = 175885;
	public const uint AllianceBell = 176573;
	public const uint KharazhanBell = 182064;
}

internal struct ItemIds
{
	//Amberpineouthouse
	public const uint AnderholsSliderCider = 37247;
}

internal struct QuestIds
{
	//GildedBrazier
	public const uint TheFirstTrial = 9678;

	//Dalarancrystal
	public const uint LearnLeaveReturn = 12790;
	public const uint TeleCrystalFlag = 12845;

	//Tadpoles
	public const uint OhNoesTheTadpoles = 11560;

	//Amberpineouthouse
	public const uint DoingYourDuty = 12227;

	//Missingfriends
	public const uint MissingFriends = 10852;

	//Thecleansing
	public const uint TheCleansingHorde = 11317;
	public const uint TheCleansingAlliance = 11322;
}

internal struct TextIds
{
	//Missingfriends
	public const uint SayFree0 = 0;
}

internal struct GossipConst
{
	//Dalarancrystal
	public const string GoTeleToDalaranCrystalFailed = "This Teleport Crystal Cannot Be Used Until The Teleport Crystal In Dalaran Has Been Used At Least Once.";

	//Felcrystalforge
	public const uint GossipFelCrystalforgeText = 31000;
	public const uint GossipFelCrystalforgeItemTextReturn = 31001;
	public const string GossipFelCrystalforgeItem1 = "Purchase 1 Unstable Flask Of The Beast For The Cost Of 10 Apexis Shards";
	public const string GossipFelCrystalforgeItem5 = "Purchase 5 Unstable Flask Of The Beast For The Cost Of 50 Apexis Shards";
	public const string GossipFelCrystalforgeItemReturn = "Use The Fel Crystalforge To Make Another Purchase.";

	//Bashircrystalforge
	public const uint GossipBashirCrystalforgeText = 31100;
	public const uint GossipBashirCrystalforgeItemTextReturn = 31101;
	public const string GossipBashirCrystalforgeItem1 = "Purchase 1 Unstable Flask Of The Sorcerer For The Cost Of 10 Apexis Shards";
	public const string GossipBashirCrystalforgeItem5 = "Purchase 5 Unstable Flask Of The Sorcerer For The Cost Of 50 Apexis Shards";
	public const string GossipBashirCrystalforgeItemReturn = "Use The Bashir Crystalforge To Make Another Purchase.";

	//Amberpineouthouse
	public const uint GossipOuthouseInuse = 12775;
	public const uint GossipOuthouseVacant = 12779;

	public const string GossipUseOuthouse = "Use The Outhouse.";
	public const string AnderholsSliderCiderNotFound = "Quest Item Anderhol'S Slider Cider Not Found.";
}

internal struct SoundIds
{
	//BrewfestMusic
	public const uint EventBrewfestdwarf01 = 11810;  // 1.35 Min
	public const uint EventBrewfestdwarf02 = 11812;  // 1.55 Min 
	public const uint EventBrewfestdwarf03 = 11813;  // 0.23 Min
	public const uint EventBrewfestgoblin01 = 11811; // 1.08 Min
	public const uint EventBrewfestgoblin02 = 11814; // 1.33 Min
	public const uint EventBrewfestgoblin03 = 11815; // 0.28 Min

	//Brewfestmusicevents
	public const uint EventBmSelectMusic = 1;
	public const uint EventBmStartMusic = 2;

	//Bells
	//BellHourlySoundFX
	public const uint BellTollHorde = 6595; // Horde
	public const uint BellTollTribal = 6675;
	public const uint BellTollAlliance = 6594; // Alliance
	public const uint BellTollNightelf = 6674;
	public const uint BellTolldwarfgnome = 7234;
	public const uint BellTollKharazhan = 9154; // Kharazhan
}

internal struct AreaIds
{
	public const uint Silvermoon = 3430; // Horde
	public const uint Undercity = 1497;
	public const uint Orgrimmar1 = 1296;
	public const uint Orgrimmar2 = 14;
	public const uint Thunderbluff = 1638;
	public const uint Ironforge1 = 809; // Alliance
	public const uint Ironforge2 = 1;
	public const uint Stormwind = 12;
	public const uint Exodar = 3557;
	public const uint Darnassus = 1657;
	public const uint Shattrath = 3703; // General
	public const uint TeldrassilZone = 141;
	public const uint KharazhanMapid = 532;
}

internal struct ZoneIds
{
	public const uint Tirisfal = 85;
	public const uint Undercity = 1497;
	public const uint DunMorogh = 1;
	public const uint Ironforge = 1537;
	public const uint Teldrassil = 141;
	public const uint Darnassus = 1657;
	public const uint Ashenvale = 331;
	public const uint HillsbradFoothills = 267;
	public const uint Duskwood = 10;
}

internal struct Misc
{
	// These Are In Seconds
	//Brewfestmusictime
	public static TimeSpan EventBrewfestdwarf01Time = TimeSpan.FromSeconds(95);
	public static TimeSpan EventBrewfestdwarf02Time = TimeSpan.FromSeconds(155);
	public static TimeSpan EventBrewfestdwarf03Time = TimeSpan.FromSeconds(23);
	public static TimeSpan EventBrewfestgoblin01Time = TimeSpan.FromSeconds(68);
	public static TimeSpan EventBrewfestgoblin02Time = TimeSpan.FromSeconds(93);
	public static TimeSpan EventBrewfestgoblin03Time = TimeSpan.FromSeconds(28);

	//Bellhourlymisc
	public const uint GameEventHourlyBells = 73;
}

[Script]
internal class go_gilded_brazier : GameObjectAI
{
	public go_gilded_brazier(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		if (Me.GoType == GameObjectTypes.Goober)
			if (player.GetQuestStatus(QuestIds.TheFirstTrial) == QuestStatus.Incomplete)
			{
				Creature Stillblade = player.SummonCreature(CreatureIds.Stillblade, 8106.11f, -7542.06f, 151.775f, 3.02598f, TempSummonType.DeadDespawn, TimeSpan.FromMinutes(1));

				if (Stillblade)
					Stillblade.AI.AttackStart(player);
			}

		return true;
	}
}

[Script]
internal class go_tablet_of_the_seven : GameObjectAI
{
	public go_tablet_of_the_seven(GameObject go) : base(go) { }

	/// @todo use gossip option ("Transcript the Tablet") instead, if Trinity adds support.
	public override bool OnGossipHello(Player player)
	{
		if (Me.GoType != GameObjectTypes.QuestGiver)
			return true;

		if (player.GetQuestStatus(4296) == QuestStatus.Incomplete)
			player.CastSpell(player, 15065, false);

		return true;
	}
}

[Script]
internal class go_ethereum_prison : GameObjectAI
{
	public go_ethereum_prison(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		Me.UseDoorOrButton();
		var Random = (int)(RandomHelper.Rand32() % (CreatureIds.PrisonEntry.Length / sizeof(uint)));

		Creature creature = player.SummonCreature(CreatureIds.PrisonEntry[Random], Me.Location.X, Me.Location.Y, Me.Location.Z, Me.Location.GetAbsoluteAngle(player.Location), TempSummonType.TimedDespawnOutOfCombat, TimeSpan.FromSeconds(30));

		if (creature)
			if (!creature.IsHostileTo(player))
			{
				var pFaction = creature.GetFactionTemplateEntry();

				if (pFaction != null)
				{
					uint spellId = 0;

					switch (pFaction.Faction)
					{
						case 1011:
							spellId = SpellIds.RepLc;

							break;
						case 935:
							spellId = SpellIds.RepShat;

							break;
						case 942:
							spellId = SpellIds.RepCe;

							break;
						case 933:
							spellId = SpellIds.RepCon;

							break;
						case 989:
							spellId = SpellIds.RepKt;

							break;
						case 970:
							spellId = SpellIds.RepSpor;

							break;
					}

					if (spellId != 0)
						creature.CastSpell(player, spellId, false);
					else
						Log.outError(LogFilter.Scripts, $"go_ethereum_prison summoned Creature (entry {creature.Entry}) but faction ({creature.Faction}) are not expected by script.");
				}
			}

		return false;
	}
}

[Script]
internal class go_ethereum_stasis : GameObjectAI
{
	public go_ethereum_stasis(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		Me.UseDoorOrButton();
		var Random = (int)(RandomHelper.Rand32() % CreatureIds.StasisEntry.Length / sizeof(uint));

		player.SummonCreature(CreatureIds.StasisEntry[Random], Me.Location.X, Me.Location.Y, Me.Location.Z, Me.Location.GetAbsoluteAngle(player.Location), TempSummonType.TimedDespawnOutOfCombat, TimeSpan.FromSeconds(30));

		return false;
	}
}

[Script]
internal class go_resonite_cask : GameObjectAI
{
	public go_resonite_cask(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		if (Me.GoType == GameObjectTypes.Goober)
			Me.SummonCreature(CreatureIds.Goggeroc, 0.0f, 0.0f, 0.0f, 0.0f, TempSummonType.TimedDespawnOutOfCombat, TimeSpan.FromMinutes(5));

		return false;
	}
}

[Script]
internal class go_southfury_moonstone : GameObjectAI
{
	public go_southfury_moonstone(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		//implicitTarget=48 not implemented as of writing this code, and manual summon may be just ok for our purpose
		//player.CastSpell(player, SpellSummonRizzle, false);

		Creature creature = player.SummonCreature(CreatureIds.Rizzle, 0.0f, 0.0f, 0.0f, 0.0f, TempSummonType.DeadDespawn);

		if (creature)
			creature.CastSpell(player, SpellIds.Blackjack, false);

		return false;
	}
}

[Script]
internal class go_tele_to_dalaran_crystal : GameObjectAI
{
	public go_tele_to_dalaran_crystal(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		if (player.GetQuestRewardStatus(QuestIds.TeleCrystalFlag))
			return false;

		player.Session.SendNotification(GossipConst.GoTeleToDalaranCrystalFailed);

		return true;
	}
}

[Script]
internal class go_tele_to_violet_stand : GameObjectAI
{
	public go_tele_to_violet_stand(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		if (player.GetQuestRewardStatus(QuestIds.LearnLeaveReturn) ||
			player.GetQuestStatus(QuestIds.LearnLeaveReturn) == QuestStatus.Incomplete)
			return false;

		return true;
	}
}

[Script]
internal class go_blood_filled_orb : GameObjectAI
{
	public go_blood_filled_orb(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		if (Me.GoType == GameObjectTypes.Goober)
			player.SummonCreature(CreatureIds.Zelemar, -369.746f, 166.759f, -21.50f, 5.235f, TempSummonType.TimedDespawnOutOfCombat, TimeSpan.FromSeconds(30));

		return true;
	}
}

[Script]
internal class go_soulwell : GameObjectAI
{
	public go_soulwell(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		var owner = Me.OwnerUnit;

		if (!owner ||
			!owner.IsTypeId(TypeId.Player) ||
			!player.IsInSameRaidWith(owner.AsPlayer))
			return true;

		return false;
	}
}

[Script]
internal class go_amberpine_outhouse : GameObjectAI
{
	public go_amberpine_outhouse(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		var status = player.GetQuestStatus(QuestIds.DoingYourDuty);

		if (status == QuestStatus.Incomplete ||
			status == QuestStatus.Complete ||
			status == QuestStatus.Rewarded)
		{
			player.AddGossipItem(GossipOptionNpc.None, GossipConst.GossipUseOuthouse, GossipSender.GOSSIP_SENDER_MAIN, GossipAction.GOSSIP_ACTION_INFO_DEF + 1);
			player.SendGossipMenu(GossipConst.GossipOuthouseVacant, Me.GUID);
		}
		else
		{
			player.SendGossipMenu(GossipConst.GossipOuthouseInuse, Me.GUID);
		}

		return true;
	}

	public override bool OnGossipSelect(Player player, uint menuId, uint gossipListId)
	{
		var action = player.PlayerTalkClass.GetGossipOptionAction(gossipListId);
		player.ClearGossipMenu();

		if (action == GossipAction.GOSSIP_ACTION_INFO_DEF + 1)
		{
			player.CloseGossipMenu();
			var target = ScriptedAI.GetClosestCreatureWithEntry(player, CreatureIds.OuthouseBunny, 3.0f);

			if (target)
			{
				target.AI.SetData(1, (uint)player.NativeGender);
				Me.CastSpell(target, SpellIds.IndisposedIii);
			}

			Me.CastSpell(player, SpellIds.Indisposed);

			if (player.HasItemCount(ItemIds.AnderholsSliderCider))
				Me.CastSpell(player, SpellIds.CreateAmberseeds);

			return true;
		}
		else
		{
			player.CloseGossipMenu();
			player.Session.SendNotification(GossipConst.AnderholsSliderCiderNotFound);

			return false;
		}
	}
}

[Script]
internal class go_massive_seaforium_charge : GameObjectAI
{
	public go_massive_seaforium_charge(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		Me.SetLootState(LootState.JustDeactivated);

		return true;
	}
}

[Script]
internal class go_veil_skith_cage : GameObjectAI
{
	public go_veil_skith_cage(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		Me.UseDoorOrButton();

		if (player.GetQuestStatus(QuestIds.MissingFriends) == QuestStatus.Incomplete)
		{
			var childrenList = Me.GetCreatureListWithEntryInGrid(CreatureIds.CaptiveChild, SharedConst.InteractionDistance);

			foreach (var creature in childrenList)
			{
				player.KilledMonsterCredit(CreatureIds.CaptiveChild, creature.GUID);
				creature.DespawnOrUnsummon(TimeSpan.FromSeconds(5));
				creature.MotionMaster.MovePoint(1, Me.Location.X + 5, Me.Location.Y, Me.Location.Z);
				creature.AI.Talk(TextIds.SayFree0);
				creature.MotionMaster.Clear();
			}
		}

		return false;
	}
}

[Script]
internal class go_midsummer_bonfire : GameObjectAI
{
	public go_midsummer_bonfire(GameObject go) : base(go) { }

	public override bool OnGossipSelect(Player player, uint menuId, uint ssipListId)
	{
		player.CastSpell(player, SpellIds.StampOutBonfireQuestComplete, true);
		player.CloseGossipMenu();

		return false;
	}
}

[Script]
internal class go_midsummer_ribbon_pole : GameObjectAI
{
	public go_midsummer_ribbon_pole(GameObject go) : base(go) { }

	public override bool OnGossipHello(Player player)
	{
		var creature = Me.FindNearestCreature(CreatureIds.PoleRibbonBunny, 10.0f);

		if (creature)
		{
			creature.AI.DoAction(0);
			player.CastSpell(player, SpellIds.RibbonPoleSpells[RandomHelper.IRand(0, 2)], true);
		}

		return true;
	}
}

[Script]
internal class go_brewfest_music : GameObjectAI
{
	private TimeSpan musicTime = TimeSpan.FromSeconds(1);
	private uint rnd = 0;

	public go_brewfest_music(GameObject go) : base(go)
	{
		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							task =>
							{
								if (Global.GameEventMgr.IsHolidayActive(HolidayIds.Brewfest)) // Check if Brewfest is active
								{
									rnd = RandomHelper.URand(0, 2); // Select random music sample
									task.Repeat(musicTime);         // Select new song music after play Time is over
								}
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(2),
							task =>
							{
								if (Global.GameEventMgr.IsHolidayActive(HolidayIds.Brewfest)) // Check if Brewfest is active
								{
									switch (Me.Area)
									{
										case AreaIds.Silvermoon:
										case AreaIds.Undercity:
										case AreaIds.Orgrimmar1:
										case AreaIds.Orgrimmar2:
										case AreaIds.Thunderbluff:
											switch (rnd)
											{
												case 0:
													Me.PlayDirectMusic(SoundIds.EventBrewfestgoblin01);
													musicTime = Misc.EventBrewfestgoblin01Time;

													break;
												case 1:
													Me.PlayDirectMusic(SoundIds.EventBrewfestgoblin02);
													musicTime = Misc.EventBrewfestgoblin02Time;

													break;
												default:
													Me.PlayDirectMusic(SoundIds.EventBrewfestgoblin03);
													musicTime = Misc.EventBrewfestgoblin03Time;

													break;
											}

											break;
										case AreaIds.Ironforge1:
										case AreaIds.Ironforge2:
										case AreaIds.Stormwind:
										case AreaIds.Exodar:
										case AreaIds.Darnassus:
											switch (rnd)
											{
												case 0:
													Me.PlayDirectMusic(SoundIds.EventBrewfestdwarf01);
													musicTime = Misc.EventBrewfestdwarf01Time;

													break;
												case 1:
													Me.PlayDirectMusic(SoundIds.EventBrewfestdwarf02);
													musicTime = Misc.EventBrewfestdwarf02Time;

													break;
												default:
													Me.PlayDirectMusic(SoundIds.EventBrewfestdwarf03);
													musicTime = Misc.EventBrewfestdwarf03Time;

													break;
											}

											break;
										case AreaIds.Shattrath:
											var playersNearby = Me.GetPlayerListInGrid(Me.VisibilityRange);

											foreach (Player player in playersNearby)
												if (player.TeamId == TeamIds.Horde)
													switch (rnd)
													{
														case 0:
															Me.PlayDirectMusic(SoundIds.EventBrewfestgoblin01);
															musicTime = Misc.EventBrewfestgoblin01Time;

															break;
														case 1:
															Me.PlayDirectMusic(SoundIds.EventBrewfestgoblin02);
															musicTime = Misc.EventBrewfestgoblin02Time;

															break;
														default:
															Me.PlayDirectMusic(SoundIds.EventBrewfestgoblin03);
															musicTime = Misc.EventBrewfestgoblin03Time;

															break;
													}
												else
													switch (rnd)
													{
														case 0:
															Me.PlayDirectMusic(SoundIds.EventBrewfestdwarf01);
															musicTime = Misc.EventBrewfestdwarf01Time;

															break;
														case 1:
															Me.PlayDirectMusic(SoundIds.EventBrewfestdwarf02);
															musicTime = Misc.EventBrewfestdwarf02Time;

															break;
														default:
															Me.PlayDirectMusic(SoundIds.EventBrewfestdwarf03);
															musicTime = Misc.EventBrewfestdwarf03Time;

															break;
													}

											break;
									}

									task.Repeat(TimeSpan.FromSeconds(5)); // Every 5 second's SMSG_PLAY_MUSIC packet (PlayDirectMusic) is pushed to the client
								}
							});
	}

	public override void UpdateAI(uint diff)
	{
		Scheduler.Update(diff);
	}
}

[Script]
internal class go_midsummer_music : GameObjectAI
{
	public go_midsummer_music(GameObject go) : base(go)
	{
		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							task =>
							{
								if (!Global.GameEventMgr.IsHolidayActive(HolidayIds.MidsummerFireFestival))
									return;

								var playersNearby = Me.GetPlayerListInGrid(Me.Map.VisibilityRange);

								foreach (Player player in playersNearby)
									if (player.Team == TeamFaction.Horde)
										Me.PlayDirectMusic(12325, player);
									else
										Me.PlayDirectMusic(12319, player);

								task.Repeat(TimeSpan.FromSeconds(5)); // Every 5 second's SMSG_PLAY_MUSIC packet (PlayDirectMusic) is pushed to the client (sniffed value)
							});
	}

	public override void UpdateAI(uint diff)
	{
		Scheduler.Update(diff);
	}
}

[Script]
internal class go_darkmoon_faire_music : GameObjectAI
{
	public go_darkmoon_faire_music(GameObject go) : base(go)
	{
		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							task =>
							{
								if (Global.GameEventMgr.IsHolidayActive(HolidayIds.DarkmoonFaire))
								{
									Me.PlayDirectMusic(8440);
									task.Repeat(TimeSpan.FromSeconds(5)); // Every 5 second's SMSG_PLAY_MUSIC packet (PlayDirectMusic) is pushed to the client (sniffed value)
								}
							});
	}

	public override void UpdateAI(uint diff)
	{
		Scheduler.Update(diff);
	}
}

[Script]
internal class go_pirate_day_music : GameObjectAI
{
	public go_pirate_day_music(GameObject go) : base(go)
	{
		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							task =>
							{
								if (!Global.GameEventMgr.IsHolidayActive(HolidayIds.PiratesDay))
									return;

								Me.PlayDirectMusic(12845);
								task.Repeat(TimeSpan.FromSeconds(5)); // Every 5 second's SMSG_PLAY_MUSIC packet (PlayDirectMusic) is pushed to the client (sniffed value)
							});
	}

	public override void UpdateAI(uint diff)
	{
		Scheduler.Update(diff);
	}
}

[Script]
internal class go_bells : GameObjectAI
{
	private uint _soundId;

	public go_bells(GameObject go) : base(go) { }

	public override void InitializeAI()
	{
		var zoneId = Me.Zone;

		switch (Me.Entry)
		{
			case GameObjectIds.HordeBell:
			{
				switch (zoneId)
				{
					case ZoneIds.Tirisfal:
					case ZoneIds.Undercity:
					case ZoneIds.HillsbradFoothills:
					case ZoneIds.Duskwood:
						_soundId = SoundIds.BellTollHorde; // undead bell sound

						break;
					default:
						_soundId = SoundIds.BellTollTribal; // orc drum sound 

						break;
				}

				break;
			}
			case GameObjectIds.AllianceBell:
			{
				switch (zoneId)
				{
					case ZoneIds.Ironforge:
					case ZoneIds.DunMorogh:
						_soundId = SoundIds.BellTolldwarfgnome; // horn sound

						break;
					case ZoneIds.Darnassus:
					case ZoneIds.Teldrassil:
					case ZoneIds.Ashenvale:
						_soundId = SoundIds.BellTollNightelf; // nightelf bell sound

						break;
					default:
						_soundId = SoundIds.BellTollAlliance; // human bell sound

						break;
				}

				break;
			}
			case GameObjectIds.KharazhanBell:
				_soundId = SoundIds.BellTollKharazhan;

				break;
		}
	}

	public override void OnGameEvent(bool start, ushort eventId)
	{
		if (eventId == Misc.GameEventHourlyBells && start)
		{
			var localTm = Time.UnixTimeToDateTime(GameTime.GetGameTime()).ToLocalTime();
			var _rings = localTm.Hour % 12;

			if (_rings == 0) // 00:00 and 12:00
				_rings = 12;

			// Dwarf hourly horn should only play a single Time, each Time the next hour begins.
			if (_soundId == SoundIds.BellTolldwarfgnome)
				_rings = 1;

			for (var i = 0; i < _rings; ++i)
				Scheduler.Schedule(TimeSpan.FromSeconds(i * 4 + 1), task => Me.PlayDirectSound(_soundId));
		}
	}

	public override void UpdateAI(uint diff)
	{
		Scheduler.Update(diff);
	}
}