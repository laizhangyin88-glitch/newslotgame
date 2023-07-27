using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode
{

    public static partial class BiEventUtils
    {
        // For Bi client_lobby_status.
        public static string slotListData = null;
        public static string slotBannerGroupListData = null;
        public static string favoriteSlotIDListData = null;

        // public static object slotList = null;
        // public static object slotBannerGroupList = null;
        // public static object favoriteSlotIdList = null;

        public static void SetLobbyStatusData(LobbyResponseV6 response)
        {
            slotListData = SlotSimpleJson.SerializeObject(response.slotList);
            slotBannerGroupListData = SlotSimpleJson.SerializeObject(response.slotBannerGroupList);
            favoriteSlotIDListData = SlotSimpleJson.SerializeObject(response.favoriteSlotIdList);

            // slotList = response.slotList;
            // slotBannerGroupList = response.slotBannerGroupList;
            // favoriteSlotIdList = response.favoriteSlotIdList;
        }

        public static void AppendLobbyStatusData(Dictionary<string, object> eventData)
        {
            // eventData["slot_list"] = slotList;
            // eventData["slb_group_list"] = slotBannerGroupList;
            // eventData["favorite_slot_id_list"] = favoriteSlotIdList;

            eventData["slot_list"] = slotListData;
            eventData["slb_group_list"] = slotBannerGroupListData;
            eventData["favorite_slot_id_list"] = favoriteSlotIDListData;
        }

        public static string AddZeroPadding(int padding, object value)
        {
            string format = "";
            for (int i = 0; i < padding; ++i)
            {
                format += "0";
            }

            return ((IFormattable)value).ToString(format, null);
        }

        public static void CommonAppendRewardEventData(Dictionary<string, object> eventData, string causeType, Blackboard response, out string eventId)
        {
            eventData["type"] = causeType;
            eventId = "";

            bool isInbox = response.GetValue<bool>("isSentToInbox");

            Blackboard rewardInfo = isInbox ? response.GetValue<Blackboard>("rewardInfo") : response;

            var rewardType = response.GetValue<RewardType>("rewardType");
            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    eventId = "client_gift_coin";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["earn_coin"] = rewardInfo.GetValue<long>("credit");
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["earn_coin"] = rewardInfo.GetValue<long>("credit");
                    }
                    break;
                case RewardType.RP:
                    eventId = "client_gift_rp";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["reward"] = rewardInfo.GetValue<long>("rp");
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["reward"] = rewardInfo.GetValue<long>("rp");
                    }
                    break;

                case RewardType.DAILY_BOOST:
                    eventId = "client_gift_daily_boost";
                    if (isInbox)
                    {
                        int tier = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;
                        long baseCoins = BlackboardUtils.FindVariable<long>(rewardInfo, "baseCreditPerDay").value;
                        long totalCoins = TierUtils.GetTierFractionCoin(baseCoins, tier);
                        totalCoins = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(totalCoins, rewardInfo, "dailyBoost");

                        eventData["action_type"] = "trigger";
                        eventData["coin_per_day"] = totalCoins;
                        eventData["total_day_count"] = BlackboardUtils.FindVariable<int>(rewardInfo, "totalDayCount").value;
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["coin_per_day"] = rewardInfo.GetValue<long>("earnCredit");
                        eventData["total_day_count"] = BlackboardUtils.FindVariable<int>(rewardInfo, "dailyBoost/totalCount").value;
                    }
                    break;

                case RewardType.TIER_UPGRADE:
                    eventId = "client_gift_tier_upgrade";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["target_tier"] = rewardInfo.GetValue<int>("targetTier");
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["target_tier"] = rewardInfo.GetValue<int>("upgradedTier");
                    }
                    break;

                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    eventId = "client_gift_wheel";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["added_wheel_count"] = BlackboardUtils.FindVariable<int>(rewardInfo, "spinCount").value;
                        eventData["total_wheel_count"] = BlackboardQueryUtils.GetDailySpinCount(MetaJackpotType.DAILY_BONUS);
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["added_wheel_count"] = rewardInfo.GetValue<int>("addedSpinCount");
                        eventData["total_wheel_count"] = rewardInfo.GetValue<int>("totalSpinCount");
                    }
                    break;

                case RewardType.EXP_MULTIPLY:
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    eventId = "client_gift_exp_boost";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        long numerator = rewardInfo.GetValue<long>("multiplierNumerator");
                        eventData["multiplier"] = NumberUtils.GetMultiplierFromNumerator(numerator);
                        eventData["duration"] = ((long)(rewardInfo.GetValue<int>("durationSec")) * 1000L);
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        long numerator = rewardInfo.GetValue<long>("multiplierNumerator");
                        eventData["multiplier"] = NumberUtils.GetMultiplierFromNumerator(numerator);
                        eventData["duration"] = ((long)(rewardInfo.GetValue<int>("durationSec")) * 1000L);
                    }
                    break;

                case RewardType.GAME_SPIN:
                    eventId = "client_gift_game_spin";

                    eventData["bet_zone_id"] = "free";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["game_id"] = rewardInfo.GetValue<int>("gameId");
                        eventData["bet"] = rewardInfo.GetValue<long>("bet");
                        eventData["added_spin_count"] = rewardInfo.GetValue<int>("spinCount");
                        // eventData["total_spin_count"] = BlackboardQueryUtils.GetGameSpinTotalCount(gameID);
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["game_id"] = rewardInfo.GetValue<int>("gameId");
                        eventData["bet"] = rewardInfo.GetValue<long>("bet");
                        eventData["added_spin_count"] = rewardInfo.GetValue<int>("addedSpinCount");
                        eventData["total_spin_count"] = rewardInfo.GetValue<int>("totalSpinCount");
                    }
                    break;

                case RewardType.GAME_DEAL:
                    eventId = "client_gift_game_deal";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["game_id"] = rewardInfo.GetValue<int>("gameId");
                        eventData["bet_per_hand"] = rewardInfo.GetValue<long>("betPerHand");
                        eventData["hand_count"] = rewardInfo.GetValue<int>("handCount");
                        eventData["added_spin_count"] = rewardInfo.GetValue<int>("count");
                        // eventData["total_spin_count"] = BlackboardQueryUtils.GetGameSpinTotalCount(gameID);
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["game_id"] = rewardInfo.GetValue<int>("gameId");
                        eventData["bet_per_hand"] = rewardInfo.GetValue<long>("betPerHand");
                        eventData["hand_count"] = rewardInfo.GetValue<int>("handCount");
                        eventData["added_spin_count"] = rewardInfo.GetValue<int>("addedCount");
                        eventData["total_spin_count"] = rewardInfo.GetValue<int>("totalCount");
                    }
                    break;

                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    eventId = "client_gift_buy_bonus";
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["game_id"] = rewardInfo.GetValue<int>("gameId");
                        eventData["bet"] = rewardInfo.GetValue<long>("baseBet");
                        eventData["extra_bet"] = rewardInfo.GetValue<long>("extraBet");
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["game_id"] = rewardInfo.GetValue<int>("gameId");
                        eventData["ticketed_bonus_ticket_id"] = rewardInfo.GetValue<int>("ticketId");
                    }
                    break;
                case RewardType.SCRATCHER:
                    eventId = "client_gift_scratcher";
                    var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
                    if (isInbox)
                    {
                        eventData["action_type"] = "trigger";
                        eventData["scratcher_preset_id"] = rewardInfo.GetValue<int>("scratcherPresetId");
                        if (eventInfo != null)
                        {
                            eventData["collecting_game_event_id"] = eventInfo.id;
                            eventData["collecting_game_id"] = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
                        }

                        eventData["scratcher_type"] = rewardInfo.GetValue<RewardScratcherRule>("scratcherRule").ToString();
                    }
                    else
                    {
                        eventData["action_type"] = "collect";
                        eventData["scratcher_preset_id"] = rewardInfo.GetValue<int>("scratcherPresetId");
                        if (eventInfo != null)
                        {
                            eventData["collecting_game_event_id"] = eventInfo.id;
                            eventData["collecting_game_id"] = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
                        }

                        eventData["scratcher_type"] = rewardInfo.GetValue<RewardScratcherRule>("scratcherRule").ToString();
                    }
                    break;
                default:
                    eventId = "";
                    break;
            }
        }

        public static void AppendCommonRewardEventData(Dictionary<string, object> eventData, Blackboard response)
        {
            var rewardType = response.GetValue<RewardType>("rewardType");
            switch (rewardType)
            {
                case RewardType.CREDIT:
                    eventData["type_of_reward"] = "coin";
                    eventData["amount_of_reward"] = response.GetValue<long>("credit");
                    break;
                case RewardType.RP:
                    eventData["type_of_reward"] = "rp";
                    eventData["amount_of_reward"] = response.GetValue<long>("rp");
                    break;
                case RewardType.DAILY_BOOST:
                    eventData["type_of_reward"] = "daily_boost";
                    eventData["amount_of_reward"] = response.GetValue<long>("earnCredit");
                    break;
                case RewardType.TIER_UPGRADE:
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    eventData["type_of_reward"] = "daily_spin";
                    eventData["amount_of_reward"] = response.GetValue<int>("addedSpinCount");
                    break;
                case RewardType.GAME_SPIN:
                    break;
                case RewardType.GEM:
                    eventData["type_of_reward"] = "gem";
                    eventData["amount_of_reward"] = response.GetValue<long>("gem");
                    break;
            }
        }

        public static bool AppendCommonEventData(string eventName, Dictionary<string, object> eventData)
        {
            if( ! BlackboardQueryUtils.AppendBackUpMeInfo(eventData) )  return false;

            eventData["id"] = Guid.NewGuid().ToString();
            eventData["event_id"] = eventName;
            eventData["ts"] = TimeUtils.GetTimeStamp();
            eventData["day"] = Convert.ToInt32(TimeUtils.GetPSTDateString(TimeUtils.GetTimeStamp(), "yyyyMMdd"));
            eventData["device_id"] = NativeHelper.Instance.GetDeviceID();
            eventData["os"] = ApplicationSettings.GetPlatformName().ToUpper();
            eventData["spt"] = TimeUtils.GetSessionPlayTime();
            eventData["total_spt"] = TimeUtils.GetTotalSessionPlayTime();
            eventData["client_number_version"] = ApplicationSettings.GetClientVersionNumber();
            eventData["user_club_id"] = BlackboardUtils.FindValue<long>("/me/clubId");

            eventData["adid"] = NativeHelper.Instance.GetAdjustID();
            if (eventData["adid"] == null)
                eventData["adid"] = "";

            eventData["tracker_name"] = "";
            var trackerNameVariable = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "trackerName");
            if (trackerNameVariable != null)
            {
                eventData["tracker_name"] = trackerNameVariable.value;
            }
            else
            {
                var deviceAdjustDataVariable = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "deviceAdjustData");
                if (deviceAdjustDataVariable != null)
                    eventData["tracker_name"] = deviceAdjustDataVariable.value.GetValue<string>("trackerName");
            }

            return true;
        }

        public static void GetIAMData(Blackboard agentBB, out object iamId, out object iamType, out object isAction)
        {
            var inAppMessageId = BlackboardUtils.FindVariable<int>(agentBB, "iamId");
            var triggerType = BlackboardUtils.FindVariable<BagelCode.ClientModels.InAppMessageTriggerType>(agentBB, "triggerType");

            iamId = 0;
            isAction = false;
            iamType = "";

            if (inAppMessageId != null && triggerType != null)
            {
                iamId = inAppMessageId.value;
                iamType = triggerType.value.ToString();
                isAction = triggerType.value == BagelCode.ClientModels.InAppMessageTriggerType.UNKNOWN ? true : false;
            }
        }

        public static string GetSpinTypeName(SpinType spinType)
        {
            switch (spinType)
            {
                case SpinType.GameSpin:
                    return "game_spin";
                case SpinType.BonusSpin:
                    return "bonus_spin";
                case SpinType.BuyABonus:
                    return "buy_bonus";
            }

            return "default";
        }

        public static string GetStringFromItemType(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.CREDIT:
                    return "coin";
                case ItemType.DAILY_BOOST:
                    return "daily_boost";
                case ItemType.TIER_BOOST:
                    return "tier_boost";
                case ItemType.CREDIT_POT_OF_GOLD:
                    return "coin_pot_of_gold";
                case ItemType.CREDIT_MULTIPLIER_WHEEL:
                    return "coin_booster";
                case ItemType.DAILY_BONUS_WHEEL:
                    return "wheel";
                case ItemType.PIGGY_BANK:
                    return "pot_of_gold";
                case ItemType.EARLY_ACCESS:
                    return "early_access";
                case ItemType.CREDIT_WHEEL:
                    return "coin_wheel";
                case ItemType.TICKETED_BONUS_TICKET:
                    return "buy_bonus";
                //case ItemType.DAILY_DELIVERY_CREDIT:
                //    return "daily_delivery_coin";
                //case ItemType.DAILY_DELIVERY_GAME_SPIN:
                //    return "daily_delivery_game_spin";
                case ItemType.DAILY_MEGA_WHEEL:
                    return "mega_wheel";
                case ItemType.POG_BOOSTER:
                    return "pot_of_gold_booster";
                case ItemType.GEM:
                    return "gem";
                case ItemType.GEM_BOOSTER:
                    return "gem_booster";
                case ItemType.EPIC_PASS:
                    return "epic_pass";
                case ItemType.EPIC_PASS_V2:
                    return "epic_pass_v2";
                case ItemType.SPIN_BOOST:
                    return "spin_boost";
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    return "finder";
                case ItemType.TICKETED_BONUS_BOOSTER:
                    return "ticketed_bonus_booster";
            }
#if DEV
            Debug.LogError("item type is UNKNOWN.");
#endif
            return "UNKNOWN";
        }

        public static void AppendCommonProductEventData(Dictionary<string, object> customData, Blackboard productBB)
        {
            string type = null;
            var itemList = BlackboardUtils.FindVariable<List<Blackboard>>(productBB, "itemList");
            if (itemList != null)
            {
                var itemType = BlackboardUtils.FindVariable<ItemType>(itemList.value[0], "itemType");
                if (itemType != null) type = GetStringFromItemType(itemType.value);
            }
            else
            {
                var itemType = BlackboardUtils.FindVariable<string>(productBB, "itemType");
                if (itemType != null) type = itemType.value;
            }

            if (!string.IsNullOrEmpty(type)) customData["type"] = type;

            var productID = BlackboardUtils.FindVariable<int>(productBB, "id");
            if (productID != null) customData["product_id"] = productID.value;

            var price = BlackboardUtils.FindVariable<double>(productBB, "price");
            if (price != null) customData["price"] = (int)((price.value + 0.00001) * 100);

            var gemPrice = BlackboardUtils.FindVariable<long>(productBB, "gemPrice");
            if (gemPrice != null) customData["gem_price"] = gemPrice.value;

            var gemProductId = BlackboardUtils.FindVariable<string>(productBB, "gemProductId");
            if (gemProductId != null) customData["gem_product_id"] = gemProductId.value;
        }

        public static void AppendCommonPurchaseEventData(Dictionary<string, object> customData, Blackboard purchaseResponse,
            Blackboard productBB, object IAMType, object IAMId, object isActionIAM, bool isMultiplierEventPersonal, bool isSaleEventPersonal
        )
        {
            customData["iam_id"] = IAMId;
            customData["iam_trigger_type"] = IAMType;
            customData["is_iam_action"] = isActionIAM;
            customData["product_id"] = productBB.GetValue<int>("id");
            customData["price"] = (int)((productBB.GetValue<double>("price") + 0.00001) * 100);
            customData["original_price"] = (int)((productBB.GetValue<double>("originalPrice") + 0.00001) * 100);
            customData["nth_purchase"] = purchaseResponse.GetValue<int>("purchaseCount");
            customData["distance_from_last_purchase"] = purchaseResponse.GetValue<long>("distanceFromLastPurchaseTimestamp");
            customData["is_multiplier_event_personal"] = isMultiplierEventPersonal;
            customData["is_sale_event_personal"] = isSaleEventPersonal;
        }

        public static void AppendCommonActionData(Dictionary<string, object> customData, Blackboard actionBB, string typeName, string targetName)
        {
            var actionType = BlackboardUtils.FindVariable<ActionType>(actionBB, "type");

            customData[typeName] = actionType.value.ToString();

            string actionTarget = null;

            switch (actionType.value)
            {
                // case ActionType.NONE:
                //     break;
                case ActionType.OPEN_LINK:
                    actionTarget = BlackboardUtils.FindVariable<string>(actionBB, "url").value;
                    break;
                case ActionType.OPEN_POPUP:
                    actionTarget = BlackboardUtils.FindVariable<ActionOpenPopupType>(actionBB, "popupType").value.ToString();
                    break;
                // case ActionType.FACEBOOK_CONNECT:
                //     break;
                // case ActionType.FACEBOOK_INVITE:
                //     break;
                // case ActionType.PURCHASE_PRODUCT:
                //     public Product product = null;
                //     break;
                case ActionType.ENTER_GAME:
                    //            case ActionType.SELECT_GAME_BET_ZONE:
                    actionTarget = BlackboardUtils.FindVariable<int>(actionBB, "gameId").value.ToString();
                    break;
                case ActionType.OPEN_PROFILE_POPUP:
                    actionTarget = BlackboardUtils.FindVariable<string>(actionBB, "targetUserId").value;
                    break;
                // case ActionType.OPEN_INVITE_POPUP:
                //     public string userId = "";
                //     public string name = "";
                //     public string profileUrl = "";
                //     public int tier = 0;
                //     public int gameId = 0;
                //     public string roomId = "";
                //     break;
                case ActionType.COUPON_REDEEM:
                    actionTarget = BlackboardUtils.FindVariable<string>(actionBB, "code").value;
                    break;
                case ActionType.TRIGGER_IAM:
                    actionTarget = BlackboardUtils.FindVariable<int>(actionBB, "id").value.ToString();
                    break;
                case ActionType.CHANGE_SCENE:
                    actionTarget = BlackboardUtils.FindVariable<ActionChangeSceneType>(actionBB, "sceneType").value.ToString();
                    break;
                case ActionType.OPEN_NOTICE_ACTION_POPUP:
                    actionTarget = BlackboardUtils.FindVariable<int>(actionBB, "id").value.ToString();
                    break;
                case ActionType.ENTER_GAME_FAVORITE:
                    int favoriteId = BlackboardUtils.FindVariable<int>(actionBB, "gameId").value;
                    var favoriteList = MainBlackboard.Get().GetValue<List<int>>("favoriteSlotIdList");
                    if (favoriteList != null && favoriteList.Count != 0)
                        favoriteId = favoriteList[0];
                    actionTarget = favoriteId.ToString();
                    break;
            }

            if (!string.IsNullOrEmpty(actionTarget))
                customData[targetName] = actionTarget;
        }

        public static void AppendCollectingGameEventData(Dictionary<string, object> eventData)
        {
            var eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
            if (eventInfo != null)
            {
                var contraints = (EventDataCollectingGame)eventInfo.constraints;

                if (contraints != null)
                {
                    eventData["collecting_game_id"] = contraints.collectingGameId;
                    eventData["collecting_game_name"] = contraints.collectingGameName;
                }
            }
        }

        public static void AppendNoticeActionData(Dictionary<string, object> customData, Blackboard actionBB)
        {
            AppendCommonActionData(customData, actionBB, "notice_action_type", "notice_action_target");
        }

        public static void AppendSLBActionData(Dictionary<string, object> customData, Blackboard actionBB)
        {
            AppendCommonActionData(customData, actionBB, "slb_action_type", "slb_action_target");
        }

        public static string GenerateContextID()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetSlotEnterContextID()
        {
            var contentBB = ContentBlackboard.Get();
            if (contentBB != null)
            {
                var slotEnterContextID = BlackboardUtils.FindVariable<string>(contentBB, "slotEnterContextId");

                if (slotEnterContextID != null && !string.IsNullOrEmpty(slotEnterContextID.value))
                    return slotEnterContextID.value;
            }

            return null;
        }

        public static void SendBiChallengeClaim(Blackboard response, Blackboard challengeInfo)
        {
            if (response == null || challengeInfo == null)
                return;

            var rewardResultList = response.GetValue<List<Blackboard>>("rewardResultList");
            var nextChallengeInfo = response.GetValue<Blackboard>("nextChallengeInfo");
            var multiplier = response.GetValue<double>("multiplier");

            var progress = challengeInfo.GetValue<int>("challengeProgress");
            var challengeId = challengeInfo.GetVariable<string>("challengeId")?.value ?? string.Empty;

            for (int i = 0; i < rewardResultList.Count; i++)
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();

                var challengeType = BlackboardUtils.FindVariable<ChallengeType>(nextChallengeInfo, "challengeType");
                customData["challenge_type"] = challengeType.value.ToString().ToLower();

                customData["action_type"] = "challenge_claim";

                BiEventUtils.AppendCommonRewardEventData(customData, rewardResultList[i]);

                customData["challenge_progress"] = progress;

                int challengeCompleteCount = BlackboardQueryUtils.GetChallengeMinCount(challengeType.value);
                customData["challenge_complete_count"] = challengeCompleteCount;

                if (challengeId != null && !string.IsNullOrEmpty(challengeId))
                {
                    customData["challenge_id"] = challengeId;
                }

                customData["multiplier"] = multiplier;

                BiEventUtils.AppendLevelMultiplierEventData(customData, "coin");
                Analytics.CustomEvent("client_challenge", customData);
            }
        }

        public static void SendBiClientNotice(Blackboard owner, bool isClick, string noticeInfo, long endTimestamp, ref string contextId)
        {
            var noticeInfoBB = BlackboardUtils.FindVariable<Blackboard>(owner, noticeInfo);

            if (string.IsNullOrEmpty(contextId))
                contextId = GenerateContextID();

            if (noticeInfoBB != null)
            {
                var noticeID = BlackboardUtils.FindVariable<int>(noticeInfoBB.value, "id");
                var title = BlackboardUtils.FindVariable<string>(noticeInfoBB.value, "title");
                var message = BlackboardUtils.FindVariable<string>(noticeInfoBB.value, "message");
                var priority = BlackboardUtils.FindVariable<int>(noticeInfoBB.value, "priority");
                var imageUrl = BlackboardUtils.FindVariable<string>(noticeInfoBB.value, "imageUrl");
                var startTimestamp = BlackboardUtils.FindVariable<long>(noticeInfoBB.value, "startTimestamp");
                // var endTimestamp = BlackboardUtils.FindVariable<long>(noticeInfoBB.value, "endTimestamp");
                var abTestTag = BlackboardUtils.FindVariable<int>(noticeInfoBB.value, "abTestTag");
                var noticeType = BlackboardUtils.FindVariable<NoticeTypes>(noticeInfoBB.value, "type");

                Dictionary<string, object> customData = new Dictionary<string, object>();

                customData["type"] = isClick ? "click" : "trigger";
                customData["notice_id"] = noticeID.value;
                customData["context_id"] = contextId;
                customData["notice_type"] = noticeType.value.ToString();

                var actionBB = BlackboardUtils.FindVariable<Blackboard>(noticeInfoBB.value, "action");
                if (actionBB != null)
                    BiEventUtils.AppendNoticeActionData(customData, actionBB.value);

                customData["priority"] = priority.value;
                customData["ab_test_tag"] = abTestTag.value;
                customData["title"] = title.value;
                customData["message"] = message.value;
                customData["image_url"] = imageUrl.value;

                var constraintsBB = BlackboardUtils.FindVariable<Blackboard>(noticeInfoBB.value, "constraints");
                if (constraintsBB != null)
                {
                    var showEndTimer = BlackboardUtils.FindVariable<bool>(constraintsBB.value, "showEndTimer");
                    var useUserTimer = BlackboardUtils.FindVariable<bool>(constraintsBB.value, "useUserTimer");
                    var userTimeMin = BlackboardUtils.FindVariable<int>(constraintsBB.value, "userTimer");
                    var coolTimeSec = BlackboardUtils.FindVariable<int>(constraintsBB.value, "cooltimeSec");

                    customData["cooltime_sec"] = coolTimeSec.value;
                    customData["show_end_timer"] = showEndTimer.value;
                    customData["use_user_timer"] = useUserTimer.value;
                    customData["user_timer_min"] = userTimeMin.value;

                    if (useUserTimer.value)
                    {
                        long userTimerMS = userTimeMin.value * 60000L;
                        startTimestamp.value = endTimestamp - userTimerMS;
                    }
                    else
                    {
                        endTimestamp = BlackboardUtils.FindVariable<long>(noticeInfoBB.value, "endTimestamp").value;
                    }
                }

                customData["start_timestamp"] = startTimestamp.value;
                customData["end_timestamp"] = endTimestamp;

                Analytics.CustomEvent("client_notice", customData);
            }
        }


        // public static string GetRewardJsonString(RewardType rewardType, object value)
        public static string GetRewardJsonString(RewardType rewardType, params object[] args)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            switch (rewardType)
            {
                case RewardType.CREDIT:
                    customData["coin"] = args[0];
                    break;
                case RewardType.RP:
                    customData["rp"] = args[0];
                    break;
                case RewardType.GEM:
                    customData["gem"] = args[0];
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                    customData["energy"] = args[0];
                    break;
            }
            return SlotMaker.Json.SlotSimpleJson.SerializeObject(customData);
        }

        public static void SendBiEventEnter(string enterType, string eventType, string contextId)
        {
            var customData = new Dictionary<string, object>();
            customData["enter_type"] = enterType;
            customData["type"] = eventType;
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_event_enter", customData);
        }

        public static string GetPopupContextId(GameObject obj)
        {
            Popup popup = obj.GetComponent<Popup>();
            if (popup == null)
                popup = obj.AddComponent<Popup>();

            return popup.GetGuid();
        }
    }
}
