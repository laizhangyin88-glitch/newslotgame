using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class LinkSuperStackingSlotSymbolSafe : ActionTask
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
            int columnIndex = cell.value.column + i;
            if (columnIndex >= sm.ColumnCount)
                break;

            for (int j = 0; j < stackingRowCount.value; ++j)
            {
                int rowIndex = cell.value.row - j;
                if (rowIndex < -sm.GetReel(columnIndex).topBuffer)
                    break;

                var symbol = sm.GetSymbol(columnIndex, rowIndex);
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
