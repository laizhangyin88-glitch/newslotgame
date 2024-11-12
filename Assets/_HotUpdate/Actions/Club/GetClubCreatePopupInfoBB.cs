using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubCreatePopupInfoInfo : ActionTask<Blackboard>
{
    public BBParameter<List<int>> watcherDayList;
    public BBParameter<List<string>> clubJoinTypeList;
    public BBParameter<List<int>> requiredLevelList;

    public BBParameter<bool> enableClubRestriction;

    public BBParameter<int> defaultWatcherIndex;

    public BBParameter<int> clubNameMaxLength;
    public BBParameter<int> clubMessageMaxLegnth;

    protected override string info
    {
        get { return "Get Club Create Popup Info BB"; }
    }

    protected override void OnExecute()
    {
        watcherDayList.value = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "values/club/WATCHER_DAYS").value;
        requiredLevelList.value = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "values/club/MIN_PLAYER_LEVEL").value;
        clubJoinTypeList.value = new List<string>();
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_01"));
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_02"));

        defaultWatcherIndex.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/WATCHER_DEFAULT_INDEX").value;

        enableClubRestriction.value = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_CLUB_RESTRICTION").value;

        clubNameMaxLength.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/CLUB_NAME_MAX_LENGTH").value;
        clubMessageMaxLegnth.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/CLUB_MOTD_MAX_LENGTH").value;

        EndAction();
    }
}

}
