using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class SymbolPaysStaticField : MonoBehaviour
    {
        public StringTable.StringTableType tableType;
        public string key;
        public int symbol;
        public int paytableIndex = 0;
        public int startPayCount = 3;
        public int endPayCount = 5;
        public ContextElement staticField;

        private void Awake()
        {
            var paytable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./game/paytables").value[paytableIndex].GetValue<List<Blackboard>>("value");
            var symbolPays = paytable[symbol].GetValue<List<long>>("value");

            bool error = true;
            IContextText textElement = staticField as IContextText;
            if (textElement != null)
            {
                int i = startPayCount - 1;
                int payCount = endPayCount - startPayCount + 1;

                switch (payCount)
                {
                case 1:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, symbolPays[i], out error));
                    break;
                case 2:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, symbolPays[i], symbolPays[i + 1], out error));
                    break;
                case 3:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, symbolPays[i], symbolPays[i + 1], symbolPays[i + 2], out error));
                    break;
                case 4:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, symbolPays[i], symbolPays[i + 1], symbolPays[i + 2], symbolPays[i + 3], out error));
                    break;
                case 8:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, symbolPays[i], symbolPays[i + 1], symbolPays[i + 2], symbolPays[i + 3], symbolPays[i + 4], symbolPays[i + 5], symbolPays[i + 6], symbolPays[i + 7], out error));
                    break;
                }
            }
        }
    }
}
