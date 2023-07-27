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
public class GetChallengeClaimPassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<int> id;
    public BBParameter<double> mutliplier;
    public BBParameter<long> multiplierNumerator;
    public BBParameter<long> startTimestamp;
    public BBParameter<long> endTimestamp;

    protected override string info
    {
        get{ return string.Format("Challenge Claim Multply Event"); }
    }

    protected override void OnExecute () 
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);

        if(eventInfo != null)
        {
            id.value = eventInfo.id;
            mutliplier.value = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
            multiplierNumerator.value = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
            startTimestamp.value = eventInfo.startTimestamp;
            endTimestamp.value = eventInfo.endTimestamp;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "challengeClaimEventID", eventInfo.id);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "challengeClaimEventMultiplier", mutliplier.value);
        }
        else
        {
            id.value = 0;
            mutliplier.value = 1.0;
            multiplierNumerator.value = NumberUtils.GetGlobalDenominator();
            startTimestamp.value = 0;
            endTimestamp.value = 0;
            
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "challengeClaimEventID", 0);
            BlackboardUtils.SetOrCreateValue<double>(MainBlackboard.Get(), "challengeClaimEventMultiplier", 1.0);
        }

        EndAction(true);
    }
}

}
