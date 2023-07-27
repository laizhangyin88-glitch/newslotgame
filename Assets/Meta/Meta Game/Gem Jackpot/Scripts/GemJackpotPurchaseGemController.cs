using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.GemJackpot
{
    public class GemJackpotPurchaseGemController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;

        private ContextElement caller = null;

        private ContextElement purchaseGemTextElement;
        private ContextElement purchaseButton;
        private ContextElement closeButton;

        private bool isInit = false;

        private readonly long purchaseCheckMultiply = 3;

        protected void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(true);
            rootBB = gameObject.GetComponent<Blackboard>();

            purchaseGemTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement buttonAreaElement = ContextUtils.FindElement(rootElement, "Button Area", ContextSearchingType.ChildrenSearch);
            ContextElement buttonCloseAreaElement = ContextUtils.FindElement(rootElement, "Button Close Area", ContextSearchingType.ChildrenSearch);
            buttonAreaElement.UpdateContext(true);
            buttonCloseAreaElement.UpdateContext(true);

            purchaseButton = ContextUtils.FindElement(buttonAreaElement, "Button Purchase", ContextSearchingType.ChildrenSearch);
            closeButton = ContextUtils.FindElement(buttonCloseAreaElement, "Button Close", ContextSearchingType.ChildrenSearch);

            ContextElement buttonTextElement = ContextUtils.FindElement(purchaseButton, "Text", ContextSearchingType.ChildrenSearch);
            ContextUtils.SetText(buttonTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_PURCHASE_BUTTON_TEXT"));

            MetaContextElementUtils.SetClickable(
                purchaseButton,
                "OnPurchaseGem",
                rootElement,
                null);

            MetaContextElementUtils.SetClickable(
                closeButton,
                "OnPurchaseClose",
                rootElement,
                null);

            SetPurchaseGem();

            isInit = true;
        }

        public void OnInit(ContextElement _caller)
        {
            caller = _caller;
            InitProperty();
        }

        public void SetText(long gem, double price)
        {
            ContextUtils.SetText(purchaseGemTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "GEM_JACKPOT_PURCHASE_DESC_TEXT", gem, price));
        }

        private void SetPurchaseGem()
        {
            Blackboard purchaseBB = GetPurchaseGemProductBB();
            Blackboard itemBB = BlackboardQueryUtils.GetItemFromProduct(purchaseBB, ItemType.GEM);
            long gem = BlackboardUtils.FindValue<long>(itemBB, "gem");
            long totalGem = TierUtils.GetTierFractionCoin(gem, TierUtils.GetMeTier());
            double price = BlackboardUtils.FindValue<double>(purchaseBB, "price");

            BlackboardUtils.SetOrCreateValue(rootBB, "product", purchaseBB);
            BlackboardUtils.SetOrCreateValue(rootBB, "itemBB", itemBB);

            SetText(totalGem, price);
        }

        private Blackboard GetPurchaseGemProductBB()
        {
            List<Blackboard> productGroups = BlackboardQueryUtils.GetShopProductGroups(ShopType.GEM_JACKPOT);

            Blackboard returnBB = null;

            //long nowGem = BlackboardUtils.FindValue<long>(MainBlackboard.Get(), "me/gem");
            long checkSpinGem = GemJackpotUtils.GemForSpinSale;
            double gemjackpotFPP = GemJackpotUtils.FPP;

            if (gemjackpotFPP != 0)
            {
                for (int i = 0; i < productGroups.Count; ++i)
                {
                    List<Blackboard> productList = BlackboardQueryUtils.GetProductList(productGroups[i]);

                    double price = productList[0].GetValue<double>("price");

                    if (price == gemjackpotFPP) return productList[0];
                    else if (i + 1 < productGroups.Count)
                    {
                        List<Blackboard> nextProductList = BlackboardQueryUtils.GetProductList(productGroups[i + 1]);
                        double nextPrice = nextProductList[0].GetValue<double>("price");
                        if (price < gemjackpotFPP && nextPrice > gemjackpotFPP) return nextProductList[0];
                        else if (i == 0 && price > gemjackpotFPP) return productList[0];
                    }
                    else if (i + 1 == productGroups.Count) return productList[0];
                }
            }

            for (int i = 0; i < productGroups.Count; ++i)
            {
                List<Blackboard> productList = BlackboardQueryUtils.GetProductList(productGroups[i]);
                Blackboard itemList = BlackboardQueryUtils.GetItemFromProduct(productList[0], ItemType.GEM);

                double price = productList[0].GetValue<double>("price");

                long baseGem = itemList.GetValue<long>("gem");
                long totalGem = TierUtils.GetTierFractionCoin(baseGem, TierUtils.GetMeTier());

                if (price == 99.99)
                    returnBB = productList[0];

                if (checkSpinGem * purchaseCheckMultiply < totalGem)
                {
                    return productList[0];
                }
            }

            return returnBB;
        }
    }
}