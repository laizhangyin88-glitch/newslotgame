using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [Serializable]
    public class Deck : ICloneable
    {
        public List<int> stripIndices;
        public List<List<SymbolInfo>> deck;
        public List<List<SymbolInfo>> mask;
        public List<List<bool>> hitMap;

        private static SymbolInfo reject = new SymbolInfo{ mask = SymbolAttribute.Reject };

        public SymbolInfo GetSymbol(int reelIndex, int row)
        {
            if (SymbolMask.HasAnyAttribute(mask[reelIndex][row].mask, SymbolAttribute.Overlay | SymbolAttribute.Reject))
                return mask[reelIndex][row];
            else
                return deck[reelIndex][row];
        }

        public SymbolInfo GetDeckSymbol(int reelIndex, int row)
        {
            if (SymbolMask.HasAttribute(mask[reelIndex][row].mask, SymbolAttribute.Overlay))
                return reject;
            else if (SymbolMask.HasAttribute(mask[reelIndex][row].mask, SymbolAttribute.Reject))
                return mask[reelIndex][row];
            else
                return deck[reelIndex][row];
        }

        public SymbolInfo GetOriginalSymbol(int reelIndex, int row)
        {
            return deck[reelIndex][row];
        }

        public void UpdateHitMap(List<Cell> cellList)
        {
            for (int i = 0; i < cellList.Count; ++i)
            {
                int column = cellList[i].column;
                int row    = cellList[i].row;
                hitMap[column][row] = true;
            }
        }

        public object Clone()
        {
            var newDeck = new Deck();

            if (this.stripIndices != null)
                newDeck.stripIndices = ObjectUtils.DeepClone(this.stripIndices);

            if (this.deck != null)
                newDeck.deck   = SymbolInfo.CloneList2(this.deck);

            if (this.hitMap != null)
                newDeck.hitMap = CloneBooleanList2(this.hitMap);

            if (this.mask != null)
                newDeck.mask = SymbolInfo.CloneList2(this.mask);

            return newDeck;
        }

        private List<List<bool>> CloneBooleanList2(List<List<bool>> clonableList)
        {
            List<List<bool>> newCloneList = new List<List<bool>>();

            for (int i = 0; i < clonableList.Count; ++i)
            {
                List<bool> cloneReel = new List<bool>();
                for (int j = 0; j < clonableList[i].Count; ++j)
                {
                    cloneReel.Add(clonableList[i][j]);
                }
                newCloneList.Add(cloneReel);
            }
            return newCloneList;
        }
    }
}
