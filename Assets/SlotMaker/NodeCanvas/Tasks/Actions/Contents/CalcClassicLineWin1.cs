using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class CalcClassicLineWin1 : ActionTask<Blackboard>
    {
        [Serializable]
        public class AnyMixedCondition
        {
            public SymbolAttribute mask;
            public int symbolCount;

            public int GetMaskIndex()
            {
                return Array.IndexOf(Enum.GetValues(typeof(SymbolAttribute)), mask);
            }
        }

        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;

        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<List<Blackboard>> payLine;

        public BBParameter<long> multiplier;

        public OperationMethod MultiplierOperation = OperationMethod.Multiply;

        public BBParameter<bool> bidirectional;
        public BBParameter<bool> excludeMaxLine = false;
        public List<List<SymbolAttribute>> specialPay;
        public BBParameter<List<Blackboard>> specialPayTable;
        // specialPay lists indicate exact condition of win
        public List<List<AnyMixedCondition>> anyMixedPay;
        public BBParameter<List<Blackboard>> anyMixedPayTable;
        // other pays are listed in anyMixedPay
        public BBParameter<List<SymbolWin>> saveAs;

        protected List<MixedLineWinInfo> mixedLineWinInfos;
        protected List<SymbolWin> winList;
        private const int wildSymbolIndex = 0;

        protected long betPerLine => betCredit.value / baseWager.value;

        protected int GetDirectionalColumn(int column, int direction)
        {
            return direction == 1 ? column : (this.column.value - 1) - column;
        }

        protected SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        protected bool IsHit(ref SymbolInfo winSymbolInfo, SymbolInfo symbolInfo)
        {
            if (SymbolMask.HasReject(symbolInfo))
            {
                return false;
            }

            if (winSymbolInfo.Equals(symbolInfo))
            {
                winSymbolInfo = symbolInfo;
                return true;
            }

            if (SymbolMask.HasWild(symbolInfo))
            {
                return true;
            }

            return false;
        }

        public long FindEarnCredit(List<Blackboard> payTable, int symbolIndex, int hitCount)
        {
            return payTable[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
        }

        public long FindMixedEarnCredit(int mixedWinMask, int mixedHitCount, out int symbolIndex)
        {
            for (int count = 0; count < mixedLineWinInfos.Count; ++count)
            {
                if (SymbolMask.HasAttribute((SymbolAttribute)mixedWinMask, mixedLineWinInfos[count].masks[0]))
                {
                    symbolIndex = mixedLineWinInfos[count].payIndex;
                    return payTable.value[symbolIndex].GetValue<List<long>>("value")[mixedHitCount - 1];
                }
            }
            symbolIndex = -1;
            return 0L;
        }

        public bool HaveMixedAttribute(int column, SymbolInfo symbolInfo)
        {
            for (int count = 0; count < mixedLineWinInfos.Count; ++count)
            {
                if (mixedLineWinInfos[count].masks.Count > column &&
                    SymbolMask.HasAttribute(mixedLineWinInfos[count].masks[column], symbolInfo.mask))
                {
                    return true;
                }
            }
            return false;
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

        public bool IsSpecialPay(List<int> line, List<SymbolAttribute> specialPay, int direction)
        {
            for (var col = 0; col < line.Count; ++col)
            {
                var dColumn = GetDirectionalColumn(col, direction);
                var row = line[dColumn];
                var symbolInfo = GetSymbolInfo(dColumn, row);
                if (!SymbolMask.HasAttribute(symbolInfo.mask, specialPay[dColumn]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsAnyMixedPay(List<int> line, List<int> maskCounts, List<AnyMixedCondition> conditionList)
        {
            for (var i = 0; i < conditionList.Count; ++i)
            {
                var condition = conditionList[i];
                var maskIndex = condition.GetMaskIndex();

                if (condition.symbolCount != maskCounts[maskIndex])
                    return false;
            }
            return true;
        }

        public void GetMaskCountAndCell(List<int> maskCounts, List<List<Cell>> maskCellList, List<int> line)
        {
            for (int i = 0; i < 15; ++i)
            {
                maskCounts.Add(0);
                maskCellList.Add(new List<Cell>());
            }
            for (int col = 0; col < line.Count; ++col)
            {
                int row = line[col];
                var symbolInfo = GetSymbolInfo(col, row);
                for (int i = 0; i < 15; ++i)
                {
                    int mask = (1 << i);
                    if (SymbolMask.HasAttribute(symbolInfo.mask, mask))
                    {
                        ++maskCounts[i];
                        maskCellList[i].Add(new Cell(col, row));
                    }
                }
            }
        }

        protected long CalcOneLine(int lineIndex, int direction)
        {
            var symbolWin = new SymbolWin();

            int hitCount = 0;
            int wildHitCount = 0;
            int winSymbol    = -1;
            SymbolInfo winSymbolInfo = null;

            List<long> winSymbolMultiplierValues = new List<long>();

            List<int> winLine = GetPayLine(lineIndex);

            int  mixedHitCount = 0;
            int  mixedWinMask  = 0;
            bool needToCheckMixed = true;
            bool needToCheckLined = true;
            bool needToCheckAnyMixedPay = false;

            for (int i = 0; i < this.column.value; i++)
            {
                int colIndex = GetDirectionalColumn(i, direction);
                int rowIndex = winLine[colIndex];
                var currentSymbolInfo = GetSymbolInfo(colIndex, rowIndex);

                // First Symbol
                if (i == 0)
                {
                    winSymbolInfo = currentSymbolInfo;
                    hitCount++;
                    mixedHitCount++;
                    mixedWinMask = (int)currentSymbolInfo.mask;
                    if (SymbolMask.HasWild(winSymbolInfo))
                        wildHitCount++;
                }
                else
                {
                    if (needToCheckMixed)
                    {
                        if (SymbolMask.HasAttribute((SymbolAttribute)mixedWinMask, SymbolAttribute.Wild))
                        {
                            mixedHitCount++;
                            mixedWinMask = (int)currentSymbolInfo.mask;
                        }
                        else if (SymbolMask.HasWild(currentSymbolInfo))
                        {
                            mixedHitCount++;
                        }
                        else if (mixedWinMask != 0 && currentSymbolInfo.mask != 0 &&
                                 SymbolMask.HasAnyAttribute((SymbolAttribute)mixedWinMask, currentSymbolInfo.mask))
                        {
                            mixedHitCount++;
                            mixedWinMask = (int)currentSymbolInfo.mask & mixedWinMask;
                        }
                        else
                        {
                            needToCheckMixed = false;
                        }
                    }

                    if (needToCheckLined)
                    {
                        if (SymbolMask.HasWild(winSymbolInfo))
                        {
                            winSymbolInfo = currentSymbolInfo;
                            hitCount++;
                        }
                        else if (IsHit(ref winSymbolInfo, currentSymbolInfo))
                        {
                            hitCount++;
                        }
                        else
                        {
                            needToCheckLined = false;
                        }

                        if (SymbolMask.HasWild(currentSymbolInfo) && SymbolMask.HasWild(winSymbolInfo))
                            wildHitCount++;
                    }

                    if (!needToCheckLined && !needToCheckMixed)
                        break;
                }

                var symbolMultiplier = currentSymbolInfo.multiplier;
                if (MultiplierOperation == OperationMethod.Add && symbolMultiplier <= 1)
                    symbolMultiplier = 0;

                winSymbolMultiplierValues.Add(symbolMultiplier);

                // TODO
                // cells 는 실제 히트된 cell 만 포함하고 있어야 합니다.
                // 현재 버전에서는 hit 되지 않은 cell 도 포함되고 있습니다.
                symbolWin.cells.Add(new Cell(colIndex, rowIndex));
            }

            long lineMultiplier = MultiplierOperation == OperationMethod.Add ? 0 : 1;

            foreach (var winSymbolMultiplierValue in winSymbolMultiplierValues)
            {
                lineMultiplier = OperationUtils.Operate(lineMultiplier, winSymbolMultiplierValue, MultiplierOperation);
            }

            if (lineMultiplier == 0L)
                lineMultiplier = 1L;


            winSymbol = winSymbolInfo.symbol;
            long earnCredit = FindEarnCredit(payTable.value, winSymbolInfo.symbol, hitCount);

            if (wildHitCount > 0)
            {
                long wildEarnCredit = FindEarnCredit(payTable.value, wildSymbolIndex, wildHitCount);
                if (wildEarnCredit > earnCredit)
                {
                    int count = hitCount - wildHitCount;
                    for (int column = 0; column < count; ++column)
                        symbolWin.cells.RemoveAt(wildHitCount);

                    winSymbol = wildSymbolIndex;
                    hitCount  = wildHitCount;
                    earnCredit = wildEarnCredit;
                }
            }


            long mixedEarnCredit = FindMixedEarnCredit(mixedWinMask, mixedHitCount, out int mixedSymbolIndex);
            if (mixedEarnCredit > earnCredit)
            {
                winSymbol  = mixedSymbolIndex;
                hitCount   = mixedHitCount;
                earnCredit = mixedEarnCredit;
            }
            // line win

            for (int i = 0; i < specialPay.Count; ++i)
            {
                if (IsSpecialPay(winLine, specialPay[i], direction))
                {
                    long specialEarnCredit = FindEarnCredit(specialPayTable.value, i, column.value);
                    if (specialEarnCredit > earnCredit * lineMultiplier)
                    {
                        symbolWin.cells.Clear();
                        for (int col = 0; col < column.value; ++col)
                        {
                            int dColumn = GetDirectionalColumn(col, direction);
                            int row = winLine[dColumn];
                            symbolWin.cells.Add(new Cell(dColumn, row));
                        }
                        lineMultiplier = 1L;
                        hitCount = column.value;
                        earnCredit = specialEarnCredit;
                    }
                }
            }
            // special win

            List<int> maskCounts = new List<int>();
            List<List<Cell>> maskCellList = new List<List<Cell>>();
            GetMaskCountAndCell(maskCounts, maskCellList, winLine);
            for (int i = 0; i < anyMixedPay.Count; ++i)
            {
                List<AnyMixedCondition> conditionList = anyMixedPay[i];
                if (IsAnyMixedPay(winLine, maskCounts, conditionList))
                {
                    long anyMixedEarnCredit = FindEarnCredit(anyMixedPayTable.value, i, column.value);
                    if (anyMixedEarnCredit > earnCredit * lineMultiplier)
                    {
                        List<Cell> cellList = new List<Cell>();
                        for (int index = 0; index < conditionList.Count; ++index)
                        {
                            int maskIndex = conditionList[index].GetMaskIndex();
                            cellList.AddRange(maskCellList[maskIndex]);
                        }
                        symbolWin.cells = cellList;
                        lineMultiplier = 1L;
                        hitCount = column.value;
                        earnCredit = anyMixedEarnCredit;
                        needToCheckAnyMixedPay = true;
                    }
                }
            }
            // any special win


            if (direction == -1 && excludeMaxLine.value && (hitCount == this.column.value || needToCheckAnyMixedPay))
                return 0L;

            if (earnCredit > 0L)
            {
                symbolWin.symbolIndex = winSymbol;
                symbolWin.direction  = direction;
                symbolWin.lineIndex  = lineIndex + 1;
                symbolWin.hitCount   = hitCount;
                symbolWin.multiplier = multiplier.value * lineMultiplier;
                symbolWin.earnCredit = earnCredit * betPerLine * symbolWin.multiplier;
                winList.Add(symbolWin);

                ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
            }

            return symbolWin.earnCredit;
        }

        protected override void OnExecute()
        {
            mixedLineWinInfos = ContentCustomData.GetSlotData(slotIndex.value).mixedLineWinInfos;

            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();

            for (int i = 0; i < payLine.value.Count; ++i)
            {
                totalEarnCredit += CalcOneLine(i, 1);
                if (bidirectional.value)
                    totalEarnCredit += CalcOneLine(i, -1);
            }
            winList.Sort();

            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue(spin, "winList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

            saveAs.value = winList;

            EndAction();
        }
    }
}
