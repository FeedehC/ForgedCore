﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Paladin
{
    [SpellScript(204074)] // 204074 - Righteous Protector
    internal class spell_pal_righteous_protector : AuraScript, IHasAuraEffects
    {
        private SpellPowerCost _baseHolyPowerCost;
        public List<IAuraEffectHandler> AuraEffects { get; } = new();

        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(PaladinSpells.AvengingWrath, PaladinSpells.GuardianOfAcientKings);
        }

        public override void Register()
        {
            AuraEffects.Add(new AuraCheckEffectProcHandler(CheckEffectProc, 0, AuraType.Dummy));
            AuraEffects.Add(new AuraEffectProcHandler(HandleEffectProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
        }

        private bool CheckEffectProc(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            SpellInfo procSpell = eventInfo.GetSpellInfo();

            if (procSpell != null)
                _baseHolyPowerCost = procSpell.CalcPowerCost(PowerType.HolyPower, false, eventInfo.GetActor(), eventInfo.GetSchoolMask());
            else
                _baseHolyPowerCost = null;

            return _baseHolyPowerCost != null;
        }

        private void HandleEffectProc(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            double value = aurEff.Amount * 100 * _baseHolyPowerCost.Amount;

            GetTarget().GetSpellHistory().ModifyCooldown(PaladinSpells.AvengingWrath, TimeSpan.FromMilliseconds(-value));
            GetTarget().GetSpellHistory().ModifyCooldown(PaladinSpells.GuardianOfAcientKings, TimeSpan.FromMilliseconds(-value));
        }
    }
}
