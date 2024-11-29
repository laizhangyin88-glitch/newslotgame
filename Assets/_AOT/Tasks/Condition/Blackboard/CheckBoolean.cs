using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckBoolean : ConditionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public BBParameter<bool> valueB = true;

	protected override string info
	{
		get { return valueA + " == " + valueB; }
	}

	protected override bool OnCheck()
	{
		var variableA = BlackboardUtils.FindVariable<bool>(agent, valueA.value);
		if (variableA == null)
		{
			return false;
		}
		else
		{
			return variableA.value == valueB.value;
		}
	}
}

}
