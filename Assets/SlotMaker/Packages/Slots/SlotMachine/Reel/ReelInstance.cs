using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.IoC;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public abstract class ReelInstance : MonoBehaviour, ISpinnable
    {
        [PropertyOrder(1000)]
        public bool debug;

        [HideInInspector]
        [SerializeField]
        protected SpinState _spinState = SpinState.Stopped;

        [ShowInInspector]
        [PropertyOrder(-101)]
        public SpinState spinState
        {
            get { return _spinState; }
            set
            {
                if (_spinState != value)
                {
                    _spinState = value;
                    switch (_spinState)
                    {
                        case SpinState.Spinning:
                            _spinningTime = Time.time;
                            OnSpinning();
                            break;
                        case SpinState.Stopping:
                            _stoppingTime = Time.time;
                            OnStopping();
                            break;
                        case SpinState.PrepareStopped:
                            _prepareStoppedTime = Time.time;
                            OnPrepareStopped();
                            break;
                        case SpinState.Stopped:
                            _stoppedTime = Time.time;
                            OnStopped();
                            break;
                    }
                }
            }
        }

        private float _spinningTime;
        public float spinningTime { get { return _spinningTime; } }
        private float _stoppingTime;
        public float stoppingTime { get { return _stoppingTime; } }
        private float _prepareStoppedTime;
        public float prepareStoppedTime { get { return _prepareStoppedTime; } }
        private float _stoppedTime;
        public float stoppedTime { get { return _stoppedTime; } }

        public float GetSpinStateTime(SpinState spinState_)
        {
            switch (spinState_)
            {
                case SpinState.Spinning:
                    return _spinningTime;
                case SpinState.Stopping:
                    return _stoppingTime;
                case SpinState.PrepareStopped:
                    return _prepareStoppedTime;
                case SpinState.Stopped:
                    return _stoppedTime;
            }

            return 0f;
        }

        [PropertyOrder(-100)]
        [InlineEditor]
        public VariableBool locked;

        [Serializable]
        public class ReelInstanceEvent : UnityEvent<ReelInstance> {}

        [TabGroup("ReelInstance", "Events")]
        [PropertyOrder(500)]
        public ReelInstanceEvent onSpinning;

        [TabGroup("ReelInstance", "Events")]
        [PropertyOrder(501)]
        public ReelInstanceEvent onStopping;

        [TabGroup("ReelInstance", "Events")]
        [PropertyOrder(502)]
        public ReelInstanceEvent onPrepareStopped;

        [TabGroup("ReelInstance", "Events")]
        [PropertyOrder(503)]
        public ReelInstanceEvent onStopped;

        protected virtual void OnSpinning()
        {
            mediator.OnSpinningReel(this);
            onSpinning.Invoke(this);
        }

        protected virtual void OnStopping()
        {
            mediator.OnStoppingReel(this);
            onStopping.Invoke(this);
        }

        protected virtual void OnPrepareStopped()
        {
            mediator.OnPrepareStoppedReel(this);
            onPrepareStopped.Invoke(this);
        }

        protected virtual void OnStopped()
        {
            mediator.OnStoppedReel(this);
            onStopped.Invoke(this);
        }

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public SlotMediator mediator;

        [HideInInspector]
        [SerializeField]
        protected int _xMin;
        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int xMin
        {
            get { return _xMin; }
            set
            {
                if (_xMin != value)
                {
                    _xMin = value;
                    _xMax = Mathf.Max(_xMax, _xMin + 1);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _xMax = 1;
        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int xMax
        {
            get { return _xMax; }
            set
            {
                if (_xMax != value)
                {
                    _xMax = Mathf.Max(value, _xMin + 1);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _yMin;
        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int yMin
        {
            get { return _yMin; }
            set
            {
                if (_yMin != value)
                {
                    _yMin = value;
                    _yMax = Mathf.Max(_yMax, _yMin + 1);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _yMax = 1;
        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int yMax
        {
            get { return _yMax; }
            set
            {
                if (_yMax != value)
                {
                    _yMax = Mathf.Max(value, _yMin + 1);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _z;
        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int z
        {
            get { return _z; }
            set
            {
                if (_z != value)
                {
                    _z = value;
                }
            }
        }

        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int columnCount
        {
            get { return _xMax - _xMin; }
            set { _xMax = _xMin + Mathf.Max(value, 1); }
        }

        [TabGroup("ReelInstance", "Setup")]
        [ShowInInspector]
        public int rowCount
        {
            get { return _yMax - _yMin; }
            set { _yMax = _yMin + Mathf.Max(value, 1); }
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("DynamicControl")]
        [PropertyOrder(1010)]
        public abstract void Initialize();

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("DynamicControl")]
        [PropertyOrder(1011)]
        public abstract void Shuffle();

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("DynamicControl")]
        [PropertyOrder(1012)]
        public abstract void Clear();

        public virtual bool HasSymbol(int x, int y)
        {
            return (x >= _xMin) && (x < _xMax) && (y >= _yMin) && (y < _yMax);
        }

        public abstract Vector3 GetReelPosition();
        public abstract SymbolInstance GetSymbol(int x, int y);
        public abstract void SetSymbol(int x, int y, SymbolInstance symbolInstance);
        public abstract void AddSymbol(int x, int y, SymbolInstance symbolInstance);
        public abstract Vector3 GetSymbolPosition(int x, int y);
        public abstract SymbolInstance GetPatchingSymbol(int x, int y);

        [TabGroup("ReelInstance", "Physics")]
        [PropertyOrder(200)]
        [InlineEditor]
        public VariableFloat timeScale;

        protected float fixedDeltaTime { get { return Time.fixedDeltaTime * timeScale.value; } }
        protected float deltaTime { get { return Time.deltaTime * timeScale.value; } }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1012)]
        public virtual void Spin()
        {
            SendEvent(SPIN);
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1012)]
        public virtual void Stop()
        {
            SendEvent(STOP);
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1012)]
        public virtual void Expectation()
        {
            SendEvent(EXPECTATION);
        }

        protected const string SPIN = "Spin";
        protected const string STOP = "Stop";
        protected const string EXPECTATION = "Expectation";
        protected const string SKIP = "Skip";

        public abstract void Skip();

        public abstract void SendEvent(string eventName);
        public abstract void SendSymbolEvent(string eventName);

        public abstract void ApplyFrontPatch();
        public abstract void ApplyBackPatch();
    }
}