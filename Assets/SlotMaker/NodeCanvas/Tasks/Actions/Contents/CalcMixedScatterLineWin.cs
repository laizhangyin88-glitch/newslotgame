using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CalcMixedScatterLineWin : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<long> betCredit;
    public BBParameter<long> baseWager;
    public BBParameter<List<Blackboard>> payTable;
    public BBParameter<List<Blackboard>> payLine;
    public BBParameter<long> multiplier;
    public BBParameter<List<bool>> notMultiplied;
    public BBParameter<List<long>> symbolMultipliers;
    public BBParameter<bool> bidirectional;
    // public BBParameter<bool> excludeMaxLine; 추가 예정
    public BBParameter<List<SymbolWin>> saveAs;

    protected List<MixedLineWinInfo> mixedLineWinInfos;
    protected List<SymbolWin> winList;
    private const int wildSymbolIndex = 0;
    private SymbolMask symbolMask;

    protected long betPerLine { get { return betCredit.value / baseWager.value; } }

    protected int GetDirectionalColumn(int column, int direction)
    {
        return direction == 1 ? column : (this.column.value - 1) - column;
    }

    protected SymbolInfo GetSymbolInfo(int column, int row)
    {
        return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
    }

    protected long GetSymbolMultiplier(int symbolIndex)
    {
        if (symbolMultipliers.isNone || symbolMultipliers.isNull)
            return 1L;

        return symbolMultipliers.value[symbolIndex];
    }

    public long FindEarnCredit(int symbolIndex, int hitCount)
    {
        return payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
    }

    public List<int> GetPayLine(int lineIndex)
    {
        return payLine.value[lineIndex].GetValue<List<int>>("value");
    }

    protected long CalcOneLine(int lineIndex, int direction)
    {
        var symbolWin = new SymbolWin();
        int winSymbol    = -1;
        int wildCount = 0;
        long wildMultiplier = 1;
        int wildColumn = 0;

        long earnCredit = 0;
        long winMultiplier = 1;
        int winHitCount = 0;

        List<int> line = GetPayLine(lineIndex);
        List<int> hitCounts = new List<int>();
        List<long> multipliers = new List<long>();
        List<int> symbolColumns = new List<int>();
        List<SymbolAttribute> payMask = new List<SymbolAttribute>();

        //Initialize
        for (int i = 0; i < payTable.value.Count; i++)
        {
            hitCounts.Add(0);
            multipliers.Add(1);
            symbolColumns.Add(0);
            payMask.Add(0);
        }

        for (int column = 0; column < this.column.value; ++column)
        {
            int dColumn = GetDirectionalColumn(column, direction);
            int row = line[dColumn];
            var symbolInfo = GetSymbolInfo(dColumn, row);
            int symbolIndex = symbolInfo.symbol;

            // Wild Count & Normal Symbol Win
            // Wild Symbol Win => Use mixed win
            if (SymbolMask.HasWild(symbolInfo))
            {
                wildCount++;
                wildMultiplier *= GetSymbolMultiplier(symbolIndex);
                wildColumn |= (1 << column);
            }
            else
            {
                hitCounts[symbolIndex]++;
                if (!notMultiplied.value[symbolIndex])
                    multipliers[symbolIndex] *= GetSymbolMultiplier(symbolIndex);
                symbolColumns[symbolIndex] |= (1 << column);
                payMask[symbolIndex] = symbolInfo.mask;
            }

            // Mixed Symbol Win
            for (int count = 0; count < mixedLineWinInfos.Count; ++count)
            {
                if (SymbolMask.HasAttribute(symbolInfo.mask, mixedLineWinInfos[count].masks[0]))
                {
                    int payIndex = mixedLineWinInfos[count].payIndex;
                    hitCounts[payIndex]++;
                    if (!notMultiplied.value[payIndex])
                        multipliers[payIndex] *= GetSymbolMultiplier(symbolIndex);
                    symbolColumns[payIndex] |= (1 << column);
                    payMask[payIndex] = mixedLineWinInfos[count].masks[0];
                }
            }
        }

        // Find the highest pay
        for (int i = 0; i < payTable.value.Count; i++)
        {
            int tempHitCount = hitCounts[i];
            long tempMultiplier = multipliers[i];
            if (tempHitCount == 0)
                continue;
            if (!SymbolMask.HasAnyAttribute(payMask[i], SymbolAttribute.Wild))
            {
                if (SymbolMask.HasAnyAttribute(symbolMask.GetMask(i), SymbolAttribute.Wild))
                {
                    tempHitCount = wildCount;
                }
                else
                {
                    tempHitCount += wildCount;
                }
                tempMultiplier *= wildMultiplier;
            }
            long tempEarnCredit = FindEarnCredit(i, tempHitCount) * tempMultiplier;
            if (tempEarnCredit > earnCredit) {
                winSymbol = i;
                winMultiplier = tempMultiplier;
                winHitCount = tempHitCount;
                earnCredit = tempEarnCredit;
            }
        }

        // Add symbol win
        if (earnCredit > 0) {
            // Add win cells
            for (int column = 0; column < this.column.value; column++) {
                if (((symbolColumns[winSymbol] | wildColumn) & (1 << column)) > 0)
                {
                    int dColumn = GetDirectionalColumn(column, direction);
                    int row = line[dColumn];
                    symbolWin.cells.Add(new Cell(dColumn, row));
                }
            }

            symbolWin.symbolIndex = winSymbol;
            symbolWin.direction  = direction;
            symbolWin.lineIndex  = lineIndex+1;
            symbolWin.hitCount   = winHitCount;
            symbolWin.multiplier = multiplier.value * winMultiplier;
            symbolWin.earnCredit = earnCredit * betPerLine;
            winList.Add(symbolWin);
        }

        return symbolWin.earnCredit;
    }

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        symbolMask = slotData.symbolMask;
        mixedLineWinInfos = slotData.mixedLineWinInfos;

        long totalEarnCredit = 0L;
        winList = new List<SymbolWin>();
        for (int i = 0; i < payLine.value.Count; ++i)
        {
            totalEarnCredit += CalcOneLine(i, 1);
            if (bidirectional.value)
                totalEarnCredit += CalcOneLine(i, -1);
        }
        winList.Sort();

        var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
        BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
        ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

        saveAs.value = winList;

        EndAction();
    }
}

}
