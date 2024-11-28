using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_mystery_gift_trigger : ActionTask<Blackboard>
{
    public BBParameter<List<Blackboard>> rewardList;
    
    protected override void OnExecute()
    {
        for (int i = 0; i < rewardList.value.Count; ++i)
        {
            var eventData = new Dictionary<string, object>();
            string customEventName;
            BiEventUtils.CommonAppendRewardEventData(eventData, "mystery_gift", rewardList.value[i], out customEventName);

            if (!string.IsNullOrEmpty(customEventName))
                Analytics.CustomEvent(customEventName, eventData);
        }
        
        EndAction();
    }
}

}
