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
    public class LeaveClub : ActionTask<Blackboard>
    {
        protected override string info
        {
            get { return "Leave Club"; }
        }

        protected override void OnExecute()
        {
            var leaveClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");

            BagelCodeClientAPI.LeaveClub(leaveClubID.value,
            (response) =>
            {
                if(agent != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                    ClientAPI2Blackboard.Serialize(bb, response);

                    if(leaveClubID.value == response.clubId)
                    {
                        ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, leaveClubID.value, 0);
                        LeaveClubSuccess();
                    }

                    long lastClubLeaveTimestamp = bb.GetValue<long>("lastClubLeaveTimestamp");
                    var mainLastClubLeaveTimestampVar = BlackboardUtils.FindVariable<long>("/lastClubLeaveTimestamp");
                    mainLastClubLeaveTimestampVar.value = lastClubLeaveTimestamp;

                    EndAction(true);
                }
            },
            (error) =>
            {
                if(agent != null)
                {
                    switch(error.errorCode)
                    {
                        case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                            GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                            EndAction(true);
                            break;
                        case ClientModels.Error.NOT_IN_CLUB_ERROR:
                            ClubUtils.SetClubPlayerPrefs(ClubDefine.PLAYER_PREFS_CLUB_LEVEL, leaveClubID.value, 0);
                            LeaveClubSuccess();
                            EndAction(true);
                            break;
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                }
            });
        }

        private void LeaveClubSuccess()
        {
            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
            var userClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId");
            var clubAuthority = BlackboardUtils.FindVariable<ClubAuthority>(MainBlackboard.Get(), "clubAuthority");

            var userClubRequestedID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "userClubStateInfo/requestedClubId");
            var userClubState = BlackboardUtils.FindVariable<UserClubState>(MainBlackboard.Get(), "userClubStateInfo/userClubState");

            meClubID.value = 0L;
            userClubID.value = 0L;
            userClubRequestedID.value = 0L;
            userClubState.value = UserClubState.NOT_JOINED;
            clubAuthority.value = ClubAuthority.UNKNOWN;

            var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "clubInfo");
            if (clubInfoBB != null)
            {
                UnityEngine.Object.Destroy(clubInfoBB.value.gameObject);
                MainBlackboard.Get().RemoveVariable("clubInfo");
            }

            var clubOnlineList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "clubOnlineList");
            clubOnlineList.value.Clear();
            BlackboardQueryUtils.UpdateOnlineClubIDList();
            BlackboardQueryUtils.ClearClubBadgeInfo();
        }
    }

}
