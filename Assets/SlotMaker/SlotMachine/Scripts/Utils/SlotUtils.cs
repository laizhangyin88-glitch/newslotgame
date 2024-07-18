using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public static class SlotUtils
    {
        public static SymbolInfo GetSymbol(int slotIndex, int reelIndex, BaseReelStrip strip, int index , bool isTest=false)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            MysterySymbolTable remap = slotData.mysterySymbolTable;
            SymbolInfo symbolInfo = strip.GetSymbol(index);
            SymbolInfo newSymbolInfo = (SymbolInfo)remap.GetSymbol(reelIndex, symbolInfo);

            if (strip.stripSubSymbolOffset != 0)
            {
                var subSymbolIndex = strip.CalcIndex(index + strip.stripSubSymbolOffset);
                newSymbolInfo.subSymbol.symbol = strip.GetSymbol(subSymbolIndex).subSymbol.symbol;
            }

            if (isTest)
            {
                ReelStrip temp = strip as ReelStrip;
                string res = "==@ {";
                for (int j=0; j<temp.strip.Count; j++)
                {
                    res += "\"" + j + "\":" + temp.strip[j].symbol + ",";
                }
                res += "}";
                res = res.Replace(",}", "}");
                Debug.Log(res);
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
