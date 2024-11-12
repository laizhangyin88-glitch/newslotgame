using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace BagelCode.Task.Condition
{

[Category("★ BagelCode/Events")]
[EventReceiver("OnSpinButtonEvent")]
public class CheckSpinButtonEvent : ConditionTask<GraphOwner>
{
	protected override string info{ get {return "★ [SpinButton]"; } }
	protected override bool OnCheck(){ return false; }
	public void OnSpinButtonEvent(EventData receivedEvent){
		if (isActive){

			#if UNITY_EDITOR
			if (NodeCanvas.Editor.Prefs.logEvents){
				Debug.Log(string.Format("★ Event '{0}' Received from '{1}'", receivedEvent.name, agent.gameObject.name), agent);
			}
			#endif

			YieldReturn(true);
		}
	}
}

}
