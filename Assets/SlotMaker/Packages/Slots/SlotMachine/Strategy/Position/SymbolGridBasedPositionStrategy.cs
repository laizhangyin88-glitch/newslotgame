using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots.Strategy
{
    [CreateAssetMenu(fileName="New Position Strategy", menuName="SlotMaker2/Slot/Strategy/Symbol/Position")]
    public class SymbolGridBasedPositionStrategy : SymbolPositionStrategy
    {
        public Vector2 size;
        public Vector2 spacing;

        [ReadOnly]
        public Vector2 space;
        [ReadOnly]
        public Vector2 extents;

        public override Vector2 GetSize() { return space; }
        public override Vector2 GetExtents() { return extents; }

        public override Vector2 GetPosition(int columnCount, int rowCount, int x, int y, int symbolColumnCount, int symbolRowCount, int symbolColumnOffset, int symbolRowOffset)
        {
            var offset = new Vector2(x, -y);
            if (symbolColumnCount > 1) offset.x += ((float)(symbolColumnCount - 1) * 0.5f) - symbolColumnOffset;
            if (symbolRowCount > 1) offset.y -= ((float)(symbolRowCount - 1) * 0.5f) - symbolRowOffset;
            return new Vector2(-((float)(columnCount - 1) * 0.5f) + offset.x, ((float)(rowCount - 1) * 0.5f) + offset.y) * space;
        }

        public override Vector2 GetLocalPosition(int symbolColumnCount, int symbolRowCount, int symbolColumnOffset, int symbolRowOffset)
        {
            var offset = Vector3.zero;
            if (symbolColumnCount > 1) offset.x -= ((float)(symbolColumnCount - 1) * 0.5f) - symbolColumnOffset;
            if (symbolRowCount > 1) offset.y += ((float)(symbolRowCount - 1) * 0.5f) - symbolRowOffset;
            return offset * space;
        }

        public override int Repeat(RectTransform.Axis axis, float length)
        {
            switch (axis)
            {
            case RectTransform.Axis.Vertical:
                return Mathf.CeilToInt(length / space.y);
            case RectTransform.Axis.Horizontal:
                return Mathf.CeilToInt(length / space.x);
            }
            return 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            space = size + spacing;
            extents = space * 0.5f;
        }
#endif
    }
}