using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

	[Category("★ BagelCode/Events")]
	public class SendWinEvent : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> eventName;
		public BBParameter<float> delay;
		public bool sendGlobal;
        public BBParameter<int> slotIndex = 0;

		private const string ON_WIN_EVENT = "OnWinEvent";

		protected override string info{
			get{ return "★ " + (sendGlobal? "Global " : "") + "Send Event [" + eventName + "]("+ slotIndex +") " + (delay.value > 0? " after " + delay + " sec." : "" );}
		}

		protected override void OnUpdate(){
			if (elapsedTime >= delay.value){
				var e = new EventData(eventName.value, slotIndex.value);
				if (sendGlobal){
					MessageDispatcher.Dispatch(ON_WIN_EVENT, e);
				} else {
					agent.GetComponent<WinEventDispatcher>().Dispatch(e);
				}
				EndAction();
			}
		}
	}


	[Category("★ BagelCode/Events")]
	public class SendWinEvent<T> : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> eventName;
		public BBParameter<T> eventValue;
		public BBParameter<float> delay;
		public bool sendGlobal;
        public BBParameter<int> slotIndex = 0;

		private const string ON_WIN_EVENT = "OnWinEvent";

		protected override string info{
			get {return string.Format("★ {0}({1}) Event [{2}] ({3}){4}", (sendGlobal? "Global " : ""), eventName, slotIndex, eventValue, (delay.value > 0? " after " + delay + " sec." : "")  );}
		}

		protected override void OnUpdate(){
			if (elapsedTime >= delay.value){
				var e = new EventData<T>(eventName.value, slotIndex.value, eventValue.value);
				if (sendGlobal){
					MessageDispatcher.Dispatch(ON_WIN_EVENT, e);
				} else {
					agent.GetComponent<WinEventDispatcher>().Dispatch(e);
				}
				EndAction();
			}
		}
	}


    [Category("★ BagelCode/Events")]
    public class SendWinEventList<T> : ActionTask<GraphOwner>
    {
        [RequiredField]
        public BBParameter<string> eventName;
        public BBParameter<List<T>> eventValue;
        public BBParameter<float> delay;
        public bool sendGlobal;
        public BBParameter<int> slotIndex = 0;

        private const string ON_WIN_EVENT = "OnWinEvent";

        protected override string info{
            get {return string.Format("★ {0}({1}) Event [{2}] ({3}){4}", (sendGlobal? "Global " : ""), eventName, slotIndex, eventValue, (delay.value > 0? " after " + delay + " sec." : "")  );}
        }

        protected override void OnUpdate()
        {
            if (elapsedTime >= delay.value){
                var e = new EventData<List<T>>(eventName.value, slotIndex.value,  eventValue.value);
                if (sendGlobal){
                    MessageDispatcher.Dispatch(ON_WIN_EVENT, e);
                } else {
                    agent.GetComponent<WinEventDispatcher>().Dispatch(e);
                }
                EndAction();
            }
        }
    }

}
