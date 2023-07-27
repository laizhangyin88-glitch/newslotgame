using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMaker
{
    public static class LayoutUtils 
    {
        public static int CalcGridLayoutGroupMaxColumn(RectTransform referenceRect, GridLayoutGroup gridLayoutGroup)
        {
            float width = referenceRect.rect.width;
            float padding = gridLayoutGroup.padding.horizontal;
            float cellSize = gridLayoutGroup.cellSize.x;
            float spacing = gridLayoutGroup.spacing.x;

            if (cellSize + spacing <= 0)
                return int.MaxValue;
            else
                return Mathf.Max(1, Mathf.FloorToInt((width - padding + spacing + 0.001f) / (cellSize + spacing)));
        }

        public static int CalcGridLayoutGroupMaxRow(RectTransform referenceRect, GridLayoutGroup gridLayoutGroup)
        {
            float height = referenceRect.rect.height;
            float padding = gridLayoutGroup.padding.vertical;
            float cellSize = gridLayoutGroup.cellSize.y;
            float spacing = gridLayoutGroup.spacing.y;

            if (cellSize + spacing <= 0)
                return int.MaxValue;
            else
                return Mathf.Max(1, Mathf.FloorToInt((height - padding + spacing + 0.001f) / (cellSize + spacing)));
        }
    }
}
