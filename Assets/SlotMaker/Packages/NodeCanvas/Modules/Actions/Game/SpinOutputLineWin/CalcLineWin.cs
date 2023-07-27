using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.LineWin
{
    [Category("✶ Slots/Win/Line")]
    public class CalcLineWin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<long> multiplier = 1L;
        public WinningCombination.CombinationRule combinationRule = WinningCombination.CombinationRule.LeftToRight;
        public WinningCombination.CombinationMask combinationMask = WinningCombination.CombinationMask.Combination1;
        public bool excludeBidFullLineWin = true;

        [BlackboardOnly]
        public BBParameter<long> earnCredit;
        [BlackboardOnly]
        public BBParameter<int> winCount;

        protected override void OnExecute()
        {
            SpinOutputLineWin lineWin = target.value as SpinOutputLineWin;

            lineWin.earnCredit = 0L;
            lineWin.multiplier = multiplier.value;
            lineWin.betCredit = betCredit.value;
            lineWin.betPerLine = betCredit.value / baseWager.value;
            lineWin.spots.Clear();
            lineWin.totalWin.Clear();
            
            for (int i = lineWin.paylines.begin, end = lineWin.paylines.end; i < end; ++i)
            {
                if ((combinationRule & WinningCombination.CombinationRule.LeftToRight) == WinningCombination.CombinationRule.LeftToRight)
                    lineWin.CalcLineWin(lineWin, i, WinningCombination.CombinationRule.LeftToRight, combinationMask, true);

                if ((combinationRule & WinningCombination.CombinationRule.RightToLeft) == WinningCombination.CombinationRule.RightToLeft)
                    lineWin.CalcLineWin(lineWin, i, WinningCombination.CombinationRule.RightToLeft, combinationMask, !excludeBidFullLineWin);
            }
            
            if (!earnCredit.isNone) 
                earnCredit.value = lineWin.earnCredit;

            if (!winCount.isNone) 
                winCount.value = lineWin.winCount;

            EndAction();
        }
    }
}