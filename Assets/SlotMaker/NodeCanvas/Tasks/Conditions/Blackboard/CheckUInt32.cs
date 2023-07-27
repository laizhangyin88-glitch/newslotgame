using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckUInt32 : ConditionTask<Blackboard> 
{
	public BBParameter<string> valueA;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<uint> valueB;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{
		var variableA = BlackboardUtils.FindVariable<uint>(agent, valueA.value);
		return OperationUtils.Compare(variableA.value, valueB.value, checkType);
	}
}

}