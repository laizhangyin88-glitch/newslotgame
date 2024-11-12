using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
	public interface IMetaSystem
	{
		void SelectGame(int gameId);
		void EnterGame();
		void ClearContentData();

		void SlotSpin(long betCredit, long extraBetCredit, object customData, Action successCallback, Action errorCallback);
		void SlotClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback);
		void BoastBigWin(long betCredit, long earnCredit, Action successCallback, Action errorCallback);
		void SlotEndTurn(Action successCallback, Action errorCallback);
		void VideoPokerDeal(long betCredit, int handCount, object customData, Action successCallback, Action errorCallback);
		void VideoPokerEndDeal();
		void VideoPokerDraw(List<bool> helds, object customData, Action successCallback, Action errorCallback);
		void VideoPokerClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback);
		void KenoPlay(long betPerTicket, long extraBetPerTicket, int ticketCount, List<List<int>> pickInfoList, object customData, Action successCallback, Action errorCallback);
		void KenoClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback);
		void ClaimTicketedBonus(int ticketId, Action successCallback, Action errorCallback);
		void GambleStart(string ticketId, Action successCallback, Action errorCallback);
		void GambleDeal(object customData, Action successCallback, Action errorCallback);
		void GambleTake(Action successCallback, Action errorCallback);
		void ApplyContentsStore(Action successCallback, Action errorCallback);

		bool IsIgnoredUser(string userId, int reportCount);
		void SpentCredit(long spentCredit);
		void BackupUserSyncInfo();
		void ApplyUserSyncInfo(bool isApply);
		
		long GetSessionTimeStamp();
		long GetTotalSessionTimeStamp();
		DateTime TimeStampToUTCDateTime(long timestamp);
		DateTime TimeStampToLocalDateTime(long timestamp);

		int GetTierGroup(int tier);
		
		void SubscribeBackButton(int id, Action callback);
		void UnSubscribeBackButton(int id);
	}
}