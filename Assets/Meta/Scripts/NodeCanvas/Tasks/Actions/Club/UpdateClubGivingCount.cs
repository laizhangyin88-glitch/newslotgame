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

public class UpdateClubGivingCount : ActionTask<Blackboard>
{
    public BBParameter<string> memberListValue;

    protected override string info
    {
        get { return "Update Club Giving Count"; }
    }

    protected override void OnExecute()
    {
        var memberListBB = BlackboardUtils.FindVariable<List<Blackboard>>(agent, memberListValue.value);

        long givingCount = 0;

        if(memberListBB != null)
        {
            for(int i=0; i < memberListBB.value.Count; ++i)
            {
                givingCount += memberListBB.value[i].GetValue<long>("clubGiving");
            }
        }

        string givingCountText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_GIVING_COUNT", givingCount);

        var givingTextElement = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), "Member/Gifts Sent/Text Gifts Sent", ContextSearchingType.FullNameSearch);
        MetaContextElementUtils.SetText(givingTextElement, givingCountText);

        EndAction();
    }
}

}
