using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class FindStackSymbols : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;

    public bool ignoreOveray = true;

    public BBParameter<List<Cell>> saveAsStackSymbols;
    public BBParameter<List<int>>  saveAsStackSymbolCounts;

    protected override string info
    {
        get { return string.Format("FindStackSymbols"); }
    }

    protected override void OnExecute()
    {
        // var symbolWin = new SymbolWin();
        var deck   = ContentCustomData.GetSlotData(slotIndex.value).deck;
        var hitMap = deck.hitMap;

        saveAsStackSymbols.value = new List<Cell>();
        saveAsStackSymbolCounts.value = new List<int>();

        for (int i = 0; i < column.value; ++i)
        {
            int stackRow = 0;
            int stackCount  = 0;
            int stackSymbol = -1;

            for (int j = 0; j < row.value; ++j)
            {
                int symbol = deck.GetSymbol(i,j).symbol;
                if (j == 0)
                {
                    stackCount  = 1;
                    stackSymbol = symbol;
                }
                else if (deck.GetSymbol(i,j).symbol == stackSymbol)
                {
                    ++stackCount;
                    stackRow = j;
                }
                else
                {
                    if (stackCount > 1)
                    {
                        saveAsStackSymbolCounts.value.Add(stackCount);
                        saveAsStackSymbols.value.Add(new Cell(i,stackRow));
                    }
                    stackCount  = 1;
                    stackSymbol = symbol;
                }
            }

            if (stackCount > 1)
            {
                saveAsStackSymbolCounts.value.Add(stackCount);
                saveAsStackSymbols.value.Add(new Cell(i,stackRow));
            }
        }

        for (int i = 0; i < saveAsStackSymbols.value.Count; ++i)
        {
            Cell cell = saveAsStackSymbols.value[i];
            int stackCount = saveAsStackSymbolCounts.value[i];
            int stackColumn = cell.column;
            int stackRow  = cell.row;
            bool stackHit = false;

            for (int j = 0; j < stackCount; ++j)
            {
                if (hitMap[stackColumn][stackRow])
                {
                    stackHit = true;
                    break;
                }
                --stackRow;
            }

            if (!stackHit)
            {
                saveAsStackSymbolCounts.value.RemoveAt(i);
                saveAsStackSymbols.value.RemoveAt(i);
                --i;
            }
        }

        EndAction();
    }
}

}
