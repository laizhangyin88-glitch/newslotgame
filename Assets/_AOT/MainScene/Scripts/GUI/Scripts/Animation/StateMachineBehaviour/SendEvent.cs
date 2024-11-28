using UnityEngine;
using System.Collections;

using NodeCanvas.Framework;

namespace SlotMaker.AnimatorBehaviour
{
    [DefaultExecutionOrder(100)]
    public class SendEvent : StateMachineBehaviour 
    {
        public bool atExit;
        public bool isLocalEvent = false;
        public string eventName;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!atExit)
            {
                if(isLocalEvent)
                    SendLocalEvent(animator);
                else
                    GraphOwner.SendGlobalEvent(eventName);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (atExit)
            {
                if(isLocalEvent)
                    SendLocalEvent(animator);
                else
                    GraphOwner.SendGlobalEvent(eventName);
            }
        }

        private void SendLocalEvent(Animator animator)
        {
            if(animator == null) return;

            GraphOwner owner = animator.gameObject.GetComponent<GraphOwner>();
            if(owner != null)
            {
                owner.SendEvent(eventName);
            }
        }
    }
}
