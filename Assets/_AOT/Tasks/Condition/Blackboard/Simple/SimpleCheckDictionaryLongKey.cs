using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Collections;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class SimpleCheckDictionaryLongKey : ConditionTask<Blackboard>
{
	public BBParameter<string> valueA;
	public BBParameter<string> valueB;

	protected override string info
	{
		get { return valueA + "[" + valueB + "] exists"; }
	}

	protected override bool OnCheck()
	{
		var variableA = BlackboardUtils.FindVariable(agent, valueA.value);
        var variableB = BlackboardUtils.FindVariable<long>(agent, valueB.value);
		return ((IDictionary)variableA.value).Contains(variableB.value);
	}
}

}
