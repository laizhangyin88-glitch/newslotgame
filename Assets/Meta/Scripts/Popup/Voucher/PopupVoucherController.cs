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
    public class PopupVoucherController : MonoBehaviour
    {
        private Blackboard rootBB;
        private Animator rootAnimator;
        private ContextElement rootElement;

        private bool isInit = false;

        private ContextElement multiplierElement;
        private ContextElement eventMultiplierElement;
        private ContextElement beforeCoinElement;
        private ContextElement eventCoinElement;

        private ContextElement normalElement;
        private ContextElement normalCoinTextElement;

        private ContextElement buyButtonElement;
        private ContextElement buyButtonTextElement;
        private ContextElement buttonSaleTagElement;
        private ContextElement buttonSaleTagTextElement;
        

        private void InitProperty()
        {
            if(isInit) return;

            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            var titleTextElement = ContextUtils.FindElement(rootElement, "Title Area/Text", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetTextGlobal(titleTextElement, "POPUP_VOUCHER_TITLE");

            normalElement = ContextUtils.FindElement(rootElement, "Normal", ContextSearchingType.ChildrenSearch);
            normalCoinTextElement = ContextUtils.FindElement(normalElement, "Text", ContextSearchingType.ChildrenSearch);

            multiplierElement = ContextUtils.FindElement(rootElement, "Multiplier", ContextSearchingType.ChildrenSearch);
            beforeCoinElement = ContextUtils.FindElement(multiplierElement, "Text", ContextSearchingType.ChildrenSearch);
            eventMultiplierElement = ContextUtils.FindElement(multiplierElement, "Text/Text", ContextSearchingType.FullNameSearch);
            eventCoinElement = ContextUtils.FindElement(multiplierElement, "Text Event", ContextSearchingType.ChildrenSearch);
            

            buyButtonElement = ContextUtils.FindElement(rootElement, "Button Redeem", ContextSearchingType.ChildrenSearch);
            buyButtonTextElement = ContextUtils.FindElement(buyButtonElement, "Text", ContextSearchingType.ChildrenSearch);

            buttonSaleTagElement = ContextUtils.FindElement(buyButtonElement, "Sale Tag", ContextSearchingType.ChildrenSearch);
            buttonSaleTagTextElement = ContextUtils.FindElement(buttonSaleTagElement, "Text", ContextSearchingType.ChildrenSearch);

            var closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                "OnClose",
                rootElement,
                null
            );


            MetaContextElementUtils.SetClickable(
                buyButtonElement,
                "OnBuyItem",
                rootElement,
                null
            );

            isInit = true;
        }

        public void UpdateValues()
        {
            InitProperty();

            Blackboard rewardInfoBB = rootBB.GetValue<Blackboard>("_rewardInfo");
            Blackboard productBB = rewardInfoBB.GetValue<Blackboard>("product");

            BlackboardUtils.SetOrCreateValue<Blackboard>( rootBB, "product", productBB);

            List<Blackboard> itemList = productBB.GetValue<List<Blackboard>>("itemList");
            Blackboard item = itemList[0];

            double price = productBB.GetValue<double>("originalPrice");
            double origPrice = productBB.GetValue<double>("price");

            int meTier = TierUtils.GetMeTier();

            //////////////////////
            long baseCredit = item.GetValue<long>("baseCredit");
            long rp         = item.GetValue<long>("rp");

            long additionalCreditMultiplierNumerator = item.GetValue<long>("additionalCreditMultiplierNumerator");

            long baseCoin         = TierUtils.GetTierFractionCoin(baseCredit, meTier);
            baseCoin              = LevelUtils.GetLevelMultiplierNumeratorValue(baseCoin, "coin");
            long totalItemCoins   = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseCoin, additionalCreditMultiplierNumerator);

            long totalEventCoins  = totalItemCoins;
            //////////////////////

            var eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.VOUCHER_SHOP_EVENT_MULTIPLY);
            if(eventInfo != null)
            {
                long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);

                totalEventCoins = NumberUtils.GetMultiplierNumeratorValue(totalItemCoins, eventMultiplierNumerator);

                MetaContextElementUtils.SetTextGlobal(beforeCoinElement, "POPUP_VOUCHER_TEXT_BEFORE", totalItemCoins);
                MetaContextElementUtils.SetTextGlobal(eventMultiplierElement, "POPUP_VOUCHER_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator));

                MetaContextElementUtils.SetTextGlobal(eventCoinElement, "POPUP_VOUCHER_TEXT_AFTER", totalEventCoins, rp);
            }
            else
            {
                MetaContextElementUtils.SetTextGlobal(normalCoinTextElement, "POPUP_VOUCHER_TEXT_AFTER", totalEventCoins, rp);
            }

            BlackboardUtils.SetOrCreateValue<int>( rootBB, "_voucherShopMultiplyEventID", eventInfo == null ? 0 : eventInfo.id);

            if(price != origPrice)
            {
                double salePercent = (((origPrice/price)*-1.0) + 1);
                MetaContextElementUtils.SetTextGlobal(buttonSaleTagTextElement, "POPUP_VOUCHER_TEXT_SALE_PRICE", salePercent);
            }

            MetaContextElementUtils.SetTextGlobal(buyButtonTextElement, "POPUP_VOUCHER_REEDEM", price);

            normalElement.gameObject.SetActive(price != origPrice);
            buttonSaleTagElement.gameObject.SetActive(price != origPrice);

            multiplierElement.gameObject.SetActive(eventInfo != null);
            normalElement.gameObject.SetActive(eventInfo == null);
            
            rootAnimator.SetBool("Active", true);
        }
    }
}
