﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Priest;

[SpellScript(27827)]
public class spell_pri_spirit_of_redemption_form : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(AfterRemove, 0, AuraType.WaterBreathing, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterRemove));
	}

	private void AfterRemove(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var l_Target = Target;

		l_Target.RemoveAura(eSpells.SpiritOfRedemptionForm);
		l_Target.RemoveAura(eSpells.SpiritOfRedemptionImmunity);
	}

	private struct eSpells
	{
		public const uint SpiritOfRedemptionImmunity = 62371;
		public const uint SpiritOfRedemptionForm = 27795;
	}
}