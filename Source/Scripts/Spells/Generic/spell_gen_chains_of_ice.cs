﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Generic;

[Script] // 66020 Chains of Ice
internal class spell_gen_chains_of_ice : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectUpdatePeriodicHandler(UpdatePeriodic, 1, AuraType.PeriodicDummy));
	}

	private void UpdatePeriodic(AuraEffect aurEff)
	{
		// Get 0 effect aura
		var slow = Aura.GetEffect(0);

		if (slow == null)
			return;

		var newAmount = Math.Min(slow.Amount + aurEff.Amount, 0);
		slow.ChangeAmount(newAmount);
	}
}