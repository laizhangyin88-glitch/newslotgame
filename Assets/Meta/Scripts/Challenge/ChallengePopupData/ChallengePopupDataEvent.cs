using System.Collections;
using System.Collections.Generic;
using BagelCode.OSA_Scroll;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

using BagelCode.ClientModels;

namespace BagelCode
{
    public class ChallengePopupDataEvent : ChallengePopupData
    {
        public ChallengePopupDataEvent(GameObject _owner, bool _isPersonal)
            : base(_owner)
        {
            isPersonal = _isPersonal;
        }

        public bool isPersonal; // not for club
        public bool isCompleteToUnlock;

        private MetaChallengeType EventChallengeType => isPersonal ? MetaChallengeType.EVENT_PERSONAL : MetaChallengeType.EVENT_CLUB;

        public Blackboard challengeInfo;

        private bool isNonClubber = false;

        private bool isFirstSelect = true;

        private ContextElement challengeElement;

        private ContextElement rewardInfoElement;
        private ContextElement rewardCreditElement;

        private ContextElement gaugeElement;
        private ContextElement progressCountElement;
        private ContextElement progressCountTextElement;
        private ContextElement remainingTimerElement;

        private ContextElement progressGlowImageElement;

        private ContextElement challengeInfoElement;
        private ContextElement missionCellListElement;

        private ContextElement nonClubberElement;
        private ContextElement joinClubTitleTextElement;
        private ContextElement joinClubButtonAreaElement;
        private ContextElement joinClubButtonElement;
        private ContextElement joinClubButtonTextElement;

        private ContextElement selectBaseElement;

        private List<Blackboard> missionList;

        //

        public override void InitProperty()
        {
            isMainTab = true;

            challengeElement = ContextUtils.FindElement(root, isPersonal ?
                "Challenge Event Personal Context" : "Challenge Event Club Context", CHILDREN);

            challengeInfoElement = ContextUtils.FindElement(challengeElement, "Challenge Info", CHILDREN);
            var rewardElement = ContextUtils.FindElement(challengeInfoElement, "Reward", CHILDREN);
            rewardInfoElement = ContextUtils.FindElement(rewardElement, "Info", CHILDREN);
            rewardCreditElement = ContextUtils.FindElement(rewardElement, "Credit", CHILDREN);

            missionCellListElement = ContextUtils.FindElement(challengeElement, "Mission List", CHILDREN);

            gaugeElement = ContextUtils.FindElement(challengeInfoElement, "Gauge", CHILDREN);
            progressCountElement = ContextUtils.FindElement(challengeInfoElement, "Gauge/Count", FULL);
            progressCountTextElement = ContextUtils.FindElement(progressCountElement, "Text", CHILDREN);
            remainingTimerElement = ContextUtils.FindElement(challengeElement, "Remaining Timer", CHILDREN);

            progressGlowImageElement = ContextUtils.FindElement(challengeElement, "Glow", CHILDREN);

            selectBaseElement = ContextUtils.FindElement(root, "Left Tab Base/Tab Event/Base Select", FULL);

            if (!isPersonal)
            {
                nonClubberElement = ContextUtils.FindElement(challengeElement, "Non Clubber", CHILDREN);
                joinClubTitleTextElement = ContextUtils.FindElement(nonClubberElement, "Text Top", FULL);
                joinClubButtonAreaElement = ContextUtils.FindElement(nonClubberElement, "Button Area", FULL);
                joinClubButtonElement = ContextUtils.FindElement(joinClubButtonAreaElement, "Button Join", FULL);
                joinClubButtonTextElement = ContextUtils.FindElement(joinClubButtonElement, "Text", FULL);

                MetaContextElementUtils.SetClickable(
                    joinClubButtonElement,
                    () => EventSender.SendEvent(owner, ChallengeEventManager.ON_GO_TO_CLUB));
            }
        }

        public override void ClearPopupData()
        {

        }

        public override bool IsComplete(MetaChallengeType tabType)
        {
            if (challengeInfo == null) return false;

            return challengeInfo.GetValue<bool>("done");
        }

        public override void OnSelectTab(MetaChallengeType tabType)
        {
            if (isPersonal && tabType != MetaChallengeType.EVENT_PERSONAL)
            {
                challengeElement.gameObject.SetActive(false);
                selectBaseElement?.gameObject.SetActive(false);
                return;
            }
            else if (!isPersonal && tabType != MetaChallengeType.EVENT_CLUB)
            {
                challengeElement.gameObject.SetActive(false);
                selectBaseElement?.gameObject.SetActive(false);
                return;
            }

            ChangeTab(EventChallengeType);
            selectBaseElement?.gameObject.SetActive(true);
            UpdateBadge();
            UpdatePopupData();
        }

        public override void UpdatePopupData()
        {
            root.StartCoroutine(UpdatePopupDataCoroutine());
        }

        public override void OnUpdateChallengeInfo()
        {
            UpdateChallengeInfo();
            UpdateBadge();
        }

        public override void UpdateChallengeInfo()
        {
            isUpdated = true;

            var responseInfo = rootBB.GetVariable<Blackboard>("challengeInfoResponse")?.value;
            challengeInfo = BlackboardQueryUtils.GetEventChallengeInfoBB(responseInfo, isPersonal);

            if(challengeInfo == null)
            {
                if(isPersonal)
                {
                    if (ApplicationSettings.LogTest())
                        Debug.LogWarning("ChallengePopupDataEvent.UpdateChallengeInfo failure. The challengeInfo is null.");
                    return;
                }
                else
                {
                    isNonClubber = true;
                    return;
                }
            }

            //bool isComplete = IsComplete(EventChallengeType);
            //badgeAnimatorDict[EventChallengeType].gameObject.SetActive(!isComplete);

            isCompleteToUnlock = challengeInfo.GetValue<bool>("isCompleteToUnlock");
            missionList = challengeInfo.GetValue<List<Blackboard>>("missionList");
        }

        private IEnumerator UpdatePopupDataCoroutine()
        {
            EventInfo eventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            if (eventInfo == null)
            {
                Debug.LogError("UpdatePopupDataCoroutine failure. The eventInfo is null.");
                yield break;
            }

            // Non Clubber
            if (isNonClubber)
            {
                challengeElement.gameObject.SetActive(true);
                nonClubberElement.gameObject.SetActive(true);

                progressGlowImageElement.gameObject.SetActive(false);
                challengeCompleteElement.gameObject.SetActive(false);
                challengeInfoElement.gameObject.SetActive(false);
                missionCellListElement.gameObject.SetActive(false);

                // Title & Button
                bool isInGame = BlackboardQueryUtils.IsIngame();
                if (isInGame)
                {
                    MetaContextElementUtils.SetTextGlobal(joinClubTitleTextElement, "POPUP_CHALLENGE_EVENT_CLUB_GO_LOBBY_TEXT");
                }
                else
                {
                    MetaContextElementUtils.SetTextGlobal(joinClubTitleTextElement, "POPUP_CHALLENGE_EVENT_CLUB_JOIN_TITLE_TEXT");
                    MetaContextElementUtils.SetTextGlobal(joinClubButtonTextElement, "POPUP_CHALLENGE_EVENT_CLUB_JOIN_BUTTON_TEXT");
                }

                joinClubTitleTextElement.gameObject.SetActive(true);
                joinClubButtonAreaElement.gameObject.SetActive(!isInGame);

                ChangeTab(EventChallengeType);

                BI_ClientChallengeEnter(null, "event", isAuto ? "auto" : "manual");

                yield break;
            }
            else if (!isPersonal)
            {
                nonClubberElement.gameObject.SetActive(false);
            }

            bool isComplete = IsComplete(EventChallengeType);

            challengeCompleteElement.gameObject.SetActive(isComplete);
            challengeCompleteNextTimerElement.gameObject.SetActive(isComplete);

            challengeElement.gameObject.SetActive(!isComplete);
            challengeInfoElement.gameObject.SetActive(!isComplete);
            missionCellListElement.gameObject.SetActive(!isComplete);
            progressGlowImageElement.gameObject.SetActive(!isComplete);
            remainingTimerElement.gameObject.SetActive(!isComplete);

            // Title
            string title = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_EVENT_DESCRIPTION");
            MetaContextElementUtils.SetText(rewardInfoElement, title);

            if (isComplete)
            {
                // Complete Title
                MetaContextElementUtils.SetTextGlobal(challengeCompleteTextElement, "POPUP_CHALLENGE_EVENT_COMPLETE");

                // Complete Reward
                var rewarResultListBB = challengeInfo.GetVariable<List<Blackboard>>("rewardList")?.value ?? new List<Blackboard>();
                string lastRewardCoinText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewarResultListBB, NumberUtils.GetGlobalDenominator());
                if (!isPersonal)
                {
                    var lp = challengeInfo.GetVariable<long>("leaguePoint")?.value ?? 0L;
                    if (lp > 0L) lastRewardCoinText += " + " + StringTableUtils.GetString(GLOBAL, "COMMA_LP", lp);
                }
                MetaContextElementUtils.SetText(challengeCompleteRewardTextElement, lastRewardCoinText);
                challengeCompleteRewardTextElement.gameObject.SetActive(true);

                // Complete Next Event Timer
                string completeTimerText = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_EVENT_WAIT_NEXT");
                MetaContextElementUtils.DisableRemainingTimer(challengeCompleteNextTimerElement, completeTimerText);
            }
            else
            {
                // Create Mission Cells
                if (isFirstSelect)
                {
                    // Wait for OSA_EventChallengeCells.Start() called
                    yield return new WaitForEndOfFrame();

                    isFirstSelect = false;
                    var missionCellListRectElement = ContextUtils.FindElement(missionCellListElement, "Scroll Rect", CHILDREN);
                    var rect = missionCellListRectElement.GetComponent<OSA_EventChallengeCells>();
                    rect.CreateItemList(owner, missionList, isCompleteToUnlock, isPersonal);
                }

                // Remaining Timer
                long endTimestamp = eventInfo.endTimestamp;
                MetaContextElementUtils.SetCommonRemainingTimer(remainingTimerElement, endTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, owner);

                // Progress
                int missionCount = missionList.Count;
                int progressCount = BlackboardQueryUtils.GetChallengeMissionCompleteCount(missionList);
                float progressRatio = (float)progressCount / missionCount;
                MetaContextElementUtils.SetSliderValue(gaugeElement, progressRatio);
                string progressText = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_PROGRESS", progressCount, missionCount);
                MetaContextElementUtils.SetText(progressCountTextElement, progressText);

                // Reward
                rewardCreditElement.gameObject.SetActive(true);
                var rewardResultListBB = challengeInfo.GetValue<List<Blackboard>>("rewardList");
                string rewardText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardResultListBB, NumberUtils.GetGlobalDenominator());
                if (!isPersonal)
                {
                    var lp = challengeInfo.GetVariable<long>("leaguePoint")?.value ?? 0L;
                    if (lp > 0L) rewardText += " + " + StringTableUtils.GetString(GLOBAL, "COMMA_LP", lp);
                }
                MetaContextElementUtils.SetText(rewardCreditElement, rewardText);
            }

            BI_ClientChallengeEnter(challengeInfo, "event", isAuto ? "auto" : "manual");
        }

        public override void UpdateBadge()
        {
            bool isComplete = IsComplete(EventChallengeType);
            completeElementDict[EventChallengeType].gameObject.SetActive(isComplete);

            isBadge = isComplete && (isPersonal ? !challengeInfo.GetValue<bool>("claimed") : false);

            badgeAnimatorDict[EventChallengeType]?.SetInteger("value", isBadge ? 1 : 0);
        }
    }
}
