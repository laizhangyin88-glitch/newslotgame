using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/WallOfEpic")]
public class GetWallOfEpicInfoBB : ActionTask<Blackboard>
{
    public BBParameter<Blackboard>  infoBB;

    public BBParameter<long> winMultiplier;
    public BBParameter<int> winGrade;
    public BBParameter<string> leftTimeAgo;

    private const string DATE_DAYS = "days";
    private const string DATE_DAY = "1 day";
    private const string DATE_HOURS = "hours";
    private const string DATE_HOUR = "1 hour";
    private const string DATE_MINS = "mins";
    private const string DATE_MIN = "1 min";
    private const int DAY_SECONDS = 86400;
    private const int HOUR_SECONDS = 3600;
    private const int MINUTE_SECONDS = 60;

    protected override string info
    {
        get { return "Get Wall Of Epic Info BB"; }
    }

    protected override void OnExecute()
    {
        winMultiplier.value = BlackboardUtils.GetOrCreateVariable<long>(infoBB.value, "winRatio").value;

        leftTimeAgo.value = GetLeftTimeText( BlackboardUtils.GetOrCreateVariable<long>(infoBB.value, "reportedTimestamp").value );

        winGrade.value = 0;
        if(winMultiplier.value >= 500)
        {
            winGrade.value = 2;
        }
        else if(winMultiplier.value >= 200)
        {
            winGrade.value = 1;
        }

        EndAction();
    }

    private string GetLeftTimeText(long timestamp)
    {
        string result;

        long leftTime = (TimeUtils.GetTimeStamp() - timestamp) / 1000;

        bool error = false;

        long day = leftTime / DAY_SECONDS;
        if (day >= 1)
        {
            if(day < 10)
            {
                result = StringTableUtils.GetString(StringTable.StringTableType.Global, "WALL_OF_EPIC_LEFT_TEXT_3", out error, day);
            }
            else
            {
                result = StringTableUtils.GetString(StringTable.StringTableType.Global, "WALL_OF_EPIC_LEFT_TEXT_4", out error, day);
            }
        }
        else
        {
            long hour = leftTime / HOUR_SECONDS;
            if (hour >= 1)
            {
                result = StringTableUtils.GetString(StringTable.StringTableType.Global, "WALL_OF_EPIC_LEFT_TEXT_2", out error, hour);
            }
            else
            {
                long minute = leftTime / MINUTE_SECONDS;
                if (minute >= 1)
                {
                    result = StringTableUtils.GetString(StringTable.StringTableType.Global, "WALL_OF_EPIC_LEFT_TEXT_1", out error, minute);
                }
                else
                {
                    result = StringTableUtils.GetString(StringTable.StringTableType.Global, "WALL_OF_EPIC_LEFT_TEXT_0", out error);
                }
            }
        }

        return result;
    }
}

}
