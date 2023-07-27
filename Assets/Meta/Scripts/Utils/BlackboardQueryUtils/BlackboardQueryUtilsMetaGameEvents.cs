using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        public static void InitMetaGame()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "metaGameSpinInterrupt", false);
        }

        public static void SetMetaGameSpinInterrupt()
        {
            var isReady = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "metaEventReady", false);
            if (isReady.value)
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "metaGameSpinInterrupt", true);
        }

        public static void ClearMetaGameSpinInterrupt()
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "metaGameSpinInterrupt", false);
        }

        public static bool CheckMetaGameSpinInterrupt()
        {
            return BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "metaGameSpinInterrupt")?.value ?? false;
        }

        public static string GetMetaGameEventName()
        {
            return GetMetaGameEventName(GetMetaGameEventInfo(true));
        }

        public static string GetMetaGameEventName(MetaGameType metaGameType)
        {
            if(IsPassiveMetaGame(metaGameType, out bool isOtherMetaGame))
            {
                return isOtherMetaGame ?
                    GetOtherMetaGameEventName() :
                    GetMetaGameEventName();
            }
            else if(metaGameType == MetaGameType.HIDDEN_OBJECTS)
            {
                return StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_EVENT_NAME_HIDDEN_OBEJCTS");
            }
            else if(metaGameType == MetaGameType.VIP_LOUNGE)
            {
                return StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_LOUNGE_NAME");
            }
            else if (metaGameType == MetaGameType.SEASON_PASS_V2)
            {
                return StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_ALWAYS_NAME");
            }

            Debug.LogError("BlackboardQueryUtilsMetaGameEvent.GetMetaGameEventName failure. " +
                metaGameType.ToString() + " is undefined type.");
            return "";
        }

        public static string GetMetaGameEventName(EventInfo eventInfo)
        {
            if (eventInfo != null)
            {
                switch (eventInfo.type)
                {
                    case EventInfoType.COLLECTING_GAME:
                        return GetMetaGameEventName(eventInfo.type, ((EventDataCollectingGame)eventInfo.constraints).collectingGameId);
                    case EventInfoType.BOSS_RAIDERS:
                        return GetMetaGameEventName(eventInfo.type, ((EventDataBossRaiders)eventInfo.constraints).themeId);
                    //case EventInfoType.LUCKY_FIVE:
                    //case EventInfoType.SEASON_PASS:
                    //case EventInfoType.GEM_JACKPOT:
                    //case EventInfoType.VIP_LOUNGE:
                    default:
                        return GetMetaGameEventName(eventInfo.type, 0);
                    case EventInfoType.CLUB_ARENA:
                        return GetMetaGameEventName(eventInfo.type, 0);
                }
            }

            return "";
        }

        public static string GetMetaGameEventName(EventInfoType eventInfoType, int seasonIndex = 0)
        {
            switch (eventInfoType)
            {
                case EventInfoType.COLLECTING_GAME:
                    string eventNameKey = string.Format("LOBBY_EVENT_NAME_COLLECTABLE_{0}", seasonIndex);
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, eventNameKey);
                case EventInfoType.LUCKY_FIVE:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_EVENT_NAME_LUCKY_FIVE");
                case EventInfoType.SEASON_PASS:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_NAME");
                case EventInfoType.SEASON_PASS_V2:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "EPIC_PASS_ALWAYS_NAME");
                case EventInfoType.BOSS_RAIDERS:
                    string bossRaidersEventNameKey = string.Format("LOBBY_EVENT_NAME_BOSS_RAIDERS_{0}", seasonIndex); // seasonIndex ?
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, bossRaidersEventNameKey);
                case EventInfoType.CLUB_ARENA:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_ARENA_NAME");
                case EventInfoType.GEM_JACKPOT:
                    return StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_NAME");
            }

            return "";
        }

        // return is meta game info updated
        public static bool UpdateMetaGameInfo(MetaGameInfoV1 metaGameInfo, long serverTimestamp, bool useSpinInterrupt = true)
        {
            if (metaGameInfo != null && metaGameInfo.info != null)
            {
                var metaGameInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "metaGameEnterInfo");
                var timestamp = metaGameInfoBB.GetVariable<long>("serverTime");

                if(metaGameInfoBB == null || metaGameInfoBB.GetValue<EventInfoType>("type") != metaGameInfo.type)
                {
                    BlackboardUtils.ClearBlackboard(metaGameInfoBB);
                }

                if (timestamp == null || timestamp.value < serverTimestamp)
                {
                    // Utils Update First!!!
                    if (metaGameInfo.type == EventInfoType.SEASON_PASS && metaGameInfo.info != null)
                        EpicPassUtils.UpdateSeasonPassPointInfo(metaGameInfo.info as SeasonPassPointUpdateInfo);
                    
                    ClientAPI2Blackboard.Serialize(metaGameInfoBB, metaGameInfo);
                    BlackboardUtils.SetOrCreateValue(metaGameInfoBB, "serverTime", serverTimestamp);

                    // InitMetaGame();
                    if (useSpinInterrupt)
                    {
                        SetMetaGameSpinInterrupt();
                    }

                    return true;
                }
            }
            else
            {
                if (useSpinInterrupt)
                    ClearMetaGameSpinInterrupt();
            }

            return false;
        }

        public static bool UpdateEpicPassAlwaysInfo(SeasonPassPointUpdateInfoV2 seasonPassUpdateInfo, long serverTimestamp)
        {
            Blackboard epicPassBB = EpicPassUtilsV2.EpicPassInfo;
            if (seasonPassUpdateInfo != null && epicPassBB != null)
            {
                var epicPassTimestamp = epicPassBB.GetVariable<long>("serverTime");

                if (epicPassTimestamp == null || epicPassTimestamp.value < serverTimestamp)
                {
                    EpicPassUtilsV2.UpdateSeasonPassPointInfoForBB(seasonPassUpdateInfo);
                    BlackboardUtils.SetOrCreateValue(epicPassBB, "serverTime", serverTimestamp);

                    return true;
                }
            }

            return false;
        }

        public static bool IsEpicPassAlwaysActive()
        {
            if (IsActiveMetaGameEvent(EventInfoType.SEASON_PASS_V2) && MetaGameUtils.IsEpicPassAlwayslLock())
            {
                //EventInfo eventInfo = GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
                return true;
            }
            return false;
        }

        public static long GetMinEligibleBet(MetaGameType metaGameType)
        {
            if (IsCollectingGame(metaGameType))
            {
                var gaugeLevelList = GetMetaGameEnterInfo().GetValue<List<Blackboard>>("gaugeLevelList");
                var betList = GetBetList();

                for(int i = 0; i < gaugeLevelList.Count;++i)
                {
                    for(int j = 0; j < betList.Count; ++j)
                    {
                        int gaugeLevel = gaugeLevelList[i].GetValue<int>("gaugeLevel");
                        if (gaugeLevel > 0)
                        {
                            if (betList[j] == gaugeLevelList[i].GetValue<long>("bet"))
                                return betList[j];
                        }
                    }
                }

                return -1L;
            }

            switch (metaGameType)
            {
                case MetaGameType.CLUB_ARENA:
                    {
                        if (ClubArenaUtils.ClubArenaInfo == null) return -1L;

                        List<Blackboard> energyBundles = ClubArenaUtils.ClubArenaInfo.GetVariable<List<Blackboard>>("energyBundleInfoList")?.value ?? new List<Blackboard>();
                        return energyBundles.First(b => b.GetValue<long>("maximumEnergy") > 0).GetValue<long>("bet");
                    }
                case MetaGameType.BOSS_RAIDERS:
                    {
                        if (BossRaidersUtils.BossRaidersInfo == null || BossRaidersUtils.EnergyBundleInfoList == null) return -1L;

                        List<Blackboard> energyBundles = BossRaidersUtils.EnergyBundleInfoList;
                        return energyBundles.First(b => b.GetValue<long>("totalEnergyBundleEarning") > 0).GetValue<long>("bet");
                    }
                case MetaGameType.SEASON_PASS:
                    {
                        var metaGameInfo = GetMetaGameEnterInfo();
                        if (metaGameInfo != null)
                        {
                            var eligibleBetInfoList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(metaGameInfo, "eligibleBetInfoList");
                            if (eligibleBetInfoList != null && eligibleBetInfoList.value != null &&
                                eligibleBetInfoList.value.Count > 0)
                            {
                                return eligibleBetInfoList.value[0].GetValue<long>("totalBet");
                            }
                        }
                        return -1L;
                    }
                case MetaGameType.SEASON_PASS_V2:
                    {
                        if (EpicPassUtilsV2.EpicPassInfo == null) return -1L;

                        List<Blackboard> gaugeLevelList = BlackboardUtils.GetOrCreateVariable<List<Blackboard>>(EpicPassUtilsV2.EpicPassInfo, "gaugeLevelList")?.value ?? null;
                        if (gaugeLevelList != null && gaugeLevelList.Count > 0)
                            return gaugeLevelList[0].GetValue<long>("totalBet");
                        return -1L;
                    }
                case MetaGameType.HIDDEN_OBJECTS:
                    {
                        return GetMinEligibleBetList(ItemType.HIDDEN_UNIVERSE_FINDER)?[0] ?? -1L;
                    }
                case MetaGameType.LUCKY_FIVE:
                    {
                        var metaGameInfo = GetMetaGameEnterInfo();
                        if (metaGameInfo != null)
                        {
                            return BlackboardUtils.GetOrCreateVariable<long>(metaGameInfo, "eligibleMinBet")?.value ?? -1L;
                        }
                        return -1L;
                    }
                case MetaGameType.BUILD_DREAM_SEASON:
                    {
                        return BlackboardUtils.FindVariable<long>("./metaEligibleBetThreshold/vipLounge/buildDreamBetAmount")?.value ?? 0L;
                    }
                case MetaGameType.LEVEL_UP_DASH:
                    {
                        return 0;
                    }
            }

            Debug.LogError("BlackboardQueryUtilsMetaGameEvents.GetMinEligibleBet failure. " + metaGameType + " is undefined MetaGameType.");
            return -1L; // todo
        }

        public static List<long> GetMinEligibleBetList(ItemType itemType)
        {
            var eligibleBetLevelBB = BlackboardUtils.FindValue<Blackboard>("./metaEligibleBetThreshold");

            switch (itemType)
            {
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    {
                        return BlackboardUtils.GetOrCreateVariable<List<long>>(eligibleBetLevelBB, "hiddenUniverseBetList")?.value ?? null;
                    }
            }

            return null;
        }

        public static long GetNextEligibleBet(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.HIDDEN_UNIVERSE_FINDER:
                    {
                        var minBetList = GetMinEligibleBetList(itemType);
                        long currentBet = GetTotalBet();

                        if (minBetList != null && minBetList.Count > 0)
                        {
                            for (int i = 0; i < minBetList.Count; ++i)
                            {
                                long requireBet = minBetList[i];
                                if (currentBet < requireBet)
                                {
                                    return requireBet;
                                }
                            }
                            return minBetList.Last();
                        }
                    }
                    break;
            }

            return -1L;
        }

        public static void UpdateMetaGameInfoEndTurn(MetaGameInfoV1 metaGameInfo, long serverTimestamp)
        {
            // Send endturn event. use meta games.
            UpdateMetaGameInfo(metaGameInfo, serverTimestamp, false);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_END_TURN_META);
        }

        public static void UpdateMetaGameEnterInfo(MetaGameEnterInfoV1 metaGameEnterInfo)
        {
            if (metaGameEnterInfo != null &&
                metaGameEnterInfo.info != null &&
                IsPassiveMetaGame(metaGameEnterInfo.type, out _))
            {
                var metaGameEnterInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "metaGameEnterInfo");
                if(metaGameEnterInfoBB == null || metaGameEnterInfoBB.GetValue<EventInfoType>("type") != metaGameEnterInfo.type)
                {
                    BlackboardUtils.ClearBlackboard(metaGameEnterInfoBB);
                }

                ClientAPI2Blackboard.Serialize(metaGameEnterInfoBB, metaGameEnterInfo);
            }
        }

        public static void UpdateSeasonPassEnterInfo(SeasonPassEnterInfoV2 seasonPassEnterInfo)
        {
            if (seasonPassEnterInfo != null && IsPassiveMetaGame(EventInfoType.SEASON_PASS_V2, out _))
            {
                var seasonPassEnterInfoBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), EpicPassUtilsV2.EPIC_PASS_GAME_INFO);
                if (seasonPassEnterInfoBB == null)
                    BlackboardUtils.ClearBlackboard(seasonPassEnterInfoBB);
                ClientAPI2Blackboard.Serialize(seasonPassEnterInfoBB, seasonPassEnterInfo);

                EventInfo eventInfo = GetMetaGameEventInfo(EventInfoType.SEASON_PASS_V2);
                EpicPassUtilsV2.EventId = eventInfo?.id ?? 0;
            }
        }

        public static void GetMetaGamePassiveEvent(
            bool isInGame, bool isContents, bool allowInactive, bool isOtherMetaGame,
            out string bundleName, out string sharedBundleName, out string iconAssetName, out EventInfoType eventType, out int eventId, out long endTimestamp,
            out bool enableShare, out string eventName, out List<string> bundles)
        {
            string LOBBY_BUTTON_ASSET_NAME = "Lobby Button Scene";
            string INGAME_BUTTON_ASSET_NAME = "In Game Button Scene";

            EventInfo eventInfo = (!isOtherMetaGame) ? GetMetaGameEventInfo(allowInactive) : GetOtherMetaGameEventInfo(allowInactive);
            bool isLockedFeature = IsLockedFeature(LockedFeatureType.META_GAME);

            if (eventInfo != null && !isLockedFeature)
            {
                bundleName = GetMetaBundleName(eventInfo, isContents);
                sharedBundleName = GetSharedMetaBundleName(eventInfo, isContents);
                eventType = eventInfo.type;
                eventId = eventInfo.id;
                endTimestamp = eventInfo.endTimestamp;

                iconAssetName = isInGame ? INGAME_BUTTON_ASSET_NAME : LOBBY_BUTTON_ASSET_NAME;

                enableShare = IsShareEnabled(eventInfo);
                eventName = ((!isOtherMetaGame) ? GetMetaGameEventName() : GetOtherMetaGameEventName()).ToUpper();

                bundles = new List<string>();
                bundles.Add(bundleName);
                if (!string.IsNullOrEmpty(sharedBundleName))
                    bundles.Add(sharedBundleName);

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), (!isOtherMetaGame) ? "metaGameEventID" : "otherMetaGameEventID", eventInfo.id);
            }
            else
            {
                bundleName = "";
                sharedBundleName = "";
                iconAssetName = "";
                eventType = EventInfoType.UNKNOWN;
                eventId = 0;
                endTimestamp = 0;
                enableShare = false;
                eventName = "";
                bundles = null;

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), (!isOtherMetaGame) ? "metaGameEventID" : "otherMetaGameEventID", 0);
            }
        }

        public static Blackboard GetMetaGameEnterInfo()
        {
            var metaGameEnterInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/metaGameEnterInfo");

            if (metaGameEnterInfo == null || metaGameEnterInfo.value == null)
                return null;

            return metaGameEnterInfo.value;
        }

        public static bool IsCollectingGame(MetaGameType type)
        {
            return (int)type < 100 && type != MetaGameType.NONE;
        }

        public static bool IsMetaButtonForceLocked(MetaGameType type)
        {
            if(type == MetaGameType.BUILD_DREAM_SEASON)
            {
                return VipLounge.VipLounge.Utils.IsEnded;
            }
            return false;
        }

        public static bool IsPassiveMetaGame(MetaGameType type, out bool isOtherMetaGame)
        {
            if (type == MetaGameType.VIP_LOUNGE ||
                type == MetaGameType.LEVEL_UP_DASH ||
                type == MetaGameType.BUILD_DREAM_SEASON ||
                type == MetaGameType.HIDDEN_OBJECTS ||
                type == MetaGameType.SEASON_PASS_V2)
            {
                isOtherMetaGame = false;
                return false;
            }
            if (type == MetaGameType.GEM_JACKPOT ||
                type == MetaGameType.GOLDEN_TOWER)
            {
                isOtherMetaGame = true;
                return false;
            }
            return MetaGameTypeToEventInfoType(type, out isOtherMetaGame) != EventInfoType.UNKNOWN;
        }

        public static bool IsPassiveMetaGame(EventInfoType type, out bool isOtherMetaGame)
        {
            if (type == EventInfoType.VIP_LOUNGE ||
                type == EventInfoType.LEVEL_UP_DASH ||
                type == EventInfoType.BUILD_DREAM_SEASON)
            {
                isOtherMetaGame = false;
                return false;
            }
            if (type == EventInfoType.GEM_JACKPOT)
            {
                isOtherMetaGame = true;
                return false;
            }
            return EventInfoTypeToMetaGameType(type, out isOtherMetaGame) != MetaGameType.NONE;
        }

        public static EventInfoType MetaGameTypeToEventInfoType(MetaGameType fromType, out bool isOtherMetaGame)
        {
            isOtherMetaGame = false;

            if(fromType == MetaGameType.GEM_JACKPOT)
            {
                isOtherMetaGame = true;
                return EventInfoType.GEM_JACKPOT;
            }

            string stringType = fromType.ToString();
            if (Enum.TryParse(stringType, out EventInfoType eventInfoType))
            {
                return eventInfoType;
            }

            int intType = (int)fromType;
            if(0 < intType && intType < 100)
            {
                return EventInfoType.COLLECTING_GAME;
            }

            if (fromType != MetaGameType.NONE)
                Debug.LogWarning(string.Format("BlackboardQueryUtilsMetaGameEvents.MetaGameTypeToEventInfoType failure. {0} is not \"MetaGame\".", fromType));
            return EventInfoType.UNKNOWN;
        }

        public static MetaGameType EventInfoTypeToMetaGameType(EventInfoType fromType, out bool isOtherMetaGame)
        {
            isOtherMetaGame = false;

            if(fromType == EventInfoType.GEM_JACKPOT)
            {
                isOtherMetaGame = true;
                return MetaGameType.GEM_JACKPOT;
            }

            string stringType = fromType.ToString();
            if (Enum.TryParse(stringType, out MetaGameType gameType))
            {
                return gameType;
            }
            else if(fromType == EventInfoType.COLLECTING_GAME)
            {
                var eventInfo = GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME);
                if (eventInfo != null)
                {
                    int collectingGameId = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
                    return (MetaGameType)collectingGameId;
                }
            }

            return MetaGameType.NONE;
        }

        public static bool IsMetaGameUnlocked(MetaGameType type)
        {
            if (IsPassiveMetaGame(type, out _))
            {
                return !MetaGameUtils.IsMetaGameLevelLocked();
            }

            if (type == MetaGameType.HIDDEN_OBJECTS)
            {
                return true;
            }

            if (type == MetaGameType.VIP_LOUNGE)
            {
                return true;
            }

            // todo shk vip lounge
            return true;
        }

        public static EventInfo GetMetaGameEventInfo(bool allowInactive = false)
        {
            foreach (MetaGameType type in Enum.GetValues(typeof(MetaGameType)))
            {
                if (!IsPassiveMetaGame(type, out _)) continue;

                var eventType = MetaGameTypeToEventInfoType(type, out _);
                EventInfo eventInfo = GetMetaGameEventInfo(eventType, allowInactive);
                if (eventInfo != null) return eventInfo;
            }

            return null;
        }

        public static EventInfo GetMetaGameEventInfo(EventInfoType type, bool allowInactive = false)
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(type, allowInactive);
            if (eventInfo != null)
                return eventInfo;

            return null;
        }

        public static bool IsActiveMetaGameEvent(EventInfoType type)
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(type);
            return eventInfo != null;
        }

        public static List<EventInfo> GetActiveEventInfoList(List<EventInfoType> types, bool allowInactive = false)
        {
            List<EventInfo> listInfo = new List<EventInfo>();
            if (types != null && types.Count > 0)
            {
                for (int i = 0; i < types.Count; ++i)
                {
                    EventInfo info = PassiveEventManager.Instance.GetActiveEventInfo(types[i], allowInactive);
                    if (info != null) listInfo.Add(info);
                }
            }
            return listInfo;
        }

        public static string GetMetaBundleName(EventInfo eventInfo, bool isContents = false)
        {
            if (eventInfo != null)
            {
                if (eventInfo.type == EventInfoType.COLLECTING_GAME)
                {
                    var collectingGameId = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
                    return GetMetaBundleName(eventInfo.type, isContents, collectingGameId);
                }
                else if (eventInfo.type == EventInfoType.BOSS_RAIDERS)
                {
                    var bossRaidersThemeId = ((EventDataBossRaiders)eventInfo.constraints).themeId;
                    return GetMetaBundleName(eventInfo.type, isContents, bossRaidersThemeId);
                }

                return GetMetaBundleName(eventInfo.type, isContents);
            }

            return "";
        }

        public static bool CheckEarnMetaGameItem(MetaGameType metaGameType)
        {
            return GetEarnMetaGameItemCount(metaGameType) > 0L;
        }

        public static void ClearEarnMetaGameItem()
        {
            var metaGameInfo = GetMetaGameEnterInfo();
            if(metaGameInfo != null)
            {
                var packIdVar = metaGameInfo.GetVariable<int>("packId");
                if (packIdVar != null) packIdVar.value = -1;
            }

            if (BossRaidersUtils.BossRaidersInfo != null)
                BlackboardUtils.SetOrCreateValue(BossRaidersUtils.BossRaidersInfo, "energyEarning", 0L);
            if (EpicPassUtils.EpicPassInfo != null)
                BlackboardUtils.SetOrCreateValue(EpicPassUtils.EpicPassInfo, "earnPoint", 0L);
            if (ClubArenaUtils.ClubArenaInfo != null)
                BlackboardUtils.SetOrCreateValue(ClubArenaUtils.ClubArenaInfo, "energyEarning", 0L);
            if (HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo != null)
                BlackboardUtils.SetOrCreateValue(HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo, "earnFinderFromSpin", 0);
            if (VipLounge.VipLounge.Utils.VipLoungeInfo != null)
                BlackboardUtils.SetOrCreateValue(VipLounge.VipLounge.Utils.VipLoungeInfo, "earnLoungePoint", 0L);
            if (EpicPassUtilsV2.EpicPassInfo != null)
                BlackboardUtils.SetOrCreateValue(EpicPassUtilsV2.EpicPassInfo, "earnPoint", 0L);
            // todo..
        }

        public static long GetEarnMetaGameItemCount(MetaGameType metaGameType)
        {
            if(IsCollectingGame(metaGameType))
            {
                var metaGameInfo = GetMetaGameEnterInfo();

                int packId = metaGameInfo.GetVariable<int>("packId")?.value ?? -1;
                return (packId == -1) ? 0L : 1L;
            }

            switch(metaGameType)
            {
                case MetaGameType.BOSS_RAIDERS:
                    return BossRaidersUtils.EarnEnergy;
                case MetaGameType.SEASON_PASS:
                    return EpicPassUtils.EarnPoint;
                case MetaGameType.SEASON_PASS_V2:
                    return EpicPassUtilsV2.EarnPoint;
                case MetaGameType.CLUB_ARENA:
                    return ClubArenaUtils.EarnEnergy;
                // todo other meta game
                case MetaGameType.HIDDEN_OBJECTS:
                    return HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo.GetVariable<int>("earnFinderFromSpin")?.value ?? 0;
                case MetaGameType.VIP_LOUNGE:
                    return VipLounge.VipLounge.Utils.VipLoungeInfo.GetVariable<long>("earnLoungePoint")?.value ?? 0L;
                case MetaGameType.BUILD_DREAM_SEASON:
                    return VegasDreams.VegasDreams.Utils.SpinEarnDepot;
                case MetaGameType.LUCKY_FIVE:
                    return CheckMetaGameSpinInterrupt() ? 1L : 0L;
                case MetaGameType.GOLDEN_TOWER:
                case MetaGameType.GEM_JACKPOT:
                case MetaGameType.LEVEL_UP_DASH:
                    return 0L;
            }

            Debug.LogError("GetEarnMetaGameItemCount failure. " + metaGameType.ToString() + " is undefined <MetaGameType>.");
            return 0L;
        }

        public static string GetMetaBundleName(EventInfoType eventType, bool isContents = false, int collectingGameId = 0)
        {
            switch (eventType)
            {
                case EventInfoType.COLLECTING_GAME:
                    {
                        return isContents ?
                            StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_COLLECTING_GAME_ASSET_CONTENTS") + (collectingGameId - 1) :
                            StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_COLLECTING_GAME_ASSET_COMMON") + (collectingGameId - 1);
                    }
                case EventInfoType.LUCKY_FIVE:
                    return isContents ? "mglucky5contents0" : "mglucky5common0";
                case EventInfoType.SEASON_PASS:
                    return isContents ? "mgepicpasscontents" : "mgepicpasscommon";
                case EventInfoType.BOSS_RAIDERS:
                    {
                        return isContents ?
                            StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_ASSET_CONTENTS") + collectingGameId :
                            StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_ASSET_COMMON") + collectingGameId;
                    }
                case EventInfoType.CLUB_ARENA:
                    return isContents ? "mgclubarenacontents" : "mgclubarenacommon";
                case EventInfoType.GEM_JACKPOT:
                    return isContents ? "mggemjackpotcontents" : "mggemjackpotcommon";
                case EventInfoType.VIP_LOUNGE:
                    return isContents ? "mgviploungecontents" : "mgviploungecommon";
                case EventInfoType.BUILD_DREAM_SEASON:
                    return isContents ? $"mgvegasdreamscontents{collectingGameId}" : "mgvegasdreamscommon";
                case EventInfoType.SEASON_PASS_V2:
                    return EpicPassUtilsV2.BUNDLE_NAME;
            }

            return "";
        }

        public static string GetSharedMetaBundleName(EventInfo eventInfo, bool isContents)
        {
            if (eventInfo != null)
            {
                switch (eventInfo.type)
                {
                    case EventInfoType.COLLECTING_GAME:
                        {
                            return isContents ?
                                StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_COLLECTING_GAME_ASSET_CONTENTS_SHARED") :
                                StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_COLLECTING_GAME_ASSET_COMMON_SHARED");
                        }
                    case EventInfoType.BOSS_RAIDERS:
                        {
                            return isContents ?
                                StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_ASSET_CONTENTS_SHARED") :
                                StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_ASSET_COMMON_SHARED");
                        }
                }
            }

            return "";
        }

        public static string GetMetaLoadingAssetName(EventInfo eventInfo, int collectingGameId = 0)
        {
            if (eventInfo != null)
            {
                switch (eventInfo.type)
                {
                    case EventInfoType.BOSS_RAIDERS:
                        return "Boss Raiders Loading Scene";
                    case EventInfoType.CLUB_ARENA:
                        return "Club Arena Loading Scene";
                }
            }
            return "";
        }

        public static List<string> GetMetaGameCommonWebImageList(EventInfo eventInfo)
        {
            // Common web image list.
            if (eventInfo != null)
            {
                switch (eventInfo.type)
                {
                    case EventInfoType.SEASON_PASS:
                        {
                            var metaEnterInfoBB = GetMetaGameEnterInfo();

                            if (metaEnterInfoBB != null)
                            {
                                List<string> imageList = new List<string>();

                                string iconBigImageURL = BlackboardUtils.GetOrCreateVariable<string>(metaEnterInfoBB, "iconBigImageUrl").value;
                                string pointIconImageURL = BlackboardUtils.GetOrCreateVariable<string>(metaEnterInfoBB, "pointIconImageUrl").value;
                                string backgroundImageURL = BlackboardUtils.GetOrCreateVariable<string>(metaEnterInfoBB, "backgroundImageUrl").value;

                                if (!string.IsNullOrEmpty(iconBigImageURL))
                                    imageList.Add(iconBigImageURL);
                                if (!string.IsNullOrEmpty(pointIconImageURL))
                                    imageList.Add(pointIconImageURL);
                                if (!string.IsNullOrEmpty(backgroundImageURL))
                                    imageList.Add(backgroundImageURL);

                                if (imageList.Count > 0)
                                    return imageList;
                            }
                        }
                        break;
                    case EventInfoType.SEASON_PASS_V2:
                        {
                            Blackboard epicPassInfoBB = EpicPassUtilsV2.EpicPassInfo;

                            if (epicPassInfoBB != null)
                            {
                                List<string> imageList = new List<string>();

                                string iconTabImageURL = EpicPassUtilsV2.TabIconImageUrl;
                                string pointIconImageURL = EpicPassUtilsV2.PointIconImageUrl;
                                string backgroundImageURL = EpicPassUtilsV2.BackgroundImageUrl;

                                if (!string.IsNullOrEmpty(iconTabImageURL))
                                    imageList.Add(iconTabImageURL);
                                if (!string.IsNullOrEmpty(pointIconImageURL))
                                    imageList.Add(pointIconImageURL);
                                if (!string.IsNullOrEmpty(backgroundImageURL))
                                    imageList.Add(backgroundImageURL);

                                if (imageList.Count > 0)
                                    return imageList;
                            }
                        }
                        break;
                }
            }

            return null;
        }

        public static bool IsShareEnabled(EventInfo eventInfo)
        {
            switch (eventInfo.type)
            {
                case EventInfoType.COLLECTING_GAME:
                    return true;
            }

            return false;
        }

        public static int GetIngameID()
        {
            var contentsBB = ContentBlackboard.Get();
            if (contentsBB != null)
            {
                var roomBB = contentsBB.GetVariable<Blackboard>("game");
                if (roomBB != null && roomBB.value != null)
                {
                    var gameID = roomBB.value.GetVariable<int>("gameId");
                    if (gameID != null)
                        return gameID.value;
                }
            }

            return 0;
        }

        public static List<SlotMaker.TestSuite.DebugSpin> GetMetaGameDebugSpin(EventInfoType type)
        {
            switch (type)
            {
                case EventInfoType.BOSS_RAIDERS:
                    return BossRaidersUtils.GetDebugSpins();
                case EventInfoType.CLUB_ARENA:
                    return ClubArenaUtils.GetDebugSpins();
            }
            return null;
        }

        public static void SetMetaGameDebugSpin(EventInfoType type, int debugIndex)
        {
            switch (type)
            {
                case EventInfoType.BOSS_RAIDERS:
                    BossRaidersUtils.SetDebugSpins(debugIndex);
                    break;
                case EventInfoType.CLUB_ARENA:
                    ClubArenaUtils.SetDebugSpins(debugIndex);
                    break;
            }
        }

        public static void SetOrientation(Orientation target)
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "currentOrientation", target);
        }

        public static void SetOrientationBB(Blackboard targetBB, string key, Orientation setOrientation)
        {
            if (targetBB != null && !string.IsNullOrEmpty(key))
            {
                BlackboardUtils.SetOrCreateValue(targetBB, key, setOrientation);
            }
        }

        public static void MetaGameCrashReport(string name)
        {
            UnityEngine.CrashReportHandler.CrashReportHandler.SetUserMetadata("Meta.metaGameName", name);
        }

        public static List<string> GetMetaExtraBundlesName(EventInfo eventInfo, bool isContents = false)
        {
            if (eventInfo == null)
                return null;

            if (eventInfo.type == EventInfoType.BOSS_RAIDERS)
            {
                int bossRaidersThemeId = ((EventDataBossRaiders)eventInfo.constraints).themeId;
                return GetMetaExtraBundlesName(eventInfo.type, isContents, bossRaidersThemeId);
            }

            return null;
        }

        public static List<string> GetMetaExtraBundlesName(EventInfoType eventType, bool isContents = false, int collectingGameId = 0)
        {
            List<string> extraAssetNameList = new List<string>();
            switch (eventType)
            {
                case EventInfoType.BOSS_RAIDERS:
                    // todo : Change Character AssetBundle StringKey
                    if (isContents)
                        extraAssetNameList.Add(string.Format("mgbossraiderscharacter{0}", collectingGameId));
                    return extraAssetNameList;
            }

            return null;
        }

        public static void SetMetaGameSceneState(SceneState sceneState)
        {
            var currentSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "currentSceneState");
            var prevSceneState = BlackboardUtils.GetOrCreateVariable<SceneState>(MainBlackboard.Get(), "prevSceneState");

            if (sceneState != currentSceneState.value)
            {
                UnityEngine.CrashReportHandler.CrashReportHandler.SetUserMetadata("Meta.prevSceneState", prevSceneState.value.ToString());
                UnityEngine.CrashReportHandler.CrashReportHandler.SetUserMetadata("Meta.currentSceneState", currentSceneState.value.ToString());

                prevSceneState.value = currentSceneState.value;
                currentSceneState.value = sceneState;
            }
        }
#if DEV
        public static void SetMetaItemDebug(MetaGameType metaGameType)
        {
            if (IsCollectingGame(metaGameType))
            {
                var metaGameInfo = GetMetaGameEnterInfo();
                BlackboardUtils.SetOrCreateValue(metaGameInfo, "packId", (int)metaGameType * 4 - 3);
                return;
            }

            switch (metaGameType)
            {
                case MetaGameType.BOSS_RAIDERS:
                    BlackboardUtils.SetOrCreateValue(BossRaidersUtils.BossRaidersInfo, "energyEarning", 1L);
                    return;
                case MetaGameType.SEASON_PASS:
                    BlackboardUtils.SetOrCreateValue(EpicPassUtils.EpicPassInfo, "earnPoint", 1L);
                    return;
                case MetaGameType.SEASON_PASS_V2:
                    BlackboardUtils.SetOrCreateValue(EpicPassUtilsV2.EpicPassInfo, "earnPoint", 1L);
                    return;
                case MetaGameType.CLUB_ARENA:
                    BlackboardUtils.SetOrCreateValue(ClubArenaUtils.ClubArenaInfo, "energyEarning", 1L);
                    return;
                case MetaGameType.LUCKY_FIVE:
                    var enterInfo = GetMetaGameEnterInfo();
                    BlackboardUtils.SetOrCreateValue(enterInfo, "earnPoint", 1L);
                    return;
                // todo other meta game
                case MetaGameType.HIDDEN_OBJECTS:
                    BlackboardUtils.SetOrCreateValue(HiddenObjects.HiddenObjects.Utils.HiddenObjectsInfo, "earnFinderFromSpin", 1);
                    return;
                case MetaGameType.VIP_LOUNGE:
                    BlackboardUtils.SetOrCreateValue(VipLounge.VipLounge.Utils.VipLoungeInfo, "earnLoungePoint", 1L);
                    return;
            }

            Debug.LogError("BlackboardQueryUtilsMetaGameEvents.SetMetaItemDebug failure. " + metaGameType + " is undefined MetaGameType.");
        }
#endif
    }
}
