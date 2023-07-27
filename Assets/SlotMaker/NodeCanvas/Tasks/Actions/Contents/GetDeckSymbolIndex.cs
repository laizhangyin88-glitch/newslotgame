using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class GetDeckSymbolIndex : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;

	public BBParameter<int> saveAs;

	protected override void OnExecute()
	{
		Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
		saveAs.value = deck.GetOriginalSymbol(cell.value.column, cell.value.row).symbol;
		EndAction();
	}
}

}
