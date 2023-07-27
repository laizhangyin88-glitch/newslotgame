using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ClearSpotDeckMask : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<int> column;
    public BBParameter<int> row;

    public BBParameter<int> reelCount;
    protected override void OnExecute()
    {
        var deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        deck.mask = new List<List<SymbolInfo>>();

        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
            int column = reelIndex % this.column.value;
            int row    = reelIndex / this.column.value;

            if (row == 0)
            {
                deck.mask.Add(new List<SymbolInfo>());
            }
            deck.mask[column].Add(new SymbolInfo());
        }

        EndAction();
    }
}

}
