using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class FriendGiftSend : ActionTask<Blackboard>
    {
        protected override string info
        {
            get
            {
                return "Friend Gift Send";
            }
        }

        protected override void OnExecute()
        {
            var friendList = BlackboardQueryUtils.GetFriendList();

            for (int i = 0; i < friendList.Count; ++i)
            {
                var friend = friendList[i];
                if (friend.GetValue<bool>("accepted"))
                {
                    long lastSendGiftTimestamp = friend.GetValue<long>("lastSendGiftTimestamp");
                    int sendGiftTimeInterval = BlackboardUtils.FindVariable<int>(agent, "/values/misc/FRIEND_GIFT_SEND_INTERVAL_SEC").value;

                    if (BagelCode.TimeUtils.GetTimeStamp() - lastSendGiftTimestamp > (long)sendGiftTimeInterval * 1000)
                    {
                        friendList[i].SetValue("lastSendGiftTimestamp", TimeUtils.GetTimeStamp());
                    }
                }
            }

            EndAction(true);

            BagelCodeClientAPI.FriendGiftSend(
            (response) =>
            {
                for (int i = 0; i < friendList.Count; ++i)
                {
                    if (response.receiverUserIdList.Contains(friendList[i].GetValue<string>("userId")))
                    {
                        friendList[i].SetValue("lastSendGiftTimestamp", response.lastSendGiftTimestamp);
                    }
                }

                return;
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }
    }
}
