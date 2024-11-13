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

public class GetClubInfoBBPop : ActionTask<Blackboard>
{
    public BBParameter<string>  clubInfoValue;
    public BBParameter<string>  memberListValue;

    public BBParameter<string> informationBalloonText;

    public BBParameter<int> maxMemberCount;

    protected override string info
    {
        get { return "Get Club Info BB Pop"; }
    }

    protected override void OnExecute()
    {
        var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfoValue.value);
        var memberList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, memberListValue.value);

        if (clubInfoBB != null)
        {
            int memberCount = memberList == null ? 0 : memberList.value.Count;
            int onlineMemberCount = 0;
            int coCaptainMemberCount = 0;

            for (int i = 0; i < memberCount; ++i)
            {
                bool isOnline = memberList.value[i].GetValue<bool>("isOnline");
                if (isOnline)
                {
                    onlineMemberCount++;
                }

                var memberAuthority = memberList.value[i].GetValue<ClubAuthority>("authority");
                bool isCoCaptain = false;
                if (memberAuthority == ClubAuthority.COLEADER)
                {
                    isCoCaptain = true;
                }

                if (isCoCaptain)
                {
                    coCaptainMemberCount++;
                }
            }

            var level = clubInfoBB.value.GetValue<int>("level");
            var minPlayerLevel = clubInfoBB.value.GetValue<int>("minPlayerLevel");

            maxMemberCount.value = ClubUtils.GetClubMaxMemberCount(level);
            var totalCoCaptain = ClubUtils.GetClubMaxCoCaptainCount(level);

            int watcher = clubInfoBB.value.GetValue<int>("watcher");

            informationBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_INFORMATION_BALLOON", memberCount, maxMemberCount.value, onlineMemberCount, watcher, minPlayerLevel, coCaptainMemberCount, totalCoCaptain);
            
        }

        var agentElement = agent.gameObject.GetComponent<ContextElement>();

        var informationBalloonTextElement = ContextUtils.FindElement(agentElement, "Member/Club Information Area/Club Information Speech Bubble/Text", ContextSearchingType.FullNameSearch);
        MetaContextElementUtils.SetText(informationBalloonTextElement, informationBalloonText.value);

        EndAction();
    }
}

}
