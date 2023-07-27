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
    public class GetVipDealProductInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard>  vipDealInfoBB;
        public BBParameter<Blackboard>  saveAsProductBB;

        public BBParameter<int>    purchaseMultiplierPercent;
        public BBParameter<long>   purchaseMultipliedBaseCredit;
        public BBParameter<double>  tierMultiplier;
        public BBParameter<double>  eventMultiplier;
        public BBParameter<long>   totalBaseCredit;
        public BBParameter<long>   totalCredit;
        public BBParameter<long>   rewardPoint;
        public BBParameter<float>  itemPrice;
        public BBParameter<float>  origItemPrice;
        public BBParameter<bool>   isSoldOut;
        public BBParameter<int>    coinLevel;

        public BBParameter<bool>   isFree;
        public BBParameter<bool>   isMaxBadge;
        public BBParameter<double> multiplier;

        public BBParameter<string> vipDealUUID;
        public BBParameter<string> levelMultiplierType;

        private List<long> coinGradeList = new List<long>() {9000000, 30000000};

        protected override string info
        {
            get { return "Get Vip Deal Product Info BB"; }
        }

        protected override void OnExecute()
        {
            saveAsProductBB.value = BlackboardUtils.FindVariable<Blackboard>(vipDealInfoBB.value, "product").value;
            var coinItemBB = BlackboardQueryUtils.GetItemFromProduct(saveAsProductBB.value, ItemType.CREDIT);

            itemPrice.value         = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(saveAsProductBB.value, "price").value);
            origItemPrice.value     = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(saveAsProductBB.value, "originalPrice").value);

            long eventMultiplierNumerator = BlackboardUtils.FindVariable<long>(saveAsProductBB.value, "eventMultiplierNumerator").value;
            eventMultiplier.value   = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);

            long itemMultiplierNumerator = vipDealInfoBB.value.GetValue<long>("multiplierNumerator");
            multiplier.value        = NumberUtils.GetMultiplierFromNumerator(itemMultiplierNumerator);
            isFree.value            = vipDealInfoBB.value.GetValue<bool>("isFree");
            isSoldOut.value         = vipDealInfoBB.value.GetValue<bool>("isSoldOut");
            vipDealUUID.value       = vipDealInfoBB.value.GetValue<string>("dealUuid");
            isMaxBadge.value        = multiplier.value >= 10f;

            var   rp         = BlackboardUtils.FindVariable<long>(coinItemBB, "rp");
            var   baseCredit = BlackboardUtils.FindVariable<long>(coinItemBB, "baseCredit");
            long additionalCreditMultiplierNumerator = BlackboardUtils.FindVariable<long>(coinItemBB, "additionalCreditMultiplierNumerator").value;

            purchaseMultiplierPercent.value     = System.Convert.ToInt32(additionalCreditMultiplierNumerator/NumberUtils.GetGlobalDenominator()*100);
            purchaseMultipliedBaseCredit.value  = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseCredit.value, additionalCreditMultiplierNumerator);

            int tier = TierUtils.GetMeTier();

            tierMultiplier.value   = TierUtils.GetTierMultiplier( tier );

            long totalBaseCoins = TierUtils.GetTierFractionCoin(baseCredit.value, tier);
            string lmTypeValue = (levelMultiplierType == null || string.IsNullOrEmpty(levelMultiplierType.value)) ? "vipDeal" : levelMultiplierType.value;
            totalBaseCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalBaseCoins, lmTypeValue);
            totalBaseCoins = NumberUtils.GetAdditionalMultiplierNumeratorValue(totalBaseCoins, additionalCreditMultiplierNumerator);

            totalBaseCredit.value  = totalBaseCoins;
            totalCredit.value      = NumberUtils.GetMultiplierNumeratorValue(totalBaseCoins, eventMultiplierNumerator);
            totalCredit.value      = NumberUtils.GetMultiplierNumeratorValue(totalCredit.value, itemMultiplierNumerator);
            rewardPoint.value      = rp.value;


            ///////// Hard Code ///////////////
            if(totalCredit.value > coinGradeList[1])
            {
                coinLevel.value = 2;
            }
            else if(coinGradeList[0] < totalCredit.value && totalCredit.value <= coinGradeList[1])
            {
                coinLevel.value = 1;
            }
            else
            {
                coinLevel.value = 0;
            }
            //////////////////////////////////

            EndAction();
        }
    }
}
