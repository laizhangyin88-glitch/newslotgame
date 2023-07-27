using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_purchase_reward : ActionTask<Blackboard>
{
    public BBParameter<List<Blackboard>> rewardList;

    protected override void OnExecute()
    {
        for (int i = 0; i < rewardList.value.Count; ++i)
        {
            var eventData = new Dictionary<string, object>();
            string customEventName;
            BiEventUtils.CommonAppendRewardEventData(eventData, "purchase_bonus_reward", rewardList.value[i], out customEventName);
            
            if (!string.IsNullOrEmpty(customEventName))
                Analytics.CustomEvent(customEventName, eventData);
        }
        
        EndAction();
    }
}

}
