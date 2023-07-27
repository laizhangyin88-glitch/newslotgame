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
    public class GetGemBoosterInformation : ActionTask<ContextElement>
    {
        public BBParameter<Blackboard> gemProduct;
        public BBParameter<Blackboard> gemBoosterProduct;
        public BBParameter<long> maxGem;
        public BBParameter<long> totalBaseGem;
        public BBParameter<long> maxMultiplier;
        
        protected override string info
        {
            get { return "Get Gem Booster Information"; }
        }

        protected override void OnExecute()
        {
            int tier = TierUtils.GetMeTier();
            long baseGem = 0;
            long totalGem = 0;
            long gemProductTotalGems = 0;

            var gemItemBB = BlackboardQueryUtils.GetItemFromProduct(gemProduct.value, ItemType.GEM);
            baseGem = gemItemBB.GetValue<long>("gem");
            totalGem = totalBaseGem.value;

            gemProductTotalGems = totalGem;

            var gemBoosterItemBB = BlackboardQueryUtils.GetItemFromProduct(gemBoosterProduct.value, ItemType.GEM_BOOSTER);
            var settingList = BlackboardUtils.FindVariable<List<Blackboard>>(gemBoosterItemBB, "setting");

            long multiplierNumerator = settingList.value[0].GetValue<long>("multiplierNumerator");
            maxMultiplier.value = multiplierNumerator;

            double multiplier = NumberUtils.GetMultiplierFromNumerator(multiplierNumerator);

            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER_MULTIPLY);

            if(eventInfo != null)
            {
                long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                multiplierNumerator = NumberUtils.GetMultiplierNumeratorValue(multiplierNumerator, eventMultiplierNumerator);
            }

            EventInfo shopEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_SHOP_EVENT_MULTIPLY);

            if(shopEventInfo != null)
            {
                long shopEventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(shopEventInfo);
                multiplierNumerator = NumberUtils.GetMultiplierNumeratorValue(multiplierNumerator, shopEventMultiplierNumerator);
            }

            maxGem.value = NumberUtils.GetMultiplierNumeratorValue(gemProductTotalGems, multiplierNumerator);
            EndAction();
        }
    }
}
