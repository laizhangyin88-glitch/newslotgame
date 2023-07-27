using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class VideoPokerInGameBetController : InGameBetController
    {
        private Variable<long> betPerHand;
        public long BetPerHand
        {
            get { return betPerHand.value; }
            set { betPerHand.value = value; }
        }

        private Variable<int> handCount;
        private int HandCount
        {
            get { return handCount.value; }
            set { handCount.value = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            betPerHand = BlackboardUtils.GetOrCreateVariable<long>(ContentBlackboard.Get(), "betPerHand");
            handCount = BlackboardUtils.GetOrCreateVariable<int>(ContentBlackboard.Get(), "handCount");

            var handsGroupIndex = BlackboardUtils.GetOrCreateVariable<int>("./handsGroupIndex");
            var handList = BlackboardUtils.FindVariable<List<int>>("./game/handList");
            HandCount = handList.value[handsGroupIndex.value];
        }

        public override void InitGame()
        {
            base.InitGame();
        }

        public override void EnterTurn()
        {
            base.EnterTurn();
        }

        public override void ExitTurn()
        {
            base.ExitTurn();
        }

        public override void BroadcastUpdateBetCreditEvent(long bet)
        {
            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdateBetCredit", bet * HandCount));
        }

        public override void UpdateBetCredit(long credit)
        {
            BetCredit = credit;
            BetPerHand = credit / (long)HandCount;
            ExtraBetCredit = GetExtraBetCredit(credit);
            TotalBetCredit = BetCredit + ExtraBetCredit;

            if (BetList.Contains(BetPerHand))
            {
                PlayerPrefs.SetString("LAST_BET_CREDIT", BetCredit.ToString());
                PlayerPrefs.SetString("LAST_EXTRA_BET_CREDIT", ExtraBetCredit.ToString());
            }

            MessageDispatcher.Dispatch("OnCreditEvent", new EventData<long>("UpdatedTotalBetCredit", TotalBetCredit));

            UpdateLevelRestriction(TotalBetCredit);
        }

        protected override void UpdateLevelRestriction(long bet)
        {
            bet = bet / (long)HandCount;
            base.UpdateLevelRestriction(bet);
        }

        protected override void UpdateMaxBetIndex()
        {
            MaxBetIndex = 0;
            for (int i = 0; i < BetList.Count; ++i)
            {
                if (Level < GetBetToLevelRestriction(BetList[i]))
                    break;

                MaxBetIndex = i;
            }

            MaxBetCredit = BetList[MaxBetIndex] * HandCount;
        }

        public void UpdateHandCount(int hands)
        {
            BroadcastUpdateBetCreditEvent(BetPerHand);

            UpdateMaxBetIndex();
        }
    }
}
