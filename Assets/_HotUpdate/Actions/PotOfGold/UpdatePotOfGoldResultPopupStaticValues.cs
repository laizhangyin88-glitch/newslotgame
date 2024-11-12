using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

    [Category("★ BagelCode/Popup/Purchase")]
    public class UpdatePotOfGoldResultPopupStaticValues : ActionTask<Blackboard> 
    {
        public BBParameter<string> titleKey;

        public BBParameter<Blackboard> saveAsItemUseResult;
        public BBParameter<Blackboard> saveAsUserSyncInfo;

        private ContextElement titleTextElement;
        private ContextElement coinTextElement;

        private ContextElement eventMultiplierArea;
        private Transform      eventBadgeObject;
        private ContextElement inflationBadgeElement;

        // private ContextElement eventMultiplierText;

        private Blackboard purchaseResponse;

        private long earnCoin = 0;
        private long eventMultiplierNumerator;

        private long inflationNumerator;
        private bool isInflation;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        protected override void OnExecute()
        {
            InitProperty();

            UpdateResult();

            EndAction();
        }

        private void InitProperty()
        {
            inflationNumerator = NumberUtils.GetShopInflationNumerator(ShopType.PIGGY_BANK);
            // inflationNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/INFLATION_NUMERATOR_POG").value;
            isInflation = inflationNumerator > NumberUtils.GetGlobalDenominator();

            ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

            titleTextElement = ContextUtils.FindElement(agentElement, "Title Area/Text", ContextSearchingType.FullNameSearch);
            coinTextElement = ContextUtils.FindElement(agentElement, "Text", ContextSearchingType.ChildrenSearch);
            eventMultiplierArea = ContextUtils.FindElement(agentElement, "Badge Area", ContextSearchingType.ChildrenSearch);

            eventBadgeObject = eventMultiplierArea.transform.Find("Badge Event");
            inflationBadgeElement = ContextUtils.FindElement(eventMultiplierArea, "Badge Inflation", ContextSearchingType.FullNameSearch);

            purchaseResponse = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "purchaseResponse").value;

            var itemUseResultList = purchaseResponse.GetValue<List<Blackboard>>("itemUseResultList");
            saveAsItemUseResult.value = itemUseResultList[0];
            saveAsUserSyncInfo.value = purchaseResponse.GetValue<Blackboard>("userSyncInfo");

            var eventNumerator = BlackboardUtils.FindVariable<long>(saveAsItemUseResult.value, "multiplierNumerator");
            if(eventNumerator != null)
                eventMultiplierNumerator = eventNumerator.value;
            else
                eventMultiplierNumerator = NumberUtils.GetGlobalDenominator();

            inflationBadgeElement.gameObject.SetActive(false);
            eventBadgeObject.gameObject.SetActive(false);

            earnCoin = saveAsItemUseResult.value.GetValue<long>("earnCredit");
        }

        private void UpdateResult()
        {
            MetaContextElementUtils.SetText(titleTextElement, StringTableUtils.GetString(tableType, titleKey.value));

            if(eventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
            {
                if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.PIGGY_BANK) )
                {
                    long viewAddPercent = NumberUtils.GetAdditionalPercent(eventMultiplierNumerator);
                    MetaContextElementUtils.SimpleSetText(eventMultiplierArea, "Text", StringTableUtils.GetString(tableType, "TEXT_COMMON_PURCHASE_ADDITIONAL_PERCENT_BADGE", viewAddPercent));
                }
                else
                {
                    MetaContextElementUtils.SimpleSetText(eventMultiplierArea, "Text", StringTableUtils.GetString(tableType, "TEXT_POT_OF_GOLD_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator)));
                }
                
                eventBadgeObject.gameObject.SetActive(true);
                eventMultiplierArea.gameObject.SetActive(true);
            }
            else if(isInflation)
            {
                MetaContextElementUtils.SimpleSetText(inflationBadgeElement, "Text", StringTableUtils.GetString(tableType, "TEXT_POT_OF_GOLD_INFLATION_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(inflationNumerator)) );
                inflationBadgeElement.gameObject.SetActive(true);
                eventMultiplierArea.gameObject.SetActive(true);
            }
            else
            {
                eventMultiplierArea.gameObject.SetActive(false);
            }

            MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_COIN_TEXT", earnCoin));
        }
    }

}
