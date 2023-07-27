using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ProjectionSymbolMask : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;

	protected override string info
	{
        get { return string.Format("Projection Symbol Mask by ({0})", cell); }
	}

	protected override void OnExecute()
	{
		Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        SymbolInfo maskedSymbol = deck.mask[cell.value.column][cell.value.row];
        SymbolInfo symbol = deck.deck[cell.value.column][cell.value.row];

        deck.mask[cell.value.column][cell.value.row].mask = (SymbolAttribute)OperationUtils.Operate((int)symbol.mask, (int)maskedSymbol.mask, BitwiseOperationMethod.Or);
        deck.mask[cell.value.column][cell.value.row].symbol = symbol.symbol;
        EndAction();
	}
}

}
