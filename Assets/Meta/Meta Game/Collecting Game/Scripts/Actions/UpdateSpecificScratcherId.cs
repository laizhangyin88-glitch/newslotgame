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
    public class UpdateSpecificScratcherId : ActionTask<ContextElement>
    {
        public BBParameter<int> nextScratcherId;
        public BBParameter<int> currentScratcherId;
        public BBParameter<bool> update;

        protected override string info
        {
            get { return String.Format("Update Scratcher Id to {0}", nextScratcherId); }
        }

        protected override void OnExecute()
        {
            int currentScratcherIndex = BlackboardQueryUtils.GetIndexOfScratcher(currentScratcherId.value);
            int nextScratcherIndex = BlackboardQueryUtils.GetIndexOfScratcher(nextScratcherId.value);

            ContextElement currentTab = ContextUtils.FindElement(agent, string.Format("Tab Base Area/Tab Scratcher {0}", currentScratcherIndex + 1), ContextSearchingType.FullNameSearch);
            ContextElement nextTab = ContextUtils.FindElement(agent, string.Format("Tab Base Area/Tab Scratcher {0}", currentScratcherIndex + 1), ContextSearchingType.FullNameSearch);
            
            currentTab.GetComponent<Animator>().SetBool("Active", false);
            nextTab.GetComponent<Animator>().SetBool("Active", true);

            if (currentScratcherIndex != nextScratcherIndex)
            {
                agent.GetComponent<Animator>().SetTrigger("Change");
                update.value = true;
            }
            else
                update.value = false;

            if (update.value)
                GSManager.Instance.GetHandler("UI_Button_Normal").Play();
            
            currentScratcherId.value = nextScratcherId.value;
            SendBIEvent();
            EndAction();
        }
        
        private void SendBIEvent()
        {
            var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
            
            string eventName = "client_click_scratcher_page";
            var eventData = new Dictionary<string, object>();
            
            eventData["type"] = "click_tab";
            
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