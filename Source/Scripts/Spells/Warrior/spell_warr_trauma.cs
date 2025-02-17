﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Warrior;

[Script] // 215538 - Trauma
internal class spell_warr_trauma : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		var target = eventInfo.ActionTarget;
		//Get 25% of Damage from the spell casted (Slam & Whirlwind) plus Remaining Damage from Aura
		var damage = (int)(MathFunctions.CalculatePct(eventInfo.DamageInfo.Damage, aurEff.Amount) / Global.SpellMgr.GetSpellInfo(WarriorSpells.TRAUMA_EFFECT, CastDifficulty).MaxTicks);
		CastSpellExtraArgs args = new(TriggerCastFlags.FullMask);
		args.AddSpellMod(SpellValueMod.BasePoint0, damage);
		Caster.CastSpell(target, WarriorSpells.TRAUMA_EFFECT, args);
	}
}