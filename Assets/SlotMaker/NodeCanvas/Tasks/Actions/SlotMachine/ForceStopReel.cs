using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class ForceStopReel : ActionTask<Transform>
{
    public BBParameter<int> reelIndex;

    protected override string info { get { return string.Format("ForceStop({0})", reelIndex); } }

    protected override void OnExecute()
    {
        var movement = agent.GetComponent<SlotMachine>().GetReel(reelIndex.value).movement;
        movement.ForceStop();

        EndAction();
    }
}

}
