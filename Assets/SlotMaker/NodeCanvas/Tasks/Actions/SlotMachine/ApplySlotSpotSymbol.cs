using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class ApplySlotSpotSymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<Cell> cell;

    protected override string info { get { return string.Format("Apply Slot Symbol({0})", cell); } }

    protected override void OnExecute()
    {
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        int reelIndex = cell.value.row * column.value + cell.value.column;
        sm.GetSymbol(reelIndex, cell.value.row).Apply();
        EndAction();
    }
}

}
