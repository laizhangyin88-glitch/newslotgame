using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
    [CreateAssetMenu(fileName = "New Scatter Win Strategy", menuName = "SlotMaker2/Math/Output/Strategy/Scatter/Default")]
    public class SpinOutputScatterWinStrategy : ScriptableObject
    {
        public virtual void CalcScatterWin(SpinOutputScatterWin output)
        {
            var win = new SpinWin();
            win.combinationRule = output.paytable.combinationRule;

            var visibleAreas = output.source.visibleAreas;
            int count = visibleAreas.Count;
            for (int i = 0; i < count; ++i)
            {
                int index = (win.combinationRule == WinningCombination.CombinationRule.RightToLeft) ? (count - 1) - i : i;
                var visibleArea = visibleAreas[index];
                int x = visibleArea.xMin;
                for (int y = visibleArea.yMin; y < visibleArea.yMax; ++y)
                {
                    int z = 0;
                    var symbol = output.source.GetBackSymbol(x, y, out z);

                    if (SpinWin.IsHit(symbol, output.paytable.symbol, output.paytable.any))
                        win.spots.Add(new Cell3(x, y, z));
                    else if (win.combinationRule != WinningCombination.CombinationRule.Scatter)
                        break;

                    win.multiplier = SpinWin.OperateMultiplier(win.multiplier, symbol.multiplier, output.symbolMultiplierOperationMethod);
                }
            }

            win.hitCount = win.spots.Count;
            win.multiplier *= output.multiplier;
            win.earnCredit = output.paytable[win.hitCount - 1] * win.multiplier * output.betCredit;
            win.winningCombination = output.paytable;
            
            output.win = win;
        }
    }
}