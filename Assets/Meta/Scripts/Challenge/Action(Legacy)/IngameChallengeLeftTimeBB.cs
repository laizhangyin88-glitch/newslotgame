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

public class IngameChallengeLeftTimeBB : ActionTask<Blackboard>
{
    public BBParameter<bool> useWait;
    public BBParameter<float> waitTimeSec;

    protected override string info
    {
        get { return "Get Ingame Challenge Left Time BB"; }
    }

    protected override void OnExecute()
    {
        useWait.value = false;
        waitTimeSec.value = float.MaxValue;

        var challengeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "challengeInfoList");

        int timeZoneOffset = TimeUtils.GetTimeZoneOffset();
        DateTime currentDataTime = BagelCode.TimeUtils.GetCurrentDateTime();

        for(int i=0; i<challengeInfoList.value.Count; ++i)
        {
            bool isDone = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "done").value;
            bool isClaimed = BlackboardUtils.FindVariable<bool>(challengeInfoList.value[i], "claimed").value;

            if(isDone && !isClaimed)
            {
                long startTimestamp = BlackboardUtils.FindVariable<long>(challengeInfoList.value[i], "startTimestamp").value;
                DateTime startTimeDate = TimeUtils.ParseTimestampToDateTime( startTimestamp);
                startTimeDate = startTimeDate.AddHours(-timeZoneOffset);
                
                float totalSec = (float)(startTimeDate - currentDataTime).TotalSeconds;
                if(totalSec < waitTimeSec.value)
                {
                    waitTimeSec.value = totalSec;
                }

                useWait.value = true;
            }
        }

        EndAction();
    }
}

}
