using ParadoxNotion.Services;
using UnityEngine;

namespace BagelCode.AnimatorBehaviour
{
    public class SendEventMetaStateMachine : StateMachineBehaviour
    {
        public bool atExit;
        public bool isGlobalEvent = false;
        public string eventType;
        public string eventName;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!atExit)
            {
                SendEvent(animator);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (atExit)
            {
                SendEvent(animator);
            }
        }

        private void SendEvent(Animator animator)
        {
            if (animator == null) return;

            if (isGlobalEvent)
            {
                EventSender.SendGlobalEvent(eventType, eventName);
            }
            else
            {
                EventSender.SendEvent(animator.gameObject, eventType, eventName);
            }
        }
    }
}