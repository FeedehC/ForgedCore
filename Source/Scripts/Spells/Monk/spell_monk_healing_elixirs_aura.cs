﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Monk;

[SpellScript(122280)]
public class spell_monk_healing_elixirs_aura : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(OnProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	private void OnProc(AuraEffect UnnamedParameter, ProcEventInfo eventInfo)
	{
		PreventDefaultAction();

		if (!Caster)
			return;

		if (eventInfo.DamageInfo != null)
			return;

		if (eventInfo.DamageInfo != null)
			return;

		var caster = Caster;

		if (caster != null)
			if (caster.HealthBelowPctDamaged(35, eventInfo.DamageInfo.Damage))
			{
				caster.CastSpell(caster, MonkSpells.HEALING_ELIXIRS_RESTORE_HEALTH, true);
				caster.SpellHistory.ConsumeCharge(MonkSpells.HEALING_ELIXIRS_RESTORE_HEALTH);
			}
	}
}