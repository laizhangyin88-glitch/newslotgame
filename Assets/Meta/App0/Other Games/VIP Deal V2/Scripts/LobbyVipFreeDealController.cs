using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class LobbyVipFreeDealController : EventMonoBehaviour
    {
        private ContextElement root;
        private EventTagController eventTagController;

        private bool isInit = false;

        private SimpleReserveTimer _reserveTimer;
        private SimpleReserveTimer reserveTimer
        {
            get
            {
                if (_reserveTimer == null)
                {
                    _reserveTimer = GetComponent<SimpleReserveTimer>();
                    if (_reserveTimer == null)
                        _reserveTimer = gameObject.AddComponent<SimpleReserveTimer>();
                }
                return _reserveTimer;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateVariables();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            reserveTimer.Stop();
        }

        private void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            root.UpdateContext(true);

            var eventTimerArea = ContextUtils.FindElement(root, "Event Timer Area", ContextSearchingType.ChildrenSearch);

            var eventTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag Without Text", eventTimerArea.transform);
            eventTagController = eventTagObject.GetComponent<EventTagController>();

            MetaContextElementUtils.SetClickable(root, OnClick);

            isInit = true;
        }

        private void OnClick()
        {
            MetaContextElementUtils.SetBooleanProperty(root, false); // disable click

            AEUtils.SendAE("client_click_vip_deal", ("type", "Lobby"));

            GSManager.Instance.GetHandler(VipDealV2.Defines.SOUND_LOBBY_DEAL_BUTTON_CLICK);
            EventSender.SendGlobalMetaEvent(VipDealV2.Events.ON_ENTER_VIP_DEAL);
        }

        public void UpdateVariables()
        {
            InitProperty();

            var vipDealInfo = VipDealV2.Utils.GetActiveInfo();
            if (vipDealInfo != null)
            {
                var endTimestamp = vipDealInfo.GetValue<long>("endTimestamp");

                eventTagController.Initialize(endTimestamp,
                                                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                                                null,
                                                "Ended",
                                                true,
                                                null,
                                                null
                );

                reserveTimer.SetReserveCallback(endTimestamp,
                    () =>
                    {
                        gameObject.SetActive(false);
                    }
                );
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
