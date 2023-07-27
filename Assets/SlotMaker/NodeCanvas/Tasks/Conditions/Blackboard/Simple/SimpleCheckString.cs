using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class SimpleCheckString : ConditionTask<Blackboard> 
{
	public BBParameter<string> valueA;
	public BBParameter<string> valueB;

	protected override string info
	{
		get { return valueA + " == " + valueB; }
	}

	protected override bool OnCheck() 
	{
		var variableA = BlackboardUtils.FindVariable<string>(agent, valueA.value);
        var variableB = BlackboardUtils.FindVariable<string>(agent, valueB.value);
		return string.Equals(variableA.value, variableB.value);
	}
}

}
