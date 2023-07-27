using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class EndGamble : ActionTask
{
    protected override void OnExecute()
    {
        Blackboard cb = ContentBlackboard.Get();
        var turn = cb.GetValue<Blackboard>("turn");
        var gamble = cb.GetValue<Blackboard>("gamble");

        long timestamp = MetaSystem.GetTimeStamp();
        BlackboardUtils.SetOrCreateValue<long>(gamble, "endTime", timestamp);

        cb.RemoveVariable("gamble");

        EndAction();
    }
}

}
