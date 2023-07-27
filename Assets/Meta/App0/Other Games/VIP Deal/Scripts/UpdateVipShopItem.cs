using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Vip Deal")]
    public class UpdateVipShopItem : ActionTask<Blackboard>
    {
        public BBParameter<long> baseCoin;
        public BBParameter<long> totalCoin;
        public BBParameter<long> rp;
        public BBParameter<float> multiplier;
        public BBParameter<float> price;
        public BBParameter<bool> isFree;

        protected override string info
        {
            get { return "Update Vip Shop Item"; }
        }

        protected override void OnExecute()
        {
            var agentElement = agent.gameObject.GetComponent<ContextElement>();

            var offerText01Element = ContextUtils.FindElement(agentElement, "Offer Text 01", ContextSearchingType.ChildrenSearch);
            var offerText02Element = ContextUtils.FindElement(agentElement, "Offer Text 02", ContextSearchingType.ChildrenSearch);
            var offerText03Element = ContextUtils.FindElement(agentElement, "Offer Text 03", ContextSearchingType.ChildrenSearch);

            var buyButtonElement = ContextUtils.FindElement(agentElement, "Button Buy", ContextSearchingType.ChildrenSearch);
            var freeButtonElement = ContextUtils.FindElement(agentElement, "Button Free", ContextSearchingType.ChildrenSearch);

            buyButtonElement.gameObject.SetActive(!isFree.value);
            freeButtonElement.gameObject.SetActive(isFree.value);

            var multiplierTextElement   = ContextUtils.FindElement(agentElement, "Multiplier/Text", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetText(offerText01Element, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_BASE_COIN", baseCoin.value));
            MetaContextElementUtils.SetText(offerText02Element, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_TOTAL_COIN", totalCoin.value));
            MetaContextElementUtils.SetText(multiplierTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_MULTIPLIER", multiplier.value));

            if(isFree.value)
            {

                var priceElement = ContextUtils.FindElement(freeButtonElement, "Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(priceElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_BUY_FREE"));

                MetaContextElementUtils.SetText(offerText03Element, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_FREE_DESC", price.value));
            }
            else
            {
                
                var priceElement = ContextUtils.FindElement(buyButtonElement, "Text", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetText(priceElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_BUY_PRICE", price.value));
                MetaContextElementUtils.SetText(offerText03Element, StringTableUtils.GetString(StringTable.StringTableType.Global, "VIP_DEAL_SHOP_ITEM_RP", rp.value));
            }
            

            EndAction();
        }
    }
}
