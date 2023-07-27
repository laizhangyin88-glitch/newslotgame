using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationStack : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public SymbolAttribute symbolMask;
        public bool allowCombination;
        public int stackingCount;
        public BBParameter<List<Cell>> saveAs;
        [SerializeField] protected bool needAllSpots = true;
        [SerializeField] protected bool checkAll;
        [SerializeField] protected bool continuous;
        [SerializeField] protected bool ignoreExpectation;
        [SerializeField] protected List<bool> masks;

        protected override string info { get { return string.Format("UpdateExpectationStack({0}, {1})", symbolMask, stackingCount); } }

        private bool IsIgnoreColumn(int col)
        {
            return checkAll ? false : !masks[col];
        }

        private bool IsStacking(ref SymbolInfo symbolInfo, SymbolInfo newSymbolInfo)
        {
            if (SymbolMask.HasAttribute(newSymbolInfo, symbolMask))
            {
                if (symbolInfo == null)
                {
                    symbolInfo = newSymbolInfo;
                    return true;
                }

                if (!allowCombination && !symbolInfo.Equals(newSymbolInfo))
                    return false;

                return true;
            }

            return false;
        }

        private void SubmitCells(Expectation expectation, List<Cell> cells)
        {
            int count = cells.Count;
            if (needAllSpots)
            {
                for (int i = 0; i < count; ++i)
                {
                    expectation.expectationSpots[cells[i].column].Add(cells[i]);
                }
            }
            else
            {
                var lastCell = cells[count - 1];
                expectation.expectationSpots[lastCell.column].Add(lastCell);
            }

            saveAs.value.Add(cells[count - 1]);
        }

        protected override void OnExecute()
        {
            saveAs.value = new List<Cell>();

            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            for (int i = 0; i < column.value; ++i)
            {
                if (IsIgnoreColumn(i))
                    continue;

                if (i != 0 && !ignoreExpectation)
                    expectation.expectations[i] = true;

                SymbolInfo symbolInfo = null;
                List<Cell> cells = new List<Cell>();
                bool found = false;
                for (int j = 0; j < row.value; ++j)
                {
                    var newSymbolInfo = deck.GetDeckSymbol(i, j);
                    if (IsStacking(ref symbolInfo, newSymbolInfo))
                    {
                        cells.Add(new Cell(i, j));
                    }
                    else
                    {
                        if (cells.Count >= stackingCount)
                        {
                            SubmitCells(expectation, cells);
                            found = true;
                        }
                        symbolInfo = null;
                        cells.Clear();
                    }
                }

                if (cells.Count >= stackingCount)
                {
                    SubmitCells(expectation, cells);
                    found = true;
                }

                if (!found && continuous) break;
             }
             EndAction();
    	}

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();

            needAllSpots = UnityEditor.EditorGUILayout.Toggle("Need All Spots", needAllSpots);
            checkAll = UnityEditor.EditorGUILayout.Toggle("Check All", checkAll);
            if (!checkAll)
                masks = (List<bool>)EditorUtils.ReflectedFieldInspector("Masks", masks, typeof(List<bool>));
            ignoreExpectation = UnityEditor.EditorGUILayout.Toggle("Ignore Expectation", ignoreExpectation);
            continuous = UnityEditor.EditorGUILayout.Toggle("Continuous", continuous);
        }

        #endif
    }
}
