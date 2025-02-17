﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Monk;

[SpellScript(191840)]
public class spell_monk_essence_font_heal : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 0, Targets.UnitDestAreaAlly));
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 1, Targets.UnitDestAreaAlly));
	}

	private void FilterTargets(List<WorldObject> p_Targets)
	{
		var caster = Caster;

		if (caster != null)
		{
			p_Targets.RemoveIf((WorldObject @object) =>
			{
				if (@object == null || @object.AsUnit == null)
					return true;

				var unit = @object.AsUnit;

				if (unit == caster)
					return true;

				if (unit.HasAura(MonkSpells.ESSENCE_FONT_HEAL) && unit.GetAura(MonkSpells.ESSENCE_FONT_HEAL).Duration > 5 * Time.InMilliseconds)
					return true;

				return false;
			});

			if (p_Targets.Count > 1)
			{
				p_Targets.Sort(new HealthPctOrderPred());
				p_Targets.Resize(1);
			}
		}
	}
}