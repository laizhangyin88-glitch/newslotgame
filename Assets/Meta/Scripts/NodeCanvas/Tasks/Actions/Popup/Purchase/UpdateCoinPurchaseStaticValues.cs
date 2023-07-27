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
    public class UpdateCoinPurchaseStaticValues : ActionTask<Blackboard>
    {
        public BBParameter<string> titleKey;

        public BBParameter<Blackboard> saveAsItemUseResult;
        public BBParameter<Blackboard> saveAsUserSyncInfo;

        public BBParameter<bool> saveAsUseTierProgress;

        public BBParameter<string> levelMultiplierType;

        private ContextElement titleTextElement;
        private ContextElement coinTextElement;
        private ContextElement eventMultiplierArea;
        private ContextElement eventMultiplierText;
        private ContextElement tierProgressElement;

        private Blackboard purchaseResponse;

        private long origLevelMultiplyEarnCoin = 0;
        private long earnCoin = 0;
        private long earnRP = 0;
        private long eventMultiplierNumerator;
        private bool applyTierMultiplier = false;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        protected override void OnExecute()
        {
            InitProperty();

            UpdateResult();

            if(earnRP == 0)
                UpdateResultWithoutRP();
            else
                UpdateWithRP();

            EndAction();
        }

        private void InitProperty()
        {
            ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

            titleTextElement = ContextUtils.FindElement(agentElement, "Title Area/Text", ContextSearchingType.FullNameSearch);
            coinTextElement = ContextUtils.FindElement(agentElement, "Text", ContextSearchingType.ChildrenSearch);
            eventMultiplierArea = ContextUtils.FindElement(agentElement, "Badge Area", ContextSearchingType.ChildrenSearch);
            eventMultiplierText = ContextUtils.FindElement(eventMultiplierArea, "Text", ContextSearchingType.ChildrenSearch);
            tierProgressElement = ContextUtils.FindElement(agentElement, "Tier Progress", ContextSearchingType.ChildrenSearch);

            purchaseResponse = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "purchaseResponse").value;

            var itemUseResultList = purchaseResponse.GetValue<List<Blackboard>>("itemUseResultList");
            saveAsItemUseResult.value = itemUseResultList[0];
            saveAsUserSyncInfo.value = purchaseResponse.GetValue<Blackboard>("userSyncInfo");

            var eventNumerator = BlackboardUtils.FindVariable<long>(saveAsItemUseResult.value, "eventMultiplierNumerator");
            if(eventNumerator != null)
                eventMultiplierNumerator = eventNumerator.value;
            else
                eventMultiplierNumerator = NumberUtils.GetGlobalDenominator();

            var applyTierMultiplierValue = BlackboardUtils.FindVariable<bool>(saveAsItemUseResult.value, "applyTierMultiplier");
            applyTierMultiplier = applyTierMultiplierValue == null ? false : applyTierMultiplierValue.value;

            string lmTypeValue = (levelMultiplierType == null || string.IsNullOrEmpty(levelMultiplierType.value)) ? "coin" : levelMultiplierType.value;
            origLevelMultiplyEarnCoin = LevelUtils.GetLevelMultiplierNumeratorValue(saveAsItemUseResult.value.GetValue<long>("origEarnCredit"), lmTypeValue);
            earnCoin = saveAsItemUseResult.value.GetValue<long>("earnCredit");
            earnRP = saveAsItemUseResult.value.GetValue<long>("earnRp");
        }

        private void UpdateResult()
        {
            MetaContextElementUtils.SetText(titleTextElement, StringTableUtils.GetString(tableType, titleKey.value));

            if(eventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
            {
                if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.COIN) )
                {
                    long viewAddPercent = NumberUtils.GetAdditionalPercent(eventMultiplierNumerator);
                    MetaContextElementUtils.SetText(eventMultiplierText, StringTableUtils.GetString(tableType, "TEXT_COMMON_PURCHASE_ADDITIONAL_PERCENT_BADGE", viewAddPercent));
                }
                else
                {
                    MetaContextElementUtils.SetText(eventMultiplierText, StringTableUtils.GetString(tableType, "SHOP_EVENT_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator)));
                }

                eventMultiplierArea.gameObject.SetActive(true);
            }
            else
            {
                eventMultiplierArea.gameObject.SetActive(false);
            }
        }

        private void UpdateWithRP()
        {
            MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_COIN_TEXT", earnCoin));

            tierProgressElement.gameObject.SetActive(true);
            saveAsUseTierProgress.value = true;
        }

        private void UpdateResultWithoutRP()
        {
            if(applyTierMultiplier)
            {
                var meTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/tier").value;
                var tierMultiplierText = TierUtils.GetFrationMultiplierText(meTier);

                if(eventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
                    MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_COIN_EVENT_WITHOUT_RP_TEXT", origLevelMultiplyEarnCoin, tierMultiplierText, meTier, NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator), earnCoin));
                else
                {
                    MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_COIN_WITHOUT_RP_TEXT", origLevelMultiplyEarnCoin, tierMultiplierText, meTier, earnCoin));
                }
            }
            else
            {
                if(eventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
                    MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_COIN_EVENT_WITHOUT_RP_TIER_TEXT", origLevelMultiplyEarnCoin, NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator), earnCoin));
                else
                {
                    MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_COIN_WITHOUT_RP_TIER_TEXT", earnCoin));
                }
            }


            tierProgressElement.gameObject.SetActive(false);
            saveAsUseTierProgress.value = false;
        }
    }

}
