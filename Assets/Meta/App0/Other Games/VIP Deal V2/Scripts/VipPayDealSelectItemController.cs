using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class VipPayDealSelectItemController : VipDealV2SelectItemControllerBase
    {
        protected override string GetSelectCellText(Blackboard dealInfo)
        {
            var product = BlackboardUtils.FindValue<Blackboard>(dealInfo, "product");
            double price = BlackboardUtils.FindValue<double>(product, "price");
            return StringTableUtils.GetString(GLOBAL, "VIP_DEAL_ITEM_PRICE", price);
        }

        protected override long GetDealTotalCredit(Blackboard dealInfo)
        {
            var product = BlackboardUtils.FindValue<Blackboard>(dealInfo, "product");

            var coinItemBB = BlackboardQueryUtils.GetItemFromProduct(product, ItemType.CREDIT);
            var baseCredit = BlackboardUtils.FindValue<long>(coinItemBB, "baseCredit");

            int tier = TierUtils.GetMeTier();
            long totalBaseCoins = TierUtils.GetTierFractionCoin(baseCredit, tier);

            long additionalCreditMultiplierNumerator = BlackboardUtils.FindValue<long>(coinItemBB, "additionalCreditMultiplierNumerator");
            totalBaseCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalBaseCoins, "vipDeal");
            totalBaseCoins = NumberUtils.GetAdditionalMultiplierNumeratorValue(totalBaseCoins, additionalCreditMultiplierNumerator);

            long eventMultiplierNumerator = BlackboardUtils.FindValue<long>(product, "eventMultiplierNumerator");
            long itemMultiplierNumerator = BlackboardUtils.FindValue<long>(dealInfo, "multiplierNumerator");
            long totalCredit = NumberUtils.GetMultiplierNumeratorValue(totalBaseCoins, eventMultiplierNumerator);
            totalCredit = NumberUtils.GetMultiplierNumeratorValue(totalCredit, itemMultiplierNumerator);

            return totalCredit;
        }
    }
}
