using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class PokerPaysStaticField : MonoBehaviour 
    {
        public StringTable.StringTableType tableType;
        public string key;
        public string payTablePath;
        public int paytableIndex = 0;
        public ContextElement staticField;

        private Variable<List<long>> paytable;
        private Variable<long> baseWagerPerHand;
        private Variable<long> betPerHand;

        void Awake()
        {
            paytable = BlackboardUtils.FindVariable<List<long>>(payTablePath);
            baseWagerPerHand = BlackboardUtils.FindVariable<long>("./game/baseWagerPerHand");
            betPerHand = BlackboardUtils.GetOrCreateVariable<long>("./betPerHand");
            betPerHand.onValueChanged += OnChangedBetPerHand;
        }

        void OnDestroy()
        {
            betPerHand.onValueChanged -= OnChangedBetPerHand;
        }

        void Start()
        {
            Refresh();
        }

        void OnChangedBetPerHand(string name, object value)
        {
            Refresh();
        }

        void Refresh()
        {
            bool error = true;
            IContextText textElement = staticField as IContextText;

            textElement.SetText(StringTableUtils.GetString(tableType, key, paytable.value[paytableIndex] * betPerHand.value / baseWagerPerHand.value, out error));
        }
    }
}