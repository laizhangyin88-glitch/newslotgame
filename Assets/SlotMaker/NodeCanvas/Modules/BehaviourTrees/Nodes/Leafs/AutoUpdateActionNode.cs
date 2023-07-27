using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.BehaviourTrees
{

[Name("★ Auto Update Action")]
[Color("ff1493")]
[Description("Executes an action and returns Success or Failure. Returns Running until the action is finished")]
[Icon("Action")]
public class AutoUpdateActionNode : ActionNode {

	protected override Status OnExecute(Component agent, IBlackboard blackboard){

		if (action == null){
			return Status.Failure;
		}

		if (status == Status.Resting)
		{
			var owner = graphAgent.GetComponent<ManualGraphOwner>();
			if (owner != null)
				owner.BeginAutoUpdate();
		}

		if (status == Status.Resting || status == Status.Running){
			Status actionStatus = action.ExecuteAction(agent, blackboard);
			if (actionStatus != Status.Running)
			{
				var owner = graphAgent.GetComponent<ManualGraphOwner>();
				if (owner != null)
					owner.EndAutoUpdate();
			}
			return actionStatus;
		}

		return status;
	}
}

}
