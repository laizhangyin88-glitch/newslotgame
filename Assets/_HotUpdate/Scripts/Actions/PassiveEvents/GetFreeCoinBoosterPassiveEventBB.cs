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
public class GetFreeCoinBoosterPassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<int> id;
    public BBParameter<long> startTimestamp;
    public BBParameter<long> endTimestamp;

    public BBParameter<EventInfoType> eventType;    // EventInfoType.FREE_COIN_BOOSTER
    public BBParameter<string> keyValue;            // freeCoinBoosterEventID

    protected override string info
    {
        get{ return string.Format("Free Coin Booster Event"); }
    }

    protected override void OnExecute () 
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(eventType.value);

        if(eventInfo != null)
        {
            id.value = eventInfo.id;
            startTimestamp.value = eventInfo.startTimestamp;
            endTimestamp.value = eventInfo.endTimestamp;

            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), keyValue.value, eventInfo.id);
        }
        else
        {
            id.value = 0;
            startTimestamp.value = 0;
            endTimestamp.value = 0;
            
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), keyValue.value, 0);
        }

        EndAction(true);
    }
}

}
