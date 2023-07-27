using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Condition
{

[Category("★ BagelCode/Friend")]
public class CheckIsFriend : ConditionTask<Blackboard> 
{
    public BBParameter<string> userId;

    protected override string info
    {
        get { return "Check " + userId + " is my Friend"; }
    }

    protected override bool OnCheck() 
    {
        var variableA = BlackboardUtils.FindVariable<string>(agent, userId.value);
        if (variableA.value == null)
        {
            return false;
        }

        return BagelCode.BlackboardQueryUtils.IsMyFriend(variableA.value);
    }
}

}
