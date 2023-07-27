using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/SlotMachine")]
public class CheckSlotSequence : ConditionTask<Transform>
{
    public BBParameter<SpinState> spinState;
    public BBParameter<SpinState> condition;

    protected override string info { get { return string.Format("{0} >= {1}", spinState, condition); } }

    protected override bool OnCheck()
    {
        return (int)spinState.value >= (int)condition.value;
    }
}

}
