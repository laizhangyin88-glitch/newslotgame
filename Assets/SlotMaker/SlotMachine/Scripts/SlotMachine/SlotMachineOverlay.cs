using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class SlotMachineOverlay : BaseSlotMachineOverlay
    {
        public enum OverlayStyle
        {
            DefaultOverlay,
            SpotOverlay,
            CombinationOverlay
        };
        public OverlayStyle overlayStyle = OverlayStyle.DefaultOverlay;

    	public override BaseSymbol AddSymbol(int column, int row, SymbolInfo newSymbol)
    	{
            var reel = FindReel(column, row);
            var symbol = slotMachine.CreateSymbol();
            symbol.column = column;
            symbol.row = row;
            symbol.Initialize(reel, newSymbol);
            symbol.gameObject.name = string.Format("symbol {0}x{1}", column, row);

            int hashCode = symbol.GetHashCode();
            int siblingIndex = 0;
            int count = symbols.Count;
            for (; siblingIndex < count; ++siblingIndex)
            {
                if (hashCode < symbols[siblingIndex].GetHashCode())
                    break;
            }

            var symbolRect = symbol.rectTransform;
            symbolRect.SetParent(symbolsTransform, false);
            symbolRect.SetSiblingIndex(siblingIndex);
            
            symbolRect.sizeDelta = reel.cellSize;
            symbolRect.position = reel.rectTransform.position;
            
            Vector3 anchoredPosition3D = symbolRect.anchoredPosition3D + reel.CalcSymbolPosition(reel.BeginColumn, reel.BeginRow, column, row);
            anchoredPosition3D.z -= slotMachine.layoutGroup.spacing.w;
            symbolRect.anchoredPosition3D = anchoredPosition3D;

            symbols.Insert(siblingIndex, symbol);
            slotMachine.AddOverlaySymbol(symbol);

            return symbol;
    	}

        private BaseReel FindReel(int column, int row)
        {
            switch (overlayStyle)
            {
            case OverlayStyle.DefaultOverlay:
                return slotMachine.GetReel(column);
            case OverlayStyle.SpotOverlay:
                return slotMachine.GetReel(row * slotMachine.ColumnCount + column);
            case OverlayStyle.CombinationOverlay:
                return slotMachine.FindReel(column, row);
            }
            return null;
        }
    }
}
