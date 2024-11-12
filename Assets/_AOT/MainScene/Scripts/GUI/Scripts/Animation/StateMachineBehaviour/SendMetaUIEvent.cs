using UnityEngine;
using System.Collections;

using NodeCanvas.Framework;
using ParadoxNotion;

namespace SlotMaker.AnimatorBehaviour
{
    public class SendMetaUIEvent : StateMachineBehaviour 
    {
        public bool atExit;
        public bool isLocalEvent = false;
        public string eventName;
        
        private const string ON_META_UI_EVENT = "OnMetaUIEvent";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!atExit)
            {
                if(isLocalEvent)
                    SendLocalEvent(animator);
                else
                    MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData(eventName));
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (atExit)
            {
                if(isLocalEvent)
                    SendLocalEvent(animator);
                else
                    MessageDispatcher.Dispatch(ON_META_UI_EVENT, new EventData(eventName));
            }
        }

        private void SendLocalEvent(Animator animator)
        {
            if(animator == null) return;
            
            MetaUIEventDispatcher owner = animator.gameObject.GetComponent<MetaUIEventDispatcher>();
            if(owner != null)
            {
                owner.Dispatch(new EventData(eventName));
            }
        }
    }
}
