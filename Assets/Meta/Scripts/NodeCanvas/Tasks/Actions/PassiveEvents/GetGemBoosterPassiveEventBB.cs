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
public class GetGemBoosterPassiveEventBB : ActionTask<Blackboard> 
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
        get{ return string.Format("Gem Booster (All or Minimum) Multply Event"); }
    }

    protected override void OnExecute () 
    {
        minMultiplierEventID.value = 0;
        allMultiplierEventID.value = 0;

        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER_MULTIPLY);

        // Get Max Multiplier Event. 
        if(eventInfo != null)
        {
            id.value = eventInfo.id;
            allMultiplierEventID.value = eventInfo.id;
            mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
            startTimestamp.value = eventInfo.startTimestamp;
            endTimestamp.value = eventInfo.endTimestamp;
            eventInfoType.value = EventInfoType.GEM_BOOSTER_MULTIPLY;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "gemBoosterAllMultiplierEventID", eventInfo.id);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "gemBoosterAllMultiplierEventMultiplier", mutliplier.value);

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "gemBoosterEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "gemBoosterEventMultiplier", 1.0);
        }
        else
        {
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "gemBoosterAllMultiplierEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "gemBoosterAllMultiplierEventMultiplier", 1.0);

            // Get Min Multiplier Event. 
            eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER);

            if(eventInfo != null)
            {
                id.value = eventInfo.id;
                minMultiplierEventID.value = eventInfo.id;
                mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
                startTimestamp.value = eventInfo.startTimestamp;
                endTimestamp.value = eventInfo.endTimestamp;
                eventInfoType.value = EventInfoType.GEM_BOOSTER;

                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "gemBoosterEventID", eventInfo.id);
                BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "gemBoosterEventMultiplier", mutliplier.value);
            }
            else
            {
                id.value = 0;
                mutliplier.value = 1.0;
                startTimestamp.value = 0;
                endTimestamp.value = 0;
                eventInfoType.value = EventInfoType.UNKNOWN;
                
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "gemBoosterEventID", 0);
                BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "gemBoosterEventMultiplier", 1.0);
            }
        }

        EndAction(true);
    }
}

}
