using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker.Slots.Strategy
{
    [CreateAssetMenu(fileName = "New Index Based Line Win Strategy", menuName = "SlotMaker2/Math/Output/Strategy/Line Win/Index Based")]
    public class SpinOutputIndexBasedLineWinStrategy : SpinOutputLineWinStrategy
    {
        private const int WILD_INDEX = 0;

        public override void CalcLineWin(SpinOutputLineWin output, int lineIndex, WinningCombination.CombinationRule combinationRule, WinningCombination.CombinationMask combinationMask, bool includeFullLineWin)
        {
            var lineWin = new SpinLineWin();
            var singleLine = output.paylines.value[lineIndex];

            OverridenSymbolEntity winningSymbol = null;
            int hitCount = 0;
            int wildHitCount = 0;

            var visibleAreas = output.source.visibleAreas;
            int count = visibleAreas.Count;
            for (int i = 0; i < count; ++i)
            {
                int index = (combinationRule == WinningCombination.CombinationRule.LeftToRight) ? i : (count - 1) - i;
                var visibleArea = visibleAreas[index];
                int x = visibleArea.xMin;
                int y = singleLine[index];
                int z = 0;
                var symbol = output.source.GetBackSymbol(x, y, out z);

                if (SpinWin.IsHit(symbol, ref winningSymbol))
                    ++hitCount;
                else
                    break;

                if (winningSymbol.IsWild() && symbol.IsWild())
                    ++wildHitCount;

                lineWin.multiplier = SpinWin.OperateMultiplier(lineWin.multiplier, symbol.multiplier, output.symbolMultiplierOperationMethod);
                lineWin.spots.Add(new Cell3(x, y, z));
            }

            var winningCombination = output.paytable[winningSymbol.value];
            long winningPay = winningCombination[hitCount - 1];

            if (!winningCombination.HasCombinationMask(combinationMask))
                winningPay = 0L;

            if (wildHitCount > 0)
            {
                var wildCombination = output.paytable[WILD_INDEX];
                long wildPay = wildCombination[wildHitCount - 1];
                if (wildPay > winningPay)
                {
                    winningCombination = wildCombination;
                    hitCount = wildHitCount;
                    winningPay = wildPay;
                }
            }

            if (!includeFullLineWin && (hitCount == count))
                return;

            if (winningPay > 0L)
            {
                lineWin.lineIndex = lineIndex;
                lineWin.multiplier *= output.multiplier;
                lineWin.earnCredit = winningPay * lineWin.multiplier * output.betPerLine;
                lineWin.winningCombination = winningCombination;
                lineWin.combinationRule = combinationRule;
                lineWin.hitCount = hitCount;

                int spotsCount = lineWin.spots.Count;
                if (hitCount < spotsCount)
                    lineWin.spots.RemoveRange(hitCount, spotsCount - hitCount);
                foreach (var spot in lineWin.spots)
                {
                    output.spots.Add(spot);
                }

                output.totalWin.Add(lineWin);
                output.earnCredit += lineWin.earnCredit;
            }
        }
    }
}