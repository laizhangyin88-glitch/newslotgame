using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [ExecuteInEditMode]
    public class StaticField : MonoBehaviour 
    {
        public StringTable.StringTableType tableType;
        public string key;
        public ContextElement staticField;

        private void Awake()
        {
            bool error = true;
            IContextText textElement = staticField as IContextText;
            if (textElement != null)
                textElement.SetText(StringTableUtils.GetString(tableType, key, out error));
        }    
    }
}
