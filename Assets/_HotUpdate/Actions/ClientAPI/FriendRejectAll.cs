using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class FriendRejectAll : ActionTask <Blackboard> 
{
    protected override string info 
    { 
        get 
        { 
            return string.Format("Decline all friend requests");
        } 
    }

// TODO : Already Friend case. then just change accpted -> true 
    protected override void OnExecute()
    {
        BagelCodeClientAPI.FriendRejectAll(
        (response) =>
        {
            BlackboardQueryUtils.RejectAllRequests();
            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
            EndAction(true);
        });
    }
}

}
