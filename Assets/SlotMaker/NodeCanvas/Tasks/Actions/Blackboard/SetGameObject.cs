using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetGameObject : ActionTask<Blackboard>
{
    public BBParameter<string> key;
    
    [BlackboardOnly]
    public BBParameter<GameObject> value;

    protected override string info
    {
        get { return string.Format("{0} = {1}", key, value); }
    }

    protected override void OnExecute()
    {
        var variableA = BlackboardUtils.GetOrCreateVariable<GameObject>(agent, key.value);
        if (variableA == null)
        {
            Debug.LogError("[Blackboard] Null variableA founded in " + key.value + "in " + agent.name);
            EndAction(false);
        }
        else
        {
            variableA.value = value.value;
            EndAction();
        }
    }
}

}
