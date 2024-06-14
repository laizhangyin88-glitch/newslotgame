using Newtonsoft.Json;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SimpleJSON;
using SlotMaker;
using SlotMaker.Contents;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class EnterGame : ActionTask<Blackboard>
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

        private List<List<int>> TranslateReelSetList(JSONNode list)
        {
            List<List<int>> temp = new List<List<int>>();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if(temp.Count <= 0)
                {
                    for (global::System.Int32 j = 0; j < item.Count; j++)
                    {
                        temp.Add(new List<int>());
                    }
                }
                for (global::System.Int32 j = 0; j < item.Count; j++)
                {
                    temp[j].Add(item[j] - 1);
                }
            }
            return temp;
        }

        private List<List<int>> TranslatePayTables(JSONNode node)
        {
            List<List<int>> temp = new List<List<int>>();
            for (int i = 0; i < node.Count; i++)
            {
                temp.Add(new List<int>());
                for (global::System.Int32 j = 0; j < 2; j++)
                {
                    temp[i].Add(0);
                }
            }
            for (int i = 0; i < node.Count; i++)
            {
                var item = node[i];
                foreach (var data in item)
                {
                    temp[i].Add(data.Value);
                }
            }
            return temp;
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
                    Debug.LogError("进入游戏的数据......." + res.ToString());
                    var contentJsonStr = "{\"game_type\":1,\"game_info\":{\"game_id\":21,\"game_title\":\"rhr\",\"base_wager\":30,\"pay_lines\":[[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[0,0,1,2,2],[2,2,1,0,0],[1,2,2,2,1],[1,0,0,0,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[1,2,1,0,1],[1,0,1,2,1],[0,2,0,2,0],[2,0,2,0,2],[1,0,2,0,1],[1,2,0,2,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0],[2,0,0,0,2],[0,0,1,0,0],[2,2,1,2,2],[0,1,2,2,2],[2,1,0,0,0],[1,0,1,0,1]]],\"reel_set_list\":[{\"reel_sequence_list\":[[7,6,7,0,4,5,4,5,8,4,5,4,6,9,7,1,3,1,7,6,9,7,6,7,6,8,7,5,4,5,6,7,9,6,7,6,9,6,7,4,5,4,5,6,7,6,2,3,2,6,7,0,6,7,9,6,7,6,7,9,6],[5,4,5,7,10,6,1,2,3,10,5,4,5,6,8,8,3,2,3,6,10,5,4,5,10,6,5,4,5,10,6,5,4,8,8,5,6,10,6,0,10,1,2,1,6,10,0,6,10,3,6,10],[5,4,5,7,9,5,4,5,7,10,7,4,5,4,9,6,7,10,7,5,4,5,10,7,6,2,3,1,8,8,8,2,1,3,9,4,5,4,7,9,7,0,10,3,1,2,9,4,5,4,10],[1,2,3,6,8,8,7,0,3,1,2,0,6,3,2,3,0,7,6,0,7,1,3,2,7,6,5,4,5,7,10,6,0,6,7,6,3,2,3,4,5,4,6,7,6,7,2,3,1,0,6],[6,4,5,4,3,2,1,0,6,7,6,5,4,5,1,3,2,8,6,7,6,9,5,4,5,9,6,2,1,3,7,5,4,5,0,7,1,3,1,6,5,4,5,2,3,2,4,5,4]]},{\"reel_sequence_list\":[[7,6,7,11,4,5,4,5,8,4,5,4,6,9,7,1,3,1,7,6,9,7,6,7,6,8,7,5,4,5,6,7,9,6,7,6,9,6,7,4,5,4,5,6,7,6,2,3,2,6,7,11,6,7,9,6,7,6,7,9,6],[5,4,5,7,10,6,1,2,3,10,5,4,5,6,8,8,3,2,3,6,10,5,4,5,10,6,5,4,5,10,6,5,4,8,8,5,6,10,6,11,10,1,2,1,6,10,11,6,10,3,6,10],[5,4,5,7,9,5,4,5,7,10,7,4,5,4,9,6,7,10,7,5,4,5,10,7,6,2,3,1,8,8,8,2,1,3,9,4,5,4,7,9,7,11,10,3,1,2,9,4,5,4,10],[1,2,3,6,8,8,7,11,3,1,2,11,6,3,2,3,6,7,6,11,7,1,3,2,7,6,5,4,5,7,10,6,11,6,7,6,3,2,3,4,5,4,6,7,6,7,6,2,3,1,11,6,7],[6,4,5,4,3,2,1,11,6,7,6,5,4,5,1,3,2,8,6,7,6,9,5,4,5,9,6,2,1,3,7,5,4,5,11,7,1,3,1,6,5,4,5,2,3,2,4,5,4]]}],\"paytables\":[[[0,0,125,600,1500],[0,0,50,200,400],[0,0,30,85,200],[0,0,15,45,90],[0,0,10,30,75],[0,0,8,30,60],[0,0,4,8,20],[0,0,2,5,15],[0,0,0,0,0],[0,0,0,0,0],[0,0,0,0,0],[0,0,125,600,1500],[0,0,5,25,50],[0,0,2,5,8]]],\"bonus_info\":{\"hot_rush_scatter_pay\":[0,0,1,3,8,30,75,500,1000,1000,1000,1000,1000,1000],\"free_spin_count\":[0,0,0,15],\"cash_wheel\":{\"total_spot\":20,\"item_index_list\":[0,1,0,0,3,0,0,2,0,0,1,0,0,1,0,0,0,1,0,0],\"item_value_list\":[3,0,5,3,0,4,10,0,3,2,0,4,5,0,2,3,6,0,2,4],\"item_weight_list\":[10,8,10,10,3,10,10,6,10,10,8,10,10,8,10,10,10,8,10,10],\"item_weight_total\":181}},\"extra_bet_ratio_list\":[{\"numerator\":0,\"denominator\":1}],\"custom_data\":null,\"reel_set_index\":{\"current_index\":0,\"next_index\":0}},\"jackpot_info\":{\"type\":2,\"info\":{\"eligible_min_bet\":0,\"eligible_min_bet_per_jackpot\":[0,0,0,0,0],\"base_bet\":30,\"info_list\":[{\"current\":240.9,\"prev\":240.9},{\"current\":913.26,\"prev\":913.26},{\"current\":2375.25,\"prev\":2375.25},{\"current\":15100.2,\"prev\":15100.2},{\"current\":30050.1,\"prev\":30050.1}]}},\"gamble_asset_bundle_name\":\"\",\"win_type_multiplier_info\":{\"BIG\":10,\"SUPER_BIG\":20,\"MEGA\":30,\"SUPER_MEGA\":50,\"EPIC\":100},\"delay_between_spin_ms\":0,\"contents_store_info\":{\"game_id\":21,\"contents_store\":{}}}";
                    var contentJson = JSONNode.Parse(contentJsonStr);

                    contentJson["game_info"]["game_id"] = gameID.value;
                    contentJson["game_info"]["game_title"] = globalStore.GetGameTitle(gameID.value);
                    JSONNode tempJson = JSONNode.Parse("[]");
                    tempJson = res["game_config"]["win_line"];
                    contentJson["game_info"]["pay_lines"].Clear();
                    contentJson["game_info"]["pay_lines"].Add(tempJson);
                    contentJson["game_info"]["base_wager"] = res["game_config"]["win_line"].Count;

                    var tempCard = TranslatePayTables(res["game_config"]["card_mutiple"]);
                    string cardStr = JsonConvert.SerializeObject(tempCard);
                    JSONNode cardNode = JSONNode.Parse(cardStr);
                    contentJson["game_info"]["paytables"].Clear();
                    contentJson["game_info"]["paytables"].Add(cardNode);

                    ///转换 普通列表
                    var tempList1 = TranslateReelSetList(res["game_config"]["regular_game_reel"]);
                    Dictionary<string, List<List<int>>> tempArray = new Dictionary<string, List<List<int>>>();
                    tempArray.Add("reel_sequence_list", tempList1);
                    string tempStr2 = JsonConvert.SerializeObject(tempArray);
                    JSONNode node1 = JSONNode.Parse(tempStr2);
                    ///转换免费列表
                    var tempList2 = TranslateReelSetList(res["game_config"]["free_game_reel"]);
                    Dictionary<string, List<List<int>>> tempArray2 = new Dictionary<string, List<List<int>>>();
                    tempArray2.Add("reel_sequence_list", tempList2);                    
                    string tempStr3 = JsonConvert.SerializeObject(tempArray);
                    JSONNode node2 = JSONNode.Parse(tempStr3);

                    contentJson["game_info"]["reel_set_list"].Clear();
                    contentJson["game_info"]["reel_set_list"].Add(node1);
                    contentJson["game_info"]["reel_set_list"].Add(node2);

                    //string resStr = res.ToString();

                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/room_enter_response_v3");
                    ClientModels.RoomEnterResponseV3 response = JsonUtility.FromJson<ClientModels.RoomEnterResponseV3>(jsn8.text);

                    ////System.Collections.Generic.List<long> betList = res["bet_list"].AsStringList.Select(s => long.Parse(s)).ToList();
                    ////response.betList = betList;

                    response.betList = new List<long>() { 30, 60, 120, 300, 600, 1200, 6000 };
                    Debug.LogError("!!!整合后的数据:" + contentJson.ToString());

                    response.contents = contentJson.ToString();

                    var bb = ContentBlackboard.Get();
                    //Debug.LogError(contentJson);
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

                        BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastGameSpin", true);
                    }
                    else
                    {
                        BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "isLastGameSpin", false);
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

                    if (res.HasKey("last_regular_message") && res["last_regular_message"] != null)
                    {
                        Debug.Log($"【last_regular_message】  = {res["last_regular_message"].ToString()} ");
                        LastFreeGameManager.Instance.GetFreeSpinHistory(res["last_regular_message"].ToString());
                    }
                    else
                    {
                        Debug.Log("【last_regular_message】  is null");
                    }

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
                    if (agent != null)
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

        private void Serialize(IBlackboard bb, BagelCode.ClientModels.RoomEnterResponseV3 roomEnterResponse)
        {
            ClientAPI2Blackboard.Serialize(bb, roomEnterResponse);
            BlackboardUtils.SetOrCreateValue<ContentsRequestType>(bb, "requestType", ContentsRequestType.Enter);
            ContentsSerializer.Deserialize(bb);
        }

        private void ErrorHandle(BagelCodeHTTPError error)
        {
            switch (error.errorCode)
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
