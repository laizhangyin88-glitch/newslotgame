using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class ReelBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public SlotMediator slot;

        [Serializable]
        public class ReelInstanceEvent : UnityEvent<ReelInstance> {}

        public ReelInstanceEvent onSpinningReel;
        public ReelInstanceEvent onStoppingReel;
        public ReelInstanceEvent onPrepareStoppedReel;
        public ReelInstanceEvent onStoppedReel;

        protected void OnEnable()
        {
            slot.onSpinningReel       += OnSpinningReel;
            slot.onStoppingReel       += OnStoppingReel;
            slot.onPrepareStoppedReel += OnPrepareStoppedReel;
            slot.onStoppedReel        += OnStoppedReel;
        }

        protected void OnDisable()
        {
            slot.onSpinningReel       -= OnSpinningReel;
            slot.onStoppingReel       -= OnStoppingReel;
            slot.onPrepareStoppedReel -= OnPrepareStoppedReel;
            slot.onStoppedReel        -= OnStoppedReel;
        }

        protected void OnSpinningReel(ReelInstance reelInstance)
        {
            onSpinningReel.Invoke(reelInstance);
        }

        protected void OnStoppingReel(ReelInstance reelInstance)
        {
            onStoppingReel.Invoke(reelInstance);
        }

        protected void OnPrepareStoppedReel(ReelInstance reelInstance)
        {
            onPrepareStoppedReel.Invoke(reelInstance);
        }

        protected void OnStoppedReel(ReelInstance reelInstance)
        {
            onStoppedReel.Invoke(reelInstance);
        }
    }
}