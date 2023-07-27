using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CalcScatterSpotWin : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<int> reelCount;
    public BBParameter<long> betCredit;
    public BBParameter<long> baseWager;
    public BBParameter<List<long>> payTable;
    public BBParameter<long> multiplier = 1L; //NOTE: it should be set as ./spin/multiplier
    public BBParameter<int> symbolCount;

    public int symbolIndex;
    public SymbolAttribute symbolMask;
    public BBParameter<bool> continuous = false;

    public BBParameter<SymbolWin> saveAs;

    protected override string info
    {
        get { return string.Format("Calc Scatter ({0}) Spot Win", symbolMask); }
    }

    protected long betPerSymbol { get { return betCredit.value / baseWager.value; } }

    protected override void OnExecute()
    {
        var symbolWin = new SymbolWin();
        Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;

        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
            bool found = false;
            int column = reelIndex % this.column.value;
            int row    = reelIndex / this.column.value;

            var symbolInfo = deck.GetSymbol(column, row);
            if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
            {
                symbolWin.cells.Add(new Cell(column, row));
                found = true;
            }

            if (!found && !continuous.value) break;
        }

        if (symbolCount.isNone)
        {
            int  hitCount = Mathf.Min(symbolWin.cells.Count, payTable.value.Count);

            long earnCredit = (long)(payTable.value[hitCount - 1] * betPerSymbol * multiplier.value);

            if (earnCredit > 0L)
            {
                var bonus = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
                ContentBlackboardUtils.AddEarnCredit(bonus, earnCredit);

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
