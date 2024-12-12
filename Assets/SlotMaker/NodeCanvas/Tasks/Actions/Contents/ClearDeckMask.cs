using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ClearDeckMask : ActionTask
{
    public BBParameter<int> slotIndex = 0;

    protected override void OnExecute()
    {
        var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        var visibleCounts = slotData.visibleCounts;

        int totalColumn = slotData.column;
        int totalRow = slotData.row;
        var deck = slotData.deck;
        deck.mask = new List<List<SymbolInfo>>();
        for (int column = 0; column < totalColumn; ++column)
        {
            var colMask = new List<SymbolInfo>();
            int visibleOffset = totalRow - visibleCounts[column];

            for (int row = 0; row < totalRow; ++row)
            {
                if (row < visibleOffset)
                    colMask.Add(new SymbolInfo{ mask = SymbolAttribute.Reject });
                else
                    colMask.Add(new SymbolInfo());
            }

            deck.mask.Add(colMask);
        }

        EndAction();
    }
}

}
