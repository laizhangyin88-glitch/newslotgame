using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestClubList : ActionTask <Blackboard> 
{
    public BBParameter<string> biContextID;

    protected override string info
    { 
        get 
        { 
            return string.Format("Request Club List");
        } 
    }
    protected override void OnExecute()
    {
        biContextID.value = "";
        
        BagelCodeClientAPI.ClubListRequest(
        (response) =>
        {
            if(agent != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                biContextID.value = response.analyticContextId;

                if(response.showDeclinePopup)
                {
                    GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_JOIN_REQUEST_DECLINE");
                }

                BlackboardUtils.SetOrCreateList(agent, "invitedClubList", response.invitedClubList, ClientAPI2Blackboard.Serialize);
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
