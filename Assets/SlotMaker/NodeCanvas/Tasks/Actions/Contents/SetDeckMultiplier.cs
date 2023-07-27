using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class SetDeckMultiplier : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;
    public BBParameter<int> multiplier;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;

	protected override string info
	{
        get { return string.Format("Set Deck Symbol Multiplier({0}, {1}, {2}))", cell, multiplier, Operation.ToString()); }
	}

	protected override void OnExecute()
	{
		Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        SymbolInfo symbol = deck.GetOriginalSymbol(cell.value.column, cell.value.row);
        symbol.multiplier = OperationUtils.Operate(symbol.multiplier, multiplier.value, Operation);
		EndAction();
	}
}

}
