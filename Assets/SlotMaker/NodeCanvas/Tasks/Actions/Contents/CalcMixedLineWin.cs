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
    public class CalcMixedLineWin : ActionTask<Blackboard>
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
        public OperationMethod MultiplierOperation = OperationMethod.Multiply;
        public BBParameter<bool> bidirectional;
        public BBParameter<bool> excludeMaxLine = false;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<MixedLineWinInfo> mixedLineWinInfos;
        protected List<SymbolWin> winList;
        private const int wildSymbolIndex = 0;

        protected long betPerLine { get { return betCredit.value / baseWager.value; } }

        protected int GetDirectionalColumn(int column, int direction)
        {
            return direction == 1 ? column : (this.column.value - 1) - column;
        }

        protected SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        protected long GetSymbolMultiplier(int symbolIndex)
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

        protected bool IsHit(ref SymbolInfo winSymbolInfo, SymbolInfo symbolInfo)
        {
            if (SymbolMask.HasReject(symbolInfo))
            {
                return false;
            }
            else if (winSymbolInfo.Equals(symbolInfo))
            {
                winSymbolInfo = symbolInfo;
                return true;
            }
            else if (SymbolMask.HasWild(symbolInfo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public long FindEarnCredit(int symbolIndex, int hitCount)
        {
            if (symbolIndex >= payTable.value.Count) return 0;
            return payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
        }

        public long FindMixedEarnCredit(int mixedWinMask, int mixedHitCount, out int symbolIndex)
        {
            for (int count = 0; count < mixedLineWinInfos.Count; ++count)
            {
                if (SymbolMask.HasAttribute((SymbolAttribute)mixedWinMask, mixedLineWinInfos[count].masks[0]))
                {
                    symbolIndex = mixedLineWinInfos[count].payIndex;
                    if (symbolIndex >= payTable.value.Count) return 0;
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

        protected long CalcOneLine(int lineIndex, int direction)
        {
            var symbolWin = new SymbolWin();

            int hitCount = 0;
            int wildHitCount = 0;
            int winSymbol = -1;
            SymbolInfo winSymbolInfo = null;
            long lineMultiplier = 1L;
            List<int> line = GetPayLine(lineIndex);

            int mixedHitCount = 0;
            int mixedWinMask = 0;
            bool mixedCheck = true;
            bool linedCheck = true;

            for (int column = 0; column < this.column.value; ++column)
            {
                int dColumn = GetDirectionalColumn(column, direction);
                int row = line[dColumn];
                var symbolInfo = GetSymbolInfo(dColumn, row);

                if (column == 0)
                {
                    winSymbolInfo = symbolInfo;
                    ++hitCount;
                    ++mixedHitCount;
                    mixedWinMask = (int)symbolInfo.mask;
                    if (SymbolMask.HasWild(winSymbolInfo))
                    {
                        ++wildHitCount;
                    }
                }
                else
                {
                    if (mixedCheck)
                    {
                        if (SymbolMask.HasAttribute((SymbolAttribute)mixedWinMask, SymbolAttribute.Wild))
                        {
                            ++mixedHitCount;
                            mixedWinMask = (int)symbolInfo.mask;
                        }
                        else if (SymbolMask.HasWild(symbolInfo))
                        {
                            ++mixedHitCount;
                        }
                        else if (mixedWinMask != 0 && symbolInfo.mask != 0 &&
                                 SymbolMask.HasAnyAttribute((SymbolAttribute)mixedWinMask, symbolInfo.mask))
                        {
                            ++mixedHitCount;
                            mixedWinMask = (int)((int)symbolInfo.mask & mixedWinMask);
                        }
                        else
                        {
                            mixedCheck = false;
                        }
                    }

                    if (linedCheck)
                    {
                        if (SymbolMask.HasWild(winSymbolInfo))
                        {
                            winSymbolInfo = symbolInfo;
                            ++hitCount;
                        }
                        else if (IsHit(ref winSymbolInfo, symbolInfo))
                        {
                            ++hitCount;
                        }
                        else
                        {
                            linedCheck = false;
                        }

                        if (SymbolMask.HasWild(symbolInfo) && SymbolMask.HasWild(winSymbolInfo))
                            ++wildHitCount;
                    }

                    if (!linedCheck && !mixedCheck)
                        break;
                }

                // multiplier끼리 더하거나 곱할 수 있습니다.
                lineMultiplier = (long)OperationTools.Operate(lineMultiplier, GetSymbolMultiplier(symbolInfo.symbol), MultiplierOperation);

                // TODO
                // cells 는 실제 히트된 cell 만 포함하고 있어야 합니다.
                // 현재 버전에서는 hit 되지 않은 cell 도 포함되고 있습니다.
                symbolWin.cells.Add(new Cell(dColumn, row));
            }
            winSymbol = winSymbolInfo.symbol;

            long earnCredit = FindEarnCredit(winSymbolInfo.symbol, hitCount);

            if (wildHitCount > 0)
            {
                long wildEarnCredit = FindEarnCredit(wildSymbolIndex, wildHitCount);
                if (wildEarnCredit > earnCredit)
                {
                    int count = hitCount - wildHitCount;
                    for (int column = 0; column < count; ++column)
                        symbolWin.cells.RemoveAt(wildHitCount);

                    winSymbol = wildSymbolIndex;
                    hitCount = wildHitCount;
                    earnCredit = wildEarnCredit;
                }
            }

            int mixedSymbolIndex = -1;
            long mixedEarnCredit = FindMixedEarnCredit(mixedWinMask, mixedHitCount, out mixedSymbolIndex);
            if (mixedEarnCredit > earnCredit)
            {
                winSymbol = mixedSymbolIndex;
                hitCount = mixedHitCount;
                earnCredit = mixedEarnCredit;
            }

            if (direction == -1 && excludeMaxLine.value == true && hitCount == this.column.value)
                return 0L;

            if (earnCredit > 0L)
            {
                if (MultiplierOperation == OperationMethod.Add && lineMultiplier != 1L)
                    --lineMultiplier;

                symbolWin.symbolIndex = winSymbol;
                symbolWin.direction = direction;
                symbolWin.lineIndex = lineIndex + 1;
                symbolWin.hitCount = hitCount;
                symbolWin.multiplier = multiplier.value * lineMultiplier;
                symbolWin.earnCredit = earnCredit * betPerLine * symbolWin.multiplier;
                winList.Add(symbolWin);

                ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
            }

            return symbolWin.earnCredit;
        }

        private int GetPayLineIndex(List<int> line)
        {
            
            for (int i = 0; i < payLine.value.Count; i++)
            {
                var tempLine = GetPayLine(i);
                int count = 0;
                for (int j = 0; j < tempLine.Count; j++)
                {
                    if ((tempLine[j] == line[j]))
                    {
                        count++;
                        if(count >= 5)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return 0;
        }

        private long FillLineData(List<SymbolWin> winList, int dircetion)
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();
            Variable<Blackboard> spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            var response = spinBB.value.GetValue<Blackboard>("response");
            var list = response.GetValue<List<List<int>>>("total_line_result");
            long totalEarnCredit = 0L;
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    List<int> line = list[i];
                    SymbolInfo temp = null;
                    int hitCount = 0;
                    SymbolWin symbolWin = new SymbolWin();
                    symbolWin.direction = dircetion;
                    symbolWin.lineIndex = GetPayLineIndex(line) + 1;
                    for (int j = 0; j < line.Count; j++)
                    {
                        if (temp == null)
                        {
                            temp = deck.deck[j][line[j]];
                            hitCount++;
                            symbolWin.symbolIndex = temp.symbol;
                            symbolWin.cells.Add(new Cell(j, line[j]));
                            winList.Add(symbolWin);
                        }
                        else
                        {
                            if(temp.symbol == deck.deck[j][line[j]].symbol)
                            {
                                symbolWin.cells.Add(new Cell(j, line[j]));
                                hitCount++;
                                winList.Add(symbolWin);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    long lineMultiplier = 1L;
                    lineMultiplier = (long)OperationTools.Operate(lineMultiplier, GetSymbolMultiplier(temp.symbol), MultiplierOperation);
                    if (MultiplierOperation == OperationMethod.Add && lineMultiplier != 1L)
                        --lineMultiplier;
                    symbolWin.hitCount = hitCount;
                    symbolWin.multiplier = multiplier.value * lineMultiplier;
                    long earnCredit = FindEarnCredit(temp.symbol, hitCount);
                    symbolWin.earnCredit = earnCredit * betPerLine * symbolWin.multiplier;
                    totalEarnCredit += earnCredit;
                    winList.Add(symbolWin);
                }
            }
            return totalEarnCredit;
        }

        protected override void OnExecute()
        {
            mixedLineWinInfos = ContentCustomData.GetSlotData(slotIndex.value).mixedLineWinInfos;

            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();
            if (globalStore.IsInNewGame())
            {
                totalEarnCredit += FillLineData(winList, 1);
                if (bidirectional.value)
                    totalEarnCredit += FillLineData(winList, -1);
            }
            else
            {
                for (int i = 0; i < payLine.value.Count; ++i)
                {
                    totalEarnCredit += CalcOneLine(i, 1);
                    if (bidirectional.value)
                        totalEarnCredit += CalcOneLine(i, -1);
                }
            }
            winList.Sort();

            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

            saveAs.value = winList;

            EndAction();
        }
    }

}
