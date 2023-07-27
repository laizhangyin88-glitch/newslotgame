using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class MultiReelLayoutGroup : ReelLayoutGroup
    {
        [DetailedInfoBox("Click the DetailedInfo about Reel Group Offset List", "Use for jagged reels layout\n- Description - just for info about reel group\n- Offset - offset for each reel group\n- Reel Group Start Index - use for identify first reel in the reel group\n(use SetReelGroupOffsetList action for change ReelGroupOffsetList)")]

        [SerializeField]
        private List<ReelGroupOffset> reelGroupOffsetList;
        public List<ReelGroupOffset> ReelGroupOffsetList
        {
            get { return reelGroupOffsetList; }
            set { reelGroupOffsetList = value; }
        }

        public override void SetLayout()
        {
            var size = rectTransform.rect.size;

            rowCounts.Clear();
            int rowCount = 0;
            int minRow   = int.MaxValue;

            for (int i = 0; i < rectTransform.childCount; ++i)
            {
                var rt = rectTransform.GetChild(i);
                var reel = rt.GetComponent(typeof(BaseReel)) as BaseReel;

                for (int column = reel.beginColumn; column < reel.endColumn; ++column)
                {
                    if (column > (rowCounts.Count - 1))
                    {
                        rowCounts.Add(0);
                    }

                    rowCounts[column] += reel.RowCount;
                    rowCount = Mathf.Max(rowCount, rowCounts[column]);
                    minRow   = Mathf.Min(minRow, reel.beginRow);
                }
            }

            Vector2 alignment = new Vector2(((int)childAlignment % 3) * 0.5f, ((int)childAlignment / 3) * 0.5f);
            Vector2 startOffset = Vector2.zero;
            startOffset.x = -alignment.x * size.x;
            startOffset.y = alignment.y * size.y;
            Vector2 anchor = new Vector2(alignment.x, 1.0f - alignment.y);

            int reelGroupIndex = 0;
            Vector3 reelsOffset = Vector3.zero;

            for (int i = 0; i < rectTransform.childCount; ++i)
            {
                if (reelGroupIndex < reelGroupOffsetList.Count)
                {
                    var currentGroupOffset = reelGroupOffsetList[reelGroupIndex];
                    if (i >= currentGroupOffset.reelGroupStartIndex)
                    {
                        reelGroupIndex++;
                        reelsOffset = currentGroupOffset.offset;
                    }
                }

                var rt = rectTransform.GetChild(i) as RectTransform;
                rt.anchorMin = rt.anchorMax = anchor;

                var reel = rt.GetComponent<Reel>();

                float width  = CalcScalarWithSpacing(cellSize.x, spacing.x, reel.ColumnCount);
                float height = CalcScalarWithSpacing(cellSize.y, spacing.y, reel.RowCount);
                rt.sizeDelta = new Vector2(width, height);

                float x = startOffset.x + (cellSize.x + spacing.x) * reel.beginColumn + (width * 0.5f) + reelsOffset.x;
                float y = startOffset.y - ((cellSize.y + spacing.y) * (reel.beginRow - minRow)) * (gridBase ? 1f : alignment.y) - (height * 0.5f) + reelsOffset.y;
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

        [System.Serializable]
        public class ReelGroupOffset
        {
            [LabelWidth(100)]
            public string description;
            public Vector3 offset;
            public int reelGroupStartIndex;
        }
    }
}
