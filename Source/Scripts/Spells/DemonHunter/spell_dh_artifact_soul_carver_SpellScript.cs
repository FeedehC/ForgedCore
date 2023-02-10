﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.DemonHunter;

[SpellScript(207407)]
public class spell_dh_artifact_soul_carver_SpellScript : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects => new();

	private void HandleOnHit(uint UnnamedParameter)
	{
		var target = GetHitUnit();

		if (target != null)
		{
			var attackPower    = GetCaster().GetTotalAttackPowerValue(WeaponAttackType.BaseAttack);
			var damage         = (165.0f / 100.0f) * attackPower + (165.0f / 100.0f) * attackPower;
			var damageOverTime = (107.415f / 100.0f) * attackPower;
			GetCaster().CastSpell(target, DemonHunterSpells.SPELL_DH_SOUL_CARVER_DAMAGE, (int)damage);
			GetCaster().CastSpell(target, DemonHunterSpells.SPELL_DH_SOUL_CARVER_DAMAGE, (int)damageOverTime);
			// Code for shattering the soul fragments
		}
	}

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleOnHit, 2, SpellEffectName.WeaponPercentDamage, SpellScriptHookType.EffectHitTarget));
	}
}