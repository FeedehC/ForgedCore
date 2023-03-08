﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Warrior
{
    [Script] // 46968 - Shockwave
	internal class spell_warr_shockwave : SpellScript, ISpellAfterCast, IHasSpellEffects
	{
		private uint _targetCount;

		public override bool Validate(SpellInfo spellInfo)
		{
			if (!ValidateSpellInfo(WarriorSpells.SHOCKWAVE, WarriorSpells.SHOCKWAVE_STUN))
				return false;

			return spellInfo.Effects.Count > 3;
		}

		public override bool Load()
		{
			return GetCaster().IsTypeId(TypeId.Player);
		}

		// Cooldown reduced by 20 sec if it strikes at least 3 targets.
		public void AfterCast()
		{
			if (_targetCount >= (uint)GetEffectInfo(0).CalcValue())
				GetCaster().ToPlayer().GetSpellHistory().ModifyCooldown(GetSpellInfo().Id, TimeSpan.FromSeconds(-GetEffectInfo(3).CalcValue()));
		}

		public override void Register()
		{
			SpellEffects.Add(new EffectHandler(HandleStun, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
		}

		public List<ISpellEffect> SpellEffects { get; } = new();

		private void HandleStun(int effIndex)
		{
			GetCaster().CastSpell(GetHitUnit(), WarriorSpells.SHOCKWAVE_STUN, true);
			++_targetCount;
		}
	}
}