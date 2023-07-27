using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class GetSlotMachine : ActionTask
{
    public BBParameter<int> slotIndex;
	public BBParameter<GameObject> saveAs;

	protected override string info { get { return string.Format("{0} = GetSlotMachine({1})", saveAs, slotIndex); } }

	protected override void OnExecute()
	{
        saveAs.value = ContentCustomData.GetSlotData(slotIndex.value).slotMachine;
		EndAction();
	}
}

}
