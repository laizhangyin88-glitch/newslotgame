using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [Serializable]
    public class SymbolInfo : IEquatable<SymbolInfo>, ICloneable
    {
        public int symbol;
        public int multiplier = 1;
        public SymbolAttribute mask;
        public SymbolLink link = new SymbolLink();
        public Dictionary<string, object> customData = null;
        public SubSymbolInfo subSymbol = null;

        public bool Equals(SymbolInfo other)
        {
            return symbol == other.symbol;
        }

        public object Clone()
        {
            var newSymbol = new SymbolInfo();
            newSymbol.symbol = this.symbol;
            newSymbol.multiplier = this.multiplier;
            newSymbol.mask = this.mask;
            newSymbol.link = (SymbolLink)link.Clone();
            newSymbol.subSymbol = (this.subSymbol == null) ? null : (SubSymbolInfo)subSymbol.Clone();

            if (this.customData == null)
            {
                newSymbol.customData = null;
            }
            else
            {
                newSymbol.customData = new Dictionary<string, object>();
                foreach (var pair in this.customData)
                {
                    newSymbol.customData.Add(pair.Key, pair.Value);
                }
            }

            return newSymbol;
        }

        public static List<SymbolInfo> CloneList1(List<SymbolInfo> list1)
        {
            var newList = new List<SymbolInfo>();
            foreach (var si in list1)
            {
                newList.Add((SymbolInfo)si.Clone());
            }
            return newList;
        }

        public static List<List<SymbolInfo>> CloneList2(List<List<SymbolInfo>> list2)
        {
            var newList = new List<List<SymbolInfo>>();
            foreach (var list1 in list2)
            {
                newList.Add(CloneList1(list1));
            }
            return newList;
        }
    }
}
