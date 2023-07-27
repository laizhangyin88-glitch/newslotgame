using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class GetSubSymbolIndex : ActionTask<Blackboard>
{
    // This action is the equivalent of GetSymbolIndex for SubSymbol
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;

    public BBParameter<int> saveAs;

    protected override string info
    {
        get { return string.Format("Get SubSymbol Index ({0}))", cell); }
    }

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        Deck deck = slotData.deck;

        var symbolInfo = deck.GetSymbol(cell.value.column, cell.value.row);
        saveAs.value = symbolInfo.subSymbol.symbol;

        EndAction();
    }
}

}
