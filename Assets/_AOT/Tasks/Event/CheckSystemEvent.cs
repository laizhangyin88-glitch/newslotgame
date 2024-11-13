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
[EventReceiver("OnSystemEvent")]
public class CheckSystemEvent : ConditionTask<GraphOwner>
{
	[RequiredField]
	public BBParameter<string> eventName;

	protected override string info{ get {return "★ [" + eventName.ToString() + "]"; } }
	protected override bool OnCheck(){ return false; }
	public void OnSystemEvent(EventData receivedEvent){
		if (isActive && receivedEvent.name.Equals(eventName.value, StringComparison.Ordinal)){

			#if UNITY_EDITOR
			if (NodeCanvas.Editor.Prefs.logEvents){
				Debug.Log(string.Format("★ Event '{0}' Received from '{1}'", receivedEvent.name, agent.gameObject.name), agent);
			}
			#endif

			YieldReturn(true);
		}
	}
}

[Category("★ BagelCode/Events")]
[EventReceiver("OnSystemEvent")]
public class CheckSystemEvent<T> : ConditionTask<GraphOwner>
{
	[RequiredField]
	public BBParameter<string> eventName;
	[BlackboardOnly]
	public BBParameter<T> saveEventValue;

	protected override string info{ get {return string.Format("★ Event [{0}]\n{1} = EventValue", eventName, saveEventValue);} }
	protected override bool OnCheck(){ return false; }
	public void OnSystemEvent(EventData receivedEvent){
		if (isActive && receivedEvent.name.Equals(eventName.value, StringComparison.Ordinal)){
			if (receivedEvent.value is T){
				saveEventValue.value = (T)receivedEvent.value;
			}

			#if UNITY_EDITOR
			if (NodeCanvas.Editor.Prefs.logEvents){
				Debug.Log(string.Format("★ Event '{0}' Received from '{1}'", receivedEvent.name, agent.gameObject.name), agent);
			}
			#endif

			YieldReturn(true);
		}
	}
}

[Category("★ BagelCode/Events")]
[EventReceiver("OnSystemEvent")]
public class CheckSystemEventValue<T> : ConditionTask<GraphOwner>
{
	[RequiredField]
	public BBParameter<string> eventName;
	public BBParameter<T> value;

	protected override string info{ get {return string.Format("★ Event [{0}].value == {1}", eventName, value);} }
	protected override bool OnCheck(){ return false; }
	public void OnSystemEvent(EventData receivedEvent){
		if (receivedEvent is EventData<T> && isActive && receivedEvent.name.Equals(eventName.value, StringComparison.Ordinal)){
			var receivedValue = ((EventData<T>)receivedEvent).value;
			if (receivedValue != null && receivedValue.Equals(value.value)){

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

}
