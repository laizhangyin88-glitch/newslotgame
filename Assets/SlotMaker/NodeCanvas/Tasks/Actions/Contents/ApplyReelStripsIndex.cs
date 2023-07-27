using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class ApplyReelStripsIndex : ActionTask<Blackboard>
{
    public BBParameter<string> reelStripsIndex = "./game/reelSetIndex/nextIndex";

    protected override void OnExecute()
    {
        var index = BlackboardUtils.FindVariable<int>(agent, reelStripsIndex.value).value;
        GlobalReelStrips.Instance.index = index;

        EndAction();
    }
}

}
