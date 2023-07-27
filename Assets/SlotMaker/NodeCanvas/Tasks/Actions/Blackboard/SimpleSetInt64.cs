using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SimpleSetInt64 : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public OperationMethod Operation = OperationMethod.Set;
    public BBParameter<string> valueB;

    protected override string info
    {
        get { return valueA + OperationUtils.GetOperationString(Operation) + valueB; }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<long>(agent, valueA.value);
        var variableB = BlackboardUtils.GetOrCreateVariable<long>(agent, valueB.value);
        if (variableA == null || variableB == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
            EndAction(false);
        }
        else
        {
            variableA.value = OperationUtils.Operate(variableA.value, variableB.value, Operation);
            EndAction();
        }
    }
}

}
