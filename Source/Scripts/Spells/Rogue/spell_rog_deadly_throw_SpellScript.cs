﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Rogue;

[SpellScript(26679)]
public class spell_rog_deadly_throw_SpellScript : SpellScript, ISpellOnHit
{
	public void OnHit()
	{
		var target = HitUnit;

		if (target != null)
		{
			var caster = Caster.AsPlayer;

			if (caster != null)
				if (caster.GetPower(PowerType.ComboPoints) >= 5)
					caster.CastSpell(target, 137576, true);
		}
	}
}