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

public class GetClubFeedMessageInfoBB : ActionTask<Blackboard>
{
    public BBParameter<int> cellIndex;
    public BBParameter<List<GameObject>> bgList; // 0, 1

    public BBParameter<string>  clubFeedInfoValue;
    public BBParameter<string> leftTimeAgoText;

    protected override string info
    {
        get { return "Get Club Feed Message Info BB"; }
    }

    protected override void OnExecute()
    {
        var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);

        if(clubFeedInfo != null)
        {
            var createdTimestamp = BlackboardUtils.GetOrCreateVariable<long>(clubFeedInfo.value, "createdTimestamp").value;
            leftTimeAgoText.value = ClubUtils.GetClubFeedLeftTimeText( TimeUtils.GetTimeStamp() - createdTimestamp );

            UpdateBG();
        }

        EndAction();
    }

    private void UpdateBG()
    {
        int cellStyle = cellIndex.value%2;

        for(int i=0; i<bgList.value.Count; ++i)
        {
            bgList.value[i].SetActive(i==cellStyle);
        }
    }
}

}
