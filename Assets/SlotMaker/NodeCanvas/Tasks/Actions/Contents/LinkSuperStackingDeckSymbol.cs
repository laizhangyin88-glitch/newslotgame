using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class LinkSuperStackingDeckSymbol : ActionTask
{
    public BBParameter<int> slotIndex = 0;
	public BBParameter<Cell> cell;
	public BBParameter<int> stackingRowCount;
	public BBParameter<int> stackingColCount;

	protected override void OnExecute()
	{
        Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        for (int i = 0; i < stackingColCount.value; ++i)
        {
            for (int j = 0; j < stackingRowCount.value; ++j)
            {
                var symbol = deck.GetOriginalSymbol(cell.value.column, cell.value.row - j);
                var link = symbol.link;
                link.rowCount = stackingRowCount.value;
                link.rowOffset = j;
                link.columnCount = stackingColCount.value;
                link.columnOffset = -i;
            }
        }

		EndAction();
	}
}

}
