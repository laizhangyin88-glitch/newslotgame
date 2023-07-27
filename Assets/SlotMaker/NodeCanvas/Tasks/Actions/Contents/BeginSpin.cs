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
public class BeginSpin : ActionTask
{
    protected override void OnExecute()
    {
        var cb = ContentBlackboard.Get();
        var parent = cb.GetValue<Blackboard>("current");
        var parentType = parent.GetValue<ContentNodeType>("type");

        BlackboardUtils.FindVariable<int>(cb, "game/spinCount").value += 1;

        Blackboard spin = null;
        if (parentType == ContentNodeType.Turn)
        {
            spin = (Blackboard)BlackboardUtils.GetOrCreateBlackboard(parent, "spin");
            BlackboardUtils.SetOrCreateValue<int>(spin, "spinIndex", 0);
        }
        else if (parentType == ContentNodeType.Bonus)
        {
            spin = (Blackboard)BlackboardUtils.CreateBlackboard("spin");
            var spinList = BlackboardUtils.AddToBlackboardList(parent, "spinList", spin);
            BlackboardUtils.SetOrCreateValue<int>(spin, "spinIndex", spinList.Count - 1);
        }
        else
        {
            Debug.LogError("[Content] BeginSpin failed. Spin always placed under Turn or Bonus.");
            return;
        }
        BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "spin", spin);
        cb.SetValue("current", spin);

        string guid = Guid.NewGuid().ToString();
        long timestamp = MetaSystem.GetTimeStamp();

        BlackboardUtils.SetOrCreateValue<ContentNodeType>(spin, "type", ContentNodeType.Spin);
        BlackboardUtils.SetOrCreateValue<Blackboard>(spin, "parent", parent);
        BlackboardUtils.SetOrCreateValue(spin, "uid", guid);
        BlackboardUtils.SetOrCreateValue<long>(spin, "earnCredit", 0L);
        BlackboardUtils.SetOrCreateValue<long>(spin, "singleCredit", 0L);
        BlackboardUtils.SetOrCreateValue<long>(spin, "multiplier", 1L);
        BlackboardUtils.SetOrCreateValue<long>(spin, "beginTime", timestamp);

        ContentEvent.BeginSpin(spin);

        EndAction();
    }
}

}
