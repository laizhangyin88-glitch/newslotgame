using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Animator")]
public class CheckAnimatorActive : ConditionTask<Animator>
{
    protected override string info
    {
        get
        {
            var agentName = agent != null ? ("<b>" + agent.name + "</b>") : agentInfo;
            return string.Format("{0}.animator is active", agentName != "<b>owner</b>" ? agentName : "<b>NULL</b>");
        }
    }

    protected override bool OnCheck()
    {
        return agent.isActiveAndEnabled;
    }
}

}
