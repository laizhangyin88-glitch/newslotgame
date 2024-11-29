using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_friend_collect : ActionTask<Blackboard>
    {
        protected override void OnExecute()
        {
            var collectableCredits = BlackboardUtils.FindVariable<long>(agent, "collectableCredits");
            var receivedGiftCounts = BlackboardUtils.FindVariable<int>(agent, "receivedGiftCounts");
            var friendList = BlackboardQueryUtils.GetFriendList();
            int friendCount = 0;

            for (int i = 0; i < friendList.Count; i++)
            {
                if (friendList[i].GetValue<bool>("accepted"))
                {
                    friendCount++;
                }
            }

            if (collectableCredits == null || receivedGiftCounts == null)
            {
                EndAction();
                return;
            }

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["earn_coin"] = collectableCredits.value;
            customData["received_friend_count"] = receivedGiftCounts.value;
            customData["total_friend_count"] = friendCount;
            if (VipLounge.VipLounge.Utils.IsEnded)
                customData["vip_lounge_extra_reward"] = null;
            else
                customData["vip_lounge_extra_reward"] = collectableCredits.value - NumberUtils.GetDevideNumeratorValue(collectableCredits.value, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("FRIENDS_BONUS"));

            Analytics.CustomEvent("client_friend_collect", customData);

            EndAction();
        }
    }
}
