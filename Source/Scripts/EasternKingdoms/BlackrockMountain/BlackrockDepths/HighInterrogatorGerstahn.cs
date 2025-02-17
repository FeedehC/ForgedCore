// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockDepths.HighInterrogatorGerstahn;

internal struct SpellIds
{
	public const uint Shadowwordpain = 10894;
	public const uint Manaburn = 10876;
	public const uint Psychicscream = 8122;
	public const uint Shadowshield = 22417;
}

[Script]
internal class boss_high_interrogator_gerstahn : ScriptedAI
{
	public boss_high_interrogator_gerstahn(Creature creature) : base(creature) { }

	public override void Reset()
	{
		Scheduler.CancelAll();
	}

	public override void JustEngagedWith(Unit who)
	{
		Scheduler.Schedule(TimeSpan.FromSeconds(4),
							task =>
							{
								var target = SelectTarget(SelectTargetMethod.Random, 0, 100.0f, true);

								if (target)
									DoCast(target, SpellIds.Shadowwordpain);

								task.Repeat(TimeSpan.FromSeconds(7));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(14),
							task =>
							{
								var target = SelectTarget(SelectTargetMethod.Random, 0, 100.0f, true);

								if (target)
									DoCast(target, SpellIds.Manaburn);

								task.Repeat(TimeSpan.FromSeconds(10));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(32),
							task =>
							{
								DoCastVictim(SpellIds.Psychicscream);
								task.Repeat(TimeSpan.FromSeconds(30));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(8),
							task =>
							{
								DoCast(Me, SpellIds.Shadowshield);
								task.Repeat(TimeSpan.FromSeconds(25));
							});
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim())
			return;

		Scheduler.Update(diff, () => DoMeleeAttackIfReady());
	}
}