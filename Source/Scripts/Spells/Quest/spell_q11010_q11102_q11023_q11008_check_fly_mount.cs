﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Quest;

[Script] // 40160 - Throw Bomb
internal class spell_q11010_q11102_q11023_q11008_check_fly_mount : SpellScript, ISpellCheckCast
{
	public SpellCastResult CheckCast()
	{
		var caster = Caster;

		// This spell will be cast only if caster has one of these Auras
		if (!(caster.HasAuraType(AuraType.Fly) || caster.HasAuraType(AuraType.ModIncreaseMountedFlightSpeed)))
			return SpellCastResult.CantDoThatRightNow;

		return SpellCastResult.SpellCastOk;
	}
}