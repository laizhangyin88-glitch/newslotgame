using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class LinkSuperStackingSlotSymbol : ActionTask
{
	public BBParameter<GameObject> slotMachine;
	public BBParameter<Cell> cell;
	public BBParameter<int> stackingRowCount;
	public BBParameter<int> stackingColCount;

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        for (int i = 0; i < stackingColCount.value; ++i)
        {
            for (int j = 0; j < stackingRowCount.value; ++j)
            {
                var symbol = sm.GetSymbol(cell.value.column + i, cell.value.row - j);
                var link = symbol.symbolInfo.link;

                link.rowCount = stackingRowCount.value;
                link.rowOffset = j;
                link.columnCount = stackingColCount.value;
                link.columnOffset = -i;
            }
        }

		EndAction();
	}
}

}
