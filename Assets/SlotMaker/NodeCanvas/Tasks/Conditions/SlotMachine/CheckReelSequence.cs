using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/SlotMachine")]
public class CheckReelSequence : ConditionTask<Transform>
{
    public BBParameter<int> reelIndex;
    public BBParameter<SpinState> condition;

    protected override string info { get { return string.Format("Reel Sequence({0})", reelIndex); } }

    protected override bool OnCheck()
    {
        var slotMachine = agent.GetComponent<SlotMachine>();
        var movement = slotMachine.GetReel(reelIndex.value).movement;
        return (int)movement.spinState >= (int)condition.value;
    }
}

}
