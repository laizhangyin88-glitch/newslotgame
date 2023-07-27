using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.IoC;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public abstract class SlotInstance : MonoBehaviour, ISpinnable
    {
        [PropertyOrder(1000)]
        public bool debug;

        [HideInInspector]
        [SerializeField]
        protected SpinState _spinState = SpinState.Stopped;

        [ShowInInspector]
        [PropertyOrder(-100)]
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
        private float _stoppedTime;
        public float stoppedTime { get { return _stoppedTime; } }

        public float GetSpinStateTime(SpinState spinState_)
        {
            if ((int)_spinState >= (int)spinState_)
            {
                switch (spinState_)
                {
                    case SpinState.Spinning:
                        return _spinningTime;
                    case SpinState.Stopping:
                        return _stoppingTime;
                    case SpinState.Stopped:
                        return _stoppedTime;
                }
            }

            return 0f;
        }

        [Serializable]
        public class SlotInstanceEvent : UnityEvent<SlotInstance> { }

        [TabGroup("SlotInstance", "Events")]
        [PropertyOrder(500)]
        public SlotInstanceEvent onSpinning;

        [TabGroup("SlotInstance", "Events")]
        [PropertyOrder(501)]
        public SlotInstanceEvent onStopping;

        [TabGroup("SlotInstance", "Events")]
        [PropertyOrder(502)]
        public SlotInstanceEvent onStopped;

        protected virtual void OnSpinning()
        {
            mediator.OnSpinning(this);
            onSpinning.Invoke(this);
        }

        protected virtual void OnStopping()
        {
            mediator.OnStopping(this);
            onStopping.Invoke(this);
        }

        protected virtual void OnStopped()
        {
            mediator.OnStopped(this);
            onStopped.Invoke(this);
        }

        [TabGroup("SlotInstance", "Setup")]
        [InlineEditor]
        public SlotMediator mediator;

        [HideInInspector]
        [SerializeField]
        protected int _xMin;
        [TabGroup("SlotInstance", "Setup")]
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
        [TabGroup("SlotInstance", "Setup")]
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
        [TabGroup("SlotInstance", "Setup")]
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
        [TabGroup("SlotInstance", "Setup")]
        [ShowInInspector]
        public int yMax
        {
            get { return _yMax; }
            set
            {
                if (_yMax != value)
                {
                    _yMax = Mathf.Max(value, _xMin + 1);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _zMin;
        [TabGroup("SlotInstance", "Setup")]
        [ShowInInspector]
        public int zMin
        {
            get { return _zMin; }
            set
            {
                if (_zMin != value)
                {
                    _zMin = value;
                    _zMax = Mathf.Max(_zMax, _zMin + 1);
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _zMax = 1;
        [TabGroup("SlotInstance", "Setup")]
        [ShowInInspector]
        public int zMax
        {
            get { return _zMax; }
            set
            {
                if (_zMax != value)
                {
                    _zMax = Mathf.Max(value, _zMin + 1);
                }
            }
        }

        [TabGroup("SlotInstance", "Setup")]
        [ShowInInspector]
        public int columnCount
        {
            get { return _xMax - _xMin; }
            set { _xMax = _xMin + Mathf.Max(value, 1); }
        }

        [TabGroup("SlotInstance", "Setup")]
        [ShowInInspector]
        public int rowCount
        {
            get { return _yMax - _yMin; }
            set { _yMax = _yMin + Mathf.Max(value, 1); }
        }

        [TabGroup("SlotInstance", "Setup")]
        [ShowInInspector]
        public int layerCount
        {
            get { return _zMax - _zMin; }
            set { _zMax = _zMin + Mathf.Max(value, 1); }
        }

        [TabGroup("SlotInstance", "Setup")]
        [InlineEditor]
        public VariableFloat timeScale;

        [Serializable]
        public class SlotLayer
        {
            public List<ReelInstance> reels = new List<ReelInstance>();

            public void Clear()
            {
                foreach (var reel in reels)
                {
                    reel.Clear();
                }
            }

            public void SendSymbolEvent(string eventName)
            {
                foreach (var reel in reels)
                {
                    reel.SendSymbolEvent(eventName);
                }
            }

            public int GetReelIndex(ReelInstance reelInstance)
            {
                for (int i = 0, count = reels.Count; i < count; ++i)
                {
                    if (reels[i] == reelInstance)
                        return i;
                }
                return -1;
            }

            public ReelInstance GetReelByIndex(int index)
            {
                return reels[index];
            }

            public Vector3 GetReelPositionByIndex(int index)
            {
                return GetReelByIndex(index).GetReelPosition();
            }

            public ReelInstance GetReel(int x, int y)
            {
                foreach (var reel in reels)
                {
                    if (reel.HasSymbol(x, y))
                        return reel;
                }
                return null;
            }

            public Vector3 GetReelPosition(int x, int y)
            {
                var reel = GetReel(x, y);
                return reel != null ? reel.GetReelPosition() : Vector3.zero;
            }

            public SymbolInstance GetSymbol(int x, int y)
            {
                var reel = GetReel(x, y);
                return reel != null ? reel.GetSymbol(x, y) : null;
            }

            public void SetSymbol(int x, int y, SymbolInstance symbolInstance)
            {
                var reel = GetReel(x, y);
                if (reel != null) reel.SetSymbol(x, y, symbolInstance);
            }

            public void AddSymbol(int x, int y, SymbolInstance symbolInstance)
            {
                var reel = GetReel(x, y);
                if (reel != null) reel.AddSymbol(x, y, symbolInstance);
            }

            public Vector3 GetSymbolPosition(int x, int y)
            {
                var reel = GetReel(x, y);
                return reel != null ? reel.GetSymbolPosition(x, y) : Vector3.zero;
            }
        }
        [TabGroup("SlotInstance", "Setup")]
        public List<SlotLayer> layers = new List<SlotLayer>();

        protected float fixedDeltaTime { get { return Time.fixedDeltaTime * timeScale.value; } }
        protected float deltaTime { get { return Time.deltaTime * timeScale.value; } }

        protected virtual void Awake()
        {
            mediator.Instance = this;
        }

        protected virtual void OnDestroy()
        {
            mediator.Instance = null;
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("DynamicControl")]
        [PropertyOrder(1010)]
        public virtual void Initialize()
        {
            foreach (var layer in layers)
            {
                foreach (var reel in layer.reels)
                {
                    reel.Initialize();
                }
            }
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("DynamicControl")]
        [PropertyOrder(1011)]
        public virtual void Clear()
        {
            foreach (var layer in layers)
            {
                layer.Clear();
            }
        }

        public virtual void Clear(int layer)
        {
            layers[layer].Clear();
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("DynamicControl")]
        [PropertyOrder(1012)]
        public virtual void Shuffle()
        {
            foreach (var layer in layers)
            {
                foreach (var reel in layer.reels)
                {
                    reel.Shuffle();
                }
            }
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1013)]
        public virtual void Spin()
        {
            SendEvent("Spin");
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1014)]
        public virtual void Stop()
        {
            SendEvent("Stop");
        }

        [ShowIf("debug")]
        [Button]
        [ButtonGroup("MovementControl")]
        [PropertyOrder(1020)]
        public abstract void Skip();


        public virtual int GetReelIndex(ReelInstance reelInstance)
        {
            return layers[reelInstance.z].GetReelIndex(reelInstance);
        }

        public virtual ReelInstance GetReelByIndex(int index, int layer)
        {
            return layers[layer].GetReelByIndex(index);
        }

        public virtual Vector3 GetReelPositionByIndex(int index, int layer)
        {
            return layers[layer].GetReelPositionByIndex(index);
        }

        public virtual ReelInstance GetReel(int x, int y, int z)
        {
            return layers[z].GetReel(x, y);
        }

        public virtual Vector3 GetReelPosition(int x, int y, int z)
        {
            return layers[z].GetReelPosition(x, y);
        }

        public virtual SymbolInstance GetSymbol(int x, int y, int z)
        {
            return layers[z].GetSymbol(x, y);
        }

        public virtual void SetSymbol(int x, int y, int z, SymbolInstance symbolInstance)
        {
            layers[z].SetSymbol(x, y, symbolInstance);
        }

        public virtual void AddSymbol(int x, int y, int z, SymbolInstance symbolInstance)
        {
            layers[z].AddSymbol(x, y, symbolInstance);
        }

        public virtual void MoveSymbolLayer(int x, int y, int z, int targetLayer)
        {
            var symbolInstance = GetSymbol(x, y, z);
            SetSymbol(x, y, z, null);

            var targetSymbolInstance = GetSymbol(x, y, targetLayer);
            if (targetSymbolInstance == null)
                SetSymbol(x, y, targetLayer, symbolInstance);
            else
                AddSymbol(x, y, targetLayer, symbolInstance);
        }

        public virtual Vector3 GetSymbolPosition(int x, int y, int z)
        {
            return layers[z].GetSymbolPosition(x, y);
        }

        public virtual void TotalWin(SpinOutputWinningSubset eventData)
        {
            SendSymbolEvent("Win", eventData.spots);
        }

        public virtual void SingleWin(SpinWin eventData)
        {
            SendSymbolEvent("Win", eventData.spots);
        }
        
        public virtual void SingleWin(List<Cell3> spots)
        {
            SendSymbolEvent("Win", spots);
        }

        public virtual void SingleWin(List<List<Cell3>> spots)
        {
            foreach (var spot in spots)
            {
                SingleWin(spot);
            }
        }

        public virtual void SkipWin(SpinOutputWinningSubset eventData)
        {
            SendSymbolEvent("Idle", eventData.spots);
        }

        public abstract void SendEvent(string eventName);

        public virtual void SendSymbolEvent(string eventName, IEnumerable<Cell3> spots)
        {
            foreach (var spot in spots)
            {
                GetSymbol(spot.x, spot.y, spot.z).SendEvent(eventName);
            }
        }

        public virtual void SendSymbolEvent(string eventName, int layer)
        {
            layers[layer].SendSymbolEvent(eventName);
        }
    }
}