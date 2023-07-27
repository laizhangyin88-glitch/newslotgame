using UnityEngine;
using System.Collections.Generic;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class EpicPassUtilsV2
    {
        public class Sounds
        {
            public const string EPIC_PASS_FLY_POINT_SCENE = "EpicPassAlways_FlyPointScene";
            public const string EPIC_PASS_GET_POINT_SCENE = "EpicPassAlways_GetPointScene";
            public const string EPIC_PASS_FLY_POINT = "EpicPassAlways_FlyPoint";
            public const string EPIC_PASS_GET_POINT = "EpicPassAlways_GetPoint";
            public const string EPIC_PASS_LEVEL_UP = "EpicPassAlways_LevelUp";
            public const string EPIC_PASS_LEVEL_UP_POPUP = "EpicPassAlways_LevelUpPopup";
            public const string EPIC_PASS_UNLOCK = "EpicPassAlways_Unlock";
            public const string EPIC_PASS_REWARD = "Get_EpicPass_Reward";
            public const string EPIC_PASS_RESTART = "Restart_EpicPass";
        }

        public static readonly string EPIC_PASS_GAME_INFO = "seasonPassEnterInfoV2";

        public static int MAX_REWARD_CELL_ITEM_COUNT = 2;
        public static string BUNDLE_NAME = "epicpassalways";

        public static string ON_LEVEL_UP_EVENT = "OnEpicPassLevelUp";
        public static string ON_REFRESH_EVENT = "OnEpicPassRefresh";
        public static string ON_REWARD_REFRESH = "OnEpicPassRewardRefresh";
        public static string ON_POINT_REFRESH = "OnEpicPassPointRefresh";

        private const string EPIC_PASS_REWARD_INFO_LIST = "rewardInfoList";
        private const string EPIC_PASS_REWARD_RESULT_LIST = "rewardResultList";
        private const string EPIC_PASS_REWARD_FREE_RESULT_LIST = "rewardFreeResultList";
        private const string EPIC_PASS_REWARD_PAID_RESULT_LIST = "rewardPaidResultList";

        private static bool isReset = false;
        private static bool isClickProcess = false;
        public static string contextId { get; set; }

        private static Blackboard epicPassInfo = null;
        public static Blackboard EpicPassInfo
        {
            get
            {
                if (epicPassInfo == null)
                {
                    var info = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), EPIC_PASS_GAME_INFO);
                    if (info != null)
                        epicPassInfo = info.value;
                }

                return epicPassInfo;
            }
        }

        private static List<Blackboard> gaugeLevelList;
        private static List<Blackboard> GaugeLevelList
        {
            get
            {
                if (gaugeLevelList != null && gaugeLevelList.Count > 0)
                {
                    if (gaugeLevelList[0] == null)
                        gaugeLevelList.Clear();
                }

                if (gaugeLevelList == null || gaugeLevelList.Count == 0)
                {
                    if (EpicPassInfo != null)
                    {
                        var list = EpicPassInfo.GetVariable<List<Blackboard>>("gaugeLevelList");
                        if (list != null)
                            gaugeLevelList = list.value;
                    }
                }

                return gaugeLevelList;
            }
        }

        public static int Level
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<int>("level");

                return 1;
            }
        }

        public static int MaxLevel
        {
            get
            {
                if (EpicPassInfo != null)
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
        public static bool IsReset
        {
            get { return isReset; }
            set { isReset = value; }
        }

        public static bool IsClickProcess
        {
            get { return isClickProcess; }
            set { isClickProcess = value; }
        }

        public static bool Paid
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<bool>("isPaid");
                return false;
            }
        }

        public static bool IsDisplayCollectAll
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<bool>("collectAllButtonDisplayed");
                return false;
            }
        }

        public static bool IsEnabledCollectAll
        {
            get
            {
                if (EpicPassInfo != null)
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
            get { return "season_pass_reset"; }
        }

        public static string gemProductId
        {
            get
            {
                EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
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
                if (EpicPassInfo != null)
                {
                    var prevLevel = EpicPassInfo.GetVariable<int>("prevLevel");
                    if (prevLevel != null)
                        return prevLevel.value;
                }

                return Level;
            }
        }

        public static long PrevPoint
        {
            get
            {
                if (EpicPassInfo != null)
                {
                    var prevPoint = EpicPassInfo.GetVariable<long>("prevRequiredPoint");
                    if (prevPoint != null)
                        return PrevRequiredPoint - prevPoint.value;
                }

                return 0L;
            }
        }

        public static long PrevRequiredPoint
        {
            get
            {
                if (EpicPassInfo != null)
                {
                    var prevReqPoint = EpicPassInfo.GetVariable<long>("prevRequiredPointMax");
                    if (prevReqPoint != null)
                        return prevReqPoint.value;
                }

                return 0L;
            }
        }

        public static long EarnPoint
        {
            get
            {
                if (EpicPassInfo != null)
                {
                    var earnPoint = EpicPassInfo.GetVariable<long>("earnPoint");
                    if (earnPoint != null)
                        return earnPoint.value;
                }

                return 0L;
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
                    BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "unclaimedRewardCount", value);
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

        public static string BackgroundImageUrl
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<string>("backgroundImageUrl");
                return "";
            }
        }

        public static string PointIconImageUrl
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<string>("pointIconImageUrl");
                return "";
            }
        }

        public static string TabIconImageUrl
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<string>("tabIconImageUrl");
                return "";
            }
        }

        public static string BigIconImageUrl
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<string>("bigIconImageUrl");
                return "";
            }
        }

        public static List<Blackboard> RewardResultList
        {
            get
            {
                if (EpicPassInfo != null)
                    return BlackboardUtils.FindVariable<List<Blackboard>>(epicPassInfo, EPIC_PASS_REWARD_RESULT_LIST)?.value ?? null;
                return null;
            }
        }

        public static List<Blackboard> RewardFreeResultList
        {
            get
            {
                if (EpicPassInfo != null)
                    return BlackboardUtils.FindVariable<List<Blackboard>>(epicPassInfo, EPIC_PASS_REWARD_FREE_RESULT_LIST)?.value ?? null;
                return null;
            }
        }

        public static List<Blackboard> RewardPaidResultList
        {
            get
            {
                if (EpicPassInfo != null)
                    return BlackboardUtils.FindVariable<List<Blackboard>>(epicPassInfo, EPIC_PASS_REWARD_PAID_RESULT_LIST)?.value ?? null;
                return null;
            }
        }

        public static bool IsPaidReward
        {
            get
            {
                if (EpicPassInfo != null)
                    return EpicPassInfo.GetValue<bool>("isPaidReward");
                return false;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue<bool>(EpicPassInfo, "isPaidReward", value);
            }
        }

        public static int SeasonPassEventId
        {
            get { return BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2)?.id ?? 0; }
        }

        public static bool RefreshInbox
        {
            get
            {
                if (EpicPassInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<bool>(EpicPassInfo, "refreshInbox").value;
                return false;
            }
            private set
            {
                if (EpicPassInfo != null)
                    BlackboardUtils.SetOrCreateValue<bool>(EpicPassInfo, "refreshInbox", value);
            }
        }

        public static int EventId
        {
            get
            {
                if (EpicPassInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<int>(EpicPassInfo, "eventId").value;
                return 0;
            }
            set
            {
                if (EpicPassInfo != null)
                    BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "eventId", value);
            }
        }

        public static bool CheckWelcomePopup
        {
            get
            {
                if (EpicPassInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<bool>(EpicPassInfo, "checkWelcomePopup").value;
                return false;
            }
            set
            {
                if (EpicPassInfo != null)
                    BlackboardUtils.SetOrCreateValue<bool>(EpicPassInfo, "checkWelcomePopup", value);
            }
        }

        public static void ClearRewardResultList()
        {
            if (RewardFreeResultList != null)
                BlackboardUtils.DestroyBlackboardList(epicPassInfo, EPIC_PASS_REWARD_FREE_RESULT_LIST);
            if (RewardPaidResultList != null)
                BlackboardUtils.DestroyBlackboardList(epicPassInfo, EPIC_PASS_REWARD_PAID_RESULT_LIST);
            if (RewardResultList != null)
                BlackboardUtils.DestroyBlackboardList(epicPassInfo, EPIC_PASS_REWARD_RESULT_LIST);
        }

        public static bool CheckRewardToInbox(List<RewardResult> rewardResultList)
        {
            bool hasInbox = false;
            if (rewardResultList == null || rewardResultList.Count == 0)
                hasInbox = false;
            else
            {
                int rewardCount = rewardResultList.Count;
                for (int i = 0; i < rewardCount; ++i)
                {
                    if (rewardResultList[i].isSentToInbox)
                    {
                        hasInbox = true;
                        break;
                    }
                }
            }
            RefreshInbox = hasInbox;
            return hasInbox;
        }
        // Collect
        public static List<Blackboard> UpdateCollectRewards(List<RewardResult> rewardResultList, bool isPaid)
        {
            if (EpicPassInfo == null) return new List<Blackboard>();

            IsPaidReward = isPaid;

            ClearRewardResultList();

            BlackboardUtils.SetOrCreateList(EpicPassInfo, EPIC_PASS_REWARD_RESULT_LIST, rewardResultList, ClientAPI2Blackboard.Serialize);
            BlackboardUtils.SetOrCreateList(EpicPassInfo, EPIC_PASS_REWARD_FREE_RESULT_LIST, rewardResultList, ClientAPI2Blackboard.Serialize);

            return RewardResultList;
        }

        public static List<Blackboard> UpdateCollectAllRewards(List<RewardResult> freeRewardResultList, List<RewardResult> paidRewardResultList)
        {
            ClearRewardResultList();

            if (freeRewardResultList != null)
                BlackboardUtils.SetOrCreateList(EpicPassInfo, EPIC_PASS_REWARD_FREE_RESULT_LIST, freeRewardResultList, ClientAPI2Blackboard.Serialize);
            if (paidRewardResultList != null)
                BlackboardUtils.SetOrCreateList(EpicPassInfo, EPIC_PASS_REWARD_PAID_RESULT_LIST, paidRewardResultList, ClientAPI2Blackboard.Serialize);

            List<RewardResult> rewardResultList = new List<RewardResult>(freeRewardResultList);
            rewardResultList.AddRange(paidRewardResultList);
            BlackboardUtils.SetOrCreateList(EpicPassInfo, EPIC_PASS_REWARD_RESULT_LIST, rewardResultList, ClientAPI2Blackboard.Serialize);

            return RewardResultList;
        }

        public static void UpdateEpicPassRewards(List<SeasonPassRewardInfoV2> rewardInfoList)
        {
            if (EpicPassInfo == null) return;

            var rewardInfoListBB = EpicPassInfo.GetValue<List<Blackboard>>(EPIC_PASS_REWARD_INFO_LIST);
            for (int i = 0; i < rewardInfoListBB.Count; ++i)
            {
                if (rewardInfoList.Count > i)
                {
                    if (rewardInfoList[i].free != null)
                    {
                        var freeReward = rewardInfoListBB[i].GetValue<Blackboard>("free");
                        freeReward.SetValue("isClaimed", rewardInfoList[i].free.isClaimed);
                    }

                    if (rewardInfoList[i].paid != null)
                    {
                        var paidReward = rewardInfoListBB[i].GetValue<Blackboard>("paid");
                        paidReward.SetValue("isClaimed", rewardInfoList[i].paid.isClaimed);
                    }
                }
            }
        }

        public static void UpdateAdsView(long lastAdsViewTimestamp, long nextAdsResetTimestamp)
        {
            if (EpicPassInfo == null) return;

            EpicPassInfo.SetValue("lastAdsViewTimestamp", lastAdsViewTimestamp);
            EpicPassInfo.SetValue("nextAdsResetTimestamp", nextAdsResetTimestamp);
        }

        public static void UpdateAdsClaim(SeasonPassPointUpdateInfoV2 updateInfo, long updateAdsFreePoint, long lastAdsClaimTimestamp)
        {
            if (EpicPassInfo == null) return;

            AdsFreePoint = updateAdsFreePoint;
            EpicPassInfo.SetValue("lastAdsClaimTimestamp", lastAdsClaimTimestamp);
            UpdateSeasonPassPointInfoForBB(updateInfo);

            UnclaimedRewardCount = updateInfo.unclaimedRewardCount;
        }

        public static void UpdateSeasonPassInfo(SeasonPassInfoResponseV2 response)
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), EPIC_PASS_GAME_INFO);
            ClientAPI2Blackboard.Serialize(bb, response.seasonPassInfoV2);

            BackUpInfo();
        }

        public static void UpdateSeasonPassPointInfo(SeasonPassPointUpdateInfoV2 updateInfo)
        {
            if (updateInfo == null) return;
            // Hack code.
            if (updateInfo.unclaimedRewardCount < 0)
                updateInfo.unclaimedRewardCount = UnclaimedRewardCount;
            if (EpicPassInfo == null) return;

            BackUpInfo();

            BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "level", updateInfo.level);
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "requiredPoint", updateInfo.requiredPoint);
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "requiredPointMax", updateInfo.requiredPointMax);
        }

        public static void UpdateSeasonPassPointInfoForBB(SeasonPassPointUpdateInfoV2 updateInfo)
        {
            UpdateSeasonPassPointInfo(updateInfo);
            ClientAPI2Blackboard.Serialize(EpicPassInfo, updateInfo);
        }

        public static void UpdateMetaIcon()
        {
            EventData eventData = new EventData(ON_POINT_REFRESH);
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        public static Blackboard GetGaugeLevelBB(long bet)
        {
            if (EpicPassInfo == null) return null;

            if (GaugeLevelList == null) return null;

            var list = GaugeLevelList;
            Blackboard retBB = null;

            for (int i = 0; i < list.Count; ++i)
            {
                long unitBet = list[i].GetValue<long>("totalBet");

                if (bet < unitBet) continue;
                retBB = list[i];
            }

            return retBB;
        }

        public static float GetCurrentGaugeAsFloat(long totalBet)
        {
            var list = GaugeLevelList;
            if (list == null || list.Count == 0)
                return 0.0f;

            //int currentGaugeLevel = gaugeBB.GetValue<int>("gaugeLevel");
            int currentGaugeLevel = 0;
            int maxGaugeLevel = list[list.Count - 1].GetValue<int>("gaugeLevel");
            for (int i = 0; i < list.Count; ++i)
            {
                if (totalBet < list[i].GetValue<long>("totalBet"))
                    continue;
                currentGaugeLevel = list[i].GetValue<int>("gaugeLevel");
            }

            if (currentGaugeLevel > 0)
                return (float)currentGaugeLevel / (float)maxGaugeLevel;

            return 0.0f;
        }

        public static void BackUpInfo()
        {
            if (EpicPassInfo == null) return;
            // Backup
            BlackboardUtils.SetOrCreateValue<int>(EpicPassInfo, "prevLevel", EpicPassInfo.GetValue<int>("level"));
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "prevRequiredPoint", EpicPassInfo.GetValue<long>("requiredPoint"));
            BlackboardUtils.SetOrCreateValue<long>(EpicPassInfo, "prevRequiredPointMax", EpicPassInfo.GetValue<long>("requiredPointMax"));
        }

        public static void ResetEpicPassVariable()
        {
            IsReset = true;
        }

        public static GameObject GetRewardPopupObject()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Popup Epic Pass Always Rewards Scene",
                PopupManager.Instance.transform,
                "Area");
            popupObj.name = "Popup Epic Pass Reward";
            return popupObj;
        }

        public static void OpenRewardLostScene()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Popup Epic Pass Always Reward Lost Scene",
                PopupManager.Instance.transform,
                "Area"
            );
        }

        public static void OpenRestartScene()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Popup Epic Pass Always Unlock Scene",
                PopupManager.Instance.transform,
                "Area"
            );
            Blackboard popupBB = popupObj.GetComponent<Blackboard>();
            if (popupBB != null)
                BlackboardUtils.SetOrCreateValue(popupBB, "isUnlock", false);
            ReqeustSeasonPassInfo();
        }

        public static void OpenRewardScene()
        {
            GetRewardPopupObject();
            ReqeustSeasonPassInfo();
        }

        public static void OpenRewardInboxScene()
        {
            var popupObj = MetaObjectUtils.MakeScene(
                MetaStringDefine.LOBBY_BUNDLE_NAME,
                "Popup Epic Pass Always Reward Inbox Scene",
                PopupManager.Instance.transform,
                "Area"
            );
        }

        public static void SetPaid(int unclaimedRewardCount)
        {
            if (EpicPassInfo == null) return;

            EpicPassInfo.SetValue("isPaid", true);
            UnclaimedRewardCount = unclaimedRewardCount;
            //EpicPassInfo.SetValue("unclaimedRewardCount", unclaimedRewardCount);
            EpicPassInfo.SetValue("collectAllEnabled", true);
        }

        public static void CollectAll()
        {
            if (EpicPassInfo == null) return;

            UnclaimedRewardCount = 0;

            var rewardInfoListBB = EpicPassInfo.GetValue<List<Blackboard>>(EPIC_PASS_REWARD_INFO_LIST);
            for (int i = 0; i < rewardInfoListBB.Count; ++i)
            {
                if (i >= Level)
                    break;

                var freeReward = rewardInfoListBB[i].GetValue<Blackboard>("free");
                if (freeReward != null)
                    freeReward.SetValue("isClaimed", true);

                if (Paid)
                {
                    var paidReward = rewardInfoListBB[i].GetValue<Blackboard>("paid");
                    if (paidReward != null)
                        paidReward.SetValue("isClaimed", true);
                }
            }
        }
        // Request API
        public static void ReqeustSeasonPassInfo()
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            if (metaGameInfo == null)
                return;

            BagelCodeClientAPI.SeasonPassInfoRequestV2(metaGameInfo.id,
                (response) =>
                {
                    if (EpicPassInfo == null) return;
                    UpdateSeasonPassInfo(response);

                    EventData eventData = new EventData(ON_REFRESH_EVENT);
                    MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, eventData);
                },
                (error) =>
                {
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_REQUEST_ERROR:
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            );
        }

        public static void RequestEpicPassReset()
        {
            Blackboard AEProduct = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(EpicPassInfo, "AEProduct");
            ProductUtils.MakeAEProduct(AEProduct, gemProductId, SeasonPassResetType);
            BiEventUtils.ItemClick(AEProduct, contextId, false);

            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
            BagelCodeClientAPI.RequestSeasonPassResetV2(metaGameInfo.id,
                (response) =>
                {
                    if (EpicPassInfo == null) return;

                    ResetEpicPassVariable();

                    BiEventUtils.GemTransaction(AEProduct, contextId, true, false);
                    BiEventUtils.ItemAcquired(AEProduct, contextId, false);
                    UnclaimedRewardCount = 0;

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    List<RewardInfo> nextRewardList = response.nextRewardList;
                    BlackboardUtils.SetOrCreateList(EpicPassInfo, "nextRewardList", nextRewardList, ClientAPI2Blackboard.Serialize);

                    // Notice Rewards have been send to inbox
                    if (response.freeRewardResultList.Count > 0 || response.paidRewardResultList.Count > 0)
                    {
                        RefreshInbox = true;
                        OpenRewardInboxScene();
                    }
                    else
                    {
                        OpenRestartScene();
                    }
                },
                (error) =>
                {
                    if (EpicPassInfo != null)
                        BiEventUtils.GemTransaction(AEProduct, contextId, false, false);
                    switch (error.errorCode)
                    {
                        case Error.INVALID_SEASON_PASS_LEVEL_OR_REWARD_CONDITION_ERROR:
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
           );
        }

        public static void RequestRefreshInbox()
        {
            if (!RefreshInbox)
                return;

            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new EventData(InboxEvent.ON_COLLECT_COMMON_REWARDS));

            RefreshInbox = false;
        }

        public static void SendWelcomeVIPLoungePopup(GameObject obj)
        {
            if (CheckWelcomePopup && BlackboardQueryUtils.IsVipLoungeEnabled())
            {
                var eventData = new EventData<GameObject>(VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, obj);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                CheckWelcomePopup = false;
            }
        }
    }
}