using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class CalcScatterAdjacentLineWin : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<long>> payTable;
        public BBParameter<List<Blackboard>> payLine;
        public BBParameter<long> multiplier = 1;
        public BBParameter<string> creditPath = "./spin";
        public BBParameter<int> symbolIndex;
        public BBParameter<int> symbolCount;
        public BBParameter<SymbolAttribute> symbolMask;
        public BBParameter<List<SymbolWin>> saveAs;

        private long betPerLine { get { return betCredit.value / baseWager.value; } }

        private SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

        public long FindEarnCredit(int hitCount)
        {
            if (payTable.isNull || payTable.isNone)
                return 0;

            return payTable.value[hitCount - 1];
        }

        private SymbolWin GetBiggestSymbolWin(List<SymbolWin> symbolWins)
        {
            SymbolWin symbolWin = null;
            long maxSymbolCount = symbolCount.value - 1;

            foreach(SymbolWin win in symbolWins)
            {
                if (maxSymbolCount < win.hitCount)
                {
                    win.earnCredit = FindEarnCredit(win.hitCount) * betPerLine * win.multiplier;
                    maxSymbolCount = win.hitCount;
                    symbolWin = win;
                }
            }
            return symbolWin;
        }

        protected long CalcOneLine(int lineIndex, List<SymbolWin> winList)
        {
            List<SymbolWin> symbolWinsInLine = new List<SymbolWin>();
            List<int> line = GetPayLine(lineIndex);
            
            bool found = false;

            for (int colIndex = 0; colIndex < column.value; ++colIndex)
            {
                int rowIndex = line[colIndex];
                SymbolInfo symbolInfo = GetSymbolInfo(colIndex, rowIndex);

                if (!SymbolMask.HasAttribute(symbolInfo, symbolMask.value) || SymbolMask.HasReject(symbolInfo))
                {
                    found = false;
                    continue;
                }

                if (!found)
                {
                    var win = new SymbolWin();
                    win.direction = 0;
                    win.lineIndex = lineIndex + 1;
                    win.multiplier = multiplier.value;
                    win.symbolIndex = symbolIndex.value;
                    win.cells.Add(new Cell(colIndex, rowIndex));
                    win.hitCount = win.cells.Count;
                    symbolWinsInLine.Add(win);

                    found = true;
                }
                else
                {
                    var win = symbolWinsInLine[symbolWinsInLine.Count - 1];
                    win.cells.Add(new Cell(colIndex, rowIndex));
                    win.hitCount = win.cells.Count;
                }
            }

            var symbolWin = GetBiggestSymbolWin(symbolWinsInLine);
            if (symbolWin == null)
                return 0L;

            winList.Add(symbolWin);
            return symbolWin.earnCredit;
        }

        protected override void OnExecute()
        {
            List<SymbolWin> winList = new List<SymbolWin>();
            long totalEarnCredit = 0L;
            for (int i = 0; i < payLine.value.Count; ++i)
                totalEarnCredit += CalcOneLine(i, winList);

            if (totalEarnCredit > 0 && !creditPath.isNull && !creditPath.isNone)
            {
                var spin = BlackboardUtils.FindVariable<Blackboard>(null, creditPath.value).value;
                BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
                ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);
            }

            winList.Sort();
            saveAs.value = winList;
            EndAction();
        }

#if UNITY_EDITOR
        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();
        }
#endif
    }

}
