﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Warlock;

// 111898 - Grimoire: Felguard
[SpellScript(111898)]
public class spell_warlock_grimoire_felguard : SpellScript, ISpellCheckCast
{
	public SpellCastResult CheckCast()
	{
		var caster = Caster.AsPlayer;

		if (caster == null)
			return SpellCastResult.CantDoThatRightNow;

		// allow only in Demonology spec
		if (caster.GetPrimarySpecialization() != TalentSpecialization.WarlockDemonology)
			return SpellCastResult.NoSpec;

		return SpellCastResult.SpellCastOk;
	}
}