using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class LobbyBottomShopIconController : MonoBehaviour
    {
        [SerializeField]
        private ShopType shopType;

        private ContextElement root;
        private Blackboard bb;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;
        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

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

        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext(false);

            InitContext();

            isInit = true;
        }

        private void InitContext()
        {
            var eventTimerArea = ContextUtils.FindElement(root, "Event Timer Area", CHILDREN);

            var boosterEventShopBB = BlackboardQueryUtils.GetShopBB(shopType);

            BlackboardUtils.SetOrCreateValue<Blackboard>(bb, "shopBB", boosterEventShopBB);
            MetaContextElementUtils.SetClickable(root, () => {
                if (shopType == ShopType.COIN)
                    EventSender.SendGlobalEvent("OpenShop");
                else if (shopType == ShopType.GEM)
                    EventSender.SendGlobalEvent("OpenGemShop");
            });

            // TODO : Timer Logic Change

            var eventInfo = GetBuyButtonPriorityPassiveEvent();

            MetaContextElementUtils.SetActive(eventTimerArea, eventInfo != null);

            if (eventInfo != null)
            {
                var eventTagObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Event Tag Without Text", eventTimerArea.transform);
                var eventTagController = eventTagObject.GetComponent<EventTagController>();

                eventTagController.Initialize(eventInfo.endTimestamp,
                                                "TIME_FORMAT_HHMMSS_TOTALHOUR",
                                                null,
                                                "Ended",
                                                true,
                                                null,
                                                null
                );

                reserveTimer.SetReserveCallback(eventInfo.endTimestamp,
                    () => 
                    {
                        gameObject.SetActive(false);
                    }
                );
            }

        }

        private void EndEvent()
        {
            var eventTimerArea = ContextUtils.FindElement(root, "Event Timer Area", CHILDREN);
            MetaContextElementUtils.SetActive(eventTimerArea, false);
        }

        private EventInfo GetBuyButtonPriorityPassiveEvent()
        {
            EventInfo eventInfo = null;

            if (shopType == ShopType.COIN)
            {
                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.FREE_COIN_BOOSTER);
                if(eventInfo != null) return eventInfo;

                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COIN_SHOP_EVENT_MULTIPLY);
                if(eventInfo != null && !eventInfo.hideBadge) return eventInfo;

                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY);
                if(eventInfo != null) return eventInfo;

                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL);
                if(eventInfo != null) return eventInfo;
            }
            else if (shopType == ShopType.GEM)
            {
                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.FREE_GEM_BOOSTER);
                if(eventInfo != null) return eventInfo;

                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_SHOP_EVENT_MULTIPLY);
                if(eventInfo != null && !eventInfo.hideBadge) return eventInfo;

                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER_MULTIPLY);
                if (eventInfo != null) return eventInfo;

                eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER);
                if (eventInfo != null) return eventInfo;
            }
            
            return null;
        }

        #if UNITY_EDITOR
        [Button]
        private void TestEndEvent()
        {
            EndEvent();
        }
        #endif
    }
}
