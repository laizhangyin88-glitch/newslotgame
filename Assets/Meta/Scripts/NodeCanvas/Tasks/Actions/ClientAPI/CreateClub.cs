using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class CreateClub : ActionTask<Blackboard> 
{
    public BBParameter<string>  clubName;
    public BBParameter<string>  message;
    public BBParameter<int>     watcher;
    public BBParameter<string>  symbol;
    public BBParameter<int>     minLevel;
    public BBParameter<int>     joinTypeIndex;

    public BBParameter<bool> isSuccess;
    public BBParameter<bool> isClosePopup;

    protected override string info 
    {
        get { return "Create Club"; }
    }

    protected override void OnExecute()
    {
        isSuccess.value = false;
        isClosePopup.value = false;

        ClubJoinType joinType = joinTypeIndex.value == 0 ? ClubJoinType.PUBLIC : ClubJoinType.PRIVATE;

        BagelCodeClientAPI.CreateClub(watcher.value, clubName.value.Trim(), message.value.Trim(), symbol.value, minLevel.value, joinType,
        (response) =>
        {
            var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

            ClientAPI2Blackboard.Serialize(bb, response);
            

            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
            var clubAuthority = BlackboardUtils.FindVariable<ClubAuthority>(MainBlackboard.Get(), "clubAuthority");
            var userClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId");
            var userClubRequestedID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "userClubStateInfo/requestedClubId");
            var userClubState = BlackboardUtils.FindVariable<UserClubState>(MainBlackboard.Get(), "userClubStateInfo/userClubState");
            meClubID.value = response.clubId;
            userClubID.value = response.clubId;
            userClubRequestedID.value = 0L;
            userClubState.value = UserClubState.JOINED;
            clubAuthority.value = ClubAuthority.LEADER;

            var clubInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "clubInfo");
            ClientAPI2Blackboard.Serialize(clubInfoBB, response.clubInfo);

            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.ApplyUserSyncInfo();
            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData<long>("OnCreateClubSuccess", response.clubId));

            if (PlayerPrefs.HasKey(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS))
                PlayerPrefs.DeleteKey(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS);

            isSuccess.value = true;
            EndAction(true);
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.NOT_ENOUGH_CREDIT_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_NOT_ENOUGH_COIN");
                    EndAction(true);
                    break;
                case ClientModels.Error.ALREADY_IN_CLUB_ERROR:
                    isClosePopup.value = true;
                    EndAction(true);
                    break;
                case ClientModels.Error.INVALID_NAME_FORMAT_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    EndAction(true);
                    break;
                case ClientModels.Error.ALREADY_EXIST_CLUB_NAME_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_ALREADY_NAME");
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
