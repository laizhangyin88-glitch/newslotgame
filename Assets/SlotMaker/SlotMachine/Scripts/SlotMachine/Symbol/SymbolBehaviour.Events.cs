using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;
using System;
using UnityEngine.Events;
using System.Reflection;

namespace SlotMaker
{
    public abstract partial class SymbolBehaviour : MonoBehaviour
    {
        public UnityEvent onEntry;
        public UnityEvent onSkip;
        public UnityEvent onStopEffect;
        public UnityEvent onPrepareStop;
        public UnityEvent onWin;

        protected Dictionary<string, UnityEvent> events = new Dictionary<string, UnityEvent>();

        private void Awake()
        {
            events = new Dictionary<string, UnityEvent>
            {
                { "Entry", onEntry },
                { "Skip", onSkip },
                { "Stop Effect", onStopEffect },
                { "PrepareStop", onPrepareStop },
                { "Win", onWin },
            };
        }

        protected SymbolEventHandler eventHandler;

        public virtual void StartBehaviour(SymbolEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;

            onEntry.AddListener(OnEntry);
            onSkip.AddListener(OnSkip);
            onStopEffect.AddListener(OnStopEffect);
            onPrepareStop.AddListener(OnPrepareStop);
            onWin.AddListener(OnWin);
        }

        public virtual void StopBehaviour()
        {
            this.eventHandler = null;

            onEntry.RemoveListener(OnEntry);
            onSkip.RemoveListener(OnSkip);
            onStopEffect.RemoveListener(OnStopEffect);
            onPrepareStop.RemoveListener(OnPrepareStop);
            onWin.RemoveListener(OnWin);
        }

        public void ExecuteStateMethod(string eventName)
        {
            UnityEvent eventDel;
            if (events.TryGetValue(eventName, out eventDel))
                eventDel.Invoke();
            else
                gameObject.SendMessage(eventName, SendMessageOptions.DontRequireReceiver);
        }

        public abstract void OnEntry();
        public abstract void OnSkip();
        public abstract void OnStopEffect();
        public abstract void OnPrepareStop();
        public abstract void OnWin();
    }

}