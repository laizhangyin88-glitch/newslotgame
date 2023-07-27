using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckFloat : ConditionTask<Blackboard> 
{
	public BBParameter<string> valueA;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<float> valueB;

	[SliderField(0,0.1f)]
	public float differenceThreshold = 0.05f;

	protected override string info
	{
		get { return valueA + OperationUtils.GetCompareString(checkType) + valueB; }
	}

	protected override bool OnCheck() 
	{
		var variableA = BlackboardUtils.FindVariable<float>(agent, valueA.value);
		return OperationUtils.Compare(variableA.value, valueB.value, checkType, differenceThreshold);
	}
}

}