﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.DeathKnight;

[SpellScript(new uint[]
{
	48263, 48265, 48266
})]
public class spell_dk_presence_AuraScript : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		if (ScriptSpellId == DeathKnightSpells.FROST_PRESENCE)
			AuraEffects.Add(new AuraEffectApplyHandler(HandleImprovedFrostPresence, 0, AuraType.Any, AuraEffectHandleModes.Real));

		if (ScriptSpellId == DeathKnightSpells.UNHOLY_PRESENCE)
			AuraEffects.Add(new AuraEffectApplyHandler(HandleImprovedUnholyPresence, 0, AuraType.Any, AuraEffectHandleModes.Real));

		AuraEffects.Add(new AuraEffectApplyHandler(HandleEffectRemove, 0, AuraType.Any, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterRemove));
	}

	private void HandleImprovedFrostPresence(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var target = Target;
		var impAurEff = target.GetAuraEffect(DeathKnightSpells.IMPROVED_FROST_PRESENCE, 0);

		if (impAurEff != null)
			impAurEff.SetAmount(impAurEff.CalculateAmount(Caster));
	}

	private void HandleImprovedUnholyPresence(AuraEffect aurEff, AuraEffectHandleModes UnnamedParameter)
	{
		var target = Target;
		var impAurEff = target.GetAuraEffect(DeathKnightSpells.IMPROVED_UNHOLY_PRESENCE, 0);

		if (impAurEff != null)
			if (!target.HasAura(DeathKnightSpells.IMPROVED_UNHOLY_PRESENCE_TRIGGERED))
				target.CastSpell(target, DeathKnightSpells.IMPROVED_UNHOLY_PRESENCE_TRIGGERED, new CastSpellExtraArgs(TriggerCastFlags.FullMask).AddSpellMod(SpellValueMod.BasePoint0, (int)impAurEff.Amount).SetTriggeringAura(aurEff));
	}

	private void HandleEffectRemove(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var target = Target;
		var impAurEff = target.GetAuraEffect(DeathKnightSpells.IMPROVED_FROST_PRESENCE, 0);

		if (impAurEff != null)
			impAurEff.SetAmount(0);

		target.RemoveAura(DeathKnightSpells.IMPROVED_UNHOLY_PRESENCE_TRIGGERED);
	}
}