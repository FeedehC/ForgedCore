﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Monk;

[SpellScript(132464)]
public class spell_monk_chi_wave_healing_bolt : SpellScript, ISpellOnHit
{
	public void OnHit()
	{
		if (!OriginalCaster)
			return;

		var _player = OriginalCaster.AsPlayer;

		if (_player != null)
		{
			var target = HitUnit;

			if (target != null)
				_player.CastSpell(target, MonkSpells.CHI_WAVE_HEAL, true);
		}
	}
}