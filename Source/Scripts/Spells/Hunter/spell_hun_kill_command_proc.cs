﻿using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Hunter;

[SpellScript(83381)]
public class spell_hun_kill_command_proc : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects => new();

	private void HandleDamage(uint UnnamedParameter)
	{
		var caster = GetCaster();
		var owner  = caster.GetOwner();
		var target = GetExplTargetUnit();

		// (1.5 * (rap * 3) * bmMastery * lowNerf * (1 + versability))
		var dmg     = (uint)(4.5f * owner.m_unitData.RangedAttackPower);
		var lowNerf = (uint)(Math.Min((int)owner.GetLevel(), 20) * 0.05f);

		var ownerPlayer = owner.ToPlayer();

		if (ownerPlayer != null)
			dmg = MathFunctions.AddPct(ref dmg, ownerPlayer.m_activePlayerData.Mastery);

		dmg *= lowNerf;

		dmg = caster.SpellDamageBonusDone(target, GetSpellInfo(), dmg, DamageEffectType.Direct, GetEffectInfo(0));
		dmg = target.SpellDamageBonusTaken(caster, GetSpellInfo(), dmg, DamageEffectType.Direct);

		SetHitDamage(dmg);
	}

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDamage, 0, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
	}
}