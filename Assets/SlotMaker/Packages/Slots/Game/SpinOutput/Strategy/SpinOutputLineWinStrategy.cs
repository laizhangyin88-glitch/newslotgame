using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
    [Serializable]
    public abstract class SpinOutputLineWinStrategy : ScriptableObject
    {
        public abstract void CalcLineWin(SpinOutputLineWin output, int lineIndex, WinningCombination.CombinationRule combinationRule, WinningCombination.CombinationMask combinationMask, bool includeFullLineWin);
    }
}