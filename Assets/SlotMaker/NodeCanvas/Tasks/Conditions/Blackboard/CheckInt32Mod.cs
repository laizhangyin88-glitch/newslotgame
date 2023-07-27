using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckInt32Mod : ConditionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public BBParameter<int> mod;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<int> valueB;

	protected override string info
	{
		get { return string.Format("({0} % {1}) {2} {3}", valueA, mod, OperationUtils.GetCompareString(checkType), valueB); }
	}

	protected override bool OnCheck()
	{
		var variableA = BlackboardUtils.FindVariable<int>(agent, valueA.value);
		return OperationUtils.Compare(variableA.value % mod.value, valueB.value, checkType);
	}
}

}
