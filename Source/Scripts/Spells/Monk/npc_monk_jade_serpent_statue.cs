﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Monk;

[CreatureScript(60849)]
public class npc_monk_jade_serpent_statue : ScriptedAI
{
	public npc_monk_jade_serpent_statue(Creature c) : base(c) { }

	public override void UpdateAI(uint diff)
	{
		var owner = Me.OwnerUnit;

		if (owner != null)
		{
			var player = owner.AsPlayer;

			if (player != null)
			{
				if (player.Class != PlayerClass.Monk)
				{
					return;
				}
				else
				{
					if (player.GetPrimarySpecialization() != TalentSpecialization.MonkMistweaver && Me.IsInWorld)
						Me.DespawnOrUnsummon();
				}
			}
		}
	}
}