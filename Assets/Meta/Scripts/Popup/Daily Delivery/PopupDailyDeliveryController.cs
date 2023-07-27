using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupDailyDeliveryController : MonoBehaviour
    {
        public Blackboard rootBlackboard;
        public Animator rootAnimator;

        public string biContextID;

        private Blackboard dailyDeliveryBB;

        private List<Blackboard> rewardList;
        private List<int> claimedRewardIdxList;
        private int nextClaimRewardIdx;

        private ContextElement rootElement;
        private ContextElement dailyListElement;
        private ContextElement buttonOkElement;
        private ContextElement buttonTextElement;
        private ContextElement titleTextElement;
        private ContextElement imageDailyDelivery;
        private ContextElement imageToday;
        private ContextElement imageTomorrow;

        private List<ContextElement> dailyDeliveryElementList;

        private bool isInit = false;
        private bool existTomorrow = false;

        public void OnInitDailyDelivery(Blackboard rewardInfoBB)
        {
            if(isInit) return;

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator   = gameObject.GetComponent<Animator>();

            // Init context. 
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            dailyListElement = ContextUtils.FindElement(rootElement, "Daily Layout", ContextSearchingType.ChildrenSearch);
            buttonOkElement = ContextUtils.FindElement(rootElement, "Button Collect", ContextSearchingType.ChildrenSearch);
            buttonTextElement = ContextUtils.FindElement(buttonOkElement, "Text", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(rootElement, "Title Area/Text", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetText(titleTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_DAILY_DELIVERY_TITLE") );

            imageDailyDelivery = ContextUtils.FindElement(rootElement, "Image Daily Delivery", ContextSearchingType.ChildrenSearch);
            imageToday = ContextUtils.FindElement(imageDailyDelivery, "Anchor Before", ContextSearchingType.ChildrenSearch);
            imageTomorrow = ContextUtils.FindElement(imageDailyDelivery, "Anchor After", ContextSearchingType.ChildrenSearch);

            rewardList = rewardInfoBB.GetValue<List<Blackboard>>("rewardList");
            claimedRewardIdxList = rewardInfoBB.GetValue<List<int>>("claimedRewardIdxList");
            nextClaimRewardIdx = rewardInfoBB.GetValue<int>("nextClaimRewardIdx");
            
            dailyDeliveryElementList = new List<ContextElement>();

            for (int i = 0; i < rewardList.Count; ++i)
            {
                GameObject dailyListCell = MetaIconUtils.MakeDailyListCellObject(i, dailyListElement.transform, null);
                ContextElement dailyListCellElement = dailyListCell.GetComponent<ContextElement>();
                dailyListCellElement.UpdateContext(false);

                ContextElement cellTextElement = ContextUtils.FindElement(dailyListCellElement, "Day Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(cellTextElement, (i+1).ToString());
                dailyDeliveryElementList.Add(dailyListCell.GetComponent<ContextElement>());

                var elementAnimator = dailyListCell.GetComponent<Animator>();

                bool isScaleUp = false;
                bool isGet     = false;
                bool isMiss    = false;

                if(i == nextClaimRewardIdx)
                {
                    // Tomorrow.
                    existTomorrow = true;
                    UpdateStaticValue(rewardList[i], imageTomorrow);
                }
                else if (i + 1 == nextClaimRewardIdx)
                {
                    // Today.
                    isScaleUp = true;

                    UpdateStaticValue(rewardList[i], imageToday);
                }
                else if (i + 1 < nextClaimRewardIdx)
                {
                    // Previous.
                    if (claimedRewardIdxList.Contains(i))
                    {
                        isGet = true;
                    }
                    else
                    {
                        isMiss = true;
                    }
                }

                elementAnimator.SetBool("Scale Up", isScaleUp);
                elementAnimator.SetBool("Get", isGet);
                elementAnimator.SetBool("Miss", isMiss);
            }

            MetaContextElementUtils.SetText(buttonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK") );
            MetaContextElementUtils.SetClickable(
                buttonOkElement,
                existTomorrow ? "OnShowTomorrow" : "OnOk",
                rootElement,
                null
            );

            // Back Button Event. 
            GraphOwner owner = GetComponent<GraphOwner>();
            MetaSystem.SubscribeBackButton(owner.GetHashCode(), () => { owner.SendEvent(existTomorrow ? "OnShowTomorrow" : "OnOk"); });

            isInit = true;
        }

        private void OnDestroy()
        {
            if(MetaSystem.Instance != null)
            {
                GraphOwner owner = GetComponent<GraphOwner>();
                MetaSystem.UnSubscribeBackButton(owner.GetHashCode());
            }
        }

        public void UpdateTodayValues()
        {
            var dailyListCellBefore = dailyDeliveryElementList[nextClaimRewardIdx - 1];
            var elementAnimatorBefore = dailyListCellBefore.gameObject.GetComponent<Animator>();
            elementAnimatorBefore.SetBool("Scale Up", false);
            elementAnimatorBefore.SetBool("Get", true);
            elementAnimatorBefore.SetBool("Miss", false);
        }

        public void ChangeState()
        {
            var imageAnimator = imageDailyDelivery.GetComponent<Animator>();
            imageAnimator.SetTrigger("Change");
            MetaContextElementUtils.SetText(buttonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_DAILY_DELIVERY_TOMORROW_BUTTON_TEXT") );

            var dailyListCellAfter = dailyDeliveryElementList[nextClaimRewardIdx];
            var elementAnimatorAfter = dailyListCellAfter.gameObject.GetComponent<Animator>();
            elementAnimatorAfter.SetBool("Scale Up", true);
        }

        private void UpdateStaticValue(Blackboard rewardInfo, ContextElement rewardElement)
        {
            ContextElement iconAreaElement = ContextUtils.FindElement(rewardElement, "Icon Area", ContextSearchingType.ChildrenSearch);
            ContextElement textElement = ContextUtils.FindElement(rewardElement, "Text", ContextSearchingType.ChildrenSearch);
            var rewardType = rewardInfo.GetValue<RewardType>("type");
            bool isMultiline = true;
            bool isInbox = true;

            MetaCommonRewardUtils.CommonRewardResultSetter(iconAreaElement, textElement, rewardInfo, rewardType, isInbox, isMultiline);
        }
    }
}