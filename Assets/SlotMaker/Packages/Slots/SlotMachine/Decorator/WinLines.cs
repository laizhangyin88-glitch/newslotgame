using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
using SlotMaker.Rendering;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class WinLines : MonoBehaviour
    {
        public SortingGroup sortingGroup;

        [InlineEditor]
        public SortingLayerNameIDs sortingLayerIDs;

        [Space]
        public List<WinLine> lines;

        [InlineEditor]
        public Paylines paylines;

        public GameObject prefab;
        
        [InlineEditor]
        public VariableColorList colors;
        
        [InlineEditor]
        public VariableGradientList gradients;

        public enum RowGroupOrdering
        {
            DoNothing,
            Descending,
            Intersecting
        }
        public RowGroupOrdering rowGroupOrdering = RowGroupOrdering.Intersecting;
        public int maxRow;
        public float groupInterval;
        
        [Button("Show")]
        public void ActiveAllLines()
        {
            foreach (var line in lines)
            {
                line.ActiveLine();
            }
        }

        [Button("Hide")]
        public void InActiveAllLines()
        {
            foreach (var line in lines)
            {
                line.InActiveLine();
            }
            SetSortingLayer(0);
        }

        private void SetSortingLayer(int layerIndex)
        {
            sortingGroup.sortingLayerID = sortingLayerIDs.GetID(layerIndex);
        }

        private void ShowAllWinningLines(SpinOutputWinningSubset eventData)
        {
            var spinOutputLineWin = eventData as SpinOutputLineWin;
            if (spinOutputLineWin != null)
            {
                var totalWin = spinOutputLineWin.totalWin;
                foreach (var win in totalWin)
                {
                    lines[win.lineIndex].ActiveLine();
                }
            }
        }

        public void TotalWin(SpinOutputWinningSubset eventData)
        {
            SetSortingLayer(0);
            ShowAllWinningLines(eventData);
        }

        public void TotalWinLine(SpinOutputWinningSubset eventData)
        {
            SetSortingLayer(1);
            ShowAllWinningLines(eventData);
        }

        public void SingleWin(SpinWin eventData)
        {
            var win = eventData as SpinLineWin;
            if (win != null)
            {
                lines[win.lineIndex].Win(win);
            }
        }

#if UNITY_EDITOR
        [Button]
        public void Create()
        {
            Clear();

            for (int i = 0, count = paylines.value.Count; i < count; ++i)
            {
                var line = CreateWinLine(i);
                lines.Add(line);
            }
        }

        protected WinLine CreateWinLine(int index)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.name = string.Format("WinLine {0}", index);
            go.transform.SetParent(gameObject.transform, false);
            return go.GetComponent<WinLine>();
        }

        private class GroupInfo
        {
            public int groupIndex;
            public float direction;
            
            public float NextOffset()
            {
                float offset = (float)((groupIndex++ + 1) / 2) * direction;
                direction *= -1f;
                return offset;
            }
        }

        [Button]
        public void UpdateLines()
        {
            var groupInfos = new GroupInfo[maxRow];
            for (int i = 0; i < maxRow; ++i)
            {
                groupInfos[i] = new GroupInfo
                {
                    groupIndex = 0,
                    direction = i < (maxRow / 2) ? 1f : -1f
                };
            }

            for (int i = 0, count = paylines.value.Count; i < count; ++i)
            {
                var line = lines[i];
                line.SetLineIndex(i);

                float offset = groupInterval;
                switch (rowGroupOrdering)
                {
                case RowGroupOrdering.DoNothing:
                    break;
                case RowGroupOrdering.Descending:
                    offset *= groupInfos[paylines.value[i][0]].groupIndex++;
                    offset *= -1f;
                    break;
                case RowGroupOrdering.Intersecting:
                    offset *= groupInfos[paylines.value[i][0]].NextOffset();
                    break;
                }
                
                line.SetLine(paylines.value[i], maxRow, offset);
            }
        }

        [Button]
        public void UpdateColors()
        {
            int colorCount = colors.Count;
            for (int i = 0, count = lines.Count; i < count; ++i)
            {
                lines[i].SetColor(colors[i % colorCount]);
            }
        }

        [Button]
        public void UpdateGradients()
        {
            int gradientCount = gradients.Count;
            for (int i = 0, count = lines.Count; i < count; ++i)
            {
                lines[i].SetGradient(gradients[i % gradientCount]);
            }
        }

        [Button]
        public void Clear()
        {
            lines.Clear();
            gameObject.DestroyChildren();
        }
#endif

        //////////////////////////////////////////////////////////////////////////////////////////
        /// EDITOR
        //////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        private void OnValidate()
        {
        	if (sortingGroup == null) sortingGroup = GetComponent<SortingGroup>();
        }
#endif
    }
}
