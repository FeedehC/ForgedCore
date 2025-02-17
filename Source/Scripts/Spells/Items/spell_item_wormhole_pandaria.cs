﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Items;

[Script] // 126755 - Wormhole: Pandaria
internal class spell_item_wormhole_pandaria : SpellScript, IHasSpellEffects
{
	private readonly uint[] WormholeTargetLocations =
	{
		ItemSpellIds.Wormholepandariaisleofreckoning, ItemSpellIds.Wormholepandariakunlaiunderwater, ItemSpellIds.Wormholepandariasravess, ItemSpellIds.Wormholepandariarikkitunvillage, ItemSpellIds.Wormholepandariazanvesstree, ItemSpellIds.Wormholepandariaanglerswharf, ItemSpellIds.Wormholepandariacranestatue, ItemSpellIds.Wormholepandariaemperorsomen, ItemSpellIds.Wormholepandariawhitepetallake
	};

	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleTeleport, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleTeleport(int effIndex)
	{
		PreventHitDefaultEffect(effIndex);
		var spellId = WormholeTargetLocations.SelectRandom();
		Caster.CastSpell(HitUnit, spellId, true);
	}
}