using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class GiftClubFeed : ActionTask <Blackboard> 
{
    public BBParameter<string> feedIDValue;
    public BBParameter<string> mgEventIDValue;

    public BBParameter<bool> saveAsIsSuccess;
    public BBParameter<bool> saveAsIsRefreshFeed;

    protected override string info
    { 
        get 
        { 
            return "Gift Club Feed";
        } 
    }
    protected override void OnExecute()
    {
        var feedID = BlackboardUtils.FindVariable<long>(agent, feedIDValue.value);
        var mgEventID = BlackboardUtils.FindVariable<int>(agent, mgEventIDValue.value);

        saveAsIsSuccess.value = false;
        saveAsIsRefreshFeed.value = false;

        BagelCodeClientAPI.GiftClubFeed(feedID.value, mgEventID.value,
        (response) =>
        {
            if(agent != null)
            {
                saveAsIsSuccess.value = true;
                EndAction(true);
            }
        },
        (error) =>
        {
            // Debug.LogError(error.errorCode);
            switch(error.errorCode)
            {
                case ClientModels.Error.FEATURE_NOT_UNLOCKED_ERROR:
                case ClientModels.Error.INVALID_CLUB_REQUEST_ERROR:
                case ClientModels.Error.INVALID_META_GAME_ERROR:
                    {
                        GlobalErrorHandler.OpenAlertPopup("ERROR_INVALID_CLUB_REQUEST_ERROR", BlackboardQueryUtils.GetMetaGameEventName());
                        if(agent != null)
                        {
                            saveAsIsRefreshFeed.value = true;
                            EndAction(true);
                        }
                    }
                    break;
                case ClientModels.Error.TARGET_NOT_IN_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_TARGET_NOT_IN_CLUB_ERROR");
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.MAX_CLUB_SHARE_COUNT_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_MAX_CLUB_SHARE_COUNT_ERROR");
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction(true);
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        if(agent != null)
                            EndAction(true);
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
