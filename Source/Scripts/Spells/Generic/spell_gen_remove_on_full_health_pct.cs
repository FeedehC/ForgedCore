﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Generic;

[Script] // 71316 - Glacial Strike
internal class spell_gen_remove_on_full_health_pct : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(PeriodicTick, 2, AuraType.PeriodicDamagePercent));
	}

	private void PeriodicTick(AuraEffect aurEff)
	{
		// they apply Damage so no need to check for ticks here

		if (Target.IsFullHealth)
		{
			Remove(AuraRemoveMode.EnemySpell);
			PreventDefaultAction();
		}
	}
}