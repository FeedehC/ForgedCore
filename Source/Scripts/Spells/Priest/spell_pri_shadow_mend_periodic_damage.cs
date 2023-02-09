﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Priest;

[Script] // 187464 - Shadow Mend (Damage)
internal class spell_pri_shadow_mend_periodic_damage : AuraScript, IAuraCheckProc, IHasAuraEffects
{
	public override bool Validate(SpellInfo spellInfo)
	{
		return ValidateSpellInfo(PriestSpells.ShadowMendDamage);
	}

	public bool CheckProc(ProcEventInfo eventInfo)
	{
		return eventInfo.GetDamageInfo() != null;
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(HandleDummyTick, 0, AuraType.PeriodicDummy));
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 1, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	private void HandleDummyTick(AuraEffect aurEff)
	{
		CastSpellExtraArgs args = new(TriggerCastFlags.FullMask);
		args.SetOriginalCaster(GetCasterGUID());
		args.SetTriggeringAura(aurEff);
		args.AddSpellMod(SpellValueMod.BasePoint0, aurEff.GetAmount());
		GetTarget().CastSpell(GetTarget(), PriestSpells.ShadowMendDamage, args);
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		var newAmount = (int)(aurEff.GetAmount() - eventInfo.GetDamageInfo().GetDamage());

		aurEff.ChangeAmount(newAmount);

		if (newAmount < 0)
			Remove();
	}
}