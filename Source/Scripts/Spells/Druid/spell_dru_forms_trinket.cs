﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Druid;

[Script] // 37336 - Druid Forms Trinket
internal class spell_dru_forms_trinket : AuraScript, IAuraCheckProc, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public bool CheckProc(ProcEventInfo eventInfo)
	{
		var target = eventInfo.Actor;

		switch (target.ShapeshiftForm)
		{
			case ShapeShiftForm.BearForm:
			case ShapeShiftForm.DireBearForm:
			case ShapeShiftForm.CatForm:
			case ShapeShiftForm.MoonkinForm:
			case ShapeShiftForm.None:
			case ShapeShiftForm.TreeOfLife:
				return true;
			default:
				break;
		}

		return false;
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.ProcTriggerSpell, AuraScriptHookType.EffectProc));
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		PreventDefaultAction();
		var target = eventInfo.Actor;
		uint triggerspell;

		switch (target.ShapeshiftForm)
		{
			case ShapeShiftForm.BearForm:
			case ShapeShiftForm.DireBearForm:
				triggerspell = DruidSpellIds.FormsTrinketBear;

				break;
			case ShapeShiftForm.CatForm:
				triggerspell = DruidSpellIds.FormsTrinketCat;

				break;
			case ShapeShiftForm.MoonkinForm:
				triggerspell = DruidSpellIds.FormsTrinketMoonkin;

				break;
			case ShapeShiftForm.None:
				triggerspell = DruidSpellIds.FormsTrinketNone;

				break;
			case ShapeShiftForm.TreeOfLife:
				triggerspell = DruidSpellIds.FormsTrinketTree;

				break;
			default:
				return;
		}

		target.CastSpell(target, triggerspell, new CastSpellExtraArgs(aurEff));
	}
}