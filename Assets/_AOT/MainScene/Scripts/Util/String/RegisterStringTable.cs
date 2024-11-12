using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;

namespace SlotMaker
{
    public class RegisterStringTable : MonoBehaviour
    {
        public StringTable.StringTableType tableType = StringTable.StringTableType.Content;

        [InlineEditor]
        public StringTableObject table;

        private void Awake()
        {
            if (StringTable.Instance == null) return;

            var bb = StringTable.Get(tableType);
            if (bb == null) return;

            // Patch
            foreach (var pair in bb.variables)
            {
                table.SetString(pair.Key, (string)pair.Value.value);
            }

            // Upload
            foreach (var pair in table.stringTable)
            {
                Variable variable = bb.GetVariable<string>(pair.key);
                if (variable == null) 
                {
                    variable = bb.AddVariable(pair.key, typeof(string));
                    variable.value = pair.value;
                }
            }
        }
    }
}
