﻿using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Hunter;

[SpellScript(190928)]
public class spell_hun_mongoose_bite : SpellScript, ISpellAfterHit
{
	public override bool Validate(SpellInfo UnnamedParameter)
	{
		if (Global.SpellMgr.GetSpellInfo(HunterSpells.SPELL_HUNTER_MONGOOSE_BITE, Difficulty.None) != null)
		{
			return false;
		}
		return true;
	}

	public void AfterHit()
	{
		int  dur = 0;
		Aura aur = GetCaster().GetAura(HunterSpells.SPELL_HUNTER_MONGOOSE_FURY);
		if (aur != null)
		{
			dur = aur.GetDuration();
		}

		GetCaster().CastSpell(GetCaster(), HunterSpells.SPELL_HUNTER_MONGOOSE_FURY, true);

		aur = GetCaster().GetAura(HunterSpells.SPELL_HUNTER_MONGOOSE_FURY);
		if (aur != null)
		{
			if (dur != 0)
			{
				aur.SetDuration(dur);
			}
		}
	}
}