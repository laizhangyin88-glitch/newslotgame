using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class EpicPassUtils
    {
        public class Sounds
        {
            public const string EPIC_PASS_ENTER_SCENE = "EnterScene";
            public const string EPIC_PASS_FLY_POINT_SCENE = "FlyPointScene";
            public const string EPIC_PASS_GET_POINT_SCENE = "GetPointScene";
            public const string EPIC_PASS_FLY_POINT = "FlyPoint";
            public const string EPIC_PASS_GET_POINT = "GetPoint";
            public const string EPIC_PASS_LEVEL_UP = "LevelUp";
            public const string EPIC_PASS_LEVEL_UP_POPUP = "LevelUpPopup";
            public const string EPIC_PASS_UNLOCK = "Unlock";
        }

        private const string EPIC_PASS_GAME_INFO = "metaGameEnterInfo";
        private const string ELIGIBLE_BET_INFO_LIST = "eligibleBetInfoList";
        private const string POINT_MULTIPLY_INFO_LIST = "pointMultiplyInfoList";

        private const string EPIC_PASS_REWARD_INFO_LIST = "rewardInfoList";
        private const string EPIC_PASS_REWARD_RESULT_LIST = "rewardResultList";

        public static string ON_LEVEL_UP_EVENT = "OnEpicPassLevelUp";
        public static string ON_REFRESH_EVENT = "OnEpicPassRefresh";
        public static string ON_REWARD_REFRESH = "OnEpicPassRewardRefresh";

        private static Dictionary<WinType, long> winTypePointMultiplyDict = new Dictionary<WinType, long>();
        private static bool isReset = false;
        private static bool isClickProcess = false;
        public static string contextId { get; set; }

        private static Blackboard epicPassInfo = null;
        public static Blackboard EpicPassInfo
        {
            get
            {
                if(epicPassInfo == null)
                {
                    var info = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), EPIC_PASS_GAME_INFO);
                    if (info != null)
                    {
                        var type = info.value.GetValue<EventInfoType>("type");
                        if (type == EventInfoType.SEASON_PASS)
                        {
                            epicPassInfo = info.value;
                        }
                    }
                }

                return epicPassInfo;
            }
        }
        // eligibleBetInfoList[0] is smallest
        private static List<Blackboard> eligibleBetInfoList;
        private static List<Blackboard> EligibleBetInfoList
        {
            get
            {
                if(eligibleBetInfoList != null && eligibleBetInfoList.Count > 0)
                {
                    if(eligibleBetInfoList[0] == null)
                        eligibleBetInfoList.Clear();
                }

                if(eligibleBetInfoList == null || eligibleBetInfoList.Count == 0)
                {
                    if(EpicPassInfo != null)
                    {
                        var list = EpicPassInfo.GetVariable<List<Blackboard>>(ELIGIBLE_BET_INFO_LIST);
                        if(list != null)
                            eligibleBetInfoList = list.value;
                    }
                }

                return eligibleBetInfoList;
            }
        }

        public static int Level
        {
            get
            {
                if(EpicPassInfo != null)
                    return EpicPassInfo.GetValue<int>("level");

                return 1;
            }
        }

        public static int MaxLevel
        {
            get
            {
                if(EpicPassInfo != null)
                    return EpicPassInfo.GetValue<int>("maxLevel");

                return 1;
            }
        }

        public static long Point
        {
            get
            {
                if (EpicPassInfo != null)
                    return RequiredPoint - EpicPassInfo.GetValue<long>("requiredPoint");
                return 0L;
            }
        }

        public static long RequiredPoint
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<long>("requiredPointMax");
                return 0L;
            }
        }

        public static bool Paid
        {
            get
            {
                if(EpicPassInfo != null)
                    return EpicPassInfo.GetValue<bool>("isPaid");
                return false;
            }
        }

        public static bool IsMaxLevel
        {
            get
            {
                if (EpicPassInfo != null)
                    return (Level == MaxLevel);
                return false;
            }
        }

        public static long ResetGemPrice
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<long>("resetGemPrice");
                return 0L;
            }
        }

        //Used for initialization in the Reward Popup
        public static bool IsReset
        {
            get
            {
                return isReset;
            }
            set
            {
                isReset = value;
            }
        }

        public static bool IsClickProcess
        {
            get { return isClickProcess; }
            set { isClickProcess = value; }
        }

        public static bool IsDisplayCollectAll
        {
            get
            {
                if(EpicPassInfo != null)
                    return EpicPassInfo.GetValue<bool>("collectAllButtonDisplayed");

                return false;
            }
        }

        public static bool IsEnabledCollectAll
        {
            get
            {
                if(EpicPassInfo != null)
                    return EpicPassInfo.GetValue<bool>("collectAllEnabled");

                return false;
            }
        }

        public static Blackboard GetResetProduct
        {
            get
            {
                if (EpicPassInfo != null)
                {
                    var products = EpicPassInfo.GetValue<List<Blackboard>>("recommendedGemProductList");

                    long myGem = BlackboardUtils.FindValue<long>(null, "/me/gem");
                    var myTier = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;

                    for (int i = 0; i < products.Count; i++)
                    {
                        Blackboard itemBB = BlackboardQueryUtils.GetItemFromProduct(products[i], ItemType.GEM);
                        long gem = itemBB.GetValue<long>("gem");
                        long tierGem = TierUtils.GetTierFractionCoin(gem, myTier);

                        if (tierGem + myGem > ResetGemPrice)
                            return products[i];
                    }
                }

                return null;
            }
        }

        public static string SeasonPassResetType
        {
            get
            {
                return "season_pass_reset";
            }
        }

        public static string gemProductId
        {
            get
            {
                EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
                if (metaGameInfo != null)
                    return SeasonPassResetType + "|" + metaGameInfo.id;
                else
                    return string.Empty;
            }
        }

        public static int PrevLevel
        {
            get
            {
                if(EpicPassInfo != null)
                {
                    var prevLevel = EpicPassInfo.GetVariable<int>("prevLevel");
                    if(prevLevel != null)
                        return prevLevel.value;
                }

                return Level;
            }
        }

        public static long PrevPoint
        {
            get
            {
                if(EpicPassInfo != null)
                {
                    var prevPoint = EpicPassInfo.GetVariable<long>("prevRequiredPoint");
                    if(prevPoint != null)
                        return PrevRequiredPoint - prevPoint.value;
                }

                return 0L;
            }
        }

        public static long PrevRequiredPoint
        {
            get
            {
                if(EpicPassInfo != null)
                {
                    var prevReqPoint = EpicPassInfo.GetVariable<long>("prevRequiredPointMax");
                    if(prevReqPoint != null)
                        return prevReqPoint.value;
                }

                return 0L;
            }
        }

        public static long EarnPoint
        {
            get
            {
                if(EpicPassInfo != null)
                {
                    var earnPoint = EpicPassInfo.GetVariable<long>("earnPoint");
                    if(earnPoint != null)
                        return earnPoint.value;
                }

                return 0L;
            }
        }

        public static string PointImageUrl
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetVariable<string>("pointIconImageUrl").value;
                return string.Empty;
            }
        }

        public static int UnclaimedRewardCount
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetVariable<int>("unclaimedRewardCount")?.value ?? 0;
                return 0;
            }

            set
            {
                if (EpicPassInfo != null && value > -1)
                {
                    BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "unclaimedRewardCount", value);
                }
            }
        }

        public static List<long> SkippedRequiredPointMaxList
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetVariable<List<long>>("skippedRequiredPointMaxList").value;
                return null;
            }
        }

        public static long AdsFreePoint
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<long>("adsFreePoint");
                return 0;
            }

            set
            {
                BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "adsFreePoint", value);
            }
        }

        public static void Clear()
        {
            epicPassInfo = null;
            if(eligibleBetInfoList != null)
            {
                eligibleBetInfoList.Clear();
                eligibleBetInfoList = null;
            }

            winTypePointMultiplyDict.Clear();
        }

        public static void UpdateSeasonPassInfo(SeasonPassInfoResponse response)
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), EPIC_PASS_GAME_INFO);
            ClientAPI2Blackboard.Serialize(bb, response);

            BackUpInfo();
        }

        public static void BackUpInfo()
        {
            if(EpicPassInfo == null) return;
            // Backup
            BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "prevLevel", EpicPassInfo.GetValue<int>("level"));
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "prevRequiredPoint", EpicPassInfo.GetValue<long>("requiredPoint"));
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "prevRequiredPointMax", EpicPassInfo.GetValue<long>("requiredPointMax"));
        }

        public static void UpdateSeasonPassPointInfo(SeasonPassPointUpdateInfo updateInfo)
        {
            if(updateInfo == null) return;
            // Hack code.
            if(updateInfo.unclaimedRewardCount < 0)
                updateInfo.unclaimedRewardCount = UnclaimedRewardCount;
            //////
            if(EpicPassInfo == null) return;

            BackUpInfo();

            BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "level", updateInfo.level);
            // BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "point", updateInfo.point);
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "requiredPoint", updateInfo.requiredPoint);
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "requiredPointMax", updateInfo.requiredPointMax);
        }

        public static Blackboard GetEligibleBetInfoBB(long bet)
        {
            if (EpicPassInfo == null) return null;

            if (EligibleBetInfoList == null) return null;

            var list = EligibleBetInfoList;
            Blackboard retBB = null;

            for (int i = 0; i < list.Count; ++i)
            {
                long unitBet = list[i].GetValue<long>("totalBet");

                if (bet < unitBet) continue;
                retBB = list[i];
            }

            return retBB;
        }

        public static long GetWinTypePoint(Blackboard eligibleBetInfoBB, WinType winType)
        {
            if (eligibleBetInfoBB == null) return 0L;

            var pointInfoList = eligibleBetInfoBB.GetValue<List<Blackboard>>("bigWinPointInfo");
            if(pointInfoList != null)
            {
                for(int i=0; i < pointInfoList.Count; ++i)
                {
                    var targetWinType = pointInfoList[i].GetValue<WinType>("winType");
                    if( targetWinType == winType)
                    {
                        return pointInfoList[i].GetValue<long>("point");
                    }
                }
            }

            return 0L;
        }

        public static void UpdateEpicPassRewards(List<SeasonPassRewardInfo> rewardInfoList)
        {
            if(EpicPassInfo == null) return;

            var rewardInfoListBB = EpicPassInfo.GetValue<List<Blackboard>>(EPIC_PASS_REWARD_INFO_LIST);
            for(int i=0; i < rewardInfoListBB.Count; ++i)
            {
                if(rewardInfoList.Count > i)
                {
                    if(rewardInfoList[i].freeReward != null)
                    {
                        var freeReward = rewardInfoListBB[i].GetValue<Blackboard>("freeReward");
                        freeReward.SetValue("isClaimed", rewardInfoList[i].freeReward.isClaimed);
                    }

                    if(rewardInfoList[i].paidReward != null)
                    {
                        var paidReward = rewardInfoListBB[i].GetValue<Blackboard>("paidReward");
                        paidReward.SetValue("isClaimed", rewardInfoList[i].paidReward.isClaimed);
                    }
                }
            }
        }

        public static List<Blackboard> UpdateCollectRewards(List<RewardResult> rewardResultList)
        {
            if(EpicPassInfo == null) return new List<Blackboard>();

            BlackboardUtils.SetOrCreateList(EpicPassInfo, EPIC_PASS_REWARD_RESULT_LIST, rewardResultList, ClientAPI2Blackboard.Serialize);

            return EpicPassInfo.GetValue<List<Blackboard>>(EPIC_PASS_REWARD_RESULT_LIST);
        }

        public static void UpdateAdsView(long lastAdsViewTimestamp, long nextAdsResetTimestamp)
        {
            if (EpicPassInfo == null) return;

            EpicPassInfo.SetValue("lastAdsViewTimestamp", lastAdsViewTimestamp);
            EpicPassInfo.SetValue("nextAdsResetTimestamp", nextAdsResetTimestamp);
        }

        public static void UpdateAdsClaim(SeasonPassPointUpdateInfo updateInfo, long updateAdsFreePoint, long lastAdsClaimTimestamp)
        {
            if (EpicPassInfo == null) return;

            AdsFreePoint = updateAdsFreePoint;
            EpicPassInfo.SetValue("lastAdsClaimTimestamp", lastAdsClaimTimestamp);
            UpdateSeasonPassPointInfo(updateInfo);

            EpicPassInfo.SetValue("unclaimedRewardCount", updateInfo.unclaimedRewardCount);
        }

        public static void SetPaid(int unclaimedRewardCount)
        {
            if (EpicPassInfo == null) return;

            EpicPassInfo.SetValue("isPaid", true);
            EpicPassInfo.SetValue("unclaimedRewardCount", unclaimedRewardCount);
            EpicPassInfo.SetValue("collectAllEnabled", true);
        }

        public static void RequestEpicPassReset()
        {
            Blackboard AEProduct = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(EpicPassInfo, "AEProduct");
            ProductUtils.MakeAEProduct(AEProduct, gemProductId, SeasonPassResetType);
            BiEventUtils.ItemClick(AEProduct, contextId, false);

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            BagelCodeClientAPI.RequestSeasonPassReset(metaGameInfo.id,
                (response) =>
                {
                    if (EpicPassInfo == null) return;

                    ResetEpicPassVariable();

                    BiEventUtils.GemTransaction(AEProduct, contextId, true, false);
                    BiEventUtils.ItemAcquired(AEProduct, contextId, false);
                    UnclaimedRewardCount = 0;

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    // Notice Rewards have been send to inbox
                    if (response.rewardResultList.Count > 0)
                    {
                        OpenRewardInboxScene();
                    }
                    else
                    {
                        OpenRewardScene();
                    }
                },
                (error) =>
                {
                    if (EpicPassInfo != null)
                        BiEventUtils.GemTransaction(AEProduct, contextId, false, false);
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_LEVEL_OR_REWARD_CONDITION_ERROR:
                            // request error pass ?
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
           );
        }

        public static void ResetEpicPassVariable()
        {
            IsReset = true;
        }

        public static void OpenRewardLostScene()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Popup Reward Lost Scene",
                PopupManager.Instance.transform,
                "Area"
            );
        }

        public static void OpenRewardScene()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                BlackboardQueryUtils.GetMetaBundleName(EventInfoType.SEASON_PASS),
                "Popup Epic Pass Reward Scene",
                PopupManager.Instance.transform,
                "Area"
            );

            ReqeustSeasonPassInfo();
        }

        public static void OpenRewardInboxScene()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Popup Reward Inbox Scene",
                PopupManager.Instance.transform,
                "Area"
            );
        }

        public static void ReqeustSeasonPassInfo()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if (metaGameInfo == null)
                return;

            BagelCodeClientAPI.SeasonPassInfoRequest(metaGameInfo.id,
                (response) =>
                {
                    if (EpicPassInfo == null) return;
                    UpdateSeasonPassInfo(response);

                    EventData eventData = new EventData(EpicPassUtils.ON_REFRESH_EVENT);
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case ClientModels.Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }

        public static void UpdateUnClaimedItem()
        {
            if(EpicPassInfo == null) return;

            var rewardInfoListBB = EpicPassInfo.GetValue<List<Blackboard>>(EPIC_PASS_REWARD_INFO_LIST);

            int unClaimedItemCount = 0;
            for (int i = 0; i < rewardInfoListBB.Count; ++i)
            {
                if (i >= Level)
                    break;

                var freeReward = rewardInfoListBB[i].GetValue<Blackboard>("freeReward");
                if (freeReward != null)
                {
                    var reward = freeReward.GetVariable<Blackboard>("reward");
                    if (reward != null && !freeReward.GetValue<bool>("isClaimed"))
                        unClaimedItemCount++;
                }

                if (Paid)
                {
                    var paidReward = rewardInfoListBB[i].GetValue<Blackboard>("paidReward");
                    if (paidReward != null)
                    {
                        var reward = paidReward.GetVariable<Blackboard>("reward");
                        if (reward != null && !paidReward.GetValue<bool>("isClaimed"))
                            unClaimedItemCount++;
                    }
                }
            }

            UnclaimedRewardCount = unClaimedItemCount;
        }

        public static void CollectAll()
        {
            if(EpicPassInfo == null) return;

            UnclaimedRewardCount = 0;

            var rewardInfoListBB = EpicPassInfo.GetValue<List<Blackboard>>(EPIC_PASS_REWARD_INFO_LIST);
            for(int i=0; i < rewardInfoListBB.Count; ++i)
            {
                if (i >= Level)
                    break;

                var freeReward = rewardInfoListBB[i].GetValue<Blackboard>("freeReward");
                if(freeReward != null)
                    freeReward.SetValue("isClaimed", true);

                if(Paid)
                {
                    var paidReward = rewardInfoListBB[i].GetValue<Blackboard>("paidReward");
                    if (paidReward != null)
                        paidReward.SetValue("isClaimed", true);
                }
            }
        }

        public static bool CheckUnlockedLevel()
        {
            return MetaGameUtils.IsMetaGameLevelLocked();
        }
    }
}
