using System.Collections.Generic;
using BagelCode.ClientModels;
using UnityEngine;
using SlotMaker;
using System.Linq;

namespace BagelCode
{
    public class EligibleBetJackpot : EligibleBetItem
    {
        private int prevEligibleBetIndex = 0;

        private List<long> baseEligibleMinBetList;

        [SerializeField] private List<JackpotType> jackpotTypeList;
        [SerializeField] private List<long> eligibleMinBetList;

        private bool isBetLast = false;

        public override bool Init()
        {
            base.Init();

            var jackpotMinBetList = BlackboardQueryUtils.GetJackpotMinBetList(); // extraBet 적용 안된 값
            jackpotTypeList = BlackboardQueryUtils.GetJackpotTypeList();

            /// Exception
            if (jackpotMinBetList == null || jackpotTypeList == null || jackpotTypeList.Count != jackpotMinBetList.Count)
            {
                if (ApplicationSettings.LogTest())
                {
                    Debug.Log("jackpotTypeList.Count: " + (jackpotTypeList?.Count ?? 0));
                    Debug.Log("jackpotMinBetList.Count: " + (jackpotMinBetList?.Count ?? 0));
                }
                return false;
            }

            // jackpot list 와 bet list 매칭
            baseEligibleMinBetList = new List<long>(jackpotMinBetList);
            baseEligibleMinBetList.Sort();

            long minBet = BlackboardQueryUtils.GetBetList()[0];
            int i = 0;
            while (baseEligibleMinBetList.IsValidIndex(i))
            {
                 if(baseEligibleMinBetList[i] <= minBet)
                {
                    baseEligibleMinBetList.RemoveAt(i);
                    jackpotTypeList.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            UpdateEligibleMinBetList();

            return true;
        }

        private void UpdateEligibleMinBetList()
        {
            var betList = BlackboardQueryUtils.GetBetList();
            eligibleMinBetList = new List<long>();

            int j = 0; // LastBetIndex
            for (int i = 0; i < baseEligibleMinBetList.Count; ++i)
            {
                long minBet = baseEligibleMinBetList[i];

                for (; j < betList.Count; ++j)
                {
                    long lastBet = betList[j];
                    if (lastBet >= minBet)
                    {
                        eligibleMinBetList.Add(BlackboardQueryUtils.GetTotalBet(j));
                        break;
                    }
                }
            }

            long totalBet = BlackboardQueryUtils.GetTotalBet();
            isBetLast = totalBet >= eligibleMinBetList.LastOrDefault();

            // Update Texts
            if (isBetEnough)
            {
                UpdateTitle();
                UpdateInfo();
            }
        }

        public override void UpdateExtraBetIndex(int index)
        {
            UpdateEligibleMinBetList();
        }

        protected override bool IsAvailableInternal()
        {
            return jackpotTypeList != null && jackpotTypeList.Count > 0;
        }

        public override void UpdateBet(long totalBet)
        {
            if (!IsAvailableInternal()) return;

            if (eligibleMinBetList == null || eligibleMinBetList.Count == 0) return;

            long minBet = eligibleMinBetList[0];
            if (minBet == -1L) return;

            bool prevBetEnough = isBetEnough;

            isBetEnough = minBet <= totalBet;

            isBetOver = !prevBetEnough && isBetEnough;
            isChanged = prevBetEnough != isBetEnough;

            isBetLast = totalBet >= eligibleMinBetList.LastOrDefault();

            int eligibleBetIndex = 0;
            for (int i = 0; i < eligibleMinBetList.Count; ++i)
            {
                if (eligibleMinBetList[i] <= totalBet)
                    eligibleBetIndex = i;
                else break;
            }

            bool isChangeEligibleBetIndex = prevEligibleBetIndex != eligibleBetIndex;
            if (prevEligibleBetIndex != eligibleBetIndex) prevEligibleBetIndex = eligibleBetIndex;

            isBonus = BlackboardUtils.FindVariable<bool>("./isGameSpin")?.value ?? false;

            // enough 상태 이면서
            // 처음 on 되었거나
            // Threshold 바뀌면 On
            isAvailable = isBetEnough && (isBetOver || isChangeEligibleBetIndex) && !isBonus;

            // Update Texts
            if (isAvailable)
            {
                UpdateTitle();
                UpdateInfo();
            }
        }

        protected override string GetTitle()
        {
            JackpotType type = CurrentJackpotType();
            string key = string.Format("META_{0}_JACKPOT_ELIGIBLE_TITLE", type.ToString());
            return StringTableUtils.GetString(GLOBAL, key);
        }

        protected override string GetInfo()
        {
            JackpotType type = NextJackpotType();
            string key1, key2;

            if (!isBetLast)
            {
                key1 = "META_JACKPOT_ELIGIBLE_RAISE";
                key2 = string.Format("META_JACKPOT_ELIGIBLE_TO_GET_{0}", type.ToString());
                return StringTableUtils.GetString(GLOBAL, key1, GetThreshold()) + "\n" +
                    StringTableUtils.GetString(GLOBAL, key2);
            }
            else
            {
                key1 = "META_JACKPOT_ELIGIBLE_QUALIFIED";
                return StringTableUtils.GetString(GLOBAL, key1);
            }
        }

        public override long GetThreshold()
        {
            return NextEligibleBet();
        }

        private JackpotType CurrentJackpotType()
        {
            int i = NextEligibleIndex() - 1;
            return jackpotTypeList.IsValidIndex(i) ? jackpotTypeList[i] : JackpotType.MINI;
        }

        private JackpotType NextJackpotType()
        {
            int i = NextEligibleIndex();
            return jackpotTypeList.IsValidIndex(i) ? jackpotTypeList[i] : JackpotType.MINI;
        }

        private long NextEligibleBet()
        {
            int i = NextEligibleIndex();
            return eligibleMinBetList.IsValidIndex(i) ? eligibleMinBetList[i] : -1;
        }

        private int NextEligibleIndex()
        {
            long currentBet = BlackboardQueryUtils.GetTotalBet();

            if (eligibleMinBetList != null && eligibleMinBetList.Count > 0)
            {
                for (int i = 0; i < eligibleMinBetList.Count; ++i)
                {
                    long requireBet = eligibleMinBetList[i];
                    if (currentBet < requireBet)
                    {
                        return i;
                    }
                }
                return eligibleMinBetList.Count;
            }

            return -1;
        }
    }
}
