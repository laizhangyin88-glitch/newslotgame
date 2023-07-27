using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetOverlaySymbol : ActionTask
{
	public BBParameter<GameObject> slotMachine;
	public BBParameter<Cell> cell;

	public BBParameter<GameObject> saveAs;

	protected override string info { get { return string.Format("{0} = GetSymbol({1})", saveAs, cell); } }

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();

        if (sm.GetOverlaySymbol(cell.value.GetHashCode()) != null)
        {
            saveAs.value = sm.GetOverlaySymbol(cell.value.GetHashCode()).gameObject;
        }
        else
        {
            saveAs.value = null;
        }

		EndAction();
	}
}

}
