using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public static partial class BiEventUtils
    {
        public static void BiEventInboxAccept(Blackboard inboxInfo, RewardCauseType rewardCauseType)
        {
            var response = BlackboardUtils.FindVariable<Blackboard>(null, "/inboxResponse/" + string.Format("ID_{0}", inboxInfo.GetValue<int>("id")));

            if (response != null)
            {
                switch (response.value.GetValue<InboxTypes>("inboxType"))
                {
                    case InboxTypes.REWARD:
                        InboxReward(inboxInfo, response.value, rewardCauseType);
                        break;
                    case InboxTypes.FACEBOOK_SHARE:
                        InboxFacebookShareReward(response.value);
                        break;
                    case InboxTypes.TOURNAMENT_WIN:
                        InboxTournamentWin(response.value);
                        break;
                    case InboxTypes.TICKETED_BONUS_TICKET:
                    case InboxTypes.TICKETED_BONUS_TICKET_FOR_BOOST:
                        break;
                    case InboxTypes.SOCIAL_CREDIT:
                        break;
                }
            }
        }

        private static void CouponCollect(Blackboard inboxInfo, Blackboard inboxResponse)
        {
            var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

            if (inboxType.value != InboxTypes.REWARD)
                return;

            string rewardType = "coin";
            long rewardCredit = BlackboardUtils.FindVariable<long>(inboxInfo, "reward/credit").value;
            string couponCode = inboxInfo.GetValue<string>("couponCode");

            var eventData = new Dictionary<string, object>();
            eventData["coupon_id"] = couponCode;
            eventData["type_of_reward"] = rewardType;
            eventData["amount_of_reward"] = rewardCredit;

            Analytics.CustomEvent("client_coupon_collect", eventData);
        }

        private static void InboxReward(Blackboard inboxInfo, Blackboard inboxResponse, RewardCauseType rewardCauseType)
        {
            RewardType rewardType = inboxResponse.GetVariable<RewardType>("rewardType")?.value ?? RewardType.UNKNOWN;
            if (rewardType == RewardType.PURCHASE_COUPON)
                return;

            if (rewardCauseType == RewardCauseType.COUPON)
            {
                long amount = 0L;
                if(rewardType == RewardType.CREDIT)
                {
                    amount = BlackboardUtils.FindVariable<long>(inboxResponse, "credit")?.value ?? 0L;
                }
                else if(rewardType == RewardType.GEM)
                {
                    amount = BlackboardUtils.FindVariable<long>(inboxResponse, "gem")?.value ?? 0L;
                }

                string couponCode = inboxInfo.GetValue<string>("couponCode");

                var eventData = new Dictionary<string, object>();
                eventData["coupon_id"] = couponCode;
                eventData["type_of_reward"] = rewardType.ToString();
                eventData["amount_of_reward"] = amount;

                Analytics.CustomEvent("client_coupon_collect", eventData);
            }
            else
            {
                var eventData = new Dictionary<string, object>();
                string customEventName;
                BiEventUtils.CommonAppendRewardEventData(eventData, GetRewardCauseTypeString(rewardCauseType), inboxResponse, out customEventName);

                switch (rewardCauseType)
                {
                    case RewardCauseType.GIVEAWAY:
                        eventData["freebie_id"] = inboxInfo.GetValue<int>("eventId");
                        break;
                    case RewardCauseType.LOYALTY_STAMP_REWARD:
                        eventData["current_claim_count"] = 0; // collect only 0.
                        break;
                    case RewardCauseType.DAILY_DELIVERY:
                        {
                            // override cause type
                            switch (rewardType)
                            {
                                case RewardType.CREDIT:
                                case RewardType.CREDIT_WITH_MULTIPLIER:
                                    eventData["type"] = "daily_delivery_coin";
                                    break;
                                case RewardType.GAME_SPIN:
                                    eventData["type"] = "daily_delivery_game_spin";
                                    break;
                            }
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(customEventName))
                    Analytics.CustomEvent(customEventName, eventData);
            }
        }

        private static void InboxFacebookShareReward(Blackboard inboxResponse)
        {
            var eventData = new Dictionary<string, object>();
            eventData["type"] = "fb_share_reward";
            eventData["reward"] = inboxResponse.GetValue<long>("rp");

            Analytics.CustomEvent("client_gift_rp", eventData);
        }

        private static void InboxTournamentWin(Blackboard inboxResponse)
        {
            var eventData = new Dictionary<string, object>();
            eventData["tournament_id"] = inboxResponse.GetValue<string>("tournamentId");
            eventData["round"] = inboxResponse.GetValue<int>("serialWinCount") + 1;
            eventData["rank"] = inboxResponse.GetValue<int>("rank");
            eventData["earn_coin"] = inboxResponse.GetValue<long>("actualWinCredit");
            eventData["prize_multiplier"] = inboxResponse.GetValue<double>("serialWinBonus");

            Analytics.CustomEvent("client_tournament_collect", eventData);
        }

        private static string GetRewardCauseTypeString(RewardCauseType causeType)
        {
            switch (causeType)
            {
                case RewardCauseType.GM_GIFT:
                    return "gm_gift";
                case RewardCauseType.GIVEAWAY:
                    return "freebie";
                case RewardCauseType.LOYALTY_STAMP_REWARD:
                    return "bingo_of_the_month";
                case RewardCauseType.SOCIAL_BOAST_REWARD:
                    return "fb_share_reward";
                case RewardCauseType.MYSTERY_GIFT:
                    return "mystery_gift";
                // case RewardCauseType.TOURNAMENT_WIN:
                //     break;
                case RewardCauseType.DAILY_DELIVERY:
                    return "daily_delivery_coin";
                case RewardCauseType.SOCIAL_CREDIT:
                    return "social_reward";
                case RewardCauseType.FCFS_TICKET:
                    return "fb_share_clicker_reward";
                case RewardCauseType.PURCHASE_BONUS_REWARD:
                    return "purchase_bonus_reward";
                case RewardCauseType.STATUS_MATCH:
                    return "status_match";
                case RewardCauseType.TIER_MATCH_OFFER:
                    return "tier_match_offer";
                case RewardCauseType.ACTION_REWARD:
                    return "action_reward";
                case RewardCauseType.LUCKY_FIVE:
                    return "lucky_five";
            }

            return causeType.ToString();
        }
    }
}
