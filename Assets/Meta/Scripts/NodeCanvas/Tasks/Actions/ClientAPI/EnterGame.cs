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
using BagelCode.ClientModels;
using SlotMaker.Slots;
using System.Runtime.Remoting.Contexts;



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
            //新游戏接口
            if (globalStore.IsNewGame(gameID.value))
            {
                NetManager.Instance.Post(RPCName.newEnterGame, req,
                (res) =>
                {
                    var contentJsonStr = "{\"game_type\":1,\"game_info\":{\"game_id\":21,\"game_title\":\"rhr\",\"base_wager\":30,\"pay_lines\":[[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[0,0,1,2,2],[2,2,1,0,0],[1,2,2,2,1],[1,0,0,0,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[1,2,1,0,1],[1,0,1,2,1],[0,2,0,2,0],[2,0,2,0,2],[1,0,2,0,1],[1,2,0,2,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0],[2,0,0,0,2],[0,0,1,0,0],[2,2,1,2,2],[0,1,2,2,2],[2,1,0,0,0],[1,0,1,0,1]]],\"reel_set_list\":[{\"reel_sequence_list\":[[7,6,7,0,4,5,4,5,8,4,5,4,6,9,7,1,3,1,7,6,9,7,6,7,6,8,7,5,4,5,6,7,9,6,7,6,9,6,7,4,5,4,5,6,7,6,2,3,2,6,7,0,6,7,9,6,7,6,7,9,6],[5,4,5,7,10,6,1,2,3,10,5,4,5,6,8,8,3,2,3,6,10,5,4,5,10,6,5,4,5,10,6,5,4,8,8,5,6,10,6,0,10,1,2,1,6,10,0,6,10,3,6,10],[5,4,5,7,9,5,4,5,7,10,7,4,5,4,9,6,7,10,7,5,4,5,10,7,6,2,3,1,8,8,8,2,1,3,9,4,5,4,7,9,7,0,10,3,1,2,9,4,5,4,10],[1,2,3,6,8,8,7,0,3,1,2,0,6,3,2,3,0,7,6,0,7,1,3,2,7,6,5,4,5,7,10,6,0,6,7,6,3,2,3,4,5,4,6,7,6,7,2,3,1,0,6],[6,4,5,4,3,2,1,0,6,7,6,5,4,5,1,3,2,8,6,7,6,9,5,4,5,9,6,2,1,3,7,5,4,5,0,7,1,3,1,6,5,4,5,2,3,2,4,5,4]]},{\"reel_sequence_list\":[[7,6,7,11,4,5,4,5,8,4,5,4,6,9,7,1,3,1,7,6,9,7,6,7,6,8,7,5,4,5,6,7,9,6,7,6,9,6,7,4,5,4,5,6,7,6,2,3,2,6,7,11,6,7,9,6,7,6,7,9,6],[5,4,5,7,10,6,1,2,3,10,5,4,5,6,8,8,3,2,3,6,10,5,4,5,10,6,5,4,5,10,6,5,4,8,8,5,6,10,6,11,10,1,2,1,6,10,11,6,10,3,6,10],[5,4,5,7,9,5,4,5,7,10,7,4,5,4,9,6,7,10,7,5,4,5,10,7,6,2,3,1,8,8,8,2,1,3,9,4,5,4,7,9,7,11,10,3,1,2,9,4,5,4,10],[1,2,3,6,8,8,7,11,3,1,2,11,6,3,2,3,6,7,6,11,7,1,3,2,7,6,5,4,5,7,10,6,11,6,7,6,3,2,3,4,5,4,6,7,6,7,6,2,3,1,11,6,7],[6,4,5,4,3,2,1,11,6,7,6,5,4,5,1,3,2,8,6,7,6,9,5,4,5,9,6,2,1,3,7,5,4,5,11,7,1,3,1,6,5,4,5,2,3,2,4,5,4]]}],\"paytables\":[[[0,0,125,600,1500],[0,0,50,200,400],[0,0,30,85,200],[0,0,15,45,90],[0,0,10,30,75],[0,0,8,30,60],[0,0,4,8,20],[0,0,2,5,15],[0,0,0,0,0],[0,0,0,0,0],[0,0,0,0,0],[0,0,125,600,1500],[0,0,5,25,50],[0,0,2,5,8]]],\"bonus_info\":{\"hot_rush_scatter_pay\":[0,0,1,3,8,30,75,500,1000,1000,1000,1000,1000,1000],\"free_spin_count\":[0,0,0,15],\"cash_wheel\":{\"total_spot\":20,\"item_index_list\":[0,1,0,0,3,0,0,2,0,0,1,0,0,1,0,0,0,1,0,0],\"item_value_list\":[3,0,5,3,0,4,10,0,3,2,0,4,5,0,2,3,6,0,2,4],\"item_weight_list\":[10,8,10,10,3,10,10,6,10,10,8,10,10,8,10,10,10,8,10,10],\"item_weight_total\":181}},\"extra_bet_ratio_list\":[{\"numerator\":0,\"denominator\":1}],\"custom_data\":null,\"reel_set_index\":{\"current_index\":0,\"next_index\":0}},\"jackpot_info\":{\"type\":2,\"info\":{\"eligible_min_bet\":0,\"eligible_min_bet_per_jackpot\":[0,0,0,0,0],\"base_bet\":30,\"info_list\":[{\"current\":240.9,\"prev\":240.9},{\"current\":913.26,\"prev\":913.26},{\"current\":2375.25,\"prev\":2375.25},{\"current\":15100.2,\"prev\":15100.2},{\"current\":30050.1,\"prev\":30050.1}]}},\"gamble_asset_bundle_name\":\"\",\"win_type_multiplier_info\":{\"BIG\":10,\"SUPER_BIG\":20,\"MEGA\":30,\"SUPER_MEGA\":50,\"EPIC\":100},\"delay_between_spin_ms\":0,\"contents_store_info\":{\"game_id\":21,\"contents_store\":{}}}";
                    var contentJson = JSONNode.Parse(contentJsonStr);

                    contentJson["game_info"]["game_id"] = gameID.value;
                    contentJson["game_info"]["game_title"] = globalStore.GetGameTitle(gameID.value);
                    JSONNode tempJson = JSONNode.Parse("[]");
                    tempJson = res["game_config"]["win_line"];
                    contentJson["game_info"]["pay_lines"].Clear();
                    contentJson["game_info"]["pay_lines"].Add(tempJson);
                    contentJson["game_info"]["base_wager"] = res["game_config"]["win_line"].Count;


                    //string resStr = res.ToString();

                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/room_enter_response_v3");
                    ClientModels.RoomEnterResponseV3 response = JsonUtility.FromJson<ClientModels.RoomEnterResponseV3>(jsn8.text);

                    ////System.Collections.Generic.List<long> betList = res["bet_list"].AsStringList.Select(s => long.Parse(s)).ToList();
                    ////response.betList = betList;

                    response.betList = new List<long>() { 30, 60, 120, 300, 600, 1200, 6000 };
                    Debug.Log("!!!json新:" + contentJson.ToString());

                    response.contents = contentJson.ToString();

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
            }
            else
            {
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
                    //if (res["contents"]["game_info"]["game_id"].AsInt == 21)
                    //{
                    //    res["contents"]["game_info"]["game_title"] = "bst";
                    //}
                    response.contents = res["contents"].ToString();
                    Debug.Log("!!!json旧:" + response.contents.ToString());



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
            }
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
