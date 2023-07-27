using UnityEngine;
using System.Collections;

namespace SlotMaker
{
	public static class ReelClipRangeUtils
	{
		public static void MergeClipRange(BaseSlotMachine slot, int begin, int end)
		{
			var bound = new Grid();
			var reels = slot.reels;
			for (int i = begin; i < end; ++i)
			{
				if (i == 0)
					GridUtils.Copy(bound, reels[i]);
				else 
					GridUtils.Merge(bound, reels[i]);
			}

			var layout = slot.layoutGroup;
			float width = layout.cellSize.x + layout.spacing.x;

			var clipRange = new Vector4
			(
				-width * 0.5f * (bound.ColumnCount - 1) - width, 
				0f,
				layout.cellSize.x * bound.ColumnCount + layout.spacing.x * (bound.ColumnCount - 1),
				layout.cellSize.y * bound.RowCount + layout.spacing.y * (bound.RowCount - 1)
			);

			for (int i = begin; i < end; ++i)
			{
				clipRange.x += width;

				var panel = reels[i].GetComponent<SpritePanel>();
				if (panel != null)
					panel.clipRange = clipRange;
			}
		}
	}
}
