using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class CascadingDeckSymbols : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<List<Cell>> cells;

    protected override string info { get { return string.Format("Cascading Deck"); } }

    protected override void OnExecute()
    {
        var deck = (Deck)ContentCustomData.GetSlotData(slotIndex.value).deck.Clone();
        var strips = GlobalReelStrips.Instance.GetReelStrips();

        int cellCount = cells.value.Count;
        for (int i = 0; i < cellCount; ++i)
        {
            var cell = cells.value[i];
            var strip = strips.GetReelStrip(cell.column);

            for (int j = cell.row; j > 0; --j)
            {
                deck.deck[cell.column][j] = deck.deck[cell.column][j-1];
            }
            deck.stripIndices[cell.column] = strip.CalcIndex(--deck.stripIndices[cell.column]);
            deck.deck[cell.column][0] = (SymbolInfo)strip.GetSymbol(deck.stripIndices[cell.column]).Clone();
        }
        
        ContentCustomData.GetSlotData(slotIndex.value).deck = deck;

        EndAction();
    }
}

}
