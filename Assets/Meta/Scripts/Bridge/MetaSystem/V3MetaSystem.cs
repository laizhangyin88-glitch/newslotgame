#define NEW_NET
using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;
using SlotMaker;
using SlotMaker.Json;
using SlotMaker.TestSuite;
using SlotMaker.Contents;
using ParadoxNotion;
using NodeCanvas.Framework;
using Action = System.Action;
using System.Linq;
using SimpleJSON;
using SlotMaker.Cards;
using SlotMaker.Slots.Tasks.Actions.Game;
using BagelCode.Internal;
using BagelCode.Protobuf;
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


            //免费游戏
            if (BlackboardUtils.GetOrCreateVariable<bool>(ContentBlackboard.Get(), "islastFreeSpin").value)
            {
                BlackboardUtils.SetOrCreateValue(ContentBlackboard.Get(), "islastFreeSpin", false);

                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                response.contents = BlackboardUtils.GetOrCreateVariable<string>(ContentBlackboard.Get(), "lastFreeSpinContent").value;

                SlotSpinSuccess(response, betCredit, extraBetCredit, spinType);

                if (successCallback != null)
                    successCallback();

                return;
            }


            string debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + "}";

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


            Debug.Log("@ SlotSpin is_free_spin : " + debug_param);
      
            NetManager.Instance.Post(RPCName.slotSpin, req,
            (res) =>
            {

                string resStr = res.ToString();
                TextAsset jsn8 = Resources.Load<TextAsset>("tempdata/slot_spin_response_v3");
                ClientModels.SlotSpinResponseV3 response = JsonUtility.FromJson<ClientModels.SlotSpinResponseV3>(jsn8.text);
                response.contents = res["contents"].ToString();

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

		public void SlotSpinSuccess(ClientModels.SlotSpinResponseV3 response, long betCredit, long extraBetCredit, SpinType spinType)
		{
            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            if(spinBB == null || spinBB.value == null) return;

            var bb = BlackboardUtils.GetOrCreateBlackboard(spinBB.value, "response");
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
            if (successCallback != null)
                successCallback();
            return;
#endif

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

        void VideoPokerDealSuccess(ClientModels.VideoPokerDealResponseV3 response, long betCredit)
        {
            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            if(spinBB == null || spinBB.value == null) return;

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

            string debug_param = "{\"is_free_spin\":" + globalStore.test_is_free_spin + "}";

            Dictionary<string,object> req = new Dictionary<string, object>
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



        void KenoPlaySuccess(ClientModels.KenoPlayResponseV1 response, long betPerTicket)
        {


            string oldJson = JsonUtility.ToJson(response);
            Debug.Log($"@A KenoPlayResponseV1 = {oldJson}");


            var spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            if(spinBB == null || spinBB.value == null) return;

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
                if(gamble != null)
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
                if(gamble != null)
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
                if(gamble != null)
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

        void CommonError(BagelCodeHTTPError error)
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
            if(BackButtonManager.Instance != null)
                BackButtonManager.Instance.SubscribeBackButton(callback, id);
        }

        public void UnSubscribeBackButton(int id)
        {
            if(BackButtonManager.Instance != null)
                BackButtonManager.Instance.UnSubscribeBackButton(id);
        }
	}
}
