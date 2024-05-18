using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.Contents;
using System.Collections.Generic;
using System;
using System.Linq;
using SimpleJSON;
//using Boo.Lang;


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

#if NEW_NET

            RPCHall.ReqEnterGame req = new RPCHall.ReqEnterGame();
            req.game_id = gameID.value;

           // NetManager.Instance.Post(RPCName.enterGame,
            NetManager.Instance.Post(RPCName.enterGame, req,
                (res) =>
                {
                    string resStr = res.ToString();

                    //string jsonUrl = null;
                    //switch ((GameID)gameID.value)
                    //{
                    //    case GameID.KenoClassic:
                    //        jsonUrl = $"tempdata/room_enter_response_v3__{"kenoclassic"}";
                    //        break;
                    //}

                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/room_enter_response_v3");
                    ClientModels.RoomEnterResponseV3 response = JsonUtility.FromJson<ClientModels.RoomEnterResponseV3>(jsn8.text);

                    System.Collections.Generic.List<long> betList = res["bet_list"].AsStringList.Select(s => long.Parse(s)).ToList();
                    response.betList = betList;
                    response.contents = res["contents"].ToString();


                    /*if (res.HasKey("last_session_content"))
                    {

                        JSONNode _cnt;

                        string boundIdStr = "{\"bonus_id\":2102,\"type\":2,\"result\":{\"type\":0,\"bet_credit\":8000,\"added_spin_count\":20},\"earn_credit\":0,\"claim_type\":3,\"uid\":\"171119377516341340\"}";

                        JSONNode _bound = JSONNode.Parse(boundIdStr);

                        if (res.HasKey("last_session_content"))
                        {
                            _cnt = res["last_session_content"];
                        }
                        else
                        {
                            TextAsset jsn9 = Resources.Load<TextAsset>("tempdata/free_spin_content_777");
                            _cnt = JSONNode.Parse(jsn9.text);
                        }

                        var bet = 0;
                        var addedSpinCount = 15;
                        if (_cnt.HasKey("free_spin_info"))
                        {
                            bet = _cnt["free_spin_info"]["bet"];
                            addedSpinCount = _cnt["free_spin_info"]["count"]  - 1;  //剩余局数
                        }
                        else
                        {
                            Debug.LogError("没有 free_spin_info 节点");
                        }

                        _bound["result"]["bet_credit"] = bet;
                        _bound["result"]["added_spin_count"] = addedSpinCount;

                        if (addedSpinCount >0 ) //剩余局数大于0
                        {
                            _cnt["bonus_result"].Add(_bound);
                        }

                        //TextAsset jsn9 = Resources.Load<TextAsset>("tempdata/free_spin_content_777");
                        //_cnt = JSONNode.Parse(jsn9.text);


                        string strRes01 = _cnt.ToString();
                        BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "lastFreeSpinContent", strRes01);

                        Debug.Log($"last_session_content = {strRes01}");//last_session_content

                        BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastFreeSpin", true);
        
                    }
                    else
                    {
                        BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastFreeSpin", false);
                    }*/


                    //Debug.Log($" @contents =  {response.contents}");


                    var bb = ContentBlackboard.Get();

                    Serialize(bb, response);

                    BlackboardQueryUtils.UpdateSeat(response.room);
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
                    BlackboardQueryUtils.UpdateMetaGameEnterInfo(response.metaGameEnterInfo);
                    BlackboardQueryUtils.UpdateSeasonPassEnterInfo(response.seasonPassEnterInfo);
                    BlackboardQueryUtils.ApplyUserSyncInfo();

                    IAMRouter.Instance.SortTrigger(BagelCode.ClientModels.InAppMessageTriggerType.ALL_IN, gameID.value);

                    enterSuccess.value = true;
                    EndAction(true);

                },
                (error) =>
                {
                    string oldJson = JsonUtility.ToJson(error);
                    Debug.Log($"@A SlotSpinResponseV3 = {oldJson}");
                    ErrorHandle(error);
                }
             );

            return;
#endif





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

                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A SlotSpinResponseV3 = {oldJson}");


                if (agent != null)
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

        string oldJson = JsonUtility.ToJson(response);
        Debug.Log($"@A RoomEnterResponseV3 = {oldJson}");


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
