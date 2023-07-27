using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
[Description("Split the string into string list")]
public class SplitString : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<string> delimiter;
    [BlackboardOnly]
    public BBParameter<List<string>> valueB;

    protected override string info
    {
        get { return string.Format("{0} = {1}.split({2})", valueB, valueA, delimiter); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<string>(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
            EndAction(false);
        }
        else
        {
            valueB.value = new List<string>(variableA.value.Split(delimiter.value.ToCharArray()));
            EndAction();
        }
    }
}

}
