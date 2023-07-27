using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class FriendGiftCollect : ActionTask<Blackboard>
    {
        protected override string info
        {
            get
            {
                return "Friend Gift Collect";
            }
        }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.FriendGiftCollect(
            (response) =>
            {
                var friendList = BlackboardQueryUtils.GetFriendList();
                var collectableCredits = BlackboardUtils.GetOrCreateVariable<long>(agent, "collectableCredits");
                var receivedGiftCounts = BlackboardUtils.GetOrCreateVariable<int>(agent, "receivedGiftCounts");
                int receivedFriendCounts = 0;

                for (int i = 0; i < friendList.Count; ++i)
                {
                    if (friendList[i].GetValue<bool>("accepted")
                    && friendList[i].GetValue<int>("receivedGiftCount") > 0)
                    {
                        friendList[i].SetValue("receivedGiftCount", 0);
                        receivedFriendCounts++;
                    }
                }

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                receivedGiftCounts.value = receivedFriendCounts;
                collectableCredits.value = response.earnCredit;
                BlackboardQueryUtils.AddCoins(response.earnCredit);


                EndAction(true);
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }
    }
}
