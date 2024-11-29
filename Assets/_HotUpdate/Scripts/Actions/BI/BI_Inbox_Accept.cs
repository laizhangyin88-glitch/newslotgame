using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_Inbox_Accept : ActionTask<Blackboard>
{
    public BBParameter<RewardCauseType> rewardCauseType;

    protected override string info
    {
        get { return string.Format("BI Inbox Accept ({0})", rewardCauseType); }
    }

    protected override void OnExecute()
    {
        var inboxInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "inboxInfo");
        var response = BlackboardUtils.FindVariable<Blackboard>(null, "/inboxResponse/" + string.Format("ID_{0}", inboxInfo.value.GetValue<int>("id")));

        // if(response != null)
        // {
        //     switch(rewardCauseType.value)
        //     {
        //         case RewardCauseType.GM_GIFT:
        //             GmGift(response.value);
        //             break;
        //         case RewardCauseType.GIVEAWAY:
        //             Freebie(inboxInfo.value, response.value);
        //             break;
        //         case RewardCauseType.LOYALTY_STAMP_REWARD:
        //             DailyBingo(response.value);
        //             break;
        //         case RewardCauseType.SOCIAL_BOAST_REWARD:
        //             FacebookShareReward(response.value);
        //             break;
        //         case RewardCauseType.MYSTERY_GIFT:
        //             MysteryGift(response.value);
        //             break;
        //         case RewardCauseType.TOURNAMENT_WIN:
        //             TournamentCollect(response.value);
        //             break;
        //         case RewardCauseType.COUPON:
        //             CouponCollect(inboxInfo.value, response.value);
        //             break;
        //         case RewardCauseType.DAILY_DELIVERY:
        //             DailyDeliveryCollect(response.value);
        //             break;
        //         case RewardCauseType.SOCIAL_CREDIT:
        //             SocialReward(response.value);
        //             break;
        //         case RewardCauseType.FCFS_TICKET:
        //             FacebookFirstClickerCollect(response.value);
        //             break;
        //     }
        // }

        if(response != null)
        {
            switch(response.value.GetValue<InboxTypes>("inboxType"))
            {
                case InboxTypes.REWARD:
//                case InboxTypes.GAME_COMPENSATION:
                    InboxReward(inboxInfo.value, response.value);
                    break;
                // case InboxTypes.MESSAGE:
                //     break;
                // case InboxTypes.MESSAGE_WARNING:
                //     break;
                // case InboxTypes.GAME_COMPENSATION:
                //     break;
                // case InboxTypes.FACEBOOK_FRIEND_CONNECT:
                //     break;
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

        EndAction();
    }

    // // Reward 
    // private void GmGift(Blackboard inboxResponse)
    // {
    //     var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

    //     if(inboxType.value != InboxTypes.REWARD)
    //         return;
    //     if(inboxResponse.GetValue<RewardType>("rewardType") == RewardType.PURCHASE_COUPON)
    //         return;

    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, "gm_gift", inboxResponse, out customEventName);

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    // // Reward
    // private void Freebie(Blackboard inboxInfo, Blackboard inboxResponse)
    // {
    //     var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

    //     if(inboxType.value != InboxTypes.REWARD)
    //         return;
    //     if(inboxResponse.GetValue<RewardType>("rewardType") == RewardType.PURCHASE_COUPON)
    //         return;

    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, "freebie", inboxResponse, out customEventName);
    //     eventData["freebie_id"] = inboxInfo.GetValue<int>("eventId");

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    // // Reward
    // private void DailyBingo(Blackboard inboxResponse)
    // {
    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, "bingo_of_the_month", inboxResponse, out customEventName);
    //     eventData["action_type"] = "collect";
    //     eventData["current_claim_count"] = 0; // collect only 0.

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    // Reward
    // private void MysteryGift(Blackboard inboxResponse)
    // {
    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, "mystery_gift", inboxResponse, out customEventName);
    //     eventData["action_type"] = "collect";

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    // // Share
    // private void FacebookShareReward(Blackboard inboxResponse)
    // {
    //     var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

    //     var eventData = new Dictionary<string, object>();
    //     string customEventName = "";

    //     if (inboxType.value == InboxTypes.REWARD)
    //     {
    //         BiEventUtils.AppendRewardEventData(eventData, "fb_share_reward", inboxResponse, out customEventName);
    //     }
    //     else if (inboxType.value == InboxTypes.FACEBOOK_SHARE)
    //     {
    //         customEventName = "client_gift_rp";
    //         eventData["type"] = "fb_share_reward";
    //         eventData["reward"] = inboxResponse.GetValue<long>("rp");
    //     }

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    // Tournament
    // private void TournamentCollect(Blackboard inboxResponse)
    // {
    //     var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

    //     if(inboxType.value != InboxTypes.TOURNAMENT_WIN)
    //         return;

    //     var eventData = new Dictionary<string, object>();
    //     eventData["tournament_id"]  = inboxResponse.GetValue<string>("tournamentId");
    //     eventData["round"]          = inboxResponse.GetValue<int>("serialWinCount") + 1;
    //     eventData["rank"]           = inboxResponse.GetValue<int>("rank");
    //     eventData["earn_coin"]      = inboxResponse.GetValue<long>("actualWinCredit");

    //     Analytics.CustomEvent("client_tournament_collect", eventData);
    // }

    // Reward??? InboxType????????Coupon???????????????????????????????
    private void CouponCollect(Blackboard inboxInfo, Blackboard inboxResponse)
    {
        var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

        if(inboxType.value != InboxTypes.REWARD)
            return;

        string rewardType   = "coin";
        long   rewardCredit = BlackboardUtils.FindVariable<long>(inboxInfo, "reward/credit").value;
        string couponCode   = inboxInfo.GetValue<string>("couponCode");

        var eventData = new Dictionary<string, object>();
        eventData["coupon_id"]          = couponCode;
        eventData["type_of_reward"]     = rewardType;
        eventData["amount_of_reward"]   = rewardCredit;

        Analytics.CustomEvent("client_coupon_collect", eventData);
    }

    // // RewardCauseType???????????????
    // private void DailyDeliveryCollect(Blackboard inboxResponse)
    // {
    //     var rewardType = inboxResponse.GetValue<RewardType>("rewardType");
    //     string deliveryCauseType = "";

    //     switch(rewardType)
    //     {
    //         case RewardType.CREDIT:
    //             deliveryCauseType = "daily_delivery_coin";
    //             break;
    //         case RewardType.GAME_SPIN:
    //             deliveryCauseType = "daily_delivery_game_spin";
    //             break;
    //     }

    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, deliveryCauseType, inboxResponse, out customEventName);
    //     eventData["action_type"] = "collect";

    //     Analytics.CustomEvent(customEventName, eventData);
    // }

    // // Reward
    // private void SocialReward(Blackboard inboxResponse)
    // {
    //     var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

    //     if(inboxType.value != InboxTypes.REWARD)
    //         return;
    //     if(inboxResponse.GetValue<RewardType>("rewardType") == RewardType.PURCHASE_COUPON)
    //         return;

    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, "social_reward", inboxResponse, out customEventName);

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    // // Reward FSFC
    // private void FacebookFirstClickerCollect(Blackboard inboxResponse)
    // {
    //     var inboxType = BlackboardUtils.FindVariable<InboxTypes>(inboxResponse, "inboxType");

    //     if(inboxType.value != InboxTypes.REWARD)
    //         return;

    //     var eventData = new Dictionary<string, object>();
    //     string customEventName;
    //     BiEventUtils.AppendRewardEventData(eventData, "fb_share_clicker_reward", inboxResponse, out customEventName);

    //     if (!string.IsNullOrEmpty(customEventName))
    //         Analytics.CustomEvent(customEventName, eventData);
    // }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void InboxReward(Blackboard inboxInfo, Blackboard inboxResponse)
    {
        if(inboxResponse.GetValue<RewardType>("rewardType") == RewardType.PURCHASE_COUPON)
            return;

        if(rewardCauseType.value == RewardCauseType.COUPON)
        {
            string rewardType   = "coin";
            long   rewardCredit = BlackboardUtils.FindVariable<long>(inboxInfo, "reward/credit").value;
            string couponCode   = inboxInfo.GetValue<string>("couponCode");

            var eventData = new Dictionary<string, object>();
            eventData["coupon_id"]          = couponCode;
            eventData["type_of_reward"]     = rewardType;
            eventData["amount_of_reward"]   = rewardCredit;

            Analytics.CustomEvent("client_coupon_collect", eventData);
        }
        else
        {
            var eventData = new Dictionary<string, object>();
            string customEventName;
            BiEventUtils.CommonAppendRewardEventData(eventData, GetRewardCauseTypeString(rewardCauseType.value), inboxResponse, out customEventName);

            switch(rewardCauseType.value)
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
                        var rewardType = inboxResponse.GetValue<RewardType>("rewardType");
                        switch(rewardType)
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

    private void InboxFacebookShareReward(Blackboard inboxResponse)
    {
        var eventData = new Dictionary<string, object>();
        eventData["type"] = "fb_share_reward";
        eventData["reward"] = inboxResponse.GetValue<long>("rp");

        Analytics.CustomEvent("client_gift_rp", eventData);
    }

    private void InboxTournamentWin(Blackboard inboxResponse)
    {
        var eventData = new Dictionary<string, object>();
        eventData["tournament_id"]    = inboxResponse.GetValue<string>("tournamentId");
        eventData["round"]            = inboxResponse.GetValue<int>("serialWinCount") + 1;
        eventData["rank"]             = inboxResponse.GetValue<int>("rank");
        eventData["earn_coin"]        = inboxResponse.GetValue<long>("actualWinCredit");
        eventData["prize_multiplier"] = inboxResponse.GetValue<double>("serialWinBonus");

        Analytics.CustomEvent("client_tournament_collect", eventData);
    }

    private string GetRewardCauseTypeString(RewardCauseType causeType)
    {
        switch(causeType)
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
