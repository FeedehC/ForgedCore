﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Items;

[Script] // 13280 Gnomish Death Ray
internal class spell_item_gnomish_death_ray : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDummy(int effIndex)
	{
		var caster = Caster;
		var target = HitUnit;

		if (target)
		{
			if (RandomHelper.URand(0, 99) < 15)
				caster.CastSpell(caster, ItemSpellIds.GnomishDeathRaySelf, true); // failure
			else
				caster.CastSpell(target, ItemSpellIds.GnomishDeathRayTarget, true);
		}
	}
}