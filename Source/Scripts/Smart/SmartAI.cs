﻿// Copyright (c) CypherCore <http://github.com/CypherCore> All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE. See LICENSE file in the project root for full license information.

using Framework.Constants;
using Game;
using Game.AI;
using Game.Entities;
using Game.Scripting;
using Game.DataStorage;
using Game.Scripting.Interfaces.IAreaTrigger;
using Game.Scripting.Interfaces.IAreaTriggerEntity;
using Game.Scripting.Interfaces.IScene;
using Game.Scripting.Interfaces.IQuest;

namespace Scripts.Smart
{
    [Script]
    class SmartTrigger : ScriptObjectAutoAddDBBound, IAreaTriggerOnTrigger
    {
        public SmartTrigger() : base("SmartTrigger") { }

        public bool OnTrigger(Player player, AreaTriggerRecord trigger)
        {
            if (!player.IsAlive())
                return false;

            Log.outDebug(LogFilter.ScriptsAi, "AreaTrigger {0} is using SmartTrigger script", trigger.Id);
            SmartScript script = new();
            script.OnInitialize(player, trigger);
            script.ProcessEventsFor(SmartEvents.AreatriggerOntrigger, player, trigger.Id);
            return true;
        }
    }

    [Script]
    class SmartAreaTriggerEntityScript : ScriptObjectAutoAddDBBound, IAreaTriggerEntityGetAI
    {
        public SmartAreaTriggerEntityScript() : base("SmartAreaTriggerAI") { }

        public AreaTriggerAI GetAI(AreaTrigger areaTrigger)
        {
            return new SmartAreaTriggerAI(areaTrigger);
        }
    }

    [Script]
    class SmartScene : ScriptObjectAutoAddDBBound, ISceneOnSceneStart, ISceneOnSceneTrigger, ISceneOnSceneChancel, ISceneOnSceneComplete
    {
        public SmartScene() : base("SmartScene") { }

        public void OnSceneStart(Player player, uint sceneInstanceID, SceneTemplate sceneTemplate)
        {
            SmartScript smartScript = new();
            smartScript.OnInitialize(null, null, sceneTemplate);
            smartScript.ProcessEventsFor(SmartEvents.SceneStart, player);
        }

        public void OnSceneTriggerEvent(Player player, uint sceneInstanceID, SceneTemplate sceneTemplate, string triggerName)
        {
            SmartScript smartScript = new();
            smartScript.OnInitialize(null, null, sceneTemplate);
            smartScript.ProcessEventsFor(SmartEvents.SceneTrigger, player, 0, 0, false, null, null, triggerName);
        }

        public void OnSceneCancel(Player player, uint sceneInstanceID, SceneTemplate sceneTemplate)
        {
            SmartScript smartScript = new();
            smartScript.OnInitialize(null, null, sceneTemplate);
            smartScript.ProcessEventsFor(SmartEvents.SceneCancel, player);
        }

        public void OnSceneComplete(Player player, uint sceneInstanceID, SceneTemplate sceneTemplate)
        {
            SmartScript smartScript = new();
            smartScript.OnInitialize(null, null, sceneTemplate);
            smartScript.ProcessEventsFor(SmartEvents.SceneComplete, player);
        }
    }

    [Script]
    class SmartQuest : ScriptObjectAutoAddDBBound, IQuestOnQuestStatusChange, IQuestOnQuestObjectiveChange
    {
        public SmartQuest() : base("SmartQuest") { }

        // Called when a quest status change
        public void OnQuestStatusChange(Player player, Quest quest, QuestStatus oldStatus, QuestStatus newStatus)
        {
            SmartScript smartScript = new();
            smartScript.OnInitialize(null, null, null, quest);
            switch (newStatus)
            {
                case QuestStatus.Incomplete:
                    smartScript.ProcessEventsFor(SmartEvents.QuestAccepted, player);
                    break;
                case QuestStatus.Complete:
                    smartScript.ProcessEventsFor(SmartEvents.QuestCompletion, player);
                    break;
                case QuestStatus.Failed:
                    smartScript.ProcessEventsFor(SmartEvents.QuestFail, player);
                    break;
                case QuestStatus.Rewarded:
                    smartScript.ProcessEventsFor(SmartEvents.QuestRewarded, player);
                    break;
                case QuestStatus.None:
                default:
                    break;
            }
        }

        // Called when a quest objective data change
        public void OnQuestObjectiveChange(Player player, Quest quest, QuestObjective objective, int oldAmount, int newAmount)
        {
            ushort slot = player.FindQuestSlot(quest.Id);
            if (slot < SharedConst.MaxQuestLogSize && player.IsQuestObjectiveComplete(slot, quest, objective))
            {
                SmartScript smartScript = new();
                smartScript.OnInitialize(null, null, null, quest);
                smartScript.ProcessEventsFor(SmartEvents.QuestObjCompletion, player, objective.Id);
            }
        }
    }
}
