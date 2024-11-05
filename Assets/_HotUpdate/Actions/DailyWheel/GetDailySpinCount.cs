using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bonus")]
public class GetDailySpinCount : ActionTask<Blackboard>
{
    public BBParameter<int> count;
    public BBParameter<MetaJackpotType> type;

    protected override string info
    {
        get { return string.Format("Get {0} spin count\n (if select UNKNOWN type, then get sum of daily/mega count)", type.value.ToString()); }
    }

    protected override void OnExecute()
    {
        if (type.value == MetaJackpotType.UNKNOWN)
        {
            int dailyCount = BlackboardQueryUtils.GetDailySpinCount(MetaJackpotType.DAILY_BONUS);
            int megaCount =  BlackboardQueryUtils.GetDailySpinCount(MetaJackpotType.DAILY_MEGA_WHEEL);
            count.value = dailyCount + megaCount;
        }
        else
        {
            count.value = BlackboardQueryUtils.GetDailySpinCount(type.value);
        }

        EndAction(true);
    }
}

}
