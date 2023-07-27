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
    public class CalcAdjacentLineWin : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<List<Blackboard>> payLine;
        public BBParameter<long> multiplier;
        public BBParameter<List<long>> symbolMultipliers;
        public BBParameter<string> creditPath = "./spin";

        public OperationMethod MultiplierOperation = OperationMethod.Multiply;
        public SymbolAttribute ignoreSymbolAttribute = (SymbolAttribute)0;

        public BBParameter<List<SymbolWin>> saveAs;

        private long betPerLine { get { return betCredit.value / baseWager.value; } }

        private long GetSymbolMultiplier(int symbolIndex)
        {
            if (MultiplierOperation == OperationMethod.Add)
            {
                if (symbolMultipliers.isNone || symbolMultipliers.isNull || symbolMultipliers.value[symbolIndex] == 1)
                    return 0L;
            }
            if (symbolMultipliers.isNone || symbolMultipliers.isNull)
                return 1L;

            return symbolMultipliers.value[symbolIndex];
        }

        public long GetLineMultiplier(int symbolIndex, int hitCount)
        {
            long lineMultiplier = 1L;

            for (int i = 0; i < hitCount; i++)
                lineMultiplier = (long)OperationTools.Operate(lineMultiplier, GetSymbolMultiplier(symbolIndex), MultiplierOperation);

            if (MultiplierOperation == OperationMethod.Add && lineMultiplier != 1L)
                --lineMultiplier;

            return lineMultiplier;
        }

        private SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

        public long FindEarnCredit(int symbolIndex, int hitCount)
        {
            return payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
        }

        private SymbolWin GetBiggestSymbolWin(List<SymbolWin> symbolWins)
        {
            SymbolWin symbolWin = null;
            long maxEarnCredit = 0;

            foreach(SymbolWin win in symbolWins)
            {
                long earnCredit = FindEarnCredit(win.symbolIndex, win.cells.Count);
                long multiplier = GetLineMultiplier(win.symbolIndex, win.hitCount) * win.multiplier;
                long totalEarnCredit = earnCredit * betPerLine * multiplier;

                if (maxEarnCredit < totalEarnCredit)
                {
                    win.earnCredit = maxEarnCredit = totalEarnCredit;
                    win.multiplier = multiplier;
                    symbolWin = win;
                }
            }
            return symbolWin;
        }

        protected long CalcOneLine(int lineIndex, List<SymbolWin> winList)
        {
            List<SymbolWin> symbolWinsInLine = new List<SymbolWin>();
            List<int> line = GetPayLine(lineIndex);
            
            int symbolIndex = -1;

            for (int column = 0; column < this.column.value; ++column)
            {
                int row = line[column];
                SymbolInfo symbolInfo = GetSymbolInfo(column, row);

                if (SymbolMask.HasAttribute(symbolInfo, ignoreSymbolAttribute) || SymbolMask.HasReject(symbolInfo))
                {
                    symbolIndex = -1;
                    continue;
                }

                if (symbolIndex == -1 || symbolIndex != symbolInfo.symbol)
                {
                    var win = new SymbolWin();
                    win.direction = 0;
                    win.lineIndex = lineIndex + 1;
                    win.multiplier = multiplier.value;
                    win.symbolIndex = symbolInfo.symbol;
                    win.cells.Add(new Cell(column, row));
                    win.hitCount = win.cells.Count;
                    symbolWinsInLine.Add(win);

                    symbolIndex = symbolInfo.symbol;
                }
                else
                {
                    var win = symbolWinsInLine[symbolWinsInLine.Count - 1];
                    win.cells.Add(new Cell(column, row));
                    win.hitCount = win.cells.Count;
                }
            }

            var symbolWin = GetBiggestSymbolWin(symbolWinsInLine);
            if (symbolWin == null)
                return 0L;

            ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
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
