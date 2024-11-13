using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bingo")]
public class CheckDailyBIngoReward : ConditionTask
{
    protected override string info
    {
        get { return string.Format("Check Daily Bingo Reward"); }
    }

    protected override bool OnCheck() 
    {
        var dailyBingoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "dailyBingoInfo");

        if(dailyBingoBB != null)
        {
            var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(dailyBingoBB.value, "rewardList");

            if(rewardList != null)
                return rewardList.value.Count > 0;
        }

        return false;
    }
}

}
