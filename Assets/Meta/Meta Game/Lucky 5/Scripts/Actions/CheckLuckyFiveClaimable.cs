using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Task.Condition
{

[Category("★ BagelCode/Meta Games/Lucky Five")]
public class CheckLuckyFiveClaimable : ConditionTask<Blackboard> 
{
    protected override string info
    {
        get { return "Check Lucky Five Claimable"; }
    }

    protected override bool OnCheck() 
    {
        return BlackboardQueryUtils.IsLuckyFiveClaimable();
    }
}

}