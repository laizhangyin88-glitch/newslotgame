using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_slb : ActionTask<Blackboard>
{
    public BBParameter<string> slbInfoValue;
    public BBParameter<long>   endTimestamp;
    public BBParameter<string> contextID;

    protected override string info
    {
        get { return string.Format("BI Client SLB({0})", slbInfoValue); }
    }

    protected override void OnExecute()
    {
        var slbInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, slbInfoValue.value);
        if(slbInfoBB != null)
        {
            var slbID = BlackboardUtils.FindVariable<string>(slbInfoBB.value, "id");
            var priority = BlackboardUtils.FindVariable<int>(slbInfoBB.value, "priority");
            var comment = BlackboardUtils.FindVariable<string>(slbInfoBB.value, "comment");
            var imageUrl = BlackboardUtils.FindVariable<string>(slbInfoBB.value, "imageUrl");
            var startTimestamp = BlackboardUtils.FindVariable<long>(slbInfoBB.value, "startTimestamp");
            // var endTimestamp = BlackboardUtils.FindVariable<long>(slbInfoBB.value, "endTimestamp");

            if (string.IsNullOrEmpty(contextID.value))
                contextID.value = BiEventUtils.GenerateContextID();

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["slb_id"] = slbID.value;
            customData["context_id"] = contextID.value;

            var actionBB = BlackboardUtils.FindVariable<Blackboard>(slbInfoBB.value, "action");
            if(actionBB != null)
                BiEventUtils.AppendSLBActionData(customData, actionBB.value);

            customData["priority"] = priority.value;
            customData["comment"] = comment.value;
            customData["image_url"] = imageUrl.value;

            var constraintsBB = BlackboardUtils.FindVariable<Blackboard>(slbInfoBB.value, "constraints");
            if(constraintsBB != null)
            {
                var showEndTimer = BlackboardUtils.FindVariable<bool>(constraintsBB.value, "showEndTimer");
                var useUserTimer = BlackboardUtils.FindVariable<bool>(constraintsBB.value, "useUserTimer");
                var userTimeMin = BlackboardUtils.FindVariable<int>(constraintsBB.value, "userTimer");

                customData["show_end_timer"] = showEndTimer.value;
                customData["use_user_timer"] = useUserTimer.value;

                if(userTimeMin != null)
                {
                    customData["user_timer_min"] = userTimeMin.value;
                }

                if(userTimeMin != null && useUserTimer.value)
                {
                    long userTimerMS = userTimeMin.value * 60000L;
                    startTimestamp.value = endTimestamp.value - userTimerMS;
                }
                else
                {
                    endTimestamp.value = BlackboardUtils.FindVariable<long>(slbInfoBB.value, "endTimestamp").value;
                }
            }

            customData["start_timestamp"] = startTimestamp.value;
            customData["end_timestamp"] = endTimestamp.value;

            Analytics.CustomEvent("client_click_slb", customData);
        }

        EndAction();
    }
}

}
