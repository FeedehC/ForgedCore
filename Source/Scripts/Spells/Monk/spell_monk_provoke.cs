﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Monk;

[Script] // 115546 - Provoke
internal class spell_monk_provoke : SpellScript, ISpellCheckCast, IHasSpellEffects
{
	private const uint BlackOxStatusEntry = 61146;

	public List<ISpellEffect> SpellEffects { get; } = new();

	public override bool Validate(SpellInfo spellInfo)
	{
		if (!spellInfo.GetExplicitTargetMask().HasAnyFlag(SpellCastTargetFlags.UnitMask)) // ensure GetExplTargetUnit() will return something meaningful during CheckCast
			return false;

		return ValidateSpellInfo(MonkSpells.ProvokeSingleTarget, MonkSpells.ProvokeAoe);
	}

	public SpellCastResult CheckCast()
	{
		if (ExplTargetUnit.GetEntry() != BlackOxStatusEntry)
		{
			var singleTarget = Global.SpellMgr.GetSpellInfo(MonkSpells.ProvokeSingleTarget, CastDifficulty);
			var singleTargetExplicitResult = singleTarget.CheckExplicitTarget(Caster, ExplTargetUnit);

			if (singleTargetExplicitResult != SpellCastResult.SpellCastOk)
				return singleTargetExplicitResult;
		}
		else if (ExplTargetUnit.GetOwnerGUID() != Caster.GetGUID())
		{
			return SpellCastResult.BadTargets;
		}

		return SpellCastResult.SpellCastOk;
	}

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDummy(int effIndex)
	{
		PreventHitDefaultEffect(effIndex);

		if (HitUnit.GetEntry() != BlackOxStatusEntry)
			Caster.CastSpell(HitUnit, MonkSpells.ProvokeSingleTarget, true);
		else
			Caster.CastSpell(HitUnit, MonkSpells.ProvokeAoe, true);
	}
}