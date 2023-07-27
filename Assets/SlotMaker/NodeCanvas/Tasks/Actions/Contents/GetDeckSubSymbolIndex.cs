using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetDeckSubSymbolIndex : ActionTask
{
    // This action is the equivalent of GetDeckSymbolIndex for SubSymbol
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;

    public BBParameter<int> saveAs;

    protected override string info
    {
        get { return string.Format("Get Deck SubSymbol Index ({0}))", cell); }
    }

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        Deck deck = slotData.deck;

        var deckSymbolInfo = deck.GetOriginalSymbol(cell.value.column, cell.value.row);
        saveAs.value = deckSymbolInfo.subSymbol.symbol;

        EndAction();
    }
}

}
