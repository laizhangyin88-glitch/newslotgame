using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

[Category("★ BagelCode/Contents")]
public class EndBonus : ActionTask
{
    protected override void OnExecute()
    {
        var cb = ContentBlackboard.Get();
        var bonus = cb.GetValue<Blackboard>("bonus");
        var parent = bonus.GetValue<Blackboard>("parent");
        var parentType = parent.GetValue<ContentNodeType>("type");
        long timestamp = MetaSystem.GetTimeStamp();

        BlackboardUtils.SetOrCreateValue<long>(bonus, "endTime", timestamp);

        ContentEvent.EndBonus(bonus);

        cb.SetValue("current", parent);
        if (parentType == ContentNodeType.Spin)
        {
            BlackboardUtils.SetOrCreateValue<Blackboard>(cb, "spin", parent);

            var spinParent = parent.GetValue<Blackboard>("parent");
            var spinParentType = spinParent.GetValue<ContentNodeType>("type");
            if (spinParentType == ContentNodeType.Turn)
            {
                cb.RemoveVariable("bonus");
            }
            else if (spinParentType == ContentNodeType.Bonus)
            {
                cb.SetValue("bonus", spinParent);
            }
            else
            {
                Debug.LogError("[Content] Spin circle detected.");
            }
        }
        else if (parentType == ContentNodeType.Bonus)
        {
            cb.SetValue("bonus", parent);
        }
        else
        {
            Debug.LogError("[Content] EndBonus failed. Bonus could not be placed under Turn.");
            return;
        }

        EndAction();
    }
}

}
