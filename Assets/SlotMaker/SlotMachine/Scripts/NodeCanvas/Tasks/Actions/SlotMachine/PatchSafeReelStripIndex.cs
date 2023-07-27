using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class PatchSafeReelStripIndex : ActionTask
{
	public BBParameter<GameObject> slotMachine;

	protected override void OnExecute()
	{
		BaseSlotMachine sm = slotMachine.value.GetComponent<BaseSlotMachine>();
		foreach (var reel in sm.reels)
		{
			

			var frontSymbol = reel.symbols[0].symbolInfo;
			int srcPatchCount = (frontSymbol.link.rowCount - frontSymbol.link.rowOffset) - 1;

			int dstIndex = reel.strip.CalcIndex(reel.index - 1);
			var dstSymbol = SlotUtils.GetSymbol(sm.slotIndex, reel.reelIndex, reel.strip, dstIndex);
			int dstPatchCount = dstSymbol.link.rowCount - dstSymbol.link.rowOffset;

			if (srcPatchCount != dstPatchCount)
			{
				if (dstSymbol.link.rowOffset == 0)
					dstPatchCount = 0;

				int patchCount = srcPatchCount - dstPatchCount;
				if (patchCount != 0)
					reel.index = reel.strip.CalcIndex(reel.index + patchCount);
			}
		}

		EndAction();
	}
}

}
