using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using SlotMaker.Cards;

namespace BagelCode
{
    public class PokerPaysTitleField : MonoBehaviour 
    {
        public StringTable.StringTableType tableType;
        public string key;
        public int paytableIndex = 0;
        public ContextElement staticField;

        private void Start()
        {
            bool error = true;
            IContextText textElement = staticField as IContextText;

            var pokerPayRuleTable = (PokerPayTable)(CustomCardData.Instance.payTable);
            string pokerPaysTitle = pokerPayRuleTable.ranks[paytableIndex].name;

            textElement.SetText(StringTableUtils.GetString(tableType, key, pokerPaysTitle, out error));
        }
    }
}