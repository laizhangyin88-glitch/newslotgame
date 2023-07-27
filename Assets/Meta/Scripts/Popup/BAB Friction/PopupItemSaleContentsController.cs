using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupItemSaleContentsController : PopupShopItemControllerBase
    {
        private List<Blackboard> productList;
        private Blackboard productBB;
        private bool isEvent;

        private string baseItemText;
        private string eventTagText;
        private string buttonEventText;

        private bool isInit = false;
        private int eventID = -1;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private Blackboard rootBlackboard;
        private ContextElement rootElement;

        private ContextElement itemTextElement;
        private ContextElement rpTextElement;
        private ContextElement buyButtonElement;
        private ContextElement buyButtonTextElement;
        private ContextElement eventTag;

        public void InitProperty(List<Blackboard> productList)
        {
            if(isInit) return;

            this.productList = productList;

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            itemTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            rpTextElement = ContextUtils.FindElement(rootElement, "Vip Point", ContextSearchingType.ChildrenSearch);
            eventTag = ContextUtils.FindElement(rootElement, "Event Tag", ContextSearchingType.ChildrenSearch);
            buyButtonElement = ContextUtils.FindElement(rootElement, "Button Area/Button Buy", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetClickable(
                buyButtonElement,
                "OnBuyItem",
                rootElement,
                null
            );

            productBB = productList[0];

            BlackboardUtils.SetOrCreateValue<Blackboard>( rootBlackboard, "product", productBB);

            isInit = true;
        }

        // Call from ShopController, PassiveBehavuour.
        public void UpdateValues()
        {
            if(!isInit) return;

            UpdateStaticValues();

            if(isEvent)
            {
                eventTag.gameObject.SetActive(true);
                UpdateEventTagValues();
            }
            else
            {
                eventTag.gameObject.SetActive(false);
                UpdateEventValues();
            }
        }

        private void UpdateStaticValues()
        {
            int tier = TierUtils.GetMeTier();
            double itemPrice = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB, "price").value);

            var gemItemBB = BlackboardQueryUtils.GetItemFromProduct(productBB, ItemType.GEM);

            long baseGem = gemItemBB.GetValue<long>("gem");
            long rp  = gemItemBB.GetValue<long>("rp");

            long totalBaseGem = TierUtils.GetTierFractionCoin(baseGem, tier);

            var eventInfo = GetPassiveEventMultiplierNumerator();
            
            eventID = eventInfo == null ? -1 : eventInfo.id;
            isEvent = eventInfo != null;

            BlackboardUtils.SetOrCreateValue<int>( rootBlackboard, "_gemBabPromotionShopMultiplyEventID", eventInfo == null ? 0 : eventInfo.id);

            buyButtonTextElement = ContextUtils.FindElement(buyButtonElement, "Text 01", ContextSearchingType.ChildrenSearch);
            buyButtonTextElement.gameObject.SetActive(false);

            if(isEvent)
            {
                long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                long totalGem = NumberUtils.GetMultiplierNumeratorValue(totalBaseGem, eventMultiplierNumerator);

                switch(eventInfo.type)
                {
                    case EventInfoType.GEM_BAB_PROMOTION_SHOP_DISCOUNT_EVENT_MULTIPLY:
                        {
                            baseItemText = StringTableUtils.GetString(tableType, "ITEM_SALE_TOTAL_GEMS", totalGem);
                            buyButtonTextElement.gameObject.SetActive(true);

                            double itemDiscountPrice = NumberUtils.GetDiscountNumeratorValue(itemPrice, eventMultiplierNumerator);
                            MetaContextElementUtils.SimpleSetText(buyButtonElement, "Text 01", StringTableUtils.GetString(tableType, "ITEM_SALE_BUY_BUTTON_DISCOUNT", itemDiscountPrice));
                            buttonEventText = StringTableUtils.GetString(tableType, "ITEM_SALE_BUY_BUTTON", itemPrice);

                            long discountValue = NumberUtils.GetDiscountPercentValue(eventMultiplierNumerator);
                            eventTagText = StringTableUtils.GetString(tableType, "ITEM_SALE_EVENT_DISCOUNT", discountValue);
                        }
                        break;
                    default:
                        {
                            baseItemText = StringTableUtils.GetString(tableType, "ITEM_SALE_EVENT_TOTAL_GEMS", totalGem, totalBaseGem);
                            buttonEventText = StringTableUtils.GetString(tableType, "ITEM_SALE_BUY_BUTTON", itemPrice);

                            if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM_BAB_PROMOTION) )
                            {
                                long viewAddPercent = PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo);
                                eventTagText = StringTableUtils.GetString(tableType, "ITEM_SALE_EVENT_PERCENT", viewAddPercent);
                            }
                            else
                            {
                                double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);
                                eventTagText = StringTableUtils.GetString(tableType, "ITEM_SALE_EVENT_MULTIPLIER", eventMultiplier);
                            }
                        }
                        break;

                }
            }
            else
            {
                baseItemText = StringTableUtils.GetString(tableType, "ITEM_SALE_TOTAL_GEMS", totalBaseGem);
                eventTagText = "";
                buttonEventText = StringTableUtils.GetString(tableType, "ITEM_SALE_BUY_BUTTON", itemPrice);
            }

            MetaContextElementUtils.SetText(itemTextElement, baseItemText);
            MetaContextElementUtils.SimpleSetText(eventTag, "Text", eventTagText);
            MetaContextElementUtils.SetText(rpTextElement, StringTableUtils.GetString(tableType, "SHOP_VIP_CREDIT", rp));
            MetaContextElementUtils.SimpleSetText(buyButtonElement, "Text 02", buttonEventText);
        }

        private void UpdateEventTagValues()
        {
            MetaContextElementUtils.SimpleSetText(eventTag, "Text", eventTagText);
        }

        private void UpdateEventValues()
        {
            MetaContextElementUtils.SetText(itemTextElement, baseItemText);
            MetaContextElementUtils.SimpleSetText(eventTag, "Text", eventTagText);
        }
        
        private EventInfo GetPassiveEventMultiplierNumerator()
        {
            EventInfo multiplyEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BAB_PROMOTION_SHOP_EVENT_MULTIPLY);
            if(multiplyEventInfo != null)
                return multiplyEventInfo;

            EventInfo discountEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BAB_PROMOTION_SHOP_DISCOUNT_EVENT_MULTIPLY);
            if(discountEventInfo != null)
                return discountEventInfo;
            
            return null;
        }
        
        public void StartPassiveEvent()
        {
            var eventInfo = GetPassiveEventMultiplierNumerator();
            if(eventInfo != null && eventInfo.id != eventID)
            {
                eventID = eventInfo.id;
                UpdateValues();
            }
        }
    }
}
