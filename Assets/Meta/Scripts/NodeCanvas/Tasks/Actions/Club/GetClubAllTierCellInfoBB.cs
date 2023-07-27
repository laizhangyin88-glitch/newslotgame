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

public class GetClubAllTierCellInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  tierInfoValue;
    public BBParameter<string>  myClubTierInfoValue;

    public BBParameter<int>     cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1 : normal, 2 : myClub

    protected override string info
    {
        get { return "Get Club All Tier Cell Info BB"; }
    }

    protected override void OnExecute()
    {
        var tierInfo = BlackboardUtils.FindVariable<Blackboard>(agent, tierInfoValue.value);

        if(tierInfo != null)
        {
            var myClubTierInfo = BlackboardUtils.FindVariable<Blackboard>(agent, myClubTierInfoValue.value);

            int cellStyle = cellIndex.value%2;

            if(myClubTierInfo != null)
            {
                if(tierInfo.value.GetValue<int>("leagueTier") == myClubTierInfo.value.GetValue<int>("leagueTier"))
                    cellStyle = 2;
            }

            for(int i=0; i<bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i==cellStyle);
            }
        }

        EndAction();
    }
}

}
