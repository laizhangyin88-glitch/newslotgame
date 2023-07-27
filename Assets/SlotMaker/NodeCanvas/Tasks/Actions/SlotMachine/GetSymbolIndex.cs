using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class GetSymbolIndex : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
	public BBParameter<int> column;
	public BBParameter<int> row;
	public BBParameter<int> index;

    public BBParameter<int> saveAs;

    protected override string info
    {
		get { return string.Format("GetSymbolIndex({0},{1} or GetSymbolIndex({2}))", column, row, index); }
    }

    protected override void OnExecute()
    {
		var slotData = ContentCustomData.GetSlotData(slotIndex.value);
        Deck deck = slotData.deck;

        int spotColumn = 0;
		int spotRow    = 0;

		if (!index.isNone)
		{
			int totalColumn = slotData.column;
			spotColumn = index.value % totalColumn;
			spotRow    = index.value / totalColumn;
		}
		else
		{
			spotColumn = column.value;
			spotRow    = row.value;
		}

	    var symbolInfo = deck.GetSymbol(spotColumn, spotRow);
        saveAs.value = symbolInfo.symbol;

        EndAction();
    }
}

}
