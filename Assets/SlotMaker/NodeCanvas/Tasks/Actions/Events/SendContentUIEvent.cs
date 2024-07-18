using UnityEngine;
using System.Collections;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

	[Category("★ BagelCode/Events")]
	public class SendContentUIEvent : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> eventName;
		public BBParameter<float> delay;
		public bool sendGlobal;

		private const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";

		protected override string info{
			get{ return "★ " +(sendGlobal? "Global " : "") + "Send Event [" + eventName + "]" + (delay.value > 0? " after " + delay + " sec." : "" );}
		}

		protected override void OnUpdate(){
            if (elapsedTime >= delay.value){
				var e = new EventData(eventName.value);
				if (sendGlobal){
					MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, e);
				} else {
					agent.GetComponent<ContentUIEventDispatcher>().Dispatch(e);
				}
				EndAction();
			}
		}
	}


	[Category("★ BagelCode/Events")]
	public class SendContentUIEvent<T> : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> eventName;
		public BBParameter<T> eventValue;
		public BBParameter<float> delay;
		public bool sendGlobal;

		private const string ON_CONTENT_UI_EVENT = "OnContentUIEvent";

		protected override string info{
			get {return string.Format("★ {0} Event [{1}] ({2}){3}", (sendGlobal? "Global " : ""), eventName, eventValue, (delay.value > 0? " after " + delay + " sec." : "")  );}
		}

		protected override void OnUpdate(){
			if (elapsedTime >= delay.value){
				var e = new EventData<T>(eventName.value, eventValue.value);
				if (sendGlobal){
					MessageDispatcher.Dispatch(ON_CONTENT_UI_EVENT, e);
				} else {
					agent.GetComponent<ContentUIEventDispatcher>().Dispatch(e);
				}
				EndAction();
			}
		}
	}

}
