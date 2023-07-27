using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CalcScatterLineWin : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<long> betCredit;
    public BBParameter<long> baseWager;
    public BBParameter<List<long>> payTable;
    public BBParameter<List<int>> freeSpinCounts;
    public BBParameter<List<Blackboard>> payLine;
    public BBParameter<long> multiplier = 1L; //NOTE: it should be set as ./spin/multiplier
    public BBParameter<bool> bidirectional;
    public BBParameter<bool> excludeMaxLine = false;

    public int symbolIndex;
    public SymbolAttribute symbolMask;

    public BBParameter<List<SymbolWin>> saveAs;
    public BBParameter<int> saveAsTotalFreeSpinCount;

    private List<SymbolWin> winList;
    private int totalFreeSpinCount;
    private SymbolMask mask;

    protected override string info
    {
        get { return string.Format("Calc ScatterLine({0}) Win", symbolMask); }
    }

    protected long betPerLine { get { return betCredit.value / baseWager.value; } }

    protected int GetDirectionalColumn(int column, int direction)
    {
        return direction == 1 ? column : (this.column.value - 1) - column;
    }

    protected SymbolInfo GetSymbolInfo(int column, int row)
    {
        return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
    }

    protected bool IsHit(ref SymbolInfo winSymbolInfo, SymbolInfo symbolInfo)
    {
        if (SymbolMask.HasReject(symbolInfo))
        {
            return false;
        }
        else if (winSymbolInfo.Equals(symbolInfo))
        {
            winSymbolInfo = symbolInfo;
            return true;
        }
        else if (SymbolMask.HasWild(symbolInfo))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public long FindEarnCredit(int hitCount)
    {
        if (payTable.isNone)
            return 0;

        return payTable.value[hitCount - 1];
    }

    public int FindEarnFreeSpinCount(int hitCount)
    {
        return freeSpinCounts.value[hitCount - 1];
    }

    public List<int> GetPayLine(int lineIndex)
    {
        return payLine.value[lineIndex].GetValue<List<int>>("value");
    }

    protected long CalcOneLine(int lineIndex, int direction)
    {
        var symbolWin = new SymbolWin();

        int hitCount = 0;
        // int wildHitCount = 0;
        SymbolInfo winSymbolInfo = SlotUtils.CreateSymbolInfo(-1, mask);
        // long lineMultiplier = 1L;
        List<int> line = GetPayLine(lineIndex);

        for (int column = 0; column < this.column.value; ++column)
        {
            int dColumn = GetDirectionalColumn(column, direction);
            int row = line[dColumn];
            var symbolInfo = GetSymbolInfo(dColumn, row);

            if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
            {
                winSymbolInfo = symbolInfo;
                ++hitCount;
            }
            else
            {
                break;
            }

            // TODO
            // cells 는 실제 히트된 cell 만 포함하고 있어야 합니다.
            // 현재 버전에서는 hit 되지 않은 cell 도 포함되고 있습니다.
            symbolWin.cells.Add(new Cell(dColumn, row));
        }

        if (hitCount <= 0)
            return 0L;

        if (hitCount == this.column.value)
        {
            if (direction == -1 && excludeMaxLine.value == true)
                return 0L;
        }

        int freeSpinCount = FindEarnFreeSpinCount(hitCount);
        if (freeSpinCount > 0)
        {
            long earnCredit = FindEarnCredit(hitCount);
            symbolWin.symbolIndex = winSymbolInfo.symbol;
            symbolWin.direction = direction;
            symbolWin.lineIndex = lineIndex + 1;
            symbolWin.hitCount  = hitCount;
            symbolWin.multiplier = multiplier.value;
            symbolWin.earnCredit = earnCredit * betPerLine * symbolWin.multiplier;
            totalFreeSpinCount += freeSpinCount;

            winList.Add(symbolWin);
        }

        return symbolWin.earnCredit;
    }

    protected override void OnExecute()
    {
        mask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;
        long totalEarnCredit = 0L;
        winList = new List<SymbolWin>();
        totalFreeSpinCount = 0;
        for (int i = 0; i < payLine.value.Count; ++i)
        {
            totalEarnCredit += CalcOneLine(i, 1);
            if (bidirectional.value)
                totalEarnCredit += CalcOneLine(i, -1);
        }
        winList.Sort();

        if (totalEarnCredit > 0L)
        {
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "scatterWinList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);
        }

        saveAs.value = winList;
        saveAsTotalFreeSpinCount.value = totalFreeSpinCount;
        EndAction();
    }
}

}
