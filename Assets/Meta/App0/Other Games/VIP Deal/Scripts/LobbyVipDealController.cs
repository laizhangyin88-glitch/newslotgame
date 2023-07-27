using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    public class LobbyVipDealController : MonoBehaviour
    {
        private ContextElement rootElement;
        private EventTagController eventTagController;

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

        private bool isInit = false;

        private void OnEnable()
        {
            UpdateVariables();
        }

        private void OnDisable()
        {
            reserveTimer.Stop();
        }

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext(true);

            var eventTimerArea = ContextUtils.FindElement(rootElement, "Event Timer Area", ContextSearchingType.ChildrenSearch); 

            var eventTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag Without Text", eventTimerArea.transform);
            eventTagController = eventTagObject.GetComponent<EventTagController>();

            MetaContextElementUtils.SetClickable(
                rootElement,
                "EnterVipDeal",
                rootElement,
                null
            );

            isInit = true;
        }

        public void UpdateVariables()
        {
            InitProperty();
            
            var vipDealInfo = BlackboardQueryUtils.GetActiveVipDealInfo();
            if(vipDealInfo != null)
            {
                var endTimestamp = vipDealInfo.GetValue<long>("endTimestamp");

                eventTagController.Initialize(  endTimestamp,
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