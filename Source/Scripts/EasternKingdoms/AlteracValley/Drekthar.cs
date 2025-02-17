// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.EasternKingdoms.AlteracValley.Drekthar;

internal struct SpellIds
{
	public const uint Whirlwind = 15589;
	public const uint Whirlwind2 = 13736;
	public const uint Knockdown = 19128;
	public const uint Frenzy = 8269;
	public const uint SweepingStrikes = 18765; // not sure
	public const uint Cleave = 20677;          // not sure
	public const uint Windfury = 35886;        // not sure
	public const uint Stormpike = 51876;       // not sure
}

internal struct TextIds
{
	public const uint SayAggro = 0;
	public const uint SayEvade = 1;
	public const uint SayRespawn = 2;
	public const uint SayRandom = 3;
}

[Script]
internal class boss_drekthar : ScriptedAI
{
	public boss_drekthar(Creature creature) : base(creature) { }

	public override void Reset()
	{
		Scheduler.CancelAll();
	}

	public override void JustEngagedWith(Unit who)
	{
		Talk(TextIds.SayAggro);

		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							TimeSpan.FromSeconds(20),
							task =>
							{
								DoCastVictim(SpellIds.Whirlwind);
								task.Repeat(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(18));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							TimeSpan.FromSeconds(20),
							task =>
							{
								DoCastVictim(SpellIds.Whirlwind2);
								task.Repeat(TimeSpan.FromSeconds(7), TimeSpan.FromSeconds(25));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(12),
							task =>
							{
								DoCastVictim(SpellIds.Knockdown);
								task.Repeat(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(6),
							task =>
							{
								DoCastVictim(SpellIds.Frenzy);
								task.Repeat(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(30));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(20),
							TimeSpan.FromSeconds(30),
							task =>
							{
								Talk(TextIds.SayRandom);
								task.Repeat();
							});
	}

	public override void JustAppeared()
	{
		Reset();
		Talk(TextIds.SayRespawn);
	}

	public override bool CheckInRoom()
	{
		if (Me.GetDistance2d(Me.HomePosition.X, Me.HomePosition.Y) > 50)
		{
			EnterEvadeMode();
			Talk(TextIds.SayEvade);

			return false;
		}

		return true;
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim() ||
			!CheckInRoom())
			return;

		Scheduler.Update(diff, () => DoMeleeAttackIfReady());
	}
}