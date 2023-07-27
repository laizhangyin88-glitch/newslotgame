using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [Serializable]
    public class MysterySymbolTable
    {
        public List<List<SymbolInfo>> mysterySymbolReels;

        private bool initialized = false;
        public bool singleMysteryTable;

        public void Initialize(int reelCount, SymbolMask symbolMask, bool useSingleMysteryTable)
        {
            mysterySymbolReels = new List<List<SymbolInfo>>();
            for (int reelIndex = 0; reelIndex < reelCount; ++reelIndex)
            {
                List<SymbolInfo> mysterySymbols = new List<SymbolInfo>();
                for (int i = 0; i < symbolMask.mask.Count; ++i)
                {
                    mysterySymbols.Add(SlotUtils.CreateSymbolInfo(i, symbolMask));
                }
                mysterySymbolReels.Add(mysterySymbols);
            }

            initialized = true;
            singleMysteryTable = useSingleMysteryTable;
        }

        private int GetReelIndex(int reelIndex)
        {
            return singleMysteryTable ? 0 : reelIndex;
        }

        public void SetSymbol(int reelIndex, int index, int symbolIndex, SymbolMask symbolMask)
        {
            reelIndex = GetReelIndex(reelIndex);
            mysterySymbolReels[reelIndex][index].symbol = symbolIndex;
            mysterySymbolReels[reelIndex][index].mask   = symbolMask.GetMask(symbolIndex);
        }

        public void SetSymbolMultiplier(int reelIndex, int index, int symbolMultiplier)
        {
            reelIndex = GetReelIndex(reelIndex);
            mysterySymbolReels[reelIndex][index].multiplier = symbolMultiplier;
        }

        public SymbolInfo GetSymbol(int reelIndex, SymbolInfo symbolInfo)
        {
            reelIndex = GetReelIndex(reelIndex);
            SymbolInfo newSymbol = (SymbolInfo)symbolInfo.Clone();

            if (!initialized) 
                return newSymbol;

            int symbol = symbolInfo.symbol;
            newSymbol.symbol = mysterySymbolReels[reelIndex][symbol].symbol;
            newSymbol.mask = mysterySymbolReels[reelIndex][symbol].mask;
            newSymbol.multiplier = mysterySymbolReels[reelIndex][symbol].multiplier;

            return newSymbol;
        }
    }
}
