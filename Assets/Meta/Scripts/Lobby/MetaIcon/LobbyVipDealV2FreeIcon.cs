using ParadoxNotion;

namespace BagelCode
{
    public class LobbyVipDealV2FreeIcon : LobbyMetaIcon
    {
        public override void OnStart()
        {
            if (iconObject == null)
                CheckVipDealEvent();
        }

        public override void OnEventRecv(string eventName, EventData eventData)
        {

        }

        protected override bool CheckActiveEvent()
        {
            return !VipDealV2.Utils.IsTierLock() &&
                VipDealV2.Utils.GetActiveInfo() != null &&
                VipDealV2.Utils.GetIsViewed(false) == false; // free isn't viewed
        }

        private void CheckVipDealEvent()
        {
            bool isActive = CheckActiveEvent();

            if (isActive)
                MakeEventButton("Lobby Button VIP Free Deal Area");
        }
    }
}
