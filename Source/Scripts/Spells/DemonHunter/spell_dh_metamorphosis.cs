﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.DemonHunter;

[SpellScript(191427)]
public class spell_dh_metamorphosis : SpellScript, ISpellBeforeCast
{
	public override bool Validate(SpellInfo UnnamedParameter)
	{
		if (!Global.SpellMgr.HasSpellInfo(DemonHunterSpells.METAMORPHOSIS_HAVOC, Difficulty.None) || !Global.SpellMgr.HasSpellInfo(DemonHunterSpells.METAMORPHOSIS_JUMP, Difficulty.None) || !Global.SpellMgr.HasSpellInfo(DemonHunterSpells.METAMORPHOSIS_STUN, Difficulty.None))
			return false;

		return true;
	}

	public void BeforeCast()
	{
		var caster = Caster;

		if (caster == null)
			return;

		var player = caster.ToPlayer();

		if (player == null)
			return;

		var dest = ExplTargetDest;

		if (dest != null)
			player.CastSpell(new Position(dest.X, dest.Y, dest.Z), DemonHunterSpells.METAMORPHOSIS_JUMP, true);

		if (player.HasAura(DemonHunterSpells.DEMON_REBORN)) // Remove CD of Eye Beam, Chaos Nova and Blur
		{
			player.GetSpellHistory().ResetCooldown(DemonHunterSpells.CHAOS_NOVA, true);
			player.GetSpellHistory().ResetCooldown(DemonHunterSpells.BLUR, true);
			player.GetSpellHistory().AddCooldown(DemonHunterSpells.BLUR_BUFF, 0, TimeSpan.FromMinutes(1));
			player.GetSpellHistory().ResetCooldown(DemonHunterSpells.BLUR_BUFF, true);
			player.GetSpellHistory().ResetCooldown(DemonHunterSpells.EYE_BEAM, true);
		}
	}
}