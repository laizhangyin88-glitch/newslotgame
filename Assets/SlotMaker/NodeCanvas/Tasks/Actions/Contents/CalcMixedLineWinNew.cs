using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;
using SimpleJSON;

namespace BagelCode.Tasks.Actions.Contents
{

    [Category("★ BagelCode/Contents")]
    public class CalcMixedLineWinNew : ActionTask<Blackboard>
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


        private bool isWild(SymbolInfo symbolInfo)
        {
            return SymbolMask.HasAttribute(symbolInfo, SymbolAttribute.Wild);
        }


        protected long CalcAllLine(int direction)
        {

            var bb = ContentBlackboard.Get();

            //Dictionary<int, int>  changeCode = bb.GetValue<Dictionary<int, int>>("changeCode");
            Dictionary<int, int> changeCode = bb.GetValue<Blackboard>("gameNew").GetValue<Dictionary<int, int>>("changeCode");

            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = (Deck)slotData.deck.Clone();
            Variable<Blackboard> spinBB = ContentBlackboard.Get().GetVariable<Blackboard>("spin");
            string responseNew = spinBB.value.GetValue<string>("responseNew");
            JSONNode res = JSONNode.Parse(responseNew);


            JSONNode lineResult = res["game_result"]["total_result"]; //提个禅道
            Dictionary<int, JSONNode> lineResultDic = new Dictionary<int, JSONNode>();
            foreach (JSONNode item in lineResult)
            {
                if (item.HasKey("index"))
                {
                    lineResultDic.Add((int)item["index"],item);
                }
            }

            JSONNode lineInfo = res["game_result"]["win_line_reward_list"];
            for (int i=0; i < lineInfo.Count; i++)
            {
                var symbolWin = new SymbolWin();
                int lineIndex = (int)lineInfo[i]["index"];
                long credit = (long)lineInfo[i]["credit"];
                int hitCount = lineResultDic[lineIndex]["count"];
                int winSymbol = lineResultDic[lineIndex]["value"];
                if (changeCode.ContainsKey(winSymbol))
                    winSymbol = changeCode[winSymbol];

                List<int> line = GetPayLine(lineIndex);
                //for (int column = 0; column < this.column.value; ++column)
                for (int column = 0; column < hitCount; ++column)
                {
                    int dColumn = GetDirectionalColumn(column, direction);
                    int row = line[dColumn];
                    //var symbolInfo = GetSymbolInfo(dColumn, row);
                    symbolWin.cells.Add(new Cell(dColumn, row));
                }

                symbolWin.symbolIndex = winSymbol; 
                symbolWin.direction = direction;
                symbolWin.lineIndex = lineIndex + 1;
                symbolWin.hitCount = hitCount;
                //symbolWin.multiplier = multiplier.value * lineMultiplier;
                //symbolWin.earnCredit = earnCredit * betPerLine * symbolWin.multiplier;
                symbolWin.multiplier = 1L;
                symbolWin.earnCredit = credit;//    earnCredit * betPerLine * symbolWin.multiplier;
                winList.Add(symbolWin);

                ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(symbolWin.cells);
            }
            return (long)res["game_result"]["earn_credit"];
        }


        protected override void OnExecute()
        {
            mixedLineWinInfos = ContentCustomData.GetSlotData(slotIndex.value).mixedLineWinInfos;
            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();
            totalEarnCredit = CalcAllLine(bidirectional.value?-1:1); //bidirectional 双向？
            winList.Sort();

            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

            saveAs.value = winList;

            EndAction();
        }
    }

}
