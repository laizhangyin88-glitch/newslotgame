using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [RequireComponent(typeof(RectTransform))]
    public class BaseSlotMachine : MonoBehaviour, ISlotMachine, IGrid
    {
    	public ObjectPool reelPool;
    	public ObjectPool symbolPool;

        public int BeginColumn { get { return beginColumn; } set { beginColumn = value; } }
        public int BeginRow { get { return beginRow; } set { beginRow = value; } }
        public int EndColumn { get { return endColumn; } set { endColumn = value; } }
        public int EndRow { get { return endRow; } set { endRow = value; } }
        public int ColumnCount { get { return endColumn - beginColumn; } }
        public int RowCount    { get { return endRow - beginRow; } }
        public int ExpandTopCount { get { return expandTopCount; } set { expandTopCount = value; } }

    	public int beginColumn;
    	public int beginRow;
    	public int endColumn;
    	public int endRow;
    	public int expandTopCount;

        public int slotIndex;
        public bool ignoreOverlay;

    	public bool staticReel;

    	public RectTransform rectTransform;
    	public RectTransform reelsTransform;
    	public ReelLayoutGroup layoutGroup;
    	public SlotMachineMovement movement;
    	public BaseSlotMachineOverlay overlay;

    	public Vector2 cellSize { get { return layoutGroup.cellSize; } }
    	public Vector4 spacing  { get { return layoutGroup.spacing; } }

        public virtual void Shuffle()
        {
            for (int i = 0; i < reels.Count; ++i)
            {
                GetReel(i).Shuffle();
            }
            InitializeSymbols();
        }

        public virtual void Shuffle(List<int> indices)
        {
            for (int i = 0; i < reels.Count; ++i)
            {
                GetReel(i).Shuffle(indices[i]);
            }
            InitializeSymbols();
        }

        public virtual void SetStripIndices(List<int> indices, int offset)
        {
            for (int i = 0; i < reels.Count; ++i)
            {
                GetReel(i).nextIndex = indices[i] + offset;
            }
        }

    	public BaseSlotMachine Save()
    	{
    		var go = new GameObject();
    		go.name = "Slot Machine";
    		var snapshot = go.AddComponent<BaseSlotMachine>();

    		var ro = new GameObject();
            ro.name = "Reels";
            ro.transform.SetParent(go.transform, false);
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			snapshot.CreateReel(reels[i]).transform.SetParent(ro.transform, false);
    		}

    		if (HasOverlaySymbols())
    		{
    			snapshot.CreateOverlay();

                count = overlay.symbols.Count;
    			for (int i = 0; i < count; ++i)
    			{
    				snapshot.overlay.AddSymbol(overlay.symbols[i]);
    			}
    		}

    		return snapshot;
    	}

    	public void Load(BaseSlotMachine snapshot)
    	{
    		if (staticReel)
    			ClearSymbols();
    		else
    			Clear();

    		var reels = snapshot.reels;
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			CreateReel(reels[i]);
    		}

    		if (HasOverlaySymbols())
    			overlay.Clear();

    		if (snapshot.HasOverlaySymbols())
    		{
                count = snapshot.overlay.symbols.Count;
    			for (int i = 0; i < count; ++i)
    			{
    				overlay.AddSymbol(snapshot.overlay.symbols[i]);
    			}
    		}
    	}

    	public void Clean()
    	{
    		if (staticReel)
    			ClearSymbols();
    		else
    			Clear();

    		reelPool.ClearPool();
    		symbolPool.ClearPool();
    	}

    	public void Clear()
    	{
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			GetReel(i).Clear();
    		}

    		reels.Clear();
    	}

    	public void ClearSymbols()
    	{
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			GetReel(i).ClearSymbols();
    		}
    	}

    	public List<BaseReel> reels = new List<BaseReel>();
    	public List<BaseReel> GetReels()
    	{
    		return reels;
    	}

    	public bool HasReel(int reelIndex)
    	{
    		return reelIndex < reels.Count;
    	}

    	public BaseReel GetReel(int reelIndex)
    	{
    		return reels[reelIndex];
    	}

        public BaseReel FindReel(int column, int row)
        {
            int count = reels.Count;
            for (int i = 0; i < count; ++i)
            {
                if (reels[i].ContainsSymbol(column, row))
                    return reels[i];
            }
            return null;
        }

    	public virtual BaseReel CreateReel()
    	{
    		var go = gameObject.AddChild("Reel");
    		var reel = go.AddComponent<BaseReel>();
    		reel.rectTransform = reel.symbolsTransform = reel.GetComponent<RectTransform>();
    		return reel;
    	}

    	public virtual BaseReel CreateReel(BaseReel src)
    	{
    		if (staticReel)
    		{
    			var dst = GetReel(src.reelIndex);
    			dst.Initialize(this, src);
    			return dst;
    		}
    		else
    		{
    			var dst = CreateReel();
    			dst.transform.SetParent(reelsTransform, false);
    			dst.Initialize(this, src);
    			reels.Add(dst);
    			return dst;
    		}
    	}

    	public BaseReel CreateReel(int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount)
    	{
    		var reel = CreateReel();
    		reel.transform.SetParent(reelsTransform, false);
    		reel.Initialize(this, reels.Count, beginColumn, beginRow, endColumn, endRow, expandTopCount);
    		reels.Add(reel);
    		return reel;
    	}

    	public BaseReel CreateReel(int reelIndex, int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount)
    	{
    		var reel = GetReel(reelIndex);
    		reel.Initialize(this, reelIndex, beginColumn, beginRow, endColumn, endRow, expandTopCount);
    		return reel;
    	}

    	public BaseReel InsertReel(int reelIndex, int beginColumn, int beginRow, int endColumn, int endRow, int expandTopCount)
    	{
    		var reel = CreateReel();
    		reel.transform.SetParent(reelsTransform, false);
    		reel.transform.SetSiblingIndex(reelIndex);
    		reel.Initialize(this, reelIndex, beginColumn, beginRow, endColumn, endRow, expandTopCount);
    		reels.Insert(reelIndex, reel);
    		return reel;
    	}

    	public void MergeReel(int begin, int end)
    	{
    		var beginReel = GetReel(begin);
    		for (int i = begin + 1; i < end; ++i)
    		{
    			beginReel.Merge(GetReel(i));
    		}
    		reels.RemoveRange(begin + 1, end - begin - 1);

    		UpdateReelsIndex();
    	}

    	public void SplitPerSymbolReel(int index)
    	{
    		var src = GetReel(index);
    		reels.RemoveAt(index);

    		for (int column = src.beginColumn; column < src.endColumn; ++column)
    		{
    			for (int row = src.beginRow; row < src.endRow; ++row)
    			{
    				var dst = InsertReel(index++, column, row, column + 1, row + 1, 0);
    				dst.CopySymbols(src);
    			}
    		}
    		src.Clear();

    		UpdateReelsIndex();
    	}

    	protected void UpdateReelsIndex()
    	{
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			reels[i].reelIndex = i;
    		}
    	}

        public void InitializeSymbols()
        {
            Visit((sb) => { sb.Initialize(); });
            for (int i = 0; i < reels.Count; ++i)
            {
                GetReel(i).PushBackSymbols(0);
            }
        }

    	public virtual BaseSymbol CreateSymbol()
    	{
    		var go = gameObject.AddChild("Symbol");
    		var symbol = go.AddComponent<BaseSymbol>();
    		return symbol;
    	}

    	public BaseSymbol GetSymbol(int reelIndex, int row)
    	{
    		return GetReel(reelIndex).GetSymbol(reelIndex, row);
    	}

    	public BaseSymbol GetPivotSymbol(int reelIndex, int row)
    	{
    		return GetPivotSymbol(GetSymbol(reelIndex, row));
    	}

    	public BaseSymbol GetPivotSymbol(BaseSymbol symbol)
    	{
    		int hashCode = symbol.GetHashCode();
    		if (ignoreOverlay || !HasOverlaySymbol(hashCode))
            {
                var link = symbol.symbolInfo.link;
                if (!link.isPivot)
                    symbol = GetSymbol(symbol.column + link.columnOffset, symbol.row + link.rowOffset);
            }
            else
            {
                symbol = GetOverlaySymbol(hashCode);

                var link = symbol.symbolInfo.link;
                if (!link.isPivot)
                {
                    hashCode = Cell.GetHashCode(symbol.column + link.columnOffset, symbol.row + link.rowOffset);
                    symbol = GetOverlaySymbol(hashCode);
                }
            }

    		return symbol;
    	}

    	public List<BaseSymbol> GetSymbols()
    	{
    		List<BaseSymbol> temp = new List<BaseSymbol>();
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			var reel = reels[i];
                int symbolCount = reel.symbols.Count;
    			for (int j = 0; j < symbolCount; ++j)
    			{
    				temp.Add(reel.symbols[j]);
    			}
    		}
    		return temp;
    	}

    	public void Visit(Action<BaseSymbol> visitor)
    	{
            int count = reels.Count;
    		for (int i = 0; i < count; ++i)
    		{
    			var reel = reels[i];
    			reel.Visit(visitor);
    		}
    	}

    	protected Dictionary<int, BaseSymbol> overlaySymbols = new Dictionary<int, BaseSymbol>();

    	public virtual BaseSlotMachineOverlay CreateOverlay()
    	{
    		var ov = gameObject.AddComponent<BaseSlotMachineOverlay>();
    		ov.rectTransform = ov.symbolsTransform = ov.GetComponent<RectTransform>();
    		ov.slotMachine = this;
    		overlay = ov;
    		return ov;
    	}

    	public void ClearOverlaySymbols()
    	{
    		overlaySymbols.Clear();
    	}

    	public void AddOverlaySymbol(BaseSymbol symbol)
    	{
    		overlaySymbols[symbol.GetHashCode()] = symbol;
    	}

        public void RemoveOverlaySymbol(int index)
        {
            overlaySymbols.Remove(index);
        }

    	public BaseSymbol GetOverlaySymbol(int index)
    	{
    		BaseSymbol symbol;
    		if (overlaySymbols.TryGetValue(index, out symbol))
    			return symbol;

    		return null;
    	}

    	public bool HasOverlaySymbols()
    	{
    		return overlaySymbols.Count > 0;
    	}

    	public bool HasOverlaySymbol(int index)
    	{
    		return overlaySymbols.ContainsKey(index);
    	}

    	public void Skip()
    	{
    		Visit((sb) =>
    		{
    			GetPivotSymbol(sb).Skip();
    		});
    	}

    	protected virtual Vector4 GetClipRange() { return Vector4.zero; }

    	public virtual void SetDirty() {}
    	protected virtual void OnChangedSlotMachine() {}
    	protected virtual void OnChangedRect() {}
    	protected virtual void OnExpandedTop() {}
    	protected virtual void OnContractedTop() {}
    }
}
