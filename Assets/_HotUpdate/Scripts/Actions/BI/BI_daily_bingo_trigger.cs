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
public class BI_daily_bingo_trigger : ActionTask<Blackboard>
{
    public BBParameter<List<Blackboard>> rewardList;

    public BBParameter<int> currentClaimCount;
    public BBParameter<bool> isAuto;

    protected override void OnExecute()
    {
        for(int i=0; i<rewardList.value.Count; ++i)
        {
            var eventData = new Dictionary<string, object>();
            string customEventName;
            BiEventUtils.CommonAppendRewardEventData(eventData, "bingo_of_the_month", rewardList.value[i], out customEventName);
            eventData["current_claim_count"] = currentClaimCount.value;
            eventData["is_auto"] = isAuto.value;

            if (!string.IsNullOrEmpty(customEventName))
                Analytics.CustomEvent(customEventName, eventData);
        }

        EndAction();
    }
}

}
