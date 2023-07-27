using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using BagelCode.ClientModels;

namespace BagelCode.MetaGame.Tasks.Actions
{
    [Category("★ BagelCode/Popup")]
    public class GetCoinBoosterInformation : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard> coinProduct;
        public BBParameter<Blackboard> coinBoosterProduct;
        public BBParameter<long> maxCoin;
        public BBParameter<long> totalCoins;
        public BBParameter<long> maxMultiplier;
        
        protected override string info
        {
            get { return "Get Coin Booster Information"; }
        }

        protected override void OnExecute()
        {
            long baseCoin = 0;
            long totalCoin = 0;
            long coinProductTotalCoins = 0;

            BlackboardQueryUtils.GetCoinProductPredictCoin(coinProduct.value, out baseCoin, out totalCoin, out coinProductTotalCoins);

            var coinBoosterItemBB = BlackboardQueryUtils.GetItemFromProduct(coinBoosterProduct.value, ItemType.CREDIT_MULTIPLIER_WHEEL);
            var settingList = BlackboardUtils.FindVariable<List<Blackboard>>(coinBoosterItemBB, "setting");

            long multiplierNumerator = settingList.value[0].GetValue<long>("multiplierNumerator");
            maxMultiplier.value = multiplierNumerator;

            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY);

            if(eventInfo != null)
            {
                long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                multiplierNumerator = NumberUtils.GetMultiplierNumeratorValue(multiplierNumerator, eventMultiplierNumerator);
            }

            EventInfo shopEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COIN_SHOP_EVENT_MULTIPLY);

            if(shopEventInfo != null)
            {
                long shopEventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(shopEventInfo);
                multiplierNumerator = NumberUtils.GetMultiplierNumeratorValue(multiplierNumerator, shopEventMultiplierNumerator);
            }

            maxCoin.value = NumberUtils.GetMultiplierNumeratorValue(coinProductTotalCoins, multiplierNumerator);
            totalCoins.value = coinProductTotalCoins;

            EndAction();
        }
    }
}
