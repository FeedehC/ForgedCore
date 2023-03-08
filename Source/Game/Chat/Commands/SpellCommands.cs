﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.DataStorage;
using Game.Entities;
using Game.Spells;

namespace Game.Chat
{
    class SpellCommands
    {
        [CommandNonGroup("cooldown", RBACPermissions.CommandCooldown)]
        static bool HandleCooldownCommand(CommandHandler handler, uint? spellIdArg)
        {
            Unit target = handler.GetSelectedUnit();
            if (!target)
            {
                handler.SendSysMessage(CypherStrings.PlayerNotFound);
                return false;
            }

            Player owner = target.CharmerOrOwnerPlayerOrPlayerItself;
            if (!owner)
            {
                owner = handler.GetSession().Player;
                target = owner;
            }

            string nameLink = handler.GetNameLink(owner);
            if (!spellIdArg.HasValue)
            {
                target.                SpellHistory.ResetAllCooldowns();
                target.                SpellHistory.ResetAllCharges();
                handler.SendSysMessage(CypherStrings.RemoveallCooldown, nameLink);
            }
            else
            {
                SpellInfo spellInfo = Global.SpellMgr.GetSpellInfo(spellIdArg.Value, target.Map.GetDifficultyID());
                if (spellInfo == null)
                {
                    handler.SendSysMessage(CypherStrings.UnknownSpell, owner == handler.GetSession().Player ? handler.GetCypherString(CypherStrings.You) : nameLink);
                    return false;
                }

                target.
                SpellHistory.ResetCooldown(spellInfo.Id, true);
                target.                SpellHistory.ResetCharges(spellInfo.ChargeCategoryId);
                handler.SendSysMessage(CypherStrings.RemoveallCooldown, spellInfo.Id, owner == handler.GetSession().Player ? handler.GetCypherString(CypherStrings.You) : nameLink);
            }
            return true;
        }

        [CommandNonGroup("aura", RBACPermissions.CommandAura)]
        static bool HandleAuraCommand(CommandHandler handler, uint spellId)
        {
            Unit target = handler.GetSelectedUnit();
            if (!target)
            {
                handler.SendSysMessage(CypherStrings.SelectCharOrCreature);

                return false;
            }

            SpellInfo spellInfo = Global.SpellMgr.GetSpellInfo(spellId, target.Map.GetDifficultyID());
            if (spellInfo == null)
                return false;

            ObjectGuid castId = ObjectGuid.Create(HighGuid.Cast, SpellCastSource.Normal, target.Location.MapId, spellId, target.Map.GenerateLowGuid(HighGuid.Cast));
            AuraCreateInfo createInfo = new(castId, spellInfo, target.Map.GetDifficultyID(), SpellConst.MaxEffectMask, target);
            createInfo.SetCaster(target);

            Aura.TryRefreshStackOrCreate(createInfo);

            return true;
        }

        [CommandNonGroup("unaura", RBACPermissions.CommandUnaura)]
        static bool HandleUnAuraCommand(CommandHandler handler, uint spellId = 0)
        {
            Unit target = handler.GetSelectedUnit();
            if (!target)
            {
                handler.SendSysMessage(CypherStrings.SelectCharOrCreature);

                return false;
            }

            if (spellId == 0)
            {
                target.RemoveAllAuras();
                return true;
            }

            SpellInfo spellInfo = Global.SpellMgr.GetSpellInfo(spellId, Difficulty.None);
            if (spellInfo != null)
            {
                target.RemoveAura(spellInfo.Id);
                return true;
            }

            return true;
        }

        [CommandNonGroup("setskill", RBACPermissions.CommandSetskill)]
        static bool HandleSetSkillCommand(CommandHandler handler, uint skillId, uint level, uint? maxSkillArg)
        {
            Player target = handler.GetSelectedPlayerOrSelf();
            if (!target)
            {
                handler.SendSysMessage(CypherStrings.NoCharSelected);
                return false;
            }

            SkillLineRecord skillLine = CliDB.SkillLineStorage.LookupByKey(skillId);
            if (skillLine == null)
            {
                handler.SendSysMessage(CypherStrings.InvalidSkillId, skillId);
                return false;
            }

            bool targetHasSkill = target.GetSkillValue((SkillType)skillId) != 0;

            // If our target does not yet have the skill they are trying to add to them, the chosen level also becomes
            // the max level of the new profession.
            ushort max = (ushort)maxSkillArg.GetValueOrDefault(targetHasSkill ? target.GetPureMaxSkillValue((SkillType)skillId) : level);

            if (level == 0 || level > max)
                return false;

            // If the player has the skill, we get the current skill step. If they don't have the skill, we
            // add the skill to the player's book with step 1 (which is the first rank, in most cases something
            // like 'Apprentice <skill>'.
            target.SetSkill((SkillType)skillId, (uint)(targetHasSkill ? target.GetSkillStep((SkillType)skillId) : 1), level, max);
            handler.SendSysMessage(CypherStrings.SetSkill, skillId, skillLine.DisplayName[handler.GetSessionDbcLocale()], handler.GetNameLink(target), level, max);
            return true;
        }
    }
}
