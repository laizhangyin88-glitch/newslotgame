using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.OSA_Scroll;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class ChallengePopupDataDailyTab : ChallengePopupData
    {
        private MetaChallengeType currentChallengeType;
        private ContextElement eventTagElement;
        private ContextElement eventTagTextElement;

        private ContextElement selectBaseElement;

        // Daily left tab 
        public ChallengePopupDataDailyTab(GameObject _owner)
            : base(_owner) { }

        public override void InitProperty()
        {
            isMainTab = true;

            ContextElement leftTabElement = ContextUtils.FindElement(root, "Left Tab Base", CHILDREN);
            ContextElement dailyTabElement = ContextUtils.FindElement(leftTabElement, "Tab Normal", CHILDREN);

            eventTagElement = ContextUtils.FindElement(dailyTabElement, "Event Tag Area", CHILDREN);
            eventTagTextElement = ContextUtils.FindElement(eventTagElement, "Text", CHILDREN);

            selectBaseElement = ContextUtils.FindElement(dailyTabElement, "Base Select", CHILDREN);
        }

        public override bool IsComplete(MetaChallengeType tabType)
        {
            return true;
        }

        public override void OnUpdateChallengeInfo()
        {
            UpdateChallengeInfo();
            UpdateBadge();
        }

        public override void UpdateChallengeInfo()
        {
            isUpdated = true;

            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
            if (eventInfo != null)
            {
                eventTagElement.gameObject.SetActive(true);

                double multipler = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                MetaContextElementUtils.SetTextGlobal(eventTagTextElement, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", multipler);
            }
            else
                eventTagElement.gameObject.SetActive(false);
        }

        public override void OnSelectTab(MetaChallengeType tabType)
        {
            if (tabType != MetaChallengeType.NORMAL)
            {
                if (tabType != MetaChallengeType.DAILY &&
                    tabType != MetaChallengeType.EXPERT &&
                    tabType != MetaChallengeType.MASTER &&
                    tabType != MetaChallengeType.CLUB &&
                    tabType != MetaChallengeType.EVENT_PERSONAL &&
                    tabType != MetaChallengeType.EVENT_CLUB)
                {
                    selectBaseElement?.gameObject.SetActive(false);
                    return;
                }
            }

            currentChallengeType = tabType;
            ChangeTab(tabType);
            selectBaseElement?.gameObject.SetActive(true);
            UpdateBadge();
            UpdatePopupData();
        }

        public override void UpdatePopupData()
        {
            ChallengeUtils.MetaChallengeTypeToChallengeType(currentChallengeType, out ChallengeType personalChallengeType, out ClubChallengeType clubChallengeType);
        }

        public override void UpdateBadge()
        {
            Blackboard responseInfo = rootBB.GetVariable<Blackboard>("challengeInfoResponse")?.value ?? null;
            if (responseInfo == null)
                return;

            var challengeInfoList = responseInfo.GetVariable<List<Blackboard>>("challengeInfoList")?.value;
            if (challengeInfoList != null)
            {
                foreach (var info in challengeInfoList)
                {
                    var cType = info.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;

                    ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(cType, out MetaChallengeType challengeType);
                    bool isDone = info.GetValue<bool>("done");
                    if (isDone)
                    {
                        Variable<bool> claimed = info.GetVariable<bool>("claimed");
                        if (claimed != null && claimed.value == true)
                            continue;

                        isBadge = true;
                        break;
                    }
                    isBadge = false;
                }
                badgeAnimatorDict[MetaChallengeType.NORMAL]?.SetInteger("value", IsBadge ? 1 : 0);
            }
        }
    }
}
