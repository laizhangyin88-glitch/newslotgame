using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker.Slots.Strategy;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class SolidWinLine : WinLine
    {
        public LineRenderer lineRenderer;

        [InlineEditor]
        public SymbolPositionStrategy positionStrategy;

        public bool makeSideEdge;
        [ShowIf("makeSideEdge")]
        public float sideEdgeLength;

        public override void Win(SpinLineWin win)
        {
            onWin.Invoke();
        }

        public override void SetLineIndex(int lineIndex_)
        {
            base.SetLineIndex(lineIndex_);
            lineRenderer.sortingOrder = lineIndex;
        }

        public override void SetLine(List<int> line, int maxRow, float offset)
        {
            var extents = positionStrategy.GetExtents();

            var points = new Vector3[line.Count + (makeSideEdge ? 2 : 0)];
            int spotCount = line.Count;
            for (int i = 0; i < spotCount; ++i)
            {
                int row = line[i];
                var pos = positionStrategy.GetPosition(spotCount, maxRow, i, row, 1, 1, 0, 0);
                pos.y += offset;

                if (makeSideEdge)
                {
                    if (i == 0)
                    {
                        points[i] = pos;
                        points[i].x -= extents.x - sideEdgeLength;
                    }

                    points[i + 1] = pos;

                    if (i == spotCount - 1)
                    {
                        points[i + 2] = pos;
                        points[i + 2].x += extents.x - sideEdgeLength;
                    }
                }
                else
                {
                    points[i] = pos;
                }
            }

            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }

        public override void SetColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public override void SetGradient(Gradient gradient)
        {
            lineRenderer.colorGradient = gradient;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        }
#endif
    }
}