﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Hunter;

[SpellScript(257044)]
public class spell_hun_rapid_fire : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects => new List<IAuraEffectHandler>();

	private void OnTick(AuraEffect aurEff)
	{
		Unit target = GetTarget();
		if (target != null)
		{
			if (GetCaster())
			{
				GetCaster().CastSpell(target, HunterSpells.SPELL_HUNTER_RAPID_FIRE_MISSILE, true);
				if (GetCaster().GetPowerPct(PowerType.Focus) != 100)
				{
					GetCaster().ModifyPower(PowerType.Focus, +1);
				}
			}
		}
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(OnTick, 1, AuraType.PeriodicDummy));
	}
}