﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Monk;

[SpellScript(210802)]
public class spell_monk_spirit_of_the_crane_passive : AuraScript, IHasAuraEffects, IAuraCheckProc
{
	public List<IAuraEffectHandler> AuraEffects => new List<IAuraEffectHandler>();

	public override bool Validate(SpellInfo UnnamedParameter)
	{
		return ValidateSpellInfo(MonkSpells.SPELL_MONK_SPIRIT_OF_THE_CRANE_MANA, MonkSpells.SPELL_MONK_BLACKOUT_KICK_TRIGGERED);
	}

	public bool CheckProc(ProcEventInfo eventInfo)
	{
		if (eventInfo.GetSpellInfo().Id != MonkSpells.SPELL_MONK_BLACKOUT_KICK_TRIGGERED)
		{
			return false;
		}
		return true;
	}

	private void HandleProc(AuraEffect UnnamedParameter, ProcEventInfo UnnamedParameter2)
	{
		// TODO: Basepoints can be float now... this is 1 but needs to be lower.
		GetTarget().CastSpell(GetTarget(), MonkSpells.SPELL_MONK_SPIRIT_OF_THE_CRANE_MANA, true);
	}

	public override void Register()
	{

		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}
}