using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class SetDeckSymbol : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;
    public BBParameter<int> symbolIndex;
    public BitwiseOperationMethod Operation = BitwiseOperationMethod.Set;
	public SymbolAttribute attribute;

	protected override string info
	{
        get { return string.Format("Set Deck Symbol({0}, {1}, ({2}){3}))", cell, symbolIndex, Operation.ToString(), attribute); }
	}

	protected override void OnExecute()
	{
		Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        SymbolInfo symbol = deck.GetOriginalSymbol(cell.value.column, cell.value.row);
        symbol.symbol = symbolIndex.value;
        symbol.mask = (SymbolAttribute)OperationUtils.Operate((int)symbol.mask, (int)attribute, Operation);
		EndAction();
	}
}

}
