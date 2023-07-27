using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class CreateSpotSlotMachine : ActionTask<Transform>
{
    public BBParameter<int> column;
    public BBParameter<int> row;

    public BBParameter<int> reelCount;
    public BBParameter<List<int>> visibleCounts;

    protected override void OnExecute()
    {
        var slotMachine = agent.GetComponent<SlotMachine>();
        var isStatic = slotMachine.staticReel;
        for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
        {
            int beginRow = reelIndex / column.value;
            int endRow   = beginRow + visibleCounts.value[reelIndex];
            int beginColumn = reelIndex % column.value;
            if (isStatic)
            {
                slotMachine.CreateReel(reelIndex, beginColumn, beginRow, beginColumn + 1, endRow, 0);
            }
            else
            {
                slotMachine.CreateReel(beginColumn, beginRow, beginColumn + 1, endRow, 0);
            }
        }
        slotMachine.Shuffle();
        ContentCustomData.GetSlotData(slotMachine.slotIndex).slotMachine = slotMachine.gameObject;
        EndAction();
    }
}

}
