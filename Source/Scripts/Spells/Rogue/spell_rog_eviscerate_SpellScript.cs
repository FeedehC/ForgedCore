﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Rogue;

[Script] // 196819 - Eviscerate
internal class spell_rog_eviscerate_SpellScript : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(CalculateDamage, 0, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
	}

	private void CalculateDamage(int effIndex)
	{
		var damagePerCombo = HitDamage;
		var t5 = Caster.GetAuraEffect(RogueSpells.T52pSetBonus, 0);

		if (t5 != null)
			damagePerCombo += t5.Amount;

		var finalDamage = damagePerCombo;
		var costs = Spell.PowerCost;
		var c = costs.Find(cost => cost.Power == PowerType.ComboPoints);

		if (c != null)
			finalDamage *= c.Amount;

		HitDamage = finalDamage;
	}
}