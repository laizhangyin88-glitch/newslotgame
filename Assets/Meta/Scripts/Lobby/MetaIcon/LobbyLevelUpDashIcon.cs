using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode
{
    public class LobbyLevelUpDashIcon : LobbyMetaIcon
    {
        public override void OnStart()
        {
            if (iconObject == null)
            {
                UpdateLevelUpDashIconData();
            }
            else
            {
                var eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.LEVEL_UP_DASH_MISSION);
                if (eventInfo == null ||
                    !LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash())
                {
                    LevelUpDash.LevelUpDash.Utils.FinishLevelUpDash();
                    DestroyIconObject();
                    OnStart();
                }
            }
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {
            if (iconObject != null)
            {
                int eventId = (int)eventData.value;
                var targetEventInfo = PassiveEventManager.Instance.GetEventInfoFromID(eventId, true);
                if(eventName.Equals(ON_PASSIVE_EVENT) && eventData.name.Equals(ON_REFRESH_PASSIVE) &&
                    targetEventInfo.type == EventInfoType.LEVEL_UP_DASH_MISSION)
                {
                    var eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.LEVEL_UP_DASH_MISSION);
                    if(eventInfo == null ||
                        !LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash())
                    {
                        LevelUpDash.LevelUpDash.Utils.FinishLevelUpDash();
                        DestroyIconObject();
                        OnStart();
                    }
                }
            }
            return;
        }

        protected override bool CheckActiveEvent()
        {
            return LevelUpDash.LevelUpDash.Utils.IsActiveLevelUpDash();
        }

        private void UpdateLevelUpDashIconData()
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

            MakeEventButton("Level Up Dash Event Button");
        }
    }
}
