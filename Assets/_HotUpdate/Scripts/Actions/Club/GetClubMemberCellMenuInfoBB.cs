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

public class GetClubMemberCellMenuInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;

    public BBParameter<ClubAuthority> myAuthority;

    protected override string info
    {
        get { return "Get Club Member Cell Leader Menu Info BB"; }
    }

    protected override void OnExecute()
    {
        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

        ContextElement promoteAreaElement = ContextUtils.FindElement(agentElement, "Club Info Speech Bubble/Button Promote Area", ContextSearchingType.FullNameSearch); 
        ContextElement demoteAreaElement  = ContextUtils.FindElement(agentElement, "Club Info Speech Bubble/Button Demote Area", ContextSearchingType.FullNameSearch);

        var memberInfo = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);

        if(memberInfo != null)
        {
            var targetAuthority = memberInfo.value.GetValue<ClubAuthority>("authority");
            
            switch(myAuthority.value)
            {
                case ClubAuthority.LEADER:
                    if(targetAuthority == ClubAuthority.MEMBER)
                    {
                        promoteAreaElement.gameObject.SetActive(true);
                        demoteAreaElement.gameObject.SetActive(false);
                    }
                    else
                    {
                        promoteAreaElement.gameObject.SetActive(false);
                        demoteAreaElement.gameObject.SetActive(true);
                    }
                    break;
                default:
                    promoteAreaElement.gameObject.SetActive(false);
                    demoteAreaElement.gameObject.SetActive(false);
                    break;
            }
        }

        EndAction();
    }
}

}
