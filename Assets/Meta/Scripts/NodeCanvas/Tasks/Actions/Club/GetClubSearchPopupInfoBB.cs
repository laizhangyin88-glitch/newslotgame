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

public class GetClubSearchPopupInfoBB : ActionTask<Blackboard>
{
    public BBParameter<List<int>> clubLevelList;
    public BBParameter<List<int>> clubRestrictionLevelList;
    public BBParameter<List<string>> clubJoinTypeList;

    public BBParameter<int> clubMaxLevel;

    public BBParameter<bool> enableClubRestriction;

    protected override string info
    {
        get { return "Get Club Search Popup Info BB"; }
    }

    protected override void OnExecute()
    {
        clubMaxLevel.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEVEL/MAX_LEVEL").value;

        clubLevelList.value = new List<int>();
        for(int i=0; i<clubMaxLevel.value; ++i)
        {
            clubLevelList.value.Add(i+1);
        }
        
        clubRestrictionLevelList.value = BlackboardUtils.FindVariable<List<int>>(MainBlackboard.Get(), "values/club/MIN_PLAYER_LEVEL").value;

        clubJoinTypeList.value = new List<string>();
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_00"));
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_01"));
        clubJoinTypeList.value.Add(StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_JOIN_TEXT_02"));

        enableClubRestriction.value = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_CLUB_RESTRICTION").value;

        EndAction();
    }
}

}
