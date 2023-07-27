using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ SlotMaker/SlotMachine")]
public class UndoReelStrip : ActionTask
{
    public BBParameter<int> reelIndex;

    protected override void OnExecute()
    {
        var strips = GlobalReelStrips.Instance.GetReelStrips();
        var strip = strips.GetReelStrip(reelIndex.value);

        strip.UnDo();

        EndAction();
    }
}

}
