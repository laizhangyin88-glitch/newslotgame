using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta/Purchase/CoinBooster")]
    public class UpdateCoinBoosterPurchaseStaticValues : ActionTask<Blackboard>
    {
        public BBParameter<List<Blackboard>> productBBList;

        public BBParameter<long> prevEarnCoins;

        public BBParameter<ContextElement> saveAsCloseButtonElement;
        public BBParameter<ContextElement> saveAsPurchaseButtonElement;
        public BBParameter<ContextElement> saveAsFreeButtonElement;

        public BBParameter<ContextElement> saveAsCurrentMultiplierElement;
        public BBParameter<ContextElement> saveAsCurrentElement;
        public BBParameter<ContextElement> saveAsWheelContextElement;

        public BBParameter<ContextElement> saveAshighlightContextElement;

        public BBParameter<ContextElement> saveAsResultMultiplierElement;
        public BBParameter<ContextElement> saveAsResultElement;

        public BBParameter<string> purchaseFormat;      // POPUP_COIN_MULTIPLIER_PURCHASE_COIN_TEXT

        private bool isInit = false;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private ContextElement agentElement;
        private ContextElement saleObjectElement;
        private ContextElement salePercentTextElement;

        protected override string info
        {
            get { return "Update Booster Purchase Static VAlues"; }
        }

        protected override void OnExecute()
        {
            InitProperty();
            UpdateValues();

            EndAction();
        }

        private void InitProperty()
        {
            if (isInit) return;

            agentElement = agent.gameObject.GetComponent<ContextElement>();
            agentElement.UpdateContext(false);

            saveAsPurchaseButtonElement.value = ContextUtils.FindElement(agentElement, "Button Boost", ContextSearchingType.ChildrenSearch);
            saveAsFreeButtonElement.value = ContextUtils.FindElement(agentElement, "Button Free", ContextSearchingType.ChildrenSearch);

            saveAsCloseButtonElement.value = ContextUtils.FindElement(agentElement, "Button Close", ContextSearchingType.ChildrenSearch);
            var oddsButtonElement = ContextUtils.FindElement(agentElement, "Button Odds", ContextSearchingType.ChildrenSearch);

            saveAsCurrentMultiplierElement.value = ContextUtils.FindElement(agentElement, "Text Normal/Text Multiplier", ContextSearchingType.FullNameSearch);
            saveAsCurrentElement.value = ContextUtils.FindElement(agentElement, "Text Normal/Text Coin", ContextSearchingType.FullNameSearch);
            saveAsWheelContextElement.value = ContextUtils.FindElement(agentElement, "Wheel", ContextSearchingType.ChildrenSearch);

            saveAshighlightContextElement.value = ContextUtils.FindElement(agentElement, "Wheel/Highlight", ContextSearchingType.FullNameSearch);

            saveAsResultMultiplierElement.value = ContextUtils.FindElement(agentElement, "Text Result/Text Multiplier Result", ContextSearchingType.FullNameSearch);
            saveAsResultElement.value = ContextUtils.FindElement(agentElement, "Text Result/Text Coin Result", ContextSearchingType.FullNameSearch);

            saleObjectElement = ContextUtils.FindElement(agentElement, "Button Boost/Sale Tag", ContextSearchingType.FullNameSearch);
            salePercentTextElement = ContextUtils.FindElement(agentElement, "Button Boost/Sale Tag/Text", ContextSearchingType.FullNameSearch);

            MetaContextElementUtils.SetClickable(
                saveAsCloseButtonElement.value,
                "OnClose",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            MetaContextElementUtils.SetClickable(
                saveAsPurchaseButtonElement.value,
                "OnSpin",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            MetaContextElementUtils.SetClickable(
                saveAsFreeButtonElement.value,
                "OnFreeSpin",
                false,
                false,
                SendEvent,
                ownerSystem
            );

            MetaContextElementUtils.SetClickable(
                oddsButtonElement,
                "OnOpenProbPopup",
                false,
                true,
                SendEvent,
                ownerSystem
            );

            isInit = true;
        }

        private void UpdateValues()
        {
            MetaContextElementUtils.SimpleSetText(agentElement, "Text Purchased", StringTableUtils.GetString(tableType, purchaseFormat.value, prevEarnCoins.value));
            MetaContextElementUtils.SimpleSetText(agentElement, "Text Purchased Result", StringTableUtils.GetString(tableType, purchaseFormat.value, prevEarnCoins.value));
            MetaContextElementUtils.SimpleSetText(agentElement, "Text Result/Text Multiplier", StringTableUtils.GetString(tableType, "POPUP_COIN_MULTIPLIER_RESULT_TEXT"), ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetText(agentElement, "Button Odds/Text", StringTableUtils.GetString(tableType, "BUTTON_ODDS"), ContextSearchingType.FullNameSearch);

            int salePercent = BlackboardQueryUtils.GetProductSalePercent(productBBList.value[0]);
            if (salePercent > 0)
                MetaContextElementUtils.SetText(salePercentTextElement, StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", salePercent));
            saleObjectElement.gameObject.SetActive(salePercent > 0);
        }
    }
}
