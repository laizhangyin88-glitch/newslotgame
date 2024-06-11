using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class CalcSpotWin : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<int> reelCount;
        public BBParameter<int> symbolCount;
        public BBParameter<long> betCredit;
        public BBParameter<long> baseWager;
        public BBParameter<List<Blackboard>> payTable;
        public BBParameter<long> multiplier;
        public BBParameter<List<long>> symbolMultipliers;

        public BBParameter<List<int>> symbolIndices;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<SymbolWin> winList;

        protected long betPerSymbol
        { get { return betCredit.value / baseWager.value; } }

        private SymbolMask mask;

        protected SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        protected long GetSymbolMultiplier(int symbolIndex)
        {
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
            long earnCredit = payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
            return payTable.value[symbolIndex].GetValue<List<long>>("value")[hitCount - 1];
        }

        protected long CalcOneSymbol(int symbolIndex)
        {
            // long totalEarnCredit = 0L;
            int direction = 1;
            var symbolWin = new SymbolWin();
            var winSymbolInfo = SlotUtils.CreateSymbolInfo(symbolIndex, mask);
            int hitCount = 0;

            for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
            {
                int column = reelIndex % this.column.value;
                int row = reelIndex / this.column.value;

                var symbolInfo = GetSymbolInfo(column, row);
                if (SymbolMask.HasWild(winSymbolInfo) || IsHit(ref winSymbolInfo, symbolInfo))
                {
                    ++hitCount;
                    symbolWin.cells.Add(new Cell(column, row));
                }
            }

            if (hitCount <= 0)
                return 0L;

            long earnCredit = FindEarnCredit(symbolIndex, hitCount);
            if (earnCredit > 0)
            {
                symbolWin.symbolIndex = symbolIndex;
                symbolWin.direction = direction;
                symbolWin.hitCount = hitCount;
                symbolWin.wayCount = null;
                symbolWin.multiplier = multiplier.value * GetSymbolMultiplier(symbolIndex);
                symbolWin.earnCredit = earnCredit * betPerSymbol * symbolWin.multiplier;

                winList.Add(symbolWin);
            }

            return symbolWin.earnCredit;
        }

        protected override void OnExecute()
        {
            mask = ContentCustomData.GetSlotData(slotIndex.value).symbolMask;
            long totalEarnCredit = 0L;
            winList = new List<SymbolWin>();

            for (int count = 0; count < symbolIndices.value.Count; ++count)
            {
                int symbolIndex = symbolIndices.value[count];
                totalEarnCredit += CalcOneSymbol(symbolIndex);
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
