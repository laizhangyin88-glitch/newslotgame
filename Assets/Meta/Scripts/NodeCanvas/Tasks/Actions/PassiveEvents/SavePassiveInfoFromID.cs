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
public class SavePassiveInfoFromID : ActionTask<Blackboard> 
{
    public BBParameter<int> id;

    private const string SAVE_EVENT_INFO = "saveEventInfo";

    protected override string info
    {
        get{ return string.Format("Save Passive Event = {0}", id); }
    }

    protected override void OnExecute () 
    {
        if(id.value > 0)
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetEventInfoFromID(id.value);
            BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), SAVE_EVENT_INFO);

            if(eventInfo != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), SAVE_EVENT_INFO);

                ClientAPI2Blackboard.Serialize(bb, eventInfo);
            }
        }

        EndAction(true);
    }
}

}
