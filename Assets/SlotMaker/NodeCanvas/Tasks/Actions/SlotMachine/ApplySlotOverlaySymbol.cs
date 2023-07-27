using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class ApplySlotOverlaySymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;

    protected override string info { get { return string.Format("Apply Slot OverlaySymbol({0})", cell); } }

    protected override void OnExecute()
    {
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        int hashCode = Cell.GetHashCode(cell.value.column, cell.value.row);
        sm.GetOverlaySymbol(hashCode).Apply();
        EndAction();
    }
}

}
