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
    public class JoinClub : ActionTask<Blackboard>
    {
        public BBParameter<long> clubID;

        public BBParameter<bool> isSuccess;
        public BBParameter<bool> isRefreshClubScene;
        public BBParameter<UserClubState> joinState;

        public BBParameter<string> uiJoinType;

        protected override string info
        {
            get { return "Join Club"; }
        }

        protected override void OnExecute()
        {
            isSuccess.value = false;
            isRefreshClubScene.value = false;

            BagelCodeClientAPI.JoinClub(clubID.value, uiJoinType.value,
            (response) =>
            {
                if(agent != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                    ClientAPI2Blackboard.Serialize(bb, response);

                    var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                    var userClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId");
                    var clubAuthority = BlackboardUtils.FindVariable<ClubAuthority>(MainBlackboard.Get(), "clubAuthority");

                    var userClubRequestedID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "userClubStateInfo/requestedClubId");
                    var userClubState = BlackboardUtils.FindVariable<UserClubState>(MainBlackboard.Get(), "userClubStateInfo/userClubState");

                    userClubRequestedID.value = response.userClubStateInfo.requestedClubId;
                    userClubState.value = response.userClubStateInfo.userClubState;
                    joinState.value = response.userClubStateInfo.userClubState;

                    int clubLevel = BlackboardUtils.FindValue<int>(agent, "clubInfo/level");

                    ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, clubID.value, clubLevel);
                    PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, TimeUtils.GetTimeStamp());
                    ClubUtils.ResetClubMetaGamePlayerPrefs();

                    if (userClubState.value == UserClubState.JOINED)
                    {
                        meClubID.value = response.clubId;
                        userClubID.value = response.clubId;
                        BlackboardUtils.SetOrCreateList<UserProfile>(MainBlackboard.Get(), "clubOnlineList", response.clubOnlineList, ClientAPI2Blackboard.Serialize);

                        BlackboardQueryUtils.ClearClubBadgeInfo();

                        clubAuthority.value = ClubAuthority.MEMBER;

                        var clubInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "clubInfo");
                        ClientAPI2Blackboard.Serialize(clubInfoBB, response.clubInfo);
                    }
                    else
                    {
                        meClubID.value = 0L;
                        userClubID.value = 0L;
                        clubAuthority.value = ClubAuthority.UNKNOWN;

                        var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "clubInfo");
                        if (clubInfoBB != null) {
                            UnityEngine.Object.Destroy(clubInfoBB.value.gameObject);
                            MainBlackboard.Get().RemoveVariable("clubInfo");
                        }
                    }

                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    isSuccess.value = true;
                    EndAction(true);
                }
            },
            (error) =>
            {
#if DEV
                Debug.LogError(error.errorCode);
#endif
                switch(error.errorCode)
                {

                    case ClientModels.Error.CLUB_ALREADY_DECLINED_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERRRO_CLUB_ALREADY_DECLINED");
                        isSuccess.value = false;
                        if(agent != null)
                            EndAction(true);
                        break;
                    case ClientModels.Error.ALREADY_IN_CLUB_ERROR:
                        isRefreshClubScene.value = true;
                        isSuccess.value = false;
                        EndAction(true);
                        break;
                    case ClientModels.Error.CLUB_REACHED_MAX_MEMBER_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_MEMBER_FULL");
                        isSuccess.value = false;
                        EndAction(true);
                        break;
                    case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        isSuccess.value = false;
                        EndAction(true);
                        break;
                    case ClientModels.Error.CLUB_NOT_EXIST_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        isSuccess.value = false;
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
