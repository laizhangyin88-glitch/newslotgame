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

public class GetClubRequestButtonInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string> clubInfoResponseValue;

    public BBParameter<bool> enableRequestButton;
    public BBParameter<int> requestCount;
    public BBParameter<int> biAuthorityNumber;

    protected override string info
    {
        get { return "Get Club Request Button Info BB"; }
    }

    protected override void OnExecute()
    {
        enableRequestButton.value = false;
        requestCount.value = 0;
        biAuthorityNumber.value = (int)ClubAuthority.MEMBER;

        var clubInfoResponse = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfoResponseValue.value);

        if(clubInfoResponse != null)
        {
            var myClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/clubId");
            if(myClubID != null && myClubID.value > 0)
            {
                var clubInfo = BlackboardUtils.FindVariable<Blackboard>(clubInfoResponse.value, "myClubInfo");

                if(clubInfo != null)
                {
                    var clubID = BlackboardUtils.FindVariable<long>(clubInfo.value, "clubId");
                    if(clubID != null && clubID.value == myClubID.value)
                    {
                        var myAuthority = clubInfo.value.GetValue<ClubAuthority>("authority");
                        biAuthorityNumber.value = (int)myAuthority;
                        
                        switch(myAuthority)
                        {
                            case ClubAuthority.LEADER:
                            case ClubAuthority.COLEADER:
                                enableRequestButton.value = true;
                                requestCount.value = clubInfoResponse.value.GetValue<int>("numClubRequest");
                                break;
                        }
                    }
                }
            }
        }

        EndAction();
    }
}

}
