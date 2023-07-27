using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ExpandSlotMachine : ActionTask
{
    public BBParameter<GameObject> slotMachine;
    public BBParameter<List<int>> visibleCounts;

    protected override void OnExecute()
    {
        var sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		var reels = sm.GetReels();
		for (int i = 0; i < reels.Count; ++i)
		{
			var reel = reels[i];
			int visibleCount = visibleCounts.value[i];
			int rowCount = reel.RowCount;
			if (visibleCount > rowCount)
			{
				reel.ExpandTop(visibleCount - rowCount);
			}
			else if (visibleCount < rowCount)
			{
				reel.ContractTop(rowCount - visibleCount);
			}
		}

        EndAction();
    }
}

}
