using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/SlotMachine")]
public class SpinReel : ActionTask<Transform>
{
    public BBParameter<int> reelIndex;

    protected override string info { get { return string.Format("Spin({0})", reelIndex); } }

    protected override void OnExecute()
    {
        var reel = agent.GetComponent<SlotMachine>().GetReel(reelIndex.value);
        reel.movement.Spin();

        EndAction();
    }
}

}
