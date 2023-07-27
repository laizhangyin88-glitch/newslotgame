using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
    [Serializable]
    public abstract class SymbolPositionStrategy : ScriptableObject
    {
        public abstract Vector2 GetSize();
        public abstract Vector2 GetExtents();
        public abstract Vector2 GetPosition(int columnCount, int rowCount, int x, int y, int symbolColumnCount, int symbolRowCount, int symbolColumnOffset, int symbolRowOffset);
        public abstract Vector2 GetLocalPosition(int symbolColumnCount, int symbolRowCount, int symbolColumnOffset, int symbolRowOffset);
        public abstract int Repeat(RectTransform.Axis axis, float length);
    }
}