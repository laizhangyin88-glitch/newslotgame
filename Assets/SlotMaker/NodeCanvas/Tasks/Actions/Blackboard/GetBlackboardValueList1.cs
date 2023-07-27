using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
public class GetBlackboardValueList1<T> : ActionTask<Blackboard>  
{
	public BBParameter<string> valueA;
    public BBParameter<int> index;
	
    [BlackboardOnly]
	public BBParameter<List<T>> saveAs;

	protected override string info
	{
		get { return string.Format("{0} = {1}[{2}]", saveAs, valueA, index); }
	}

	protected override void OnExecute()
	{
		var variable = BlackboardUtils.FindVariable<List<Blackboard>>(agent, valueA.value);
		if (variable == null)
		{
			Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
			EndAction(false);
		}
		else
		{
			saveAs.value = variable.value[index.value].GetValue<List<T>>("value");
			EndAction();
		}
	}
}

}
