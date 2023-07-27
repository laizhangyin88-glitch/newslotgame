using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class SetDeckMaskSubSymbol : ActionTask
{
    public BBParameter<int> slotIndex = 0;
    public BBParameter<Cell> cell;
    public BBParameter<int> subSymbolIndex;
	protected override string info
	{
        get { return string.Format("Set Deck Mask Sub Symbol({0}, {1})", cell, subSymbolIndex); }
	}

	protected override void OnExecute()
	{
		Deck deck = ContentCustomData.GetSlotData(slotIndex.value).deck;
        SymbolInfo symbol = deck.mask[cell.value.column][cell.value.row];
        if (symbol.subSymbol == null) 
        {
            symbol.subSymbol = new SubSymbolInfo();
        }
        symbol.subSymbol.symbol = subSymbolIndex.value;
		EndAction();
	}
}

}