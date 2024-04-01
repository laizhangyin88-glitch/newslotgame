using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SimpleJSON;
using BagelCode.ClientModels;
using JetBrains.Annotations;
using SlotMaker.Json;
using ParadoxNotion;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class Lobby : ActionTask
    {
        protected override string info { get { return "Request Lobby"; } }

        protected override void OnExecute()
        {

#if NEW_NET

            NetManager.Instance.Post(RPCName.lobby, null,
            (res) =>
            {
                string resStr = res.ToString();


                //long credit = res["balance"].AsLong;
                //BlackboardQueryUtils.SetMyCredit(credit);
                //Debug.LogError($"Refresh {BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value}");

                long oldCredit = (long)(BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value ?? 0);
                //long oldCredit = credit != null? (long)credit : 0;
                if (oldCredit != res["balance"].AsLong)
                {
                    BlackboardQueryUtils.SetMyCredit(res["balance"].AsLong);
                    Debug.LogError($"Refresh {BlackboardUtils.FindVariable(MainBlackboard.Get(), "me/credit").value}");

                    MessageDispatcher.Dispatch("OnCreditEvent", new EventData<bool>("UpdateNaviCredit", false));
                    //EventSender.SendGlobalEvent("OnCreditEvent", "UpdateNaviCredit");
                }


                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/lobby_response_v6");
                ClientModels.LobbyResponseV6 response = JsonUtility.FromJson<ClientModels.LobbyResponseV6>(jsn8.text);

                //response.contents = res["contents"].ToString();
                //string oldJson = JsonUtility.ToJson(response);
                //Debug.Log($"@A LobbyResponseV6 = {oldJson}");
                /*
                List<long> betList = res["bet_list"].AsStringList.Select(s => long.Parse(s)).ToList();
                response.betList = betList;
                response.contents = res["contents"].ToString();*/


                string res00 = res["bigwin_rank_list"].ToString();

                string res01 = NetManager.ChangeJsonKeyToCameCase(res00);


                //List<BigwinRankInfo> bigwinRankInfo = JsonUtility.FromJson<List<BigwinRankInfo>>(res01);  报错
                //List<BigwinRankInfo> bigwinRankInfo1 = SlotSimpleJson.DeserializeObject<List<BigwinRankInfo>>(res01); // 这个也是解析不出来，里面层有list
                //string testStr08 = SlotSimpleJson.SerializeObject(bigwinRankInfo1);

                response.bigwinRankList = null;


                //string testStr = JsonUtility.ToJson(response.bigwinRankList);


                if (response.bigwinRankList == null)
                {
                    response.bigwinRankList = new List<BigwinRankInfo>();
                   /* foreach (var item in res["bigwin_rank_list"])  //插件
                    {
                        string res003 = item.ToString();
                        string res02 = NetManager.ChangeJsonKeyToCameCase(res003);
                        BigwinRankInfo tmp01 = JsonUtility.FromJson<BigwinRankInfo>(res02);
                        response.bigwinRankList.Add(tmp01);
                    }*/

                    for (int i = 0; i < res["bigwin_rank_list"].Count; i++)
                    {
                        BigwinRankInfo br = new BigwinRankInfo();
                        br.rankList = new List<BigwinRankEntry>();

                        br.gameId = res["bigwin_rank_list"][i]["game_id"].AsInt;

                        JSONNode ranking_list = res["bigwin_rank_list"][i]["ranking_list"];
                        for (int k = 0; k < ranking_list.Count; k++)
                        {
                            BigwinRankEntry bre = new BigwinRankEntry();
                            bre.name = ranking_list[k]["name"];
                            bre.profileUrl = ranking_list[k]["profile_url"];
                            bre.bigwinCredit = ranking_list[k]["bigwin_credit"];
                            bre.tier = ranking_list[k]["tier"];

                            br.rankList.Add(bre);
                        }
                        response.bigwinRankList.Add(br);

                        //string res003 = res["bigwin_rank_list"][i].ToString();
                        //string res02 = NetManager.ChangeJsonKeyToCameCase(res003);
                        //BigwinRankInfo tmp01 = JsonUtility.FromJson<BigwinRankInfo>(res02);
                        //response.bigwinRankList.Add(tmp01);
                    }
                    //string testStr02 = JsonUtility.ToJson(response.bigwinRankList);
                    //string testStr05 = SlotSimpleJson.SerializeObject(response.bigwinRankList);

                }

                //globalStore.gameInfoList = res["game_info_list"]?? res["gameInfoList"]; 不可用，因为是JSONNode对象
                globalStore.gameInfoList = res.HasKey("game_info_list")? res["game_info_list"]:res["gameInfoList"];
                globalStore.gameInfoList = JSONNode.Parse(NetManager.ChangeJsonKeyToCameCase(globalStore.gameInfoList.ToString()));

                string resStr2 = globalStore.gameInfoList.ToString();

                Dictionary<string, string> slotWebImageURL = new Dictionary<string, string>();

                Dictionary<int, JSONNode> gameInfos = new Dictionary<int, JSONNode>();
                List<int> gameIds = new List<int>();
                for (int i = 0; i < globalStore.gameInfoList.Count; ++i)
                {
                    gameInfos.Add(globalStore.gameInfoList[i]["gameId"], globalStore.gameInfoList[i]);
                    gameIds.Add(globalStore.gameInfoList[i]["gameId"]);
                }

                var j = 0;
                while (j < response.gameInfoList.Count)
                {
                    if (gameIds.Contains(response.gameInfoList[j].gameId))
                    {
                        j++;
                    }
                    else
                    {
                        response.gameInfoList.RemoveAt(j);
                    }
                }
                j = 0;
                while (j < response.slotList.Count)
                {
                    if (gameIds.Contains(response.slotList[j].gameId))
                    {
                        j++;
                    }
                    else
                    {
                        response.slotList.RemoveAt(j);
                    }
                }

                for (int i = 0; i < response.gameInfoList.Count; ++i)
                {
                    JSONNode temp = gameInfos[response.gameInfoList[i].gameId];
                    response.gameInfoList[i].longImageUrl = temp["longImageUrl"];
                    response.gameInfoList[i].shortImageUrl = temp["shortImageUrl"];
                    response.gameInfoList[i].gameTitle = temp["gameTitle"];
                    response.gameInfoList[i].gameFilter = temp["gameFilter"];

                    //int type = temp["gameType"] ? temp["gameType"].AsInt : (int)GameType.UNKNOWN;
                    int type = temp["gameType"].AsInt;
                    response.gameInfoList[i].gameType = (BagelCode.ClientModels.GameType)type;
                }


                string oldJson = SlotSimpleJson.SerializeObject(response.gameInfoList);
                //;  JsonUtility.ToJson(response.gameInfoList);


                ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
                BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "shortcut");

                BlackboardQueryUtils.UpdateCelebInfo();
                BlackboardQueryUtils.UpdateGameAndSlotInfoFromLobby();  // 图片icon 和 路径
                IAMRouter.Instance.UpdateIAMInfo();

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.ApplyUserSyncInfo();
                BlackboardQueryUtils.CreateVIPLoungePrevBadgeCount();

                // For bi client_lobby_status.
                BiEventUtils.SetLobbyStatusData(response);
                BlackboardQueryUtils.SaveWebSocketHost(response.wsHost);

                EndAction(true);

            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
            return;

#endif
            if (ApplicationSettings.LogTest())
                Debug.Log("Request Lobby");

            BagelCodeClientAPI.Lobby(
        (response) =>
        {


            string oldJson = JsonUtility.ToJson(response);
            Debug.Log($"@A LobbyResponseV6 = {oldJson}");



            ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
            BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "shortcut");

            BlackboardQueryUtils.UpdateCelebInfo();
            BlackboardQueryUtils.UpdateGameAndSlotInfoFromLobby();  // 图片icon 和 路径
            IAMRouter.Instance.UpdateIAMInfo();

            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.ApplyUserSyncInfo();
            BlackboardQueryUtils.CreateVIPLoungePrevBadgeCount();

            // For bi client_lobby_status.
            BiEventUtils.SetLobbyStatusData(response);

            BlackboardQueryUtils.SaveWebSocketHost(response.wsHost);

            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
        }
    }

}
