﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Rogue;

[SpellScript(195452)]
public class spell_rog_nightblade_AuraScript : AuraScript, IHasAuraEffects
{
	private int _cp;
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public bool CheckProc(ProcEventInfo eventInfo)
	{
		if (eventInfo.DamageInfo.AttackType == WeaponAttackType.BaseAttack || eventInfo.DamageInfo.AttackType == WeaponAttackType.OffAttack)
		{
			var caster = eventInfo.Actor;
			var target = eventInfo.ActionTarget;

			if (caster == null || target == null)
				return false;

			caster.CastSpell(target, RogueSpells.NIGHTBLADE_SLOW, true);

			return true;
		}

		return false;
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(HandleApply, 0, AuraType.PeriodicDamage, AuraEffectHandleModes.RealOrReapplyMask));
	}

	private void HandleApply(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
	{
		var caster = Caster;

		if (caster == null)
			return;

		var maxcp = caster.HasAura(RogueSpells.DEEPER_STRATAGEM) ? 6 : 5;
		_cp = Math.Min(caster.GetPower(PowerType.ComboPoints) + 1, maxcp);

		var aur = Aura;

		if (aur != null)
		{
			aur.SetMaxDuration(6000 + 2000 * _cp);
			aur.RefreshDuration();
		}

		if (caster != null)
			caster.ModifyPower(PowerType.ComboPoints, -1 * (_cp - 1));

		var catEntry = Global.SpellMgr.GetSpellInfo(RogueSpells.SHADOW_DANCE, Difficulty.None);

		if (caster.HasAura(RogueSpells.DEEPENING_SHADOWS) && RandomHelper.randChance(20 * _cp))
			caster.SpellHistory.ModifyCooldown(catEntry, TimeSpan.FromMilliseconds(_cp * -3000));

		if (caster != null)
			if (caster.HasAura(RogueSpells.RELENTLESS_STRIKES) && RandomHelper.randChance(20 * _cp))
				caster.CastSpell(caster, RogueSpells.RELENTLESS_STRIKES_POWER, true);

		if (caster.HasAura(RogueSpells.ALACRITY) && RandomHelper.randChance(20 * _cp))
			caster.CastSpell(caster, RogueSpells.ALACRITY_BUFF, true);
	}
}