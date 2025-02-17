﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.DemonHunter;

[SpellScript(224509)]
public class spell_dh_frailty : AuraScript, IHasAuraEffects
{
	double _damage = 0;
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(PeriodicTick, 0, AuraType.PeriodicDummy));
		AuraEffects.Add(new AuraEffectProcHandler(OnProc, 0, AuraType.PeriodicDummy, AuraScriptHookType.EffectProc));
	}

	private void OnProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		PreventDefaultAction();
		var caster = Caster;

		if (caster == null || caster != eventInfo.Actor || eventInfo.DamageInfo != null)
			return;

		_damage += MathFunctions.CalculatePct(eventInfo.DamageInfo.Damage, aurEff.Amount);
	}

	private void PeriodicTick(AuraEffect UnnamedParameter)
	{
		var caster = Caster;

		if (caster == null)
			return;

		if (_damage != 0)
		{
			caster.CastSpell(caster, DemonHunterSpells.FRAILTY_HEAL, (int)(_damage * .1), true);
			_damage = 0;
		}
	}
}