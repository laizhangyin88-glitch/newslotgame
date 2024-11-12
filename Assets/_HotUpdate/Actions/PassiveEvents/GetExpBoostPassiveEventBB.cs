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
public class GetExpBoostPassiveEventBB : ActionTask<Blackboard> // legacy
{
    public BBParameter<int> id;
    public BBParameter<double> mutliplier;
    public BBParameter<long> endTimestamp;

    protected override string info
    {
        get{ return string.Format("Exp Boost Event"); }
    }

    protected override void OnExecute () 
    {
        EventInfo expBoostEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.EXP_MULTIPLY);

        if(expBoostEventInfo != null)
        {
            id.value = expBoostEventInfo.id;
            mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(expBoostEventInfo);
            endTimestamp.value = expBoostEventInfo.endTimestamp;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "expBoostEventID", expBoostEventInfo.id);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "expBoostEventMultiplier", mutliplier.value);
        }
        else
        {
            id.value = 0;
            mutliplier.value = 1.0;
            endTimestamp.value = 0;
            
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "expBoostEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "expBoostEventMultiplier", 1.0);
        }

        EndAction(true);
    }
}

}
