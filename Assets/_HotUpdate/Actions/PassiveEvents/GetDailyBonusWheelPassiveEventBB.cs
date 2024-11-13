using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/PassiveEvents")]
public class GetDailyBonusWheelPassiveEventBB : ActionTask<Blackboard>
{
    public BBParameter<int> id;
    public BBParameter<bool> isHideBadge;
    public BBParameter<double> mutliplier;
    public BBParameter<long> mutliplierNumerator;
    public BBParameter<long> startTimestamp;
    public BBParameter<long> endTimestamp;

    protected override string info
    {
        get{ return string.Format("Daily Bonus Wheel Multply Event"); }
    }

    protected override void OnExecute()
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.DAILY_WHEEL_MULTIPLY_FREEBIE);

        if(eventInfo != null)
        {
            id.value = eventInfo.id;
            isHideBadge.value = eventInfo.hideBadge;
            mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
            mutliplierNumerator.value = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            startTimestamp.value = eventInfo.startTimestamp;
            endTimestamp.value = eventInfo.endTimestamp;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "dailyWheelEventID", eventInfo.id);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "dailyWheelEventMultiplier", mutliplier.value);
        }
        else
        {
            id.value = 0;
            mutliplier.value = 1.0;
            mutliplierNumerator.value = NumberUtils.GetGlobalDenominator();
            startTimestamp.value = 0;
            endTimestamp.value = 0;
            isHideBadge.value = false;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "dailyWheelEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "dailyWheelEventMultiplier", 1.0);
        }

        EndAction(true);
    }
}

}
