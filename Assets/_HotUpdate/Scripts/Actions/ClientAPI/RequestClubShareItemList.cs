using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RequestClubShareItemList : ActionTask <Blackboard> 
{
    public BBParameter<long> clubID;
    public BBParameter<int> mgEventID;

    public BBParameter<bool> saveAsIsSuccess;
    public BBParameter<bool> saveAsIsRefreshFeed;

    protected override string info
    { 
        get 
        { 
            return string.Format("Request Club Share Item List");
        } 
    }
    protected override void OnExecute()
    {
        saveAsIsSuccess.value = false;
        saveAsIsRefreshFeed.value = false;

        BagelCodeClientAPI.RequestClubShareItemList(clubID.value, mgEventID.value,
        (response) =>
        {
            if(agent != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                saveAsIsSuccess.value = true;

                EndAction();
            }
        },
        (error) =>
        {
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
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
