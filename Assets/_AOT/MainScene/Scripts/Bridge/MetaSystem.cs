using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SlotMaker
{
	public class MetaSystem : MonoSingleton<MetaSystem> 
	{
		IMetaSystem metaSystem;

		public BlendMode mode;

		public static void InitializeMetaSystem(IMetaSystem metaSystem)
		{
			Instance.metaSystem = metaSystem;
		}

		public static void SelectGame(int gameId)
		{
			Instance.metaSystem.SelectGame(gameId);
		}

		public static void EnterGame()
		{
			Instance.metaSystem.EnterGame();
		}

		public static void ClearContentData()
		{
			Instance.metaSystem.ClearContentData();
		}

		public static void SlotSpin(long betCredit, long extraBetCredit, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.SlotSpin(betCredit, extraBetCredit, customData, successCallback, errorCallback);
		}

		public static void SlotClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.SlotClaimBonus(claimId, customData, successCallback, errorCallback);
		}

		public static void BoastBigWin(long betCredit, long earnCredit, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.BoastBigWin(betCredit, earnCredit, successCallback, errorCallback);
		}

		public static void SlotEndTurn(Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.SlotEndTurn(successCallback, errorCallback);
		}

		public static void VideoPokerDeal(long betCredit, int handCount, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.VideoPokerDeal(betCredit, handCount, customData, successCallback, errorCallback);
		}

		public static void VideoPokerEndDeal()
		{
			Instance.metaSystem.VideoPokerEndDeal();
		}

		public static void VideoPokerDraw(List<bool> helds, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.VideoPokerDraw(helds, customData, successCallback, errorCallback);
		}

		public static void VideoPokerClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.VideoPokerClaimBonus(claimId, customData, successCallback, errorCallback);
		}

		public static void KenoPlay(long betCredit, long extraBetCredit, int ticketCount, List<List<int>> pickInfoList, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.KenoPlay(betCredit, extraBetCredit, ticketCount, pickInfoList, customData, successCallback, errorCallback);
		}

		public static void KenoClaimBonus(string claimId, object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.KenoClaimBonus(claimId, customData, successCallback, errorCallback);
		}

		public static void ClaimTicketedBonus(int ticketId, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.ClaimTicketedBonus(ticketId, successCallback, errorCallback);
		}

		public static void GambleStart(string ticketId, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.GambleStart(ticketId, successCallback, errorCallback);
		}

		public static void GambleDeal(object customData, Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.GambleDeal(customData, successCallback, errorCallback);
		}

		public static void GambleTake(Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.GambleTake(successCallback, errorCallback);
		}

		public static void ApplyContentsStore(Action successCallback, Action errorCallback)
		{
			Instance.metaSystem.ApplyContentsStore(successCallback, errorCallback);
		}

		public static bool IsIgnoredUser(string userId, int reportCount)
		{
			return Instance.metaSystem.IsIgnoredUser(userId, reportCount);
		}

		public static void SpentCredit(long spentCredit)
		{
			Instance.metaSystem.SpentCredit(spentCredit);
		}

		public static void BackupUserSyncInfo()
		{
			Instance.metaSystem.BackupUserSyncInfo();
		}

		public static void ApplyUserSyncInfo(bool isApply)
		{
			Instance.metaSystem.ApplyUserSyncInfo(isApply);
		}

		static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long GetTimeStamp()
		{
			return (long)((System.DateTime.UtcNow - Jan1St1970).TotalMilliseconds);
		}

		public static long GetLocalTimeStamp()
		{
			return (long)((System.DateTime.Now - Jan1St1970).TotalMilliseconds);
		}

		public static int GetTimeZoneOffset()
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
		}

		public static long GetSessionTimeStamp()
		{
			return Instance.metaSystem.GetSessionTimeStamp();
		}

		public static long GetTotalSessionTimeStamp()
		{
			return Instance.metaSystem.GetTotalSessionTimeStamp();
		}

		public static DateTime TimeStampToUTCDateTime(long timestamp)
		{
			return Instance.metaSystem.TimeStampToUTCDateTime(timestamp);
		}

		public static DateTime TimeStampToLocalDateTime(long timestamp)
		{
			return Instance.metaSystem.TimeStampToLocalDateTime(timestamp);
		}

		public static int GetTierGroup(int tier)
		{
			return Instance.metaSystem.GetTierGroup(tier);
		}

		public static void SubscribeBackButton(int id, Action callback)
		{
			Instance?.metaSystem.SubscribeBackButton(id, callback);
		}

		public static void UnSubscribeBackButton(int id)
		{
			Instance?.metaSystem.UnSubscribeBackButton(id);
		}
	}
}
