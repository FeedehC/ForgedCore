﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.DeathKnight;

[SpellScript(156004)]
public class spell_dk_defile_aura : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(HandleApply, 0, AuraType.Dummy, AuraEffectHandleModes.Real));
	}


	private void HandleApply(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var target = Target;
		var caster = Caster;

		if (target == null || caster == null)
			return;

		var oneSec = TimeSpan.FromSeconds(1);

		caster.Events.AddRepeatEventAtOffset(() =>
											{
												if (target == null || caster == null)
													return default;

												caster.CastSpell(target, DeathKnightSpells.DEFILE_DAMAGE, true);

												if (target.HasAura(156004) && caster != null)
													return oneSec;

												return default;
											},
											oneSec);
	}
}