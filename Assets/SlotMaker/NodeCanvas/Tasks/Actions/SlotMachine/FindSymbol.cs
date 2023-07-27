using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class FindSymbol : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;

    public BBParameter<int> symbolCount;
    public BBParameter<int> symbolIndex;

    public BBParameter<SymbolWin> saveAs;

    protected override string info
    {
        get { return string.Format("Find Symbol({0})", symbolIndex); }
    }

    protected override void OnExecute()
    {
        var symbolWin = new SymbolWin();
        Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;

        for (int i = 0; i < column.value; ++i)
        {
            for (int j = 0; j < row.value; ++j)
            {
                var symbolInfo = deck.GetSymbol(i, j);
                if (symbolInfo.symbol == symbolIndex.value)
                {
                    symbolWin.cells.Add(new Cell(i, j));
                }
            }
        }

        saveAs.value = symbolWin;
        EndAction();
    }
}

}
