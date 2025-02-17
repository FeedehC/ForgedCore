﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Items;

[Script] // 8213 Savory Deviate Delight
internal class spell_item_savory_deviate_delight : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override bool Load()
	{
		return Caster.TypeId == TypeId.Player;
	}


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHit));
	}

	private void HandleDummy(int effIndex)
	{
		var caster = Caster;
		uint spellId = 0;

		switch (RandomHelper.URand(1, 2))
		{
			// Flip Out - ninja
			case 1:
				spellId = (caster.NativeGender == Gender.Male ? ItemSpellIds.FlipOutMale : ItemSpellIds.FlipOutFemale);

				break;
			// Yaaarrrr - pirate
			case 2:
				spellId = (caster.NativeGender == Gender.Male ? ItemSpellIds.YaaarrrrMale : ItemSpellIds.YaaarrrrFemale);

				break;
		}

		caster.CastSpell(caster, spellId, true);
	}
}