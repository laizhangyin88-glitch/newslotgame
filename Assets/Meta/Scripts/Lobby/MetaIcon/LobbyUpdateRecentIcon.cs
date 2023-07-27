using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class LobbyUpdateRecentIcon : LobbyMetaIcon
    {
        public override void OnStart()
        {
            CheckRecentUpdate();
        }

        public override void OnEventRecv(string eventName, EventData eventData) { }

        private void CheckRecentUpdate()
        {
            // Admin setting - Kill switch
            bool isUpdateActive = BlackboardUtils.FindVariable<bool>(null, "/values/misc/ENABLE_NEED_UPDATE_META_ICON")?.value ?? false;
            if (isUpdateActive == true)
            {
                int clientVersion = ApplicationSettings.GetClientVersionNumber();
                int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;
                isUpdateActive = clientVersion < recentVersion;
            }

            if (isUpdateActive == true)
            {
                if (iconObject == null)
                    MakeEventButton("Need Update Button");
            }
            else
                DestroyIconObject();
        }

        protected override bool CheckActiveEvent()
        {
            return BlackboardQueryUtils.GetMetaGameEventInfo() != null;
        }
    }
}