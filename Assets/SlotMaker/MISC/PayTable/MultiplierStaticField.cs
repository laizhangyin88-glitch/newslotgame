using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class MultiplierStaticField : MonoBehaviour 
    {
        public StringTable.StringTableType tableType;
        public string key;
        public string tablePath;
        public int index = 0;
        public ContextElement staticField;

        private Variable<List<long>> table;

        void Awake()
        {
            table = BlackboardUtils.FindVariable<List<long>>(tablePath);
        }

        void Start()
        {
            Refresh();
        }

        void Refresh()
        {
            bool error = true;
            IContextText textElement = staticField as IContextText;

            textElement.SetText(StringTableUtils.GetString(tableType, key, table.value[index], out error));
        }
    }
}