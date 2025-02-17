﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.DeathKnight;

[Script] // 47496 - Explode, Ghoul spell for Corpse Explosion
internal class spell_dk_ghoul_explode : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDamage, 0, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
		SpellEffects.Add(new EffectHandler(Suicide, 1, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDamage(int effIndex)
	{
		HitDamage = (int)Caster.CountPctFromMaxHealth(GetEffectInfo(2).CalcValue(Caster));
	}

	private void Suicide(int effIndex)
	{
		var unitTarget = HitUnit;

		if (unitTarget)
			// Corpse Explosion (Suicide)
			unitTarget.CastSpell(unitTarget, DeathKnightSpells.CorpseExplosionTriggered, true);
	}
}