﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Warrior
{
    [Script] // 215538 - Trauma
	internal class spell_warr_trauma : AuraScript, IHasAuraEffects
	{
		public List<IAuraEffectHandler> AuraEffects { get; } = new();

		public override bool Validate(SpellInfo spellInfo)
		{
			return ValidateSpellInfo(WarriorSpells.TRAUMA_EFFECT);
		}

		public override void Register()
		{
			AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
		}

		private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
		{
			var target = eventInfo.GetActionTarget();
			//Get 25% of Damage from the spell casted (Slam & Whirlwind) plus Remaining Damage from Aura
			var                damage = (int)(MathFunctions.CalculatePct(eventInfo.GetDamageInfo().GetDamage(), aurEff.Amount) / Global.SpellMgr.GetSpellInfo(WarriorSpells.TRAUMA_EFFECT, GetCastDifficulty()).GetMaxTicks());
			CastSpellExtraArgs args   = new(TriggerCastFlags.FullMask);
			args.AddSpellMod(SpellValueMod.BasePoint0, damage);
			GetCaster().CastSpell(target, WarriorSpells.TRAUMA_EFFECT, args);
		}
	}
}