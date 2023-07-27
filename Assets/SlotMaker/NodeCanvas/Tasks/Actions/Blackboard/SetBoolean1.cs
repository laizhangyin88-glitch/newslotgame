using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetBoolean1 : ActionTask<Blackboard> 
{
    public BBParameter<string> valueA;
    public BBParameter<bool> valueB;

    protected override string info
    {
        get
        {
            return "Set " + valueA + " to " + valueB;
        }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<bool>(agent, valueA.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
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
