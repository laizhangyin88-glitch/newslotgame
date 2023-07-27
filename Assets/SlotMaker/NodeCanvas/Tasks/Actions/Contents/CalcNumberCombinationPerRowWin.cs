using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.LineWin
{
    [Category("✶ Slots/Win")]
    public class CalcNumberCombinationPerRowWin : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> row = 0;
        public BBParameter<int> column = 0;
        public BBParameter<long> baseBet = 0;
        public BBParameter<long> baseWager = 0;
        public BBParameter<long> totalMultiplier = 1L;
        public BBParameter<List<long>> multiplierPerRow = new List<long>();
        public BBParameter<bool> checkAll = true;
        [BlackboardOnly]
        public BBParameter<List<bool>> masks = new List<bool>();
        public WinningCombination.CombinationRule combinationRule = WinningCombination.CombinationRule.LeftToRight;
        public BBParameter<List<WinningCombination.CombinationRule>> combinationRulePerRow = new List<WinningCombination.CombinationRule>();

        public BBParameter<List<int>> numberValuePerSymbol = new List<int>();
        public BBParameter<List<int>> digitNumberPerSymbol = new List<int>();
        public BBParameter<List<bool>> ignoreFlagPerSymbol = new List<bool>();

        [BlackboardOnly]
        public BBParameter<List<SymbolWin>> saveAs;

        protected override void OnExecute()
        {
            SlotData slotData = ContentCustomData.GetSlotData(slotIndex.value);

            var winList = new List<SymbolWin>();
            long betPerLine = baseBet.value / baseWager.value;
            Deck deck = slotData.deck;

            for (int rowIndex = 0; rowIndex < row.value; rowIndex++)
            {
                if (checkAll.value == false && masks.value[rowIndex] == false)
                    continue;

                var winInfo = CalcRowWin(deck, rowIndex, betPerLine);
                if (winInfo.earnCredit > 0)
                {
                    winList.Add(winInfo);
                    ContentCustomData.GetSlotData(slotIndex.value).deck.UpdateHitMap(winInfo.cells);
                }
            }

            saveAs.value = winList;
            EndAction();
        }

        protected SymbolWin CalcRowWin(Deck deck, int rowIndex, long betPerLine)
        {
            int startColIndex = 0;
            int indexAdder = 1;
            int escapeIndex = column.value;

            var combinationRule = GetCombinationRuleAtRow(rowIndex);
            if (combinationRule == WinningCombination.CombinationRule.RightToLeft)
            {
                startColIndex = column.value - 1;
                indexAdder = -1;
                escapeIndex = -1;
            }

            long digit = 1;
            long earnCredit = 0;
            List<Cell> cellList = new List<Cell>();

            for (int colIndex = startColIndex; colIndex != escapeIndex; colIndex += indexAdder)
            {
                var symbol = deck.GetSymbol(colIndex, rowIndex);
                bool isIgnoreSymbol = ignoreFlagPerSymbol.value.Count > symbol.symbol && ignoreFlagPerSymbol.value[symbol.symbol];
                if (SymbolMask.HasAttribute(symbol, SymbolAttribute.Blank) || isIgnoreSymbol)
                    continue;

                int symbolIndex = symbol.symbol;
                if (numberValuePerSymbol.value[symbolIndex] > 0)
                {
                    long number = numberValuePerSymbol.value[symbolIndex];
                    earnCredit += number * digit * betPerLine;
                }

                cellList.Add(new Cell(colIndex, rowIndex));
                var digitMultiplier = System.Math.Pow(10, digitNumberPerSymbol.value[symbolIndex]);
                digit *= System.Convert.ToInt64(digitMultiplier);
            }

            DeleteUnnecessaryZeroCell(cellList, deck);
            var winInfo = new SymbolWin();
            winInfo.multiplier = totalMultiplier.value * GetMultiplierAtRow(rowIndex);
            winInfo.earnCredit = earnCredit * winInfo.multiplier;
            winInfo.cells = cellList;
            winInfo.lineIndex = rowIndex + 1;

            return winInfo;
        }

        protected WinningCombination.CombinationRule GetCombinationRuleAtRow(int rowIndex)
        {
            if (combinationRulePerRow != null && combinationRulePerRow.value.Count > rowIndex)
                return combinationRulePerRow.value[rowIndex];
            else
                return combinationRule;
        }

        protected long GetMultiplierAtRow(int rowIndex)
        {
            if (multiplierPerRow != null && multiplierPerRow.value.Count > rowIndex)
                return multiplierPerRow.value[rowIndex];
            else
                return 1;
        }

        // for combination right to left
        protected void DeleteUnnecessaryZeroCell(List<Cell> winCellList, Deck deck)
        {
            for(int i= winCellList.Count -1;i >= 0;i--)
            {
                var cell = winCellList[i];
               int symbolIndex = deck.GetSymbol(cell.column, cell.row).symbol;
                if (numberValuePerSymbol.value[symbolIndex] > 0)
                    break;
                else
                    winCellList.RemoveAt(i);
            }
        }
    }
}