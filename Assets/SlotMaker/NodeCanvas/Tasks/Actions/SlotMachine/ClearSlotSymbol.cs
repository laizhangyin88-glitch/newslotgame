using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class ClearSlotSymbol : ActionTask<Transform>
{
	protected override void OnExecute()
	{
        var slotMachine = agent.GetComponent<BaseSlotMachine>();
        var reels = slotMachine.GetReels();

        int reelCount = reels.Count;
        for (int i = 0; i < reelCount; ++i)
        {
            var reel = reels[i];
            reel.Remove(reel.beginRow, reel.RowCount);
        }

		EndAction();
	}
}

}
