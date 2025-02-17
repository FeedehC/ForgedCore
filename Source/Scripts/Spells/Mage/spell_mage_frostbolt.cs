﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Mage;

[Script] // 116 - Frostbolt
internal class spell_mage_frostbolt : SpellScript, ISpellOnHit
{
	public void OnHit()
	{
		var target = HitUnit;

		if (target != null)
			Caster.CastSpell(target, MageSpells.Chilled, new CastSpellExtraArgs(TriggerCastFlags.IgnoreCastInProgress));
	}
}