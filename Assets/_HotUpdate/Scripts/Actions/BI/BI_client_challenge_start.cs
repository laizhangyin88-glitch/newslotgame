using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_challenge_start : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

        int timeZoneOffset = TimeUtils.GetTimeZoneOffset();
        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

        for(int i=0; i<challengeInfoList.value.Count; ++i)
        {
            var challengeType = BlackboardUtils.FindVariable<ChallengeType>(challengeInfoList.value[i], "challengeType");
            bool isDone = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done").value;
            bool isClaimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed").value;
            long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
            DateTime startTimeDate = TimeUtils.ParseTimestampToDateTime( startTimestamp);
            startTimeDate = startTimeDate.AddHours(-timeZoneOffset);
            startTimestamp = (long)((startTimeDate - TimeUtils.Jan1St1970).TotalMilliseconds);

            bool isPlayable = false;
            if(currentTimestamp >= startTimestamp && !isDone && !isClaimed)
                isPlayable = true;

            var challengeID = BlackboardUtils.FindVariable<string>(challengeInfoList.value[i], "challengeId");

            if(challengeID != null)
            {
                Dictionary<string, object> customData = new Dictionary<string, object>();
                customData["challenge_type"] = challengeType.value.ToString().ToLower();
                customData["challenge_id"] = challengeID.value;
                customData["status"] = isPlayable ? "ongoing" : "pending";

                Analytics.CustomEvent("client_challenge_start", customData);
            }
        }

        EndAction();
    }
}

}
