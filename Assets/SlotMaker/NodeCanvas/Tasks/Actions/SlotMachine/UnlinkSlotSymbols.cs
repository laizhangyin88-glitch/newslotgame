using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class UnlinkSlotSymbols : ActionTask
{
	public BBParameter<GameObject> slotMachine;

	protected override void OnExecute()
	{
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		sm.Visit((sb) =>
		{
			sb.symbolInfo.link.Reset();
		});

		EndAction();
	}
}

}
