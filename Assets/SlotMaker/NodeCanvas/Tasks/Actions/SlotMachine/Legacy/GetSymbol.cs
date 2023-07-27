using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents.Legacy
{

[Category("★ BagelCode/SlotMachine/Legacy")]
public class GetSymbol : ActionTask
{
	public BBParameter<int> column;
	public BBParameter<int> row;
	public BBParameter<int> index;

	public BBParameter<BaseSymbol> saveAs;

	protected override string info
	{
		get { return string.Format("[Legacy]GetSymbol({0},{1} or GetSymbol({2}))", column, row, index); }
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

		saveAs.value = slotMachine.GetSymbol(spotColumn, spotRow);
		EndAction();
	}
}

}
