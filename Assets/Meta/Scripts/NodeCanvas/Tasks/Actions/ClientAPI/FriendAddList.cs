using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class FriendAddList : ActionTask<Blackboard>
    {
        public BBParameter<List<string>> targetUserIdList;
        public BBParameter<string> type;
        public BBParameter<int> savedBeforeFriendCount;
        public BBParameter<string> result;

        protected override string info
        {
            get
            {
                return "Request Add Friend List";
            }
        }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.FriendAddList(targetUserIdList.value, type.value,
                (response) =>
                {
                    savedBeforeFriendCount.value = BlackboardQueryUtils.GetFriendList(true).Count;

                    if (agent != null && response.friendInfoList.Count > 0)
                    {
                        foreach(FriendInfo friendInfo in response.friendInfoList)
                        {
                            if (!BlackboardQueryUtils.UpdateFriendItemAsAccepted(friendInfo.userId))
                            {
                                BlackboardQueryUtils.AddFriend(friendInfo);
                            }
                        }
                    }
                    bool error;
                    result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "COMMON_ADD_FRIEND_OK", out error);

                    BlackboardQueryUtils.UpdateCelebInfo();

                    EndAction(true);
                },
                (error) =>
                {
                    bool stringError;
                    switch (error.errorCode)
                    {
                        case BagelCode.ClientModels.Error.TARGET_ALREADY_FRIEND_ERROR:
                            result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "COMMON_ADD_FRIEND_" + error.errorCode, out stringError);
                            break;
                        case BagelCode.ClientModels.Error.FRIEND_COUNT_MAX_ERROR:
                            result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "ERROR_EXCEED_MAX_FRIEND", out stringError);
                            break;
                        case BagelCode.ClientModels.Error.CANT_ADD_MYSELF_FRIEND_ERROR:
                            result.value = SlotMaker.StringTableUtils.GetString(SlotMaker.StringTable.StringTableType.Global, "ERROR_CANT_ADD_MYSELF_FRIEND", out stringError);
                            break;
                        default:
                            result.value = "<style=body>OTHER ERROR</style>";
                            break;
                    }
                    EndAction(true);
                });
        }
    }
}