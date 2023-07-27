using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetStringToLong : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;
    public BBParameter<string> valueB;

    protected override string info
    {
        get {return string.Format("Set {0} to {1}", valueB, valueA);}
    }

    protected override void OnExecute()
    {
    	var variableA = BlackboardUtils.GetOrCreateVariable<long>(agent, valueA.value);

    	if(variableA == null)
    	{
    		Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
    		EndAction(false);
    	}

        variableA.value = System.Convert.ToInt64(valueB.value);
        EndAction();
    }
}

}
