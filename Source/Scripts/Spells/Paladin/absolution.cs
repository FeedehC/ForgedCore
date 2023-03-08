﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IPlayer;
using Game.Spells;

namespace Scripts.Spells.Paladin
{
    //212056
    [Script]
    public class absolution : ScriptObjectAutoAdd, IPlayerOnSpellCast
    {
        public Class PlayerClass { get; } = Class.Paladin;
        public absolution() : base("absolution")
        {
        }

        public void OnSpellCast(Player player, Spell spell, bool skipCheck)
        {
            if (player.GetClass() != Class.Paladin)
                return;

            uint absolution = 212056;

            if (spell.SpellInfo.Id == absolution)
            {
                List<Unit> allies = new List<Unit>();
                player.GetFriendlyUnitListInRange(allies, 30.0f, false);
                foreach (var targets in allies)
                {
                    if (targets.IsDead())
                    {
                        Player playerTarget = targets.ToPlayer();
                        if (playerTarget != null)
                        {
                            playerTarget.ResurrectPlayer(0.35f, false);
                        }
                    }
                }
            }
        }
    }
}
