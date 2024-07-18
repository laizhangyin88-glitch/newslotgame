using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class FindScatterSymbolAtReel : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public SymbolAttribute symbolMask;

    public bool ignoreOverlay = false;

    public BBParameter<List<Cell>> saveAs;

    protected override string info
    {
        get { return string.Format("Find Scatter({0}) at Reel({1})", symbolMask, column); }
    }

    protected override void OnExecute()
    {
        saveAs.value = new List<Cell>();
        Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        
        for (int j = 0; j < row.value; ++j)
        {
            var symbolInfo = deck.GetSymbol(column.value, j);
            if (ignoreOverlay && SymbolMask.HasAttribute(symbolInfo, SymbolAttribute.Overlay)) continue;
            if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
            {
                saveAs.value.Add(new Cell(column.value, j));
            }
        }
        EndAction();
    }
}

}
