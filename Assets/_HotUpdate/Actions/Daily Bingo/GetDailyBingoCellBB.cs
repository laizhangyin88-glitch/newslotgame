using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bingo")]

public class GetDailyBingoCellBB : ActionTask<Blackboard>
{

    public BBParameter<Blackboard> cellInfoBB;
    public BBParameter<int> cellIndex;
    public BBParameter<int> currentDay;
    public BBParameter<int> dayOffSet;
    public BBParameter<int> tier;
    public BBParameter<string> todayTextColorCode;
    public BBParameter<string> textColorCode;
    public BBParameter<string> textHightlightColorCode;
    public BBParameter<string> badgeTextColorCode;
    public BBParameter<bool> isClaimed;

    public BBParameter<string> saveText;
    public BBParameter<string> saveBadgeText;
    public BBParameter<bool> saveIsBadge;
    public BBParameter<bool> saveIsHightLight;
    public BBParameter<bool> saveIsToday;

    private const int MAX_DAY = 25;

    protected override string info
    {
        get { return string.Format("Get Daily Bingo Cell BB({0})", cellInfoBB); }
    }

    protected override void OnExecute()
    {
        bool isError = false;

        bool isFree = BlackboardUtils.FindVariable<bool>(cellInfoBB.value, "free").value;
        saveIsHightLight.value = BlackboardUtils.FindVariable<bool>(cellInfoBB.value, "highlight").value;

        bool isToday =  IsToday();

        if(isFree)
        {
            saveText.value = "-";
            saveBadgeText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_FREE_TEXT", out isError, badgeTextColorCode.value);
            saveIsBadge.value = true;
        }
        else
        {
            if(cellIndex.value + 1 == MAX_DAY)
            {
                if(saveIsHightLight.value)
                {
                    saveText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_FINAL_TEXT", out isError,  isToday ? todayTextColorCode.value : textHightlightColorCode.value );
                }
                else
                {
                    saveText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_FINAL_TEXT", out isError,  isToday ? todayTextColorCode.value : textColorCode.value );
                }
                
                saveIsBadge.value = IsPrevDay();
                // saveBadgeText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_FINAL_TEXT", out isError, badgeTextColorCode.value );
                saveBadgeText.value = GetDayText(badgeTextColorCode.value);
            }
            else
            {
                if(saveIsHightLight.value)
                {
                    saveText.value = GetRewardText( isToday ? todayTextColorCode.value : textHightlightColorCode.value );
                }
                else
                {
                    saveText.value = GetRewardText( isToday ? todayTextColorCode.value : textColorCode.value);
                }

                saveBadgeText.value = GetDayText(badgeTextColorCode.value);
                saveIsBadge.value = IsPrevDay();
            }
        }

        saveIsToday.value = isToday;

        EndAction();
    }

    private bool IsToday()
    {
        if(isClaimed.value)
            return false;

        return cellIndex.value == currentDay.value;
    }

    private bool IsPrevDay()
    {
        if(isClaimed.value)
            return cellIndex.value - 1 < currentDay.value;
            
        return cellIndex.value < currentDay.value;
    }

    private string GetDayText(string colorCode)
    {
        bool isError = false;
        int dayCount = cellIndex.value - dayOffSet.value + 1;
        return StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_DAY_TEXT", out isError, colorCode, dayCount);
    }

    private string GetRewardText(string colorCode)
    {
        bool isError = false;
        long credit = (long)BlackboardUtils.FindVariable<int>(cellInfoBB.value, "credit").value;

        return StringTableUtils.GetString(StringTable.StringTableType.Global, "DAILY_BINGO_CELL_TEXT", out isError, colorCode, credit);
    }
}

}
