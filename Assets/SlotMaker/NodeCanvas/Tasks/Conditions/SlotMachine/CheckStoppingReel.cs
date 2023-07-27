using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/SlotMachine")]
public class CheckStoppingReel : ConditionTask<Transform>
{
    public BBParameter<int> reelIndex;

    protected override string info { get { return string.Format("IsStopping({0})", reelIndex); } }

    protected override bool OnCheck()
    {
        var movement = agent.GetComponent<SlotMachine>().GetReel(reelIndex.value).movement;
        return movement.IsStopping();
    }
}

}
