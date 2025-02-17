﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Rogue;

[SpellScript(108216)]
public class spell_rog_dirty_tricks_AuraScript : AuraScript
{
	public bool CheckProc(ProcEventInfo eventInfo)
	{
		var spellInfo = eventInfo.DamageInfo.SpellInfo;

		if (spellInfo == null)
			return true;

		if (eventInfo.Actor.GUID != CasterGUID)
			return true;

		if (spellInfo.Mechanic == Mechanics.Bleed || (spellInfo.GetAllEffectsMechanicMask() & (ulong)Mechanics.Bleed) != 0 || spellInfo.Dispel == DispelType.Poison)
			if (eventInfo.Actor.HasAura(108216))
				return false;

		return true;
	}
}