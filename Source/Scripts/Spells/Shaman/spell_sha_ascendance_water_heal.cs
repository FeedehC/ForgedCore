﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Shaman;

// Ascendance (Water)(heal) - 114083
[SpellScript(114083)]
public class spell_sha_ascendance_water_heal : SpellScript, IHasSpellEffects
{
	private uint m_TargetSize = 0;
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(OnEffectHeal, 0, SpellEffectName.Heal, SpellScriptHookType.EffectHitTarget));
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 0, Targets.UnitSrcAreaAlly));
	}

	private void OnEffectHeal(int effIndex)
	{
		HitHeal = (int)(HitHeal / m_TargetSize);
	}

	private void FilterTargets(List<WorldObject> p_Targets)
	{
		m_TargetSize = (uint)p_Targets.Count;
	}
}