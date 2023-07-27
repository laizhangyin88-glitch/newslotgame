using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class InGameChallengeLayerController : EventMonoBehaviour
    {
        public float displayTime = 1f;
        public bool isPersonalChallengeLayer;

        private ContextElement root;
        private Blackboard rootBB;
        private Animator rootAnim;

        private ContextElement progressTextElement;

        private ContextElement missionProgressGaugeElement;
        private ContextElement missionProgressTextElement;

        private ContextElement textDescElement;
        private ContextElement missionIconTextElement;
        private ContextElement rewardTextElement;
        private ContextElement bigRewardTextElement;

        private ContextElement completeRewardTextElement;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

        private void Start()
        {
            InitProperty();

            StartCoroutine(LayerUpdateCoroutine());
        }

        private void InitProperty()
        {
            root = GetComponent<ContextElement>();
            rootBB = GetComponent<Blackboard>();
            rootAnim = GetComponent<Animator>();

            root.UpdateContext(false);

            progressTextElement = ContextUtils.FindElement(root, "Challenge Info Anchor/Text", FULL);

            missionProgressGaugeElement = ContextUtils.FindElement(root, "Base Anchor/Gauge", FULL);
            missionProgressTextElement = ContextUtils.FindElement(root, "Base Anchor/Gauge/Text", FULL);

            textDescElement = ContextUtils.FindElement(root, "Base Anchor/Text Description", FULL);
            missionIconTextElement = ContextUtils.FindElement(root, "Base Anchor/Icon Text", FULL);
            rewardTextElement = ContextUtils.FindElement(root, "Base Anchor/Reward/Text", FULL);
            bigRewardTextElement = ContextUtils.FindElement(root, "Complete Anchor/Complete Reward/Text Complete Reward", FULL);

            completeRewardTextElement = ContextUtils.FindElement(root, "Complete Anchor/Complete Reward/Text Complete Reward", FULL);

            RegisterHandleEventType(MetaEventDefine.ON_CONTENT_UI_EVENT);
            Register(MetaEventDefine.ON_CONTENT_UI_EVENT, "HideUI", Close);
        }

        private void Close()
        {
            EventSender.SendCalleeCallback(gameObject);
            Destroy(gameObject);
        }

        private IEnumerator LayerUpdateCoroutine()
        {
            GSManager.Instance.GetHandler("UI_Kudo_Appear").Play();

            var completeInfo = rootBB.GetValue<Blackboard>("_completeInfo");

            var rewardInfo = completeInfo.GetVariable<Blackboard>("rewardResult")?.value;
            var missionInfo = completeInfo.GetValue<Blackboard>("mission");
            var simpleChallengeInfo = completeInfo.GetValue<Blackboard>("challenge");

            var challengeType = simpleChallengeInfo.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
            var clubChallengeType = simpleChallengeInfo.GetVariable<ClubChallengeType>("type")?.value ?? ClubChallengeType.UNKNOWN;

            bool useThumbnail = false;
            int gameId = missionInfo.GetVariable<int>("gameId")?.value ?? -1;

            // Send Event
            var eventData = new EventData<Blackboard>(ChallengeEventManager.ON_SHOOT_CHALLENGE_MISSION, completeInfo);
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);

            // Challenge Progress
            int progress = simpleChallengeInfo.GetValue<int>("challengeProgress");
            int maxProgress = simpleChallengeInfo.GetValue<int>("maxChallengeProgress");
            string progressText = StringTableUtils.GetString(GLOBAL, "A_PER_B", progress, maxProgress);
            MetaContextElementUtils.SetText(progressTextElement, progressText);

            // Check Next Stage
            bool done = progress >= maxProgress;
            bool isNextStage = false;
            string stageText = string.Empty;
            if (!isPersonalChallengeLayer)
            {
                CheckIsNextStage(simpleChallengeInfo, ref done, out isNextStage, out stageText);
            }

            // Claimed State
            bool isClaimEnabled = false;
            if (isPersonalChallengeLayer)
            {
                GetClaimedState(challengeType, out isClaimEnabled);
            }

            // Reward Text
            RewardType rewardType = RewardType.UNKNOWN;
            if (isPersonalChallengeLayer)
            {
                GetRewardText(rewardInfo, out string rewardText, out rewardType);
                MetaContextElementUtils.SetText(rewardTextElement, rewardText);
                MetaContextElementUtils.SetText(completeRewardTextElement, rewardText);
            }
            else
            {
                GetClubRewardText(rewardInfo, missionInfo, isNextStage, out string rewardText, out string bigRewardText);
                MetaContextElementUtils.SetText(rewardTextElement, rewardText);
                MetaContextElementUtils.SetText(bigRewardTextElement, bigRewardText);
            }

            // Set Anim
            rootAnim.SetBool("Active", true);
            if (isPersonalChallengeLayer)
            {
                if (challengeType == ChallengeType.DAILY)
                    rootAnim.SetInteger("Challenge", 0);
                else if (challengeType == ChallengeType.EXPERT)
                    rootAnim.SetInteger("Challenge", 1);
                else if (challengeType == ChallengeType.MASTER)
                    rootAnim.SetInteger("Challenge", 2);
                else if (challengeType == ChallengeType.EVENT)
                    rootAnim.SetInteger("Challenge", 4);
                bool isRewardCoin = rewardType == RewardType.CREDIT;
                rootAnim.SetBool("IsRewardCoin", isRewardCoin);
                rootAnim.SetBool("IsRewardItem", !isRewardCoin);
                rootAnim.SetBool("IsNextStage", false);
            }
            else
            {
                if (clubChallengeType == ClubChallengeType.NORMAL)
                    rootAnim.SetInteger("Challenge", 3);
                else if (clubChallengeType == ClubChallengeType.EVENT)
                    rootAnim.SetInteger("Challenge", 5);
                rootAnim.SetBool("IsRewardCoin", false);
                rootAnim.SetBool("IsRewardItem", false);
                rootAnim.SetBool("IsNextStage", isNextStage);
            }

            // Mission Title
            string missionTitle = GetMissionTitle(missionInfo);
            if (missionTitle == "UNKNOWN")
            {
                useThumbnail = true;
                gameId = -1;
            }
            MetaContextElementUtils.SetText(textDescElement, missionTitle);

            // Mission Icon
            MakeMissionIconObj(missionInfo, ref gameId, ref useThumbnail);

            // Mission Progress
            if (isPersonalChallengeLayer)
            {
                GetMissionProgressText(missionInfo, out string missionProgressText, out float missionProgress);
                MetaContextElementUtils.SetFloatProperty(missionProgressGaugeElement, missionProgress);
                MetaContextElementUtils.SetText(missionProgressTextElement, missionProgressText);
            }
            else
            {
                GetClubMissionProgressText(missionInfo, out string missionProgressText, out float missionProgress);
                MetaContextElementUtils.SetFloatProperty(missionProgressGaugeElement, missionProgress);
                MetaContextElementUtils.SetText(missionProgressTextElement, missionProgressText);
            }

            if (isNextStage)
            {
                var onShowNextStageTrigger = new EventTrigger(this, "OnShowNextStage");
                yield return new WaitUntilTrigger(onShowNextStageTrigger);

                MetaContextElementUtils.SetText(completeRewardTextElement, stageText);
            }

            // On Complete Effect
            var onCompleteEffectTrigger = new EventTrigger(this, "OnCompleteEffect");
            yield return new WaitUntilTrigger(onCompleteEffectTrigger);

            bool waitCompleteEffect = isPersonalChallengeLayer ? isClaimEnabled : done;
            if (waitCompleteEffect)
            {
                string eventName = isPersonalChallengeLayer ?
                    ChallengeEventManager.ON_START_COMPLETE_CHALLENGE_POPUP :
                    ChallengeEventManager.ON_START_COMPLETE_CLUB_CHALLENGE_POPUP;
                var completeEventData = new EventData<Blackboard>(eventName, completeInfo);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, completeEventData);

                var onEndCompleteChallengePopupTrigger = new EventTrigger(this, MetaEventDefine.ON_META_UI_EVENT, ChallengeEventManager.ON_END_COMPLETE_CHALLENGE_POPUP);
                yield return new WaitUntilTrigger(onEndCompleteChallengePopupTrigger);
            }

            EventSender.SendCalleeCallback(gameObject);

            yield return new WaitForSeconds(displayTime);

            Destroy(gameObject);
        }

        private string GetMissionTitle(Blackboard missionInfo)
        {
            if (isPersonalChallengeLayer)
            {
                return ChallengeUtils.GetChallengeMissionTitle(missionInfo);
            }
            else
            {
                return ChallengeUtils.ClubChallengeMissionTypeToText(missionInfo);
            }
        }

        private GameObject MakeMissionIconObj(Blackboard missionInfo, ref int gameId, ref bool useThumbnail)
        {
            GameObject missionIconObj = null;
            if (isPersonalChallengeLayer)
            {
                missionIconObj = MetaIconUtils.MakeChallengeMissionIconObject(missionInfo, gameId, transform, "Anchor/Base Anchor/Mission/Image Area");
            }
            else
            {
                missionIconObj = MetaIconUtils.MakeClubChallengeMissionIconObject(missionInfo, gameId, transform, "Anchor/Base Anchor/Mission/Image Area");
            }

            if (missionIconObj is null)
            {
                useThumbnail = true;
            }
            else
            {
                missionIconObj.transform.SetAsFirstSibling();
            }

            if (useThumbnail)
            {
                string gameTitle = "Default";
                if (gameId > 0)
                {
                    var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
                    gameTitle = gameInfo.GetValue<string>("gameTitle");
                }

                string thumbnailName = StringTableUtils.GetString(GLOBAL, "SLOT_THUMBNAIL_NORMAL", gameTitle);

                var thumbnailPrefab = AssetBundleManager.LoadAsset<GameObject>("slotthumb1", thumbnailName);
                if (thumbnailPrefab != null)
                {
                    missionIconObj = GameObject.Instantiate(thumbnailPrefab) as GameObject;
                    missionIconObj.name = thumbnailName;

                    var parent = transform.Find("Anchor/Base Anchor/Mission/Thumbnail Area");
                    missionIconObj.transform.SetParent(parent, false);
                }
            }

            return missionIconObj;
        }

        private void CheckIsNextStage(Blackboard simpleChallengeInfo, ref bool done, out bool isNextStage, out string stageText)
        {
            stageText = string.Empty;
            isNextStage = false;
            int stage = simpleChallengeInfo.GetVariable<int>("stage")?.value ?? -1;
            int maxStage = BlackboardQueryUtils.GetClubChallengeMaxStage(simpleChallengeInfo);
            
            if (stage >= 0 && done)
            {
                stage += 1;
                int nextStage = stage + 1;
                if (stage < maxStage)
                {
                    stageText = StringTableUtils.GetString(GLOBAL, "FEED_CLUB_CHALLENGE_MISSION_NEXT_STAGE_TEXT", nextStage);
                    done = false;
                    isNextStage = true;
                }
            }
        }

        private void GetMissionProgressText(Blackboard missionInfo, out string progressText, out float progress)
        {
            progressText = BlackboardQueryUtils.GetChallengeProgressText(missionInfo, out progress, out _);
        }

        private void GetClubMissionProgressText(Blackboard missionInfo, out string progressText, out float progress)
        {
            long currentCount = missionInfo.GetValue<long>("progress");
            long completeCount = missionInfo.GetValue<long>("completeCount");

            if (currentCount > completeCount)
                currentCount = completeCount;

            progress = (float)currentCount / completeCount;

            ClubChallengeMissionType missionType = missionInfo.GetValue<ClubChallengeMissionType>("missionType");

            switch (missionType)
            {
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BONUS_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_TARGETED:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_FREE_SPIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_ANY:
                case ClubChallengeMissionType.ACHIEVE_TOTAL_LP_BY_BIG_WIN_TARGETED:
                    progressText = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", currentCount, completeCount);
                    break;
                default:
                    progressText = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_DEFAULT", currentCount, completeCount);
                    break;
            }
        }

        private void GetRewardText(Blackboard rewardInfo, out string rewardText, out RewardType rewardType)
        {
            if (rewardInfo == null)
            {
                rewardType = RewardType.UNKNOWN;
                rewardText = string.Empty;
            }
            else
            {
                rewardType = rewardInfo.GetVariable<RewardType>("rewardType")?.value ?? RewardType.UNKNOWN;
                rewardText = BlackboardQueryUtils.GetChallengeMissionRewardResultText(rewardInfo);
            }
        }

        private void GetClubRewardText(Blackboard rewardInfo, Blackboard missionInfo, bool isNextStage, out string rewardText, out string bigRewardText)
        {
            if (rewardInfo == null)
            {
                rewardText = string.Empty;
                bigRewardText = string.Empty;
                return;
            }

            rewardText = BlackboardQueryUtils.GetChallengeMissionRewardResultText(rewardInfo);
            bigRewardText = rewardText;

            var leaguePoint = missionInfo.GetVariable<long>("leaguePoint")?.value ?? 0;
            if (leaguePoint > 0)
            {
                string lpText = StringTableUtils.GetString(GLOBAL, "SIMPLE_LP", leaguePoint);
                rewardText += " + " + lpText;
                bigRewardText += " + " + lpText;
            }

            if (isNextStage)
                bigRewardText = StringTableUtils.GetString(GLOBAL, "FEED_CLUB_CHALLENGE_MISSION_AND_STAGE_COMPLTE_TEXT", bigRewardText);
            else
                bigRewardText = StringTableUtils.GetString(GLOBAL, "FEED_CLUB_CHALLENGE_MISSION_COMPLTE_TEXT", bigRewardText);
        }

        private void GetClaimedState(ChallengeType challengeType, out bool isEnabled)
        {
            isEnabled = false;
            long currentTimestamp = TimeUtils.GetTimeStamp();

            var challengeInfoList = MainBlackboard.Get().GetValue<List<Blackboard>>("challengeInfoList");
            for (int i = 0; i < challengeInfoList.Count; ++i)
            {
                var info = challengeInfoList[i];
                var cType = info.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
                if (cType == challengeType)
                {
                    long startTimestamp = BlackboardUtils.FindVariable<long>(info, "startTimestamp").value;
                    startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);
                    if (startTimestamp < currentTimestamp)
                    {
                        var done = BlackboardUtils.FindVariable<bool>(info, "done");
                        var claimed = BlackboardUtils.FindVariable<bool>(info, "claimed");

                        isEnabled = done.value && !claimed.value;
                        break;
                    }
                }
            }
        }
    }
}
