#define NEW_NET

using BagelCode.ClientModels;
using BagelCode.Internal;
using Dreamteck.Splines.Primitives;
using NodeCanvas.Framework;
using ParadoxNotion;
using SimpleJSON;
using SlotMaker;
using SlotMaker.Contents;
using SlotMaker.Json;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Action = System.Action;

//using System.Runtime.Remoting.Contexts

namespace BagelCode
{
    public class V3MetaSystem : IMetaSystem
    {
        public void SelectGame(int gameId)
        {
            var gameInfo = BlackboardQueryUtils.GetGameInfo(gameId);
            BlackboardQueryUtils.SetEnterGameInfo(gameId, "EnterGame", "default");
        }

        public void EnterGame()
        {
            EventSender.SendGlobalEvent("OnEnterGame");
        }

        public void ClearContentData()
        {
            var bb = ContentBlackboard.Get();

            bb.AddVariable("isEarlyAccess", false);
            bb.AddVariable("levelRestriction", 0);
            bb.AddVariable("isGameSpin", false);
            bb.AddVariable("spinType", SpinType.None);
            bb.AddVariable("gameSpinCount", 0);
            bb.AddVariable("enabledMaxBet", true);
            bb.AddVariable("enabledSuperBonus", false);
            bb.AddVariable("enabledBuyABonus", false);
            bb.AddVariable("enabledInstantBonus", false);
            bb.AddVariable("bonusIamInfo", typeof(Blackboard));
        }

        public void SlotSpin(long betCredit, long extraBetCredit, object customData, Action successCallback, Action errorCallback)
        {
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            int collectingGameChestDropRateMultiplyEventId = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "collectingGameChestDropRateMultiplyEventId").value;
            // int collectingGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "collectingGameEventID").value;
            string roomID = BlackboardUtils.GetOrCreateVariable<string>(null, "./room/roomId").value;

            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGameSpin = (spinType == SpinType.GameSpin);
            bool isBonusSpin = (spinType == SpinType.BonusSpin);
            bool isAutoSpin = BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "autoSpin").value;

            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
            // customData = ParseSlotSpinCustomData(customData);

            List<int> expEventIdList = BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY);
            expEventIdList.AddRange(BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY_EXTENDABLE));

            bool isHighRollerBet = BlackboardUtils.FindVariable<bool>("/isExtendedBetIndex")?.value ?? false;
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

#if NEW_NET

            if (TestManager.Instance.isTestSpin)
            {
                TestManager.Instance.getSpinData((res) =>
                {
                    string resStr = res.ToString();
                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                    ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                    response.contents = res["contents"].ToString();

                    SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                    if (successCallback != null)
                        successCallback();
                });
                return;
            }

            /*string testRes = TestManager.Instance.getSpin();
            if (testRes != "")
            {
                SimpleJSON.JSONNode dataDict = SimpleJSON.JSONNode.Parse(testRes as string);
                SimpleJSON.JSONNode res = dataDict["data"];

                    string resStr = res.ToString();
                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                    ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                    response.contents = res["contents"].ToString();

                    SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                    if (successCallback != null)
                        successCallback();
                return;
            }*/

            if (LastFreeGameManager.Instance.isLastGameSpin)
            {
                LastFreeGameManager.Instance.getResponseData(RPCName.slotSpin,
                (res) =>
                {
                    string resStr = res.ToString();
                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                    ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                    response.contents = res["contents"].ToString();

                    SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                    if (successCallback != null)
                        successCallback();
                });
                return;
            }

            /*string debug_param = "";
            if (globalStore.test_spin_tab.Length > 0)
            {
                string lstStr = "[";
                for (int i =0;i<globalStore.test_spin_tab.Length;i++)
                {
                    lstStr += $"{globalStore.test_spin_tab[i]},";
                }
                lstStr += "]";
                lstStr = lstStr.Replace(",]","]");
                debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + ",\"reel_output_list\":"+ lstStr + "}";
            }
            else
            {
                debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + "}";
            }*/

            string debug_param = "";
            int code = TestManager.Instance.getCode();
            int[] lst = TestManager.Instance.getList();
            if (lst.Length > 0)
            {
                string lstStr = "[";
                for (int i = 0; i < lst.Length; i++)
                {
                    lstStr += $"{lst[i]},";
                }
                lstStr += "]";
                lstStr = lstStr.Replace(",]", "]");
                debug_param = "{\"is_free_spin\":" + code + ",\"reel_output_list\":" + lstStr + "}";
            }
            else
            {
                debug_param = "{\"is_free_spin\":" + code + "}";
            }
            //新游戏接口
            if (globalStore.IsNewGame(gameId))
            {
                //var test = "[[2,2,2,3,4],[5,1,8,5,3],[1,2,5,4,3]]";
                //var debug_param1 = "[\"shuffling_list\":" + test + "]";

                ///测试用的数据
                List<int> debug_param2 = new List<int>() { 58, 127, 154, 10, 128 };

                Dictionary<string, object> debug_param1 = new Dictionary<string, object> {
                    {"first_index_list",
                        debug_param2
                    },
                    {
                        "is_free",
                        0
                    }
                };
                Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"bet",betCredit},
                {"extra_bet",extraBetCredit },
                { "debug_param", debug_param1},
                
            };
                Debug.Log("@ SlotSpin is_free_spin : " + debug_param);

                NetManager.Instance.Post(RPCName.new_slot_spin, req,
                (res) =>
                {
                    Debug.LogError("拉霸下发数据......." + res.ToString());
                    Debug.LogError("拉霸结果....... + " + res["game_result"]["first_index_list"].ToString());
                    //var contentStr = "{\"game_id\":21,\"result\":{\"reel_output_list\":[47,19,36,21,38],\"paytable_index\":0,\"earn_credit\":0},\"bonus_result\":[],\"custom_data\":{},\"reel_set_index\":{\"current_index\":0,\"next_index\":0},\"free_spin_info\":{\"type\":0,\"bet\":0,\"count\":0,\"spun_count\":0,\"total_count\":0,\"initial_count\":0,\"multiplier\":0,\"sticky_wild\":[],\"extra_data\":{}},\"jackpot_info\":{\"type\":2,\"info\":{\"eligible_min_bet\":0,\"eligible_min_bet_per_jackpot\":[0,0,0,0,0],\"base_bet\":30,\"info_list\":[{\"current\":241.02,\"prev\":240.96},{\"current\":913.5,\"prev\":913.38},{\"current\":2375.85,\"prev\":2375.55},{\"current\":15100.68,\"prev\":15100.44},{\"current\":30050.34,\"prev\":30050.22}]}},\"contents_store\":{}}";
                    //var content = JSONNode.Parse(contentStr);

                    TextAsset jsn7 = Resources.Load<TextAsset>("tempdata/slot_spin_content_id21");
                    var content = JSONNode.Parse(jsn7.text);

                    var result = res["game_result"]["total_result"];
                    for (int i = 0; i < result.Count; i++)
                    {
                        var temp = result[i];
                        if (temp["win_line"] != null)
                        { 
                            Debug.LogError("win line ...... + " + temp["win_line"].ToString()); 
                        }
                    }
                    
                    //var contentStr = "{\"game_id\":21,\"result\":{\"reel_output_list\":[47,19,36,21,38],\"paytable_index\":0,\"earn_credit\":0},\"bonus_result\":[],\"custom_data\":{},\"reel_set_index\":{\"current_index\":0,\"next_index\":0},\"free_spin_info\":{\"type\":0,\"bet\":0,\"count\":0,\"spun_count\":0,\"total_count\":0,\"initial_count\":0,\"multiplier\":0,\"sticky_wild\":[],\"extra_data\":{}},\"jackpot_info\":{\"type\":2,\"info\":{\"eligible_min_bet\":0,\"eligible_min_bet_per_jackpot\":[0,0,0,0,0],\"base_bet\":30,\"info_list\":[{\"current\":241.02,\"prev\":240.96},{\"current\":913.5,\"prev\":913.38},{\"current\":2375.85,\"prev\":2375.55},{\"current\":15100.68,\"prev\":15100.44},{\"current\":30050.34,\"prev\":30050.22}]}},\"contents_store\":{}}";
                    //var content = JSONNode.Parse(contentStr);
                    content["game_id"] = gameId;
                    content["result"]["earn_credit"] = res["game_result"]["earn_credit"];

                    content["result"]["reel_output_list"] = res["game_result"]["first_index_list"]; //获取索引

                    if (res["game_result"]["free_game_credit"] != null) ///记录铃铛的得分
                    {
                        content["result"]["free_game_credit"] = res["game_result"]["free_game_credit"];
                    }
#if UNITY_EDITOR
                    string testStr0 = "";
                    for (int i = 0; i < res["game_result"]["shuffling_list"].Count; i++)
                    {
                        for (int j = 0; j < res["game_result"]["shuffling_list"][i].Count; j++)
                        {
                            testStr0 += $"{(int)res["game_result"]["shuffling_list"][i][j]},";
                        }
                        testStr0 += "\n";
                    }
                    Debug.Log($"==@ game_reel\n{testStr0}");

                    string testStr1 = "";
                    string testStr2 = "";
                    int k = 0;
                    while (k<3)
                    {
                        for (int i =0;i< res["game_result"]["first_index_list"].Count; i++)
                        {
                            int j = res["game_result"]["first_index_list"][i]+k;

                            if (j >= globalStore.reelSetList1[i].Count)
                            {
                                j -= globalStore.reelSetList1[i].Count;
                            }
                            testStr1 += $"{(int)globalStore.reelSetList1[i][j]},";
                            testStr2 += $"{(int)globalStore.reelSetList2[i][j]},";
                        }
                        testStr1 += '\n';
                        testStr2 += '\n';
                        k++;
                    }
                    Debug.Log($"==@ regular_game_reel\n{testStr1}");
                    Debug.Log($"==@ free_game_reel\n{testStr2}");
#endif


                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                    //保存连线的结果
                    content["result"]["total_line_result"] = res["game_result"]["total_result"];
                    //保存奖励数据
                    content["result"]["win_line_reward_list"] = res["game_result"]["win_line_reward_list"];

                    content["result"]["spin_once_earn_credit"] = res["game_result"]["earn_credit"];

                    ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                    response.contents = content.ToString();
                    Debug.Log("###新的slotSpin：" + res.ToString());


                    SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                    SlotSpinSuccessNew(res.ToString());

                    if (successCallback != null)
                        successCallback();
                },
                (error) =>
                {
                    CommonError(error);

                    if (errorCallback != null)
                        errorCallback();
                });
            }
            else
            {
                Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"bet",betCredit},
                {"extra_bet",extraBetCredit },
                {"game_id",gameId },
                {"custom_data", (customData is int) ? new { extra_int = (int)customData } : customData},
                {"contents_version",ContentsVersionManager.Instance.GetContentsVersion() },
                // { "debug_param", "{\"is_free_spin\":0}"}
                { "debug_param", debug_param}
            };
                //globalStore.test_is_free_spin = 0;

                NetManager.Instance.Post(RPCName.slotSpin, req,
                (res) =>
                {
                    string resStr = res.ToString();
                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                    ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                    response.contents = res["contents"].ToString();
                    Debug.Log("###旧的slotSpin：" + res["contents"].ToString());

                    SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                    if (successCallback != null)
                        successCallback();
                },
                (error) =>
                {
                    CommonError(error);

                    if (errorCallback != null)
                        errorCallback();
                });
            }
            return;
#endif

            BagelCodeClientAPI.SlotSpin(betCredit, extraBetCredit, roomID, isGameSpin, isBonusSpin, metaGameEventID, gameId, collectingGameChestDropRateMultiplyEventId, customData, isAutoSpin, expEventIdList, isHighRollerBet, seasonPassEventId,
            (response) =>
            {
                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A SlotSpinResponseV3 = {oldJson}");

                SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }


        public void SlotSpinSuccessNew(string str)
        {
            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            BlackboardUtils.SetOrCreateValue(spinBB.value, "responseNew", str);
        }


        public void SlotSpinSuccess(ClientModels.SlotSpinResponseV3 response, long betCredit, long extraBetCredit, SpinType spinType)
        {
            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            if (spinBB == null || spinBB.value == null) return;

            var bb = BlackboardUtils.GetOrCreateBlackboard(spinBB.value, "response");
            var json = JSONNode.Parse(response.contents);
            var totalList = new List<List<int>>();
            //保存划线的数据
            for (int i = 0; i < json["result"]["total_line_result"].Count; i++)
            {
                var list = json["result"]["total_line_result"][i];
                if (list["win_line"] != null)
                {   ///保存连线的结果
                    var list1 = new List<int>();
                    for (int j = 0; j < list["win_line"].Count; j++)
                    {
                        list1.Add(list["win_line"][j]);
                    }
                    totalList.Add(list1);
                }
            } 
            BlackboardUtils.SetOrCreateValue(bb, "total_line_result", totalList);
            if(json["result"]["free_game_credit"] != null)///保存铃铛的得分
            {
                long freeGameCredit = json["result"]["free_game_credit"];
                BlackboardUtils.SetOrCreateValue(bb, "free_game_credit", freeGameCredit);
            }
            //保存中奖的数据
            List<Dictionary<string, object>> dict = new List<Dictionary<string, object>>();
            for (int i = 0; i < json["result"]["win_line_reward_list"].Count; i++)
            {
                var list = json["result"]["win_line_reward_list"][i];
                Dictionary<string, object> temp = new Dictionary<string, object>();
                if (list["index"] != null)
                {
                    temp.Add("index", list["index"]);
                }
                if (list["credit"]!= null)
                {
                    temp.Add("credit", list["credit"]);
                }
                dict.Add(temp);
            }
            BlackboardUtils.SetOrCreateValue(bb, "win_line_reward_list", dict);

            long spin_once_earn = json["result"]["spin_once_earn_credit"];
            BlackboardUtils.SetOrCreateValue(bb, "spin_once_earn_credit", spin_once_earn);
            ClientAPI2Blackboard.Serialize(bb, response);
            BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.SlotSpin);
            ContentsSerializer.Deserialize(bb);

            BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
            BlackboardQueryUtils.UpdatePotOfGold(response.userSyncInfo.piggyCredit);
            BlackboardQueryUtils.UpdateUnlockFeature(response.featureUnlockList);
            BlackboardQueryUtils.UpdateMetaGameInfo(response.metaGameInfo, response.serverTime);
            BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betCredit, response.gameSpinCount, "./gameSpinCountPerBet");

            BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betCredit, response.bonusSpinCount, "./bonusSpinCountPerBet");
            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.UpdateHiddenUniverseFinder(response.earnFinder);
            BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeCompositeInfo?.vipLoungeInfo ?? null);
            BlackboardQueryUtils.UpdateVegasDreamsTotalDepotCount(response.vipLoungeCompositeInfo?.buildDreamInfo ?? null);
            BlackboardQueryUtils.UpdateMysteryGiftInfo(response.nextMysteryGiftLevel, response.mysteryGiftInfo, response.serverTime);
            LevelUpDash.LevelUpDash.Utils.UpdateLevelUpDashInfo(response.levelUpDashInfo);
        }

        public void SlotClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback)
        {
#if NEW_NET

            if (TestManager.Instance.isTestClaimBonus)
            {
                TestManager.Instance.getClaimBonusData((res) =>
                {
                    string resStr = res.ToString();

                    //string oldJson = JsonUtility.ToJson(response);
                    //Debug.Log($"@A KenoPlayResponseV1 = {oldJson}");

                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_claim_bonus_response_v3");
                    ClientModels.SlotClaimBonusResponseV3 response = JsonUtility.FromJson<ClientModels.SlotClaimBonusResponseV3>(jsn8.text);

                    response.contents = res["contents"].ToString();

                    var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("turn"), "claims");
                    bb = BlackboardUtils.GetOrCreateBlackboard(bb, claimId);
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.SlotClaimBonus);

                    ContentsSerializer.Deserialize(bb);

                    // BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                    if (successCallback != null)
                        successCallback();
                });
                return;
            }



            if (globalStore.nowGameID == 33 || globalStore.IsNewGame(globalStore.nowGameID)) //雷神
            {
                if (successCallback != null)
                    successCallback();
                return;
            }

            if (LastFreeGameManager.Instance.isLastGameSpin)
            {
                LastFreeGameManager.Instance.getResponseData(RPCName.claimBonus,
                (res) =>
                {
                    string resStr = res.ToString();

                    //string oldJson = JsonUtility.ToJson(response);
                    //Debug.Log($"@A KenoPlayResponseV1 = {oldJson}");

                    TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_claim_bonus_response_v3");
                    ClientModels.SlotClaimBonusResponseV3 response = JsonUtility.FromJson<ClientModels.SlotClaimBonusResponseV3>(jsn8.text);

                    response.contents = res["contents"].ToString();

                    var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("turn"), "claims");
                    bb = BlackboardUtils.GetOrCreateBlackboard(bb, claimId);
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.SlotClaimBonus);

                    ContentsSerializer.Deserialize(bb);

                    // BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    if (successCallback != null)
                        successCallback();
                });
                return;
            }

            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;
            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            bool isGameSpin = (spinType == SpinType.GameSpin);
            bool isBonusSpin = (spinType == SpinType.BonusSpin);
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

            /* string selected_index = "{\"selected_index\":" + (int)customData + "}";
             Dictionary<string, object> req = new Dictionary<string, object>
             {
                 {"decision_info",selected_index},
             };*/

            //string debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + "}";
            //string debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + ",\"reel_table\":[5,23,15]}";
            //Debug.Log("@ SlotSpin is_free_spin : " + debug_param);

            JSONNode data = JSONNode.Parse("{}");
            data.Add("selected_index", (int)customData);

            //data.Add("debug_param", debug_param);
            JSONNode req = JSONNode.Parse("{}");
            req.Add("decision_info", data);
            req.Add("uid", claimId);
            req.Add("isGameSpin", isGameSpin);
            req.Add("isBonusSpin", isBonusSpin);
            req.Add("metaGameEventId", metaGameEventID);
            req.Add("seasonPassEventId", seasonPassEventId);

            NetManager.Instance.Post(RPCName.claimBonus, req,
            (res) =>
            {
                string resStr = res.ToString();

                //string oldJson = JsonUtility.ToJson(response);
                //Debug.Log($"@A KenoPlayResponseV1 = {oldJson}");

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_claim_bonus_response_v3");
                ClientModels.SlotClaimBonusResponseV3 response = JsonUtility.FromJson<ClientModels.SlotClaimBonusResponseV3>(jsn8.text);

                response.contents = res["contents"].ToString();

                var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("turn"), "claims");
                bb = BlackboardUtils.GetOrCreateBlackboard(bb, claimId);
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.SlotClaimBonus);

                ContentsSerializer.Deserialize(bb);

                // BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
            return;/**/

            /*if (successCallback != null)
                successCallback();
            return;*/
#else

            int gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId").value;

            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            bool isGameSpin = (spinType == SpinType.GameSpin);
            bool isBonusSpin = (spinType == SpinType.BonusSpin);
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

            BagelCodeClientAPI.SlotClaimBonus(gameId, metaGameEventID, claimId, isGameSpin, isBonusSpin, customData, seasonPassEventId,
            (response) =>
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("turn"), "claims");
                bb = BlackboardUtils.GetOrCreateBlackboard(bb, claimId);
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.SlotClaimBonus);

                ContentsSerializer.Deserialize(bb);

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });

#endif
        }

        public void BoastBigWin(long betCredit, long earnCredit, Action successCallback, Action errorCallback)
        {
#if NEW_NET
            Debug.Log("【remove rpc】: /v0/room/boast/bigwin");
            if (successCallback != null)
                successCallback();
            return;
#endif
            var roomId = BlackboardUtils.FindVariable<string>("./room/roomId");

            BagelCodeClientAPI.RoomBigWin(betCredit, earnCredit, roomId.value, null, null);

            if (successCallback != null)
                successCallback();
            // (response) =>
            // {
            //     if (successCallback != null)
            //         successCallback();
            // },
            // (error) =>
            // {
            //     CommonError(error);

            //     if (errorCallback != null)
            //         errorCallback();
            // });
        }

        public void SlotEndTurn(Action successCallback, Action errorCallback)
        {
#if UNITY_EDITOR
            Debug.Log("@ /v3/report/turn_end");
#endif

#if NEW_NET

            if (successCallback != null)
            {
                successCallback();
            }
            return;
#endif
            uint blockseq = BlackboardUtils.FindVariable<uint>("./turn/spin/response/common/blockseq").value;
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            // todo : epic pass always event id
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

            BagelCodeClientAPI.SlotEndTurn(blockseq, metaGameEventID, seasonPassEventId,
            (response) =>
            {
                EpicPassUtilsV2.UpdateSeasonPassPointInfoForBB(response.seasonPassUpdateInfo);
                BlackboardQueryUtils.UpdateMetaGameInfoEndTurn(response.metaGameInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                // CommonError(error);

                // if (errorCallback != null)
                //     errorCallback();
            });
        }

        public void VideoPokerDeal(long betCredit, int handCount, object customData, Action successCallback, Action errorCallback)
        {
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            int gameId = BlackboardUtils.GetOrCreateVariable<int>("./game/gameId").value;
            int collectingGameChestDropRateMultiplyEventId = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "collectingGameChestDropRateMultiplyEventId").value;

            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGameDeal = (spinType == SpinType.GameSpin);
            bool isBonusDeal = (spinType == SpinType.BonusSpin);

            List<int> expEventIdList = BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY);
            expEventIdList.AddRange(BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY_EXTENDABLE));

            bool isHighRollerBet = BlackboardUtils.FindVariable<bool>("/isExtendedBetIndex")?.value ?? false;

            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

#if NEW_NET
            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"bet_per_hand",betCredit},
                { "hand_count",handCount},
            };

            NetManager.Instance.Post(RPCName.jacksDeal, req,
            (res) =>
            {
                string resStr = res.ToString();

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/video_poker_deal_response_v3");
                ClientModels.VideoPokerDealResponseV3 response = JsonUtility.FromJson<ClientModels.VideoPokerDealResponseV3>(jsn8.text);
                response.contents = res["contents"].ToString();

                VideoPokerDealSuccess(response, betCredit);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
            return;
#endif

            BagelCodeClientAPI.VideoPokerDeal(betCredit, handCount, isGameDeal, isBonusDeal, metaGameEventID, gameId, collectingGameChestDropRateMultiplyEventId, expEventIdList, isHighRollerBet, customData, seasonPassEventId,
            (response) =>
            {
                // "/vd3/video_poker/deal"
                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A VideoPokerDealResponseV3 = {oldJson}");

                VideoPokerDealSuccess(response, betCredit);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        private void VideoPokerDealSuccess(ClientModels.VideoPokerDealResponseV3 response, long betCredit)
        {
            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            if (spinBB == null || spinBB.value == null) return;

            var bb = BlackboardUtils.GetOrCreateBlackboard(spinBB.value, "response");
            ClientAPI2Blackboard.Serialize(bb, response);
            BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.VideoPokerDeal);
            ContentsSerializer.Deserialize(bb);

            BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
            BlackboardQueryUtils.UpdatePotOfGold(response.userSyncInfo.piggyCredit);
            BlackboardQueryUtils.UpdateUnlockFeature(response.featureUnlockList);
            BlackboardQueryUtils.UpdateMetaGameInfo(response.metaGameInfo, response.serverTime);
            BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betCredit, response.gameDealCount, "./gameSpinCountPerBet");
            BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betCredit, response.bonusDealCount, "./bonusSpinCountPerBet");
            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.UpdateHiddenUniverseFinder(response.earnFinder);
            BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeCompositeInfo?.vipLoungeInfo ?? null);
            BlackboardQueryUtils.UpdateVegasDreamsTotalDepotCount(response.vipLoungeCompositeInfo?.buildDreamInfo ?? null);
            BlackboardQueryUtils.UpdateMysteryGiftInfo(response.nextMysteryGiftLevel, response.mysteryGiftInfo, response.serverTime);
            LevelUpDash.LevelUpDash.Utils.UpdateLevelUpDashInfo(response.levelUpDashInfo);
        }

        public void VideoPokerEndDeal()
        {
#if NEW_NET
            return;
#endif
            //埋点数据：
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;

            uint blockseq = BlackboardUtils.FindVariable<uint>(null, "./turn/spin/response/common/blockseq").value;
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;
            BagelCodeClientAPI.VideoPokerEndDeal(blockseq, metaGameEventID, seasonPassEventId,
            (response) =>
            {
                BlackboardQueryUtils.UpdateMetaGameInfoEndTurn(response.metaGameInfo, response.serverTime);
                EpicPassUtilsV2.UpdateSeasonPassPointInfoForBB(response.seasonPassUpdateInfo);
            },
            (error) =>
            {
            });
        }

        public void VideoPokerDraw(List<bool> helds, object customData, Action successCallback, Action errorCallback)
        {
            int gameId = BlackboardUtils.GetOrCreateVariable<int>("./game/gameId").value;

            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGameDeal = (spinType == SpinType.GameSpin);
            bool isBonusDeal = (spinType == SpinType.BonusSpin);
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

#if NEW_NET
            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"helds",helds },
            };

            NetManager.Instance.Post(RPCName.jacksDraw, req,
            (res) =>
            {
                string resStr = res.ToString();

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/video_poker_draw_response_v2");
                ClientModels.VideoPokerDrawResponseV2 response = JsonUtility.FromJson<ClientModels.VideoPokerDrawResponseV2>(jsn8.text);
                response.contents = res["contents"].ToString();

                var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("spin"), "response");
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.VideoPokerDraw);
                ContentsSerializer.Deserialize(bb);

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
            return;
#endif

            BagelCodeClientAPI.VideoPokerDraw(helds, gameId, isBonusDeal, isGameDeal, metaGameEventID, customData, seasonPassEventId,
            (response) =>
            {
                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A VideoPokerDrawResponseV2 = {oldJson}");

                var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("spin"), "response");
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.VideoPokerDraw);
                ContentsSerializer.Deserialize(bb);

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        public void VideoPokerClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback)
        {
            int gameID = BlackboardUtils.GetOrCreateVariable<int>("./game/gameId").value;
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGameDeal = (spinType == SpinType.GameSpin);
            bool isBonusDeal = (spinType == SpinType.BonusSpin);
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

            BagelCodeClientAPI.VideoPokerClaimBonus(gameID, isBonusDeal, isGameDeal, metaGameEventID, claimId, customData, seasonPassEventId,
            (response) =>
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("turn"), "claims");
                bb = BlackboardUtils.GetOrCreateBlackboard(bb, claimId);
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.VideoPokerClaimBonus);
                ContentsSerializer.Deserialize(bb);

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        public void ClaimTicketedBonus(int ticketId, Action successCallback, Action errorCallback)
        {
            BagelCodeClientAPI.TicketedBonusClaim(ticketId,
            (response) =>
            {
                var cb = ContentBlackboard.Get();
                var bb = BlackboardUtils.GetOrCreateBlackboard(cb, "ticketClaim");

                ClientAPI2Blackboard.Serialize(bb, response);

                var bonusCredit = BlackboardUtils.FindVariable<long>("./bonus/response/earnCredit");
                // var claimCredit = bb.GetValue<long>("earnCredit");
                bonusCredit.value += response.earnCredit;

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        public void KenoPlay(long betPerTicket, long extraBetPerTicket, int ticketCount, List<List<int>> pickInfoList, object customData, Action successCallback, Action errorCallback)
        {
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            int gameId = BlackboardUtils.GetOrCreateVariable<int>("./game/gameId").value;
            int collectingGameChestDropRateMultiplyEventId = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "collectingGameChestDropRateMultiplyEventId").value;
            bool isAutoPick = BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "isAutoQuickPick").value;

            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGamePlay = (spinType == SpinType.GameSpin);
            bool isBonusPlay = (spinType == SpinType.BonusSpin);

            List<int> expEventIdList = BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY);
            expEventIdList.AddRange(BlackboardQueryUtils.GetEventIdList(EventInfoType.EXP_MULTIPLY_EXTENDABLE));

            bool isHighRollerBet = BlackboardUtils.FindVariable<bool>("/isExtendedBetIndex")?.value ?? false;

            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

            /*
            object a = new
            {
                gameId = gameId,
                betPerTicket = betPerTicket,
                extraBetPerTicket = extraBetPerTicket,
                ticketCount = ticketCount,
                pickInfoList = pickInfoList,
                customData = customData,
                contentsVersion = ContentsVersionManager.Instance.GetContentsVersion(),
            };

            string stra = JsonUtility.ToJson(a);//测试代码

            string contents = ContentsSerializer.SerializeKenoPlay(gameId, betPerTicket, extraBetPerTicket, ticketCount, pickInfoList, customData);
            Debug.Log($"@A ReqKenoSpin = {contents}"); //测试代码*/

#if NEW_NET

            //object req = null;
            //switch ((GameID)gameId)
            //{
            //    case GameID.KenoClassic:
            //        req = new RPCKenoClassic.ReqKenoSpin();
            //        break;
            //}

            //JSONNode data = JSONNode.Parse("{\"bet_per_ticket\":1000,\"pick_info_list\":[[25,41,52,55,56,63,68,71,77,80]]}");

            // string debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + "}";
            string debug_param = "{\"is_free_spin\":" + TestManager.Instance.getCode() + "}";

            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"bet_per_ticket",betPerTicket },
                { "pick_info_list",pickInfoList},
                { "debug_param", debug_param}
            };

            Debug.Log("@ KenoPlay is_free_spin : " + debug_param);

            NetManager.Instance.Post(RPCName.kenoSpin, req,
            (res) =>
            {
                string resStr = res.ToString();

                //string oldJson = JsonUtility.ToJson(response);
                //Debug.Log($"@A KenoPlayResponseV1 = {oldJson}");

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/keno_play_response_v1");
                ClientModels.KenoPlayResponseV1 response = JsonUtility.FromJson<ClientModels.KenoPlayResponseV1>(jsn8.text);

                response.contents = res["contents"].ToString();

                Debug.Log($" @contents =  {response.contents}");

                var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
                if (spinBB == null || spinBB.value == null) return;

                var bb = BlackboardUtils.GetOrCreateBlackboard(spinBB.value, "response");
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.KenoPlay);
                ContentsSerializer.Deserialize(bb);

                BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
                BlackboardQueryUtils.UpdatePotOfGold(response.userSyncInfo.piggyCredit);
                BlackboardQueryUtils.UpdateUnlockFeature(response.featureUnlockList);
                BlackboardQueryUtils.UpdateMetaGameInfo(response.metaGameInfo, response.serverTime);
                BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betPerTicket, response.gamePlayCount, "./gameSpinCountPerBet");
                BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betPerTicket, response.bonusPlayCount, "./bonusSpinCountPerBet");
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.UpdateHiddenUniverseFinder(response.earnFinder);
                BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeCompositeInfo?.vipLoungeInfo ?? null);
                BlackboardQueryUtils.UpdateVegasDreamsTotalDepotCount(response.vipLoungeCompositeInfo?.buildDreamInfo ?? null);
                BlackboardQueryUtils.UpdateMysteryGiftInfo(response.nextMysteryGiftLevel, response.mysteryGiftInfo, response.serverTime);
                LevelUpDash.LevelUpDash.Utils.UpdateLevelUpDashInfo(response.levelUpDashInfo);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            }
            );

            return;
#endif

            BagelCodeClientAPI.KenoPlay(betPerTicket, extraBetPerTicket, ticketCount, isAutoPick, pickInfoList, isGamePlay, isBonusPlay, metaGameEventID, gameId, collectingGameChestDropRateMultiplyEventId, expEventIdList, isHighRollerBet,
                customData, seasonPassEventId,
            (response) =>
            {
                KenoPlaySuccess(response, betPerTicket);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        private void KenoPlaySuccess(ClientModels.KenoPlayResponseV1 response, long betPerTicket)
        {
            string oldJson = JsonUtility.ToJson(response);
            Debug.Log($"@A KenoPlayResponseV1 = {oldJson}");

            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            if (spinBB == null || spinBB.value == null) return;

            var bb = BlackboardUtils.GetOrCreateBlackboard(spinBB.value, "response");
            ClientAPI2Blackboard.Serialize(bb, response);
            BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.KenoPlay);
            ContentsSerializer.Deserialize(bb);

            BlackboardQueryUtils.UpdateTournament(response.tournamentInfo);
            BlackboardQueryUtils.UpdatePotOfGold(response.userSyncInfo.piggyCredit);
            BlackboardQueryUtils.UpdateUnlockFeature(response.featureUnlockList);
            BlackboardQueryUtils.UpdateMetaGameInfo(response.metaGameInfo, response.serverTime);
            BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betPerTicket, response.gamePlayCount, "./gameSpinCountPerBet");
            BlackboardQueryUtils.UpdateBonusSpinCountPerBet(betPerTicket, response.bonusPlayCount, "./bonusSpinCountPerBet");
            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.UpdateHiddenUniverseFinder(response.earnFinder);
            BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeCompositeInfo?.vipLoungeInfo ?? null);
            BlackboardQueryUtils.UpdateVegasDreamsTotalDepotCount(response.vipLoungeCompositeInfo?.buildDreamInfo ?? null);
            BlackboardQueryUtils.UpdateMysteryGiftInfo(response.nextMysteryGiftLevel, response.mysteryGiftInfo, response.serverTime);
            LevelUpDash.LevelUpDash.Utils.UpdateLevelUpDashInfo(response.levelUpDashInfo);
        }

        public void KenoClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback)
        {
#if NEW_NET
            if (successCallback != null)
                successCallback();
            return;
#endif

            int gameID = BlackboardUtils.GetOrCreateVariable<int>("./game/gameId").value;
            int metaGameEventID = BlackboardUtils.GetOrCreateVariable<int>(MainBlackboard.Get(), "metaGameEventID").value;
            SpinType spinType = BlackboardUtils.GetOrCreateVariable<SpinType>(ContentBlackboard.Get(), "spinType").value;
            bool isGamePlay = (spinType == SpinType.GameSpin);
            bool isBonusPlay = (spinType == SpinType.BonusSpin);
            int seasonPassEventId = EpicPassUtilsV2.SeasonPassEventId;

            BagelCodeClientAPI.KenoClaimBonus(gameID, isBonusPlay, isGamePlay, metaGameEventID, claimId, customData, seasonPassEventId,
            (response) =>
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get().GetValue<Blackboard>("turn"), "claims");
                bb = BlackboardUtils.GetOrCreateBlackboard(bb, claimId);
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.KenoClaimBonus);
                ContentsSerializer.Deserialize(bb);

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        public void GambleStart(string ticketId, Action successCallback, Action errorCallback)
        {
#if NEW_NET

            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"blockseq",BagelCodeHTTP.FetchBlockSeq() },
                {"ackMask", BagelCodeHTTP.GenerateAckBits()},
                {"contents",ContentsSerializer.SerializeGambleStart(ticketId)},
            };
            NetManager.Instance.Post(RPCName.jacksGambleStart, req,
            (res) =>
            {
                string resStr = res.ToString();

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/gamble_start_response_v2");
                ClientModels.GambleStartResponseV2 response = JsonUtility.FromJson<ClientModels.GambleStartResponseV2>(jsn8.text);

                response.contents = res["contents"].ToString();

                var gamble = BlackboardUtils.FindVariable<Blackboard>("./turn/gamble");
                if (gamble != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(gamble.value, "response");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.GambleStart);
                    ContentsSerializer.Deserialize(bb);
                }

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
            return;
#endif

            BagelCodeClientAPI.GambleStart(ticketId,
            (response) =>
            {
                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A GambleStartResponseV2 = {oldJson}");

                var gamble = BlackboardUtils.FindVariable<Blackboard>("./turn/gamble");
                if (gamble != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(gamble.value, "response");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.GambleStart);
                    ContentsSerializer.Deserialize(bb);
                }

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        public void GambleDeal(object customData, Action successCallback, Action errorCallback)
        {
#if NEW_NET

            /*GambleDealRequestV2 request = new GambleDealRequestV2
            {
                blockseq = BagelCodeHTTP.FetchBlockSeq(),
                ackMask = BagelCodeHTTP.GenerateAckBits(),
                contents = ContentsSerializer.SerializeGambleDeal(customData)
            };*/

            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"blockseq",BagelCodeHTTP.FetchBlockSeq()},
                {"ackMask",BagelCodeHTTP.GenerateAckBits()},
                {"contents",ContentsSerializer.SerializeGambleDeal(customData) }
            };

            NetManager.Instance.Post(RPCName.jacksGambleDeal, req,
            (res) =>
            {
                string resStr = res.ToString();

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/gamble_deal_response_v2");
                ClientModels.GambleDealResponseV2 response = JsonUtility.FromJson<ClientModels.GambleDealResponseV2>(jsn8.text);

                response.contents = res["contents"].ToString();

                var gamble = BlackboardUtils.FindVariable<Blackboard>("./turn/gamble");
                if (gamble != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(gamble.value, "response");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.GambleDeal);
                    ContentsSerializer.Deserialize(bb);
                }

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
            return;
#endif

            BagelCodeClientAPI.GambleDeal(customData,
            (response) =>
            {
                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A GambleDealResponseV2 = {oldJson}");

                var gamble = BlackboardUtils.FindVariable<Blackboard>("./turn/gamble");
                if (gamble != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(gamble.value, "response");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.GambleDeal);
                    ContentsSerializer.Deserialize(bb);
                }

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        public void GambleTake(Action successCallback, Action errorCallback)
        {
#if NEW_NET

            Dictionary<string, object> req = new Dictionary<string, object>
            {
                {"blockseq",BagelCodeHTTP.FetchBlockSeq()},
                {"ackMask",BagelCodeHTTP.GenerateAckBits()},
                {"contents",ContentsSerializer.SerializeGambleTake() }
            };

            NetManager.Instance.Post(RPCName.jacksGambleTake, req,
            (res) =>
            {
                string resStr = res.ToString();

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/gamble_take_response_v2");
                ClientModels.GambleTakeResponseV2 response = JsonUtility.FromJson<ClientModels.GambleTakeResponseV2>(jsn8.text);

                response.contents = res["contents"].ToString();

                if (res.HasKey("userSyncInfo"))
                {
                    string str1 = res["userSyncInfo"].ToString();
                    response.userSyncInfo = JsonUtility.FromJson<UserSyncInfo>(str1);
                }

                var gamble = BlackboardUtils.FindVariable<Blackboard>("./turn/gamble");
                if (gamble != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(gamble.value, "response");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.GambleTake);
                    ContentsSerializer.Deserialize(bb);
                }

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.UpdatePotOfGold(response.userSyncInfo.piggyCredit);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
            return;
#endif

            BagelCodeClientAPI.GambleTake(
            (response) =>
            {
                string oldJson = JsonUtility.ToJson(response);
                Debug.Log($"@A GambleTakeResponseV2 = {oldJson}");

                var gamble = BlackboardUtils.FindVariable<Blackboard>("./turn/gamble");
                if (gamble != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(gamble.value, "response");
                    ClientAPI2Blackboard.Serialize(bb, response);
                    BlackboardUtils.SetOrCreateValue<int>(bb, "requestType", (int)ContentsRequestType.GambleTake);
                    ContentsSerializer.Deserialize(bb);
                }

                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.UpdatePotOfGold(response.userSyncInfo.piggyCredit);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);

                if (errorCallback != null)
                    errorCallback();
            });
        }

        private void CommonError(BagelCodeHTTPError error)
        {
            Debug.LogWarning("V3MetaSystem.CommonError invoked!!");
            Debug.LogError(SlotSimpleJson.SerializeObject(error));

            switch (error.errorCode)
            {
                case ClientModels.Error.NOT_IN_ROOM_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();
                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_NOT_EXIST_ROOM", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                        info.callback1 = delegate
                        {
                            MessageDispatcher.Dispatch("OnContentEvent", new EventData("LeaveGame"));
                        };

                        ErrorPopupHandler.Instance.OpenError(info);
                    }
                    break;

                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        }

        public void ApplyContentsStore(Action successCallback, Action errorCallback)
        {
            BagelCodeClientAPI.RequestContentsStore(
            (response) =>
            {
                var gameBB = BlackboardUtils.GetOrCreateBlackboard(ContentBlackboard.Get(), "game");
                var bb = BlackboardUtils.GetOrCreateBlackboard(gameBB, "contentsStoreInfo");
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardUtils.SetOrCreateValue<ContentsRequestType>(bb, "requestType", ContentsRequestType.ContentsStore);
                ContentsSerializer.Deserialize(bb);

                if (successCallback != null)
                    successCallback();
            },
            (error) =>
            {
                CommonError(error);
                if (errorCallback != null)
                    errorCallback();
            });
        }

        public bool IsIgnoredUser(string userId, int reportCount)
        {
            return BlackboardQueryUtils.IsIgnoredUser(userId, reportCount);
        }

        public void SpentCredit(long spentCredit)
        {
            BlackboardQueryUtils.SpentCredit(spentCredit);
        }

        public void BackupUserSyncInfo()
        {
            BlackboardQueryUtils.BackUpMe();
        }

        public void ApplyUserSyncInfo(bool isApply)
        {
            BlackboardQueryUtils.ApplyUserSyncInfo(isApply);
        }

        public long GetTimeStamp()
        {
            return TimeUtils.GetTimeStamp();
        }

        public long GetLocalTimeStamp()
        {
            return TimeUtils.GetCurrentLocalTime();
        }

        public long GetSessionTimeStamp()
        {
            return TimeUtils.GetSessionPlayTime();
        }

        public long GetTotalSessionTimeStamp()
        {
            return TimeUtils.GetTotalSessionPlayTime();
        }

        public DateTime TimeStampToUTCDateTime(long timestamp)
        {
            return TimeUtils.ParseTimestampToDateTime(timestamp);
        }

        public DateTime TimeStampToLocalDateTime(long timestamp)
        {
            return TimeUtils.ParseTimestampToLocalDateTime(timestamp);
        }

        public int GetTierGroup(int tier)
        {
            return TierUtils.GetTierGroup(tier);
        }

        public void SubscribeBackButton(int id, Action callback)
        {
            if (BackButtonManager.Instance != null)
                BackButtonManager.Instance.SubscribeBackButton(callback, id);
        }

        public void UnSubscribeBackButton(int id)
        {
            if (BackButtonManager.Instance != null)
                BackButtonManager.Instance.UnSubscribeBackButton(id);
        }
    }
}
