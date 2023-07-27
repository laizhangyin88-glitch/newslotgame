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
public class BeginBonus1 : ActionTask
{
    public BBParameter<Blackboard> bonusResult;

    protected override string info { get { return string.Format("Begin Bonus with {0}", bonusResult); } }

    protected override void OnExecute()
    {
        var cb = ContentBlackboard.Get();
        var parent = cb.GetValue<Blackboard>("current");

        var bonus = (Blackboard)BlackboardUtils.CreateBlackboard("bonus");
        var bonusList = BlackboardUtils.AddToBlackboardList(parent, "bonusList", bonus);
        BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "bonus", bonus);
        cb.SetValue("current", bonus);

        string guid = Guid.NewGuid().ToString();
        long timestamp = MetaSystem.GetTimeStamp();
        int bonusId = BlackboardUtils.FindVariable<int>(bonusResult.value, "bonusId").value;

        BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusIndex", bonusList.Count - 1);
        BlackboardUtils.SetOrCreateValue<ContentNodeType>(bonus, "type", ContentNodeType.Bonus);
        BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "parent", parent);
        BlackboardUtils.SetOrCreateValue(bonus, "uid", guid);
        BlackboardUtils.SetOrCreateValue<int>(bonus, "bonusId", bonusId);
        BlackboardUtils.SetOrCreateValue<Blackboard>(bonus, "response", bonusResult.value);
        BlackboardUtils.SetOrCreateValue<long>(bonus, "earnCredit", 0L);
        BlackboardUtils.SetOrCreateValue<long>(bonus, "singleCredit", 0L);
        BlackboardUtils.SetOrCreateValue<long>(bonus, "beginTime", timestamp);

        ContentEvent.BeginBonus(bonus);

        EndAction();
    }
}

}
