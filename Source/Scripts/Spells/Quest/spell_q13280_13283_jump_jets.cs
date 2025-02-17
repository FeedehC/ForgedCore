﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Quest;

[Script] // 4336 - Jump Jets
internal class spell_q13280_13283_jump_jets : SpellScript, ISpellOnCast
{
	public void OnCast()
	{
		var caster = Caster;

		if (caster.IsVehicle)
		{
			var rocketBunny = caster.VehicleKit1.GetPassenger(1);

			rocketBunny?.CastSpell(rocketBunny, QuestSpellIds.JumpRocketBlast, true);
		}
	}
}