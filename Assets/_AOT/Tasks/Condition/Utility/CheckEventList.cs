using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;

using ParadoxNotion.Services;

namespace NodeCanvas.Tasks.Conditions{

    [Category("✫ Utility")]
    [Description("Check if an event is received and return true for one frame. Optionaly save the received event's value")]
    [EventReceiver("OnCustomEvent")]
    public class CheckEventList<T> : ConditionTask<GraphOwner> {

        [RequiredField]
        public BBParameter<string> eventName;
        [BlackboardOnly]
        public BBParameter<List<T>> saveEventValue;

        protected override string info{ get {return string.Format("Event [{0}]\n{1} = EventValue", eventName, saveEventValue);} }
        protected override bool OnCheck(){ return false; }
        public void OnCustomEvent(EventData receivedEvent){
            if (isActive && receivedEvent.name.ToUpper() == eventName.value.ToUpper()){
                if (receivedEvent.value is List<T>){
                    saveEventValue.value = (List<T>)receivedEvent.value;
                }

                #if UNITY_EDITOR
                if (NodeCanvas.Editor.Prefs.logEvents){
                    Debug.Log(string.Format("Event '{0}' Received from '{1}'", receivedEvent.name, agent.gameObject.name), agent);
                }
                #endif

                YieldReturn(true);
            }
        }
    }
}
