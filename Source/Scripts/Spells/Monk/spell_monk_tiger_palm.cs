﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Monk;

[SpellScript(100780)]
public class spell_monk_tiger_palm : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects => new();

	private void HandleHit(uint UnnamedParameter)
	{
		var powerStrikes = GetCaster().GetAura(MonkSpells.SPELL_MONK_POWER_STRIKES_AURA);

		if (powerStrikes != null)
		{
			SetEffectValue(GetEffectValue() + powerStrikes.GetEffect(0).GetBaseAmount());
			powerStrikes.Remove();
		}
	}

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleHit, 1, SpellEffectName.Energize, SpellScriptHookType.EffectHitTarget));
	}
}