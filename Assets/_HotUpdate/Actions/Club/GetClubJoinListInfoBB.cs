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

public class GetClubJoinListInfoBB : ActionTask<Blackboard>
{
    public BBParameter<int> invitedClubCount;

    protected override string info
    {
        get { return "Get Club Join List Info BB"; }
    }

    protected override void OnExecute()
    {
        invitedClubCount.value = 0;

        var invitedClubList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, "invitedClubList");

        if(invitedClubList != null)
        {
            invitedClubCount.value = invitedClubList.value.Count;
        }

        EndAction();
    }
}

}
