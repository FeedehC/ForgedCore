﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.DemonHunter;

[SpellScript(213010)]
public class spell_dh_artifact_charred_warblades : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new List<IAuraEffectHandler>();

	private void OnProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		var caster = GetCaster();

		if (caster == null || eventInfo.GetDamageInfo() != null)
			return;

		if (eventInfo.GetDamageInfo() != null || (eventInfo.GetDamageInfo().GetSchoolMask() & SpellSchoolMask.Fire) == 0)
			return;

		var heal = MathFunctions.CalculatePct(eventInfo.GetDamageInfo().GetDamage(), aurEff.Amount);
		caster.CastSpell(caster, ShatteredSoulsSpells.CHARRED_WARBLADES_HEAL, (int)heal);
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(OnProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}
}