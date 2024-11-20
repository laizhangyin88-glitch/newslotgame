using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/DailyBoost")]
public class SetActivatedDailyBoostBB : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;

    [BlackboardOnly]
    public BBParameter<int>     leftCollectCount;

    [BlackboardOnly]
    public BBParameter<bool>    isCollectable;

    [BlackboardOnly]
    public BBParameter<long>    coins;

    [BlackboardOnly]
    public BBParameter<Blackboard> dailyBoostInfo;

    protected override string info
    {
        get { return "Set Activated DailyBoost BB"; }
    } 

    protected override void OnExecute()
    {
        BlackboardQueryUtils.UpdateDailyBoostState();

        isCollectable.value = false;
        dailyBoostInfo.value = null;
        leftCollectCount.value = 0;

        var bb = BlackboardUtils.FindVariable<Blackboard>( agent, valueA.value);
        if(bb != null)
        {
            leftCollectCount.value  = BlackboardUtils.FindVariable<int>(  bb.value, "leftCollectDayCount").value;
            isCollectable.value     = BlackboardUtils.FindVariable<bool>( bb.value, "isCollectable").value;
            var terminated          = BlackboardUtils.FindVariable<bool>( bb.value, "isTerminated");
            var earnCoins           = BlackboardUtils.FindVariable<long>( bb.value, "credit");

            int tier = TierUtils.GetMeTier();
            coins.value = TierUtils.GetTierFractionCoin(earnCoins.value, tier);
            coins.value = LevelUtils.GetLevelMultiplierNumeratorValue(coins.value, "dailyBoost");

            if(!terminated.value)
            {
                dailyBoostInfo.value = bb.value;
            }
        }
        

        EndAction();
    }
}

}
