﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Priest;

[SpellScript(32379)]
public class spell_pri_shadow_word_death : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDamage, 0, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDamage(int effIndex)
	{
		var target = HitUnit;

		if (target != null)
		{
			if (target.Health < HitDamage)
				Caster.CastSpell(Caster, PriestSpells.SHADOW_WORD_DEATH_ENERGIZE_KILL, true);
			else
				Caster.ModifyPower(PowerType.Insanity, SpellInfo.GetEffect(2).BasePoints);
		}
	}
}