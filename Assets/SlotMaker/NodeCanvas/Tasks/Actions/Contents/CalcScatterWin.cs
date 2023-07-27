using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CalcScatterWin : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<int> columnOffset = 0;
    public BBParameter<int> rowOffset = 0;
    public BBParameter<long> betCredit;
    public BBParameter<List<long>> payTable;
    public BBParameter<long> multiplier = 1L; //NOTE: it should be set as ./spin/multiplier
    public BBParameter<int> symbolCount;

    public int symbolIndex;
    public SymbolAttribute symbolMask;
    public BBParameter<bool> continuous = false;
    public BBParameter<string> creditPath = "./spin";

    public BBParameter<SymbolWin> saveAs;

    protected override string info
    {
        get { return string.Format("Calc Scatter({0}) Win", symbolMask); }
    }

    protected override void OnExecute()
    {
        var symbolWin = new SymbolWin();
        Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        for (int i = 0; i < column.value; ++i)
        {
            int dColumn = i + columnOffset.value;
            bool found = false;
            for (int j = 0; j < row.value; ++j)
            {
                int dRow = j + rowOffset.value;
                var symbolInfo = deck.GetSymbol(dColumn, dRow);
                if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
                {
                    symbolWin.cells.Add(new Cell(dColumn, dRow));
                    found = true;
                }
            }
            if (!found && !continuous.value) break;
        }

        if (symbolCount.isNone)
        {
            int  hitCount = Mathf.Min(symbolWin.cells.Count, payTable.value.Count);
            long earnCredit = (long)(payTable.value[hitCount - 1] * betCredit.value * multiplier.value);

            if (earnCredit > 0L)
            {
                if (!string.IsNullOrEmpty(creditPath.value))
                {
                    var bonus = BlackboardUtils.FindVariable<Blackboard>(null, creditPath.value).value;
                    ContentBlackboardUtils.AddEarnCredit(bonus, earnCredit);                    
                }

                symbolWin.hitCount    = hitCount;
                symbolWin.multiplier = multiplier.value;
                symbolWin.earnCredit  = earnCredit;
                symbolWin.symbolIndex = symbolIndex;
                saveAs.value = symbolWin;
            }
        }
        else
        {
            if (symbolCount.value <= symbolWin.cells.Count)
            {
                symbolWin.hitCount    = symbolWin.cells.Count;
                symbolWin.symbolIndex = symbolIndex;
                saveAs.value = symbolWin;
            }
        }

        EndAction();
    }
}

}
