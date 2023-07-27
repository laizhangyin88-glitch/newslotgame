using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/SlotMachine")]
public class GetReel : ActionTask
{
	public BBParameter<int> slotIndex = 0;

	public BBParameter<int> index;

	public BBParameter<BaseReel> saveAs;

	protected override string info
	{
		get { return string.Format("SlotMachine({0}).GetReel({1})", slotIndex, index); }
	}
    protected override void OnExecute()
    {
		GameObject slotMachine = ContentCustomData.GetSlotData(slotIndex.value).slotMachine;
		saveAs.value = slotMachine.GetComponent<BaseSlotMachine>().GetReel(index.value);

		EndAction();
	}
}

}
