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
[EventReceiver("OnSlotDetailEvent")]
public class CheckSlotDetailEvent : ConditionTask<GraphOwner>
{
	[RequiredField]
	public BBParameter<string> eventName;

	public BBParameter<int> slotIndex = 0;

	protected override string info{ get {return string.Format("★ [{0}]({1})", eventName, slotIndex);} }
	protected override bool OnCheck(){ return false; }
	public void OnSlotDetailEvent(EventData receivedEvent){
		if (isActive && receivedEvent.name.Equals(eventName.value, StringComparison.Ordinal) && receivedEvent.id == slotIndex.value){

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
[EventReceiver("OnSlotDetailEvent")]
public class CheckSlotDetailEvent<T> : ConditionTask<GraphOwner>
{
	[RequiredField]
	public BBParameter<string> eventName;
	[BlackboardOnly]
	public BBParameter<T> saveEventValue;
	public BBParameter<int> slotIndex = 0;

	protected override string info{ get {return string.Format("★ Event [{0}]({1})\n{2} = EventValue", eventName, slotIndex, saveEventValue);} }
	protected override bool OnCheck(){ return false; }
	public void OnSlotDetailEvent(EventData receivedEvent){
		if (isActive && receivedEvent.name.Equals(eventName.value, StringComparison.Ordinal) && receivedEvent.id == slotIndex.value){
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
[EventReceiver("OnSlotDetailEvent")]
public class CheckSlotDetailEventValue<T> : ConditionTask<GraphOwner>
{
	[RequiredField]
	public BBParameter<string> eventName;
	public BBParameter<T> value;

	public BBParameter<int> slotIndex = 0;

	protected override string info{ get {return string.Format("★ Event [{0}]({1}).value == {2}", eventName, slotIndex, value);} }
	protected override bool OnCheck(){ return false; }
	public void OnSlotDetailEvent(EventData receivedEvent){
		if (receivedEvent is EventData<T> && isActive && receivedEvent.name.Equals(eventName.value, StringComparison.Ordinal) && receivedEvent.id == slotIndex.value){
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
