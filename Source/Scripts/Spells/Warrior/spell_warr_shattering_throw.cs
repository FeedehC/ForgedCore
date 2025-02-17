﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Warrior;

// 64380, 65941 - Shattering Throw
[SpellScript(new uint[]
{
	64380, 65941
})]
public class spell_warr_shattering_throw : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScript, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScript(int effIndex)
	{
		PreventHitDefaultEffect(effIndex);

		// remove shields, will still display immune to damage part
		var target = HitUnit;

		if (target != null)
			target.RemoveAurasWithMechanic((ulong)Mechanics.ImmuneShield, AuraRemoveMode.EnemySpell);
	}
}