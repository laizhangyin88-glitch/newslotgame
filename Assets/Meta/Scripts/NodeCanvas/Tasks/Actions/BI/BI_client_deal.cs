using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_deal : ActionTask<Blackboard>
{
	public BBParameter<string> iamIdValue;
    public BBParameter<string> contextID;

    private const string IAM_KEY = "IAMTimer:{0}";
    private const string IAM_DEAL_KEY = "IAMDeal_ID";

    protected override void OnExecute()
    {
        var iamID = BlackboardUtils.FindVariable<int>(agent, iamIdValue.value);
        var iamInfoBB = BlackboardQueryUtils.GetIAMBlackboard(iamID.value);
        if(iamInfoBB == null)
        {
            EndAction();
            return;
        }

        if(string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["context_id"] = contextID.value;
        customData["iam_id"] = iamID.value;
        AppendTimestampData(customData, iamInfoBB);
        
        Analytics.CustomEvent("client_click_deal", customData);

        EndAction();
    }

    private void AppendTimestampData(Dictionary<string, object> eventData, Blackboard iamInfoBB)
    {
        long startTimestamp = BlackboardUtils.FindVariable<long>(iamInfoBB, "startTimestamp").value;
        long endTimestamp = BlackboardUtils.FindVariable<long>(iamInfoBB, "endTimestamp").value;
        bool useUserTimer = BlackboardUtils.FindVariable<bool>(iamInfoBB, "useUserTimer").value;
        int userTimerMin = BlackboardUtils.FindVariable<int>(iamInfoBB, "userTimerMin").value;

        string id = BlackboardUtils.FindVariable<int>(iamInfoBB, "id").value.ToString();
        string iamKey = string.Format(IAM_KEY, id);
        string userStartTimeText = PlayerPrefs.GetString(iamKey, "0");
        long userStartTimestamp = System.Convert.ToInt64(userStartTimeText);
        long userEndTimestamp = userStartTimestamp + (System.Convert.ToInt64(userTimerMin) * 60000L);

        if(useUserTimer && userTimerMin > 0)
        {
            startTimestamp = userStartTimestamp;

            if(endTimestamp == 0L || userEndTimestamp < endTimestamp)
            {
                endTimestamp = userEndTimestamp;
            }
        }

        eventData["iam_start_timestamp"] = startTimestamp;
        eventData["iam_end_timestamp"] = endTimestamp;
        eventData["use_user_timer"] = useUserTimer;
        eventData["user_timer_min"] = userTimerMin;
    }
}

}
