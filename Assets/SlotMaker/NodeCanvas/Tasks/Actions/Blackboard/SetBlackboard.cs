using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetBlackboard : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<Blackboard> valueB;

    protected override string info
    {
        get { return string.Format("{0} = {1}", valueA, valueB); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<Blackboard>(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + " in " + agent.name);
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
