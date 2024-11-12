using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class MOG : ActionTask<Blackboard>
{
    public BBParameter<string> source;

    private long currentMog = 0L;
    private long intervalTime = 0L;
    private string text = "";

    protected override string info { get { return "MOG"; } }
    
    protected override void OnExecute()
    {
        var common = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "common");
        var mog = common.value.GetValue<long>("MOG");
        // var data = BlackboardUtils.FindVariable<List<Blackboard>>(agent, source.value);

        // if(data.value != null && 
        if(CheckMog(mog))
        {
            // var mogBB = (Blackboard)BlackboardUtils.CreateBlackboard("Poll");
            // BlackboardUtils.SetOrCreateValue<PollType>(mogBB, "__event__", PollType.NOTICE);
            // BlackboardUtils.SetOrCreateValue<long>(mogBB, "id", 0L);
            // BlackboardUtils.SetOrCreateValue<bool>(mogBB, "isHighlighted", true);
            // BlackboardUtils.SetOrCreateValue<string>(mogBB, "message", string.Format("Our app will undergo maintenance in {0}.<br>Sorry for the interruption. We will be back soon!", text));
            // BlackboardUtils.AddToBlackboardList(agent, "data", mogBB);
            MetaFeedUtils.SendMaintenanceFeed(text);
        }

        EndAction(true);
    }

    public bool CheckMog(long mog)
    {
        if(mog == 0L)
        {
            return false;
        }

        long currentTimestamp = TimeUtils.GetTimeStamp();

        if(currentMog == 0 || currentMog != mog)
        {
            currentMog = mog;
            intervalTime = currentMog - currentTimestamp;
            if(intervalTime > 300000L)
            {
                text = "5 minutes";
                intervalTime = 300000L;
            }
            else if(intervalTime > 180000L)
            {
                text = "3 minutes";
                intervalTime = 180000L;
            }
            else if(intervalTime > 60000L)
            {
                text = "1 minute";
                intervalTime = 60000L;
            }
            else if(intervalTime > 30000L)
            {
                text = "30 seconds";
                intervalTime = 30000L;
            }
            else if(intervalTime > 0L)
            {
                text = string.Format("{0} seconds", System.Convert.ToInt32(intervalTime / 1000L));
                intervalTime = 1000L;
            }
            else
            {
                intervalTime = 0L;
            }
        }

        if(intervalTime != 0 && intervalTime >= (currentMog - currentTimestamp))
        {
            if(intervalTime == 1000L)
            {
                intervalTime = 0L;
            }
            else
            {
                currentMog = 0L;
            }
            return true;
        }
           return false;
    }
}

}
