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
public class GetTimeBonusPassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<int> id;
    public BBParameter<double> mutliplier;
    public BBParameter<long> endTimestamp;
    public BBParameter<long> multiplierNumerator;

    protected override string info
    {
        get{ return string.Format("TimeBonus Event"); }
    }

    protected override void OnExecute () 
    {
        EventInfo timeBonusEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.TIME_BONUS);
        
        if(timeBonusEventInfo != null)
        {
            id.value = timeBonusEventInfo.id;
            mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(timeBonusEventInfo);
            endTimestamp.value = timeBonusEventInfo.endTimestamp;
            multiplierNumerator.value = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(timeBonusEventInfo);
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "timeBonusEventID", timeBonusEventInfo.id);
        }
        else
        {
            id.value = 0;
            mutliplier.value = 1.0;
            endTimestamp.value = 0;
            multiplierNumerator.value = NumberUtils.GetGlobalDenominator();
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "timeBonusEventID", 0);
        }

        EndAction(true);
    }
}

}
