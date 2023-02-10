﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.DemonHunter;

[SpellScript(206478)]
public class spell_dh_demonic_appetite : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects => new();

	private void OnProc(AuraEffect UnnamedParameter, ProcEventInfo UnnamedParameter2)
	{
		PreventDefaultAction();
		var caster = GetCaster();

		if (caster == null)
			return;

		//caster->CastSpell(caster, SPELL_DH_SHATTERED_SOULS_MISSILE, new CastSpellExtraArgs(TriggerCastFlags.FullMask).AddSpellMod(SpellValueMod.BasePoint0, (int)SPELL_DH_LESSER_SOUL_SHARD));
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(OnProc, 0, AuraType.ProcTriggerSpell, AuraScriptHookType.EffectProc));
	}
}