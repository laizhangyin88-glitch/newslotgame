using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlotMaker
{
	[RequireComponent(typeof(PooledObject))]
	public class Reel : BaseReel
	{
		public Vector4 offset { get; set; }

		public UnityEvent        onSkip;
		public UnityEvent        onChangedReel;
		public UnityEvent 		 onCreatedReel;
		public UnityEvent 		 onDestroyedReel;
		public UnityVector4Event onChangedRect;
		public UnityVector4Event onExpandedTop;
		public UnityVector4Event onContractedTop;

		[Serializable]
		public class InsertReelEvent : UnityEvent<float, int, int> {}
		public InsertReelEvent onInserted;
		public InsertReelEvent onReplaced;
		public InsertReelEvent onRemoved;

	    public UnityStringEvent onPlay;

		private PooledObject _pooledObject;
		protected PooledObject pooledObject { get { return _pooledObject ?? (_pooledObject = GetComponent<PooledObject>()); } }

	    public override void Clear()
		{
			ClearSymbols();
			pooledObject.ReturnToPool();

			base.Clear();
		}

		public override void ClearSymbols()
		{
	        int count = symbols.Count;
			for (int i = 0; i < count; ++i)
			{
				symbols[i].Clear();
			}
			symbols.Clear();
		}

		public override void Skip()
		{
			base.Skip();

			if (onSkip != null)
				onSkip.Invoke();
		}

		public override bool IsOutOfBound()
		{
			return symbols[topBuffer - 1].transform.localPosition.y < offset.y;
		}

		public override void CalcLayoutOffset()
		{
			float width = cellSize.x + spacing.x;
			float height = cellSize.y + spacing.y;

			offset = new Vector4
			(
				width * -((float)(ColumnCount - 1) * 0.5f),
				height * ((float)((RowCount - expandTopCount) - 1) * 0.5f + (float)expandTopCount),
				width,
				height
			);
		}

		public override Vector3 CalcSymbolPosition(int beginColumn, int beginRow, int column, int row)
		{
			float x = offset.x + offset.z * (column - beginColumn);
			float y = offset.y + offset.w * -(row - beginRow);
			float z = GetZPosition(column, row);
			return new Vector3(x, y, z);
		}

		public override Vector3 CalcSymbolPosition(BaseSymbol symbol)
		{
			return CalcSymbolPosition(beginColumn, beginRow, symbol.column, symbol.row);
		}

        protected override int GetFrontSymbolDiffCount()
        {
            float symbolHeight = offset.w;
            float frontBound = 0.5f * (float)(RowCount + expandTopCount + 1) * symbolHeight;
            var frontBoundSymbol = symbols[topBuffer - 1];

            float boundSymbolDisplacement = frontBoundSymbol.transform.localPosition.y - frontBound;
            return (int)(boundSymbolDisplacement / symbolHeight);
        }

        protected override int GetBackSymbolDiffCount()
        {
            float symbolHeight = offset.w;
            float backBound = -0.5f * (float)(RowCount - expandTopCount + 1) * symbolHeight;
            var backBoundSymbol = symbols[(symbols.Count - 1) - (bottomBuffer - 1)];

            float boundSymbolDisplacement = backBoundSymbol.transform.localPosition.y - backBound;
            return (int)(boundSymbolDisplacement / symbolHeight);
        }

		protected override void UpdateSymbolTransform(BaseSymbol symbol)
		{
			var rt = symbol.rectTransform;
			rt.anchoredPosition3D = CalcSymbolPosition(symbol);
			rt.sizeDelta = cellSize;
		}

		protected override void UpdateFrontSymbolTransform(BaseSymbol symbol)
		{
			var src = symbols[0].rectTransform;
			var dst = symbol.rectTransform;
			var newPosition = src.anchoredPosition3D;
			newPosition.y += offset.w;
			newPosition.z = GetZPosition(symbol.column, symbol.row);
			dst.anchoredPosition3D = newPosition;
			dst.sizeDelta = src.sizeDelta;
		}
	    
	    protected override void UpdateBackSymbolTransform(BaseSymbol symbol)
	    {
	        var src = symbols[symbols.Count - 1].rectTransform;
			var dst = symbol.rectTransform;
			var newPosition = src.anchoredPosition3D;
			newPosition.y -= offset.w;
			newPosition.z = GetZPosition(symbol.column, symbol.row);
			dst.anchoredPosition3D = newPosition;
			dst.sizeDelta = src.sizeDelta;
	    }

		protected override void UpdateSymbolZPosition(BaseSymbol symbol)
		{
			var newPosition = symbol.rectTransform.anchoredPosition3D;
			newPosition.z = GetZPosition(symbol.column, symbol.row);
			symbol.rectTransform.anchoredPosition3D = newPosition;
		}

		protected override float GetZPosition(int column, int row)
		{
			return -(spacing.w * column + spacing.z * row);
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

		protected override void OnChangedReel()
		{
			onChangedReel.Invoke();

			slotMachine.SetDirty();
		}

		protected override void OnCreatedReel()
		{
			onCreatedReel.Invoke();
		}

		protected override void OnDestroyedReel()
		{
			onDestroyedReel.Invoke();
		}

		public override void OnChangedRect()
		{
			onChangedRect.Invoke(GetClipRange());
		}
	    
		protected override void OnExpandedTop()
		{
			onExpandedTop.Invoke(GetClipRange());
		}

		protected override void OnContractedTop()
		{
			onContractedTop.Invoke(GetClipRange());
		}

		protected override void OnInserted(int row, int count)
		{
			onInserted.Invoke(cellSize.y + spacing.y, row, count);
		}

		protected override void OnReplaced(int row, int count)
		{
			onReplaced.Invoke(cellSize.y + spacing.y, row, count);
		}

		protected override void OnRemoved(int row, int count)
		{
			onRemoved.Invoke(cellSize.y + spacing.y, row, count);
		}

	    protected override void OnPlay(string animationName)
	    {
	        onPlay.Invoke(animationName);
	    }
	}
}
