using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetSlotSpotPivotSymbol : ActionTask
{
	public BBParameter<GameObject> slotMachine;
	public BBParameter<Cell> cell;

	public BBParameter<int> column;
	public BBParameter<int> row;

	public BBParameter<GameObject> saveAs;

	protected override string info { get { return string.Format("{0} = GetSlotSpotPivotSymbol({1})", saveAs, cell); } }

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		int reelIndex = cell.value.row * column.value + cell.value.column;
		saveAs.value = sm.GetPivotSymbol(reelIndex, cell.value.row).gameObject;

		EndAction();
	}
}

}
