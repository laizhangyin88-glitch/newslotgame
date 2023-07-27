using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bingo")]

public class GetDailyBingoBB : ActionTask<Blackboard>
{
    public BBParameter<string> todayColorCode;
    public BBParameter<string> textColorCode;
    public BBParameter<string> textHighlightColorCode;
    public BBParameter<string> overlayColorCode;
    public BBParameter<string> badgeTextColorCode;
    public BBParameter<string> lineColorCode;
    public BBParameter<string> descTextColorCode;
    public BBParameter<string> leftDayColorCode;
    public BBParameter<string> totalWinColorCode;
    public BBParameter<string> rewardTextColorCode;

    public BBParameter<bool> isClaimed;

    protected override string info
    {
        get { return string.Format("Get Daily Bingo BB"); }
    }

    protected override void OnExecute()
    {
        isClaimed.value = false;

        var dailyBingoInfoBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "dailyBingoInfo");

        if(dailyBingoInfoBB != null)
        {
            var colorSettingBB = BlackboardUtils.FindVariable<Blackboard>(dailyBingoInfoBB.value, "board/colorSetting");

            todayColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "todayTextColor").value;
            textColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "cellTextColor").value;
            textHighlightColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "cellHighlightTextColor").value;
            overlayColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "cellOverlayColor").value;
            badgeTextColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "stampTextColor").value;
            lineColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "highlightStrokeColor").value;
            descTextColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "descriptionTextColor").value;
            leftDayColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "daysLeftTextColor").value;
            totalWinColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "totalWinTextColor").value;
            rewardTextColorCode.value = BlackboardUtils.FindVariable<string>(colorSettingBB.value, "rewardTextColor").value;

            var rewardListBB = BlackboardUtils.FindVariable<List<Blackboard>>(dailyBingoInfoBB.value, "rewardList");

            if(rewardListBB != null)
            {
                isClaimed.value = rewardListBB.value.Count == 0;
            }
            
        }

        EndAction();
    }
}

}
