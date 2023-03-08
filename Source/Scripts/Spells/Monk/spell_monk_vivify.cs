﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Maps;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Monk;

[SpellScript(116670)]
public class spell_monk_vivify : SpellScript, IHasSpellEffects, ISpellAfterCast, ISpellBeforeCast
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	private void FilterRenewingMist(List<WorldObject> targets)
	{
		targets.RemoveIf(new UnitAuraCheck<WorldObject>(false, MonkSpells.RENEWING_MIST_HOT, GetCaster().GetGUID()));
	}

	public void BeforeCast()
	{
		if (GetCaster().GetCurrentSpell(CurrentSpellTypes.Channeled) && GetCaster().GetCurrentSpell(CurrentSpellTypes.Channeled).SpellInfo.Id == MonkSpells.SOOTHING_MIST)
		{
			GetSpell().CastFlagsEx = SpellCastFlagsEx.None;
			var targets = GetCaster().GetCurrentSpell(CurrentSpellTypes.Channeled).Targets;
			GetSpell().InitExplicitTargets(targets);
		}
	}

	public void AfterCast()
	{
		var caster = GetCaster().ToPlayer();

		if (caster == null)
			return;

		if (caster.HasAura(MonkSpells.LIFECYCLES))
			caster.CastSpell(caster, MonkSpells.LIFECYCLES_ENVELOPING_MIST, true);
	}

	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterRenewingMist, 1, Targets.UnitDestAreaAlly));
	}
}