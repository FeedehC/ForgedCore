﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Monk;

[Script] // 124255 - Stagger - SPELL_MONK_STAGGER_DAMAGE_AURA
internal class spell_monk_stagger_damage_aura : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override bool Validate(SpellInfo spellInfo)
	{
		return ValidateSpellInfo(MonkSpells.StaggerLight, MonkSpells.StaggerModerate, MonkSpells.StaggerHeavy);
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(OnPeriodicDamage, 0, AuraType.PeriodicDamage));
	}

	private void OnPeriodicDamage(AuraEffect aurEff)
	{
		// Update our light/medium/heavy stagger with the correct stagger amount left
		var auraStagger = spell_monk_stagger.FindExistingStaggerEffect(GetTarget());

		if (auraStagger != null)
		{
			var auraEff = auraStagger.GetEffect(1);

			if (auraEff != null)
			{
				float total      = auraEff.GetAmount();
				float tickDamage = aurEff.GetAmount();
				auraEff.ChangeAmount((int)(total - tickDamage));
			}
		}
	}
}