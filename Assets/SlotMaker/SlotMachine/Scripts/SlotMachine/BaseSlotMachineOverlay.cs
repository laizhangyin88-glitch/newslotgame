using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class BaseSlotMachineOverlay : MonoBehaviour, ISlotMachineOverlay
    {
        public int symbolCount { get { return symbols.Count; } }

        public RectTransform rectTransform;
        public RectTransform symbolsTransform;
        public BaseSlotMachine slotMachine;

        public List<BaseSymbol> symbols = new List<BaseSymbol>();

        public void Clear()
        {
            int count = symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[i];
                symbol.Clear();
            }

            symbols.Clear();
            slotMachine.ClearOverlaySymbols();
        }

        public virtual BaseSymbol AddSymbol(BaseSymbol src)
        {
            var symbol = AddSymbol(src.column, src.row, src.symbolInfo);
            symbol.rectTransform.anchoredPosition3D = src.rectTransform.anchoredPosition3D;
            symbol.rectTransform.sizeDelta = src.rectTransform.sizeDelta;
            return symbol;
        }

        public virtual BaseSymbol AddSymbol(int column, int row, SymbolInfo newSymbol)
        {
            var symbol = slotMachine.CreateSymbol();
            symbol.transform.SetParent(symbolsTransform, false);
            symbol.column = column;
            symbol.row = row;
            symbol.symbolInfo = (SymbolInfo)newSymbol.Clone();

            symbols.Add(symbol);
            slotMachine.AddOverlaySymbol(symbol);

            return symbol;
        }

        public virtual bool RemoveSymbol(int column, int row)
        {
            int hashCode = Cell.GetHashCode(column, row);
    		if (slotMachine.HasOverlaySymbol(hashCode))
            {
                var symbol = slotMachine.GetOverlaySymbol(hashCode);
                symbols.Remove(symbol);
                slotMachine.RemoveOverlaySymbol(hashCode);
                symbol.Clear();
                
                return true;
            }

            return false;
        }
    }
}
