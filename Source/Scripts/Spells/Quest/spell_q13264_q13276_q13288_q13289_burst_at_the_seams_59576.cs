﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Quest;

[Script] // 59576 - Burst at the Seams
internal class spell_q13264_q13276_q13288_q13289_burst_at_the_seams_59576 : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScript, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScript(int effIndex)
	{
		var creature = Caster.AsCreature;

		if (creature != null)
		{
			creature.CastSpell(creature, QuestSpellIds.BloatedAbominationFeignDeath, true);
			creature.CastSpell(creature, QuestSpellIds.BurstAtTheSeams59579, true);
			creature.CastSpell(creature, QuestSpellIds.BurstAtTheSeamsBone, true);
			creature.CastSpell(creature, QuestSpellIds.BurstAtTheSeamsBone, true);
			creature.CastSpell(creature, QuestSpellIds.BurstAtTheSeamsBone, true);
			creature.CastSpell(creature, QuestSpellIds.ExplodeAbominationMeat, true);
			creature.CastSpell(creature, QuestSpellIds.ExplodeAbominationBloodyMeat, true);
			creature.CastSpell(creature, QuestSpellIds.ExplodeAbominationBloodyMeat, true);
			creature.CastSpell(creature, QuestSpellIds.ExplodeAbominationBloodyMeat, true);
		}
	}
}