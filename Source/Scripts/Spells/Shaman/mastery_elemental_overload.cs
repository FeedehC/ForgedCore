﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IPlayer;
using Game.Spells;

namespace Scripts.Spells.Shaman;

//168534
[Script]
public class mastery_elemental_overload : ScriptObjectAutoAdd, IPlayerOnSpellCast
{
	public PlayerClass PlayerClass { get; } = PlayerClass.Shaman;

	public mastery_elemental_overload() : base("mastery_elemental_overload") { }

	public void OnSpellCast(Player player, Spell spell, bool UnnamedParameter)
	{
		if (player.GetPrimarySpecialization() != TalentSpecialization.ShamanElemental)
			return;

		if (player.HasAura(ShamanSpells.MASTERY_ELEMENTAL_OVERLOAD) && RandomHelper.randChance(15))
		{
			var spellInfo = spell.SpellInfo;

			if (spellInfo != null)
				switch (spell.SpellInfo.Id)
				{
					case ShamanSpells.LIGHTNING_BOLT_ELEM:
						player.CastSpell(player.SelectedUnit, ShamanSpells.LIGHTNING_BOLT_ELEM, true);

						break;
					case ShamanSpells.ELEMENTAL_BLAST:
						player.CastSpell(player.SelectedUnit, ShamanSpells.ELEMENTAL_BLAST, true);

						break;
					case ShamanSpells.LAVA_BURST:
						player.CastSpell(player.SelectedUnit, ShamanSpells.LAVA_BURST, true);

						break;
					case ShamanSpells.CHAIN_LIGHTNING:
						player.CastSpell(player.SelectedUnit, ShamanSpells.LAVA_BURST, true);

						break;
				}
		}
	}
}