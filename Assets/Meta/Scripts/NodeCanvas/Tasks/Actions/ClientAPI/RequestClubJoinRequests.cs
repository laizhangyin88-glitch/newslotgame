using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestClubJoinRequests : ActionTask<Blackboard> 
{
    public BBParameter<long> clubID;

    protected override string info 
    {
        get { return "Club Join Requests List"; }
    }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.ClubJoinRequestList(clubID.value,
        (response) =>
        {
            if(agent != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "clubJoinRequestsResponse");

                ClientAPI2Blackboard.Serialize(bb, response);

                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.TARGET_NOT_IN_CLUB_ERROR:
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                case ClientModels.Error.CLUB_NOT_EXIST_ERROR:
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                case ClientModels.Error.CLUB_PERMISSION_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction(true);
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
