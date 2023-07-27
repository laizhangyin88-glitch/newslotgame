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

public class GetClubCellInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;

    public BBParameter<long> clubID;

    public BBParameter<int> maxMemberCount;
    public BBParameter<string> membersText;
    public BBParameter<string> friendsCountText;

    public BBParameter<bool> isPrivateClub;
    public BBParameter<bool> isRequiredLevel;
    public BBParameter<int>  requiredLevel;

    protected override string info
    {
        get { return "Get Club Cell Info BB"; }
    }

    protected override void OnExecute()
    {
        friendsCountText.value = "";
        isPrivateClub.value = false;
        isRequiredLevel.value = false;
        requiredLevel.value = 1;

        var cellInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);

        if(cellInfoBB != null && cellInfoBB.value != null)
        {
            clubID.value = cellInfoBB.value.GetValue<long>("id");

            var level = cellInfoBB.value.GetValue<int>("level");
            var friendCount = cellInfoBB.value.GetValue<int>("friends");
            var membersCount = cellInfoBB.value.GetValue<int>("members");
            var minPlayerLevel = cellInfoBB.value.GetValue<int>("minPlayerLevel");
            var joinType = cellInfoBB.value.GetValue<ClubJoinType>("joinType");

            maxMemberCount.value = ClubUtils.GetClubMaxMemberCount(level);

            if(membersCount < maxMemberCount.value)
                membersText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_CELL_MEMBER_COUNT", membersCount, maxMemberCount.value);
            else
                membersText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_CELL_MEMBER_MAX_COUNT", membersCount, maxMemberCount.value);

            if(friendCount > 0)
                friendsCountText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_CELL_FRIENDS_COUNT", friendCount);

            isPrivateClub.value = joinType == ClubJoinType.PRIVATE ? true : false;
            isRequiredLevel.value = minPlayerLevel > 1 ? true : false;
            requiredLevel.value = minPlayerLevel;
        }

        EndAction();
    }
}

}
