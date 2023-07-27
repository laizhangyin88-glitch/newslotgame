using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

public static partial class BlackboardQueryUtils
{
    public static void ApplyDailyBoostInfo(DailyBoost dailyBoostInfo)
    {
        if(dailyBoostInfo == null) return;

        var mainBB = MainBlackboard.Get();
        if(mainBB.GetVariable("dailyBoost") == null)
        {
            BlackboardUtils.GetOrCreateBlackboard( MainBlackboard.Get(), "dailyBoost");
        }

        var dailyBoostBB = BlackboardUtils.FindVariable<Blackboard>( MainBlackboard.Get(), "/dailyBoost");
        if(dailyBoostBB != null && dailyBoostBB.value != null)
        {
            ClientAPI2Blackboard.Serialize(dailyBoostBB.value, dailyBoostInfo);
        }
    }

    public static void ApplyDailyBoostInfo(Blackboard dailyBoostInfo)
    {
        if(dailyBoostInfo == null) return;

        var mainBB = MainBlackboard.Get();
        if(mainBB.GetVariable("dailyBoost") == null)
        {
            BlackboardUtils.GetOrCreateBlackboard( MainBlackboard.Get(), "dailyBoost");
        }

        var dailyBoostBB = BlackboardUtils.FindVariable<Blackboard>( MainBlackboard.Get(), "/dailyBoost");
        if(dailyBoostBB != null)
        {
            BlackboardUtils.SetOrCreateValue<long>(dailyBoostBB.value, "credit", dailyBoostInfo.GetValue<long>("credit"));
            BlackboardUtils.SetOrCreateValue<long>(dailyBoostBB.value, "start", dailyBoostInfo.GetValue<long>("start"));
            BlackboardUtils.SetOrCreateValue<long>(dailyBoostBB.value, "end", dailyBoostInfo.GetValue<long>("end"));
            BlackboardUtils.SetOrCreateValue<long>(dailyBoostBB.value, "lastCollect", dailyBoostInfo.GetValue<long>("lastCollect"));

            BlackboardUtils.SetOrCreateValue<int>(dailyBoostBB.value, "totalCount", dailyBoostInfo.GetValue<int>("totalCount"));
            BlackboardUtils.SetOrCreateValue<int>(dailyBoostBB.value, "collectCount", dailyBoostInfo.GetValue<int>("collectCount"));

            BlackboardUtils.SetOrCreateValue<long>(dailyBoostBB.value, "rp", dailyBoostInfo.GetValue<long>("rp"));
        }
    }

    public static void UpdateDailyBoostState()
    {
        var mainBB = MainBlackboard.Get();
        if(mainBB.GetVariable("dailyBoost") == null) return;

        var dailyBoostBB = BlackboardUtils.FindVariable<Blackboard>( MainBlackboard.Get(), "/dailyBoost");

        if(dailyBoostBB != null && dailyBoostBB.value != null)
        {
            BlackboardUtils.SetOrCreateValue( dailyBoostBB.value, "leftCollectDayCount", 0);
            BlackboardUtils.SetOrCreateValue( dailyBoostBB.value, "isCollectable", false);
            BlackboardUtils.SetOrCreateValue( dailyBoostBB.value, "isTerminated", true);

            var endTime = BlackboardUtils.FindVariable<long>( dailyBoostBB.value, "end");

            DateTime endDate = TimeUtils.ParseTimestampToDateTime(endTime.value);
            DateTime currentDate    = TimeUtils.GetCurrentDateTime();
            currentDate             = currentDate.ToLocalTime();

            if(currentDate < endDate)
            {
                var lastCollectTime = BlackboardUtils.FindVariable<long>( dailyBoostBB.value, "lastCollect");

                DateTime lastCollectDate = TimeUtils.ParseTimestampToDateTime(lastCollectTime.value);
                lastCollectDate = lastCollectDate.AddDays(1);
                lastCollectDate = new DateTime (lastCollectDate.Year, lastCollectDate.Month, lastCollectDate.Day, 0, 0, 0, DateTimeKind.Local);
                
                if(lastCollectDate < endDate)
                {
                    TimeSpan leftDate = endDate - currentDate; 

                    int leftCollectCount = (int)leftDate.TotalDays;
                    
                    if(lastCollectDate <= currentDate)
                    {
                        dailyBoostBB.value.SetValue("isCollectable", true);

                        ++leftCollectCount;
                    }

                    dailyBoostBB.value.SetValue("isTerminated", false);
                    dailyBoostBB.value.SetValue("leftCollectDayCount", leftCollectCount);
                }
            }

            DateTime nextDate       = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, DateTimeKind.Local);
            nextDate                = nextDate.AddDays(1);
            nextDate                = nextDate.ToUniversalTime();
            DateTime Jan1St1970     = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long nextCollectTime    = (long)((nextDate - Jan1St1970).TotalMilliseconds);
            BlackboardUtils.SetOrCreateValue( dailyBoostBB.value, "nextCollectTime", nextCollectTime);
        }
    }
}

}

