using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class SetSlotAllReelIndex : ActionTask
{
    public BBParameter<GameObject> slotMachineObj;
    public BBParameter<int> reelCount;
    public BBParameter<List<int>> reelIndices;

    protected override void OnExecute()
    {
        var slotMachine = slotMachineObj.value.GetComponent<SlotMachine>();
        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
            slotMachine.GetReel(reelIndex).Shuffle(reelIndices.value[reelIndex]);
        }

        ContentCustomData.GetSlotData(slotMachine.slotIndex).slotMachine = slotMachine.gameObject;
        EndAction();
    }
}

}
