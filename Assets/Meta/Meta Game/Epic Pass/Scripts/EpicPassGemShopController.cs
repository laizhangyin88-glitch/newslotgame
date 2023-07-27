using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.EpicPass
{
    public class EpicPassGemShopController : MonoBehaviour
    {
        private ContextElement rootElement;
        private ContextButton buyButton;
        private double resetGemPrice;
        private long chargeGem;

        public void Initialize()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            SetGemProduct();
            UpdateContext();

            Blackboard rootblackboard = gameObject.GetComponent<Blackboard>();
            Blackboard gemShopBB = BlackboardQueryUtils.GetShopBB(ShopType.SEASON_PASS_GEM_RECOMMENDATION);
            BlackboardUtils.SetOrCreateValue<Blackboard>(rootblackboard, "_shopBB", gemShopBB);
            BlackboardUtils.SetOrCreateValue<string>(rootblackboard, "_biContextID", EpicPassUtils.contextId);

            buyButton.AddListenerOnClick(
                context =>
                {
                    EventSender.SendGlobalEvent("EpicGemShopPurchase");
                }
            );

            ContextButton closeButton = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            closeButton.AddListenerOnClick(
                context =>
                {
                    ClosePopup();
                }
            );
        }

        public void ClosePopup()
        {
            Destroy(gameObject);
        }

        private void UpdateContext()
        {
            ContextTextMeshProUGUI titleText = ContextUtils.FindElement(rootElement, "Title Area/Text", ContextSearchingType.FullNameSearch).GetComponent<ContextTextMeshProUGUI>();
            ContextTextMeshProUGUI contentsText = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            buyButton = ContextUtils.FindElement(rootElement, "Button Buy", ContextSearchingType.ChildrenSearch).GetComponent<ContextButton>();
            ContextTextMeshProUGUI buyText = ContextUtils.FindElement(buyButton, "Text", ContextSearchingType.ChildrenSearch).GetComponent<ContextTextMeshProUGUI>();

            MetaContextElementUtils.SetTextGlobal(titleText, "EPIC_PASS_GEM_SHOP_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(contentsText, "EPIC_PASS_GEM_SHOP_CONTENTS_TEXT", chargeGem);
            MetaContextElementUtils.SetTextGlobal(buyText, "EPIC_PASS_GEM_SHOP_BUTTON_TEXT", resetGemPrice);
        }

        private void SetGemProduct()
        {
            Blackboard product = EpicPassUtils.GetResetProduct;
            if (product == null)
                return;

            Blackboard rootBlackboard = gameObject.GetComponent<Blackboard>();
            if (rootBlackboard != null)
            {
                BlackboardUtils.SetOrCreateValue<Blackboard>(rootBlackboard, "product", product);
                resetGemPrice = product.GetValue<double>("price");

                var myTier = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;
                Blackboard itemBB = BlackboardQueryUtils.GetItemFromProduct(product, ItemType.GEM);
                chargeGem = TierUtils.GetTierFractionCoin(itemBB.GetValue<long>("gem"), myTier);
            }
        }

        public void RequestEpicPassReset()
        {
            if (EpicPassUtils.Paid)
            {
                EpicPassUtils.RequestEpicPassReset();
            }
            else
            {
                EpicPassUtils.OpenRewardLostScene();
            }
        }
    }
}
