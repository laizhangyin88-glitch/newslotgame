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
public class GetCoinBoosterPassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<int> id;
    public BBParameter<int> minMultiplierEventID;
    public BBParameter<int> allMultiplierEventID;
    public BBParameter<double> mutliplier;
    public BBParameter<long> startTimestamp;
    public BBParameter<long> endTimestamp;
    public BBParameter<EventInfoType> eventInfoType;

    protected override string info
    {
        get{ return string.Format("Coin Booster (All or Minimum) Multply Event"); }
    }

    protected override void OnExecute () 
    {
        minMultiplierEventID.value = 0;
        allMultiplierEventID.value = 0;

        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY);

        // Get Max Multiplier Event. 
        if(eventInfo != null)
        {
            id.value = eventInfo.id;
            allMultiplierEventID.value = eventInfo.id;
            mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
            startTimestamp.value = eventInfo.startTimestamp;
            endTimestamp.value = eventInfo.endTimestamp;
            eventInfoType.value = EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "coinBoosterAllMultiplierEventID", eventInfo.id);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "coinBoosterAllMultiplierEventMultiplier", mutliplier.value);

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "coinBoosterEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "coinBoosterEventMultiplier", 1.0);
        }
        else
        {
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "coinBoosterAllMultiplierEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "coinBoosterAllMultiplierEventMultiplier", 1.0);

            // Get Min Multiplier Event. 
            eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL);

            if(eventInfo != null)
            {
                id.value = eventInfo.id;
                minMultiplierEventID.value = eventInfo.id;
                mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                startTimestamp.value = eventInfo.startTimestamp;
                endTimestamp.value = eventInfo.endTimestamp;
                eventInfoType.value = EventInfoType.CREDIT_MULTIPLIER_WHEEL;

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "coinBoosterEventID", eventInfo.id);
                BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "coinBoosterEventMultiplier", mutliplier.value);
            }
            else
            {
                id.value = 0;
                mutliplier.value = 1.0;
                startTimestamp.value = 0;
                endTimestamp.value = 0;
                eventInfoType.value = EventInfoType.UNKNOWN;
                
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "coinBoosterEventID", 0);
                BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "coinBoosterEventMultiplier", 1.0);
            }
        }

        EndAction(true);
    }
}

}
