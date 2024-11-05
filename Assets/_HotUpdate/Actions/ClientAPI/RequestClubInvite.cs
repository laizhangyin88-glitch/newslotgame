using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestClubInvite : ActionTask <Blackboard> 
{
    public BBParameter<string> userID;

    public BBParameter<bool> isNotInClub;

    protected override string info
    { 
        get 
        { 
            return string.Format("Request Invite Club");
        } 
    }
    protected override void OnExecute()
    {
        isNotInClub.value = false;
        BagelCodeClientAPI.ClubMemberInvite(MainBlackboard.Get().GetValue<long>("clubId"), userID.value,
        (response) =>
        {
            if(agent != null)
            {
                GlobalErrorHandler.OpenAlertPopup("POPUP_CLUB_INVITE_SUCCESS_MESSAGE");
                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.ALREADY_IN_CLUB_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("POPUP_CLUB_INVITE_SUCCESS_MESSAGE");
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.CLUB_REACHED_MAX_MEMBER_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("POPUP_CLUB_INVITE_FAILED_FULL");
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("POPUP_CLUB_INVITE_FAILED_NOT_IN_CLUB");
                        isNotInClub.value = true;
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.TARGET_NOT_IN_CLUB_ERROR:
                case ClientModels.Error.CLUB_NOT_EXIST_ERROR:
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                case ClientModels.Error.CLUB_PERMISSION_ERROR:
                    if(agent != null)
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        EndAction(true);
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
