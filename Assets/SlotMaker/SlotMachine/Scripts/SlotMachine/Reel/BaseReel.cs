using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    [RequireComponent(typeof(RectTransform))]
    public class BaseReel : MonoBehaviour, IReel
    {
        public int BeginColumn
        { get { return beginColumn; } set { beginColumn = value; } }

        public int BeginRow
        { get { return beginRow; } set { beginRow = value; } }

        public int EndColumn
        { get { return endColumn; } set { endColumn = value; } }

        public int EndRow
        { get { return endRow; } set { endRow = value; } }

        public int ColumnCount
        { get { return endColumn - beginColumn; } }

        public int RowCount
        { get { return endRow - beginRow; } }

        public int ExpandTopCount
        { get { return expandTopCount; } set { expandTopCount = value; } }

        public int beginColumn;
        public int beginRow;
        public int endColumn;
        public int endRow;
        public int expandTopCount;
        public int topBuffer = 1;
        public int bottomBuffer = 1;

        public int index;
        public int nextIndex;
        public int reelIndex;

        public int frontIndex
        {
            get { return index; }
        }

        public int backIndex
        {
            get { return strip.CalcIndex(index + symbols.Count - 1); }
        }

        public RectTransform rectTransform;
        public RectTransform symbolsTransform;
        public ReelMovement movement;
        public BaseSlotMachine slotMachine;

        public BaseReelStrip _strip;

        public BaseReelStrip strip
        {
            get {
                if (_strip != null)
                {
                    Debug.LogError("i am here _strip");
                }

                return _strip ?? GlobalReelStrips.Instance.GetReelStrips().GetReelStrip(reelIndex);

            }
            set { _strip = value; }
        }

        public SymbolAssets _symbolAssets;

        public SymbolAssets symbolAssets
        {
            get { return _symbolAssets ?? GlobalSymbolAssets.Instance; }
            set { _symbolAssets = value; }
        }

        public class Patch
        {
            public int srcIndex = -1;
            public int dstIndex = -1;

            public void SetPatch(int srcIndex, int dstIndex)
            {
                this.srcIndex = srcIndex;
                this.dstIndex = dstIndex;
            }

            public void Reset()
            {
                SetPatch(-1, -1);
            }

            public int GetDstIndex()
            { return dstIndex; }

            public bool CheckSrcIndex(int index)
            { return srcIndex == index; }
        }

        public Patch frontPatch = new Patch();
        public Patch backPatch = new Patch();

        public virtual int GetNextFrontIndex()
        {
            if (frontPatch.CheckSrcIndex(frontIndex))
            {
                int patchedIndex = frontPatch.GetDstIndex();
                frontPatch.Reset();
                return strip.CalcIndex(patchedIndex);
            }

            return strip.CalcIndex(index - 1);
        }

        public virtual int GetNextBackIndex()
        {
            if (backPatch.CheckSrcIndex(backIndex))
            {
                int patchedIndex = backPatch.GetDstIndex();
                backPatch.Reset();
                return strip.CalcIndex(patchedIndex);
            }

            return strip.CalcIndex(backIndex + 1);
        }

        public virtual void Shuffle()
        {
            index = strip.GetRandomIndex();
        }

        public virtual void Shuffle(int reelIndex)
        {
            index = strip.CalcIndex(reelIndex);
        }

        public virtual void SwapIndex()
        {
            if (nextIndex >= 0)
            {
                index = nextIndex;
                nextIndex = -1;
            }
        }

        // TODO: Will be used when polishing `UpdateSlotMachineIndices.cs`
        public virtual void SetReelStripIndex(int index)
        {
            nextIndex = strip.CalcIndex(index + beginRow - topBuffer);
        }

        public Vector2 cellSize
        { get { return slotMachine.cellSize; } }

        public Vector4 spacing
        { get { return slotMachine.spacing; } }

        public List<BaseSymbol> symbols = new List<BaseSymbol>();

        public List<BaseSymbol> GetSymbols()
        {
            return symbols;
        }

        [Button]
        void test_ShowSymbol()
        {
            string res = "";
            foreach (BaseSymbol item in symbols)
            {
                res += $"{item.symbolInfo.symbol},";
            }
            Debug.Log($"【Test】:{res}");
        }
        public virtual BaseSymbol GetSymbol(int column, int row)
        {
            return symbols[row - beginRow + topBuffer];
        }

        public virtual void Clear()
        {
            OnDestroyedReel();
        }

        public virtual void ClearSymbols()
        { }

        public virtual void Initialize(BaseSlotMachine slotMachine, BaseReel src)
        {
            Debug.LogError("初始化..............");
            this.slotMachine = slotMachine;
            this.beginColumn = src.beginColumn;
            this.beginRow = src.beginRow;
            this.endColumn = src.endColumn;
            this.endRow = src.endRow;
            this.expandTopCount = src.expandTopCount;
            this.reelIndex = src.reelIndex;
            this.index = src.index;
            this.nextIndex = src.nextIndex;
            this.topBuffer = src.topBuffer;

            CalcLayoutOffset();

            int count = src.symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                var symbol = slotMachine.CreateSymbol();
                symbol.transform.SetParent(symbolsTransform, false);
                symbol.Initialize(this, src.symbols[i]);
                UpdateSymbolTransform(symbol);
                symbols.Add(symbol);
            }

            OnCreatedReel();
            OnChangedRect();
            OnChangedReel();
        }

        public virtual void Initialize(BaseSlotMachine slotMachine, int reelIndex, int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount)
        {
            this.slotMachine = slotMachine;
            this.beginColumn = beginColumn;
            this.beginRow = beginRow;
            this.endColumn = endColumn;
            this.endRow = endRow;
            this.expandTopCount = expandTopCount;
            this.reelIndex = reelIndex;

            CalcLayoutOffset();

            int physicalEndRow = endRow + bottomBuffer;
            for (int column = beginColumn; column < endColumn; ++column)
            {
                for (int row = beginRow - topBuffer; row < physicalEndRow; ++row)
                {
                    var symbol = slotMachine.CreateSymbol();
                    symbol.transform.SetParent(symbolsTransform, false);
                    symbol.Initialize(this, column, row);
                    UpdateSymbolTransform(symbol);
                    symbols.Add(symbol);
                }
            }

            OnCreatedReel();
            OnChangedRect();
            OnChangedReel();
        }

        public virtual void CopySymbols(BaseReel src)
        {
            int physicalEndRow = endRow + bottomBuffer;
            for (int column = beginColumn; column < endColumn; ++column)
            {
                for (int row = beginRow - topBuffer; row < physicalEndRow; ++row)
                {
                    GetSymbol(column, row).Initialize(this, src.GetSymbol(column, row));
                }
            }
        }

        public virtual bool ContainsSymbol(int column, int row)
        {
            return (column >= BeginColumn && column < EndColumn) && (row >= BeginRow && row < EndRow);
        }

        public virtual void ClearExpandTop()
        {
            if (expandTopCount == 0)
                return;

            beginRow += expandTopCount;
            expandTopCount = 0;

            CalcLayoutOffset();

            OnContractedTop();
            OnChangedReel();
        }

        public virtual void ExpandTop(int count)
        {
            expandTopCount += count;
            beginRow -= count;

            CalcLayoutOffset();

            PushFrontSymbols(count);

            OnExpandedTop();
            OnChangedReel();
        }

        public virtual void ContractTop(int count)
        {
            expandTopCount -= count;
            beginRow += count;

            CalcLayoutOffset();

            OnContractedTop();
            OnChangedReel();
        }

        public virtual void Insert(int row, int count, SymbolInfo newSymbol)
        {
            expandTopCount += count;
            beginRow -= count;

            CalcLayoutOffset();

            InsertSymbols(row, count, newSymbol);
            UpdateSymbolsTransform();

            OnInserted(row, count);
            OnExpandedTop();
            OnChangedReel();
        }

        public virtual void Replace(int row, int count, SymbolInfo newSymbol)
        {
            ReplaceSymbols(row, count, newSymbol);

            OnReplaced(row, count);
        }

        public virtual void Remove(int row, int count)
        {
            expandTopCount -= count;
            beginRow += count;

            CalcLayoutOffset();

            RemoveSymbols(row, count);
            UpdateSymbolsTransform();

            OnRemoved(row, count);
            OnContractedTop();
            OnChangedReel();
        }

        public virtual void Merge(BaseReel src)
        {
            if (beginColumn == src.beginColumn)
            {
                RemoveOutBoundSymbols();
                src.RemoveOutBoundSymbols();

                PopBackSymbols(1);
                src.PopFrontSymbols(1);
            }

            endColumn = src.endColumn;
            endRow = src.endRow;

            CalcLayoutOffset();

            int count = src.symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                var symbol = slotMachine.CreateSymbol();
                symbol.transform.SetParent(symbolsTransform, false);
                symbol.Initialize(this, src.symbols[i]);
                symbols.Add(symbol);
            }
            src.Clear();

            UpdateSymbolsTransform();

            OnChangedRect();
            OnChangedReel();
        }

        public virtual void TranslateSymbols(Vector3 translation)
        {
            int count = symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[i];
                var newPosition = symbol.rectTransform.anchoredPosition3D;
                newPosition += translation;
                symbol.rectTransform.anchoredPosition3D = newPosition;
            }
        }

        private int countttt = 0;

        public virtual void PushFrontSymbol()
        {
            index = GetNextFrontIndex();

            var frontSymbol = symbols[0];
            var symbol = slotMachine.CreateSymbol();

            symbol.transform.SetParent(symbolsTransform, false);
            symbol.transform.SetAsFirstSibling();
            symbol.Initialize(this, frontSymbol.column, frontSymbol.row - 1, index);
            UpdateFrontSymbolTransform(symbol);
            symbols.Insert(0, symbol);
        }

        public virtual void PushFrontSymbols(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                PushFrontSymbol();
            }
        }

        public virtual void PopFrontSymbols(int count)
        {
            index = strip.CalcIndex(index + count);

            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[0];
                symbol.Clear();
                symbols.RemoveAt(0);
            }
        }

        public virtual void PushBackSymbol()
        {
            var backSymbol = symbols[symbols.Count - 1];
            var symbol = slotMachine.CreateSymbol();
            int nextBackIndex = GetNextBackIndex();

            symbol.transform.SetParent(symbolsTransform, false);
            symbol.transform.SetAsLastSibling();
            symbol.Initialize(this, backSymbol.column, backSymbol.row + 1, nextBackIndex);
            UpdateBackSymbolTransform(symbol);
            symbols.Add(symbol);

            if (nextBackIndex != backIndex)
                index = strip.CalcIndex(nextBackIndex - (symbols.Count - 1));
        }

        public virtual void PushBackSymbols(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                PushBackSymbol();
            }

            var symbol = symbols[symbols.Count - 1];
            while (symbol.symbolInfo.link.rowOffset != 0)
            {
                PushBackSymbol();
                symbol = symbols[symbols.Count - 1];
            }
        }

        public virtual void PopBackSymbols(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[symbols.Count - 1];

                int limitRow = endRow + bottomBuffer - 1;
                if (symbol.symbolInfo.link.isPivot)
                    limitRow += symbol.symbolInfo.link.rowCount - 1;

                if (symbol.row > limitRow)
                {
                    symbol.Clear();
                    symbols.RemoveAt(symbols.Count - 1);
                }
                else
                {
                    break;
                }
            }
        }

        public virtual void InsertSymbols(int row, int count, SymbolInfo newSymbol)
        {
            int physicalRow = (row - (beginRow + count)) + topBuffer;

            for (int i = 0; i < count; ++i)
            {
                var symbol = slotMachine.CreateSymbol();
                symbol.transform.SetParent(symbolsTransform, false);
                symbol.transform.SetSiblingIndex(physicalRow + i);
                symbol.Initialize(this, newSymbol);
                symbols.Insert(physicalRow + i, symbol);
            }
        }

        public virtual void ReplaceSymbols(int row, int count, SymbolInfo newSymbol)
        {
            for (int i = 0; i < count; ++i)
            {
                var symbol = slotMachine.GetSymbol(beginColumn, row + i);
                symbol.Change(newSymbol);
                symbol.Apply();
            }
        }

        public virtual void RemoveSymbols(int row, int count)
        {
            int physicalRow = (row - (beginRow - count)) + topBuffer;

            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[physicalRow + i];
                symbol.Clear();
            }
            symbols.RemoveRange(physicalRow, count);
        }

        public virtual void RemoveOutBoundSymbols()
        {
            while (true)
            {
                var symbol = symbols[symbols.Count - 1];

                int limitRow = endRow + bottomBuffer - 1; ;
                if (symbol.symbolInfo.link.isPivot)
                    limitRow += symbol.symbolInfo.link.rowCount - 1;

                if (symbol.row > limitRow)
                {
                    symbol.Clear();
                    symbols.RemoveAt(symbols.Count - 1);
                }
                else
                {
                    break;
                }
            }
        }

        public virtual void Play(string animationName)
        {
            OnPlay(animationName);
        }

        public void Visit(Action<BaseSymbol> visitor)
        {
            int count = symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                visitor(symbols[i]);
            }
        }

        public virtual void Skip()
        {
            int count = symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                symbols[i].Skip();
            }
        }

        public void UpdateFrontSymbolsBounds()
        {
            int neededSymbolCount = -GetFrontSymbolDiffCount();
            if (neededSymbolCount >= 0)
                PushFrontSymbols(neededSymbolCount);
            else
                PopFrontSymbols(-neededSymbolCount);

            UpdateSymbolsZOrder();
        }

        public void UpdateBackSymbolsBounds()
        {
            int neededSymbolCount = GetBackSymbolDiffCount();
            if (neededSymbolCount >= 0)
                PushBackSymbols(neededSymbolCount);
            else
                PopBackSymbols(-neededSymbolCount);

            UpdateSymbolsZOrder();
        }

        public void UpdateSymbolsBounds()
        {
            UpdateFrontSymbolsBounds();
            UpdateBackSymbolsBounds();
        }

        public void UpdateSymbolsTransform()
        {
            int count = symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[i];
                symbol.column = beginColumn;
                symbol.row = (beginRow - topBuffer) + i;
                UpdateSymbolTransform(symbol);
            }
        }

        public void UpdateSymbolsZOrder()
        {
            int count = symbols.Count;
            for (int i = 0; i < count; ++i)
            {
                var symbol = symbols[i];
                symbol.column = beginColumn;
                symbol.row = (beginRow - topBuffer) + i;
                UpdateSymbolZPosition(symbol);
            }
        }

        public virtual bool IsOutOfBound()
        { return false; }

        public virtual void CalcLayoutOffset()
        { }

        public virtual Vector3 CalcSymbolPosition(int beginColumn, int beginRow, int column, int row)
        { return Vector3.zero; }

        public virtual Vector3 CalcSymbolPosition(BaseSymbol symbol)
        { return Vector3.zero; }

        protected virtual void UpdateSymbolTransform(BaseSymbol symbol)
        { }

        protected virtual void UpdateFrontSymbolTransform(BaseSymbol symbol)
        { }

        protected virtual void UpdateBackSymbolTransform(BaseSymbol symbol)
        { }

        protected virtual int GetFrontSymbolDiffCount()
        { return 0; }

        protected virtual int GetBackSymbolDiffCount()
        { return 0; }

        protected virtual void UpdateSymbolZPosition(BaseSymbol symbol)
        { }

        protected virtual float GetZPosition(int column, int row)
        { return 0f; }

        protected virtual Vector4 GetClipRange()
        { return Vector4.zero; }

        protected virtual void OnChangedReel()
        { }

        protected virtual void OnCreatedReel()
        { }

        protected virtual void OnDestroyedReel()
        { }

        public virtual void OnChangedRect()
        { }

        protected virtual void OnExpandedTop()
        { }

        protected virtual void OnContractedTop()
        { }

        protected virtual void OnInserted(int row, int count)
        { }

        protected virtual void OnReplaced(int row, int count)
        { }

        protected virtual void OnRemoved(int row, int count)
        { }

        protected virtual void OnPlay(string animationName)
        { }
    }
}
