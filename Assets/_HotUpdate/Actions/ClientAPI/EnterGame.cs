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

        protected override void OnExecute()
        {
            var context_id = BlackboardUtils.FindVariable<string>(null, "/enterGameInfo/contextID");

#if NEW_NET

            RPCHall.ReqEnterGame req = new RPCHall.ReqEnterGame();
            req.game_id = gameID.value;

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
                    if (res.HasKey("last_regular_message") && res["last_regular_message"] != null)
                    {
                        Debug.Log($"【last_regular_message】  = {res["last_regular_message"].ToString()} ");
                        LastFreeGameManager.Instance.GetFreeSpinHistory(res["last_regular_message"].ToString());
                        var temp1 = ResetEnterGameData(resStr, res["last_regular_message"].ToString());
                        if (temp1 != null)
                        {
                            res = temp1;
                        }
                    }
                    else
                    {
                        Debug.Log("【last_regular_message】  is null");
                    }
                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/room_enter_response_v3");
                    ClientModels.RoomEnterResponseV3 response = JsonUtility.FromJson<ClientModels.RoomEnterResponseV3>(jsn8.text);

                    System.Collections.Generic.List<long> betList = res["bet_list"].AsStringList.Select(s => long.Parse(s)).ToList();
                    response.betList = betList;
                    response.contents = res["contents"].ToString();

                    if (res["use_forced_extra_bet_ratio"] != null)
                    {
                        response.useForcedExtraBetRatio = res["use_forced_extra_bet_ratio"].AsBool;
                    }

                    if (res["forced_extra_bet_ratio_index_list"] != null)
                    {
                        response.forcedExtraBetRatioIndexList = res["forced_extra_bet_ratio_index_list"].AsStringList.Select(s => int.Parse(s)).ToList();
                    }

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
        /// <summary>
        /// 断线重连的时候，重置一些有必要的游戏数据
        /// </summary>
        /// <param name="enterData"></param>
        /// <param name="lastSlotSpinData"></param>
        /// <returns></returns>
        private JSONNode ResetEnterGameData(string enterData, string lastSlotSpinData)
        {
            if(globalStore.nowGameID == 167)
            {
                JSONNode gameData = JSONNode.Parse(enterData);
                JSONNode lastSlotSpin = JSONNode.Parse(lastSlotSpinData);
                if(lastSlotSpin != null)
                {
                    JSONNode debug = JSONNode.Parse(lastSlotSpin["client_data"]["debug_param"]);
                    if(debug != null)
                    {
                        JSONNode slotSpinData = debug["slotSpinData"];
                        if(gameData != null)
                        {
                            gameData["contents"]["game_info"]["custom_data"]["collected_wild_list"] = slotSpinData;
                            return gameData;
                        }
                    }
                }
            }
            return null;
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
