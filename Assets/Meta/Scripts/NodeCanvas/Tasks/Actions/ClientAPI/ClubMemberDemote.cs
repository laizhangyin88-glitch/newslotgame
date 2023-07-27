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
public class ClubMemberDemote : ActionTask<Blackboard> 
{
    public BBParameter<string> memberInfoValue;
    public BBParameter<long> clubID;

    public BBParameter<bool> saveAsRefreshClubList;

    protected override string info 
    {
        get { return "Club Member Demote"; }
    }

    protected override void OnExecute()
    {
        var memberInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, memberInfoValue.value);
        saveAsRefreshClubList.value = false;

        if(memberInfoBB != null)
        {
            var userID = BlackboardUtils.FindVariable<string>(memberInfoBB.value, "userId");

            BagelCodeClientAPI.ClubMemberDemote(clubID.value, userID.value,
            (response) =>
            {
                if(agent != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                    ClientAPI2Blackboard.Serialize(bb, response);

                    memberInfoBB.value.SetValue("authority", ClubAuthority.MEMBER);

                    EndAction(true);
                }
            },
            (error) =>
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.WRONG_MEMBER_PERMISSION_ERROR:
                    case ClientModels.Error.TARGET_NOT_IN_CLUB_ERROR:
                        if(agent != null)
                        {
                            saveAsRefreshClubList.value = true;
                            EndAction(true);
                        }
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
        else
        {

        }
    }
}

}
