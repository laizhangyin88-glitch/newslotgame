using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class CalcDiscontinuousLineWin : ActionTask<Blackboard>
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
        public BBParameter<bool> excludeMaxLine = false;

        [SerializeField] protected bool checkAll = true;
        [SerializeField] protected List<bool> masks;

        public SymbolAttribute ignoreSymbolAttribute = (SymbolAttribute)0;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<SymbolWin> winList;

        private const int wildSymbolIndex = 0;

        protected long betPerLine
        { get { return betCredit.value / baseWager.value; } }

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

        public long FindEarnCredit(int symbolIndex, int hitCount)
        {
            /// CRASH REPORT
            long result = 0L;
            try
            {
                result = payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new System.Exception(string.Format("FindEarnCreditException: gameTitle({0}), symbolIndex({1}), hitCount({2})", BlackboardUtils.FindVariable<string>("./game/gameTitle").value, symbolIndex, hitCount));
            }
            return result;
            /// CRASH REPORT
        }

        public long GetLineMultiplier(int symbolIndex, int hitCount, int wildHitCount)
        {
            long lineMultiplier = 1L;

            for (int i = 0; i < hitCount - wildHitCount; i++)
            {
                lineMultiplier = (long)OperationTools.Operate(lineMultiplier, GetSymbolMultiplier(symbolIndex), MultiplierOperation);
            }

            for (int i = 0; i < wildHitCount; i++)
            {
                lineMultiplier = (long)OperationTools.Operate(lineMultiplier, GetSymbolMultiplier(wildSymbolIndex), MultiplierOperation);
            }

            if (MultiplierOperation == OperationMethod.Add && lineMultiplier != 1L)
                --lineMultiplier;

            return lineMultiplier;
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

        private SymbolWin GetBiggestSymbolWin(Dictionary<int, SymbolWin> symbolWins, SymbolWin wildSymbolWin)
        {
            long maxEarnCredit = 0L;
            SymbolWin biggestSymbolWin = null;
            int wildHitCount = wildSymbolWin.hitCount;

            foreach (KeyValuePair<int, SymbolWin> pair in symbolWins)
            {
                SymbolWin symbolWin = pair.Value;

                long lineMultiplier = GetLineMultiplier(symbolWin.symbolIndex, symbolWin.hitCount, wildHitCount);

                long earnCredit = FindEarnCredit(symbolWin.symbolIndex, symbolWin.hitCount) * lineMultiplier;
                if (maxEarnCredit < earnCredit)
                {
                    maxEarnCredit = earnCredit;
                    biggestSymbolWin = symbolWin;
                }
            }
            return biggestSymbolWin;
        }

        protected long CalcOneLine(int lineIndex)
        {
            Dictionary<int, SymbolWin> symbolWins = new Dictionary<int, SymbolWin>();

            int wildHitCount = 0;
            List<int> line = GetPayLine(lineIndex);
            SymbolWin biggestSymbolWin;
            SymbolWin wildSymbolWin = new SymbolWin();
            wildSymbolWin.symbolIndex = wildSymbolIndex;

            for (int column = 0; column < this.column.value; ++column)
            {
                int dColumn = column;
                int row = line[dColumn];
                SymbolInfo symbolInfo = GetSymbolInfo(dColumn, row);
                // multiplier끼리 더하거나 곱할 수 있습니다.
                if ((int)ignoreSymbolAttribute != 0 && SymbolMask.HasAttribute(symbolInfo, ignoreSymbolAttribute))
                    continue;

                // prevent "reject" symbolInfo from being winSymbolInfo
                if (SymbolMask.HasReject(symbolInfo))
                    continue;

                if (SymbolMask.HasWild(symbolInfo))
                {
                    wildSymbolWin.cells.Add(new Cell(dColumn, row));
                    wildSymbolWin.hitCount++;
                }
                else if (!symbolWins.ContainsKey(symbolInfo.symbol))
                {
                    SymbolWin symbolWin = new SymbolWin();
                    symbolWin.cells.Add(new Cell(dColumn, row));
                    symbolWin.symbolIndex = symbolInfo.symbol;
                    symbolWin.hitCount = 1;

                    symbolWins.Add(symbolInfo.symbol, symbolWin);
                }
                else
                {
                    SymbolWin symbolWin = symbolWins[symbolInfo.symbol];
                    symbolWin.cells.Add(new Cell(dColumn, row));
                    symbolWin.symbolIndex = symbolInfo.symbol;
                    symbolWin.hitCount++;
                }
            }

            foreach (KeyValuePair<int, SymbolWin> pair in symbolWins)
            {
                SymbolWin symbolWin = pair.Value;
                for (int j = 0; j < wildSymbolWin.cells.Count; j++)
                {
                    symbolWin.cells.Add(wildSymbolWin.cells[j]);
                    symbolWin.hitCount++;
                }
            }

            biggestSymbolWin = GetBiggestSymbolWin(symbolWins, wildSymbolWin);

            if (biggestSymbolWin == null)
                return 0L;

            long earnCredit = FindEarnCredit(biggestSymbolWin.symbolIndex, biggestSymbolWin.hitCount);
            if (wildSymbolWin.hitCount > 0)
            {
                long wildEarnCredit = FindEarnCredit(wildSymbolWin.symbolIndex, wildSymbolWin.hitCount);
                if (wildEarnCredit > earnCredit)
                {
                    biggestSymbolWin = wildSymbolWin;
                    earnCredit = wildEarnCredit;
                }
            }

            if (biggestSymbolWin.hitCount == this.column.value && excludeMaxLine.value == true)
                return 0L;

            if (earnCredit > 0L)
            {
                long lineMultiplier = GetLineMultiplier(biggestSymbolWin.symbolIndex, biggestSymbolWin.hitCount, wildSymbolWin.hitCount);

                biggestSymbolWin.direction = 1;
                biggestSymbolWin.lineIndex = lineIndex + 1;
                biggestSymbolWin.multiplier = multiplier.value * lineMultiplier;
                biggestSymbolWin.earnCredit = earnCredit * betPerLine * biggestSymbolWin.multiplier;
                winList.Add(biggestSymbolWin);

                ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(biggestSymbolWin.cells);
            }

            return biggestSymbolWin.earnCredit;
        }

        protected override void OnExecute()
        {
            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();
            for (int i = 0; i < payLine.value.Count; ++i)
            {
                totalEarnCredit += CalcOneLine(i);
            }
            winList.Sort();

            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            BlackboardUtils.SetOrCreateValue<List<SymbolWin>>(spin, "winList", winList);
            ContentBlackboardUtils.AddEarnCredit(spin, totalEarnCredit);

            saveAs.value = winList;

            EndAction();
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();
        }

#endif
    }
}
