using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName = "New Symbol Remap", menuName = "SlotMaker2/Math/Symbol Remap")]
    public class SymbolRemap : ScriptableObject
    {
        public SymbolEntityList symbolList;

        [NonSerialized]
        public Dictionary<int, int> remap = new Dictionary<int, int>();

        public void Remap()
        {
            foreach (var pair in remap)
            {
                symbolList[pair.Key].Copy(symbolList[pair.Value]);
            }
        }
    }
}