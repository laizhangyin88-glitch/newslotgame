using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class SlotBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public SlotMediator slot;

        [Serializable]
        public class SlotInstanceEvent : UnityEvent<SlotInstance> {}

        public SlotInstanceEvent onSpinning;
        public SlotInstanceEvent onStopping;
        public SlotInstanceEvent onStopped;

        protected void OnEnable()
        {
            slot.onSpinning += OnSpinning;
            slot.onStopping += OnStopping;
            slot.onStopped  += OnStopped;
        }

        protected void OnDisable()
        {
            slot.onSpinning -= OnSpinning;
            slot.onStopping -= OnStopping;
            slot.onStopped  -= OnStopped;
        }

        protected void OnSpinning(SlotInstance slotInstance)
        {
            onSpinning.Invoke(slotInstance);
        }

        protected void OnStopping(SlotInstance slotInstance)
        {
            onStopping.Invoke(slotInstance);
        }

        protected void OnStopped(SlotInstance slotInstance)
        {
            onStopped.Invoke(slotInstance);
        }
    }
}