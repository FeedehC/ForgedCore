﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Items;

[Script("spell_item_purified_shard_of_the_scale", ItemSpellIds.PurifiedCauterizingHeal, ItemSpellIds.PurifiedSearingFlames)]
[Script("spell_item_shiny_shard_of_the_scale", ItemSpellIds.ShinyCauterizingHeal, ItemSpellIds.ShinySearingFlames)]
internal class spell_item_shard_of_the_scale : AuraScript, IHasAuraEffects
{
	private readonly uint _damageProcSpellId;

	private readonly uint _healProcSpellId;

	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public spell_item_shard_of_the_scale(uint healProcSpellId, uint damageProcSpellId)
	{
		_healProcSpellId = healProcSpellId;
		_damageProcSpellId = damageProcSpellId;
	}


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		PreventDefaultAction();
		var caster = eventInfo.Actor;
		var target = eventInfo.ProcTarget;

		if (eventInfo.TypeMask.HasFlag(ProcFlags.DealHelpfulSpell))
			caster.CastSpell(target, _healProcSpellId, new CastSpellExtraArgs(aurEff));

		if (eventInfo.TypeMask.HasFlag(ProcFlags.DealHarmfulSpell))
			caster.CastSpell(target, _damageProcSpellId, new CastSpellExtraArgs(aurEff));
	}
}