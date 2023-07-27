using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public static class SlotUtils
    {
        public static SymbolInfo GetSymbol(int slotIndex, int reelIndex, BaseReelStrip strip, int index)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var remap = slotData.mysterySymbolTable;
            var symbolInfo = strip.GetSymbol(index);
            var newSymbolInfo = (SymbolInfo)remap.GetSymbol(reelIndex, symbolInfo);

            if (strip.stripSubSymbolOffset != 0)
            {
                var subSymbolIndex = strip.CalcIndex(index + strip.stripSubSymbolOffset);
                newSymbolInfo.subSymbol.symbol = strip.GetSymbol(subSymbolIndex).subSymbol.symbol;
            }

            return newSymbolInfo;
        }

        public static SymbolInfo CreateSymbolInfo(int symbolIndex, SymbolMask symbolMask)
        {
            return new SymbolInfo
            {
                symbol = symbolIndex,
                mask = (symbolIndex >= 0) ? symbolMask.GetMask(symbolIndex) : SymbolAttribute.Reject
            };
        }
    }
}
