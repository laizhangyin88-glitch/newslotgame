using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckUInt64 : ConditionTask<Blackboard> 
{
	public BBParameter<string> valueA;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<ulong> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{
		var variableA = BlackboardUtils.FindVariable<ulong>(agent, valueA.value);
		return OperationUtils.Compare(variableA.value, valueB.value, checkType);
	}
}

}