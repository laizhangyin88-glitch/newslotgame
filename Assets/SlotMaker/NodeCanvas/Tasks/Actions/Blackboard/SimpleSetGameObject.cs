using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SimpleSetGameObject : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<string> valueB;

    protected override string info
    {
        get { return string.Format("{0} = {1}", valueA, valueB); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<GameObject>(agent, valueA.value);
        var variableB = BlackboardUtils.GetOrCreateVariable<GameObject>(agent, valueB.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
            EndAction(false);
        }
        else
        {
            variableA.value = variableB.value;
            EndAction();
        }
    }
}

}
