﻿// Copyright (c) CypherCore <http://github.com/CypherCore> All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Maps;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;

namespace Scripts.Spells.Warlock
{
    // Eye Laser - 205231
    [SpellScript(205231)]
    public class spell_warl_eye_laser_SpellScript : SpellScript, IHasSpellEffects
    {
        public List<ISpellEffect> SpellEffects { get; } = new();

        public void HandleTargets(List<WorldObject> targets)
        {
            Unit caster = GetOriginalCaster();

            if (caster == null)
                return;

            targets.RemoveAll(new UnitAuraCheck<WorldObject>(false, SpellIds.DOOM, caster.GetGUID()));
        }

        public override void Register()
        {
            SpellEffects.Add(new ObjectAreaTargetSelectHandler(HandleTargets, 0, Targets.UnitTargetEnemy));
        }
    }
}