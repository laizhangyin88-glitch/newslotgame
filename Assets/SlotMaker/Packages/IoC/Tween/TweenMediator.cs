using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.IoC.Strategy;
using SlotMaker.IoC.Strategy.Tween;
using Sirenix.OdinInspector;

namespace SlotMaker.IoC.Tween
{
    public enum TweenPlayMode
    {
        DoNothing,
        Step,
        Sequence,
        Loop
    }

    public enum TweenTargetMode
    {
        Static,
        Dynamic
    }

    public enum TweenResetMode
    {
        DoNothing,
        SetFrom,
        SetTo
    }

    public enum TweenInterruptMode
    {
        Reset,
        Blend
    }

    public abstract class TweenMediator : MonoBehaviour
    {
        public Component target;

        [InlineEditor]
        public PropertyStrategy propertyInjector;

        public EnableAction enableAction = EnableAction.EnableBehaviour; 

        public TweenPlayMode playMode = TweenPlayMode.DoNothing;
        public LoopType loopType = LoopType.None;
        public TweenInterruptMode interruptMode = TweenInterruptMode.Reset;
        public TimeControl timeControl = TimeControl.DeltaTime;
        public TweenResetMode resetMode = TweenResetMode.SetFrom;
        public int step = 0;
        public int nextStep = 1;

        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent onReset;
        [PropertyOrder(101)]
        [FoldoutGroup("Events")]
        public UnityEvent onStart;
        [PropertyOrder(102)]
        [FoldoutGroup("Events")]
        public UnityEvent onPaused;
        [PropertyOrder(103)]
        [FoldoutGroup("Events")]
        public UnityEvent onResumed;
        [PropertyOrder(104)]
        [FoldoutGroup("Events")]
        public UnityEvent onUpdate;
        [PropertyOrder(105)]
        [FoldoutGroup("Events")]
        public UnityEvent onCompleteStep;
        [PropertyOrder(106)]
        [FoldoutGroup("Events")]
        public UnityEvent onCompleteSequence;

        public bool IsPlaying { get { return (coroutine != null) && !paused; } }
        public bool IsPaused { get { return (coroutine != null) && paused; } }
        
        [ShowInInspector]
        [PropertyRange(0f, 1f)]
        public float PlayingOffset 
        { 
            get { return (duration != 0f) ? (playback / duration) : 0f; }
            set 
            { 
                if (!IsPlaying && target && propertyInjector)
                {
                    SetupTween();
                    StartTween();
                    playback = value * duration;
                    UpdateTween();
                }
            }
        }

        protected float Direction { get { return !rewind ? 1f : -1f; } }

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
                    return Time.deltaTime * Direction;
                case TimeControl.UnscaledDeltaTime:
                    return Time.unscaledDeltaTime * Direction;
                default:
                    return 0f;
                }
            }
        }

        public abstract int StepCount { get; }

        protected float playback = 0f;
        protected float duration = 1f;
        protected float delay;
        protected Coroutine coroutine;
        protected bool paused;
        protected bool rewind;
        protected bool startCalled;

        protected virtual void Start()
        {
            startCalled = true;
            if (enableAction == EnableAction.EnableBehaviour)
                Play();
        }

        protected virtual void OnEnable()
        {
            if (startCalled && enableAction == EnableAction.EnableBehaviour)
                Play();
        }

        protected virtual void OnDisable() { Reset(); }

        public abstract void Reset();

        [Button]
        [ButtonGroup("Controller")]
        [PropertyOrder(-1)]
        public void Play()
        {
            Stop();

            if (rewind)
            {
                rewind = false;
                NextStep();
                UpdateStep();
            }

            coroutine = StartCoroutine(Run());
        }

        [Button]
        [ButtonGroup("Controller")]
        [PropertyOrder(-1)]
        public void Rewind()
        {
            Stop();

            if (!rewind)
            {
                rewind = true;
                NextStep();
                UpdateStep();
            }

            coroutine = StartCoroutine(Run());
        }        

        [Button]
        [ButtonGroup("Controller")]
        public void Stop()
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }
        
        [Button]
        [ButtonGroup("Controller")]
        public void TogglePause()
        {
            if (paused)
                Resume();
            else
                Pause();
        }

        public void Pause()
        {
            if (!paused)
            {
                paused = true;
                if (onPaused != null) onPaused.Invoke();
            }
        }

        public void Resume()
        {
            if (paused)
            {
                paused = false;
                if (onResumed != null) onResumed.Invoke();
            }
        }

        public abstract bool StaticTween();
        public abstract void SetupTween();
        public abstract void StartTween();
        public abstract void UpdateTween();
        public abstract void CompleteTween();
        public abstract bool IsCompleteTween();

        protected virtual void UpdateStaticTween()
        {
            playback = Mathf.Clamp(playback + DeltaTime, 0f, duration);
        }

        protected virtual bool IsCompleteStaticTween()
        {
            return !rewind ? (playback >= duration) : (playback <= 0f);
        }

        protected abstract void UpdateDynamicTween();
        protected abstract bool IsCompleteDynamicTween();

        protected IEnumerator Run()
        {
            bool hasMoreSequence = true;
            do
            {
                SetupTween();
                if (delay > 0f) yield return new WaitForSeconds(delay);

                StartTween();
                while (!IsCompleteTween())
                {
                    yield return null;

                    if (!paused) 
                        UpdateTween();
                }
                CompleteTween();

                switch (playMode)
                {
                case TweenPlayMode.DoNothing:
                    hasMoreSequence = false;
                    break;
                case TweenPlayMode.Step:
                    hasMoreSequence = false;
                    NextStep();
                    UpdateStep();
                    break;
                case TweenPlayMode.Sequence:
                    NextStep();
                    hasMoreSequence = !UpdateStep();
                    break;
                case TweenPlayMode.Loop:
                    hasMoreSequence = true;
                    NextStep();
                    UpdateStep();
                    break;
                default:
                    break;
                }

            } while (hasMoreSequence);
        }

        protected void NextStep()
        {
            step = Mathf.Clamp(step + (!rewind ? 1 : -1), -1, StepCount);
        }

        protected bool UpdateStep()
        {
            bool completeSequence = false;

            switch (loopType)
            {
            case LoopType.None:
                nextStep = step + 1;
                break;
            case LoopType.Loop:
                if (!rewind)
                {
                    if (step >= (StepCount - 1))
                    {
                        step = 0;
                        completeSequence = true;
                    }
                }
                else
                {
                    if (step == -1)
                    {
                        step = StepCount - 2;
                        completeSequence = true;
                    }
                }
                nextStep = step + 1;
                break;
            case LoopType.Circle:
                if (!rewind)
                {
                    if (step == (StepCount - 1))
                        nextStep = 0;
                    else if (step == StepCount)
                    {
                        step = 0;
                        nextStep = 1;
                        completeSequence = true;
                    }
                    else 
                        nextStep = step + 1;
                }
                else 
                {
                    if (step == (StepCount - 1))
                        nextStep = 0;
                    else if (step == -1)
                    {
                        step = StepCount - 1;
                        nextStep = 0;
                        completeSequence = true;
                    }
                    else
                        nextStep = step + 1;
                }
                break;
            case LoopType.PingPong:
                if (!rewind)
                {
                    if (step >= (StepCount - 1))
                    {
                        step = StepCount - 2;
                        rewind = !rewind;
                        completeSequence = true;
                    }
                    nextStep = step + 1;
                }
                else 
                {
                    if (step == -1)
                    {
                        step = 0;
                        rewind = !rewind;
                        completeSequence = true;
                    }
                    nextStep = step + 1;
                }
                break;
            default:
                break;
            }

            if (completeSequence && onCompleteSequence != null)
                onCompleteSequence.Invoke();

            return completeSequence;
        } 

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            Validate();
        }

        protected virtual void Validate()
        {
            if (!target && propertyInjector)
                target = propertyInjector.GetComponent(gameObject);
        }
#endif
    }

    public abstract class TweenMediator<T, Sequence> : TweenMediator
        where Sequence : TweenSequence<T>, new()
    {
        public List<Sequence> sequence = new List<Sequence>();

        protected TweenStrategy tween;
        protected T from;
        protected T to;
        protected T velocity;
        protected T desiredVelocity;

        public override int StepCount { get { return sequence.Count; } }
        
        protected System.Action updateTween;
        protected System.Func<bool> isCompleteTween;

        protected abstract T GetValue(Component comp);
        protected abstract void SetValue(Component comp, T value);

        public override void Reset()
        {
            velocity = default(T);

            if (StepCount > 0)
            {
                if (target)
                {
                    switch (resetMode)
                    {
                    case TweenResetMode.SetFrom:
                        SetValue(target, GetSequenceValue(sequence[0]));
                        break;
                    case TweenResetMode.SetTo:
                        SetValue(target, GetSequenceValue(sequence[StepCount - 1]));
                        break;
                    default:
                        break;
                    }
                }
            }

            if (onReset != null)
                onReset.Invoke();
        }

        public override bool StaticTween() 
        { 
            return tween.StaticTween();
        }

        public override void SetupTween()
        {
            step = Mathf.Clamp(step, 0, StepCount - 1);
            nextStep = Mathf.Clamp(nextStep, 0, StepCount - 1);
            tween = sequence[step].tween;
            duration = sequence[step].duration;
            delay = sequence[step].delay;
        }

        public override void StartTween()
        {
            playback = !rewind ? 0f : duration;
            if (interruptMode == TweenInterruptMode.Reset)
            {
                from = GetSequenceValue(sequence[step]);
                to = GetSequenceValue(sequence[nextStep]);
            }
            else if (interruptMode == TweenInterruptMode.Blend)
            {
                from = !rewind ? GetValue(target) : GetSequenceValue(sequence[step]);
                to = !rewind ? GetSequenceValue(sequence[nextStep]) : GetValue(target);
            }

            if (!target || !propertyInjector || !tween)
            {
                updateTween = null;
                isCompleteTween = null;
            }
            else if (StaticTween())
            {
                updateTween = UpdateStaticTween;
                isCompleteTween = IsCompleteStaticTween;

                SetValue(target, !rewind ? from : to);
            }
            else
            {
                updateTween = UpdateDynamicTween;
                isCompleteTween = IsCompleteDynamicTween;
            }

            if (onStart != null)
                onStart.Invoke();
        }

        public override void UpdateTween()
        {
            if (updateTween != null)
                updateTween();

            if (onUpdate != null)
                onUpdate.Invoke();
        }

        public override void CompleteTween()
        {
            if (onCompleteStep != null)
                onCompleteStep.Invoke();
        }

        public override bool IsCompleteTween()
        {
            if (isCompleteTween != null)
                return isCompleteTween();

            return true;
        }

        protected override void UpdateStaticTween()
        {
            base.UpdateStaticTween();
            
            if (sequence[nextStep].targetMode == TweenTargetMode.Dynamic)
                to = GetValue(sequence[nextStep].GetComponent());
        }

        protected override void UpdateDynamicTween()
        {
            if (sequence[nextStep].targetMode == TweenTargetMode.Dynamic)
                to = GetValue(sequence[nextStep].GetComponent());
        }

        protected T GetSequenceValue(Sequence tweenTarget)
        {
            if (tweenTarget.targetMode == TweenTargetMode.Static) return tweenTarget.value;
            return GetValue(tweenTarget.GetComponent());
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        protected override void Validate()
        {
            base.Validate();

            while (StepCount < 2)
                sequence.Add(new Sequence());

            UpdateStep();
        }
#endif
    }

    [Serializable]
    public class TweenSequence<T>
    {
        public TweenTargetMode targetMode = TweenTargetMode.Static;

        [ShowIf("targetMode", TweenTargetMode.Static)]
        public T value;

        [ShowIf("targetMode", TweenTargetMode.Dynamic)]
        public Component reference;

        [ShowIf("targetMode", TweenTargetMode.Dynamic)]
        [InlineEditor]
        public VariableComponent dynamicReference;

        public Component GetComponent()
        {
            return (reference != null) ? reference : dynamicReference.value;
        }

        public float delay;
        public float duration;

        [InlineEditor]
        public TweenStrategy tween;
    }

    [Serializable]
    public class TweenSequenceSingle : TweenSequence<float> {}

    [Serializable]
    public class TweenSequenceVector2 : TweenSequence<Vector2> {}

    [Serializable]
    public class TweenSequenceVector3 : TweenSequence<Vector3> {}

    [Serializable]
    public class TweenSequenceVector4 : TweenSequence<Vector4> {}

    [Serializable]
    public class TweenSequenceColor : TweenSequence<Color> {}
}