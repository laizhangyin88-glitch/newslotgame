using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class SetSymbol : ActionTask
{
	public BBParameter<int> column;
	public BBParameter<int> row;
	public BBParameter<int> index;

	public BBParameter<int> symbolIndex;
	public SymbolAttribute attribute;

	protected override string info
	{
		get { return string.Format("[Legacy]SetSymbol({0},{1} or SetSymbol({2}))", column, row, index); }
	}

	protected override void OnExecute()
    {
		Blackboard bb = ContentBlackboard.Get();
		BaseSlotMachine slotMachine = BlackboardUtils.FindVariable<GameObject>(bb, "slotMachine").value.GetComponent<BaseSlotMachine>();
		var slotData = ContentCustomData.GetSlotData(slotMachine.slotIndex);

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

		BaseSymbol symbol = slotMachine.GetSymbol(spotColumn, spotRow);
		symbol.symbolInfo.symbol = symbolIndex.value;
		symbol.symbolInfo.mask   = attribute;

		EndAction();
	}
}

}
