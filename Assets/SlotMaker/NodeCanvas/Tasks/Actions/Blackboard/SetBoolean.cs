using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetBoolean : ActionTask<Blackboard> 
{
    public BBParameter<string> valueA;
    public BoolSetModes setTo = BoolSetModes.True;

    protected override string info
    {
        get
        {
            if (setTo == BoolSetModes.Toggle)
                return "Toggle " + valueA;
            else 
                return "Set " + valueA + " to " + setTo;
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
            if (setTo == BoolSetModes.Toggle)
                variableA.value = !variableA.value;
            else 
                variableA.value = ((int)setTo == 1);
                
            EndAction();
        }
    }
}

}
