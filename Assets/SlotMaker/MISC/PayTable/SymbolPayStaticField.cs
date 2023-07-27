using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class SymbolPayStaticField : MonoBehaviour
    {
        public StringTable.StringTableType tableType;
        public string key;
        public int symbol;
        public int paytableIndex = 0;
        public int index;
        public ContextElement staticField;

        private void Awake()
        {
            var paytable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/paytables").value[paytableIndex].GetValue<List<Blackboard>>("value");
            var symbolPays = paytable[symbol].GetValue<List<long>>("value");

            bool error = true;
            IContextText textElement = staticField as IContextText;
            if (textElement != null)
            {
                textElement.SetText(StringTableUtils.GetString(tableType, key, symbolPays[index], out error));
            }
        }
    }
}
