using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class SetSymbolIndex : ActionTask<Blackboard>
{
    public BBParameter<int> slotIndex = 0;
	public BBParameter<int> column;
	public BBParameter<int> row;
	public BBParameter<int> index;

    public BBParameter<int> symbolIndex;

	public SymbolAttribute attribute;

	protected override string info
	{
		get { return string.Format("[Legacy]SetSymbolIndex({0},{1} or SetSymbolIndex({2}))", column, row, index); }
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

		deck.deck[spotColumn][spotRow].symbol = symbolIndex.value;
		deck.deck[spotColumn][spotRow].mask   = attribute;
		EndAction();
	}
}

}
