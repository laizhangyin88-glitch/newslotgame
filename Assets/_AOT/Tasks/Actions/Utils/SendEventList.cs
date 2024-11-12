using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions{


    [Category("★ SlotMaker/Utility")]
    [Description("Send a graph event with T value. If global is true, all graph owners in scene will receive this event. Use along with the 'Check Event' Condition")]
    public class SendEventList<T> : ActionTask<GraphOwner>
    {
        [RequiredField]
        public BBParameter<string> eventName;
        public BBParameter<List<T>> eventValue;
        public BBParameter<float> delay;
        public bool sendGlobal;

        protected override string info{
            get {return string.Format("{0} Event [{1}] ({2}){3}", (sendGlobal? "Global " : ""), eventName, eventValue, (delay.value > 0? " after " + delay + " sec." : "")  );}
        }

        protected override void OnUpdate(){
            if (elapsedTime >= delay.value){
                var e = new EventData<List<T>>(eventName.value, eventValue.value);
                if (sendGlobal){
                    Graph.SendGlobalEvent(e, agent);
                } else {
                    agent.SendEvent(e, null);
                }
                EndAction();
            }
        }
    }
}
