﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.DeathKnight;

[Script] // 69961 - Glyph of Scourge Strike
internal class spell_dk_glyph_of_scourge_strike_script : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScriptEffect, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScriptEffect(int effIndex)
	{
		var caster = Caster;
		var target = HitUnit;

		var mPeriodic = target.GetAuraEffectsByType(AuraType.PeriodicDamage);

		foreach (var aurEff in mPeriodic)
		{
			var spellInfo = aurEff.SpellInfo;

			// search our Blood Plague and Frost Fever on Target
			if (spellInfo.SpellFamilyName == SpellFamilyNames.Deathknight &&
				spellInfo.SpellFamilyFlags[2].HasAnyFlag(0x2u) &&
				aurEff.CasterGuid == caster.GUID)
			{
				var countMin = aurEff.Base.MaxDuration;
				var countMax = spellInfo.MaxDuration;

				// this Glyph
				countMax += 9000;

				if (countMin < countMax)
				{
					aurEff.Base.SetDuration(aurEff.Base.Duration + 3000);
					aurEff.Base.SetMaxDuration(countMin + 3000);
				}
			}
		}
	}
}