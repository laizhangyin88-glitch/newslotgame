using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.StateMachines
{

[Name("★ Auto Update Action State")]
[Color("ff1493")]
[Description("Execute a number of Action Tasks OnEnter. All actions will be stoped OnExit. This state is Finished when all Actions are finished as well")]
public class AutoUpdateActionState : ActionState
{
	protected override void OnEnter()
	{
		var owner = graphAgent.GetComponent<ManualGraphOwner>();
		if (owner != null)
			owner.BeginAutoUpdate();

		base.OnEnter();
	}

	protected override void OnExit()
	{
		base.OnExit();

		var owner = graphAgent.GetComponent<ManualGraphOwner>();
		if (owner != null)
			owner.EndAutoUpdate(true);
	}
}

}
