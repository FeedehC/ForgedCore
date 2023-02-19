﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Maps;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Generic;

[Script]
internal class spell_gen_cannibalize : SpellScript, ISpellCheckCast, IHasSpellEffects
{
	public override bool Validate(SpellInfo spellInfo)
	{
		return ValidateSpellInfo(GenericSpellIds.CannibalizeTriggered);
	}

	public SpellCastResult CheckCast()
	{
		var caster    = GetCaster();
		var max_range = GetSpellInfo().GetMaxRange(false);
		// search for nearby enemy corpse in range
		var check    = new AnyDeadUnitSpellTargetInRangeCheck<Unit>(caster, max_range, GetSpellInfo(), SpellTargetCheckTypes.Enemy, SpellTargetObjectTypes.CorpseEnemy);
		var searcher = new UnitSearcher(caster, check, GridType.Grid);
		Cell.VisitGrid(caster, searcher, max_range);

		if (!searcher.GetTarget())
		{
			searcher.GridType = GridType.World;
			Cell.VisitGrid(caster, searcher, max_range);
		}

		if (!searcher.GetTarget())
			return SpellCastResult.NoEdibleCorpses;

		return SpellCastResult.SpellCastOk;
	}

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHit));
	}

	public List<ISpellEffect> SpellEffects { get; } = new();

	private void HandleDummy(int effIndex)
	{
		GetCaster().CastSpell(GetCaster(), GenericSpellIds.CannibalizeTriggered, false);
	}
}