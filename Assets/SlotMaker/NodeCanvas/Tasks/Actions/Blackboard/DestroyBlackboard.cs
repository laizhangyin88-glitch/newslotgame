using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
public class DestroyBlackboard : ActionTask<Blackboard>
{
	public BBParameter<string> parent;
	public BBParameter<string> bb;

	protected override string info
	{
		get { return string.Format("Destroy {0}/{1}", parent, bb); }
	}

	protected override void OnExecute()
	{
        string variableName = null;
        var _bb = BlackboardUtils.FindBlackboard(agent, parent.value, ref variableName);
		if (_bb == null)
		{
			Debug.LogError("[Blackboard] Null blackboard founded in " + agent);
			EndAction(false);
		}
		else
		{
            BlackboardUtils.DestroyBlackboard(_bb, bb.value);
			EndAction();
		}
	}
}

}
