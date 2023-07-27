using System.Collections.Generic;
using System.Linq;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.ClientAPI;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static Blackboard GetEpicAlbumEnterInfo()
        {
            var info = BlackboardUtils.FindVariable<Blackboard>(null, "/epicAlbumEnterInfo");

            if (info == null || info.value == null)
                return null;

            return info.value;
        }

        public static List<Blackboard> GetEpicAlbumInfoList()
        {
            var info = GetEpicAlbumEnterInfo();

            if (info != null)
            {
                return info.GetValue<List<Blackboard>>("epicAlbumInfoList");
            }

            return null;
        }

        public static bool IsEmptyAlbum(Blackboard epicAlbumInfo)
        {
            if (epicAlbumInfo == null) return true;

            var gameIdList = epicAlbumInfo.GetValue<Blackboard>("categoryInfo").GetVariable<List<int>>("gameIdList")?.value;
            return gameIdList == null || gameIdList.Count == 0;
        }

        public static int GetEpicAlbumEntryLimit()
        {
            var info = GetEpicAlbumEnterInfo();

            if (info != null)
            {
                return info.GetValue<int>("albumEntryLimit");
            }

            return 8;
        }

        public static long GetEpicAlbumFianlRewardGem()
        {
            var info = GetEpicAlbumEnterInfo();

            if (info != null)
            {
                return info.GetValue<long>("finalRewardGem");
            }

            return 0;
        }

        public static Blackboard GetEpicAlbumInfo(CategoryType categoryType)
        {
            var infoList = GetEpicAlbumInfoList();

            if (infoList != null)
            {
                for (int i = 0; i < infoList.Count; i++)
                {
                    if (GetCategoryType(infoList[i]) == categoryType)
                    {
                        return infoList[i];
                    }
                }
            }

            return null;
        }

        public static CategoryType GetCategoryType(Blackboard epicAlbumInfo)
        {
            if (epicAlbumInfo != null)
                return epicAlbumInfo.GetValue<Blackboard>("categoryInfo").GetValue<CategoryType>("categoryType");

            return CategoryType.UNKNOWN;
        }

        public static List<Blackboard> GetWOEInfoList(CategoryType categoryType)
        {
            var info = GetEpicAlbumInfo(categoryType);

            if (info != null)
            {
                return info.GetValue<List<Blackboard>>("woeInfoList");
            }

            return null;
        }

        public static Blackboard GetWOEInfo(CategoryType categoryType, int gameId)
        {
            var infoList = GetWOEInfoList(categoryType);

            if (infoList != null)
            {
                for (int i = 0; i < infoList.Count; i++)
                {
                    if (infoList[i].GetValue<int>("gameId") == gameId)
                    {
                        return infoList[i];
                    }
                }
            }

            return null;
        }

        public static List<int> GetGameIdList(CategoryType categoryType)
        {
            var info = GetEpicAlbumInfo(categoryType);

            if (info != null)
            {
                List<int> gameIdList = info.GetValue<Blackboard>("categoryInfo").GetValue<List<int>>("gameIdList");

                gameIdList = gameIdList.OrderBy(id =>
                {
                    var gameInfo = GetGameInfo(id);

                    return gameInfo.GetValue<int>("minLevel");
                }).ToList();

                List<int> validIdList = new List<int>();
                List<int> invalidIdList = new List<int>();

                for (int i = 0; i < gameIdList.Count; i++)
                {
                    var slotInfo = GetSlotInfoBB(gameIdList[i]);
                    var earlyAccessSlotInfo = GetEarlyAccessSlotInfo(gameIdList[i]);

                    if (slotInfo != null) {
                        int status = BlackboardUtils.FindVariable<int>(slotInfo, "flags/status").value;

                        if (earlyAccessSlotInfo == null)
                        {
                            if (status == 1 || status == 2)
                                invalidIdList.Add(gameIdList[i]);
                            else
                                validIdList.Add(gameIdList[i]);
                        }
                    }
                }

                List<int> resultIdList = new List<int>();
                resultIdList.AddRange(validIdList);
                resultIdList.AddRange(invalidIdList);

                return resultIdList;
            }

            return null;
        }

        public static List<Blackboard> GetWOEInfoList()
        {
            var infoList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/woeInfoList");

            if (infoList == null || infoList.value == null)
                return null;

            return infoList.value;
        }

        public static Blackboard GetWOEInfo(int gameID)
        {
            var infoList = GetWOEInfoList();

            foreach(Blackboard info in infoList)
            {
                if(info.GetValue<int>("gameId") == gameID)
                {
                    return info;
                }
            }

            return null;
        }

        public static void UpdateWOEInfo(WallOfEpicInfo woeInfo)
        {
            if(!CheckUsableEpicAlbum(woeInfo.gameId))
            {
                Debug.LogError("Not Usable Game");
                return;
            }

            var woeInfoBB = GetWOEInfo(woeInfo.gameId);

            if(woeInfoBB == null)
            {
                woeInfoBB = (Blackboard)BlackboardUtils.CreateBlackboard("WallOfEpicInfo");
                BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "woeInfoList", woeInfoBB);
            }

            ClientAPI2Blackboard.Serialize(woeInfoBB, woeInfo);
        }

        public static List<int> GetGameIdList()
        {
            var gameIdList = BlackboardUtils.FindVariable<List<int>>(null, "/epicAlbumGameIdList");

            if (gameIdList == null || gameIdList.value == null)
                return null;

            return gameIdList.value;
        }

        public static bool CheckIfNewWOE(int woeId)
        {
            string key = StringTableUtils.GetString(StringTable.StringTableType.Global ,"EPIC_ALBUM_NEW_WOE_KEY", woeId);
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public static void SetNewWOE(int woeId, bool flag)
        {
            string key = StringTableUtils.GetString(StringTable.StringTableType.Global ,"EPIC_ALBUM_NEW_WOE_KEY", woeId);
            PlayerPrefs.SetInt(key, flag ? 1 : 0);
        }

        public static bool CheckIfNewWOEForCategory(int woeId)
        {
            string key = StringTableUtils.GetString(StringTable.StringTableType.Global ,"EPIC_ALBUM_NEW_WOE_KEY_FOR_CATEGORY", woeId);
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public static void SetNewWOEForCategory(int woeId, bool flag)
        {
            string key = StringTableUtils.GetString(StringTable.StringTableType.Global ,"EPIC_ALBUM_NEW_WOE_KEY_FOR_CATEGORY", woeId);
            PlayerPrefs.SetInt(key, flag ? 1 : 0);
        }

        public static bool CheckIfSharedWOE(int woeId)
        {
            string key = StringTableUtils.GetString(StringTable.StringTableType.Global ,"EPIC_ALBUM_SHARED_WOE_KEY", woeId);
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public static void SetSharedWOE(int woeId)
        {
            string key = StringTableUtils.GetString(StringTable.StringTableType.Global ,"EPIC_ALBUM_SHARED_WOE_KEY", woeId);
            PlayerPrefs.SetInt(key, 1);
        }

        public static bool CheckIfEpicAlbumMigrationNewBadge()
        {
            return PlayerPrefs.GetInt("EPIC_ALBUM_NEW_BUTTON_BADGE", 0) == 1;
        }

        public static void SetEpicAlbumMigrationNewBadge(bool isNew)
        {
            PlayerPrefs.SetInt("EPIC_ALBUM_NEW_BUTTON_BADGE", isNew ? 1 : 0);
        }

        // Lobby Button
        public static bool CheckIfNewExistInEpicAlbum()
        {
            if(CheckIfEpicAlbumMigrationNewBadge())
            {
                return true;
            }

            var woeInfoList = GetWOEInfoList();

            if (woeInfoList != null)
            {
                bool newExist = false;
                for (int i = 0; i < woeInfoList.Count; i++)
                {
                    int woeId = woeInfoList[i].GetValue<int>("id");
                    if (CheckIfNewWOEForCategory(woeId) || CheckIfNewWOE(woeId))
                    {
                        newExist = true;
                        break;
                    }
                }
                return newExist;
            }

            return false;
        }

        // Category Cover Page
        public static bool CheckIfNewExistInCategory(CategoryType categoryType)
        {
            List<Blackboard> woeInfoList = GetWOEInfoList(categoryType);

            bool newExist = false;
            for (int i = 0; i < woeInfoList.Count; i++)
            {
                int id = woeInfoList[i].GetValue<int>("id");

                if (CheckIfNewWOEForCategory(id) || CheckIfNewWOE(id))
                {
                    newExist = true;
                    break;
                }
            }

            return newExist;
        }

        public static long GetWinCreditOfGame(int gameId)
        {
            var woeInfoList = GetWOEInfoList();

            if (woeInfoList != null)
            {
                for (int i = 0; i < woeInfoList.Count; i++)
                {
                    if (gameId == woeInfoList[i].GetValue<int>("gameId"))
                    {
                        return woeInfoList[i].GetValue<long>("winCredit");
                    }
                }
            }

            return -1;
        }

        public static int GetStarCountOfGame(int gameId)
        {
            var woeInfoList = GetWOEInfoList();

            if (woeInfoList != null)
            {
                for (int i = 0; i < woeInfoList.Count; i++)
                {
                    if (gameId == woeInfoList[i].GetValue<int>("gameId"))
                    {
                        return woeInfoList[i].GetValue<int>("starCount");
                    }
                }
            }

            return 0;
        }

        public static bool CheckIfNewEpicRecord(int gameId, long earnCredit)
        {
            List<Blackboard> woeInfoList = GetWOEInfoList();

            if (woeInfoList != null)
            {
                for (int i = 0; i < woeInfoList.Count; i++)
                {
                    if (woeInfoList[i].GetValue<int>("gameId") == gameId)
                    {
                        if (earnCredit > woeInfoList[i].GetValue<long>("winCredit"))
                            return true;

                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool CheckIfGameExistInEpicAlbum(int gameId)
        {
            var gameIdList = GetGameIdList();

            if (gameIdList != null)
            {
                for (int i = 0; i < gameIdList.Count; i++)
                {
                    if (gameIdList[i] == gameId)
                        return true;
                }
            }

            return false;
        }

        public static bool CheckUsableEpicAlbum(int gameID)
        {
            var earlyAccessInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameID);
            bool enableEpicAlbum = BlackboardUtils.GetOrCreateVariable<bool>(null, "/values/misc/ENABLE_EPIC_ALBUM").value;
            bool gameExistInEpicAlbum = BlackboardQueryUtils.CheckIfGameExistInEpicAlbum(gameID);

            return earlyAccessInfo == null && enableEpicAlbum && gameExistInEpicAlbum;
        }

        public static int GetEpicAlbumRewardCount(CategoryType categoryType)
        {
            var info = GetEpicAlbumInfo(categoryType);

            if (info != null)
            {
                int STAR_COUNT = 3;

                var totalStarCount = info.GetValue<int>("totalStarCount");
                var collectedRewardStage = info.GetValue<int>("collectedRewardStage");

                int collectedStarCount = collectedRewardStage * STAR_COUNT;

                if(totalStarCount > collectedStarCount)
                {
                    return (totalStarCount - collectedStarCount) / STAR_COUNT;
                }
            }

            return 0;
        }
    }
}
