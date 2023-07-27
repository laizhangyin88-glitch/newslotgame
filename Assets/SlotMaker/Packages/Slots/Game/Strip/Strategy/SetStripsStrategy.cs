using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
    [Serializable]
    public abstract class SetStripsStrategy : ScriptableObject
    {
        public abstract void Set(List<SymbolStrips> strips, List<int> remap);
    }
}