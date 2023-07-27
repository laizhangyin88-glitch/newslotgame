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

public class GetClubFeedNoticeInfoBB : ActionTask<Blackboard>
{
    public BBParameter<int> cellIndex;

    public BBParameter<string> clubInfoValue;
    public BBParameter<string> myAuthorityValue;

    public BBParameter<string> saveAsNoticeText;

    protected override string info
    {
        get { return "Get Club Feed Notice Info BB"; }
    }

    protected override void OnExecute()
    {
        var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfoValue.value);
        var myAuthority = BlackboardUtils.FindVariable<ClubAuthority>(agent, myAuthorityValue.value);
        var notice = clubInfoBB.value.GetValue<string>("notice");

        MetaContextElementUtils.SimpleSetActive(agent.gameObject.GetComponent<ContextElement>(), "Button Edit Area", myAuthority.value == ClubAuthority.LEADER);

        if (string.IsNullOrEmpty(notice))
        {
            saveAsNoticeText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_NOTICE_DEFAULT");
        }
        else
        {
            saveAsNoticeText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_NOTICE", notice);
        }

        EndAction();
    }
}

}
