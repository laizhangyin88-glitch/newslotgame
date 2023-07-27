using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
[Category("★ BagelCode/SlotMachine")]
public class GetFromToSymbolOffset : ActionTask<Blackboard>
{
	public BBParameter<int> current;
	public BBParameter<int> next;

	// Save As
	public BBParameter<int> column;
	public BBParameter<int> row;

    protected override string info
    {
        get { return string.Format("[Legacy] Get From To Symbol Offset"); }
    }

	protected override void OnExecute()
	{
		// Legacy Slot has one SlotData -> index 0
		var slotData = ContentCustomData.GetSlotData(0);
        int totalColumn = slotData.column;

		int currentColumn = current.value % totalColumn;
		int currentRow 	  = current.value / totalColumn;

		int nextColumn = next.value % totalColumn;
		int nextRow = next.value / totalColumn;

		column.value = nextColumn - currentColumn;
		row.value 	 = nextRow - currentRow;
		EndAction();
	}
}

}
