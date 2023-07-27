using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlotMaker.IoC;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public partial class Reel2D : ReelInstance, IBody
    {
        protected override void OnStopped()
        {
            base.OnStopped();

            renderPosition -= position;
            velocity = position = Vector3.zero;
            damping = drag = 0f;
            interpolation.Reset();
        }

        [PropertyOrder(-99)]
        [InlineEditor]
        public VariableInt index;

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public List<SymbolStrip> strips = new List<SymbolStrip>();

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public VariableGameObjectList prefabs;

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public CommandSet commandAsset;

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public SymbolPositionStrategy positionStrategy;

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public SymbolSortingOrderStrategy sortingOrderStrategy;

        [TabGroup("ReelInstance", "Setup")]
        [InlineEditor]
        public StripPatchStrategy patchStrategy;

        [TabGroup("ReelInstance", "Setup")]
        public StartAction startShuffle = StartAction.DoNothing;

        [TabGroup("ReelInstance", "Setup")]
        public RectTransform content;

        [Serializable]
        public class DynamicSymbolStrip
        {
            public SymbolStrip source;
            public List<OverridenSymbolEntity> value = new List<OverridenSymbolEntity>();
#if UNITY_EDITOR
            public List<SymbolEntity> debugValue = new List<SymbolEntity>();
#endif

            public int Count { get { return value.Count; } }

            public int frontIndex { get { return value[0].index; } }
            public int backIndex { get { return value[value.Count - 1].index; } }

            [Serializable]
            public class Patch
            {
                public int front = -1;
                public int back = -1;
                public int distance;

                public void SetPatch(int front_, int back_)
                {
                    front = front_;
                    back = back_;
                    distance = 0;
                }

                public void Reset()
                {
                    SetPatch(-1, -1);
                }

                public void Pushed() { ++distance; }
                public void Popped() { --distance; }
            }
            public Patch frontPatch = new Patch();
            public Patch backPatch = new Patch();

            public DynamicSymbolStrip(SymbolStrip strip, int index, int count)
            {
                Initialize(strip, index, count);
            }

            public void Initialize(SymbolStrip strip, int index, int count)
            {
                source = strip;
                value.Clear();
#if UNITY_EDITOR
                debugValue.Clear();
#endif

                for (int i = 0; i < count; ++i)
                {
                    var entity = CloneSymbol(index + i);

                    value.Add(entity);
#if UNITY_EDITOR
                    debugValue.Add(entity.master);
#endif
                }
            }

            public void PushFront(OverridenSymbolEntity entity)
            {
                value.Insert(0, entity);
#if UNITY_EDITOR
                debugValue.Insert(0, entity.master);
#endif
                frontPatch.Pushed();
            }

            public void PushFront()
            {
                PushFront(GetFrontSymbol());
            }

            public void PushFront(int count)
            {
                for (int i = 0; i < count; ++i)
                {
                    PushFront();
                }
            }

            public void PopFront()
            {
                value[0].ReturnToPool();
                value.RemoveAt(0);
#if UNITY_EDITOR
                debugValue.RemoveAt(0);
#endif
                frontPatch.Popped();
            }

            public void PopFront(int count)
            {
                for (int i = 0; i < count; ++i)
                {
                    PopFront();
                }
            }

            public void PushBack(OverridenSymbolEntity entity)
            {
                value.Add(entity);
#if UNITY_EDITOR
                debugValue.Add(entity.master);
#endif
                backPatch.Pushed();
            }

            public void PushBack()
            {
                PushBack(GetBackSymbol());
            }

            public void PushBack(int count)
            {
                for (int i = 0; i < count; ++i)
                {
                    PushBack();
                }
            }

            public void PopBack()
            {
                value[value.Count - 1].ReturnToPool();
                value.RemoveAt(value.Count - 1);
#if UNITY_EDITOR
                debugValue.RemoveAt(debugValue.Count - 1);
#endif
                backPatch.Popped();
            }

            public void PopBack(int count)
            {
                for (int i = 0; i < count; ++i)
                {
                    PopBack();
                }
            }

            public void Insert(int beginIndex, List<OverridenSymbolEntity> entities)
            {
                value.InsertRange(beginIndex, entities);
            }

            public void Replace(int beginIndex, List<OverridenSymbolEntity> entities)
            {
                for (int i = 0, count = entities.Count; i < count; ++i)
                {
                    value[beginIndex + i] = entities[i];
                }
            }

            public void Remove(int beginIndex, int count)
            {
                value.RemoveRange(beginIndex, count);
            }

            public OverridenSymbolEntity GetFrontSymbol()
            {
                if (frontIndex == frontPatch.back)
                {
                    var symbol = CloneSymbol(frontPatch.front);
                    frontPatch.Reset();
                    return symbol;
                }

                if (value.Count > 0)
                {
                    var frontSymbol = value[0];
                    if (frontSymbol.rowOffset > 0)
                    {
                        var symbol = (OverridenSymbolEntity)frontSymbol.Clone();
                        symbol.index = source.GetCircleIndex(symbol.index - 1);
                        --symbol.rowOffset;
                        symbol.MarkOverriden();
                        return symbol;
                    }
                }
                
                return CloneSymbol(frontIndex - 1);
            }

            public OverridenSymbolEntity GetBackSymbol()
            {
                if (backIndex == backPatch.front)
                {
                    var symbol = CloneSymbol(backPatch.back);
                    backPatch.Reset();
                    return symbol;
                }

                if (value.Count > 0)
                {
                    var backSymbol = value[value.Count - 1];
                    if ((backSymbol.rowCount - backSymbol.rowOffset) > 1)
                    {
                        var symbol = (OverridenSymbolEntity)backSymbol.Clone();
                        symbol.index = source.GetCircleIndex(symbol.index + 1);
                        ++symbol.rowOffset;
                        symbol.MarkOverriden();
                        return symbol;
                    }
                }
                return CloneSymbol(backIndex + 1);    
            }

            public void SetFrontPatch(int front, int back)
            {
                frontPatch.front = source.GetCircleIndex(front);
                frontPatch.back = source.GetCircleIndex(back);
            }

            public void SetBackPatch(int front, int back)
            {
                frontPatch.front = source.GetCircleIndex(front);
                frontPatch.back = source.GetCircleIndex(back);
            }

            public void ClearPatch()
            {
                SetFrontPatch(-1, -1);
                SetBackPatch(-1, -1);
            }

            protected OverridenSymbolEntity CloneSymbol(int index)
            {
                return (OverridenSymbolEntity)source.GetSymbol(index).Clone();
            }
        }

        [TabGroup("ReelInstance", "Dynamic")]
        [PropertyOrder(100)]
        public List<DynamicSymbolStrip> dynamicStrips = new List<DynamicSymbolStrip>();
        [TabGroup("ReelInstance", "Dynamic")]
        [PropertyOrder(101)]
        public List<SymbolInstance> dynamicSymbols = new List<SymbolInstance>();

        protected void Start()
        {
            if (startShuffle == StartAction.StartBehaviour)
                Shuffle();
        }

        public override void Initialize()
        {
            int columnCount_ = columnCount;
            int rowCount_ = rowCount;
            
            dynamicStrips.Clear();
            for (int column = 0; column < columnCount_; ++column)
            {
                var dynamicStrip = new DynamicSymbolStrip(strips[column], index.value + yMin, rowCount_);
                dynamicStrips.Add(dynamicStrip);
            }

            ClearSymbolInstances();
            dynamicSymbols = new List<SymbolInstance>(new SymbolInstance[columnCount_ * rowCount_]);
            for (int row = 0; row < rowCount_; ++row)
            {
                for (int column = 0; column < columnCount_; ++column)
                {
                    if (dynamicSymbols[row * columnCount_ + column])
                        continue;

                    var dynamicStrip = dynamicStrips[column];
                    var symbol = dynamicStrip.value[row];
                    var symbolInstance = SymbolInstancePool.GetSymbolInstance(prefabs[symbol.value]);
                    symbolInstance.rectTransform.SetParent(content, false);
                    symbolInstance.rectTransform.SetAsLastSibling();

                    symbolInstance.rectTransform.anchoredPosition = positionStrategy.GetPosition(
                        columnCount_, rowCount_, column, row, symbol.columnCount, symbol.rowCount, symbol.columnOffset, symbol.rowOffset);
                    
                    symbolInstance.symbol = symbol;
                    symbolInstance.Initialize();
                    symbolInstance.gameObject.SetActive(true);

                    dynamicSymbols[row * columnCount_ + column] = symbolInstance;
                    AdjustDynamicSymbols(columnCount_, rowCount_, symbolInstance, column, row);
                }
            }
            UpdateSortingOrder();
        }

        protected void AdjustDynamicSymbols(int columnCount_, int rowCount_, SymbolInstance symbolInstance, int x_, int y_)
        {
            int rowOffsetCount = Mathf.Min(symbolInstance.symbol.rowCount - symbolInstance.symbol.rowOffset, rowCount_ - y_);
            for (int rowOffset = 0; rowOffset < rowOffsetCount; ++rowOffset)
            {
                for (int columnOffset = 0, columnOffsetCount = symbolInstance.symbol.columnCount - symbolInstance.symbol.columnOffset; columnOffset < columnOffsetCount; ++columnOffset)
                {
                    dynamicSymbols[(y_ + rowOffset) * columnCount_ + (x_ + columnOffset)] = symbolInstance;
                }
            }
        }

        public override void Shuffle()
        {
            if (strips.Count == 0)
                return;
                
            index.value = strips[0].randomIndex;
            Initialize();
        }

        public override void Clear()
        {
            dynamicStrips.Clear();
            ClearSymbolInstances();
        }

        public override Vector3 GetReelPosition()
        {
            return content.position;
        }

        public override SymbolInstance GetSymbol(int x, int y)
        {
            return dynamicSymbols[(x - _xMin) * columnCount + (y - _yMin)];
        }

        public override void SetSymbol(int x, int y, SymbolInstance symbolInstance)
        {
            // TODO
        }

        public override void AddSymbol(int x, int y, SymbolInstance symbolInstance)
        {
            GetSymbol(x, y).AddSymbol(symbolInstance);
        }

        public override Vector3 GetSymbolPosition(int x, int y)
        {
            var position = positionStrategy.GetPosition(columnCount, rowCount, x - _xMin, y - _yMin, 1, 1, 0, 0);
            return content.TransformPoint(new Vector3(position.x, position.y, 0f));
        }

        public override SymbolInstance GetPatchingSymbol(int x, int y)
        {
            var dynamicStrip = dynamicStrips[x - _xMin];
            if (dynamicStrip.frontPatch.distance > 0)
            {
                y -= rowCount - dynamicStrip.frontPatch.distance;
                if (HasSymbol(x, y)) return GetSymbol(x, y);
            }
            else if (dynamicStrip.backPatch.distance > 0)
            {
                y += rowCount - dynamicStrip.backPatch.distance;
                if (HasSymbol(x, y)) return GetSymbol(x, y);
            }
            return null;
        }

        public void PushFront()
        {
            foreach (var dynamicStrip in dynamicStrips)
            {
                dynamicStrip.PushFront();
            }

            int columnCount_ = columnCount;
            int rowCount_ = dynamicStrips[0].Count;
            
            dynamicSymbols.InsertRange(0, new SymbolInstance[columnCount_]);
            for (int column = 0; column < columnCount_; ++column)
            {
                if (dynamicSymbols[column])
                    continue;

                var dynamicStrip = dynamicStrips[column];
                var symbol = dynamicStrip.value[0];

                SymbolInstance symbolInstance = null;
                var frontSymbolInstance = dynamicSymbols[columnCount_ + column];
                var frontSymbol = dynamicStrip.value[1];
                if (frontSymbol.rowCount > 1 && frontSymbol.rowOffset > 0)
                {
                    symbolInstance = frontSymbolInstance;
                    symbolInstance.symbol = symbol;
                }
                else
                {
                    symbolInstance = SymbolInstancePool.GetSymbolInstance(prefabs[symbol.value]);
                    symbolInstance.rectTransform.SetParent(content, false);

                    var newPosition = positionStrategy.GetPosition(
                        columnCount_, frontSymbol.rowCount + 2,
                        column, 0, symbol.columnCount, symbol.rowCount, symbol.columnOffset, symbol.rowOffset);
                    newPosition.y += frontSymbolInstance.rectTransform.anchoredPosition.y;
                    symbolInstance.rectTransform.anchoredPosition = newPosition;

                    symbolInstance.symbol = symbol;
                    symbolInstance.Initialize();
                    symbolInstance.gameObject.SetActive(true);
                }
                symbolInstance.rectTransform.SetSiblingIndex(column);

                dynamicSymbols[column] = symbolInstance;
                AdjustDynamicSymbols(columnCount_, rowCount_, symbolInstance, column, 0);
            }
        }

        public void PushFront(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                PushFront();
            }
        }

        public void PopFront()
        {
            foreach (var dynamicStrip in dynamicStrips)
            {
                dynamicStrip.PopFront();
            }

            int columnCount_ = columnCount;

            SymbolInstance lastSymbolInstance = null;
            for (int column = 0; column < columnCount_; ++column)
            {
                SymbolInstance symbolInstance0 = dynamicSymbols[column];
                SymbolInstance symbolInstance1 = dynamicSymbols[columnCount_ + column];
                if (symbolInstance0 == lastSymbolInstance)
                    continue;

                if (symbolInstance0 == symbolInstance1)
                {
                    var dynamicStrip = dynamicStrips[column];
                    var symbol = dynamicStrip.value[0];
                    symbolInstance1.symbol = symbol;
                    lastSymbolInstance = symbolInstance1;
                }
                else
                {
                    symbolInstance0.ReturnToPool();
                }
            }
            dynamicSymbols.RemoveRange(0, columnCount);
        }

        public void PopFront(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                PopFront();
            }
        }

        public void PushBack()
        {
            foreach (var dynamicStrip in dynamicStrips)
            {
                dynamicStrip.PushBack();
            }

            int columnCount_ = columnCount;
            int rowCount_ = dynamicStrips[0].Count;

            dynamicSymbols.AddRange(new SymbolInstance[columnCount_]);
            for (int column = 0; column < columnCount_; ++column)
            {
                if (dynamicSymbols[(rowCount_ - 1) * columnCount_ + column])
                    continue;

                var dynamicStrip = dynamicStrips[column];
                var symbol = dynamicStrip.value[rowCount_ - 1];

                SymbolInstance symbolInstance = null;
                var backSymbolInstance = dynamicSymbols[(rowCount_ - 2) * columnCount_ + column];
                var backSymbol = dynamicStrip.value[rowCount_ - 2];
                if (backSymbol.rowCount > 1 && (backSymbol.rowCount - backSymbol.rowOffset) > 1)
                {
                    symbolInstance = backSymbolInstance;
                    symbolInstance.symbol = symbol;
                }
                else
                {
                    symbolInstance = SymbolInstancePool.GetSymbolInstance(prefabs[symbol.value]);
                    symbolInstance.rectTransform.SetParent(content, false);
                    
                    var newPosition = positionStrategy.GetPosition(
                        columnCount_, -backSymbol.rowCount,
                        column, 0, symbol.columnCount, symbol.rowCount, symbol.columnOffset, symbol.rowOffset);
                    newPosition.y += backSymbolInstance.rectTransform.anchoredPosition.y;
                    symbolInstance.rectTransform.anchoredPosition = newPosition;

                    symbolInstance.symbol = symbol;
                    symbolInstance.Initialize();
                    symbolInstance.gameObject.SetActive(true);
                }
                symbolInstance.rectTransform.SetSiblingIndex((rowCount_ - 1) * columnCount_ + column);

                dynamicSymbols[(rowCount_ - 1) * columnCount_ + column] = symbolInstance;
                AdjustDynamicSymbols(columnCount_, rowCount_, symbolInstance, column, rowCount_ - 1);
            }
        }

        public void PushBack(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                PushBack();
            }
        }

        public void PopBack()
        {
            foreach (var dynamicStrip in dynamicStrips)
            {
                dynamicStrip.PopBack();
            }
            
            int columnCount_ = columnCount;
            int rowCount_ = dynamicStrips[0].Count;

            SymbolInstance lastSymbolInstance = null;
            for (int column = 0; column < columnCount_; ++column)
            {
                SymbolInstance symbolInstance0 = dynamicSymbols[rowCount_ * columnCount_ + column];
                SymbolInstance symbolInstance1 = dynamicSymbols[(rowCount_ - 1) * columnCount_ + column];
                if (symbolInstance0 == lastSymbolInstance)
                    continue;

                if (symbolInstance0 == symbolInstance1)
                {
                    var dynamicStrip = dynamicStrips[column];
                    var symbol = dynamicStrip.value[rowCount_ - 1];
                    symbolInstance1.symbol = symbol;
                    lastSymbolInstance = symbolInstance1;
                }
                else
                {
                    symbolInstance0.ReturnToPool();
                }
            }
            dynamicSymbols.RemoveRange(rowCount_ * columnCount_, columnCount_);
        }

        public void PopBack(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                PopBack();
            }
        }

        public override void ApplyFrontPatch()
        {
            SetFrontPatch(index.value);
        }

        public override void ApplyBackPatch()
        {
            SetBackPatch(index.value);
        }

        public void SetFrontPatch(int dstIndex)
        {
            dstIndex += _yMax - 1;

            int columnCount_ = columnCount;
            int rowCount_ = rowCount;

            Vector2 symbolSize = positionStrategy.GetSize();
            Vector2 topPosition = positionStrategy.GetPosition(
                columnCount_, rowCount_, 0, 0, 1, 1, 0, 0);

            var frontSymbolInstance = dynamicSymbols[0];
            var frontSymbol = frontSymbolInstance.symbol;
            var frontPosition = frontSymbolInstance.rectTransform.anchoredPosition + 
                positionStrategy.GetLocalPosition(frontSymbol.columnCount, frontSymbol.rowCount, frontSymbol.columnOffset, frontSymbol.rowOffset);
            float topOffset = frontPosition.y + (position.y - renderPosition.y);
            topOffset = topPosition.y - topOffset;

            int srcIndex = frontSymbol.index;
            int srcPatchCount, dstPatchCount;
            patchStrategy.CalcFrontPatch(strips, srcIndex, dstIndex, out srcPatchCount, out dstPatchCount);

            desiredPosition = position;
            desiredPosition.y += -symbolSize.y * rowCount_;
            desiredPosition.y += topOffset;
            desiredPosition.y += -symbolSize.y * srcPatchCount;
            desiredPosition.y += -symbolSize.y * dstPatchCount;

            int backIndex = srcIndex - srcPatchCount;
            int frontIndex = dstIndex + dstPatchCount;
            foreach (var dynamicStrip in dynamicStrips)
            {
                dynamicStrip.SetFrontPatch(frontIndex, backIndex);
            }
        }

        public void SetBackPatch(int dstIndex)
        {
            dstIndex += _yMin;

            int columnCount_ = columnCount;
            int rowCount_ = rowCount;

            Vector2 symbolSize = positionStrategy.GetSize();
            Vector2 bottomPosition = positionStrategy.GetPosition(
                columnCount_, rowCount_, 0, rowCount_ - 1, 1, 1, 0, 0);

            var backSymbolInstance = dynamicSymbols[dynamicSymbols.Count - 1];
            var backSymbol = backSymbolInstance.symbol;
            var backPosition = backSymbolInstance.rectTransform.anchoredPosition + 
                positionStrategy.GetLocalPosition(backSymbol.columnCount, backSymbol.rowCount, backSymbol.columnOffset, backSymbol.rowOffset);
            float bottomOffset = backPosition.y + (position.y - renderPosition.y);
            bottomOffset = bottomPosition.y - bottomOffset;

            int srcIndex = backSymbol.index;
            int srcPatchCount, dstPatchCount;
            patchStrategy.CalcBackPatch(strips, srcIndex, dstIndex, out srcPatchCount, out dstPatchCount);

            desiredPosition = position;
            desiredPosition.y += symbolSize.y * rowCount_;
            desiredPosition.y += -bottomOffset;
            desiredPosition.y += symbolSize.y * srcPatchCount;
            desiredPosition.y += symbolSize.y * dstPatchCount;

            int frontIndex = srcIndex + srcPatchCount;
            int backIndex = dstIndex - dstPatchCount;
            foreach (var dynamicStrip in dynamicStrips)
            {
                dynamicStrip.SetBackPatch(frontIndex, backIndex);
            }
        }

        public override void SendSymbolEvent(string eventName)
        {
            for (int i = 0, count = content.childCount; i < count; ++i)
            {
                content.GetChild(i).GetComponent<SymbolInstance>().SendEvent(eventName);
            }
        }

        public void ClearSymbolInstances()
        {
            for (int i = 0, count = content.childCount; i < count; ++i)
            {
                content.GetChild(0).GetComponent<SymbolInstance>().ReturnToPool();
            }
            dynamicSymbols.Clear();
        }

        protected void UpdateBounds()
        {
            if (dynamicStrips.Count == 0)
                return;

            var contentBounds = new Bounds(content.rect.center, content.rect.size);

            int columnCount_ = columnCount;
            int rowCount_ = dynamicStrips[0].Count;
            int frontOffset = 0;
            int backOffset = 0;
            for (int column = 0; column < columnCount_; ++column)
            {
                var dynamicStrip = dynamicStrips[column];
                var frontSymbol = dynamicStrip.value[0];
                var backSymbol = dynamicStrip.value[rowCount_ - 1];
                var frontSymbolInstance = dynamicSymbols[column];
                var backSymbolInstance = dynamicSymbols[(rowCount_ - 1) * columnCount_ + column];

                int boundsOffset = GetBoundsOffset(RectTransform.Edge.Top, ref contentBounds, 
                    frontSymbolInstance.rectTransform.anchoredPosition + 
                    positionStrategy.GetLocalPosition(frontSymbol.columnCount, frontSymbol.rowCount, frontSymbol.columnOffset, frontSymbol.rowOffset));
                if (boundsOffset > 0) frontOffset = Mathf.Max(frontOffset, boundsOffset);
                else if (boundsOffset < 0) frontOffset = Mathf.Min(frontOffset, boundsOffset);
                
                boundsOffset = GetBoundsOffset(RectTransform.Edge.Bottom, ref contentBounds, 
                    backSymbolInstance.rectTransform.anchoredPosition + 
                    positionStrategy.GetLocalPosition(backSymbol.columnCount, backSymbol.rowCount, backSymbol.columnOffset, backSymbol.rowOffset));
                if (boundsOffset > 0) backOffset = Mathf.Max(backOffset, boundsOffset);
                else if (boundsOffset < 0) backOffset = Mathf.Min(backOffset, boundsOffset);
            }
            
            if (frontOffset > 0)
                PushFront(frontOffset);
            if (backOffset > 0)
                PushBack(backOffset);
            
            if (frontOffset < 0)
                PopFront(-frontOffset);
            if (backOffset < 0)
                PopBack(-backOffset);

            if (frontOffset != 0 || backOffset != 0)
                UpdateSortingOrder();
        }

        protected int GetBoundsOffset(RectTransform.Edge edge, ref Bounds contentBounds, Vector2 position)
        {
            var extents = positionStrategy.GetExtents();

            int offset = 0;
            switch (edge)
            {
            case RectTransform.Edge.Top:
                offset = positionStrategy.Repeat(RectTransform.Axis.Vertical, (position.y - extents.y) - contentBounds.max.y);
                if (offset > 0) return -offset;
                offset = positionStrategy.Repeat(RectTransform.Axis.Vertical, contentBounds.max.y - (position.y + extents.y));
                if (offset > 0) return offset;
                break;
            case RectTransform.Edge.Bottom:
                offset = positionStrategy.Repeat(RectTransform.Axis.Vertical, contentBounds.min.y - (position.y + extents.y));
                if (offset > 0) return -offset;
                offset = positionStrategy.Repeat(RectTransform.Axis.Vertical, (position.y - extents.y) - contentBounds.min.y);
                if (offset > 0) return offset;
                break;
            }
            return 0;
        }

        protected void UpdateSortingOrder()
        {
            int columnCount_ = columnCount;
            int rowCount_ = dynamicStrips[0].Count;

            for (int row = rowCount_ - 1; row >= 0; --row)
            {
                for (int column = columnCount_ - 1; column >= 0; --column)
                {
                    var symbolInstance = dynamicSymbols[row * columnCount_ + column];
                    var dynamicSymbol = dynamicStrips[column].value[row];

                    symbolInstance.x = _xMin + column - dynamicSymbol.columnOffset;
                    symbolInstance.y = _yMin + row - dynamicSymbol.rowOffset;
                    symbolInstance.sortingOrder = sortingOrderStrategy.GetSortingOrder(_xMin + column, _yMin + row, _z);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (content == null) content = GetComponent<RectTransform>();
        }
#endif
    }
}