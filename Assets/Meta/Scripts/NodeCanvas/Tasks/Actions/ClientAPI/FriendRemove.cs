using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class FriendRemove : ActionTask <Blackboard> 
{
    public BBParameter<string> userId;

    protected override string info 
    { 
        get 
        { 
            return string.Format("Remove friend request {0}", userId);
        } 
    }

// TODO : Already Friend case. then just change accpted -> true 
    protected override void OnExecute()
    {
        if (userId.value != null)
        {
            BagelCodeClientAPI.FriendRemove(userId.value,
            (response) =>
            {
                BlackboardQueryUtils.RemoveFriend(userId.value);
                BlackboardQueryUtils.RemoveOnlineFriendUserID(userId.value);
                BlackboardQueryUtils.UpdateCelebInfo();
                EndAction(true);
            },
            (error) =>
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.TARGET_NOT_FRIEND_ERROR:
                        BlackboardQueryUtils.RemoveFriend(userId.value);
                        BlackboardQueryUtils.RemoveOnlineFriendUserID(userId.value);
                        BlackboardQueryUtils.UpdateCelebInfo();
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
                EndAction(true);
            });
        }
        else
        {
            Debug.LogError("No UserId found." + agent.gameObject.name);
        }
    }
}

}
