using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SlotMaker.Keno.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public enum DrawState
    {
        Drawn = 0,
        Drawing = 1,
    }

    public class BallInstance : MonoBehaviour
    {
        [SerializeField]
        public BallInfo ballInfo;

        public int ballIndex { get { return ballInfo.index; } set { ballInfo.index = value; } }
        public int number { get { return ballInfo.number; } set { ballInfo.number = value; } }
        public long multiplier { get { return ballInfo.multiplier; } set { ballInfo.multiplier = value; } }
        public SymbolAttribute mask { get { return ballInfo.mask; } set { ballInfo.mask = value; } }

        public Vector3 target;
        public float gravityScale;
        public Vector3 initialSpeed;
        public float drawnThreshold;
        public Rigidbody rigidbody;

        private PooledObject _pooledObject;
        protected PooledObject pooledObject { get { return _pooledObject ?? (_pooledObject = GetComponent<PooledObject>()); } }

        public DrawState drawState
        {
            get { return ballInfo.drawState; }
            set
            {
                if (ballInfo.drawState != value)
                {
                    ballInfo.drawState = value;
                    switch (ballInfo.drawState)
                    {
                        case DrawState.Drawn:
                            OnDrawn();
                            break;
                        case DrawState.Drawing:
                            OnDrawing();
                            break;
                    }
                }
            }
        }
        public CatchState catchState
        {
            get { return ballInfo.catchState; }
            set
            {
                if (ballInfo.catchState != value)
                {
                    ballInfo.catchState = value;
                    switch (ballInfo.catchState)
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
        public List<CustomBallEvent> customEvents;
        private Dictionary<string, CustomBallEvent> customEventDict = new Dictionary<string, CustomBallEvent>();

        public void SendEvent(string eventName)
        {
            CustomBallEvent customEvent = null;
            if (customEventDict.TryGetValue(eventName, out customEvent))
                customEvent.OnEvent(this, gameObject);
        }

        public BallEvent onDrawn;
        public BallEvent onDrawing;
        public BallEvent onCatch;
        public BallEvent onRelease;

        public void OnDrawn()
        {
            onDrawn.Invoke(this, gameObject);
            if (mediator) mediator.OnDrawn(this);
        }

        public void OnDrawing()
        {
            onDrawing.Invoke(this, gameObject);
            if (mediator) mediator.OnDrawing(this);
        }

        public void OnCatch()
        {
            onCatch.Invoke(this, gameObject);
            if (mediator) mediator.OnCatchBall(this);
        }

        public void OnRelease()
        {
            onRelease.Invoke(this, gameObject);
            if (mediator) mediator.OnReleaseBall(this);
        }

        private void Awake()
        {
            foreach (var customEvent in customEvents)
                customEventDict[customEvent.eventName] = customEvent;
        }

        public void Initialize(BallInfo ballInfo)
        {
            this.ballInfo = ballInfo;

            SendEvent("Apply");
        }

        public void Reset()
        {
            pooledObject.ReturnToPool();
            catchState = CatchState.Idle;
        }

        public void Draw()
        {
            drawState = DrawState.Drawing;
        }

        public void Catch()
        {
            catchState = CatchState.Catch;
        }

        public void Skip()
        {
            SendEvent("Skip");

            drawState = DrawState.Drawn;
        }

        private void FixedUpdate()
        {
            if (drawState == DrawState.Drawn)
                return;

            if (rigidbody.isKinematic)
            {
                rigidbody.isKinematic = false;
                rigidbody.AddForce(initialSpeed, ForceMode.VelocityChange);
            }
            else
                rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);

            if (Vector3.Distance(transform.localPosition, target) <= drawnThreshold)
            {
                rigidbody.isKinematic = true;
                Skip();
            }
        }
    }
}
