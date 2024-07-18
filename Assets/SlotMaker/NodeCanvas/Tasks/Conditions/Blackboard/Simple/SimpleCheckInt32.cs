using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class SimpleCheckInt32 : ConditionTask<Blackboard> 
{
	public BBParameter<string> valueA;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<string> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{ 
		var variableA = BlackboardUtils.FindVariable<int>(agent, valueA.value);
        var variableB = BlackboardUtils.FindVariable<int>(agent, valueB.value);
		return OperationUtils.Compare(variableA.value, variableB.value, checkType);
	}
}

}
