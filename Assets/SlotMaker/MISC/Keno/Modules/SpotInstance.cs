using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SlotMaker.Keno.Events;
using SlotMaker.Keno.Commands;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    [Serializable]
    public enum MarkState
    {
        Idle = 0,
        Mark = 1,
    };

    [Serializable]
    public enum CatchState
    {
        Idle = 0,
        Catch = 1,
    };

    public class SpotInstance : MonoBehaviour
    {
        [SerializeField]
        public SpotInfo spotInfo;

        public int spotIndex { get { return spotInfo.index; } set { spotInfo.index = value; } }
        public int number { get { return spotInfo.number; } set { spotInfo.number = value; } }
        public long multiplier { get { return spotInfo.multiplier; } set { spotInfo.multiplier = value; } }
        public SymbolAttribute mask { get { return spotInfo.mask; } set { spotInfo.mask = (SymbolAttribute)value; } }

        public MarkState markState
        {
            get { return spotInfo.markState; }
            set
            {
                if (spotInfo.markState != value)
                {
                    spotInfo.markState = value;
                    switch (spotInfo.markState)
                    {
                        case MarkState.Idle:
                            OnUnpick();
                            break;
                        case MarkState.Mark:
                            OnPick();
                            break;
                    }
                }
            }
        }
        public CatchState catchState
        {
            get { return spotInfo.catchState; }
            set
            {
                if (spotInfo.catchState != value)
                {
                    spotInfo.catchState = value;
                    switch (spotInfo.catchState)
                    {
                        case CatchState.Idle:
                            OnRelease();
                            break;
                        case CatchState.Catch:
                            OnCatch();
                            break;
                    }
                }
            }
        }

        [InlineEditor]
        public KenoMediator mediator;

        [SerializeField]
        public List<CustomSpotEvent> customEvents;
        private Dictionary<string, CustomSpotEvent> customEventDict = new Dictionary<string, CustomSpotEvent>();

        public void SendEvent(string eventName)
        {
            CustomSpotEvent customEvent = null;
            if (customEventDict.TryGetValue(eventName, out customEvent))
                customEvent.OnEvent(this, gameObject);
        }

        public SpotEvent onPick;
        public SpotEvent onUnpick;
        public SpotEvent onPickFail;
        public SpotEvent onCatch;
        public SpotEvent onRelease;

        public void OnPick()
        {
            onPick.Invoke(this, gameObject);
            if (mediator) mediator.OnPick(this);
        }

        public void OnUnpick()
        {
            onUnpick.Invoke(this, gameObject);
            if (mediator) mediator.OnUnpick(this);
        }

        public void OnPickFail()
        {
            onPickFail.Invoke(this, gameObject);
            if (mediator) mediator.OnPickFail(this);
        }

        public void OnCatch()
        {
            onCatch.Invoke(this, gameObject);
            if (mediator) mediator.OnCatch(this);
        }

        public void OnRelease()
        {
            onRelease.Invoke(this, gameObject);
            if (mediator) mediator.OnRelease(this);
        }

        public bool IsHit
        {
            get { return catchState == CatchState.Catch && markState == MarkState.Mark; }
        }

        public bool CanPick
        {
            get { return mediator.pickCount != mediator.maxPickCount || markState != MarkState.Idle; }
        }

        private void Awake()
        {
            foreach (var customEvent in customEvents)
                customEventDict[customEvent.eventName] = customEvent;
        }

        public void Pick()
        {
            if (!CanPick)
            {
                OnPickFail();
                return;
            }

            if (markState == MarkState.Idle)
                markState = MarkState.Mark;
            else
                markState = MarkState.Idle;
        }

        public void Catch()
        {
            catchState = CatchState.Catch;
        }

        public void Release()
        {
            catchState = CatchState.Idle;
        }
    }
}
