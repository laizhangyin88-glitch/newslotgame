using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class ChallengePopupDataClub : ChallengePopupData
    {
        public ChallengePopupDataClub(GameObject _owner)
            : base(_owner) { }

        public Blackboard challengeInfo;

        private ContextElement challengeElement;
        private ContextElement challengeJoinElement;
        private ContextElement challengeJoinDefaultElement;
        private ContextElement challengeJoinLobbyElement;
        private ContextElement challengeInfoElement;
        private ContextElement challengeRewardElement;
        private ContextElement challengeRewardCreditElement;
        private ContextElement remainingTimer;

        private ContextElement completeElement;

        private ContextElement selectBaseElement;

        private List<GameObject> missionCellList;
        private ClubChallengeStageInfoController stageController;

        //

        public override void InitProperty()
        {
            isMainTab = true;

            challengeElement = ContextUtils.FindElement(root, "Club Challenge Context", CHILDREN);

            completeElement = ContextUtils.FindElement(challengeElement, "Challenge Info Complete", CHILDREN);
            challengeJoinElement = ContextUtils.FindElement(root, "Club Challenge Join", CHILDREN);
            challengeJoinDefaultElement = ContextUtils.FindElement(challengeJoinElement, "Default", CHILDREN);
            challengeJoinLobbyElement = ContextUtils.FindElement(challengeJoinElement, "Lobby", CHILDREN);
            challengeInfoElement = ContextUtils.FindElement(challengeElement, "Challenge Info", CHILDREN);
            challengeRewardElement = ContextUtils.FindElement(challengeInfoElement, "Reward/Info", FULL);
            challengeRewardCreditElement = ContextUtils.FindElement(challengeInfoElement, "Reward/Credit", FULL);
            remainingTimer = ContextUtils.FindElement(challengeElement, "Remaining Timer", CHILDREN);

            var stageInfoElement = ContextUtils.FindElement(challengeInfoElement, "Stage Info", CHILDREN);
            stageController = stageInfoElement.gameObject.GetComponent<ClubChallengeStageInfoController>();

            var clubJoinLobbyButtonElement = ContextUtils.FindElement(challengeJoinLobbyElement, "Button Go To Club", CHILDREN);
            MetaContextElementUtils.SimpleSetTextGlobal(clubJoinLobbyButtonElement, "Text", "BUTTON_LETS_START", CHILDREN);

            selectBaseElement = ContextUtils.FindElement(root, "Left Tab Base/Tab Club/Base Select", FULL);

            MetaContextElementUtils.SetClickable(
                clubJoinLobbyButtonElement, () => EventSender.SendEvent(owner, ChallengeEventManager.ON_GO_TO_CLUB));

            var missionListElement = ContextUtils.FindElement(challengeElement, "Club Challenge Mission List", CHILDREN);

            if (missionCellList != null)
                missionCellList.ForEach(c => c.DestroyThis());

            missionCellList = new List<GameObject>();
            for (int i = 0; i < 4; ++i)
            {
                var cellObj = MetaObjectUtils.MakeScene(MetaStringDefine.LOBBY_BUNDLE_NAME, "Club Challenge Mission Cell Scene", missionListElement.transform, null);

                var cellBB = cellObj.GetComponent<Blackboard>();
                if (cellBB != null)
                {
                    var variable = BlackboardUtils.GetOrCreateVariable<GameObject>(cellBB, "caller");
                    variable.value = owner;
                }

                cellObj.SetActive(false);
                missionCellList.Add(cellObj);
            }
        }

        public override void ClearPopupData()
        {
            if (missionCellList != null)
                missionCellList.ForEach(c => c.DestroyThis());
        }

        public override bool IsComplete(MetaChallengeType tabType)
        {
            return challengeInfo != null && challengeInfo.GetValue<bool>("done");
        }

        public override void OnUpdateChallengeInfo()
        {
            UpdateChallengeInfo();
            UpdateBadge();
        }

        public override void UpdateChallengeInfo()
        {
            isUpdated = true;

            Blackboard responseInfo = rootBB.GetVariable<Blackboard>("challengeInfoResponse")?.value;
            if (responseInfo != null)
            {
                challengeInfo = responseInfo.GetVariable<Blackboard>("clubChallengeInfo")?.value;
            }
            else
            {
                Debug.LogError("challengeInfoResponse is null");
                return;
            }
        }

        public override void OnSelectTab(MetaChallengeType tabType)
        {
            if(tabType != MetaChallengeType.CLUB)
            {
                challengeElement.gameObject.SetActive(false);
                challengeJoinElement.gameObject.SetActive(false);
                selectBaseElement?.gameObject.SetActive(false);
                return;
            }

            ChangeTab(MetaChallengeType.CLUB);
            selectBaseElement?.gameObject.SetActive(true);
            UpdatePopupData();
        }

        public override void UpdatePopupData()
        {
            bool isClubber = challengeInfo != null;
            challengeElement.gameObject.SetActive(isClubber);
            challengeJoinElement.gameObject.SetActive(!isClubber);

            Blackboard responseInfo = rootBB.GetVariable<Blackboard>("challengeInfoResponse")?.value;

            challengeCompleteElement.gameObject.SetActive(false);

            if (isClubber)
            {
                int progressCount = 0;
                int stage = challengeInfo.GetValue<int>("stage");
                int maxStage = BlackboardQueryUtils.GetClubChallengeMaxStage(challengeInfo);

                var topContributionList = responseInfo.GetValue<List<Blackboard>>("topContributionList");
                var missionList = challengeInfo.GetValue<List<Blackboard>>("missionList");
                bool isComplete = IsComplete(MetaChallengeType.CLUB);
                bool isSingle = maxStage <= 1;

                MetaContextElementUtils.SetTextGlobal(challengeCompleteTextElement, maxStage <= 1 ? "POPUP_CHALLENGE_CLUB_COMPLETE_ONE" : "POPUP_CHALLENGE_CLUB_COMPLETE");

                // Title
                string titleText = StringTableUtils.GetString(GLOBAL, maxStage <= 1 ? "POPUP_CHALLENGE_CLUB_DESCRIPTION_ONE" : "POPUP_CHALLENGE_CLUB_DESCRIPTION");
                MetaContextElementUtils.SetText(challengeRewardElement, titleText);

                // Reward
                Blackboard rewardBB = challengeInfo.GetValue<Blackboard>("totalReward");

                var clubMultiplier  = ClubUtils.GetClubRewardMultiplierNumerator();
                long rewardCoin = rewardBB != null ? NumberUtils.GetMultiplierNumeratorValue(rewardBB.GetValue<long>("credit"), clubMultiplier) : 0L;

                string rewardCoinText = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_TOTAL_REWARD_COIN", rewardCoin);
                var leaguePoint = challengeInfo.GetValue<long>("leaguePoint");
                if (leaguePoint > 0)
                    rewardCoinText += " + " + StringTableUtils.GetString(GLOBAL, "COMMA_LP", leaguePoint);
                MetaContextElementUtils.SetText(challengeRewardCreditElement, rewardCoinText);

                // Progress
                for (int i = 0; i < missionList.Count; ++i)
                {
                    bool missionComplete = missionList[i].GetValue<bool>("done");
                    int completedStage = missionList[i].GetValue<int>("completedStage");
                    if (stage >= 0 && stage == completedStage)
                        missionComplete = true;

                    if (missionComplete)
                        progressCount++;

                    if (missionCellList.Count <= i)
                        continue;

                    var cellBB = missionCellList[i].gameObject.GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "missionInfo", missionList[i]);
                    BlackboardUtils.SetOrCreateValue<Blackboard>(cellBB, "clubChallengeInfo", challengeInfo);
                    // BlackboardUtils.SetOrCreateValue<ChallengeType>(cellBB, "challengeType", challengeType);
                    // BlackboardUtils.SetOrCreateValue<int>(cellBB, "challengeIndex", 3);

                    var contributionList = topContributionList[i].GetValue<List<Blackboard>>("contributionList");
                    BlackboardUtils.SetOrCreateValue<List<Blackboard>>(cellBB, "contributionList", contributionList);

                    BlackboardUtils.SetOrCreateValue<bool>(cellBB, "updateInfo", true);

                    missionCellList[i].gameObject.SetActive(true);
                }

                // Timer
                long endTimestamp = challengeInfo.GetValue<long>("endTimestamp");
                MetaContextElementUtils.SetCommonRemainingTimer(remainingTimer, endTimestamp, 0, "TIME_FORMAT_HHMMSS_TOTALHOUR", null, null, "Ended", true, owner);

                // Set Active Complete/Info
                completeElement.gameObject.SetActive(isComplete);
                challengeInfoElement.gameObject.SetActive(!isComplete);

                BI_ClientChallengeEnter(challengeInfo, "club", isAuto ? "auto" : "manual");

                // Stage
                if (!isComplete)
                {
                    List<long> rewardRatioNumeratorList = null;

                    var stagePreset = BlackboardUtils.FindVariable<Blackboard>(challengeInfo, "stagePreset");
                    if (stagePreset != null && stagePreset.value != null)
                        rewardRatioNumeratorList = BlackboardUtils.FindVariable<List<long>>(stagePreset.value, "coinRewardRatioNumeratorList").value;

                    stageController.OnUpdateVariables(stage, maxStage, progressCount, rewardRatioNumeratorList);
                }
            }
            else
            {
                completeElement.gameObject.SetActive(false);
                challengeInfoElement.gameObject.SetActive(false);

                if (BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.CLUB))
                {
                    int unlockLevel = BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.CLUB);

                    MetaContextElementUtils.SimpleSetTextGlobal(challengeJoinDefaultElement, "Text Join", "POPUP_CHALLENGE_CLUB_JOIN_LOCKED_TEXT", CHILDREN, unlockLevel);
                    MetaContextElementUtils.SetActive(challengeJoinDefaultElement, true);
                    MetaContextElementUtils.SetActive(challengeJoinLobbyElement, false);
                }
                else
                {
                    bool isInGame = BlackboardQueryUtils.IsIngame();
                    if (isInGame)
                    {
                        MetaContextElementUtils.SimpleSetTextGlobal(challengeJoinDefaultElement, "Text Join", "POPUP_CHALLENGE_CLUB_JOIN_INGAME_TEXT", CHILDREN);
                        MetaContextElementUtils.SetActive(challengeJoinDefaultElement, true);
                        MetaContextElementUtils.SetActive(challengeJoinLobbyElement, false);
                    }
                    else
                    {
                        MetaContextElementUtils.SimpleSetTextGlobal(challengeJoinLobbyElement, "Text Join Lobby", "POPUP_CHALLENGE_CLUB_JOIN_LOBBY_TEXT", CHILDREN);
                        MetaContextElementUtils.SetActive(challengeJoinDefaultElement, false);
                        MetaContextElementUtils.SetActive(challengeJoinLobbyElement, true);
                    }
                }
            }
        }

        public override void UpdateBadge()
        {
            bool isComplete = IsComplete(MetaChallengeType.CLUB);
            completeElementDict[MetaChallengeType.CLUB].gameObject.SetActive(isComplete);
            isBadge = false;
            badgeAnimatorDict[MetaChallengeType.CLUB]?.SetInteger("value", isBadge ? 1 : 0);
        }
    }
}
