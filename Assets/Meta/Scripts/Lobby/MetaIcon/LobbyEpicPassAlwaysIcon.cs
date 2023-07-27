using BagelCode.ClientModels;
using ParadoxNotion;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode
{
    public class LobbyEpicPassAlwaysIcon : LobbyMetaIcon
    {
        public override void OnStart()
        {
            if (iconObject == null)
                UpdateEpicPassAlwaysIconData();
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {
            if (iconObject != null)
            {
                // On event state
                bool isNext = false;
                if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_START_PASSIVE))
                    isNext = ChackEventInfoType(((EventData<EventInfoType>)eventData).value);
                else if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_REFRESH_PASSIVE))
                    isNext = true;
                else if (eventName.Equals(MetaEventDefine.ON_META_UI_EVENT) && eventData.name.Equals(ON_REFRESH_META_GAME))
                    isNext = true;

                if (isNext)
                {
                    DestroyIconObject();
                    OnStart();
                }
            }
            else
            {
                // Non event state
                if (eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_START_PASSIVE))
                {
                    if (ChackEventInfoType(((EventData<EventInfoType>)eventData).value))
                        OnStart();
                }
            }
        }

        protected override bool CheckActiveEvent()
        {
            return BlackboardQueryUtils.IsEpicPassAlwaysActive();
        }

        protected override void OnInitCheckEventInfoTypeList()
        {
            base.OnInitCheckEventInfoTypeList();

            checkEventInfoTypeList.Add(EventInfoType.SEASON_PASS_V2);
        }

        private void UpdateEpicPassAlwaysIconData()
        {
            bool isActive = CheckActiveEvent();
            if (isActive)
            {
                bundleName = HiddenObjects.HiddenObjects.Defines.COMMON_BUNDLE;
                sharedBundleName = "";
                iconAssetName = LOBBY_BUTTON_ASSET_NAME;

                eventType = EventInfoType.SEASON_PASS_V2;
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

            MakeEventButton("Epic Pass Always Event Button");
        }

    }
}