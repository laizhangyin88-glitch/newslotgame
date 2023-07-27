using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Friend")]
    public class SetFriendsBB : ActionTask<Blackboard>
    {
        [BlackboardOnly]
        public BBParameter<List<Blackboard>> saveAsOnlineFriends;
        [BlackboardOnly]
        public BBParameter<List<Blackboard>> saveAsOfflineFriends;
        [BlackboardOnly]
        public BBParameter<List<Blackboard>> saveAsRecommandFriends;
        [BlackboardOnly]
        public BBParameter<List<Blackboard>> saveAsRequestFriends;
        [BlackboardOnly]
        public BBParameter<int> receivedGiftCounts;
        [BlackboardOnly]
        public BBParameter<int> collectableCredits;
        [BlackboardOnly]
        public BBParameter<int> sendableGiftCounts;
        [BlackboardOnly]
        public BBParameter<int> sendableCredits;

        [BlackboardOnly]
        public BBParameter<int> requestedFriends;

        protected override string info
        {
            get { return "Set Friends BB"; }
        }

        protected override void OnExecute()
        {
            var friendList = BlackboardQueryUtils.GetFriendList();
            var onlineFriendList = BlackboardUtils.FindVariable<List<string>>(agent, "/onlineFriendUserIdList");
            var recommendedFriendList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "/friendRecommendationList");

            var onlineFriends = new List<Blackboard>();
            var offlineFriends = new List<Blackboard>();
            var requestsFriends = new List<Blackboard>();

            int requestedCount = 0;
            int receivedCount = 0;
            long collectableCredit = 0;
            int sendableCount = 0;
            long sendableCoin = 0;

            if (friendList != null && friendList.Count > 0)
            {
                for (int i = 0; i < friendList.Count; ++i)
                {
                    var friend = friendList[i];
                    bool isOnline = false;

                    if (friend.GetValue<bool>("accepted"))
                    {
                        // isOnline
                        if (onlineFriendList != null)
                        {
                            if (onlineFriendList.value.Contains(friend.GetValue<string>("userId")))
                            {
                                isOnline = true;
                            }
                        }

                        // received gift
                        long giftCount = (long)(friend.GetValue<int>("receivedGiftCount"));
                        long giftCredit = 0;

                        switch (friend.GetValue<BagelCode.ClientModels.FriendType>("type"))
                        {
                            case BagelCode.ClientModels.FriendType.FACEBOOK:
                                giftCredit = (long)(BlackboardUtils.FindVariable<int>(agent, "/values/misc/CREDIT_GIFT_AMOUNT_FB").value);
                                break;
                            default: // BagelCode.ClientModels.FriendType.NORMAL
                                giftCredit = (long)(BlackboardUtils.FindVariable<int>(agent, "/values/misc/CREDIT_GIFT_AMOUNT").value);
                                break;
                        }

                        if (giftCount > 0) receivedCount++;

                        var friendTier = friend.GetValue<int>("tier");
                        long friendGiftMultipliedCoin = TierUtils.GetTierFractionCoin(giftCredit, friendTier);
                        friendGiftMultipliedCoin = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(friendGiftMultipliedCoin, FreebieLevelUtils.FreebieType.FRIENDS_BONUS);
                        if (VipLounge.VipLounge.Utils.BadgeCount > 0)
                            friendGiftMultipliedCoin = NumberUtils.GetMultiplierNumeratorValue(friendGiftMultipliedCoin, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("FRIENDS_BONUS"));
                        collectableCredit += friendGiftMultipliedCoin * giftCount;

                        // send gift
                        long lastSendGiftTimestamp = friend.GetValue<long>("lastSendGiftTimestamp");
                        int sendGiftTimeInterval = BlackboardUtils.FindVariable<int>(agent, "/values/misc/FRIEND_GIFT_SEND_INTERVAL_SEC").value;

                        if (BagelCode.TimeUtils.GetTimeStamp() - lastSendGiftTimestamp > (long)sendGiftTimeInterval * 1000)
                        {
                            sendableCount++;
                            sendableCoin += giftCredit;
                        }

                        // assign to list
                        if (isOnline)
                        {
                            onlineFriends.Add(friend);
                        }
                        else
                        {
                            offlineFriends.Add(friend);
                        }
                    }
                    else
                    {
                        requestsFriends.Add(friend);
                        requestedCount++;
                    }
                }
            }

            var meTier = BlackboardUtils.FindVariable<int>("/me/tier").value;
            sendableCoin = TierUtils.GetTierFractionCoin(sendableCoin, meTier);

            saveAsOnlineFriends.value = onlineFriends;
            saveAsOfflineFriends.value = offlineFriends;
            saveAsRecommandFriends.value = recommendedFriendList.value;
            saveAsRequestFriends.value = requestsFriends;
            collectableCredits.value = (int)collectableCredit;
            receivedGiftCounts.value = receivedCount;
            requestedFriends.value = requestedCount;
            sendableGiftCounts.value = sendableCount;
            sendableCredits.value = (int)sendableCoin;
            EndAction();
        }
    }
}
