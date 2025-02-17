// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockSpire.GizrulTheSlavener;

internal struct SpellIds
{
	public const uint FatalBite = 16495;
	public const uint InfectedBite = 16128;
	public const uint Frenzy = 8269;
}

internal struct PathIds
{
	public const uint Gizrul = 402450;
}

[Script]
internal class boss_gizrul_the_slavener : BossAI
{
	public boss_gizrul_the_slavener(Creature creature) : base(creature, DataTypes.GizrulTheSlavener) { }

	public override void Reset()
	{
		_Reset();
	}

	public override void IsSummonedBy(WorldObject summoner)
	{
		Me.MotionMaster.MovePath(PathIds.Gizrul, false);
	}

	public override void JustEngagedWith(Unit who)
	{
		base.JustEngagedWith(who);

		Scheduler.Schedule(TimeSpan.FromSeconds(17),
							TimeSpan.FromSeconds(20),
							task =>
							{
								DoCastVictim(SpellIds.FatalBite);
								task.Repeat(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(10));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(10),
							TimeSpan.FromSeconds(12),
							task =>
							{
								DoCast(Me, SpellIds.InfectedBite);
								task.Repeat(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(10));
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