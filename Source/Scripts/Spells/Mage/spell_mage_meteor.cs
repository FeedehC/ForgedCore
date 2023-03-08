﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Mage;

[SpellScript(153561)]
public class spell_mage_meteor : SpellScript, ISpellAfterCast
{
	public override bool Validate(SpellInfo UnnamedParameter)
	{
		return ValidateSpellInfo(MageSpells.METEOR_DAMAGE);
	}

	public void AfterCast()
	{
		var caster = Caster;
		var dest = ExplTargetDest;

		if (caster == null || dest == null)
			return;

		caster.CastSpell(new Position(dest.X, dest.Y, dest.Z), MageSpells.METEOR_TIMER, true);
	}
}