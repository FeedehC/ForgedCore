﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.DataStorage;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Paladin;

[SpellScript(25914)] // 25914 - Holy Shock
internal class spell_pal_holy_shock_heal_visual : SpellScript, ISpellAfterHit
{
	public override bool Validate(SpellInfo spellInfo)
	{
		return CliDB.SpellVisualStorage.HasRecord(SpellVisual.HolyShockHeal) && CliDB.SpellVisualStorage.HasRecord(SpellVisual.HolyShockHealCrit);
	}

	public void AfterHit()
	{
		Caster.SendPlaySpellVisual(HitUnit, IsHitCrit ? SpellVisual.HolyShockHealCrit : SpellVisual.HolyShockHeal, 0, 0, 0.0f, false);
	}
}