using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;
using BagelCode.Scratcher;

namespace BagelCode
{
    public class PopupShopWheelIconController : MonoBehaviour
    {
        private ContextElement anchorElement;
        private Blackboard rootBlackboard;
        private Animator anchorAnimator;

        private ContextElement flippingTextElement;

        private bool isInit = false;

        private Blackboard productBB;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const float FLIPPING_INTERVAL = 2f;

        public void InitProperty(Blackboard wheelProductBB)
        {
            productBB = wheelProductBB;

            if(isInit) return;

            var rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            rootBlackboard = gameObject.GetComponent<Blackboard>();

            anchorElement = ContextUtils.FindElement(rootElement, "Image Wheel Shop Item", ContextSearchingType.ChildrenSearch);
            anchorAnimator = anchorElement.gameObject.GetComponent<Animator>();

            flippingTextElement = ContextUtils.FindElement(rootElement, "Image Wheel Shop Item/Flipping Text", ContextSearchingType.FullNameSearch);

            BlackboardUtils.SetOrCreateValue<int>(rootBlackboard, "_eventID", 0);

            isInit = true;
        }

        public void UpdateEventStatus()
        {
            BlackboardUtils.SetOrCreateValue<int>(rootBlackboard, "_eventID", 0);

            EventInfo eventInfo = GetPassiveEventInfo();

            string defaultText = null;
            List<string> textList = new List<string>();
            bool isFree = false;
            
            if(eventInfo != null)
            {
                switch(eventInfo.type)
                {
                    case EventInfoType.FREE_COIN_BOOSTER:
                        defaultText = StringTableUtils.GetString(tableType, "SHOP_ITEM_WHEEL_ICON_FREE_EVENT");
                        isFree = true;
                        break;
                    case EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY:
                        defaultText = StringTableUtils.GetString(tableType, "SHOP_ITEM_WHEEL_TEXT_ALL_MULTIPLIER_EVENT", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                        break;
                    case EventInfoType.CREDIT_MULTIPLIER_WHEEL:
                        defaultText = StringTableUtils.GetString(tableType, "SHOP_ITEM_WHEEL_TEXT_MIN_EVENT", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                        break;
                }

                if(!string.IsNullOrEmpty(defaultText))
                {
                    textList.Add(defaultText);
                    BlackboardUtils.SetOrCreateValue<int>(rootBlackboard, "_eventID", eventInfo.id);
                }
            }
            
            if(!isFree)
            {
                var salePercent = BlackboardQueryUtils.GetProductSalePercent(productBB);

                if(salePercent > 0)
                {
                    textList.Add(StringTableUtils.GetString(tableType, "SHOP_ITEM_WHEEL_SALE_EVENT", salePercent));
                    textList.Add(StringTableUtils.GetString(tableType, "SHOP_ITEM_WHEEL_ICON_EVENT"));
                }
            }

            anchorAnimator.SetBool("IsEvent", textList.Count > 0);
            if(textList.Count > 0)
            {
                MetaContextElementUtils.SetFlippingText(flippingTextElement, defaultText, textList, true, FLIPPING_INTERVAL);
            }
        }

        private EventInfo GetPassiveEventInfo()
        {
            EventInfo freeEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.FREE_COIN_BOOSTER);
            if(freeEventInfo != null) return freeEventInfo;

            EventInfo allMultiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY);
            if(allMultiplierEventInfo != null) return allMultiplierEventInfo;

            EventInfo withoutEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL);
            if(withoutEventInfo != null) return withoutEventInfo;

            return null;
        }
    }

}

