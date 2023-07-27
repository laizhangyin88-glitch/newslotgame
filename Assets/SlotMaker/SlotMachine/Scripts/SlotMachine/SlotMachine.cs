using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
    [AddComponentMenu("SlotMaker/Slot Machine/Slot Machine")]
    public class SlotMachine : BaseSlotMachine
    {
    	[Serializable]
    	public class GridEvent : UnityEvent<IGrid> {}
    	public GridEvent onChangedGrid;

    	public UnityVector4Event onChangedRect;
    	public UnityVector4Event onExpandedTop;
    	public UnityVector4Event onContractedTop;

    	public override BaseReel CreateReel()
    	{
    		var po = reelPool.GetObject();
    		return po.GetComponent<Reel>();
    	}

    	public override BaseSymbol CreateSymbol()
    	{
    		var po = symbolPool.GetObject();
    		return po.GetComponent<BaseSymbol>();
    	}

    	private bool dirty;
    	public override void SetDirty() { dirty = true; }

    	private void LateUpdate()
    	{
    		if (dirty)
    		{
    			OnChangedSlotMachine();
    			dirty = false;
    		}
    	}

    	protected override Vector4 GetClipRange()
    	{
    		return new Vector4
    		(
    			0f,
    			(cellSize.y + spacing.y) * -expandTopCount * 0.5f,
    			cellSize.x * ColumnCount + spacing.x * (ColumnCount - 1),
    			cellSize.y * RowCount + spacing.y * (RowCount - 1)
    		);
    	}

    	protected override void OnChangedSlotMachine()
    	{
    		var old = new Grid();
    		GridUtils.Copy(old, this);

    		for (int i = 0; i < reels.Count; ++i)
    		{
    			if (i == 0)
    				GridUtils.Copy(this, reels[i]);
    			else
    				GridUtils.Merge(this, reels[i]);
    		}

    		if ((old.ColumnCount * old.RowCount) == 0)
    		{
    			OnChangedRect();
    		}
    		else
    		{
    			int delta = expandTopCount - old.ExpandTopCount;
    			if (delta > 0)
    				OnExpandedTop();
    			else if (delta < 0)
    				OnContractedTop();
    		}

    		if (onChangedGrid != null)
    			onChangedGrid.Invoke(this);
    	}

    	protected override void OnChangedRect()
    	{
    		if (onChangedRect != null)
    			onChangedRect.Invoke(GetClipRange());
    	}

    	protected override void OnExpandedTop()
    	{
    		if (onExpandedTop != null)
    			onExpandedTop.Invoke(GetClipRange());
    	}

    	protected override void OnContractedTop()
    	{
    		if (onContractedTop != null)
    			onContractedTop.Invoke(GetClipRange());
    	}
    }
}
