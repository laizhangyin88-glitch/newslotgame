using ParadoxNotion;

namespace BagelCode
{
    public class LobbyVipDealV2PayIcon : LobbyMetaIcon
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
                VipDealV2.Utils.GetIsViewed(false) == true; // free is viewed
        }

        private void CheckVipDealEvent()
        {
            bool isAcitve = CheckActiveEvent();

            if (isAcitve)
                MakeEventButton("Lobby Button VIP Pay Deal Area");
        }
    }
}
