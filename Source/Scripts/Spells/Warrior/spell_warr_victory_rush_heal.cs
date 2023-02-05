﻿using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Warrior
{
    // Victory Rush (heal) - 118779
    [SpellScript(118779)]
    public class spell_warr_victory_rush_heal : SpellScript, IOnHit
    {
        public override bool Validate(SpellInfo UnnamedParameter)
        {
            return Global.SpellMgr.GetSpellInfo(WarriorSpells.GLYPH_OF_MIGHTY_VICTORY, Difficulty.None) != null;
        }

        public void OnHit()
        {
            Unit caster = GetCaster();
            int heal = GetHitHeal();

            AuraEffect GlyphOfVictoryRush = caster.GetAuraEffect(WarriorSpells.GLYPH_OF_MIGHTY_VICTORY, 0);
            if (GlyphOfVictoryRush != null)
            {
                MathFunctions.AddPct(ref heal, GlyphOfVictoryRush.GetAmount());
            }

            SetHitHeal(heal);
        }
    }
}
