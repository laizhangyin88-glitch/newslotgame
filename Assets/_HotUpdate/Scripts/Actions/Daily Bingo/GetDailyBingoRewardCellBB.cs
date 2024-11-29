using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bingo")]

public class GetDailyBingoRewardCellBB : ActionTask<Blackboard>
{
    public BBParameter<int> cellDay;
    public BBParameter<int> currentDay;
    public BBParameter<string> textColorCode;
    public BBParameter<List<int>> freeDayList;
    public BBParameter<bool> isClaimed;

    public BBParameter<string> saveText;
    public BBParameter<bool> saveIsBadge;
    public BBParameter<bool> saveIsToday;

    protected override string info
    {
        get { return string.Format("Get Daily Bingo Reward Cell BB"); }
    }

    protected override void OnExecute()
    {
        // bool isError = false;

        saveText.value = GetDayText( textColorCode.value );
        saveIsBadge.value = IsPrevDay();
        saveIsToday.value = IsToday();

        EndAction();
    }

    private bool IsToday()
    {
        if(isClaimed.value)
            return cellDay.value - 1 == currentDay.value;

        return cellDay.value == currentDay.value;
    }

    private bool IsPrevDay()
    {
        if(isClaimed.value)
            return cellDay.value - 1 < currentDay.value;
        return cellDay.value < currentDay.value;
    }

    private string GetDayText(string colorCode)
    {
        bool isError = false;
        int dayCount = cellDay.value + 1 - GetFreeDayCount();
        return StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_REWARD_DAY_TEXT", out isError, colorCode, dayCount);
    }

    private int GetFreeDayCount()
    {
        int freeCount = 0;
        for(int i=0; i<freeDayList.value.Count; ++i)
        {
            if(freeDayList.value[i] <cellDay.value)
            {
                ++freeCount;
            }
            else
            {
                break;
            }
        }
        return freeCount;
    }
}

}
