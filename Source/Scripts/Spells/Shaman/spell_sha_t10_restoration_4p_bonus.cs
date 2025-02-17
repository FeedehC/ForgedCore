﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Shaman;

// 70808 - Item - Shaman T10 Restoration 4P Bonus
[SpellScript(70808)]
internal class spell_sha_t10_restoration_4p_bonus : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		PreventDefaultAction();

		var healInfo = eventInfo.HealInfo;

		if (healInfo == null ||
			healInfo.Heal == 0)
			return;

		var spellInfo = Global.SpellMgr.GetSpellInfo(ShamanSpells.ChainedHeal, CastDifficulty);
		var amount = (int)MathFunctions.CalculatePct(healInfo.Heal, aurEff.Amount);
		amount /= (int)spellInfo.MaxTicks;

		var caster = eventInfo.Actor;
		var target = eventInfo.ProcTarget;

		CastSpellExtraArgs args = new(aurEff);
		args.AddSpellMod(SpellValueMod.BasePoint0, amount);
		caster.CastSpell(target, ShamanSpells.ChainedHeal, args);
	}
}