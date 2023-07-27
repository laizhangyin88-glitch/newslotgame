using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class CascadeReel : Reel 
    {
        public override void PushFrontSymbols(int count)
        {
            beginRow -= count;

            int frontSymbolRow = endRow;
            if (symbols.Count > 0)
                frontSymbolRow = symbols[0].row;

            for (int i = 0; i < count; ++i)
            {
                index = strip.CalcIndex(--index);
                var symbol = slotMachine.CreateSymbol();

                symbol.transform.SetParent(symbolsTransform, false);
                symbol.transform.SetAsFirstSibling();
                symbol.Initialize(this, beginColumn, frontSymbolRow - 1 - i, index);
                UpdateFrontSymbolTransform(symbol, i);
                symbols.Insert(0, symbol);
            }
        }

        public void UpdateFrontSymbolTransform(BaseSymbol symbol, int pushOrder)
        {
    		var newPosition = CalcSymbolPosition(beginColumn, beginRow, symbol.column, beginRow - 1 - pushOrder);
            newPosition.z = GetZPosition(symbol.column, symbol.row);
            
            var rt = symbol.rectTransform;
            rt.anchoredPosition3D = newPosition;
            rt.sizeDelta = cellSize;
        }

        public override void Remove(int row, int count)
        {
            beginRow += count;

            RemoveSymbols(row, count);
            UpdateSymbolsZOrder();

            OnRemoved(row, count);
        }

        public override void RemoveSymbols(int row, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[row + i];
                symbol.Clear();
            }
            symbols.RemoveRange(row, count);
        }
    }
}
