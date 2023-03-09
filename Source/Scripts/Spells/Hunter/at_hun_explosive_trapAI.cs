﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Hunter;

[Script]
public class at_hun_explosive_trapAI : AreaTriggerAI
{
	public enum UsedSpells
	{
		EXPLOSIVE_TRAP_DAMAGE = 13812
	}

	public int timeInterval;

	public at_hun_explosive_trapAI(AreaTrigger areatrigger) : base(areatrigger)
	{
		timeInterval = 200;
	}

	public override void OnCreate()
	{
		var caster = At.GetCaster();

		if (caster == null)
			return;

		if (!caster.AsPlayer)
			return;

		foreach (var itr in At.InsideUnits)
		{
			var target = ObjectAccessor.Instance.GetUnit(caster, itr);

			if (!caster.IsFriendlyTo(target))
			{
				var tempSumm = caster.SummonCreature(SharedConst.WorldTrigger, At.Location, TempSummonType.TimedDespawn, TimeSpan.FromSeconds(200));

				if (tempSumm != null)
				{
					tempSumm.Faction = caster.Faction;
					tempSumm.SetSummonerGUID(caster.GUID);
					PhasingHandler.InheritPhaseShift(tempSumm, caster);
					caster.CastSpell(tempSumm, UsedSpells.EXPLOSIVE_TRAP_DAMAGE, true);
					At.Remove();
				}
			}
		}
	}

	public override void OnUnitEnter(Unit unit)
	{
		var caster = At.GetCaster();

		if (caster == null || unit == null)
			return;

		if (!caster.AsPlayer)
			return;

		if (!caster.IsFriendlyTo(unit))
		{
			var tempSumm = caster.SummonCreature(SharedConst.WorldTrigger, At.Location, TempSummonType.TimedDespawn, TimeSpan.FromSeconds(200));

			if (tempSumm != null)
			{
				tempSumm.Faction = caster.Faction;
				tempSumm.SetSummonerGUID(caster.GUID);
				PhasingHandler.InheritPhaseShift(tempSumm, caster);
				caster.CastSpell(tempSumm, UsedSpells.EXPLOSIVE_TRAP_DAMAGE, true);
				At.Remove();
			}
		}
	}
}