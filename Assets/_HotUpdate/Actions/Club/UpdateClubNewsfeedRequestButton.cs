using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
 {
     
     [Category("★ BagelCode/Club")]
     public class UpdateClubNewsfeedRequestButton : ActionTask<ContextElement>
     {
        public BBParameter<Blackboard> clubNewsfeedResponse;

        public BBParameter<bool> enableRequest;

        private ContextElement requestButtonElement;
        private ContextElement remainingTimerElement;

        private bool isInit = false;
        private GameObject dataObj = null;
         
        protected override string info
        {
            get { return "Update Club Newsfeed Request Button"; }
        }

        protected override void OnExecute()
        {
            if(enableRequest.value)
            {
                InitContext();
                UpdateVariables();
            }

            EndAction();
        }

        private void InitContext()
        {
            if(isInit) return;

            requestButtonElement = ContextUtils.FindElement(agent, "Post Input/Button Layout/Button Request", ContextSearchingType.FullNameSearch);
            remainingTimerElement = ContextUtils.FindElement(requestButtonElement, "Inactive/Remaining Timer", ContextSearchingType.FullNameSearch);

            isInit = true;
        }

        private void UpdateVariables()
        {
            MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Loading Area", false);

            long currentTimestamp = TimeUtils.GetTimeStamp();
            var metaGameRequestResetTimestamp = clubNewsfeedResponse.value.GetValue<long>("metaGameRequestResetTimestamp");

            if(currentTimestamp > metaGameRequestResetTimestamp)
            {
                MetaContextElementUtils.SetBooleanProperty(requestButtonElement, true);

                MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Active", true);
                MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Inactive", false);
            }
            else
            {
                MetaContextElementUtils.SetBooleanProperty(requestButtonElement, false);
                MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Active", false);
                MetaContextElementUtils.SimpleSetActive(requestButtonElement, "Inactive", true);

                MetaContextElementUtils.SetCommonRemainingTimer(
                    remainingTimerElement,
                    metaGameRequestResetTimestamp,
                    0,
                    "TIME_FORMAT_HHMMSS_TOTALHOUR",
                    "CLUB_NEWS_FEED_REQUEST_SHARE_ITEM_TIMER_OUTPUT",
                    null,
                    "Ended",
                    false,
                    agent.gameObject
                    );
            }
        }
     }
 }