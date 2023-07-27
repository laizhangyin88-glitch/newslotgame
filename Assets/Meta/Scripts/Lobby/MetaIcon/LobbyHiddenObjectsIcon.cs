using BagelCode.ClientModels;
using ParadoxNotion;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode
{
    public class LobbyHiddenObjectsIcon : LobbyMetaIcon
    {
        public override void OnStart()
        {
            if (iconObject == null)
                UpdateHogIconData();
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {
            return;
        }

        protected override bool CheckActiveEvent()
        {
            return BlackboardQueryUtils.IsHiddenObjectsActive();
        }

        private void UpdateHogIconData()
        {
            bool isActive = CheckActiveEvent();
            if (isActive)
            {
                bundleName = HiddenObjects.HiddenObjects.Defines.COMMON_BUNDLE;
                sharedBundleName = "";
                iconAssetName = LOBBY_BUTTON_ASSET_NAME;

                eventType = EventInfoType.UNKNOWN;
                eventId = 0;
                endTimestamp = 0;

                enableShare = false;
                eventName = "";

                bundleList = new List<string>() { HiddenObjects.HiddenObjects.Defines.COMMON_BUNDLE };
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

                bundleList = null;
            }

            MakeEventButton("Hidden Objects Event Button");
        }
    }
}
