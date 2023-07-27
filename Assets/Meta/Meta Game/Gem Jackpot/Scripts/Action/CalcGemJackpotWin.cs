using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/GemJackpot")]
    public class CalcGemJackpotWin : ActionTask<Blackboard>
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<List<int>> symbolIndices;

        public BBParameter<List<SymbolWin>> saveAs;

        protected List<SymbolWin> winList;

        protected SymbolInfo GetSymbolInfo(int column, int row)
        {
            return MetaSlotMachineContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        protected override void OnExecute()
        {
            winList = new List<SymbolWin>();

            SymbolWin coinWin = new SymbolWin();
            SymbolWin bonusWin = new SymbolWin();

            for (int count = 0; count < symbolIndices.value.Count; ++count)
            {
                int column = count;
                int row = 0;

                var symbolInfo = GetSymbolInfo(column, row);
                if (symbolInfo.symbol == 1)
                {
                    coinWin.cells.Add(new Cell(column, row));
                    ++coinWin.hitCount;
                }
                else if (symbolInfo.symbol == 3)
                {
                    bonusWin.cells.Add(new Cell(column, row));
                    ++bonusWin.hitCount;
                }
            }
            // Coin win check
            if (coinWin.cells.Count > 0)
            {
                coinWin.direction = 1;
                coinWin.symbolIndex = 1;
                winList.Add(coinWin);
            }
            // Bonus win check
            if (bonusWin.cells.Count == 5)
            {
                bonusWin.direction = 1;
                bonusWin.symbolIndex = 3;
                winList.Add(bonusWin);

            }

            saveAs.value = winList;
            EndAction();
        }
    }
}