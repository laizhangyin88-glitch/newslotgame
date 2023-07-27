using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public class EligibleBetHiddenUniverseFinder : EligibleBetItem
    {
        private int prevEligibleBetIndex = 0;

        public override bool Init()
        {
            base.Init();

            SetDefaultStringTableKey(
                "HIDDEN_OBJECTS_ELIGIBLE_BET_TITLE",
                "HIDDEN_OBJECTS_ELIGIBLE_BET_LESS_TEXT_2");

            var minEligibleBetList = BlackboardQueryUtils.GetMinEligibleBetList(ItemType.HIDDEN_UNIVERSE_FINDER);
            if(minEligibleBetList != null)
            {
                SetStringTableKey(
                    new BetTextInfo(minEligibleBetList[0], true, "HIDDEN_OBJECTS_ELIGIBLE_BET_TEXT_MEGA"),
                    new BetTextInfo(minEligibleBetList[1], true, "HIDDEN_OBJECTS_ELIGIBLE_BET_TEXT_EPIC"),
                    new BetTextInfo(minEligibleBetList[2], false, "HIDDEN_OBJECTS_ELIGIBLE_BET_TEXT_EPIC_MORE"));
            }

            return true;
        }

        protected override string GetInfo()
        {
            string textKey = isBetEnough ? betKey : betLessKey;
            bool useThreshold = !isBetEnough;

            long currentBet = BlackboardQueryUtils.GetTotalBet();

            if (isBetEnough && betTextInfoList.Count > 0)
            {
                foreach (var betTextInfo in betTextInfoList)
                {
                    if (currentBet >= betTextInfo.requireBet)
                    {
                        textKey = betTextInfo.textkey;
                        useThreshold = betTextInfo.useThreshold;
                    }
                    else break;
                }
            }
            else if (!isBetEnough)
            {
                textKey = betLessKey;
                useThreshold = true;
            }

            return useThreshold ?
                StringTableUtils.GetString(GLOBAL, textKey, GetThreshold()) :
                StringTableUtils.GetString(GLOBAL, textKey);
        }

        public override void UpdateBet(long totalBet)
        {
            if (!IsAvailableInternal()) return;

            long minBet = -1;

            var minEligibleBetList = BlackboardQueryUtils.GetMinEligibleBetList(ItemType.HIDDEN_UNIVERSE_FINDER);
            if (minEligibleBetList != null && minEligibleBetList.Count > 0)
                minBet = minEligibleBetList[0];

            if (minBet == -1L) return;

            bool prevBetEnough = isBetEnough;

            isBetEnough = minBet <= totalBet;

            isBetOver = !prevBetEnough && isBetEnough;
            isChanged = prevBetEnough != isBetEnough;

            int eligibleBetIndex = 0;
            for (int i = 0; i < minEligibleBetList.Count; ++i)
            {
                if (minEligibleBetList[i] <= totalBet)
                    eligibleBetIndex = i;
                else break;
            }

            bool isChangeEligibleBetIndex = prevEligibleBetIndex != eligibleBetIndex;
            if (prevEligibleBetIndex != eligibleBetIndex) prevEligibleBetIndex = eligibleBetIndex;

            isBonus = BlackboardUtils.FindVariable<bool>("./isGameSpin")?.value ?? false;

            // 처음 On되거나,
            // 기준 뱃보다 낮거나
            // Threshold 바뀌면 On
            isAvailable = isBetOver || totalBet < minBet || isChangeEligibleBetIndex && !isBonus;

            UpdateInfo();
        }

        protected override bool IsAvailableInternal()
        {
            var featureType = LockedFeatureType.HIDDEN_UNIVERSE;

            // Check min level
            int minLevel = BlackboardQueryUtils.GetFeatureMinLevel(featureType);
            int level = BlackboardQueryUtils.GetMyLevel();
            if (level < minLevel) return false;

            // Check feature available
            if (featureType == LockedFeatureType.HIDDEN_UNIVERSE &&
                !BlackboardQueryUtils.IsHiddenObjectsEnabled())
                return false;

            if (GetThreshold() == -1L)
                return false;

            return true;
        }

        public override long GetThreshold()
        {
            return BlackboardQueryUtils.GetNextEligibleBet(ItemType.HIDDEN_UNIVERSE_FINDER);
        }
    }
}
