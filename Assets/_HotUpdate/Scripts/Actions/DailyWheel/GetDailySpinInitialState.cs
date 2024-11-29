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
public class GetDailySpinInitialState : ActionTask<Blackboard>
{
    public BBParameter<bool> megaWheelState;

    protected override string info
    {
        get { return string.Format("Get Mega wheel state"); }
    }

    protected override void OnExecute()
    {
        int dailyCount = BlackboardQueryUtils.GetDailySpinCount(MetaJackpotType.DAILY_BONUS);
        int megaCount =  BlackboardQueryUtils.GetDailySpinCount(MetaJackpotType.DAILY_MEGA_WHEEL);

        if (megaWheelState.value && (dailyCount > 0 && megaCount < 1))
        {
            megaWheelState.value = false;
        }
        else if (!megaWheelState.value && (megaCount > 0 && dailyCount < 1))
        {
            megaWheelState.value = true;
        }

        EndAction(true);
    }
}

}
