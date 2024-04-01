using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using UnityEngine.CrashReportHandler;
using System.Linq;
using UnityEngine;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        // Key(gametitle + big or small)/ Value(url)
        private static Dictionary<string, string> slotWebImageURL = new Dictionary<string, string>();

        public static void ClearGameAndSlotInfos()
        {
            slotWebImageURL.Clear();
        }

        public static List<long> GetJackpotMinBetList()
        {
            return BlackboardUtils.FindVariable<List<long>>(ContentBlackboard.Get(), "game/jackpotInfo/eligibleMinBetPerJackpot")?.value;
        }

        public static void UpdateGameAndSlotInfoFromLobby()
        {
            // from lobby
            // Update GameInfos
            var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "gameInfoList").value;
            if (gameInfoList == null) return;

            for (int i = 0; i < gameInfoList.Count; ++i)
            {
                string gameTitle = gameInfoList[i].GetValue<string>("gameTitle");

                string longImageAssetName = StringTableUtils.GetString(StringTable.StringTableType.Global, MetaIconUtils.SLOT_THUMBNAIL_BIG, gameTitle);
                slotWebImageURL[longImageAssetName] = gameInfoList[i].GetValue<string>("longImageUrl");

                string smallImageAssetName = StringTableUtils.GetString(StringTable.StringTableType.Global, MetaIconUtils.SLOT_THUMBNAIL_SMALL, gameTitle);
                slotWebImageURL[smallImageAssetName] = gameInfoList[i].GetValue<string>("shortImageUrl");
            }
        }

        public static void UpdateGameAndSlotInfoFromLobby01(Dictionary<string, string> _slotWebImageURL)
        {
            //BlackboardQueryUtils.ClearGameAndSlotInfos();
            slotWebImageURL = _slotWebImageURL;
        }

        public static string GetGameWebImageURL(string assetName)
        {
            if(slotWebImageURL.ContainsKey(assetName))
            {
#if DEV
                if (ApplicationSettings.LogTest())
                    Debug.Log("Exists asset url : " + assetName);
#endif
                return slotWebImageURL[assetName];
            }
            return "";
        }

        public static Blackboard GetEnterGameInfo()
        {
            return BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), "enterGameInfo")?.value;
        }

        public static void SetEnterGameInfo(
                                int gameID,
                                string enterType,
                                string fromType,
                                string targetRoomId = null,
                                int fromNoticeID = 0,
                                string fromSlbID = null,
                                bool isEarlyAccess = false,
                                int bonusTicketId = 0,
                                string contextID = null
        )
        {
            BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "enterGameInfo");

            var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "enterGameInfo");

            BlackboardUtils.SetOrCreateValue(bb, "gameId", gameID);
            BlackboardUtils.SetOrCreateValue(bb, "betZoneId", "free");

            if (targetRoomId == null)
                BlackboardUtils.SetOrCreateValue(bb, "targetRoomId", "");
            else
                BlackboardUtils.SetOrCreateValue(bb, "targetRoomId", targetRoomId);

            if (fromNoticeID > 0)
                BlackboardUtils.SetOrCreateValue(bb, "noticeId", fromNoticeID);
            else
                BlackboardUtils.SetOrCreateValue(bb, "noticeId", 0);

            if (string.IsNullOrEmpty(fromSlbID))
                BlackboardUtils.SetOrCreateValue(bb, "slbId", "");
            else
                BlackboardUtils.SetOrCreateValue(bb, "slbId", fromSlbID);

            if (bonusTicketId > 0)
                BlackboardUtils.SetOrCreateValue(bb, "ticketId", bonusTicketId);
            else
                BlackboardUtils.SetOrCreateValue(bb, "ticketId", 0);

            if (string.IsNullOrEmpty(contextID))
                contextID = BiEventUtils.GenerateContextID();

            var gameTitle = GetGameTitle(gameID);
            CrashReportHandler.SetUserMetadata("GS.gameTitle", gameTitle);

            BlackboardUtils.SetOrCreateValue(bb, "enterType", enterType);
            BlackboardUtils.SetOrCreateValue(bb, "fromType", fromType);
            BlackboardUtils.SetOrCreateValue(bb, "isEarlyAccess", isEarlyAccess);
            BlackboardUtils.SetOrCreateValue(bb, "gameType", GetGameType(gameID));
            BlackboardUtils.SetOrCreateValue(bb, "gameTitle", gameTitle);
            BlackboardUtils.SetOrCreateValue(bb, "orientation", GetGameOrientation(gameID));
            BlackboardUtils.SetOrCreateValue(bb, "contextID", contextID);
        }

        public static bool IsIngame()
        {
            return MainBlackboard.Get().GetVariable<bool>("inGame")?.value ?? false;
        }

        public static bool IsSpin()
        {
            return MainBlackboard.Get().GetVariable<bool>("isSpin")?.value ?? false;
        }

        public static bool IsAutoSpin()
        {
            if (!IsIngame()) return false;
            return ContentBlackboard.Get().GetVariable<bool>("autoSpin")?.value ?? false;
        }

        public static Blackboard GetSlotInfoBB(int gameId, out bool isEarlyAccess)
        {
            var slotInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId);
            isEarlyAccess = slotInfo != null;

            if (!isEarlyAccess) slotInfo = GetSlotInfoBB(gameId);

            return slotInfo;
        }

        public static Blackboard GetSlotInfoBB(int _gameId)
        {
            var slotList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "slotList");

            for (int i = 0; i < slotList.value.Count; ++i)
            {
                var gameId = BlackboardUtils.FindVariable<int>(slotList.value[i], "gameId");
                if (gameId.value == _gameId)
                {
                    return slotList.value[i];
                }
            }

            return null;
        }

        public static Blackboard GetGameInfo(int gameId)
        {
            var gameInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "gameInfoList").value;
            for (int i = 0; i < gameInfoList.Count; ++i)
            {
                Blackboard gameInfo = gameInfoList[i];
                if (gameInfo.GetValue<int>("gameId") == gameId)
                {
                    return gameInfo;
                }
            }
            return null;
        }

        public static void UpdateUnlockGameStatus(GameInfo gameInfo, Slot slotInfo)
        {
            var gameInfoBB = GetGameInfo(gameInfo.gameId);
            if (gameInfoBB != null)
                ClientAPI2Blackboard.Serialize(gameInfoBB, gameInfo);

            var slotInfoBB = GetSlotInfoBB(slotInfo.gameId);
            if (slotInfoBB != null)
            {
                var flagsBB = slotInfoBB.GetValue<Blackboard>("flags");
                if (flagsBB != null)
                {
                    ClientAPI2Blackboard.Serialize(flagsBB, slotInfo.flags);
                }
            }
        }

        public static GameUnlockStatus GetGameUnlockStatus(int gameId)
        {
            var gameInfoBB = GetGameInfo(gameId);

            if (gameInfoBB != null)
                return gameInfoBB.GetValue<GameUnlockStatus>("unlockStatus");

            return GameUnlockStatus.UNKNOWN;
        }

        public static int GetGameMinLevelRestriction(int gameId)
        {
            var gameInfoBB = GetGameInfo(gameId);

            if (gameInfoBB != null)
                return gameInfoBB.GetValue<int>("minLevel");

            return 0;
        }

        public static List<Blackboard> GetUnlockSlotInfoList(int beforeLevel, int currentLevel)
        {
            if (beforeLevel >= currentLevel) return new List<Blackboard>();

            List<Blackboard> unlockSlotList = new List<Blackboard>();

            var slotList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "slotList");

            for (int i = 0; i < slotList.value.Count; ++i)
            {
                var gameId = BlackboardUtils.FindVariable<int>(slotList.value[i], "gameId");
                int status = BlackboardUtils.FindVariable<int>(slotList.value[i], "flags/status").value;
                if (status == 0)
                {
                    int minLevel = GetGameMinLevelRestriction(gameId.value);
                    if (beforeLevel < minLevel && minLevel <= currentLevel)
                    {
                        GameUnlockStatus unlockStatus = GetGameUnlockStatus(gameId.value);
                        if (unlockStatus != GameUnlockStatus.UNLOCKED)
                        {
                            unlockSlotList.Add(slotList.value[i]);
                        }
                    }
                }
            }

            return unlockSlotList;
        }

        public static string GetWinText(string winType)
        {
            bool isError = false;
            string winTextFormat = StringTableUtils.GetString(StringTable.StringTableType.Global, "WIN_TEXT_FORMAT", out isError, winType);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, winTextFormat, out isError);
        }

        public static int GetRandomGameID(int ignoreGameID, int level, int tier)
        {
            List<int> usableGameIDList = new List<int>();
            List<Blackboard> slotList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "/slotList").value;

            for (int i = 0; i < slotList.Count; ++i)
            {
                int gameID = BlackboardUtils.FindVariable<int>(slotList[i], "gameId").value;

                int status = BlackboardUtils.FindVariable<int>(slotList[i], "flags/status").value;
                if (status == 0 && ignoreGameID != gameID)
                {
                    if (level >= GetGameMinLevelRestriction(gameID))
                        usableGameIDList.Add(gameID);
                }
            }

            if (usableGameIDList.Count > 0)
            {
                if (usableGameIDList.Count == 1)
                {
                    return usableGameIDList[0];
                }
                else
                {
                    int index = UnityEngine.Random.Range(0, usableGameIDList.Count);
                    return usableGameIDList[index];
                }
            }
            return -1;
        }

        public static string GetGameTitle(int gameID)
        {
            var gameInfoBB = GetGameInfo(gameID);

            if (gameInfoBB != null)
                return gameInfoBB.GetValue<string>("gameTitle");

            return null;
        }

        public static Orientation GetOrientation()
        {
            return BlackboardUtils.FindVariable<Orientation>(MainBlackboard.Get(), "currentOrientation")?.value ?? Orientation.LANDSCAPE;
        }

        public static Orientation GetGameOrientation(int gameID)
        {
            var gameInfoBB = GetGameInfo(gameID);

            if (gameInfoBB != null)
                return gameInfoBB.GetValue<Orientation>("orientation");

            return Orientation.PORTRAIT;
        }

        public static string GetMetaGameTitle(int gameID)
        {
            string gameTitleFormat = string.Format("GAME_TITLE_{0}", gameID);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, gameTitleFormat);
        }

        public static void UpdateBonusSpinCountPerBet(long betCredit, int spinCount, string where)
        {
            var spinDict = BlackboardUtils.FindVariable<Dictionary<long, int>>(null, where);
            if (spinDict != null && spinDict.value.ContainsKey(betCredit))
                spinDict.value[betCredit] = spinCount;
        }

        public static GameType GetGameType(int gameID)
        {
            var gameInfoBB = GetGameInfo(gameID);

            if (gameInfoBB != null)
                return gameInfoBB.GetVariable<GameType>("gameType").value;

            return GameType.UNKNOWN;
        }

        public static bool IsBuyABonusEventPercentText()
        {
            return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_BUY_A_BONUS").value;
        }

        public static bool IsPlayableSlot(int gameID)
        {
            Blackboard slotInfoBB = GetEarlyAccessSlotInfo(gameID);
            if (slotInfoBB == null)
            {
                slotInfoBB = GetSlotInfoBB(gameID);
            }

            if (slotInfoBB != null)
            {
                int status = BlackboardUtils.FindVariable<int>(slotInfoBB, "flags/status").value;
                if (status == 0)
                    return true;
            }

            return false;
        }

        public static bool IsUseForcedExtraBetRatio()
        {
            return BlackboardUtils.FindVariable<bool>("./useForcedExtraBetRatio")?.value ?? false;
        }

        public static long GetTotalBet(int betIndex)
        {
            var betList = GetBetList();
            if (betList == null || !betList.IsValidIndex(betIndex))
            {
                Debug.LogWarning("BlackboardQueryUtils.GetTotalBet failure. " +
                    "betList:" + (betList == null) + ", " + "betIndex:" + betIndex);
                return 0L;
            }

            long baseBet = betList[betIndex];

            bool useForced = IsUseForcedExtraBetRatio();
            var extraBetRatioList = BlackboardUtils.FindValue<List<Blackboard>>("./game/extraBetRatioList");

            int extraBetRatioIndex;
            if (useForced)
            {
                var forcedExtraBetRatioIndexList = BlackboardUtils.FindVariable<List<int>>("./forcedExtraBetRatioIndexList")?.value;

                extraBetRatioIndex = Mathf.Min(extraBetRatioList.Count - 1, forcedExtraBetRatioIndexList[betIndex]);
            }
            else
            {
                extraBetRatioIndex = BlackboardUtils.FindValue<int>("./extraBetRatioIndex");
            }

            var extraBetNumerator = extraBetRatioList[extraBetRatioIndex].GetValue<int>("numerator");
            var extraBetDenominator = extraBetRatioList[extraBetRatioIndex].GetValue<int>("denominator");
            return baseBet + baseBet * extraBetNumerator / extraBetDenominator;
        }

        public static List<long> GetBetList()
        {
            return BlackboardUtils.FindVariable<List<long>>("./betList").value;
        }

        public static int GetBetIndex()
        {
            return BlackboardUtils.FindVariable<int>("./betIndex").value;
        }

        public static long GetTotalBet()
        {
            return BlackboardUtils.FindVariable<long>("./totalBetCredit")?.value ?? 0L;
        }

        public static long GetCurrentBet()
        {
            int betIndex = GetBetIndex();
            var betList = GetBetList();
            if (betList.IsValidIndex(betIndex))
            {
                return betList[betIndex];
            }

            return -1L;
        }

        public static long GetRawBaseBet(int gameId)
        {
            Dictionary<int, long> baseWagerDict = BlackboardUtils.FindVariable<Dictionary<int, long>>(MainBlackboard.Get(), "baseWagerDict")?.value ?? null;
            if (baseWagerDict != null && baseWagerDict.ContainsKey(gameId))
                return baseWagerDict[gameId];
            return 0L;
        }

        public static List<JackpotType> GetJackpotTypeList()
        {
            return BlackboardUtils.FindVariable<List<JackpotType>>(ContentBlackboard.Get(), "jackpotTypeList")?.value;
        }
    }
}
