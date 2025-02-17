// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockSpire.WarmasterVoone;

internal struct SpellIds
{
	public const uint Snapkick = 15618;
	public const uint Cleave = 15284;
	public const uint Uppercut = 10966;
	public const uint Mortalstrike = 16856;
	public const uint Pummel = 15615;
	public const uint Throwaxe = 16075;
}

[Script]
internal class boss_warmaster_voone : BossAI
{
	public boss_warmaster_voone(Creature creature) : base(creature, DataTypes.WarmasterVoone) { }

	public override void Reset()
	{
		_Reset();
	}

	public override void JustEngagedWith(Unit who)
	{
		base.JustEngagedWith(who);

		Scheduler.Schedule(TimeSpan.FromSeconds(8),
							task =>
							{
								DoCastVictim(SpellIds.Snapkick);
								task.Repeat(TimeSpan.FromSeconds(6));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(14),
							task =>
							{
								DoCastVictim(SpellIds.Cleave);
								task.Repeat(TimeSpan.FromSeconds(12));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(20),
							task =>
							{
								DoCastVictim(SpellIds.Uppercut);
								task.Repeat(TimeSpan.FromSeconds(14));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(12),
							task =>
							{
								DoCastVictim(SpellIds.Mortalstrike);
								task.Repeat(TimeSpan.FromSeconds(10));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(32),
							task =>
							{
								DoCastVictim(SpellIds.Pummel);
								task.Repeat(TimeSpan.FromSeconds(16));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							task =>
							{
								DoCastVictim(SpellIds.Throwaxe);
								task.Repeat(TimeSpan.FromSeconds(8));
							});
	}

	public override void JustDied(Unit killer)
	{
		_JustDied();
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim())
			return;

		Scheduler.Update(diff, () => DoMeleeAttackIfReady());
	}
}