using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.Contents;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class EnterGame : ActionTask <Blackboard>
{
    public BBParameter<int> gameID;
    public BBParameter<string> targetRoomID;
    public BBParameter<bool> isEarlyAccess;
    public BBParameter<bool> enterSuccess;

    protected override string info
    {
        get
        {
            return string.Format("Request Enter Game {0}", gameID);
        }
    }

    protected override void OnExecute()
    {
        var context_id = BlackboardUtils.FindVariable<string>(null, "/enterGameInfo/contextID");

        if (targetRoomID.value != null && !string.IsNullOrEmpty(targetRoomID.value))
        {
            BagelCodeClientAPI.GameEnterRoom(gameID.value, targetRoomID.value, isEarlyAccess.value, context_id.value,
            (response) =>
            {
                if(agent != null)
                    EnterGameResponse(gameID.value, response);
            },
            (error) =>
            {
                ErrorHandle(error);
            });
        }
        else if (gameID != null)
        {
            BagelCodeClientAPI.GameEnter(gameID.value, isEarlyAccess.value, context_id.value,
            (response) =>
            {
                if(agent != null)
                    EnterGameResponse(gameID.value, response);
            },
            (error) =>
            {
                ErrorHandle(error);
            });
        }
        else
        {
            Debug.LogError("No Game ID found." + agent.gameObject.name);
        }
    }

    private void EnterGameResponse(int gameID, BagelCode.ClientModels.RoomEnterResponseV3 response)
    {
        var bb = ContentBlackboard.Get();

        Serialize(bb, response);

        BlackboardQueryUtils.UpdateSeat(response.room);
        BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
        BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
        BlackboardQueryUtils.UpdateMetaGameEnterInfo(response.metaGameEnterInfo);
        BlackboardQueryUtils.UpdateSeasonPassEnterInfo(response.seasonPassEnterInfo);
        BlackboardQueryUtils.ApplyUserSyncInfo();

        IAMRouter.Instance.SortTrigger(BagelCode.ClientModels.InAppMessageTriggerType.ALL_IN, gameID);

        enterSuccess.value = true;
        EndAction(true);
    }

    void Serialize(IBlackboard bb, BagelCode.ClientModels.RoomEnterResponseV3 roomEnterResponse)
    {
        ClientAPI2Blackboard.Serialize(bb, roomEnterResponse);
        BlackboardUtils.SetOrCreateValue<ContentsRequestType>(bb, "requestType", ContentsRequestType.Enter);
        ContentsSerializer.Deserialize(bb);
    }

    private void ErrorHandle(BagelCodeHTTPError error)
    {
        switch(error.errorCode)
        {
            case ClientModels.Error.ROOM_FULL_ERROR:
                {
                    bool stringError = false;
                    ErrorPopupInfo info = new ErrorPopupInfo();

                    info.type = ErrorPopupType.OK;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_ROOM_FULL", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                    info.callback1 = delegate
                    {
                        enterSuccess.value = false;
                        EndAction();
                    };

                    ErrorPopupHandler.Instance.OpenError(info);
                }
                break;
            default:
                GlobalErrorHandler.GlobalError(error);
                break;
        }
    }
}

}
