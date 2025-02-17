﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.AI;
using Game.Entities;
using Game.Scripting;
using Scripts.EasternKingdoms.Deadmines.Bosses;

namespace Scripts.EasternKingdoms.Deadmines.NPC;

[CreatureScript(48445)]
public class npc_oaf_lackey : ScriptedAI
{
	public uint AxeHeadTimer;

	public bool below;

	public npc_oaf_lackey(Creature creature) : base(creature) { }

	public override void Reset()
	{
		AxeHeadTimer = 4000;
		below = true;
	}

	public override void UpdateAI(uint diff)
	{
		if (AxeHeadTimer <= diff)
		{
			DoCastVictim(boss_vanessa_vancleef.Spells.AXE_HEAD);
			AxeHeadTimer = RandomHelper.URand(18000, 21000);
		}
		else
		{
			AxeHeadTimer -= diff;
		}

		if (HealthBelowPct(35) && !below)
		{
			DoCast(Me, boss_vanessa_vancleef.Spells.ENRAGE);
			below = true;
		}

		DoMeleeAttackIfReady();
	}
}