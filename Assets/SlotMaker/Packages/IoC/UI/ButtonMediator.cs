using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using SlotMaker.IoC.Strategy.Tween;

namespace SlotMaker.IoC.UI
{
    public class ButtonMediator : Button
    {
        public Transform scaleTarget;
        public TweenStrategy tween;
        public Vector3 baseScale = new Vector3(1f, 1f, 1f);
        public Vector3 pressedScale = new Vector3(0.95f, 0.95f, 1f);
        public TimeControl timeControl = TimeControl.DeltaTime;

        protected Vector3 velocity;
        protected Coroutine coroutine;

        public UnityEvent onReset;
        public UnityEvent onNormal;
        public UnityEvent onHighlighted;
        public UnityEvent onPressed;
        public UnityBoolEvent onEnabled;
        public UnityBoolEvent onDisabled;

        protected float DeltaTime 
        { 
            get 
            { 
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return 0f;
#endif
                switch (timeControl)
                {
                case TimeControl.DeltaTime:
                    return Time.deltaTime;
                case TimeControl.UnscaledDeltaTime:
                    return Time.unscaledDeltaTime;
                default:
                    return 0f;
                }
            }
        }

        protected override void InstantClearState()
        {
            base.InstantClearState();

            if (onReset != null) onReset.Invoke();

            ScaleTransition(baseScale, true);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            
            if (gameObject.activeInHierarchy)
            {
                switch (state)
                {
                case SelectionState.Normal:
                    if (onNormal != null) onNormal.Invoke();
                    break;
                case SelectionState.Highlighted:
                    if (onHighlighted != null) onHighlighted.Invoke();
                    break;
                case SelectionState.Pressed:
                    if (onPressed != null) onPressed.Invoke();
                    break;
                case SelectionState.Disabled:
                    if (onEnabled != null) onEnabled.Invoke(false);
                    if (onDisabled != null) onDisabled.Invoke(true);
                    break;
                default:
                    break;
                }

                if (state != SelectionState.Disabled)
                {
                    if (onEnabled != null) onEnabled.Invoke(true);
                    if (onDisabled != null) onDisabled.Invoke(false);
                }

                ScaleTransition(state != SelectionState.Pressed ? baseScale : pressedScale, instant);
            }    
        }

        protected void ScaleTransition(Vector3 targetScale, bool instant)
        {
            if (scaleTarget == null) return;
            
            if (coroutine != null) StopCoroutine(coroutine);
            
            if (instant) scaleTarget.localScale = targetScale;
            else coroutine = StartCoroutine(TweenScale(targetScale));
        }

        protected IEnumerator TweenScale(Vector3 targetScale)
        {
            while (!tween.IsCompleteTween(scaleTarget.localScale, targetScale, velocity, Vector3.zero))
            {
                scaleTarget.localScale = tween.UpdateTween(scaleTarget.localScale, targetScale, ref velocity, Vector3.zero, DeltaTime);
                yield return null;
            }
        }
    }
}