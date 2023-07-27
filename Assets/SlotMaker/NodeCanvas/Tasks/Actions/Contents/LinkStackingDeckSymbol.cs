using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class LinkStackingDeckSymbol : ActionTask
{
    public BBParameter<int> slotIndex = 0;
	public BBParameter<Cell> cell;
	public BBParameter<int> stackingCount;

	protected override void OnExecute()
	{
        Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        for (int i = 0; i < stackingCount.value; ++i)
        {
            var symbol = deck.GetOriginalSymbol(cell.value.column, cell.value.row - i);
            var link = symbol.link;
            link.rowCount = stackingCount.value;
			link.rowOffset = i;
        }

		EndAction();
	}
}

}
