using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.LuckyFive;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Lucky Five")]
    public class UpdateLuckyFiveWonInfo : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> infoBB;
        public BBParameter<int> cellIndex;

        public BBParameter<int> saveAsAniCellIndex;

        private ContextElement agentElement;

        protected override string info
        {
            get { return "Update Lucky Five Won Info"; }
        }

        protected override void OnExecute()
        {
            if(infoBB != null && infoBB.value != null)
            {
                agentElement = agent.gameObject.GetComponent<ContextElement>();

                ContextElement avgBetTextElement = ContextUtils.FindElement(agentElement, "Bet/Text Bet", ContextSearchingType.FullNameSearch);
                ContextElement resultTextElement = ContextUtils.FindElement(agentElement, "Cards/Text Cards", ContextSearchingType.FullNameSearch);
                ContextElement wonCoinTextElement = ContextUtils.FindElement(agentElement, "Won/Text Won", ContextSearchingType.FullNameSearch);
                ContextElement statusTextElement = ContextUtils.FindElement(agentElement, "Status/Text Status", ContextSearchingType.FullNameSearch);

                var bet = infoBB.value.GetValue<long>("bet");
                var win = infoBB.value.GetValue<long>("win");
                var pay = infoBB.value.GetValue<double>("pay");
                var rule = infoBB.value.GetValue<LuckyFiveRule>("rule");
                var state = infoBB.value.GetValue<LuckyFiveWinState>("state");

                string ruleText = StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_RULE_FORMAT", rule.ToString());
                ruleText = StringTableUtils.GetString(StringTable.StringTableType.Global, ruleText);

                SetTextElementText(avgBetTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_WON_LIST_COIN_TEXT", bet));
                SetTextElementText(resultTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_WON_LIST_RULE_TEXT", ruleText, pay));
                SetTextElementText(wonCoinTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_WON_LIST_COIN_TEXT", win));
                SetTextElementText(statusTextElement, StringTableUtils.GetString(StringTable.StringTableType.Global, "LUCKY_FIVE_WON_LIST_STATUS_TEXT", state == LuckyFiveWinState.UNCLAIMED ? 1 : 2));

                saveAsAniCellIndex.value = cellIndex.value%2;
            }
            else
            {
                saveAsAniCellIndex.value = 2;
            }

            EndAction();
        }

        private void SetTextElementText(ContextElement element, string text)
        {
            IContextText textElement = element as IContextText;
            if(textElement != null)
                textElement.SetText(text);
        }
    }
}
