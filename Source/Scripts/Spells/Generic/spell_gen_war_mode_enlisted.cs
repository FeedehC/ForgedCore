﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Framework.Models;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Generic;

[Script] // 282559 - Enlisted
internal class spell_gen_war_mode_enlisted : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		var spellInfo = Global.SpellMgr.GetSpellInfo(ScriptSpellId, Difficulty.None);

		if (spellInfo.HasAura(AuraType.ModXpPct))
			AuraEffects.Add(new AuraEffectCalcAmountHandler(CalcWarModeBonus, SpellConst.EffectAll, AuraType.ModXpPct));

		if (spellInfo.HasAura(AuraType.ModXpQuestPct))
			AuraEffects.Add(new AuraEffectCalcAmountHandler(CalcWarModeBonus, SpellConst.EffectAll, AuraType.ModXpQuestPct));

		if (spellInfo.HasAura(AuraType.ModCurrencyGainFromSource))
			AuraEffects.Add(new AuraEffectCalcAmountHandler(CalcWarModeBonus, SpellConst.EffectAll, AuraType.ModCurrencyGainFromSource));

		if (spellInfo.HasAura(AuraType.ModMoneyGain))
			AuraEffects.Add(new AuraEffectCalcAmountHandler(CalcWarModeBonus, SpellConst.EffectAll, AuraType.ModMoneyGain));

		if (spellInfo.HasAura(AuraType.ModAnimaGain))
			AuraEffects.Add(new AuraEffectCalcAmountHandler(CalcWarModeBonus, SpellConst.EffectAll, AuraType.ModAnimaGain));

		if (spellInfo.HasAura(AuraType.Dummy))
			AuraEffects.Add(new AuraEffectCalcAmountHandler(CalcWarModeBonus, SpellConst.EffectAll, AuraType.Dummy));
	}

	private void CalcWarModeBonus(AuraEffect aurEff, BoxedValue<double> amount, BoxedValue<bool> canBeRecalculated)
	{
		var target = OwnerAsUnit.AsPlayer;

		if (target == null)
			return;

		switch (target.TeamId)
		{
			case TeamIds.Alliance:
				amount.Value = Global.WorldStateMgr.GetValue(WorldStates.WarModeAllianceBuffValue, target.Map);

				break;
			case TeamIds.Horde:
				amount.Value = Global.WorldStateMgr.GetValue(WorldStates.WarModeHordeBuffValue, target.Map);

				break;
		}
	}
}