using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetUInt64 : ActionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public OperationMethod Operation = OperationMethod.Set;
	public BBParameter<ulong> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetOperationString(Operation) + valueB; }
	}

	protected override void OnExecute()
	{
		var variableA = BlackboardUtils.GetOrCreateVariable<ulong>(agent, valueA.value);
		if (variableA == null)
		{
			Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
			EndAction(false);
		}
		else
		{
			variableA.value = OperationUtils.Operate(variableA.value, valueB.value, Operation);
			EndAction();
		}
	}
}

}
