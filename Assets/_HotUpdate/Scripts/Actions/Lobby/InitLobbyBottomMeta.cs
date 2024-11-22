using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;
using ParadoxNotion.Services;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class InitLobbyBottomMeta : ActionTask<Blackboard>
    {
        public BBParameter<bool> isInGame;
        public BBParameter<bool> isContents = false;
        public BBParameter<bool> allowInactive;

        public BBParameter<string> saveAsBundleName;
        public BBParameter<string> saveAsSharedBundleName;
        public BBParameter<string> saveAsIconAssetName;

        public BBParameter<BottomIconType> targetType;
        public BBParameter<EventInfoType> saveAsEventType;
        public BBParameter<int> saveAsEventID;
        public BBParameter<long> saveAsEndTimestamp;

        public BBParameter<bool> saveAsEnableShare;
        public BBParameter<string> saveAsEventName;

        public BBParameter<List<string>> saveBundles;
        public BBParameter<bool> isOtherMetaGame;

        private const string LOBBY_BUTTON_ASSET_NAME = "Lobby Bottom Button Scene";
        private const string INGAME_BUTTON_ASSET_NAME = "In Game Button Scene";

        protected override string info
        {
            get { return string.Format("Get Meta Game Info"); }
        }

        protected override void OnExecute()
        {
            bool forcedTrigger = false;
            EventInfo eventInfo = null;
            switch (targetType.value)
            {
                case BottomIconType.COLLECTING_GAME:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, allowInactive.value);
                    break;
                case BottomIconType.LUCKY_FIVE:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.LUCKY_FIVE, allowInactive.value);
                    break;
                case BottomIconType.EPIC_PASS:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS, allowInactive.value);
                    break;
                case BottomIconType.GOLD_TOWER:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.GEM_JACKPOT, allowInactive.value);
                    break;
                case BottomIconType.BOSS_RAIDERS:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.BOSS_RAIDERS, allowInactive.value);
                    break;
                case BottomIconType.BUILD_DREAM:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.BUILD_DREAM_SEASON, allowInactive.value);
                    break;
                case BottomIconType.CLUB_ARENA:
                    eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.CLUB_ARENA, allowInactive.value);
                    break;
                case BottomIconType.VIP_LOUNGE:
                case BottomIconType.HOG:
                    forcedTrigger = true;
                    break;
            }
            // EventInfo eventInfo = (!isOtherMetaGame.value) ? BlackboardQueryUtils.GetMetaGameEventInfo(allowInactive.value) : BlackboardQueryUtils.GetOtherMetaGameEventInfo(allowInactive.value);

            if (eventInfo != null)
            {
                saveAsBundleName.value = BlackboardQueryUtils.GetMetaBundleName(eventInfo, isContents.value);
                saveAsSharedBundleName.value = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, isContents.value);
                saveAsEventType.value = eventInfo.type;
                saveAsEventID.value = eventInfo.id;
                saveAsEndTimestamp.value = eventInfo.endTimestamp;

                saveAsIconAssetName.value = LOBBY_BUTTON_ASSET_NAME;

                saveAsEnableShare.value = BlackboardQueryUtils.IsShareEnabled(eventInfo);
                
                var seasonIndex = 0;
                switch (eventInfo.type)
                {
                    case EventInfoType.COLLECTING_GAME:
                        seasonIndex = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;
                        break;
                    case EventInfoType.BOSS_RAIDERS:
                        seasonIndex = ((EventDataBossRaiders)eventInfo.constraints).themeId;
                        break;

                }
                saveAsEventName.value = ((!isOtherMetaGame.value) ? BlackboardQueryUtils.GetMetaGameEventName(eventInfo.type, seasonIndex) : BlackboardQueryUtils.GetOtherMetaGameEventName()).ToUpper();

                saveBundles.value = new List<string>();
                saveBundles.value.Add(saveAsBundleName.value);
                if (!string.IsNullOrEmpty(saveAsSharedBundleName.value))
                    saveBundles.value.Add(saveAsSharedBundleName.value);
                List<string> extraBundles = BlackboardQueryUtils.GetMetaExtraBundlesName(eventInfo, isContents.value);
                if (extraBundles != null)
                    saveBundles.value.AddRange(extraBundles);

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), (!isOtherMetaGame.value) ? "metaGameEventID" : "otherMetaGameEventID", eventInfo.id);
            }
            else if (forcedTrigger)
            {
                ForcedMetaLoad();
            }
            else
            {
                saveAsBundleName.value = "";
                saveAsSharedBundleName.value = "";
                saveAsIconAssetName.value = "";
                saveAsEventType.value = EventInfoType.UNKNOWN;
                saveAsEventID.value = 0;
                saveAsEndTimestamp.value = 0;
                saveAsEnableShare.value = false;
                saveAsEventName.value = "";
                saveBundles.value = null;

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), (!isOtherMetaGame.value) ? "metaGameEventID" : "otherMetaGameEventID", 0);
            }

            EndAction(true);
        }

        private void ForcedMetaLoad()
        {
            switch (targetType.value)
            {
                case BottomIconType.VIP_LOUNGE:
                    saveAsBundleName.value = "mgviploungecommon";
                    saveAsSharedBundleName.value = "";
                    saveAsIconAssetName.value = LOBBY_BUTTON_ASSET_NAME;
                    saveAsEventType.value = EventInfoType.UNKNOWN;
                    saveAsEventID.value = -1;
                    saveAsEndTimestamp.value = 0;
                    saveAsEnableShare.value = false;
                    saveAsEventName.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_LOUNGE_NAME");
                    saveBundles.value = new List<string>();
                    saveBundles.value.Add(saveAsBundleName.value);
                    break;

                case BottomIconType.HOG:
                    saveAsBundleName.value = "mghiddenobjectscommon";
                    saveAsSharedBundleName.value = "";
                    saveAsIconAssetName.value = LOBBY_BUTTON_ASSET_NAME;
                    saveAsEventType.value = EventInfoType.UNKNOWN;
                    saveAsEventID.value = -1;
                    saveAsEndTimestamp.value = 0;
                    saveAsEnableShare.value = false;
                    saveAsEventName.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_EVENT_NAME_HIDDEN_OBEJCTS");
                    saveBundles.value = new List<string>();
                    saveBundles.value.Add(saveAsBundleName.value);
                    break;
            }
        }
    }
}
