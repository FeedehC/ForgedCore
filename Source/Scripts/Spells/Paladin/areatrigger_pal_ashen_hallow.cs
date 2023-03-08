﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Game.AI;
using Game.Entities;
using Game.Scripting;
using Game.Spells;

namespace Scripts.Spells.Paladin
{
    // 19042 - Ashen Hallow
    [Script]
    internal class areatrigger_pal_ashen_hallow : AreaTriggerAI
    {
        private TimeSpan _period;
        private TimeSpan _refreshTimer;

        public areatrigger_pal_ashen_hallow(AreaTrigger areatrigger) : base(areatrigger)
        {
        }

        public override void OnCreate()
        {
            RefreshPeriod();
            _refreshTimer = _period;
        }

        public override void OnUpdate(uint diff)
        {
            _refreshTimer -= TimeSpan.FromMilliseconds(diff);

            while (_refreshTimer <= TimeSpan.Zero)
            {
                Unit caster = at.GetCaster();

                if (caster != null)
                {
                    caster.CastSpell(at.Location, PaladinSpells.AshenHallowHeal, new CastSpellExtraArgs());
                    caster.CastSpell(at.Location, PaladinSpells.AshenHallowDamage, new CastSpellExtraArgs());
                }

                RefreshPeriod();

                _refreshTimer += _period;
            }
        }

        public override void OnUnitEnter(Unit unit)
        {
            if (unit.GetGUID() == at.GetCasterGuid())
                unit.CastSpell(unit, PaladinSpells.AshenHallowAllowHammer, true);
        }

        public override void OnUnitExit(Unit unit)
        {
            if (unit.GetGUID() == at.GetCasterGuid())
                unit.RemoveAura(PaladinSpells.AshenHallowAllowHammer);
        }

        private void RefreshPeriod()
        {
            Unit caster = at.GetCaster();

            if (caster != null)
            {
                AuraEffect ashen = caster.GetAuraEffect(PaladinSpells.AshenHallow, 1);

                if (ashen != null)
                    _period = TimeSpan.FromMilliseconds(ashen.Period);
            }
        }
    }
}
