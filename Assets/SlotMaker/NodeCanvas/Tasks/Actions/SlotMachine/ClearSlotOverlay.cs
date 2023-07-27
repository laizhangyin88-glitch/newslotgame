using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ClearSlotOverlay : ActionTask
{
	public BBParameter<GameObject> slotMachine;

	protected override void OnExecute()
	{
		BaseSlotMachineOverlay so = slotMachine.value.GetComponent<BaseSlotMachine>().overlay;
		so.Clear();

		EndAction();
	}
}

}
