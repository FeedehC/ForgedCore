﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Quest;

[Script] // 50133 - Scourging Crystal Controller
internal class spell_q11396_11399_scourging_crystal_controller : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDummy(int effIndex)
	{
		var target = HitUnit;

		if (target)
			if (target.IsTypeId(TypeId.Unit) &&
				target.HasAura(QuestSpellIds.ForceShieldArcanePurpleX3))
				// Make sure nobody else is channeling the same Target
				if (!target.HasAura(QuestSpellIds.ScourgingCrystalController))
					Caster.CastSpell(target, QuestSpellIds.ScourgingCrystalController, new CastSpellExtraArgs(CastItem));
	}
}