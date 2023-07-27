using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ProjectionDeckMask : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;

	protected override string info
	{
        get { return string.Format("Projection Deck Mask by ({0})", cell); }
	}

	protected override void OnExecute()
	{
		Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        SymbolInfo maskedSymbol = deck.mask[cell.value.column][cell.value.row];
        SymbolInfo symbol = deck.deck[cell.value.column][cell.value.row];

        symbol.symbol = maskedSymbol.symbol;
        symbol.mask   = (SymbolAttribute)OperationUtils.Operate((int)maskedSymbol.mask, (int)SymbolAttribute.Overlay, BitwiseOperationMethod.Subtract);

        deck.mask[cell.value.column][cell.value.row] = new SymbolInfo();

        EndAction();
	}
}

}
