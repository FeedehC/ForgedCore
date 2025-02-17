﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.Scripting;
using Game.Scripting.Interfaces.IAreaTrigger;

namespace Scripts.Spells.Warlock;

// 211729 - Thal'kiel's Discord
// MiscId - 6913
[Script]
public class at_warlock_artifact_thalkiels_discord : AreaTriggerScript, IAreaTriggerOnUpdate
{
	public void OnUpdate(uint diff)
	{
		var caster = At.GetCaster();

		if (caster == null)
			return;

		var timer = At.VariableStorage.GetValue<int>("_timer", 0) + diff;

		if (timer >= 1300)
		{
			At.VariableStorage.Set<int>("_timer", 0);
			caster.CastSpell(At, WarlockSpells.THALKIES_DISCORD_DAMAGE, true);
		}
		else
		{
			At.VariableStorage.Set("_timer", timer);
		}
	}
}