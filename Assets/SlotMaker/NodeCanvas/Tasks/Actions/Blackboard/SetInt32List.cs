using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class SetInt32List : ActionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public OperationMethod Operation = OperationMethod.Set;
	public BBParameter<int> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetOperationString(Operation) + valueB; }
	}

	protected override void OnExecute()
	{
		var variableA = BlackboardUtils.GetOrCreateVariable<List<int>>(agent, valueA.value);
		if (variableA == null)
		{
			Debug.LogError("[Blackboard] Null variableA founded in " + valueA.value + "in " + agent.name);
			EndAction(false);
		}
		else
		{
			for (int i = 0; i < variableA.value.Count; ++i)
			{
				variableA.value[i] = OperationUtils.Operate(variableA.value[i], valueB.value, Operation);
			}
			EndAction();
		}
	}
}

}
