using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class SimpleCheckBoolean : ConditionTask<Blackboard> 
{	
	public BBParameter<string> valueA;
	public BBParameter<string> valueB;

	protected override string info
	{
		get { return valueA + " == " + valueB; }
	}

	protected override bool OnCheck()
	{
		var variableA = BlackboardUtils.FindVariable<bool>(agent, valueA.value);
        var variableB = BlackboardUtils.FindVariable<bool>(agent, valueB.value);
		return variableA.value == variableB.value;
	}
}

}
