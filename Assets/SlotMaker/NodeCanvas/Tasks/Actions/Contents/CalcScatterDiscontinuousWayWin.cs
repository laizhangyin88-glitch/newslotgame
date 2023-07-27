using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CalcScatterDiscontinuousWayWin : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<long> betCredit;
    public BBParameter<long> baseWager;
    public BBParameter<List<long>> payTable;
    public BBParameter<long> multiplier = 1L;
    public BBParameter<string> creditPath = "./spin";
    public int symbolIndex;
    public SymbolAttribute symbolMask;
    public BBParameter<int> minHitColumnCount;
    public BBParameter<bool> includeWild;
    public BBParameter<SymbolWin> saveAs;


    protected long bet { get { return (baseWager.isNone || baseWager.isNull || baseWager.value <= 1L) ? betCredit.value : betCredit.value / baseWager.value; } }

    protected SymbolInfo GetSymbolInfo(int column, int row)
    {
        return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
    }

    public long FindEarnCredit(int hitCount)
    {
        return payTable.value[hitCount - 1];
    }

    protected SymbolWin CalcScatterDiscontinuousWay()
    {
        var symbolWin = new SymbolWin();

        int hitCount = 0;
        int nonWildHitCount = 0;
        int wayCount = 1;

        for (int column = 0; column < this.column.value; ++column)
        {
            int columnHitCount = 0;

            for (int row = 0; row < this.row.value; ++row)
            {
                var symbolInfo = GetSymbolInfo(column, row);
                bool isWildHit = includeWild.value && SymbolMask.HasWild(symbolInfo);

                if (SymbolMask.HasAttribute(symbolInfo, symbolMask) || isWildHit)
                {
                    columnHitCount++;
                    symbolWin.cells.Add(new Cell(column, row));
                    if (!isWildHit)
                    {
                        nonWildHitCount++;
                    }
                }
            }

            if (columnHitCount > 0)
            {
                ++hitCount;
                wayCount *= columnHitCount;
            }
        }

        if (hitCount == 0 || (!minHitColumnCount.isNone && hitCount < minHitColumnCount.value) || (includeWild.value && nonWildHitCount == 0))
        {
            return symbolWin;
        }

        long earnCredit = FindEarnCredit(hitCount);
        symbolWin.symbolIndex = symbolIndex;
        symbolWin.direction = 1;
        symbolWin.hitCount = hitCount;
        symbolWin.wayCount = wayCount;
        symbolWin.multiplier = multiplier.value;
        symbolWin.earnCredit = earnCredit * bet * symbolWin.multiplier * wayCount;
        return symbolWin;
    }

    protected override void OnExecute()
    {
        SymbolWin symbolWin = CalcScatterDiscontinuousWay();

        saveAs.value = symbolWin;
        if (symbolWin != null && !string.IsNullOrEmpty(creditPath.value))
        {
            var bonus = BlackboardUtils.FindVariable<Blackboard>(null, creditPath.value).value;
            ContentBlackboardUtils.AddEarnCredit(bonus, symbolWin.earnCredit);
        }

        EndAction();
    }
}

}
