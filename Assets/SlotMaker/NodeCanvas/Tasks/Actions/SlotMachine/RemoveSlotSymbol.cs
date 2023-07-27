using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class RemoveSlotSymbol : ActionTask<Transform>
{
    public BBParameter<List<Cell>> cells;
	protected override void OnExecute()
	{
        var slotMachine = agent.GetComponent<BaseSlotMachine>();
        
        var offset = new Dictionary<int, int>();
        int cellCount = cells.value.Count;
        for (int i = 0; i < cellCount; ++i)
        {
            var cell = cells.value[i];
            var reel = slotMachine.GetReel(cell.column);

            if (offset.ContainsKey(cell.column))
                offset[cell.column] += 1;
            else
                offset[cell.column] = 0;

            reel.Remove(cell.row - offset[cell.column], 1);
        }

		EndAction();
	}
}

}
