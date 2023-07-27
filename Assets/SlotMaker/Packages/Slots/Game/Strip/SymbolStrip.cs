using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Strip", menuName="SlotMaker2/Math/Strip")]
	public class SymbolStrip : VariableList<OverridenSymbolEntity>
	{
		[InlineEditor]
		public SymbolEntityList symbols;

        [Button]
        [PropertyOrder(100)]
        public void AssignIndices()
        {
            for (int i = 0, count = value.Count; i < count; ++i)
            {
                value[i].index = i;
            }
        }

        [Button]
        [PropertyOrder(101)]
        public void AdjustRowOffset()
        {
            int offset = -1;
            for (int i = 0, count = value.Count; i < count; ++i)
            {
                if (value[i].rowCount > 1)
                {
                    value[i].rowOffset = ++offset;
                }
                else
                {
                    value[i].rowOffset = 0;
                    offset = -1;
                }
            }
        }

        public int randomIndex { get { return UnityEngine.Random.Range(0, Count); } }

        public void SetStrip(List<int> strip)
        {
            Clear();
            foreach (var index in strip)
            {
                Add(new OverridenSymbolEntity{ master = symbols[index] });
            }
            AssignIndices();
            AdjustRowOffset();
        }

        public int GetCircleIndex(int index)
        {
            int count = Count;
            while (index < 0)
                index += count;
            if (index >= count)
                index %= count;
            return index;
        }

        public OverridenSymbolEntity GetSymbol(int index)
        {
            return value[GetCircleIndex(index)];
        }

        public OverridenSymbolEntity GetRandomSymbol()
        {
            return value[randomIndex];
        }
	}
}