﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Druid;

[Script]
public class at_dru_lunar_beam : AreaTriggerAI
{
	public at_dru_lunar_beam(AreaTrigger at) : base(at) { }

	public override void OnCreate()
	{
		At.SetPeriodicProcTimer(1000);
	}

	public override void OnPeriodicProc()
	{
		if (At.GetCaster())
			At.GetCaster().CastSpell(At.Location, DruidSpells.LUNAR_BEAM_DAMAGE_HEAL, true);
	}
}