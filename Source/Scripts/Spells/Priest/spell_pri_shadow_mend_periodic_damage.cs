﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Priest;

[Script] // 187464 - Shadow Mend (Damage)
internal class spell_pri_shadow_mend_periodic_damage : AuraScript, IAuraCheckProc, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public bool CheckProc(ProcEventInfo eventInfo)
	{
		return eventInfo.DamageInfo != null;
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(HandleDummyTick, 0, AuraType.PeriodicDummy));
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 1, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	private void HandleDummyTick(AuraEffect aurEff)
	{
		CastSpellExtraArgs args = new(TriggerCastFlags.FullMask);
		args.SetOriginalCaster(CasterGUID);
		args.SetTriggeringAura(aurEff);
		args.AddSpellMod(SpellValueMod.BasePoint0, aurEff.Amount);
		Target.CastSpell(Target, PriestSpells.SHADOW_MEND_DAMAGE, args);
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		var newAmount = (int)(aurEff.Amount - eventInfo.DamageInfo.Damage);

		aurEff.ChangeAmount(newAmount);

		if (newAmount < 0)
			Remove();
	}
}