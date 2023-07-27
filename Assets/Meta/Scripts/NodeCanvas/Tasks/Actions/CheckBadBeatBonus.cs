using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/BadBeatBonus")]
public class CheckBadBeatBonus : ConditionTask
{
    protected override string info
    {
        get { return string.Format("Check BBB"); }
    }

    protected override bool OnCheck() 
    {
        var bbbInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "bbbRewardInfo");

        if(bbbInfo != null)
        {
            return bbbInfo.value.GetValue<long>("credit") != 0L;
        }

        return false;
    }
}

}
