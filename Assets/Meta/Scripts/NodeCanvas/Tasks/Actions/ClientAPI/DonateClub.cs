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
public class DonateClub : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> clubInfoResponse;
    public BBParameter<Blackboard> clubInfo;
    public BBParameter<Blackboard> clubMemberInfo;

    public BBParameter<int> donateLevel;

    protected override string info 
    {
        get { return "Donate Club"; }
    }

    protected override void OnExecute()
    {
        var clubID = BlackboardUtils.FindVariable<long>(clubInfo.value, "id");

        BagelCodeClientAPI.DonateClub(clubID.value, donateLevel.value,
        (response) =>
        {
            if(agent != null)
            {
                if(clubID.value == response.clubInfo.id)
                {
                    ClientAPI2Blackboard.Serialize(clubInfo.value, response.clubInfo);
                    ClientAPI2Blackboard.Serialize(clubMemberInfo.value, response.myClubInfo);

                    var donationResetTimestamp = BlackboardUtils.FindVariable<long>(clubInfoResponse.value, "donationResetTimestamp");
                    donationResetTimestamp.value = response.donationResetTimestamp;

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();
                }

                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        if(agent != null)
                            EndAction(true);
                    }
                    break;
                case ClientModels.Error.CLUB_REACHED_MAX_LEVEL_ERROR:
                    {
                        if(agent != null && clubInfo != null)
                        {
                            var clubLevel = BlackboardUtils.FindVariable<int>(clubInfo.value, "level");
                            clubLevel.value = ClubUtils.GetMaxLevel();
                            EndAction(true);
                        }
                    }
                    break;
                case ClientModels.Error.MAX_DONATION_COUNT_ERROR:
                case ClientModels.Error.NOT_ENOUGH_CREDIT_ERROR:
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        GlobalErrorHandler.GlobalError(error);
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
