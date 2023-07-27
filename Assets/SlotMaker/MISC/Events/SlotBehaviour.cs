using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ParadoxNotion;

namespace SlotMaker
{
    public class SlotBehaviour : MonoBehaviour
    {
        public UnityEvent onSpinSlotMachine;
        public UnityEvent onStopSlotMachine;
        public UnityEvent onStoppedSlotMachine;

        private MessageDelegates slotDelegates;

        protected virtual void Awake()
        {
            slotDelegates = new MessageDelegates
            (
                new Dictionary<string, MessageDispatcher.EventDelegate>
                {
                    { "SpinSlotMachine",    SpinSlotMachine    },
                    { "StopSlotMachine",    StopSlotMachine    },
                    { "StoppedSlotMachine", StoppedSlotMachine }
                }
            );
        }

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register("OnSlotEvent", slotDelegates.Delegate);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister("OnSlotEvent", slotDelegates.Delegate);
        }

        private void SpinSlotMachine(EventData eventData)
        {
            onSpinSlotMachine.Invoke();
        }

        private void StopSlotMachine(EventData eventData)
        {
            onStopSlotMachine.Invoke();
        }

        private void StoppedSlotMachine(EventData eventData)
        {
            onStoppedSlotMachine.Invoke();
        }
    }
}
