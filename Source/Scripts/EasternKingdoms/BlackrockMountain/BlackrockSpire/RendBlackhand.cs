// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockSpire.RendBlackhand;

internal struct SpellIds
{
	public const uint Whirlwind = 13736; // sniffed
	public const uint Cleave = 15284;
	public const uint MortalStrike = 16856;
	public const uint Frenzy = 8269;
	public const uint Knockdown = 13360; // On spawn during Gyth fight
}

internal struct TextIds
{
	// Rend Blackhand
	public const uint SayBlackhand1 = 0;
	public const uint SayBlackhand2 = 1;

	public const uint EmoteBlackhandDismount = 2;

	// Victor Nefarius
	public const uint SayNefarius0 = 0;
	public const uint SayNefarius1 = 1;
	public const uint SayNefarius2 = 2;
	public const uint SayNefarius3 = 3;
	public const uint SayNefarius4 = 4;
	public const uint SayNefarius5 = 5;
	public const uint SayNefarius6 = 6;
	public const uint SayNefarius7 = 7;
	public const uint SayNefarius8 = 8;
	public const uint SayNefarius9 = 9;
}

internal struct AddIds
{
	public const uint ChromaticWhelp = 10442;
	public const uint ChromaticDragonspawn = 10447;
	public const uint BlackhandDragonHandler = 10742;
}

internal struct MiscConst
{
	public const uint NefariusPath1 = 1379670;
	public const uint NefariusPath2 = 1379671;
	public const uint NefariusPath3 = 1379672;
	public const uint RendPath1 = 1379680;
	public const uint RendPath2 = 1379681;

	public static Wave[] Wave2 = // 22 sec
	{
		new(10447, 209.8637f, -428.2729f, 110.9877f, 0.6632251f), new(10442, 209.3122f, -430.8724f, 110.9814f, 2.9147f), new(10442, 211.3309f, -425.9111f, 111.0006f, 1.727876f)
	};

	public static Wave[] Wave3 = // 60 sec
	{
		new(10742, 208.6493f, -424.5787f, 110.9872f, 5.8294f), new(10447, 203.9482f, -428.9446f, 110.982f, 4.677482f), new(10442, 203.3441f, -426.8668f, 110.9772f, 4.712389f), new(10442, 206.3079f, -424.7509f, 110.9943f, 4.08407f)
	};

	public static Wave[] Wave4 = // 49 sec
	{
		new(10742, 212.3541f, -412.6826f, 111.0352f, 5.88176f), new(10447, 212.5754f, -410.2841f, 111.0296f, 2.740167f), new(10442, 212.3449f, -414.8659f, 111.0348f, 2.356194f), new(10442, 210.6568f, -412.1552f, 111.0124f, 0.9773844f)
	};

	public static Wave[] Wave5 = // 60 sec
	{
		new(10742, 210.2188f, -410.6686f, 111.0211f, 5.8294f), new(10447, 209.4078f, -414.13f, 111.0264f, 4.677482f), new(10442, 208.0858f, -409.3145f, 111.0118f, 4.642576f), new(10442, 207.9811f, -413.0728f, 111.0098f, 5.288348f), new(10442, 208.0854f, -412.1505f, 111.0057f, 4.08407f)
	};

	public static Wave[] Wave6 = // 27 sec
	{
		new(10742, 213.9138f, -426.512f, 111.0013f, 3.316126f), new(10447, 213.7121f, -429.8102f, 110.9888f, 1.413717f), new(10447, 213.7157f, -424.4268f, 111.009f, 3.001966f), new(10442, 210.8935f, -423.913f, 111.0125f, 5.969026f), new(10442, 212.2642f, -430.7648f, 110.9807f, 5.934119f)
	};

	public static Position GythLoc = new(211.762f, -397.5885f, 111.1817f, 4.747295f);
	public static Position Teleport1Loc = new(194.2993f, -474.0814f, 121.4505f, -0.01225555f);
	public static Position Teleport2Loc = new(216.485f, -434.93f, 110.888f, -0.01225555f);
}

internal class Wave
{
	public uint entry;
	public double o_pos;
	public double x_pos;
	public double y_pos;
	public double z_pos;

	public Wave(uint _entry, float x, float y, float z, float o)
	{
		entry = _entry;
		x_pos = x;
		y_pos = y;
		z_pos = z;
		o_pos = o;
	}
}

internal struct EventIds
{
	public const uint Start1 = 1;
	public const uint Start2 = 2;
	public const uint Start3 = 3;
	public const uint Start4 = 4;
	public const uint TurnToRend = 5;
	public const uint TurnToPlayer = 6;
	public const uint TurnToFacing1 = 7;
	public const uint TurnToFacing2 = 8;
	public const uint TurnToFacing3 = 9;
	public const uint Wave1 = 10;
	public const uint Wave2 = 11;
	public const uint Wave3 = 12;
	public const uint Wave4 = 13;
	public const uint Wave5 = 14;
	public const uint Wave6 = 15;
	public const uint WavesText1 = 16;
	public const uint WavesText2 = 17;
	public const uint WavesText3 = 18;
	public const uint WavesText4 = 19;
	public const uint WavesText5 = 20;
	public const uint WavesCompleteText1 = 21;
	public const uint WavesCompleteText2 = 22;
	public const uint WavesCompleteText3 = 23;
	public const uint WavesEmote1 = 24;
	public const uint WavesEmote2 = 25;
	public const uint PathRend = 26;
	public const uint PathNefarius = 27;
	public const uint Teleport1 = 28;
	public const uint Teleport2 = 29;
	public const uint Whirlwind = 30;
	public const uint Cleave = 31;
	public const uint MortalStrike = 32;
}

[Script]
internal class boss_rend_blackhand : BossAI
{
	private bool gythEvent;
	private ObjectGuid portcullisGUID;
	private ObjectGuid victorGUID;

	public boss_rend_blackhand(Creature creature) : base(creature, DataTypes.WarchiefRendBlackhand) { }

	public override void Reset()
	{
		_Reset();
		gythEvent = false;
		victorGUID.Clear();
		portcullisGUID.Clear();
	}

	public override void JustEngagedWith(Unit who)
	{
		base.JustEngagedWith(who);
		Events.ScheduleEvent(EventIds.Whirlwind, TimeSpan.FromSeconds(13), TimeSpan.FromSeconds(15));
		Events.ScheduleEvent(EventIds.Cleave, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(17));
		Events.ScheduleEvent(EventIds.MortalStrike, TimeSpan.FromSeconds(17), TimeSpan.FromSeconds(19));
	}

	public override void IsSummonedBy(WorldObject summoner)
	{
		Me.SetImmuneToPC(false);
		DoZoneInCombat();
	}

	public override void JustDied(Unit killer)
	{
		_JustDied();
		var victor = Me.FindNearestCreature(CreaturesIds.LordVictorNefarius, 75.0f, true);

		victor?.AI.SetData(1, 2);
	}

	public override void SetData(uint type, uint data)
	{
		if (type == BRSMiscConst.Areatrigger &&
			data == BRSMiscConst.AreatriggerBlackrockStadium)
			if (!gythEvent)
			{
				gythEvent = true;

				var victor = Me.FindNearestCreature(CreaturesIds.LordVictorNefarius, 5.0f, true);

				if (victor != null)
					victorGUID = victor.GUID;

				var portcullis = Me.FindNearestGameObject(GameObjectsIds.DrPortcullis, 50.0f);

				if (portcullis != null)
					portcullisGUID = portcullis.GUID;

				Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
				Events.ScheduleEvent(EventIds.Start1, TimeSpan.FromSeconds(1));
			}
	}

	public override void MovementInform(MovementGeneratorType type, uint id)
	{
		if (type == MovementGeneratorType.Waypoint)
			switch (id)
			{
				case 5:
					Events.ScheduleEvent(EventIds.Teleport1, TimeSpan.FromSeconds(2));

					break;
				case 11:
					var gyth = Me.FindNearestCreature(CreaturesIds.Gyth, 10.0f, true);

					if (gyth)
						gyth.AI.SetData(1, 1);

					Me.DespawnOrUnsummon(TimeSpan.FromSeconds(1), TimeSpan.FromDays(7));

					break;
			}
	}

	public override void UpdateAI(uint diff)
	{
		if (gythEvent)
		{
			Events.Update(diff);

			Events.ExecuteEvents(eventId =>
			{
				switch (eventId)
				{
					case EventIds.Start1:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius0);

						Events.ScheduleEvent(EventIds.Start2, TimeSpan.FromSeconds(4));

						break;
					}
					case EventIds.Start2:
					{
						Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.HandleEmoteCommand(Emote.OneshotPoint);

						Events.ScheduleEvent(EventIds.Start3, TimeSpan.FromSeconds(4));

						break;
					}
					case EventIds.Start3:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius1);

						Events.ScheduleEvent(EventIds.Wave1, TimeSpan.FromSeconds(2));
						Events.ScheduleEvent(EventIds.TurnToRend, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.WavesText1, TimeSpan.FromSeconds(20));

						break;
					}
					case EventIds.TurnToRend:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						if (victor != null)
						{
							victor.SetFacingToObject(Me);
							victor.HandleEmoteCommand(Emote.OneshotTalk);
						}

						break;
					}
					case EventIds.TurnToPlayer:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						if (victor != null)
						{
							Unit player = victor.SelectNearestPlayer(60.0f);

							if (player != null)
								victor.SetFacingToObject(player);
						}

						break;
					}
					case EventIds.TurnToFacing1:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.SetFacingTo(1.518436f);

						break;
					}
					case EventIds.TurnToFacing2:
						Me.SetFacingTo(1.658063f);

						break;
					case EventIds.TurnToFacing3:
						Me.SetFacingTo(1.500983f);

						break;
					case EventIds.WavesEmote1:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.HandleEmoteCommand(Emote.OneshotQuestion);

						break;
					}
					case EventIds.WavesEmote2:
						Me.HandleEmoteCommand(Emote.OneshotRoar);

						break;
					case EventIds.WavesText1:
					{
						Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius2);

						Me.HandleEmoteCommand(Emote.OneshotTalk);
						Events.ScheduleEvent(EventIds.TurnToFacing1, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.WavesEmote1, TimeSpan.FromSeconds(5));
						Events.ScheduleEvent(EventIds.Wave2, TimeSpan.FromSeconds(2));
						Events.ScheduleEvent(EventIds.WavesText2, TimeSpan.FromSeconds(20));

						break;
					}
					case EventIds.WavesText2:
					{
						Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius3);

						Events.ScheduleEvent(EventIds.TurnToFacing1, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.Wave3, TimeSpan.FromSeconds(2));
						Events.ScheduleEvent(EventIds.WavesText3, TimeSpan.FromSeconds(20));

						break;
					}
					case EventIds.WavesText3:
					{
						Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius4);

						Events.ScheduleEvent(EventIds.TurnToFacing1, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.Wave4, TimeSpan.FromSeconds(2));
						Events.ScheduleEvent(EventIds.WavesText4, TimeSpan.FromSeconds(20));

						break;
					}
					case EventIds.WavesText4:
						Talk(TextIds.SayBlackhand1);
						Events.ScheduleEvent(EventIds.WavesEmote2, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.TurnToFacing3, TimeSpan.FromSeconds(8));
						Events.ScheduleEvent(EventIds.Wave5, TimeSpan.FromSeconds(2));
						Events.ScheduleEvent(EventIds.WavesText5, TimeSpan.FromSeconds(20));

						break;
					case EventIds.WavesText5:
					{
						Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius5);

						Events.ScheduleEvent(EventIds.TurnToFacing1, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.Wave6, TimeSpan.FromSeconds(2));
						Events.ScheduleEvent(EventIds.WavesCompleteText1, TimeSpan.FromSeconds(20));

						break;
					}
					case EventIds.WavesCompleteText1:
					{
						Events.ScheduleEvent(EventIds.TurnToPlayer, TimeSpan.FromSeconds(0));
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius6);

						Events.ScheduleEvent(EventIds.TurnToFacing1, TimeSpan.FromSeconds(4));
						Events.ScheduleEvent(EventIds.WavesCompleteText2, TimeSpan.FromSeconds(13));

						break;
					}
					case EventIds.WavesCompleteText2:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius7);

						Talk(TextIds.SayBlackhand2);
						Events.ScheduleEvent(EventIds.PathRend, TimeSpan.FromSeconds(1));
						Events.ScheduleEvent(EventIds.WavesCompleteText3, TimeSpan.FromSeconds(4));

						break;
					}
					case EventIds.WavesCompleteText3:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.AI.Talk(TextIds.SayNefarius8);

						Events.ScheduleEvent(EventIds.PathNefarius, TimeSpan.FromSeconds(1));
						Events.ScheduleEvent(EventIds.PathRend, TimeSpan.FromSeconds(1));

						break;
					}
					case EventIds.PathNefarius:
					{
						var victor = ObjectAccessor.GetCreature(Me, victorGUID);

						victor?.MotionMaster.MovePath(MiscConst.NefariusPath1, true);

						break;
					}
					case EventIds.PathRend:
						Me.MotionMaster.MovePath(MiscConst.RendPath1, false);

						break;
					case EventIds.Teleport1:
						Me.NearTeleportTo(194.2993f, -474.0814f, 121.4505f, -0.01225555f);
						Events.ScheduleEvent(EventIds.Teleport2, TimeSpan.FromSeconds(50));

						break;
					case EventIds.Teleport2:
						Me.NearTeleportTo(216.485f, -434.93f, 110.888f, -0.01225555f);
						Me.SummonCreature(CreaturesIds.Gyth, 211.762f, -397.5885f, 111.1817f, 4.747295f);

						break;
					case EventIds.Wave1:
					{
						var portcullis = ObjectAccessor.GetGameObject(Me, portcullisGUID);

						portcullis?.UseDoorOrButton();

						// move wave
						break;
					}
					case EventIds.Wave2:
					{
						// spawn wave
						var portcullis = ObjectAccessor.GetGameObject(Me, portcullisGUID);

						portcullis?.UseDoorOrButton();

						// move wave
						break;
					}
					case EventIds.Wave3:
					{
						// spawn wave
						var portcullis = ObjectAccessor.GetGameObject(Me, portcullisGUID);

						portcullis?.UseDoorOrButton();

						// move wave
						break;
					}
					case EventIds.Wave4:
					{
						// spawn wave
						var portcullis = ObjectAccessor.GetGameObject(Me, portcullisGUID);

						portcullis?.UseDoorOrButton();

						// move wave
						break;
					}
					case EventIds.Wave5:
					{
						// spawn wave
						var portcullis = ObjectAccessor.GetGameObject(Me, portcullisGUID);

						portcullis?.UseDoorOrButton();

						// move wave
						break;
					}
					case EventIds.Wave6:
					{
						// spawn wave
						var portcullis = ObjectAccessor.GetGameObject(Me, portcullisGUID);

						portcullis?.UseDoorOrButton();

						// move wave
						break;
					}
					default:
						break;
				}
			});
		}

		if (!UpdateVictim())
			return;

		Events.Update(diff);

		if (Me.HasUnitState(UnitState.Casting))
			return;

		Events.ExecuteEvents(eventId =>
		{
			switch (eventId)
			{
				case EventIds.Whirlwind:
					DoCast(SpellIds.Whirlwind);
					Events.ScheduleEvent(EventIds.Whirlwind, TimeSpan.FromSeconds(13), TimeSpan.FromSeconds(18));

					break;
				case EventIds.Cleave:
					DoCastVictim(SpellIds.Cleave);
					Events.ScheduleEvent(EventIds.Cleave, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(14));

					break;
				case EventIds.MortalStrike:
					DoCastVictim(SpellIds.MortalStrike);
					Events.ScheduleEvent(EventIds.MortalStrike, TimeSpan.FromSeconds(14), TimeSpan.FromSeconds(16));

					break;
			}

			if (Me.HasUnitState(UnitState.Casting))
				return;
		});

		DoMeleeAttackIfReady();
	}
}