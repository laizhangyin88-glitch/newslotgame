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

public class RequestClubShareItemRequest : ActionTask <Blackboard> 
{
    public BBParameter<GameObject> caller;
    public BBParameter<long> clubID;
    public BBParameter<int> mgEventID;
    public BBParameter<int> requestItemID;
    public BBParameter<string> requestType;

    public BBParameter<bool> saveAsSuccess;

    protected override string info
    { 
        get 
        { 
            return string.Format("Request Club Share Item({0})", requestItemID);
        } 
    }
    protected override void OnExecute()
    {
        BagelCodeClientAPI.RequestClubShareItem(clubID.value, mgEventID.value, requestItemID.value, requestType.value,
        (response) =>
        {
            if(agent != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "response");

                BlackboardUtils.ClearBlackboard(bb);
                ClientAPI2Blackboard.Serialize(bb, response);

                BlackboardQueryUtils.SetCollectingGameRequestResetTimestamp(response.metaGameRequestResetTimestamp);

                if(caller.value != null)
                {
                    var callerBB = caller.value.GetComponent<Blackboard>();
                    if(callerBB != null)
                    {
                        BlackboardUtils.SetOrCreateValue<long>(callerBB, "metaGameRequestResetTimestamp", response.metaGameRequestResetTimestamp);
                    }
                }

                saveAsSuccess.value = true;
                EndAction();
            }
        },
        (error) =>
        {
            saveAsSuccess.value = false;
            switch(error.errorCode)
            {
                case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                case ClientModels.Error.INVALID_SHARE_REQUEST_ERROR:
                    GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                    if(agent != null)
                        EndAction();
                    break;
                case ClientModels.Error.NOT_IN_CLUB_ERROR:
                    {
                        if(agent != null)
                            EndAction();
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
