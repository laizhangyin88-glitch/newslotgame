using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class VideoPokerBetButton : BetButton 
    {
        private ContextElement textTotalBet;

        private Variable<int> handCount;
        private int HandCount
        {
            get { return handCount.value; }
            set { handCount.value = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            handCount = BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "handCount");
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();
            textTotalBet = root.Find("Text Total Bet");
        }

        public override void UpdatedTotalBetCredit(long totalCredit)
        {
            base.UpdatedTotalBetCredit(totalCredit);

            object[] args = new object[]{totalCredit/(long)HandCount, (long)HandCount};

            ContextUtils.SetGlobalText(textTotalBet, "TEXT_VIDEO_POKER_BET_INFO", args);
        }
    }
}
