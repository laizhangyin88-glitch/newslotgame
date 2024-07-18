using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class UpdateSlotMachineIndices : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<List<int>> indices;
    public BBParameter<int> offset;

    protected override void OnExecute()
    {
        var sm = slotMachine.value.GetComponent<BaseSlotMachine>();
        int totalRow = ContentCustomData.GetSlotData(sm.slotIndex).row;

        sm.SetStripIndices(indices.value, totalRow + offset.value);

        EndAction();
    }
}

}
