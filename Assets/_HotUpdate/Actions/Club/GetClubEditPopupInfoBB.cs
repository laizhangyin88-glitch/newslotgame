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

public class GetClubEditPopupInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string> clubInfoValue;

    public BBParameter<List<int>> watcherDayList;
    public BBParameter<List<string>> clubJoinTypeList;
    public BBParameter<List<int>> requiredLevelList;

    public BBParameter<bool> enableClubRestriction;

    public BBParameter<int> watcherIndex;
    public BBParameter<int> joinTypeIndex;
    public BBParameter<int> requiredLevelIndex;

    public BBParameter<int> clubMessageMaxLegnth;

    protected override string info
    {
        get { return "Get Club Edit Info BB"; }
    }

    protected override void OnExecute()
    {
        var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfoValue.value);

        watcherDayList.value = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "values/club/WATCHER_DAYS").value;
        requiredLevelList.value = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "values/club/MIN_PLAYER_LEVEL").value;
        clubJoinTypeList.value = new List<string>();
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_01"));
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_02"));

        enableClubRestriction.value = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_CLUB_RESTRICTION").value;

        watcherIndex.value = watcherDayList.value.IndexOf(clubInfoBB.value.GetValue<int>("watcher"));
        joinTypeIndex.value = clubInfoBB.value.GetValue<ClubJoinType>("joinType") == ClubJoinType.PUBLIC ? 0 : 1;
        requiredLevelIndex.value = requiredLevelList.value.IndexOf(clubInfoBB.value.GetValue<int>("minPlayerLevel"));

        clubMessageMaxLegnth.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/CLUB_MOTD_MAX_LENGTH").value;

        EndAction();
    }
}

}
