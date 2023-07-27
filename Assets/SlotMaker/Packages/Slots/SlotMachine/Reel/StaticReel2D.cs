using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.IoC;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class StaticReel2D : ReelInstance
    {
        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public SymbolPositionStrategy positionStrategy;

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public SymbolSortingOrderStrategy sortingOrderStrategy;

        [TabGroup("ReelInstance", "Setup")]
        public RectTransform content;

        [TabGroup("ReelInstance", "Dynamic")]
        [PropertyOrder(101)]
        public List<SymbolInstance> dynamicSymbols = new List<SymbolInstance>();

        public override void Initialize() 
        {
            for (int i = 0, count = content.childCount; i < count; ++i)
            {
                content.GetChild(0).GetComponent<SymbolInstance>().ReturnToPool();
            }
            dynamicSymbols = new List<SymbolInstance>(new SymbolInstance[columnCount * rowCount]);
        }

        public override void Shuffle() {}

        public override void Clear()
        {
            Initialize();
        }

        public override Vector3 GetReelPosition()
        {
            return content.position;
        }

        public override SymbolInstance GetSymbol(int x, int y)
        {
            return dynamicSymbols[(x - _xMin) * rowCount + (y - _yMin)];
        }

        public override void SetSymbol(int x, int y, SymbolInstance symbolInstance)
        {
            if (symbolInstance)
            {
                symbolInstance.rectTransform.SetParent(content, false);
                symbolInstance.rectTransform.SetAsLastSibling();

                var symbol = symbolInstance.symbol;
                symbolInstance.rectTransform.anchoredPosition = positionStrategy.GetPosition(
                    columnCount, rowCount, x - _xMin, y - _yMin, symbol.columnCount, symbol.rowCount, symbol.columnOffset, symbol.rowOffset);
            }

            dynamicSymbols[(x - _xMin) * rowCount + (y - _yMin)] = symbolInstance;

            UpdateSortingOrder();
        }

        public override void AddSymbol(int x, int y, SymbolInstance symbolInstance)
        {
            var oldSymbolInstance = GetSymbol(x, y);
            if (oldSymbolInstance != null)
            {
                oldSymbolInstance.AddSymbol(symbolInstance);
            }
            else
            {
                SetSymbol(x, y, symbolInstance);                
            }
        }

        public override Vector3 GetSymbolPosition(int x, int y)
        {
            return GetSymbol(x, y).rectTransform.position;
        }

        public override void Skip() {}
        public override void SendEvent(string eventName) {}

        public override SymbolInstance GetPatchingSymbol(int x, int y) { return null; }
        public override void ApplyFrontPatch() {}
        public override void ApplyBackPatch() {}

        public override void SendSymbolEvent(string eventName)
        {
            for (int i = 0, count = content.childCount; i < count; ++i)
            {
                content.GetChild(i).GetComponent<SymbolInstance>().SendEvent(eventName);
            }
        }

        protected void UpdateSortingOrder()
        {
            int columnCount_ = columnCount;
            int rowCount_ = rowCount;

            for (int column = columnCount_ - 1; column >= 0; --column)
            {
                for (int row = rowCount_ - 1; row >= 0; --row)
                {
                    var symbolInstance = dynamicSymbols[row * columnCount_ + column];
                    if (symbolInstance)
                    {
                        var symbol = symbolInstance.symbol;

                        symbolInstance.x = _xMin + column - symbol.columnOffset;
                        symbolInstance.y = _yMin + row - symbol.rowOffset;
                        symbolInstance.sortingOrder = sortingOrderStrategy.GetSortingOrder(_xMin + column, _yMin + row, _z);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (content == null) content = GetComponent<RectTransform>();
        }
#endif
    }
}