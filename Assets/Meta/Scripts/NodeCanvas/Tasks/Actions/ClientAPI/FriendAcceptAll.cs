using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class FriendAcceptAll : ActionTask <Blackboard>
{
    protected override string info
    {
        get
        {
            return string.Format("Accept all friend requests");
        }
    }

// TODO : Already Friend case. then just change accpted -> true
    protected override void OnExecute()
    {
        BagelCodeClientAPI.FriendAcceptAll(
        (response) =>
        {
            if(agent != null)
            {
                for (int i = 0; i < response.acceptedUserIdList.Count; ++i)
                {
                    BlackboardQueryUtils.UpdateFriendItemAsAccepted(response.acceptedUserIdList[i]);
                }

                // Exception) Remove failed accept users.
                var exceptionUsers = BlackboardQueryUtils.GetFriendList(false);
                for (int i = 0; i < exceptionUsers.Count; ++i)
                {
                    var userID = exceptionUsers[i].GetValue<string>("userId");
                    BlackboardQueryUtils.RemoveFriend(userID);
                    BlackboardQueryUtils.RemoveOnlineFriendUserID(userID);
                }

                BlackboardQueryUtils.UpdateCelebInfo();

                bool isFriendMax = response.isNormalFriendCountMax;

                if(isFriendMax)
                {
                    bool stringError = false;
                    ErrorPopupInfo info = new ErrorPopupInfo();
                    info.type = ErrorPopupType.OK;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_EXCEED_MAX_FRIEND", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                    ErrorPopupHandler.Instance.OpenError(info);
                }

                EndAction(true);
            }
        },
        (error) =>
        {
            if(agent != null)
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.NO_FRIEND_REQUEST_INFO_ERROR:
                            // Nothing to do.
                            break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }

                EndAction(true);
            }
        });
    }
}

}
