﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Quest;

[Script] // 49587 Seeds of Nature's Wrath
internal class spell_q12459_seeds_of_natures_wrath : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDummy(int effIndex)
	{
		var creatureTarget = HitCreature;

		if (creatureTarget)
		{
			uint uiNewEntry = 0;

			switch (creatureTarget.Entry)
			{
				case CreatureIds.ReanimatedFrostwyrm:
					uiNewEntry = CreatureIds.WeakReanimatedFrostwyrm;

					break;
				case CreatureIds.Turgid:
					uiNewEntry = CreatureIds.WeakTurgid;

					break;
				case CreatureIds.Deathgaze:
					uiNewEntry = CreatureIds.WeakDeathgaze;

					break;
			}

			if (uiNewEntry != 0)
				creatureTarget.UpdateEntry(uiNewEntry);
		}
	}
}