using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class RemoveSlotOverlaySymbol : ActionTask
{
    public BBParameter<Cell> cell;

	protected override void OnExecute()
	{
        Blackboard bb = ContentBlackboard.Get();
        BaseSlotMachine slotMachine = BlackboardUtils.FindVariable<GameObject>(bb, "slotMachine").value.GetComponent<BaseSlotMachine>();
		BaseSlotMachineOverlay so = slotMachine.overlay;

        so.RemoveSymbol(cell.value.column, cell.value.row);

		EndAction();
	}
}

}
