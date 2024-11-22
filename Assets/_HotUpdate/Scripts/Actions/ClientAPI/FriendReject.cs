using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class FriendReject : ActionTask <Blackboard> 
{
    public BBParameter<string> userId;

    protected override string info 
    { 
        get 
        { 
            return string.Format("Reject friend request {0}", userId);
        } 
    }

// TODO : Already Friend case. then just change accpted -> true 
    protected override void OnExecute()
    {
        if (userId.value != null)
        {
            BagelCodeClientAPI.FriendReject(userId.value,
            (response) =>
            {
                BlackboardQueryUtils.RemoveFriend(userId.value);
                BlackboardQueryUtils.UpdateCelebInfo();
                EndAction(true);
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
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
