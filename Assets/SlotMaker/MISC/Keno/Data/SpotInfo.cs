using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno
{
    [Serializable]
    public class SpotInfo : IEquatable<SpotInfo>, ICloneable
    {
        public int index;

        public int number;
        public long multiplier = 1;
        public SymbolAttribute mask;

        public MarkState markState = MarkState.Idle;
        public CatchState catchState = CatchState.Idle;
        
        public bool Equals(SpotInfo other)
        {
            return number == other.number;
        }

        public bool Equals(BallInfo ball)
        {
            return number == ball.number;
        }

        public void Reset()
        {
            markState = MarkState.Idle;
            catchState = CatchState.Idle;
        }

        public object Clone()
        {
            var newSpot = new SpotInfo();
            newSpot.index = index;
            newSpot.number = number;
            newSpot.multiplier = multiplier;
            newSpot.mask = mask;
            newSpot.markState = markState;
            newSpot.catchState = catchState;

            return newSpot;
        }

        public static List<SpotInfo> CloneList1(List<SpotInfo> list1)
        {
            var newList = new List<SpotInfo>();
            foreach (var si in list1)
            {
                newList.Add((SpotInfo)si.Clone());
            }
            return newList;
        }
    }
}
