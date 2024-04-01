using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [System.Serializable]
    public class SymbolWin : IComparable<SymbolWin> 
    {
        public long earnCredit;
        public long multiplier;
        public int? lineIndex;
        public int symbolIndex;
        public List<Cell> cells = new List<Cell>();
        public int hitCount;
        public int? wayCount;
        public int direction;

        public SymbolWin()
        {
            earnCredit = 0L;
            multiplier = 1L;
            lineIndex  = 0;
            symbolIndex = 0;
            cells       = new List<Cell>();
            hitCount    = 0;
            wayCount    = 1;
            direction   = 0;
        }

        public int CompareTo(SymbolWin other)
        {
            int ret = this.earnCredit.CompareTo(other.earnCredit);
            if (ret != 0) 
            {
                return ret * -1;
            }
            else
            {
                ret = Nullable.Compare<int>(this.lineIndex, other.lineIndex);
                if (ret != 0)
                {
                    return ret;   
                }
                else 
                {
                    ret = this.direction.CompareTo(other.direction);
                    return ret * -1;
                }
            }
        }
    }
}
