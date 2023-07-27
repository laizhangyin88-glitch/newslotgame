using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.Keno.Events;
using Sirenix.OdinInspector;

namespace SlotMaker.Keno
{
    public class BallBehaviour : MonoBehaviour
    {
        [InlineEditor]
        public KenoMediator mediator;

        public BallEvent onDrawn;
        public BallEvent onDrawing;
        public BallEvent onCatch;
        public BallEvent onRelease;

        protected void OnEnable()
        {
            mediator.onDrawn += OnDrawn;
            mediator.onDrawing += OnDrawing;
            mediator.onCatchBall += OnCatch;
            mediator.onReleaseBall += OnRelease;
        }

        protected void OnDisable()
        {
            mediator.onDrawn -= OnDrawn;
            mediator.onDrawing -= OnDrawing;
            mediator.onCatchBall -= OnCatch;
            mediator.onReleaseBall -= OnRelease;
        }

        protected void OnDrawn(BallInstance ball)
        {
            onDrawn.Invoke(ball, gameObject);
        }

        protected void OnDrawing(BallInstance ball)
        {
            onDrawing.Invoke(ball, gameObject);
        }

        protected void OnCatch(BallInstance ball)
        {
            onCatch.Invoke(ball, gameObject);
        }

        protected void OnRelease(BallInstance ball)
        {
            onRelease.Invoke(ball, gameObject);
        }
    }
}
