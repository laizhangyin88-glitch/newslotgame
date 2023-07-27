using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class CalcLineWinByCell : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<int> columnOffset = 0;
        public BBParameter<int> rowOffset = 0;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<List<Blackboard>> payLineByCell;
        public BBParameter<long> multiplier = 1;

        public OperationMethod multiplierOperation = OperationMethod.Multiply;
        public SymbolAttribute ignoreSymbolAttribute = (SymbolAttribute)0;

        public BBParameter<List<SymbolWin>> saveAs;

        private const int wildSymbolIndex = 0;

        private long betPerLine { get { return betCredit.value / baseWager.value; } }

        private Cell GetCellFromIndex(int index, int totalColumn, int columnOffset, int rowOffset)
        {
            int column = index % totalColumn + columnOffset;
            int row    = index / totalColumn + rowOffset;
            return new Cell(column, row);
        }

        private SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        private long GetSymbolPay(int symbolIndex, int hitCount)
        {
            if (hitCount == 0)
                return 0;
            return payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
        }

        private List<int> GetPayLine(int lineIndex)
        {
            return payLineByCell.value[lineIndex].GetValue<List<int>>("value");
        }

        private long OperateSymbolMultiplier(long a, long b, OperationMethod multiplierOperation)
        {
            a = (multiplierOperation == OperationMethod.Add && a == 1) ? 0 : a;
            return OperationUtils.Operate(a, b, multiplierOperation);
        }

        private bool CompareSymbolEquality(SymbolInfo symbolInfo, SymbolInfo targetSymbolInfo)
        {
            if (SymbolMask.HasReject(symbolInfo))
                return false;

            return symbolInfo.Equals(targetSymbolInfo) || SymbolMask.HasWild(symbolInfo);
        }

        private bool CompareSymbolMask(SymbolInfo symbolInfo, SymbolAttribute mixedAttribute)
        {
            return SymbolMask.HasAttribute(symbolInfo, mixedAttribute) || SymbolMask.HasWild(symbolInfo);
        }

        private SymbolWin FindSymbolWin(int lineIndex)
        {
            SymbolWin symbolWin = new SymbolWin();
            SymbolInfo winSymbolInfo = null;
            foreach (int cellIndex in GetPayLine(lineIndex))
            {
                var cell = GetCellFromIndex(cellIndex, column.value, columnOffset.value, rowOffset.value);
                var symbolInfo = GetSymbolInfo(cell.column, cell.row);
                if (winSymbolInfo == null || SymbolMask.HasWild(winSymbolInfo))
                {
                    winSymbolInfo = symbolInfo;
                    symbolWin.symbolIndex = symbolInfo.symbol;
                }

                if (CompareSymbolEquality(symbolInfo, winSymbolInfo))
                    symbolWin.cells.Add(cell);
                else
                    break;
            }
            return UpdateSymbolWinInfo(symbolWin, lineIndex + 1);
        }

        private SymbolWin FindWildSymbolWin(int lineIndex)
        {
            SymbolWin symbolWin = new SymbolWin();
            symbolWin.symbolIndex = wildSymbolIndex;
            foreach (int cellIndex in GetPayLine(lineIndex))
            {
                var cell = GetCellFromIndex(cellIndex, column.value, columnOffset.value, rowOffset.value);
                var symbolInfo = GetSymbolInfo(cell.column, cell.row);
                if (SymbolMask.HasWild(symbolInfo))
                    symbolWin.cells.Add(cell);
                else
                    break;
            }
            return UpdateSymbolWinInfo(symbolWin, lineIndex + 1);
        }

        private SymbolWin FindMixedSymbolWin(int lineIndex, MixedLineWinInfo mixedLineWinInfo)
        {
            SymbolWin symbolWin = new SymbolWin();
            symbolWin.symbolIndex = mixedLineWinInfo.payIndex;
            List<int> singleLine = GetPayLine(lineIndex);
            for (int index = 0; index < singleLine.Count; ++index)
            {
                int cellIndex = singleLine[index];
                var cell = GetCellFromIndex(cellIndex, column.value, columnOffset.value, rowOffset.value);
                var symbolInfo = GetSymbolInfo(cell.column, cell.row);
                if (mixedLineWinInfo.masks.Count > index && CompareSymbolMask(symbolInfo, mixedLineWinInfo.masks[index]))
                    symbolWin.cells.Add(cell);
                else
                    break;
            }
            return UpdateSymbolWinInfo(symbolWin, lineIndex + 1);
        }

        private SymbolWin FilterMaxSymbolWin(List<SymbolWin> symbolWins)
        {
            SymbolWin symbolWin = symbolWins[0];
            foreach (var win in symbolWins)
            {
                if (symbolWin.CompareTo(win) > 0)
                    symbolWin = win;
            }
            return symbolWin;
        }

        private SymbolWin UpdateSymbolWinInfo(SymbolWin symbolWin, int lineIndex)
        {
            long lineMultiplier = 1;
            foreach (var cell in symbolWin.cells)
            {
                var symbolInfo = GetSymbolInfo(cell.column, cell.row);
                lineMultiplier = OperateSymbolMultiplier(lineMultiplier, (long)symbolInfo.multiplier, multiplierOperation);
            }
            symbolWin.lineIndex = lineIndex;
            symbolWin.hitCount = symbolWin.cells.Count;
            symbolWin.multiplier = lineMultiplier * multiplier.value;
            symbolWin.earnCredit = GetSymbolPay(symbolWin.symbolIndex, symbolWin.hitCount) * betPerLine * symbolWin.multiplier;
            return symbolWin;
        }

        private SymbolWin CalcLineSymbolWin(int lineIndex)
        {
            List<SymbolWin> symbolWins = new List<SymbolWin>();
            List<MixedLineWinInfo> mixedLineWinInfos = ContentCustomData.GetSlotData(slotIndex.value).mixedLineWinInfos;

            symbolWins.Add(FindSymbolWin(lineIndex));
            symbolWins.Add(FindWildSymbolWin(lineIndex));
            foreach (var mixedLineWinInfo in mixedLineWinInfos)
                symbolWins.Add(FindMixedSymbolWin(lineIndex, mixedLineWinInfo));

            return FilterMaxSymbolWin(symbolWins);
        }

        protected override void OnExecute()
        {
            List<SymbolWin> winList = new List<SymbolWin>();
            for (int i = 0; i < payLineByCell.value.Count; ++i)
            {
                var symbolWin = CalcLineSymbolWin(i);
                if (symbolWin.earnCredit > 0)
                {
                    ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
                    winList.Add(symbolWin);
                }
            }
            winList.Sort();

            saveAs.value = winList;
            EndAction();
        }
    }
}
