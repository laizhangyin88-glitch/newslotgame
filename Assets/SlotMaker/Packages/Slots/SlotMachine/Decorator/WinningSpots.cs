using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class WinningSpots : SlotDecorator
    {
        public void TotalWin(SpinOutputWinningSubset eventData)
        {
            foreach (var spot in eventData.spots)
            {
                AddToSymbol(spot);
            }
        }

        public void SingleWin(SpinWin eventData)
        {
            foreach (var spot in eventData.spots)
            {
                AddToSymbol(spot);
            }
        }
    }
}