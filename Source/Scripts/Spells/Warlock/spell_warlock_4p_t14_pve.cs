﻿using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Warlock
{
	// Raging Soul handled via Dark Soul: Instability (113858) \ Knowledge (113861) \ Misery (113860)
	[Script]
	public class spell_warlock_4p_t14_pve : SpellScript, ISpellAfterCast
	{
		public void AfterCast()
		{
			var caster = GetCaster();

			if (caster != null)
				if (caster.HasAura(WarlockSpells.T14_BONUS))
					caster.CastSpell(caster, WarlockSpells.RAGING_SOUL, true);
		}
	}
}