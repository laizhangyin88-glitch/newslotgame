using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class UnlinkSlotSymbol : ActionTask
{
	public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;

	protected override void OnExecute()
	{
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();

        BaseSymbol symbol = sm.GetSymbol(cell.value.column, cell.value.row);
        symbol.symbolInfo.link.Reset();

		EndAction();
	}
}

}
