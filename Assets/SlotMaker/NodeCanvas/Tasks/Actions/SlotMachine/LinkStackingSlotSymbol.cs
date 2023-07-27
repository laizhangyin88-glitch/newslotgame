using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class LinkStackingSlotSymbol : ActionTask
{
	public BBParameter<GameObject> slotMachine;
	public BBParameter<Cell> cell;
	public BBParameter<int> stackingCount;

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		for (int i = 0; i < stackingCount.value; ++i)
		{
			var symbol = sm.GetSymbol(cell.value.column, cell.value.row - i);
			var link = symbol.symbolInfo.link;

			link.rowCount = stackingCount.value;
			link.rowOffset = i;
		}

		EndAction();
	}
}

}
