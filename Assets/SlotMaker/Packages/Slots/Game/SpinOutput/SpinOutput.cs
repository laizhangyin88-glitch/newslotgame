using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    [CreateAssetMenu(fileName="New Spin Output", menuName="SlotMaker2/Math/Output/Spin Output")]
    public class SpinOutput : ScriptableObject
    {
        [Serializable]
        public class ReelOutput
        {
            public bool active = true;

            [TabGroup("ReelOutput", "Setup")]
            [InlineEditor]
            public SymbolStrip strip;

            [HideInInspector]
            [SerializeField]
            protected int _xMin;
            [TabGroup("ReelOutput", "Setup")]
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
                        _columnCount = _xMax - _xMin;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _xMax = 1;
            [TabGroup("ReelOutput", "Setup")]
            [ShowInInspector]
            public int xMax
            {
                get { return _xMax; }
                set 
                {
                    if (_xMax != value)
                    {
                        _xMax = Mathf.Max(value, _xMin + 1);
                        _columnCount = _xMax - _xMin;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _yMin;
            [TabGroup("ReelOutput", "Setup")]
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
                        _rowCount = _yMax - _yMin;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _yMax = 1;
            [TabGroup("ReelOutput", "Setup")]
            [ShowInInspector]
            public int yMax
            {
                get { return _yMax; }
                set 
                {
                    if (_yMax != value)
                    {
                        _yMax = Mathf.Max(value, _yMin + 1);
                        _rowCount = _yMax - _yMin;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _z;
            [TabGroup("ReelOutput", "Setup")]
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

            [HideInInspector]
            [SerializeField]
            protected int _columnCount = 1;
            [TabGroup("ReelOutput", "Setup")]
            [ShowInInspector]
            public int columnCount
            {
                get { return _columnCount; }
                set 
                { 
                    _xMax = _xMin + Mathf.Max(value, 1); 
                    _columnCount = _xMax - _xMin;
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _rowCount = 1;
            [TabGroup("ReelOutput", "Setup")]
            [ShowInInspector]
            public int rowCount
            {
                get { return _rowCount; }
                set 
                { 
                    _yMax = _yMin + Mathf.Max(value, 1); 
                    _rowCount = _yMax - _yMin;
                }
            }

            [InlineEditor]
            public VariableInt index;

            [NonSerialized]
            [ShowInInspector]
            public List<OverridenSymbolEntity> output = new List<OverridenSymbolEntity>();

            public void Reset()
            {
                output.Clear();
                for (int i = 0, count = columnCount * rowCount; i < count; ++i)
                {
                    output.Add(OverridenSymbolEntity.zero);
                }
            }

            public void Initialize()
            {
                if (strip)
                {
                    for (int i = 0, count = rowCount; i < count; ++i)
                    {
                        output[i] = strip.GetSymbol(index.value + i);
                    }
                }
            }

            public void SetIndex(int index_)
            {
                index.value = index_;
                Initialize();
            }

            public void Shuffle()
            {
                if (strip)
                {
                    index.value = strip.randomIndex;
                    Initialize();
                }
            }

            public void Clear()
            {
                for (int i = 0, count = columnCount * rowCount; i < count; ++i)
                {
                    output[i] = OverridenSymbolEntity.zero;
                }
            }

            public bool HasSymbol(int x, int y)
            {
                return (x >= _xMin) && (x < _xMax) && (y >= _yMin) && (y < _yMax);
            }

            public OverridenSymbolEntity GetSymbol(int x, int y)
            {
                return output[(x - _xMin) * _rowCount + (y - _yMin)];
            }

            public void SetSymbol(int x, int y, OverridenSymbolEntity symbol)
            {
                output[(x - _xMin) * _rowCount + (y - _yMin)] = symbol;
            }
        }

        [Serializable]
        public class SlotOutputLayer
        {
            public bool active;

            public List<ReelOutput> reels = new List<ReelOutput>();

            public void Reset()
            {
                for (int i = 0, count = reels.Count; i < count; ++i)
                {
                    reels[i].Reset();
                }
            }

            public void SetIndices(List<int> indices)
            {
                for (int i = 0, count = reels.Count; i < count; ++i)
                {
                    reels[i].SetIndex(indices[i]);
                }
            }

            public void Shuffle()
            {
                for (int i = 0, count = reels.Count; i < count; ++i)
                {
                    reels[i].Shuffle();
                }
            }

            public OverridenSymbolEntity GetSymbol(int x, int y)
            {
                return reels[reels.Count > 1 ? x : 0].GetSymbol(x, y);
            }

            public void SetSymbol(int x, int y, OverridenSymbolEntity symbol)
            {
                reels[reels.Count > 1 ? x : 0].SetSymbol(x, y, symbol);
            }
        }
        public List<SlotOutputLayer> layers = new List<SlotOutputLayer>();

        [Serializable]
        public class VisibleArea
        {
            [HideInInspector]
            [SerializeField]
            protected int _xMin;
            [TabGroup("VisibleArea", "Setup")]
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
                        _columnCount = _xMax - _xMin;
                        _volumn = _columnCount * _rowCount;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _xMax = 1;
            [TabGroup("VisibleArea", "Setup")]
            [ShowInInspector]
            public int xMax
            {
                get { return _xMax; }
                set 
                {
                    if (_xMax != value)
                    {
                        _xMax = Mathf.Max(value, _xMin + 1);
                        _columnCount = _xMax - _xMin;
                        _volumn = _columnCount * _rowCount;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _yMin;
            [TabGroup("VisibleArea", "Setup")]
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
                        _rowCount = _yMax - _yMin;
                        _volumn = _columnCount * _rowCount;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _yMax = 1;
            [TabGroup("VisibleArea", "Setup")]
            [ShowInInspector]
            public int yMax
            {
                get { return _yMax; }
                set 
                {
                    if (_yMax != value)
                    {
                        _yMax = Mathf.Max(value, _yMin + 1);
                        _rowCount = _yMax - _yMin;
                        _volumn = _columnCount * _rowCount;
                    }
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _columnCount = 1;
            [TabGroup("VisibleArea", "Setup")]
            [ShowInInspector]
            public int columnCount
            {
                get { return _columnCount; }
                set 
                { 
                    _xMax = _xMin + Mathf.Max(value, 1); 
                    _columnCount = _xMax - _xMin;
                    _volumn = _columnCount * _rowCount;
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _rowCount = 1;
            [TabGroup("VisibleArea", "Setup")]
            [ShowInInspector]
            public int rowCount
            {
                get { return _rowCount; }
                set 
                { 
                    _yMax = _yMin + Mathf.Max(value, 1); 
                    _rowCount = _yMax - _yMin;
                    _volumn = _columnCount * _rowCount;
                }
            }

            [HideInInspector]
            [SerializeField]
            protected int _volumn = 1;
            [TabGroup("VisibleArea", "Setup")]
            [ShowInInspector]
            public int volumn
            {
                get { return _volumn; }
            }
        }
        public List<VisibleArea> visibleAreas = new List<VisibleArea>();

        [HideInInspector]
        [SerializeField]
        protected int _xMin;
        [TabGroup("SpinOutput", "Setup")]
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
                    _columnCount = _xMax - _xMin;
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _xMax = 1;
        [TabGroup("SpinOutput", "Setup")]
        [ShowInInspector]
        public int xMax
        {
            get { return _xMax; }
            set 
            {
                if (_xMax != value)
                {
                    _xMax = Mathf.Max(value, _xMin + 1);
                    _columnCount = _xMax - _xMin;
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _yMin;
        [TabGroup("SpinOutput", "Setup")]
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
                    _rowCount = _yMax - _yMin;
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _yMax = 1;
        [TabGroup("SpinOutput", "Setup")]
        [ShowInInspector]
        public int yMax
        {
            get { return _yMax; }
            set 
            {
                if (_yMax != value)
                {
                    _yMax = Mathf.Max(value, _xMin + 1);
                    _rowCount = _yMax - _yMin;
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _zMin;
        [TabGroup("SpinOutput", "Setup")]
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
                    _layerCount = _zMax - _zMin;
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _zMax = 1;
        [TabGroup("SpinOutput", "Setup")]
        [ShowInInspector]
        public int zMax
        {
            get { return _zMax; }
            set 
            {
                if (_zMax != value)
                {
                    _zMax = Mathf.Max(value, _zMin + 1);
                    _layerCount = _zMax - _zMin;
                }
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _columnCount = 1;
        [TabGroup("SpinOutput", "Setup")]
        [ShowInInspector]
        public int columnCount
        {
            get { return _columnCount; }
            set 
            { 
                _xMax = _xMin + Mathf.Max(value, 1); 
                _columnCount = _xMax - _xMin;
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _rowCount = 1;
        [TabGroup("SpinOutput", "Setup")]
        [ShowInInspector]
        public int rowCount
        {
            get { return _rowCount; }
            set 
            { 
                _yMax = _yMin + Mathf.Max(value, 1);
                _rowCount = _yMax - _yMin;
            }
        }

        [HideInInspector]
        [SerializeField]
        protected int _layerCount = 1;
        [TabGroup("SpinOutput", "Setup")]
        [ShowInInspector]
        public int layerCount
        {
            get { return _layerCount; }
            set 
            { 
                _zMax = _zMin + Mathf.Max(value, 1); 
                _layerCount = _zMax - _zMin;
            }
        }

        [Button]
        public void Reset()
        {
            foreach (var layer in layers)
            {
                layer.Reset();
            }
        }

        [Button]
        public void Shuffle()
        {
            foreach (var layer in layers)
            {
                layer.Shuffle();
            }
        }

        public void SetIndices(List<int> indices, int layer)
        {
            layers[layer].SetIndices(indices);
        }

        public SlotOutputLayer GetLayer(int layer) { return layers[layer]; }

        public OverridenSymbolEntity GetBackSymbol(int x, int y, out int z)
        {
            for (z = _zMax - 1; z >= _zMin; --z)
            {
                var layer = layers[z];
                if (!layer.active) continue;
                var symbol = layer.GetSymbol(x, y);
                if (!symbol.isNull)
                    return symbol;
            }
            return null;
        }

        public OverridenSymbolEntity GetSymbol(int x, int y, int z)
        {
            return layers[z].GetSymbol(x, y);
        }

        public void SetSymbol(int x, int y, int z, OverridenSymbolEntity symbol)
        {
            layers[z].SetSymbol(x, y, symbol);
        }

        protected void OnEnable()
        {
            Reset();
        }
    }
}