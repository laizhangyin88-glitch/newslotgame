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
public class GetPassiveEventBB : ActionTask<Blackboard> 
{
    public BBParameter<EventInfoType> eventType;
    public BBParameter<string> savePath;

    public BBParameter<int> saveAsID;
    public BBParameter<long> saveAsEndTimestamp;

    protected override string info
    {
        get{ return string.Format("Get {0} Passive Event", eventType); }
    }

    protected override void OnExecute () 
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(eventType.value);

        if(eventInfo != null)
        {
            saveAsID.value = eventInfo.id;
            saveAsEndTimestamp.value = eventInfo.endTimestamp;

            if(string.IsNullOrEmpty(savePath.value))
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), savePath.value, eventInfo.id);
        }
        else
        {
            saveAsID.value = 0;
            saveAsEndTimestamp.value = 0L;

            if(string.IsNullOrEmpty(savePath.value))
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), savePath.value, 0);
        }

        EndAction(true);
    }
}

}
