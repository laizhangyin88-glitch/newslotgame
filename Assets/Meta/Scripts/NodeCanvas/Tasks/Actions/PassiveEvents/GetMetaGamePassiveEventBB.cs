using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/PassiveEvents")]
    public class GetMetaGamePassiveEventBB : ActionTask<Blackboard>
    {
        public BBParameter<bool> isInGame;
        public BBParameter<bool> isContents = false;
        public BBParameter<bool> allowInactive;

        public BBParameter<string> saveAsBundleName;
        public BBParameter<string> saveAsSharedBundleName;
        public BBParameter<string> saveAsIconAssetName;

        public BBParameter<EventInfoType> saveAsEventType;
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
            EventInfo eventInfo = (!isOtherMetaGame.value) ? BlackboardQueryUtils.GetMetaGameEventInfo(allowInactive.value) : BlackboardQueryUtils.GetOtherMetaGameEventInfo(allowInactive.value);
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            if (eventInfo != null && (!isLockedFeature || MetaGameUtils.IsMetaEventLevelLock()))
            {
                saveAsBundleName.value = BlackboardQueryUtils.GetMetaBundleName(eventInfo, isContents.value);
                saveAsSharedBundleName.value = BlackboardQueryUtils.GetSharedMetaBundleName(eventInfo, isContents.value);
                saveAsEventType.value = eventInfo.type;
                saveAsEventID.value = eventInfo.id;
                saveAsEndTimestamp.value = eventInfo.endTimestamp;

                saveAsIconAssetName.value = isInGame.value ? INGAME_BUTTON_ASSET_NAME : LOBBY_BUTTON_ASSET_NAME;

                saveAsEnableShare.value = BlackboardQueryUtils.IsShareEnabled(eventInfo);
                saveAsEventName.value = ((!isOtherMetaGame.value) ? BlackboardQueryUtils.GetMetaGameEventName() : BlackboardQueryUtils.GetOtherMetaGameEventName()).ToUpper();

                saveBundles.value = new List<string>();
                saveBundles.value.Add(saveAsBundleName.value);
                if (!string.IsNullOrEmpty(saveAsSharedBundleName.value))
                    saveBundles.value.Add(saveAsSharedBundleName.value);
                List<string> extraBundles = BlackboardQueryUtils.GetMetaExtraBundlesName(eventInfo, isContents.value);
                if (extraBundles != null)
                    saveBundles.value.AddRange(extraBundles);

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), (!isOtherMetaGame.value) ? "metaGameEventID" : "otherMetaGameEventID", eventInfo.id);
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
    }
}