﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Monk;

[SpellScript(MonkSpells.SPELL_MONK_FISTS_OF_FURY)]
public class spell_monk_fists_of_fury_visual : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects => new();

	private void OnApply(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		SetMaxDuration(1000); //The spell doesn't have a duration on WoWHead and never ends if we don't give it one, so one sec should be good
		SetDuration(1000);    //Same as above
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(OnApply, 0, AuraType.Dummy, AuraEffectHandleModes.Real));
	}
}