using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class IgnoreOverlay : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BoolSetModes setTo = BoolSetModes.True;

	protected override string info
	{
		get { return string.Format("Ignore Overlay"); }
	}

	protected override void OnExecute()
	{
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        if (setTo == BoolSetModes.Toggle)
            sm.ignoreOverlay = !sm.ignoreOverlay;
        else 
            sm.ignoreOverlay = ((int)setTo == 1);
		EndAction();
	}
}

}
