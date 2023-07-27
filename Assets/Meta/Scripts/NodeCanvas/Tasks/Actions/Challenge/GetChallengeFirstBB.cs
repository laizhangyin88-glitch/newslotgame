using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Challenge")]

public class GetChallengeFirstBB : ActionTask<Blackboard>
{
    public BBParameter<bool> saveAsFirst;

    private const string prevChallengeKey = "PREV_CHALLENGE_ID";

    protected override string info
    {
        get { return "Get Challenge First BB"; }
    }

    protected override void OnExecute()
    {
        string challengeID = GetChallengeID();

        if(!string.IsNullOrEmpty(challengeID))
        {
            if(PlayerPrefs.HasKey(prevChallengeKey) && PlayerPrefs.GetString(prevChallengeKey) == challengeID)
            {
                saveAsFirst.value = false;
            }
            else
            {
                saveAsFirst.value = true;
            }

            PlayerPrefs.SetString(prevChallengeKey, challengeID);
        }
        else
        {
            saveAsFirst.value = false;
        }

        EndAction();
    }

    private string GetChallengeID()
    {
        var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

        int timeZoneOffset = TimeUtils.GetTimeZoneOffset();
        long currentTimestamp = BagelCode.TimeUtils.GetTimeStamp();

        for(int i=0; i<challengeInfoList.value.Count; ++i)
        {
            var cType = BlackboardUtils.FindVariable<ChallengeType>(challengeInfoList.value[i], "challengeType");

            if(cType.value == ChallengeType.DAILY)
            {
                bool isDone = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done").value;
                bool isClaimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed").value;
                long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
                DateTime startTimeDate = TimeUtils.ParseTimestampToDateTime( startTimestamp);
                startTimeDate = startTimeDate.AddHours(-timeZoneOffset);
                startTimestamp = (long)((startTimeDate - TimeUtils.Jan1St1970).TotalMilliseconds);

                if(currentTimestamp >= startTimestamp && !isDone && !isClaimed)
                {
                    var challengeID = BlackboardUtils.FindVariable<string>(challengeInfoList.value[i], "challengeId");
                    if(challengeID != null) return challengeID.value;
                }

                break;
            }
        }

        return "";
    }
}

}
