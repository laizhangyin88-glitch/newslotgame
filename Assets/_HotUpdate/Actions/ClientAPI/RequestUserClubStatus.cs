using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestUserClubStatus : ActionTask <Blackboard> 
{
    protected override string info
    { 
        get 
        { 
            return "Request User Club Status";
        } 
    }
    protected override void OnExecute()
    {
        BagelCodeClientAPI.UserClubStatusRequest(
        (response) =>
        {
            if(agent != null)
            {
                var userClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId");

                var userClubRequestedID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "userClubStateInfo/requestedClubId");
                var userClubState = BlackboardUtils.FindVariable<UserClubState>(MainBlackboard.Get(), "userClubStateInfo/userClubState");

                userClubRequestedID.value = response.userClubStateInfo.requestedClubId;
                userClubState.value = response.userClubStateInfo.userClubState;

                switch(userClubState.value)
                {
                    case UserClubState.JOINED:
                        meClubID.value = userClubRequestedID.value;
                        userClubID.value = userClubRequestedID.value;
                        userClubRequestedID.value = 0L;
                        break;
                    case UserClubState.NOT_JOINED:
                        meClubID.value = 0L;
                        userClubID.value = 0L;
                        userClubRequestedID.value = 0L;
                        break;
                    case UserClubState.REQUESTED:
                        meClubID.value = 0L;
                        userClubID.value = 0L;
                        break;
                }

                EndAction(true);
            }
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
