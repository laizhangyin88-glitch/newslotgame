using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/IAM")]
public class GetIAMDeal : ActionTask<Blackboard>
{
    public BBParameter<int> saveAsID;
    public BBParameter<long> saveAsRemainingTimestamp;

    private const string IAM_KEY = "IAMTimer:{0}";
    private const string IAM_DEAL_KEY = "IAMDeal_ID";

    protected override string info
    {
        get { return "Get IAM Deal Info "; }
    }

    protected override void OnExecute()
    {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         saveAsID.value = 0;
//         saveAsRemainingTimestamp.value = 0L;
// #else
        var iamIdValue = PlayerPrefs.GetString(IAM_DEAL_KEY, "0");

        saveAsID.value = 0;
        saveAsRemainingTimestamp.value = 0L;

        if(iamIdValue != "0")
        {
            int iamID = System.Convert.ToInt32(iamIdValue);

            var iamBB = BlackboardQueryUtils.GetIAMBlackboard(iamID);

            if(iamBB != null && IAMRouter.Instance.IsValidIAMasDeal(iamBB))
            {
                long currentTimestamp = TimeUtils.GetTimeStamp();
                long endTimestamp = GetEndTimestamp(iamBB);

                if(currentTimestamp < endTimestamp)
                {
                    saveAsID.value = iamID;
                    saveAsRemainingTimestamp.value = endTimestamp;
                }
            }
            else
            {
                PlayerPrefs.DeleteKey(IAM_DEAL_KEY);
            }
        }
// #endif
        EndAction();
    }

    private long GetEndTimestamp(Blackboard iamInfoBB)
    {
        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

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
            if(endTimestamp == 0L)
            {
                endTimestamp = userEndTimestamp;
            }
            else
            {
                if(userEndTimestamp < endTimestamp)
                {
                    endTimestamp = userEndTimestamp;
                }
            }
        }

        if(endTimestamp == 0L || (endTimestamp > 0 && currentTimestamp < endTimestamp))
        {
            return endTimestamp;
        }
        else
        {
            return -1L;
        }
    }
}

}
