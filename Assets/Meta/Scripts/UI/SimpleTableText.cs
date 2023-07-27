using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using static SlotMaker.StringTable;

namespace Meta.Scripts.UI
{
    public class SimpleTableText : MonoBehaviour
    {
        public string tableKey;
        public StringTableType tableType;


        private void Start()
        {
            GetComponent<IContextText>().SetText(StringTableUtils.GetString(tableType, tableKey));
        }
    }
}
