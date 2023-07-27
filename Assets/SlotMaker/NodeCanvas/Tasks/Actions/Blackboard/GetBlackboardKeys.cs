using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class GetBlackboardKeys : ActionTask<Blackboard>  
{
	public BBParameter<string> valueA;
	[BlackboardOnly]
	public BBParameter<List<string>> saveAs;

	protected override string info
	{
		get { return string.Format("{0} = {1}.Keys", saveAs, valueA); }
	}

	protected override void OnExecute()
	{
	    var bb = BlackboardUtils.FindValue(agent, valueA.value) as Blackboard;
		if (bb == null)
		{
			Debug.LogError("[Blackboard] Null blackboard founded in " + valueA.value);
			EndAction(false);
		}
		else
		{
			List<string> keyList = new List<string>();
            var tempList = bb.variables.Values.ToList();
			for (int i = 0; i < tempList.Count; i++) {
                keyList.Add(tempList[i].ToString());
            }
			saveAs.value = keyList;
			EndAction();
		}
	}
}

}
