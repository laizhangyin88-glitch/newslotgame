using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace BagelCode.Tasks.Actions {

[Category("★ BagelCode/Utils")]
public class FilterBadWord : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<string> valueB;

    protected override string info
    {
        get { return string.Format("{0} = filter({1})", valueA, valueB); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<string>(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
            EndAction(false);
        }
        else
        {
            if(string.IsNullOrEmpty(valueB.value))
            {
                variableA.value = valueB.value;
            }
            else
            {
                variableA.value = StringTable.BadWordFilter(valueB.value);
            }

            EndAction();
        }
    }
}

}
