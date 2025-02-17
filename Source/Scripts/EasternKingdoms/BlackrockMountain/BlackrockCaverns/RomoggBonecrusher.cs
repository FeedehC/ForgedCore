// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;
using Game.Spells;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockCaverns.RomoggBonecrusher;

internal struct SpellIds
{
	public const uint CallForHelp = 82137; // Needs Scripting
	public const uint ChainsOfWoe = 75539;
	public const uint Quake = 75272;
	public const uint Skullcracker = 75543;
	public const uint WoundingStrike = 75571;
}

internal struct TextIds
{
	public const uint YellAggro = 0;
	public const uint YellKill = 1;
	public const uint YellSkullcracker = 2;
	public const uint YellDeath = 3;

	public const uint EmoteCallForHelp = 4;
	public const uint EmoteSkullcracker = 5;
}

internal struct MiscConst
{
	public const uint TypeRaz = 1;
	public const uint DataRomoggDead = 1;
	public static Position SummonPos = new(249.2639f, 949.1614f, 191.7866f, 3.141593f);
}

[Script]
internal class boss_romogg_bonecrusher : BossAI
{
	public boss_romogg_bonecrusher(Creature creature) : base(creature, DataTypes.RomoggBonecrusher)
	{
		Me.SummonCreature(CreatureIds.RazTheCrazed, MiscConst.SummonPos, TempSummonType.ManualDespawn, TimeSpan.FromSeconds(200));
	}

	public override void Reset()
	{
		_Reset();
	}

	public override void JustDied(Unit killer)
	{
		_JustDied();
		Talk(TextIds.YellDeath);

		var raz = Instance.GetCreature(DataTypes.RazTheCrazed);

		if (raz)
			raz.AI.SetData(MiscConst.TypeRaz, MiscConst.DataRomoggDead);
	}

	public override void KilledUnit(Unit who)
	{
		if (who.IsPlayer)
			Talk(TextIds.YellKill);
	}

	public override void JustEngagedWith(Unit who)
	{
		base.JustEngagedWith(who);

		Scheduler.Schedule(TimeSpan.FromSeconds(22),
							TimeSpan.FromSeconds(32),
							task =>
							{
								Talk(TextIds.YellSkullcracker);
								DoCast(Me, SpellIds.ChainsOfWoe);
								task.Repeat(TimeSpan.FromSeconds(22), TimeSpan.FromSeconds(32));

								Scheduler.Schedule(TimeSpan.FromSeconds(3),
													skullCrackerTask =>
													{
														Talk(TextIds.EmoteSkullcracker);
														DoCast(Me, SpellIds.Skullcracker);
													});
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(26),
							TimeSpan.FromSeconds(32),
							task =>
							{
								DoCastVictim(SpellIds.WoundingStrike, new CastSpellExtraArgs(true));
								task.Repeat(TimeSpan.FromSeconds(26), TimeSpan.FromSeconds(32));
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(45),
							task =>
							{
								DoCast(Me, SpellIds.Quake);
								task.Repeat(TimeSpan.FromSeconds(32), TimeSpan.FromSeconds(40));
							});

		Talk(TextIds.YellAggro);
		Talk(TextIds.EmoteCallForHelp);
		DoCast(Me, SpellIds.CallForHelp);
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim())
			return;

		Scheduler.Update(diff, () => DoMeleeAttackIfReady());
	}
}