﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.DeathKnight;

[SpellScript(51271)]
public class spell_dk_pillar_of_frost : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects => new();

	private void OnRemove(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var _player = GetTarget().ToPlayer();

		if (_player != null)
			_player.ApplySpellImmune(DeathKnightSpells.SPELL_DK_PILLAR_OF_FROST, SpellImmunity.Mechanic, Mechanics.Knockout, false);
	}

	private void OnApply(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var _player = GetTarget().ToPlayer();

		if (_player != null)
			_player.ApplySpellImmune(DeathKnightSpells.SPELL_DK_PILLAR_OF_FROST, SpellImmunity.Mechanic, Mechanics.Knockout, true);
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(OnApply, 0, AuraType.ModTotalStatPercentage, AuraEffectHandleModes.Real));
		AuraEffects.Add(new AuraEffectApplyHandler(OnRemove, 1, AuraType.Dummy, AuraEffectHandleModes.Real, AuraScriptHookType.EffectRemove));
	}
}