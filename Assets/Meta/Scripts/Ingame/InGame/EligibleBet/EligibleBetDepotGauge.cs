using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace BagelCode
{
    public class EligibleBetDepotGauge : EligibleBetItem
    {

        public override bool Init()
        {
            base.Init();

            return true;
        }

        public override long GetThreshold()
        {
            return BlackboardUtils.FindValue<long>("./metaEligibleBetThreshold/vipLounge/buildDreamBetAmount");
        }

        protected override bool IsAvailableInternal()
        {
            return BlackboardQueryUtils.IsVegasDreamsActive() && BlackboardQueryUtils.IsVipLoungeEnabled();
        }

        public override void UpdateBet(long totalBet)
        {
            if (!IsAvailableInternal())
            {
                isAvailable = false;
                return;
            }

            long minBet = GetThreshold();

            bool prevBetEnough = isBetEnough;
            isBetEnough = minBet <= totalBet;

            isChanged = true;

            var minBetIndex = 0;
            var betList = BlackboardQueryUtils.GetBetList();
            for (int i = 0; i < betList.Count; i++)
            {
                minBetIndex = i;
                if (minBet < betList[i]) break;
            }

            var maxRefineIndex = betList.Count - minBetIndex;
            var currentRefineIndex = BlackboardQueryUtils.GetBetIndex() - minBetIndex + 1;

            MetaContextElementUtils.SimpleSetFloatProperty(root, "Progress Bar", (float)Mathf.Max(0, currentRefineIndex) / maxRefineIndex);

            isBonus = BlackboardUtils.FindVariable<bool>("./isGameSpin")?.value ?? false;

            isAvailable = isBetEnough && !isBonus;
        }

        protected override void UpdateAnim()
        {
            long totalBet = BlackboardQueryUtils.GetTotalBet();
        }

        protected override string GetTitle()
        {
            return string.Empty;
        }

        protected override string GetInfo()
        {
            return string.Empty;
        }
    }
}
