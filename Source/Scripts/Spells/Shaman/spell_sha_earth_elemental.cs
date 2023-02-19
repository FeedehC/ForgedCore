﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Shaman
{
	// 198103
	[SpellScript(198103)]
	public class spell_sha_earth_elemental : SpellScript, IHasSpellEffects
	{
		public List<ISpellEffect> SpellEffects { get; } = new();

		private void HandleSummon(int effIndex)
		{
			GetCaster().CastSpell(GetHitUnit(), ShamanSpells.EARTH_ELEMENTAL_SUMMON, true);
		}

		public override void Register()
		{
			SpellEffects.Add(new EffectHandler(HandleSummon, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
		}
	}
}