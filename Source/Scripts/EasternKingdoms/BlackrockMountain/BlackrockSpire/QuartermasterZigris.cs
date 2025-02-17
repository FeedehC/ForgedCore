// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockSpire.QuartermasterZigris;

internal struct SpellIds
{
	public const uint Shoot = 16496;
	public const uint Stunbomb = 16497;
	public const uint HealingPotion = 15504;
	public const uint Hookednet = 15609;
}

[Script]
internal class quartermaster_zigris : BossAI
{
	public quartermaster_zigris(Creature creature) : base(creature, DataTypes.QuartermasterZigris) { }

	public override void Reset()
	{
		_Reset();
	}

	public override void JustEngagedWith(Unit who)
	{
		base.JustEngagedWith(who);

		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							task =>
							{
								DoCastVictim(SpellIds.Shoot);
								task.Repeat(TimeSpan.FromMilliseconds(500));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(16),
							task =>
							{
								DoCastVictim(SpellIds.Stunbomb);
								task.Repeat(TimeSpan.FromSeconds(14));
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