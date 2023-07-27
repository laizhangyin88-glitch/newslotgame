using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class CreateClassicSlotMachine : ActionTask<Transform>
{
    public BBParameter<int> column;
    public BBParameter<int> row;
    public BBParameter<List<int>> visibleCounts;

    protected override void OnExecute()
    {
        var slotMachine = agent.GetComponent<SlotMachine>();
        var isStatic = slotMachine.staticReel;
        for (int i = 0; i < column.value; ++i)
        {
            int beginRow = row.value - visibleCounts.value[i] - 1;
            int endRow = beginRow + visibleCounts.value[i] + 2;
            if (isStatic)
            {
                slotMachine.CreateReel(i, i, beginRow, i + 1, endRow, 0);
            }
            else
            {
                slotMachine.CreateReel(i, beginRow, i + 1, endRow, 0);
            }
        }
        slotMachine.Shuffle();
        ContentCustomData.GetSlotData(slotMachine.slotIndex).slotMachine = slotMachine.gameObject;
        EndAction();
    }
}

}
