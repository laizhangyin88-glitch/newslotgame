using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
[Description("Note that it copies shallowly")]
public class SetList : ActionTask<Blackboard>  
{
    public BBParameter<string> valueA;
    [BlackboardOnly]
    public BBParameter<IList> valueB;

    protected override string info
    {
        get { return string.Format("{0}[] = {1}[]", valueA, valueB); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.FindOrCreateVariable(agent, valueA.value, valueB.value.GetType());

        if (variableA == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
            EndAction(false);
        }
        else
        {
            variableA.value = valueB.value;
            EndAction();
        }
    }
}

}
