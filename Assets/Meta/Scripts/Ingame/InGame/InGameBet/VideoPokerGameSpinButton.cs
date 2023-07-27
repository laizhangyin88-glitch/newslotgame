using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class VideoPokerGameSpinButton : GameSpinButton 
    {
        private ContextElement textTotalBet;

        private Variable<int> handCount;
        private int HandCount
        {
            get { return handCount.value; }
            set { handCount.value = value; }
        }

        private void UpdateVideoPokerGameDealCount(long betPerHand)
        {
            Variable<Dictionary<long, int>> spinDict;
            switch(SpinType)
            {
                case SpinType.GameSpin:
                    spinDict = BlackboardUtils.FindVariable<Dictionary<long, int>>(ContentBlackboard.Get(), "gameSpinCountPerBet");
                    if (spinDict != null && spinDict.value.ContainsKey(betPerHand))
                    {
                        gameSpinCount.value = spinDict.value[betPerHand];
                    }
                    break;
                case SpinType.BonusSpin:
                    spinDict = BlackboardUtils.FindVariable<Dictionary<long, int>>(ContentBlackboard.Get(), "bonusSpinCountPerBet");
                    if (spinDict != null && spinDict.value.ContainsKey(betPerHand))
                    {
                        gameSpinCount.value = spinDict.value[betPerHand];
                    }
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            handCount = BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "handCount");

            var betPerHand = BlackboardUtils.GetOrCreateVariable<long>(ContentBlackboard.Get(), "betPerHand");
            UpdateVideoPokerGameDealCount(betPerHand.value);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void UpdateGameSpinCount(string name, object value)
        {
            ContextUtils.SetGlobalText(textGameSpinCount, "TEXT_COMMA_NUMBER", GameSpinCount);
            ContextUtils.SetGlobalText(textGameSpinCountPlural, "INGAME_GAME_DEAL_PLURAL_TEXT", GameSpinCount);
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();
            textTotalBet = root.Find("Text Total Bet");
        }
        
        public override void UpdatedTotalBetCredit(long totalCredit)
        {
            UpdateButtonState();

            if(IsGameSpin) 
            {
                object[] args = new object[]{totalCredit/(long)HandCount, (long)HandCount};
                ContextUtils.SetGlobalText(textTotalBet, "TEXT_VIDEO_POKER_BET_INFO", args);
                ContextUtils.SetGlobalText(textBetCredit, "TEXT_BET_CREDIT", totalCredit);                
            }

            UpdateVideoPokerGameDealCount(totalCredit/(long)HandCount);
        }
    }
}