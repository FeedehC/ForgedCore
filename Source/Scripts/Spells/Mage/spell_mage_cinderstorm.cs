﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Mage;

[SpellScript(198928)]
public class spell_mage_cinderstorm : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDamage, 0, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDamage(int effIndex)
	{
		var caster = Caster;
		var target = HitUnit;

		if (caster == null || target == null)
			return;

		if (target.HasAura(MageSpells.IGNITE_DOT))
		{
			//    int32 pct = Global.SpellMgr->GetSpellInfo(CINDERSTORM, Difficulty.None)->GetEffect(0).CalcValue(caster);
			var dmg = HitDamage;
			// MathFunctions.AddPct(ref dmg, pct);
			HitDamage = dmg;
		}
	}
}