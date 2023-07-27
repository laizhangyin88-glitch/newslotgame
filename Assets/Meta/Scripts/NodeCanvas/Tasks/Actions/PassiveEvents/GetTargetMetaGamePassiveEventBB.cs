using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/PassiveEvents")]
    public class GetTargetMetaGamePassiveEventBB : ActionTask<Blackboard>
    {
        public BBParameter<bool> isInGame;
        public BBParameter<bool> isContents = false;
        public BBParameter<EventInfoType> type = EventInfoType.UNKNOWN;

        public BBParameter<string> saveAsBundleName;
        public BBParameter<string> saveAsSharedBundleName;
        public BBParameter<string> saveAsIconAssetName;

        public BBParameter<int> saveAsEventID;
        public BBParameter<long> saveAsEndTimestamp;

        public BBParameter<bool> saveAsEnableShare;
        public BBParameter<string> saveAsEventName;

        public BBParameter<List<string>> saveBundles;
        public BBParameter<bool> isOtherMetaGame;

        private const string LOBBY_BUTTON_ASSET_NAME = "Lobby Button Scene";
        private const string INGAME_BUTTON_ASSET_NAME = "In Game Button Scene";

        protected override string info
        {
            get { return string.Format("Get Meta Game Info"); }
        }

        protected override void OnExecute()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(type.value);
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (eventInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                saveAsBundleName.value = BlackboardQueryUtils.GetMetaBundleName(eventInfo, isContents.value);
                saveAsSharedBundleName.value = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, isContents.value);
                saveAsEventID.value = eventInfo.id;
                saveAsEndTimestamp.value = eventInfo.endTimestamp;

                saveAsIconAssetName.value = isInGame.value ? INGAME_BUTTON_ASSET_NAME : LOBBY_BUTTON_ASSET_NAME;

                saveAsEnableShare.value = BlackboardQueryUtils.IsShareEnabled(eventInfo);
                saveAsEventName.value = BlackboardQueryUtils.GetMetaGameEventName(eventInfo);

                saveBundles.value = new List<string>();
                saveBundles.value.Add(saveAsBundleName.value);
                if (!string.IsNullOrEmpty(saveAsSharedBundleName.value))
                    saveBundles.value.Add(saveAsSharedBundleName.value);

                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(),
                    (!isOtherMetaGame.value) ? "metaGameEventID" : "otherMetaGameEventID",
                    eventInfo.id);
            }
            else
            {
                UnityEngine.Debug.LogWarning(string.Format(
                    "GetTargetMetaGamePassiveEventBB failure. " +
                    "eventInfo != null:{0}, isLockedFeature:{1}, IsMetaEventLevelLock:{2}",
                    eventInfo != null, isLockedFeature, MetaGameUtils.IsMetaEventLevelLock()));

                saveAsBundleName.value = "";
                saveAsSharedBundleName.value = "";
                saveAsIconAssetName.value = "";
                saveAsEventID.value = 0;
                saveAsEndTimestamp.value = 0;
                saveAsEnableShare.value = false;
                saveAsEventName.value = "";
                saveBundles.value = null;

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(),
                    (!isOtherMetaGame.value) ? "metaGameEventID" : "otherMetaGameEventID", 0);
            }

            EndAction(true);
        }
    }
}
