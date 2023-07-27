using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public static class MultiplierUtils
    {
        public static long GetRewardMultiplierValue(long origin, Blackboard rewardBB, string typeValue)
        {
            if (string.IsNullOrEmpty(typeValue) || rewardBB == null)
            {
                Debug.LogWarning("MultiplierUtils.GetRewardMultiplierValue failure. typeValue:" + typeValue + ", rewardBB:" + rewardBB);
                return origin;
            }

            int version = BlackboardUtils.FindVariable<int>(rewardBB, "version")?.value ?? 1;
            if (version == 0)
            {
                return 0L;
            }

            long result = origin;

            bool isApplyLevelMultiplier = BlackboardUtils.FindVariable<bool>(rewardBB, "applyLevelMultiplier")?.value ?? true;
            var levelMultiplierNumeratorVariable = BlackboardUtils.FindVariable<long>(rewardBB, "levelMultiplierNumerator");
            if (isApplyLevelMultiplier || levelMultiplierNumeratorVariable != null)
            {
                if(levelMultiplierNumeratorVariable == null)
                {
                    result = LevelUtils.GetLevelMultiplierNumeratorValue(result, typeValue);
                }
                else
                {
                    result = NumberUtils.GetMultiplierNumeratorValue(result, levelMultiplierNumeratorVariable.value);
                }
            }

            bool isApplyTierMultiplier = BlackboardUtils.FindVariable<bool>(rewardBB, "applyTierMultiplier")?.value ?? true;
            var tierMultiplierNumeratorVariable = BlackboardUtils.FindVariable<long>(rewardBB, "tierMultiplierNumerator");
            if (isApplyTierMultiplier || tierMultiplierNumeratorVariable != null)
            {
                if(tierMultiplierNumeratorVariable == null)
                {
                    result = TierUtils.GetTierFractionCoin(result, TierUtils.GetMeTier());
                }
                else
                {
                    result = NumberUtils.GetMultiplierNumeratorValue(result, tierMultiplierNumeratorVariable.value);
                }
            }

            long scratcherMultiplierNumerator = BlackboardUtils.FindVariable<long>(rewardBB, "scratcherMultiplierNumerator")?.value ?? 100L;
            result = NumberUtils.GetMultiplierNumeratorValue(result, scratcherMultiplierNumerator);

            return result;
        }
    }
}
