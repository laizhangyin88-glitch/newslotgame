using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using SlotMaker.Keno.Events;

namespace SlotMaker.Keno
{
    public class KenoBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public KenoMediator mediator;

        public KenoEvent onReady;
        public KenoEvent onPlay;
        public KenoEvent onStop;
        public KenoEvent onWait;

        protected void OnEnable()
        {
            mediator.onReady += OnReady;
            mediator.onPlay += OnPlay;
            mediator.onStop += OnStop;
            mediator.onWait += OnWait;
        }

        protected void OnDisable()
        {
            mediator.onReady -= OnReady;
            mediator.onPlay -= OnPlay;
            mediator.onStop -= OnStop;
            mediator.onWait -= OnWait;
        }

        protected void OnReady(KenoInstance kenoInstance)
        {
            onReady.Invoke(kenoInstance, gameObject);
        }

        protected void OnPlay(KenoInstance kenoInstance)
        {
            onPlay.Invoke(kenoInstance, gameObject);
        }

        protected void OnStop(KenoInstance kenoInstance)
        {
            onStop.Invoke(kenoInstance, gameObject);
        }

        protected void OnWait(KenoInstance kenoInstance)
        {
            onWait.Invoke(kenoInstance, gameObject);
        }
    }
}