using UnityEngine;
using SlotMaker;
using System.Collections.Generic;
using System;
using BagelCode.ClientModels;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.VegasDreams
{
    public class VegasDreamsDailyChestController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private List<Animator> depots = new List<Animator>();

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            anim = GetComponent<Animator>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            var index = bb.GetValue<int>("index");
            var isGiftPopup = bb.GetValue<bool>("isGiftPopup");
            
            MetaContextElementUtils.SimpleSetClickable(root, "Btn Claim", () =>
            {
                if (isGiftPopup)
                {
                    VegasDreamsAnalytics.click_button_vds("GIFT_CLAIM", "contextId");
                    var eventData = new EventData<int>(VegasDreams.Events.ON_CLICK_COLLECT_DAILY_CHEST_ON_GIFT_POPUP, index + 1);
                    EventSender.SendGlobalEvent(eventData);
                }
                else
                {
                    VegasDreamsAnalytics.click_button_vds("CHEST_CLAIM", "contextId");
                    EventSender.SendGlobalEvent(VegasDreams.Events.ON_CLICK_COLLECT_DAILY_CHEST);
                }
            });

            OnUpdateChest();
        }

        public void OnUpdateChest()
        {
            var chestList = VegasDreams.Utils.DailyChestList;
            if (chestList == null) return;

            var index = bb.GetValue<int>("index");

            foreach (var chest in chestList)
            {
                var buildingIndex = chest.GetValue<int>("buildingIndex");

                if (buildingIndex == index + 1)
                {
                    var lastCollectTimestamp = chest.GetValue<long>("lastCollectTimestamp");
                    var nextCollectTimestamp = lastCollectTimestamp + VegasDreams.Utils.DailyChestCooltime;

                    if (TimeUtils.GetTimeStamp() > nextCollectTimestamp)
                    {
                        anim.SetTrigger("Idle");
                    }
                    else
                    {
                        var timerElement = ContextUtils.FindElement(root, "Text Timer", CHILDREN);
                        var timer = timerElement.GetComponent<RemainingTimerController>();
                        timer.Init(timerElement, "TIME_FORMAT_HHMMSS", "TEXT_NORMAL", "", "Ended", true, OnUpdateChest);
                        timer.StartTimer(nextCollectTimestamp, 0);
                        anim.SetTrigger("Timer");
                    }

                    break;
                }
            }
        }
    }
}
