using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetSlotCell : ActionTask
{
	public BBParameter<GameObject> slotMachine;
	public BBParameter<Cell> cell;

	public BBParameter<GameObject> saveAs;

	protected override string info { get { return string.Format("{0} = GetSymbol({1})", saveAs, cell); } }

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		saveAs.value = sm.GetSymbol(cell.value.column, cell.value.row).gameObject;
		EndAction();
	}
}

}
