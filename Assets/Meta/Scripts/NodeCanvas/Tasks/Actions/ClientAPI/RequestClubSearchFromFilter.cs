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
public class RequestClubSearchFromFilter : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> callerBB;

    public BBParameter<int> clubLevel;
    public BBParameter<int> playerLevelRestrictionIndex;
    public BBParameter<int> searchTypeIndex;
    public BBParameter<bool> isAvailable;
    public BBParameter<bool> isFriendsInside;

    public BBParameter<bool> isSuccess;

    protected override string info 
    {
        get { return "Club Search From Filter"; }
    }

    protected override void OnExecute()
    {
        isSuccess.value = false;

        ClubJoinSearchType searchType = ClubJoinSearchType.ALL;
        if(searchTypeIndex.value == 1)
            searchType = ClubJoinSearchType.PUBLIC;
        else if(searchTypeIndex.value == 2)
            searchType = ClubJoinSearchType.PRIVATE;

        int playerLevel = 1;
        var clubRestrictionLevelList = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "values/club/MIN_PLAYER_LEVEL").value;
        if(clubRestrictionLevelList.Count > playerLevelRestrictionIndex.value)
            playerLevel = clubRestrictionLevelList[ playerLevelRestrictionIndex.value ];

        BagelCodeClientAPI.ClubSearchFromFilter(clubLevel.value, playerLevel, searchType, isAvailable.value, isFriendsInside.value,
        (response) =>
        {
            if(agent != null)
            {
                // override club join list. 
                var bb = BlackboardUtils.GetOrCreateBlackboard(callerBB.value, "response");

                // Usable BI Event. 
                callerBB.value.SetValue("biClubLevel", clubLevel.value);
                callerBB.value.SetValue("biPlayerLevel", playerLevel);
                callerBB.value.SetValue("biJoinSearchType", searchType);
                callerBB.value.SetValue("biIsAvailable", isAvailable.value);
                callerBB.value.SetValue("biIsFriendsInside", isFriendsInside.value);
                callerBB.value.SetValue("biContextID", response.analyticContextId);

                // BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                isSuccess.value = true;

                EndAction(true);
            }
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);

            // switch(error.errorCode)
            // {
            //     case ClientModels.Error.TARGET_NOT_IN_CLUB_ERROR:
            //         if(agent != null)
            //             EndAction(true);
            //         break;
            //     case ClientModels.Error.NOT_IN_CLUB_ERROR:
            //     case ClientModels.Error.CLUB_NOT_EXIST_ERROR:
            //     case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
            //     case ClientModels.Error.CLUB_PERMISSION_ERROR:
            //         GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
            //         if(agent != null)
            //             EndAction(true);
            //         break;
            //     default:
            //         GlobalErrorHandler.GlobalError(error);
            //         break;
            // }
        });
    }
}

}
