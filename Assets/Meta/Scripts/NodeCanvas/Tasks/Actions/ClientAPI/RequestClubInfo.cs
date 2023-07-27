using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestClubInfo : ActionTask <Blackboard>
{
    public BBParameter<long> clubID;
    public BBParameter<int> mgEventID;
    public BBParameter<bool> mgEnableShare;
    public BBParameter<bool> isPopup;

    public BBParameter<bool> saveAsIsSuccess;

    protected override string info
    {
        get
        {
            return string.Format("Request Club Info {0}", clubID);
        }
    }
    protected override void OnExecute()
    {
        int eventID = mgEnableShare.value ? mgEventID.value : 0;
        saveAsIsSuccess.value = false;

        BagelCodeClientAPI.ClubInfoRequest(clubID.value, isPopup.value, eventID,
        (response) =>
        {
            if(agent != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "clubInfoResponse");

                // Dummy Code
                // if(isPopup.value == false && response.myClubInfo != null)
                // {
                //     response.leagueRewardPopup = new ClientModels.ClubLeagueRewardPopupInfo();
                //     response.leagueRewardPopup.leagueTier = 3;
                //     response.leagueRewardPopup.rank = 3;
                //     response.leagueRewardPopup.tierChange = ClientModels.TierChangeType.REMAIN;
                //     response.leagueRewardPopup.clubId = response.myClubInfo.clubId;
                //     response.leagueRewardPopup.rewardExpireTimestamp = TimeUtils.GetTimeStamp() + 10000000;
                //     response.leagueRewardPopup.rewardCredit = 2000000;
                // }
                /////////////

                BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                var mainLastClubLeaveTimestampVar = BlackboardUtils.FindVariable<long>("/lastClubLeaveTimestamp");
                if(mainLastClubLeaveTimestampVar != null)
                    mainLastClubLeaveTimestampVar.value = response.lastClubLeaveTimestamp;

                var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                if(meClubID.value > 0 && meClubID.value == response.clubInfo.id)
                {
                    if(response.myClubInfo == null)
                    {
                        // Remove Club Cooltimes
                        PlayerPrefsUtils.SetInt64(ClubDefine.PLAYER_PREFS_LAST_CLUB_CHECKED_MS, 0);
                        ClubUtils.ResetClubMetaGamePlayerPrefs();
                        BlackboardQueryUtils.SetMyClubId(0);

                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                        info.type = ErrorPopupType.OK;
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

                        ErrorPopupHandler.Instance.OpenError(info);
                        MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                    }
                }

                saveAsIsSuccess.value = true;

                EndAction(true);
            }

        },
        (error) =>
        {
            switch(error.errorCode)
            {
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                        if(meClubID.value > 0)
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                            info.type = ErrorPopupType.OK;
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

                            ErrorPopupHandler.Instance.OpenError(info);
                        }

                        BlackboardQueryUtils.SetMyClubId(0);
                        MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                        EndAction(true);
                    }
                    break;
                case ClientModels.Error.CLUB_NOT_EXIST_ERROR:
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_CLUB_NOT_EXIST");
                        if(agent != null)
                        {
                            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                            if (meClubID.value == clubID.value)
                            {
                                BlackboardQueryUtils.SetMyClubId(0);
                            }
                            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                            EndAction(true);
                        }
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
