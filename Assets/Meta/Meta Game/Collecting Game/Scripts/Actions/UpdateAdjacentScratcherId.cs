using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateAdjacentScratcherId : ActionTask<Animator>
    {
        public BBParameter<bool> right;
        public BBParameter<int> currentScratcherId;

        protected override string info
        {
            get { return String.Format("Update to {0} Scratcher Id", right.value ? "RIGHT" : "LEFT"); }
        }

        protected override void OnExecute()
        {
            int adjacentScratcherId = BlackboardQueryUtils.GetAdjacentScratcherId(currentScratcherId.value, right.value);
            if (adjacentScratcherId != -1)
                currentScratcherId.value = adjacentScratcherId;
            
            agent.SetTrigger(right.value ? "PageChangeRight" : "PageChangeLeft");
            SendBIEvent();
            EndAction();
        }
        
        private void SendBIEvent()
        {
            var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
            
            string eventName = "client_click_scratcher_page";
            var eventData = new Dictionary<string, object>();

            eventData["type"] = "click_arrow";
            
            if (eventInfo != null)
            {
                eventData["collecting_game_event_id"] = eventInfo.id;
                eventData["collecting_game_id"] = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
            }

            eventData["scratcher_id"] = currentScratcherId.value;
            eventData["scratcher_preset_id"] = BlackboardQueryUtils.GetScratcher(currentScratcherId.value).GetValue<Blackboard>("reward").GetValue<int>("scratcherPresetId");
            eventData["scratcher_type"] = BlackboardQueryUtils.GetScratcher(currentScratcherId.value).GetValue<Blackboard>("reward").GetValue<RewardScratcherRule>("scratcherRule").ToString();
            
            Analytics.CustomEvent(eventName, eventData);
        }
    }
}