using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class FriendAccept : ActionTask <Blackboard>
{
    public BBParameter<string> userId;
    [BlackboardOnly]
    public BBParameter<bool> isSuccess = true;

    protected override string info
    {
        get
        {
            return string.Format("Accept friend request {0}", userId);
        }
    }

// TODO : Already Friend case. then just change accpted -> true
    protected override void OnExecute()
    {
        if (userId.value != null)
        {
            BagelCodeClientAPI.FriendAccept(userId.value,
            (response) =>
            {
                if(agent != null)
                {
                    isSuccess.value = true;
                    BlackboardQueryUtils.UpdateFriendItemAsAccepted(userId.value);
                    BlackboardQueryUtils.UpdateCelebInfo();
                    EndAction(true);
                }
            },
            (error) =>
            {
                if(agent != null)
                {
                    isSuccess.value = false;

                    switch(error.errorCode)
                    {
                        case ClientModels.Error.FRIEND_COUNT_MAX_ERROR:
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();

                                info.type = ErrorPopupType.OK;
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global,
                                    "ERROR_EXCEED_MAX_FRIEND", out stringError);
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY",
                                    out stringError);
                                ErrorPopupHandler.Instance.OpenError(info);
                            }
                            break;
                        case ClientModels.Error.NO_FRIEND_REQUEST_INFO_ERROR:
                            {
                                // Remove Cell..
                                isSuccess.value = true;
                                BlackboardQueryUtils.RemoveFriend(userId.value);
                                BlackboardQueryUtils.RemoveOnlineFriendUserID(userId.value);
                                BlackboardQueryUtils.UpdateCelebInfo();
                            }
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }

                    EndAction(true);
                }
            });
        }
        else
        {
            BagelCodeHTTPError error = new BagelCodeHTTPError();
            GlobalErrorHandler.GlobalError(error);
            Debug.LogError("No UserId found." + agent.gameObject.name);
        }
    }
}



}
