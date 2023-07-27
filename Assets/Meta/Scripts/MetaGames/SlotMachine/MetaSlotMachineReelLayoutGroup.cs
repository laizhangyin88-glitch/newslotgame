using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;

namespace BagelCode
{
    public class MetaSlotMachineReelLayoutGroup : ReelLayoutGroup
    {
        public override void SetLayout()
		{
			var size = rectTransform.rect.size;

			rowCounts.Clear();
			int rowCount = 0;
			int minRow = 1000;
			for (int i = 0; i < rectTransform.childCount; ++i)
			{
				var rt = rectTransform.GetChild(i);
				var reel = rt.GetComponent(typeof(BaseReel)) as BaseReel;

				for (int column = reel.beginColumn; column < reel.endColumn; ++column)
				{
					if (column > (rowCounts.Count - 1))
						rowCounts.Add(0);

					rowCounts[column] += reel.RowCount;
					rowCount = Mathf.Max(rowCount, rowCounts[column]);
					minRow = Mathf.Min(minRow, reel.beginRow);
				}
			}
			int columnCount = rowCounts.Count;

			Vector2 alignment = new Vector2(((int)childAlignment % 3) * 0.5f, ((int)childAlignment / 3) * 0.5f);
			Vector2 startOffset = Vector2.zero;
			startOffset.x = -alignment.x * size.x;
			startOffset.y = alignment.y * size.y;
			Vector2 anchor = new Vector2(alignment.x, 1.0f - alignment.y);

			for (int i = 0; i < rectTransform.childCount; ++i)
			{
				var rt = rectTransform.GetChild(i) as RectTransform;
				rt.anchorMin = rt.anchorMax = anchor;

				var reel = rt.GetComponent<MetaSlotMachineReel>();

				float width = CalcScalarWithSpacing(cellSize.x, spacing.x, reel.ColumnCount);
				float height = CalcScalarWithSpacing(cellSize.y, spacing.y, reel.RowCount);
				rt.sizeDelta = new Vector2(width, height);

				// int physicalRow = reel.beginRow - minRow;
				float x = startOffset.x + (cellSize.x + spacing.x) * reel.beginColumn + (width * 0.5f);
				float y = startOffset.y - ((cellSize.y + spacing.y) * (reel.beginRow - minRow)) * (gridBase ? 1f : alignment.y) - (height * 0.5f);
				float z = -(reel.beginColumn * spacing.w);
				rt.anchoredPosition3D = new Vector3(x, y, z);

				reel.OnChangedRect();
			}
			dirty = false;
		}

		private float CalcScalarWithSpacing(float scalar, float spacing, int count)
		{
			return (count > 0) ? (scalar * count + spacing * (count - 1)) : 0f;
		}
	}
}