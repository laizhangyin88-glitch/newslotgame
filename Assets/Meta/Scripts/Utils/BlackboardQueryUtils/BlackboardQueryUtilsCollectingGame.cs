using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using BagelCode.Scratcher;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

using static BagelCode.MetaGameUtils;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static Blackboard GetCollectingGameInfo()
        {
            var collectingGameInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/collectingGameInfo");

            if (collectingGameInfo == null || collectingGameInfo.value == null)
                return null;

            return collectingGameInfo.value;
        }

        public static List<Blackboard> GetScratcherList()
        {
            var scratcherList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/collectingGameInfo/scratcherList");

            if (scratcherList == null || scratcherList.value == null)
                return null;

            return scratcherList.value;
        }

        public static List<Blackboard> GetResultPieceList()
        {
            var resultPieceList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/collectingGameInfo/packOpenResultList");

            if (resultPieceList == null || resultPieceList.value == null)
                return null;

            return resultPieceList.value;
        }

        public static List<Blackboard> GetSharePieceList()
        {
            var resultPieceList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/collectingGameInfo/sharePieceList");

            if (resultPieceList == null || resultPieceList.value == null)
                return null;

            return resultPieceList.value;
        }

        public static Blackboard GetScratcher(int id)
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null || scratcherList.Count == 0)
                return null;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                if (scratcherList[i].GetValue<int>("scratcherId") == id)
                {
                    return scratcherList[i];
                }
            }

            return null;
        }

        public static int GetScratcherFirstPieceId()
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null || scratcherList.Count == 0)
                return -1;

            List<Blackboard> itemList = scratcherList[0].GetValue<List<Blackboard>>("pieceList");
            return itemList[0].GetValue<int>("pieceId");
        }

        public static Blackboard GetScratcherByItem(int id)
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null || scratcherList.Count == 0)
                return null;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                List<Blackboard> itemList = scratcherList[i].GetValue<List<Blackboard>>("pieceList");

                for (int j = 0; j < itemList.Count; j++)
                {
                    if (itemList[j].GetValue<int>("pieceId") == id)
                    {
                        return scratcherList[i];
                    }
                }
            }

            return null;
        }

        public static long GetFreePackCollectCoolTime()
        {
            var coolTime = BlackboardUtils.FindVariable<long>(null, "/collectingGameInfo/freeChestCooltime");

            if (coolTime != null)
                return coolTime.value;

            return 0;
        }

        public static long GetLastFreePackCollectTimestamp()
        {
            var timestamp = BlackboardUtils.FindVariable<long>(null, "/collectingGameInfo/lastFreePackCollectTimestamp");

            if (timestamp == null)
                return -1;

            return timestamp.value;
        }

        public static long GetLastFreePackVideoAdsCollectingTimestamp()
        {
            var timestamp = BlackboardUtils.FindVariable<long>(null, "/collectingGameInfo/lastFreeChestVideoAdsClaimTimestamp");

            if (timestamp == null)
                return -1;

            return timestamp.value;
        }

        public static void SetCollectingGameRequestResetTimestamp(long resetTimestamp)
        {
            var timestamp = BlackboardUtils.FindVariable<long>(null, "/collectingGameInfo/metaGameRequestResetTimestamp");

            if (timestamp != null)
                timestamp.value = resetTimestamp;
        }

        public static long GetCollectingGameRequestResetTimestamp()
        {
            var timestamp = BlackboardUtils.FindVariable<long>(null, "/collectingGameInfo/metaGameRequestResetTimestamp");

            if (timestamp == null)
                return -1;

            return timestamp.value;
        }

        public static List<Blackboard> GetScratcherNewBadgeList()
        {
            var scratcherList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/collectingGameNewBadgeInfo/scratcherList");

            if (scratcherList == null || scratcherList.value == null)
                return null;

            return scratcherList.value;
        }

        public static Blackboard GetScratcherNewBadge(int id)
        {
            List<Blackboard> scratcherList = GetScratcherNewBadgeList();

            if (scratcherList == null || scratcherList.Count == 0)
                return null;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                if (scratcherList[i].GetValue<int>("scratcherId") == id)
                {
                    return scratcherList[i];
                }
            }

            return null;
        }

        public static Blackboard GetPieceNewBadge(int id)
        {
            List<Blackboard> scratcherList = GetScratcherNewBadgeList();

            if (scratcherList == null || scratcherList.Count == 0)
                return null;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                List<Blackboard> pieceList = scratcherList[i].GetValue<List<Blackboard>>("pieceList");

                for (int j = 0; j < pieceList.Count; j++)
                {
                    if (pieceList[j].GetValue<int>("pieceId") == id)
                        return pieceList[j];
                }
            }

            return null;
        }

        public static int GetIndexOfScratcher(int id)
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null || scratcherList.Count == 0)
                return -1;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                if (scratcherList[i].GetValue<int>("scratcherId") == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public static Blackboard GetScratcherOfIndex(int index)
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null || scratcherList.Count == 0)
                return null;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                if (i == index)
                {
                    return scratcherList[i];
                }
            }

            return null;
        }

        public static int GetIndexOfItem(int pieceId)
        {
            EventInfo metaGameInfo = GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);

            if (metaGameInfo == null)
                return -1;

            int collectingGameId = ((EventDataCollectingGame)metaGameInfo.constraints).collectingGameId;
            int index = pieceId - 1;
            for (int i = 1; i < collectingGameId; ++i)
            {
                if (!System.Enum.IsDefined(typeof(MetaGameType), i)) // check int to enum
                {
                    Debug.LogWarning(string.Format("Warning in BlackboardQueryUtilsCollectingGame.GetIndexOfItem. {0} is undefined in <CollectingGameType>.", i));
                }

                var type = (MetaGameType)i;
                index -= CollectingGamePieceTypeCount(type);
            }

            return index;
        }

        public static Sprite GetAssetOfItem(int pieceId)
        {
            if (CollectingGameCustomData.Instance == null)
                return null;

            int index = GetIndexOfItem(pieceId);
            int pieceTypeCount = 4;

            int pieceGradeId = index / pieceTypeCount;
            int pieceTypeId = index % pieceTypeCount;
            return CollectingGameCustomData.Instance.collectingGameAssets.itemAssets[pieceGradeId].assets[pieceTypeId];
        }

        public static int GetAdjacentScratcherId(int id, bool next)
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null || scratcherList.Count == 0)
                return -1;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                if (scratcherList[i].GetValue<int>("scratcherId") == id)
                {
                    if (next)
                    {
                        if (i + 1 >= scratcherList.Count)
                            return scratcherList[0].GetValue<int>("scratcherId");

                        return scratcherList[i + 1].GetValue<int>("scratcherId");
                    }
                    else
                    {
                        if (i - 1 < 0)
                            return scratcherList[scratcherList.Count - 1].GetValue<int>("scratcherId");
                        return scratcherList[i - 1].GetValue<int>("scratcherId");
                    }
                }
            }

            return -1;
        }

        public static int GetScratcherCount()
        {
            List<Blackboard> scratcherList = GetScratcherList();

            if (scratcherList == null)
                return -1;

            return scratcherList.Count;
        }

        public static void UpdateCollectingGameScratcher(int scratcherId, CollectingGameScratcherInfo updatedScratcherInfo)
        {
            Blackboard scratcher = GetScratcher(scratcherId);

            if (scratcher == null)
                return;

            ClientAPI2Blackboard.Serialize(scratcher, updatedScratcherInfo);

            List<Blackboard> pieceList = scratcher.GetValue<List<Blackboard>>("pieceList");
            for (int i = 0; i < pieceList.Count; i++)
            {
                UpdatePiecePossessions(pieceList[i].GetValue<int>("pieceId"), pieceList[i]);
            }
        }

        public static List<Blackboard> GetChestList()
        {
            var list = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/collectingGameInfo/packList");

            if (list == null || list.value == null)
                return null;

            return list.value;
        }

        public static int GetAllPackPossessions()
        {
            var packList = GetChestList();

            int count = 0;

            if (packList != null)
            {
                for (int i = 0; i < packList.Count; i++)
                {
                    count += packList[i].GetValue<int>("possessions");
                }
            }

            return count;
        }

        public static void AddPackPossessions(int packId, int count)
        {
            var packList = GetChestList();

            if (packList != null)
            {
                for (int i = 0; i < packList.Count; i++)
                {
                    if (packList[i].GetValue<int>("packId") == packId)
                    {
                        int possessions = packList[i].GetValue<int>("possessions");
                        BlackboardUtils.SetOrCreateValue<int>(packList[i], "possessions", possessions + count);
                        break;
                    }
                }
            }

            AddTotalPossessions(count);
        }

        public static void AddTotalPossessions(int count)
        {
            var metaGameEnterInfoBB = GetMetaGameEnterInfo();
            if(metaGameEnterInfoBB != null)
            {
                EventInfoType type = metaGameEnterInfoBB.GetValue<EventInfoType>("type");

                if(type == EventInfoType.COLLECTING_GAME)
                {
                    BlackboardUtils.SetOrCreateValue<int>(metaGameEnterInfoBB, "totalPossessions", metaGameEnterInfoBB.GetValue<int>("totalPossessions") + count);
                }
            }
        }

        public static List<Blackboard> GetConverterPieceList()
        {
            var converterPieceList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/collectingGameInfo/converterPieceList");

            if (converterPieceList == null || converterPieceList.value == null)
                return null;

            return converterPieceList.value;
        }

        public static Blackboard GetPieceInfo(int pieceId)
        {
            List<Blackboard> scratcherList = GetScratcherList();

            for (int i = 0; i < scratcherList.Count; i++)
            {
                List<Blackboard> pieceList = scratcherList[i].GetValue<List<Blackboard>>("pieceList");

                for (int j = 0; j < pieceList.Count; j++)
                {
                    if (pieceList[j].GetValue<int>("pieceId") == pieceId)
                    {
                        return pieceList[j];
                    }
                }
            }

            return null;
        }

        public static Blackboard GetPackInfo(int packId)
        {
            List<Blackboard> packList = GetChestList();

            if (packList != null)
            {
                for (int i = 0; i < packList.Count; i++)
                {
                    if (packList[i].GetValue<int>("packId") == packId)
                    {
                        return packList[i];
                    }
                }
            }

            return null;
        }

        public static int GetMakableScratcherCount(int scratcherId)
        {
            var scratcherList = GetScratcherList();
            for (int i = 0; i < scratcherList.Count; i++)
            {
                if (scratcherList[i].GetValue<int>("scratcherId") == scratcherId)
                {
                    int count = Int32.MaxValue;
                    var pieceList = scratcherList[i].GetValue<List<Blackboard>>("pieceList");
                    for (int j = 0; j < pieceList.Count; j++)
                    {
                        int possessions = pieceList[j].GetValue<int>("possessions");
                        int requirement = pieceList[j].GetValue<int>("requirement");

                        int quotient = possessions / requirement;
                        if (count > quotient)
                            count = quotient;
                    }

                    return count;
                }
            }

            return -1;
        }

        public static int GetAllMakableScratcherCount()
        {
            int allMakableScratcherCount = 0;
            int scratcherCount = GetScratcherCount();

            for (int i = 0; i < scratcherCount; i++)
            {
                Blackboard scratcherInfo = GetScratcherOfIndex(i);
                if(scratcherInfo != null)
                    allMakableScratcherCount += GetMakableScratcherCount(scratcherInfo.GetValue<int>("scratcherId"));
            }
            return allMakableScratcherCount;
        }

        public static void UpdatePiecePossessions(int pieceId, Blackboard pieceInfo)
        {
            int possessions = pieceInfo.GetValue<int>("possessions");
            BlackboardUtils.SetOrCreateValue<int>(GetPieceInfo(pieceId), "possessions", possessions);
        }

        public static void InitCollectingGameNewBadgeInfoBlackboard()
        {
            var newBadgeInfoExist = BlackboardUtils.FindVariable<Blackboard>(null, "/collectingGameNewBadgeInfo");

            if (newBadgeInfoExist != null)
            {
                if (newBadgeInfoExist.value.GetValue<int>("eventId") == GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME).id)
                    return;
                else
                    BlackboardUtils.ClearBlackboard(newBadgeInfoExist.value);
            }

            var newBadgeInfo = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "collectingGameNewBadgeInfo");

            BlackboardUtils.SetOrCreateValue<int>(newBadgeInfo, "eventId", GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME).id);

            GameObject scratcherListGo = new GameObject();
            scratcherListGo.name = "scratcherList";
            scratcherListGo.transform.parent = newBadgeInfo.propertiesBindTarget.GetComponent<Transform>();
            List<Blackboard> scratcherBBList = new List<Blackboard>();


            var scratcherList = GetScratcherList();
            Variable variable = null;

            for (int i = 0; i < scratcherList.Count; i++)
            {
                var go = new GameObject();
                go.name = "scratcher";
                go.transform.parent = scratcherListGo.transform;
                var scratcherBB = go.AddComponent<Blackboard>();

                BlackboardUtils.SetOrCreateValue<int>(scratcherBB, "scratcherId", scratcherList[i].GetValue<int>("scratcherId"));

                var pieceListGo = new GameObject();
                pieceListGo.name = "pieceList";
                pieceListGo.transform.parent = go.transform;
                List<Blackboard> pieceBBList = new List<Blackboard>();

                var pieceList = scratcherList[i].GetValue<List<Blackboard>>("pieceList");
                for (int j = 0; j < pieceList.Count; j++)
                {
                    go = new GameObject();
                    go.name = "piece";
                    go.transform.parent = pieceListGo.transform;

                    var pieceBB = go.AddComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue<int>(pieceBB, "pieceId", pieceList[j].GetValue<int>("pieceId"));
                    BlackboardUtils.SetOrCreateValue<bool>(pieceBB, "new", false);
                    pieceBBList.Add(pieceBB);
                }
                variable = scratcherBB.AddVariable("pieceList", typeof(List<Blackboard>));
                variable.value = pieceBBList;

                scratcherBBList.Add(scratcherBB);
            }

            variable = newBadgeInfo.AddVariable("scratcherList", typeof(List<Blackboard>));
            variable.value = scratcherBBList;
        }

        public static void InitPieceNewBadgeInfo()
        {
            List<Blackboard> scratcherList = GetScratcherNewBadgeList();

            for (int i = 0; i < scratcherList.Count; i++)
            {
                List<Blackboard> pieceList = scratcherList[i].GetValue<List<Blackboard>>("pieceList");
                for (int j = 0; j < pieceList.Count; j++)
                {
                    BlackboardUtils.SetOrCreateValue<bool>(pieceList[j], "new", false);
                }
            }
        }

        public static void InitScratcherNewBadgeInfo(int scratcherId)
        {
            Blackboard scratcher = GetScratcherNewBadge(scratcherId);

            List<Blackboard> pieceList = scratcher.GetValue<List<Blackboard>>("pieceList");
            for (int i = 0; i < pieceList.Count; i++)
            {
                BlackboardUtils.SetOrCreateValue<bool>(pieceList[i], "new", false);
            }
        }

        public static bool CheckNewExistInScratcher(int scratcherId)
        {
            Blackboard scratcher = GetScratcherNewBadge(scratcherId);

            List<Blackboard> pieceList = scratcher.GetValue<List<Blackboard>>("pieceList");
            for (int i = 0; i < pieceList.Count; i++)
            {
                if (pieceList[i].GetValue<bool>("new"))
                    return true;
            }

            return false;
        }

        public static float GetCurrentGaugeLevelAsFloat()
        {
            var gaugeLevelList = GetMetaGameEnterInfo().GetValue<List<Blackboard>>("gaugeLevelList");
            var gameType = BlackboardUtils.FindVariable<GameType>("/enterGameInfo/gameType");
            var betCredit = gameType.value == GameType.VIDEO_POKER ? BlackboardUtils.FindVariable<long>("./betPerHand") : BlackboardUtils.FindVariable<long>("./betCredit");

            int indexOfCurrentGauge = 0;
            int currentGaugeLevel = 0;

            for (int i = 0; i < gaugeLevelList.Count; i++)
            {
                if (i == 0 || currentGaugeLevel != gaugeLevelList[i].GetValue<int>("gaugeLevel"))
                {
                    currentGaugeLevel = gaugeLevelList[i].GetValue<int>("gaugeLevel");
                    indexOfCurrentGauge = 0;
                }
                else
                    indexOfCurrentGauge++;

                if (gaugeLevelList[i].GetValue<long>("bet") <= betCredit.value
                    && ( i+1 >= gaugeLevelList.Count || gaugeLevelList[i+1].GetValue<long>("bet") > betCredit.value ) )
                    break;
            }

            int countOfCurrentGauge = GetCountOfGaugeLevel(currentGaugeLevel);
            if (countOfCurrentGauge != 0)
            {
                return currentGaugeLevel + (float)indexOfCurrentGauge / (float)countOfCurrentGauge;
            }

            return 0;
        }

        public static int GetCountOfGaugeLevel(int gaugeLevel)
        {
            var gaugeLevelList = GetMetaGameEnterInfo().GetValue<List<Blackboard>>("gaugeLevelList");
            // var gaugeLevelList = BlackboardUtils.FindVariable<List<Blackboard>>("./metaGameEnterInfo/gaugeLevelList");

            int count = 0;
            for (int i = 0; i < gaugeLevelList.Count; i++)
            {
                if (gaugeLevelList[i].GetValue<int>("gaugeLevel") == gaugeLevel)
                    count++;
            }

            return count;
        }

        public static void UpdateCollectingGameStatus()
        {
            var metaGameEnterInfoBB = GetMetaGameEnterInfo();
            if(metaGameEnterInfoBB != null)
            {
                EventInfoType type = metaGameEnterInfoBB.GetValue<EventInfoType>("type");

                if(type == EventInfoType.COLLECTING_GAME)
                {
                    BlackboardUtils.SetOrCreateValue<int>(metaGameEnterInfoBB, "totalPossessions", GetAllPackPossessions());
                    BlackboardUtils.SetOrCreateValue<int>(metaGameEnterInfoBB, "completeScratchers", GetAllMakableScratcherCount());
                }
            }
        }

        public static void CollectingGameMovePackComplete()
        {
            ClearMetaGameSpinInterrupt();
        }

        public static List<string> GetCollectingGameImageUrlList()
        {
            List<string> urlList = new List<string>();

            List<Blackboard> scratcherList = GetScratcherList();

            for (int i = 0; i < scratcherList.Count; i++)
            {
                Blackboard scratcherInfo = scratcherList[i];
                urlList.Add(scratcherInfo.GetValue<string>("scratcherSimpleImageUrl"));

                Blackboard scratcherRewardInfo = scratcherInfo.GetValue<Blackboard>("reward");
                urlList.Add(scratcherRewardInfo.GetValue<string>("backgroundImageUrl"));
                urlList.Add(scratcherRewardInfo.GetValue<string>("sampleImageUrl"));
            }

            return urlList;
        }

        public static string GetScratcherName(ScratcherName scratcherName)
        {
            if(scratcherName == ScratcherName.UNKNOWN) return "";

            string key = string.Format("COLLECTING_GAME_SCRATCHER_NAME_{0}", scratcherName.ToString());
            return StringTableUtils.GetString(StringTable.StringTableType.Global, key);
        }

        public static int GetChestIndex(int chestId)
        {
            return (chestId - 1) % 4;
        }

        public static string GetChestName(int chestId)
        {
            int index = GetChestIndex(chestId);

            if (index < 0 || index > 3) return "";

            string key = string.Format("COLLECTING_GAME_CHEST_NAME_{0}", index + 1);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, key);
        }

        public static string GetChestNameForReward(int chestId)
        {
            int index = GetChestIndex(chestId);

            if (index < 0 || index > 3) return "";

            string key = string.Format("POPUP_COMMON_REWARD_RESULT_CHEST_NAME_{0}_TEXT", index + 1);
            return StringTableUtils.GetString(StringTable.StringTableType.Global, key);
        }

        public static void SetScratcherPopupInfo(Blackboard scratcherBB, Blackboard infoBB, GameObject caller = null)
        {
            if (infoBB == null)
                return;

            if (caller != null)
                scratcherBB.AddVariable("caller", caller);

            scratcherBB.AddVariable("_scratcherId", infoBB.GetValue<int>("_scratcherId"));
            scratcherBB.AddVariable("_isAuto", infoBB.GetValue<bool>("_isAuto"));
            scratcherBB.AddVariable("_isReward", infoBB.GetValue<bool>("_isReward"));
            scratcherBB.AddVariable("_remainingCount", infoBB.GetValue<int>("_remainingCount"));
        }

        public static void ClearScratcherPrizeInfo(Blackboard scratcherBB)
        {
            var scratcherNameList = scratcherBB.GetVariable<List<string>>("_scratcherNameList");
            scratcherNameList?.value.Clear();

            var prizeList = scratcherBB.GetVariable<List<long>>("_prizeList");
            prizeList?.value.Clear();

            var winTypeList = scratcherBB.GetVariable<List<int>>("_winTypeList");
            winTypeList?.value.Clear();
        }

        public static void SetScratcherTotalResultPopupInfo(Blackboard scratcherBB, Blackboard infoBB, GameObject caller = null, bool isReward = false)
        {
            if (infoBB == null)
                return;

            if (caller != null)
                scratcherBB.AddVariable("caller", caller);

            SetScratcherPrizeInfo(scratcherBB, infoBB, isReward);
        }

        public static void SetScratcherPrizeInfoFromRewardResult(Blackboard scratcherBB, Blackboard rewardResultBB)
        {
            ScratcherName scratcherNameType = rewardResultBB.GetValue<ScratcherName>("scratcherName");
            string scratcherName = GetScratcherName(scratcherNameType);

            var scratcherNameList = BlackboardUtils.GetOrCreateVariable<List<string>>(scratcherBB, "_scratcherNameList");
            if (scratcherNameList.value == null) scratcherNameList.value = new List<string>();

            scratcherNameList.value.Add(scratcherName);

            var prizeList = BlackboardUtils.GetOrCreateVariable<List<long>>(scratcherBB, "_prizeList");
            if (prizeList.value == null) prizeList.value = new List<long>();

            var prize = rewardResultBB.GetValue<long>("credit");
            prizeList.value.Add(prize);

            var winTypeList = BlackboardUtils.GetOrCreateVariable<List<int>>(scratcherBB, "_winTypeList");
            if (winTypeList.value == null) winTypeList.value = new List<int>();

            var winType = rewardResultBB.GetValue<ScratcherBigWinType>("bigWinType");
            winTypeList.value.Add((int)winType);
        }

        public static void SetScratcherPrizeInfo(Blackboard scratcherBB, Blackboard infoBB, bool isReward)
        {
            var infoScratcherNameList = infoBB.GetVariable<List<string>>("_scratcherNameList")?.value;
            if (infoScratcherNameList == null) infoScratcherNameList = new List<string>();

            var infoPrizeList = infoBB.GetVariable<List<long>>("_prizeList")?.value;
            if (infoPrizeList == null) infoPrizeList = new List<long>();

            var infoWinTypeList = infoBB.GetVariable<List<int>>("_winTypeList")?.value;
            if (infoWinTypeList == null) infoWinTypeList = new List<int>();

            BlackboardUtils.SetOrCreateValue(scratcherBB, "_scratcherNameList", infoScratcherNameList);
            BlackboardUtils.SetOrCreateValue(scratcherBB, "_prizeList", infoPrizeList);
            BlackboardUtils.SetOrCreateValue(scratcherBB, "_winTypeList", infoWinTypeList);

            if (!isReward)
            {
                var playedRewardResult = infoBB.GetVariable<Blackboard>("_scratcherRewardResult");
                if (playedRewardResult != null && playedRewardResult.value != null)
                {
                    ScratcherName scratcherNameType = playedRewardResult.value.GetValue<ScratcherName>("scratcherName");
                    string scratcherName = GetScratcherName(scratcherNameType);

                    var scratcherNameList = scratcherBB.GetVariable<List<string>>("_scratcherNameList");
                    scratcherNameList.value.Add(scratcherName);

                    var prizeList = scratcherBB.GetVariable<List<long>>("_prizeList");
                    var prize = playedRewardResult.value.GetValue<long>("credit");
                    prizeList.value.Add(prize);

                    var winTypeList = scratcherBB.GetVariable<List<int>>("_winTypeList");
                    var winType = playedRewardResult.value.GetValue<ScratcherBigWinType>("bigWinType");
                    winTypeList.value.Add((int)winType);
                }
            }
        }

        public static IEnumerator RedeemCollectingGameScratcherCoroutine(Blackboard agent, int scratcherId, System.Action onSuccess = null, System.Action onFailure = null)
        {
            bool wait = true;
            EventInfo metaGameInfo = GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
            BagelCodeClientAPI.CollectingGameScratcherRedeemRequest(metaGameInfo.id, scratcherId,
                (response) =>
                {
                    UpdateCollectingGameScratcher(response.updatedScratcherInfo.scratcherId, response.updatedScratcherInfo);
                    UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);

                    var rewardBB = BlackboardUtils.GetOrCreateBlackboard(agent, "_scratcherRewardResult");
                    ClientAPI2Blackboard.Serialize(rewardBB, response.rewardResult);
                    ApplyRewardResult((Blackboard)rewardBB);

                    UpdateCollectingGameStatus();

                    onSuccess?.Invoke();
                    wait = false;
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);

                    onFailure?.Invoke();
                    wait = false;
                });

            while (wait)
                yield return new WaitForEndOfFrame();
        }
    }
}
