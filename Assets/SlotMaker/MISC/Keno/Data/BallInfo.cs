using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Keno
{
    [Serializable]
    public class BallInfo : IEquatable<BallInfo>, ICloneable
    {
        public int index;

        public int number;
        public long multiplier = 1;
        public SymbolAttribute mask;

        public DrawState drawState = DrawState.Drawn;
        public CatchState catchState = CatchState.Idle;
        
        public bool Equals(BallInfo other)
        {
            return number == other.number;
        }

        public bool Equals(SpotInfo spot)
        {
            return number == spot.number;
        }

        public void Reset()
        {
            drawState = DrawState.Drawn;
            catchState = CatchState.Idle;
        }

        public object Clone()
        {
            var newBall = new BallInfo();
            newBall.index = index;
            newBall.number = number;
            newBall.multiplier = multiplier;
            newBall.mask = mask;
            newBall.drawState = drawState;
            newBall.catchState = catchState;

            return newBall;
        }

        public static List<BallInfo> CloneList1(List<BallInfo> list1)
        {
            var newList = new List<BallInfo>();
            foreach (var bi in list1)
            {
                newList.Add((BallInfo)bi.Clone());
            }
            return newList;
        }
    }
}
