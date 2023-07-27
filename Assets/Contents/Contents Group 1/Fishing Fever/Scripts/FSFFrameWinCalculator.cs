using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace GameStudio.Slot.FSF
{
    public class FSFFrameWinCalculator : MonoBehaviour
    {
        public const string ON_CONTENT_UI_DETAIL_EVENT = "OnContentUIDetailEvent";
        public const string FRAME_WIN_CALC_EVENT = "CalcFrameWin";

        public const int JACKPOT_SYMBOL_INDEX = 0;
        public const int JACKPOT_BONUS_ID = 20600;

        private void OnEnable()
        {
            MessageDispatcher.Register(ON_CONTENT_UI_DETAIL_EVENT, ContentUIDetailEventDelegator);
        }


        private void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_CONTENT_UI_DETAIL_EVENT, ContentUIDetailEventDelegator);
        }

        private void ContentUIDetailEventDelegator(EventData eventData)
        {
            if (eventData.name == FRAME_WIN_CALC_EVENT && eventData.value is IBlackboard)
            {
                List<SymbolWin> winList = CalcFrameWin(FSFWinFrameController.GetFrameFromBlackboard(eventData.value as IBlackboard));
                BlackboardUtils.GetOrCreateVariable<List<SymbolWin>>(null, "./customData/winList").value = winList;
            }
        }

        private List<SymbolWin> CalcFrameWin(Frame frame)
        {
            var payTables = BlackboardUtils.FindValue<List<Blackboard>>(null, "./game/paytables")[0].GetValue<List<Blackboard>>("value"); ;
            var baseWager = BlackboardUtils.FindValue<long>(null, "./game/baseWager");
            var baseBet = BlackboardUtils.FindValue<long>(null, "./betCredit");
            var deck = ContentCustomData.Instance.slotDataList[0].deck.deck;
            var winList = new List<SymbolWin>();

            HashSet<int> winSymbolIndexSet = new HashSet<int>();
            for (int colIndex = frame.column; colIndex < frame.column + frame.width; colIndex++)
            {
                for (int rowIndex = frame.row - frame.height + 1; rowIndex <= frame.row; rowIndex++)
                {
                    int symbolIndex = deck[colIndex][rowIndex].symbol;
                    List<long> payList = payTables[symbolIndex].GetValue<List<long>>("value");
                    bool hasPay = payList[0] > 0L;
                    if (hasPay && winSymbolIndexSet.Contains(symbolIndex) == false)
                    {
                        winSymbolIndexSet.Add(symbolIndex);
                        winList.Add(CalcSpecificSymbolWin(frame,deck, payList, symbolIndex, baseBet, baseWager));
                    }
                    else if(symbolIndex == JACKPOT_SYMBOL_INDEX)
                    {
                        winSymbolIndexSet.Add(JACKPOT_SYMBOL_INDEX);
                        winList.AddRange(GetJackpotSymbolWinList(frame,deck));
                    }
                }
            }

            return winList;
        }

        private SymbolWin CalcSpecificSymbolWin(Frame frame, List<List<SymbolInfo>> deck, List<long> payList, int targetSymbolIndex, long betCredit, long baseWager)
        {
            var symbolWin = new SymbolWin();
        List<Cell> hitCellList = new List<Cell>();

            for (int colIndex = frame.column; colIndex < frame.column + frame.width; colIndex++)
                for (int rowIndex = frame.row; rowIndex > frame.row - frame.height; rowIndex--)
                {
                    int symbolIndex = deck[colIndex][rowIndex].symbol;
                    if (symbolIndex == targetSymbolIndex)
                    {
                        hitCellList.Add(new Cell(colIndex, rowIndex));
                    }
                }

            long creditByWager = betCredit / baseWager;
            long payMultplier = payList[0];

            symbolWin.cells = hitCellList;
            symbolWin.earnCredit = creditByWager * payMultplier * hitCellList.Count;
            symbolWin.hitCount = hitCellList.Count;
            symbolWin.symbolIndex = targetSymbolIndex;

            ContentCustomData.GetSlotData(0).deck.UpdateHitMap(symbolWin.cells);
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin").value;
            ContentBlackboardUtils.AddEarnCredit(spin, symbolWin.earnCredit);

            return symbolWin;
        }

        private List<SymbolWin> GetJackpotSymbolWinList(Frame frame, List<List<SymbolInfo>> deck)
        {
            var spin = BlackboardUtils.FindVariable<Blackboard>(null, "./spin");
            var responseList  = ContentBlackboardUtils.GetBonusResponseList(spin.value, JACKPOT_BONUS_ID);
            var symbolWinList = new List<SymbolWin>(responseList.Count);

            int responseIndex = 0;
            for (int colIndex = frame.column ; colIndex < frame.column + frame.width  ; colIndex++)
                for (int rowIndex = frame.row - frame.height + 1; rowIndex <= frame.row; rowIndex++)
                {
                    int symbolIndex = deck[colIndex][rowIndex].symbol;
                    if (symbolIndex == JACKPOT_SYMBOL_INDEX)
                    {
                        var response = responseList[responseIndex++];

                        var symbolWin = new SymbolWin();
                        symbolWin.symbolIndex = JACKPOT_SYMBOL_INDEX;
                        symbolWin.cells = new List<Cell>();
                        symbolWin.cells.Add(new Cell(colIndex,rowIndex));
                        symbolWin.earnCredit = response.GetValue<long>("earnCredit");
                        symbolWin.hitCount = 1;
                        // use line index as jackpot index for signboard
                        symbolWin.lineIndex = response.GetValue<int>("jackpotIndex");

                        symbolWinList.Add(symbolWin);
                    }
                }

            return symbolWinList;
        }
    }
}
