using System;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static class MetaGameUtils
    {
        public static CollectingGameInfoResponseV1 collectingGameInfoResponse { get; private set; }
        public static event System.Action OnCollectingGameInfoUpdated;

        public static void RequestCollectingGameInfo(System.Action successAction=null)
        {
            EventInfo metaGameInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME);
            if (metaGameInfo == null) return;

            BagelCodeClientAPI.CollectingGameInfoRequest(metaGameInfo.id,
                (response) =>
                {
                    collectingGameInfoResponse = response;
                    var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "collectingGameInfo");
                    ClientAPI2Blackboard.Serialize(bb, response);

                    BlackboardQueryUtils.InitCollectingGameNewBadgeInfoBlackboard();
                    successAction?.Invoke();
                    OnCollectingGameInfoUpdated?.Invoke();
                },
                (error) =>
                {
                    GlobalErrorHandler.GlobalError(error);
                }
            );
        }

        public static int CollectingGamePieceTypeCount(MetaGameType type)
        {
            int pieceCount = 4;
            int typeCount = 5;

            switch (type)
            {
                case MetaGameType.JOKERS_TABLE:
                    typeCount = 4;
                    break;
            }

            return pieceCount * typeCount;
        }

        public static bool IsPlayingMetaGame()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(true);
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);
            return (eventInfo != null && !isLockedFeature);
        }

        public static bool IsPlayingCollectingGame()
        {
            EventInfo collectingGameEventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, false);
            return collectingGameEventInfo != null;
        }

        public static bool IsPlayingLuckyFive()
        {
            EventInfo luckyFiveInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.LUCKY_FIVE, false);
            return luckyFiveInfo != null;
        }

        public static List<string> GetBundleNamesAll() {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(true);
            List<string> bundles = new List<string>();
            bundles.Add(BlackboardQueryUtils.GetMetaBundleName(eventInfo, false));
            bundles.Add(BlackboardQueryUtils.GetMetaBundleName(eventInfo, true));
            bundles.Add(BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, false));
            bundles.Add(BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, true));
            return bundles;
        }

        public static int GetEventID()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(true);
            return eventInfo != null ? eventInfo.id : -1;
        }

        public static bool UpdateMetaGameRemainingTimer(ContextElement areaElement, ContextElement timerElement, long targetTime)
        {
            if(areaElement != null && timerElement != null)
            {
                DateTime targetDate = TimeUtils.ParseTimestampToDateTime(targetTime);

                long currentTimestamp = TimeUtils.GetTimeStamp();
                DateTime currentDate = TimeUtils.ParseTimestampToDateTime(currentTimestamp);

                TimeSpan leftTimespan = targetDate - currentDate;

                if((int)leftTimespan.TotalDays > 99)
                {
                    areaElement.gameObject.SetActive(false);
                }
                else
                {
                    areaElement.gameObject.SetActive(true);

                    MetaContextElementUtils.SetCommonRemainingTimer(
                        timerElement,
                        targetTime,
                        0,
                        "TIME_FORMAT_HHMMSS_TOTALHOUR",
                        "",
                        "",
                        StringTableUtils.GetString(StringTable.StringTableType.Global, "META_GAME_ENDED"),
                        true,
                        null
                    );

                    return true;
                }
            }

            return false;
        }

        public static SymbolInfo GetSymbol(int slotIndex, int reelIndex, BaseReelStrip strip, int index)
        {
            var slotData = MetaSlotMachineContentCustomData.GetSlotData(slotIndex);
            var remap = slotData.mysterySymbolTable;
            var symbolInfo = strip.GetSymbol(index);
            var newSymbolInfo = (SymbolInfo)remap.GetSymbol(reelIndex, symbolInfo);

            if (strip.stripSubSymbolOffset != 0)
            {
                var subSymbolIndex = strip.CalcIndex(index + strip.stripSubSymbolOffset);
                newSymbolInfo.subSymbol.symbol = strip.GetSymbol(subSymbolIndex).subSymbol.symbol;
            }

            return newSymbolInfo;
        }

        public static bool IsMetaGameLevelLocked()
        {
            if (!IsMetaEventLevelLock()) return false;
            return GetMetaUnlockedLevel() > BlackboardUtils.FindVariable<int>(null, "/me/level").value || BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);
        }

        public static bool IsMetaEventLevelLock()
        {
            var enableMetaEventLevelLock = BlackboardUtils.FindVariable<bool>(null, "/values/misc/ENABLE_META_EVENT_LEVEL_LOCK");
            if (enableMetaEventLevelLock == null) return false;
            return enableMetaEventLevelLock.value;
        }

        public static int GetMetaUnlockedLevel()
        {
            // Test Level => 14
            //return 14;
            return BlackboardQueryUtils.GetFeatureMinLevel(LockedFeatureType.META_GAME);
        }

        public static void SetOrientation(Orientation target, GameObject caller = null)
        {
            var current = BlackboardQueryUtils.GetOrientation();
            if (target == current) return;

            BlackboardQueryUtils.SetOrientation(target);

            var fadeObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Change Orientation Fade In Out", null, "Popup Manager", "Fade In Out");

            var bb = fadeObj.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(bb, "caller", caller);
            BlackboardUtils.SetOrCreateValue(bb, "targetOrientation", ScreenOrientation.AutoRotation);

            bool isPortrait = target == Orientation.PORTRAIT;
            BlackboardUtils.SetOrCreateValue(bb, "autorotateToPortrait", isPortrait);
            BlackboardUtils.SetOrCreateValue(bb, "autorotateToPortraitUpsideDown", isPortrait);
            BlackboardUtils.SetOrCreateValue(bb, "autorotateToLandscapeRight", !isPortrait);
            BlackboardUtils.SetOrCreateValue(bb, "autorotateToLandscapeLeft", !isPortrait);

            // manual fade out time. 
            BlackboardUtils.SetOrCreateValue(bb, "isAutoFadeOut", true);
        }

        public static bool IsEpicPassAlwayslLock()
        {
            var enableSeasonPassV2lLock = BlackboardUtils.FindVariable<bool>(null, "/values/misc/ENABLE_SEASON_PASS_V2");
            if (enableSeasonPassV2lLock == null) return false;
            return enableSeasonPassV2lLock.value;
        }
    }
}
