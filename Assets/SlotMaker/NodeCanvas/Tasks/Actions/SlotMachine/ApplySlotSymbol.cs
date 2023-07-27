using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class ApplySlotSymbol : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<Cell> cell;

    protected override string info { get { return string.Format("Apply Slot Symbol({0})", cell); } }

    protected override void OnExecute()
    {
        BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        sm.GetSymbol(cell.value.column, cell.value.row).Apply();
        EndAction();
    }
}

}
