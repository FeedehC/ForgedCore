﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Shaman;

// Frostbrand - 196834
[SpellScript(196834)]
public class bfa_spell_frostbrand_SpellScript : SpellScript, ISpellOnHit
{
	public override bool Load()
	{
		return Caster.IsPlayer;
	}

	public void OnHit()
	{
		var caster = Caster;
		var target = HitUnit;

		if (caster == null || target == null)
			return;

		caster.CastSpell(target, ShamanSpells.FROSTBRAND_SLOW, true);
	}
}