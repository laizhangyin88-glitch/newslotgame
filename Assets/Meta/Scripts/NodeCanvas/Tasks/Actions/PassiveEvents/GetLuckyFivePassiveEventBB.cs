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
public class GetLuckyFivePassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<int> id;
    public BBParameter<long> endTimestamp;

    protected override string info
    {
        get{ return string.Format("Lucky Five Passive Event"); }
    }

    protected override void OnExecute () 
    {
        EventInfo luckyFiveEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.LUCKY_FIVE);
        
        if(luckyFiveEventInfo != null)
        {
            id.value = luckyFiveEventInfo.id;
            endTimestamp.value = luckyFiveEventInfo.endTimestamp;

            // BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "luckyFiveEventID", luckyFiveEventInfo.id);
        }
        else
        {
            id.value = 0;
            endTimestamp.value = 0;

            // BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "luckyFiveEventID", 0);
        }

        EndAction(true);
    }
}

}
