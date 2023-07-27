using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/IAM")]
    public class GetRewardTicketedBonusBooster : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> product;
        public BBParameter<InAppMessageType> iamType;
        public BBParameter<ShopType> shopType;
        public BBParameter<ItemType> itemType;
        public BBParameter<Blackboard> saveAsBoostProduct;
        public BBParameter<bool> saveAsIsNotCheckVIPLounge;

        protected override void OnExecute()
        {
            double price;
            switch (iamType.value)
            {
                case InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP:
                case InAppMessageType.SUPER_BONUS_PURCHASE_POPUP:
                    price = product.value.GetValue<double>("price");
                    break;
                case InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP:
                    // Gem ? 
                default:
                    EndAction();
                    return;
            }
            // ShopType : TICKETED_BONUS_BOOSTER / ItemType : TICKETED_BONUS_BOOSTER / BAB ?
            saveAsBoostProduct.value = BlackboardQueryUtils.GetPriceMatchProduct(price, shopType.value, itemType.value, out _);
            saveAsIsNotCheckVIPLounge.value = true;
            EndAction();
        }
    }
}