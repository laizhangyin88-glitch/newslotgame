using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SlotMaker.Keno.Events;

namespace SlotMaker.Keno
{
    public class SpotBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public KenoMediator mediator;

        public SpotEvent onPick;
        public SpotEvent onUnpick;
        public SpotEvent onPickFail;
        public SpotEvent onCatch;
        public SpotEvent onRelease;

        protected void OnEnable()
        {
            mediator.onPick += OnPick;
            mediator.onUnpick += OnUnpick;
            mediator.onPickFail += OnPickFail;
            mediator.onCatch += OnCatch;
            mediator.onRelease += OnRelease;
        }

        protected void OnDisable()
        {
            mediator.onPick -= OnPick;
            mediator.onUnpick -= OnUnpick;
            mediator.onPickFail -= OnPickFail;
            mediator.onCatch -= OnCatch;
            mediator.onRelease -= OnRelease;
        }

        protected void OnPick(SpotInstance spot)
        {
            onPick.Invoke(spot, gameObject);
        }

        protected void OnUnpick(SpotInstance spot)
        {
            onUnpick.Invoke(spot, gameObject);
        }

        protected void OnPickFail(SpotInstance spot)
        {
            onPickFail.Invoke(spot, gameObject);
        }

        protected void OnCatch(SpotInstance spot)
        {
            onCatch.Invoke(spot, gameObject);
        }

        protected void OnRelease(SpotInstance spot)
        {
            onRelease.Invoke(spot, gameObject);
        }
    }
}