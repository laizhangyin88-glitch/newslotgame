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
    public class UpdateGemPurchaseStaticValues : ActionTask<Blackboard> 
    {
        public BBParameter<string> titleKey;

        public BBParameter<Blackboard> saveAsItemUseResult;
        public BBParameter<Blackboard> saveAsUserSyncInfo;

        public BBParameter<bool> saveAsUseTierProgress;

        private ContextElement titleTextElement;
        private ContextElement coinTextElement;
        private ContextElement eventMultiplierArea;
        private ContextElement eventMultiplierText;
        private ContextElement tierProgressElement;

        private Blackboard purchaseResponse;

        private long earnGem = 0;
        private long earnRP = 0;
        private long eventMultiplierNumerator;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        protected override void OnExecute()
        {
            InitProperty();

            UpdateResult();
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

            eventMultiplierNumerator = saveAsItemUseResult.value.GetValue<long>("eventMultiplierNumerator");
            earnGem = saveAsItemUseResult.value.GetValue<long>("earnGem");
            earnRP = saveAsItemUseResult.value.GetValue<long>("earnRp");
        }

        private void UpdateResult()
        {
            MetaContextElementUtils.SetText(titleTextElement, StringTableUtils.GetString(tableType, titleKey.value));

            if(eventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
            {
                if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM) )
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
            MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, "POPUP_PURCHASE_GEM_TEXT", earnGem));

            tierProgressElement.gameObject.SetActive(true);
            saveAsUseTierProgress.value = true;
        }
    }

}
