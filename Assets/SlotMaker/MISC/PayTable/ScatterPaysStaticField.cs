using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class ScatterPaysStaticField : MonoBehaviour
    {
        public StringTable.StringTableType tableType;
        public string key;
        public string payTablePath;
        public int startPayCount = 3;
        public int endPayCount = 5;
        public ContextElement staticField;

        private void Awake()
        {
            var scatterPays = BlackboardUtils.FindVariable<List<long>>(null, payTablePath).value;

            bool error = true;
            IContextText textElement = staticField as IContextText;
            if (textElement != null)
            {
                int i = startPayCount - 1;
                int payCount = endPayCount - startPayCount + 1;

                switch (payCount)
                {
                case 2:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, scatterPays[i], scatterPays[i + 1], out error));
                    break;
                case 3:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, scatterPays[i], scatterPays[i + 1], scatterPays[i + 2], out error));
                    break;
                case 4:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, scatterPays[i], scatterPays[i + 1], scatterPays[i + 2], scatterPays[i + 3], out error));
                    break;
                case 8:
                    textElement.SetText(StringTableUtils.GetString(tableType, key, scatterPays[i], scatterPays[i + 1], scatterPays[i + 2], scatterPays[i + 3], scatterPays[i + 4], scatterPays[i + 5], scatterPays[i + 6], scatterPays[i + 7], out error));
                    break;
                }
            }
        }
    }
}
