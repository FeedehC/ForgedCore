﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Paladin;

[SpellScript(20473)] // 20473 - Holy Shock
internal class spell_pal_holy_shock : SpellScript, ISpellCheckCast, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public SpellCastResult CheckCast()
	{
		var caster = Caster;
		var target = ExplTargetUnit;

		if (target)
		{
			if (!caster.IsFriendlyTo(target))
			{
				if (!caster.IsValidAttackTarget(target))
					return SpellCastResult.BadTargets;

				if (!caster.IsInFront(target))
					return SpellCastResult.NotInfront;
			}
		}
		else
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
		var caster = Caster;
		var unitTarget = HitUnit;

		if (unitTarget != null)
		{
			if (caster.IsFriendlyTo(unitTarget))
				caster.CastSpell(unitTarget, PaladinSpells.HolyShockHealing, new CastSpellExtraArgs(Spell));
			else
				caster.CastSpell(unitTarget, PaladinSpells.HolyShockDamage, new CastSpellExtraArgs(Spell));
		}
	}
}