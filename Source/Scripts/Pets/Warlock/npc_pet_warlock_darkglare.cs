﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.ICreature;
using Game.Spells;
using Scripts.Spells.Warlock;

namespace Scripts.Pets
{
    namespace Warlock
    {
        // Darkglare - 103673
        [CreatureScript(103673)]
        public class npc_pet_warlock_darkglare : SmartAI
        {
            public npc_pet_warlock_darkglare(Creature creature) : base(creature)
            {
                Unit owner = me.GetOwner();

                if (owner == null)
                    return;

                creature.SetLevel(owner.GetLevel());
                creature.UpdateLevelDependantStats();
                creature.SetReactState(ReactStates.Assist);
                creature.SetCreatorGUID(owner.GetGUID());

                var summon = creature.ToTempSummon();


                if (summon != null)
                {
                    summon.SetCanFollowOwner(true);
                    summon.GetMotionMaster().Clear();
                    summon.GetMotionMaster().MoveFollow(owner, SharedConst.PetFollowDist, summon.GetFollowAngle());
                }
            }

            public override void UpdateAI(uint UnnamedParameter)
            {
                Unit owner = me.GetOwner();

                if (owner == null)
                    return;

                var target = me.GetAttackerForHelper();

                if (target != null)
                {
                    target.RemoveAura(WarlockSpells.DOOM, owner.GetGUID());
                    me.CastSpell(target, WarlockSpells.EYE_LASER, new CastSpellExtraArgs(TriggerCastFlags.None).SetOriginalCaster(owner.GetGUID()));
                }
            }
        }
    }
}