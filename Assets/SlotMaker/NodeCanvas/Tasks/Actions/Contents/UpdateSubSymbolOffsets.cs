using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class UpdateSubSymbolOffsets : ActionTask
{
    public BBParameter<List<int>> subSymbolOffsets;

    protected override void OnExecute()
    {
        var reelStrips = GlobalReelStrips.Instance.GetReelStrips();
        for (int i = 0; i < subSymbolOffsets.value.Count; i++)
        {
          reelStrips.GetReelStrip(i).stripSubSymbolOffset = subSymbolOffsets.value[i];
        }

        EndAction();
    }
}

}
