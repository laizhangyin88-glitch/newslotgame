using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/WelcomeBackBonus")]
public class CheckWelcomeBackBonus : ConditionTask
{
    protected override string info
    {
        get { return string.Format("Check Welcome Back Bonus"); }
    }

    protected override bool OnCheck() 
    {
        var welcomeBackBonus = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "welcomeBackRewardInfo");

        if(welcomeBackBonus != null)
        {
            return welcomeBackBonus.value.GetValue<long>("totalEarnCredit") != 0L;
        }

        return false;
    }
}

}
