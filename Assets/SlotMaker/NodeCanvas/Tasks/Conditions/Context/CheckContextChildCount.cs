using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Context")]
public class CheckContextChildCount : ConditionTask<ContextElement> 
{
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<int> valueB;

	protected override string info
	{
		get 
		{ 
			if (agent == null)
				return agentInfo + OperationUtils.GetCompareString(checkType) + valueB;
			else 
				return string.Format("({0}){1}{2}{3}", agentInfo, agent.ChildCount, OperationUtils.GetCompareString(checkType), valueB);
		}
	}

	protected override bool OnCheck() 
	{
		return OperationUtils.Compare(agent.ChildCount, valueB.value, checkType);
	}
}

}