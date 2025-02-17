﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Items;

[Script] // 23442 - Dimensional Ripper - Everlook
internal class spell_item_dimensional_ripper_everlook : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override bool Load()
	{
		return Caster.IsPlayer;
	}

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScript, 0, SpellEffectName.TeleportUnits, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScript(int effIndex)
	{
		var r = RandomHelper.IRand(0, 119);

		if (r <= 70) // 7/12 success
			return;

		var caster = Caster;

		if (r < 100) // 4/12 evil twin
			caster.CastSpell(caster, ItemSpellIds.EvilTwin, true);
		else // 1/12 fire
			caster.CastSpell(caster, ItemSpellIds.TransporterMalfunctionFire, true);
	}
}