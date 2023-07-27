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

            BagelCodeClientAPI.SlotSpin(betCredit, extraBetCredit, roomID, isGameSpin, isBonusSpin, metaGameEventID, gameId, collectingGameChestDropRateMultiplyEventId, customData, isAutoSpin, expEventIdList, isHighRollerBet, seasonPassEventId,
            (response) =>
        	{
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

		void SlotSpinSuccess(ClientModels.SlotSpinResponseV3 response, long betCredit, long extraBetCredit, SpinType spinType)
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

            BagelCodeClientAPI.VideoPokerDeal(betCredit, handCount, isGameDeal, isBonusDeal, metaGameEventID, gameId, collectingGameChestDropRateMultiplyEventId, expEventIdList, isHighRollerBet, customData, seasonPassEventId,
            (response) =>
            {
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

            BagelCodeClientAPI.VideoPokerDraw(helds, gameId, isBonusDeal, isGameDeal, metaGameEventID, customData, seasonPassEventId,
            (response) =>
            {
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
            BagelCodeClientAPI.GambleStart(ticketId,
            (response) =>
            {
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
            BagelCodeClientAPI.GambleDeal(customData,
            (response) =>
            {
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
            BagelCodeClientAPI.GambleTake(
            (response) =>
            {
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
