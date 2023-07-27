using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    // Normal: Daily, Expert, Master
    public class ChallengePopupDataNormal : ChallengePopupData
    {
        public ChallengePopupDataNormal(GameObject _owner)
            : base(_owner) { }

        private Dictionary<MetaChallengeType, Blackboard> challengeInfoDict = new Dictionary<MetaChallengeType, Blackboard>();

        private MetaChallengeType currentChallengeType;
        private MetaChallengeType prevChallengeType;

        private ContextElement challengeElement;
        private ContextElement rewardInfoElement;
        private ContextElement progressCountElement;
        private ContextElement progressCountTextElement;
        private ContextElement remainingTimer;
        private ContextElement rewardCreditElement;
        private ContextElement eventCreditAnchorElement;
        private ContextElement eventCreditTextElement;
        private ContextElement eventTagElement;
        private ContextElement eventTagTextElement;
        private ContextElement eventTimerElement;

        private List<ContextElement> missionGaugeList;
        private List<GameObject> missionCellList;

        //

        public override void InitProperty()
        {
            isMainTab = false;

            challengeElement = ContextUtils.FindElement(root, "Challenge Context", CHILDREN);

            var challengeInfoElement = ContextUtils.FindElement(challengeElement, "Challenge Info", CHILDREN);
            var rewardElement = ContextUtils.FindElement(challengeInfoElement, "Reward", CHILDREN);
            rewardInfoElement = ContextUtils.FindElement(rewardElement, "Info", CHILDREN);
            rewardCreditElement = ContextUtils.FindElement(rewardElement, "Credit", CHILDREN);
            eventCreditAnchorElement = ContextUtils.FindElement(rewardElement, "Credit Event Anchor", CHILDREN);
            eventCreditTextElement = ContextUtils.FindElement(eventCreditAnchorElement, "Credit", CHILDREN);
            eventTagElement = ContextUtils.FindElement(eventCreditAnchorElement, "Event Tag", CHILDREN);
            eventTagTextElement = ContextUtils.FindElement(eventTagElement, "Text", CHILDREN);
            eventTimerElement = ContextUtils.FindElement(eventTagElement, "Remaining Timer", CHILDREN);

            progressCountElement = ContextUtils.FindElement(challengeInfoElement, "Gauge/Count", FULL);
            progressCountTextElement = ContextUtils.FindElement(progressCountElement, "Text", CHILDREN);

            remainingTimer = ContextUtils.FindElement(challengeElement, "Remaining Timer", CHILDREN);

            var missionListElement = ContextUtils.FindElement(challengeElement, "Mission List", CHILDREN);

            if (missionCellList != null)
                missionCellList.ForEach(c => c.DestroyThis());

            missionCellList = new List<GameObject>();
            for (int i = 0; i < 8; ++i)
            {
                var cellObj = MetaObjectUtils.MakeScene(MetaStringDefine.LOBBY_BUNDLE_NAME, "Mission Cell Scene", missionListElement.transform, null);

                var cellBB = cellObj.GetComponent<Blackboard>();
                if (cellBB != null)
                {
                    var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(cellBB, "caller");
                    variable.value = owner;
                }

                cellObj.SetActive(false);
                missionCellList.Add(cellObj);
            }   

            missionGaugeList = new List<ContextElement>();
            for (int i = 0; i < 6; ++i)
                missionGaugeList.Add(ContextUtils.FindElement(challengeInfoElement, string.Format("Gauge/{0:00}", i + 1), FULL));
        }

        public override void ClearPopupData()
        {
            if (missionCellList != null)
                missionCellList.ForEach(c => c.DestroyThis());
        }

        public override bool IsComplete(MetaChallengeType challengeType)
        {
            if (!challengeInfoDict.ContainsKey(challengeType))
                return false;

            var challengeInfo = challengeInfoDict[challengeType];

            long startTimestamp = challengeInfo.GetValue<long>("startTimestamp");
            startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);

            ChallengeUtils.MetaChallengeTypeToChallengeType(challengeType, out ChallengeType personalChallengeType, out ClubChallengeType clubChallengeType);
            int minCount = BlackboardQueryUtils.GetChallengeMinCount(personalChallengeType);

            List<Blackboard> missionList = challengeInfo.GetValue<List<Blackboard>>("missionList");
            int completeCount = BlackboardQueryUtils.GetChallengeMissionCompleteCount(missionList);

            return TimeUtils.GetTimeStamp() < startTimestamp || completeCount >= minCount;
        }

        public override void OnUpdateChallengeInfo()
        {
            UpdateChallengeInfo();

            foreach(var info in challengeInfoDict.Values)
            {
                var personalChallengeType = info.GetValue<ChallengeType>("challengeType");

                ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(personalChallengeType, out MetaChallengeType challengeType);

                bool isComplete = IsComplete(challengeType);
                bool isDone = info.GetValue<bool>("done");

                badgeAnimatorDict[challengeType]?.SetInteger("value", isDone ? 1 : 0);

                completeElementDict[challengeType].gameObject.SetActive(isComplete);
            }

            UpdateBadge();
        }

        public override void UpdateChallengeInfo()
        {
            isUpdated = true;

            challengeInfoDict.Clear();

            Blackboard responseInfo = rootBB.GetVariable<Blackboard>("challengeInfoResponse")?.value;
            if (responseInfo != null)
            {
                var challengeInfoList = responseInfo.GetVariable<List<Blackboard>>("challengeInfoList")?.value;
                if (challengeInfoList != null)
                {
                    foreach (var info in challengeInfoList)
                    {
                        var cType = info.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
                        if (cType == ChallengeType.EVENT) continue;

                        ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(cType, out MetaChallengeType challengeType);
                        challengeInfoDict.Add(challengeType, info);
                    }
                }
            }
            else
            {
                Debug.LogWarning("ChallengePopupDataNormal.UpdateChallengeInfo failure. challengeInfoResponse is null");
                return;
            }
        }

        public override void OnSelectTab(MetaChallengeType tabType)
        {
            if (tabType != MetaChallengeType.DAILY && tabType != MetaChallengeType.EXPERT && tabType != MetaChallengeType.MASTER)
            {
                challengeElement.gameObject.SetActive(false);
                prevChallengeType = tabType;
                return;
            }

            currentChallengeType = tabType;
            ChangeTab(currentChallengeType);
            UpdateBadge();
            UpdatePopupData();
        }

        public override void UpdatePopupData()
        {
            ChallengeUtils.MetaChallengeTypeToChallengeType(currentChallengeType, out ChallengeType personalChallengeType, out ClubChallengeType clubChallengeType);

            if (!challengeInfoDict.ContainsKey(currentChallengeType))
                return;

            var challengeInfo = challengeInfoDict[currentChallengeType];

            // Progress
            var missionList = challengeInfo.GetValue<List<Blackboard>>("missionList");
            int minCount = BlackboardQueryUtils.GetChallengeMinCount(personalChallengeType);
            int progressCount = System.Math.Min(BlackboardQueryUtils.GetChallengeMissionCompleteCount(missionList), minCount);
            string progressText = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_PROGRESS", progressCount, minCount);
            UpdateGuageProgress(missionGaugeList, progressCountElement, progressCount);
            MetaContextElementUtils.SetText(progressCountTextElement, progressText);

            // Title
            string title = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_DESCRIPTION", minCount);
            MetaContextElementUtils.SetText(rewardInfoElement, title);

            // Reward
            var rewardResultListBB = challengeInfo.GetValue<List<Blackboard>>("rewardList");
            long challengeClaimEventMultiplierNumerator = NumberUtils.GetGlobalDenominator();
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
            if (eventInfo != null) challengeClaimEventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            string rewardCoinText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardResultListBB, NumberUtils.GetGlobalDenominator());
            string eventRewardCoinText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardResultListBB, challengeClaimEventMultiplierNumerator);
            MetaContextElementUtils.SetText(rewardCreditElement, rewardCoinText);
            MetaContextElementUtils.SetText(eventCreditTextElement, eventRewardCoinText);

            // Complete
            string completeTitleTextKey = string.Format("POPUP_CHALLENGE_WAIT_TITLE_{0}", personalChallengeType.ToString());
            MetaContextElementUtils.SetTextGlobal(challengeCompleteTextElement, completeTitleTextKey);

            // Complete Reward
            var lastRewarResultListBB = challengeInfo.GetVariable<List<Blackboard>>("lastRewardResultList")?.value ?? new List<Blackboard>();
            string lastRewardCoinText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(lastRewarResultListBB, NumberUtils.GetGlobalDenominator());
            MetaContextElementUtils.SetText(challengeCompleteRewardTextElement, lastRewardCoinText);
            challengeCompleteRewardTextElement.gameObject.SetActive(true);

            long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfo, "startTimestamp").value;
            startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);

            long endTimestamp = BlackboardUtils.FindVariable<long>(challengeInfo, "endTimestamp").value;
            endTimestamp = TimeUtils.ApplyTimeZoneOffset(endTimestamp);
            
            // Remaining Timer
            if (IsComplete(currentChallengeType))
            {
                // It is future. complete previous missions.
                challengeCompleteElement.gameObject.SetActive(true);
                challengeElement.gameObject.SetActive(false);

                MetaContextElementUtils.SetCommonRemainingTimer(challengeCompleteNextTimerElement, startTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", "POPUP_CHALLENGE_NEXT_TIMESTAMP", null, null, true, owner);
                challengeCompleteNextTimerElement.gameObject.SetActive(true);
            }
            else
            {
                challengeCompleteElement.gameObject.SetActive(false);
                challengeElement.gameObject.SetActive(true);

                MetaContextElementUtils.SetCommonRemainingTimer(remainingTimer, endTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, owner);

                for (int i = 0; i < missionCellList.Count; ++i)
                {
                    if (missionList.Count <= i)
                        break;

                    var cellBB = missionCellList[i].gameObject.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "missionInfo", missionList[i]);
                    BlackboardUtils.SetOrCreateValue<ChallengeType>(cellBB, "challengeType", personalChallengeType);
                    BlackboardUtils.SetOrCreateValue<int>(cellBB, "challengeIndex", ((int)personalChallengeType) - 1);
                    BlackboardUtils.SetOrCreateValue<bool>(cellBB, "updateInfo", true);

                    missionCellList[i].gameObject.SetActive(true);
                }

                if (eventInfo != null)
                {
                    rewardCreditElement.gameObject.SetActive(false);
                    eventCreditAnchorElement.gameObject.SetActive(true);

                    var multipler = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                    MetaContextElementUtils.SetTextGlobal(eventTagTextElement, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", multipler);

                    MetaContextElementUtils.SetCommonRemainingTimer(eventTimerElement, eventInfo.endTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, owner);
                }
                else
                {
                    rewardCreditElement.gameObject.SetActive(true);
                    eventCreditAnchorElement.gameObject.SetActive(false);
                }
            }
            if (prevChallengeType != currentChallengeType)
            {
                BI_ClientChallengeEnter(challengeInfo, personalChallengeType.ToString().ToLower(), isAuto ? "auto" : "manual");
                prevChallengeType = currentChallengeType;
            }
        }

        public override void UpdateBadge()
        {
            foreach (var info in challengeInfoDict.Values)
            {
                var personalChallengeType = info.GetValue<ChallengeType>("challengeType");

                ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(personalChallengeType, out MetaChallengeType challengeType);
                bool isDone = info.GetValue<bool>("done");
                badgeAnimatorDict[challengeType]?.SetInteger("value", isDone ? 1 : 0);
                if (isDone)
                    isBadge = true;
            }
        }
    }
}
