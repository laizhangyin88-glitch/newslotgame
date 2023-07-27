using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetInt64Modulo : ActionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public BBParameter<long> valueB;

	protected override string info
	{
		get { return valueA + "%=" + valueB; }
	}

	protected override void OnExecute()
	{
		var variableA = BlackboardUtils.GetOrCreateVariable<long>(agent, valueA.value);
		if (variableA == null)
		{
			Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
			EndAction(false);
		}
		else
		{
			variableA.value = variableA.value % valueB.value;
			EndAction();
		}
	}
}

}
