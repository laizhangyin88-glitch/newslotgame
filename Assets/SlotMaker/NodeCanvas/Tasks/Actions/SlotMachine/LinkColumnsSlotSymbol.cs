using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
[Description("Link columns using list of linked column indexes, parameter 'linkedColumns'.\nReels indexes must be ordered in a way, that reel containing main pivot symbol should go last in the list.\nImportant note: don't forget to unlink symbols when done using it.")]
public class LinkColumnsSlotSymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;
    public BBParameter<int> stackingRowCount;
    public BBParameter<int> stackingColCount;
    public BBParameter<List<int>> linkedColumns;

    protected override void OnExecute()
    {
        var baseSlotMachine = slotMachine.value.GetComponent<BaseSlotMachine>();
        int previousColumnIndex = 0;

        for (int i = 0; i < linkedColumns.value.Count; ++i)
        {
            int currentColumnIndex = linkedColumns.value[i];

            for (int j = 0; j < stackingRowCount.value; ++j)
            {
                var symbol = baseSlotMachine.GetSymbol(currentColumnIndex, cell.value.row - j);
                var link = symbol.symbolInfo.link;

                link.rowCount = stackingRowCount.value;
                link.rowOffset = j;
                link.columnCount = stackingColCount.value;
                link.columnOffset = i == 0 ? previousColumnIndex : previousColumnIndex - currentColumnIndex;
            }

            previousColumnIndex = currentColumnIndex;
        }

        EndAction();
    }
}

}
