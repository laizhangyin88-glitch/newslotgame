using UnityEngine;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static class LevelUtils
    {
        private static int maxLevel = -1;
        private static int expPerCredit = -1;

        private static List<long> requiredExpTable = new List<long>();
        private static List<long> expPerSpinTable = new List<long>();
        private static List<long> maxExpCapTable = new List<long>();

        private static List<Blackboard> levelMultiplierNumeratorTable = new List<Blackboard>();
        private static Blackboard previousLevelMultiplierNumeratorInfoBB = null;
        private static Blackboard currentLevelMultiplierNumeratorInfoBB = null;
        private static Blackboard nextLevelMultiplierNumeratorInfoBB = null;
        private static int calculateLevel = -1;

        private static readonly string SHOP_MULTIPLIER_LOGIN_COUNT = "SHOP_MULTIPLIER_LOGIN_COUNT";
        private static readonly string SHOP_MULTIPLIER_CHECK_TIME = "SHOP_MULTIPLIER_CHECK_TIME";

        public static void Clear()
        {
            maxLevel = -1;
            expPerCredit = -1;
            calculateLevel = -1;

            requiredExpTable.Clear();
            expPerSpinTable.Clear();
            maxExpCapTable.Clear();
            levelMultiplierNumeratorTable.Clear();

            previousLevelMultiplierNumeratorInfoBB = null;
            currentLevelMultiplierNumeratorInfoBB = null;
            nextLevelMultiplierNumeratorInfoBB = null;
        }

        public static int GetMaxLevel()
        {
            if(maxLevel < 1)
                maxLevel = BlackboardUtils.FindVariable<int>(null, "/values/level/MAX_LEVEL").value;

            return maxLevel;
        }

        public static long GetRequiredExp(int level)
        {
            if (GetMaxLevel() == level) return 0;

            if(requiredExpTable.Count == 0)
                requiredExpTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/level/REQUIRED_EXP_TABLE").value;

            return requiredExpTable[level];
        }

        public static long GetEarnExp(long spentCredit)
        {
            if(expPerCredit < 0)
                expPerCredit = BlackboardUtils.FindVariable<int>(null, "/values/level/EXP_PER_CREDIT").value;

            return (long)expPerCredit * spentCredit;
        }

        public static long GetAdditionalExp(int level)
        {
            if(level < 0) return 0;
            if(GetMaxLevel() == level) return 0;

            if(expPerSpinTable.Count == 0)
                expPerSpinTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/level/EXP_PER_SPIN").value;

            return expPerSpinTable[level];
        }

        public static long GetMaxEarnExp(int level, long earnExp, double eventMultiplier)
        {
            if(level < 0) return 0;
            if(GetMaxLevel() == level) return 0;
            long levelMultiplier = GetLevelMultiplierNumeratorFromType("maxExpCap");

            if(maxExpCapTable.Count == 0)
                maxExpCapTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/level/MAX_EXP_CAP").value;

            long maxCapExp = (long)((double)maxExpCapTable[level] * eventMultiplier);
            maxCapExp = NumberUtils.GetMultiplierNumeratorValue(maxCapExp, levelMultiplier);

            if(maxCapExp < earnExp)
            {
                // Debug.LogError(string.Format("Earn EXP : {0} -> {1}", earnExp, maxCapExp));
                return maxCapExp;
            }

            // Debug.LogError(string.Format("Earn EXP : {0}", earnExp));

            return earnExp;
        }

        public static long GetLevelCreditBonus(int level)
        {
            return GetLevelBonusReward(level, "/values/level/CREDIT_BONUS");
        }

        public static long GetLevelRpBonus(int level)
        {
            return GetLevelBonusReward(level, "/values/level/RP_BONUS");
        }

        public static long GetLevelGemBonus(int level)
        {
            return GetLevelBonusReward(level, "/values/level/GEM_BONUS");
        }

        public static long GetLevelBonusReward(int level, string key)
        {
            if (level < 0 || GetMaxLevel() == level) return 0L;

            List<long> rewardTable = BlackboardUtils.FindVariable<List<long>>(null, key)?.value ?? null;
            if (rewardTable == null || rewardTable.Count < level) return 0L;

            return rewardTable[level];
        }

        public static void CalculateLevelMultiplierNumeratorInfoBB(int level)
        {
            if( calculateLevel == level ) return;
            calculateLevel = level;

            if( levelMultiplierNumeratorTable.Count == 0 )
                levelMultiplierNumeratorTable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/values/level/LEVEL_MULTIPLIER_NEW_NUMERATOR_TABLE").value;

            for (int i = 0; i < levelMultiplierNumeratorTable.Count; ++i)
            {
                if (level >= levelMultiplierNumeratorTable[i].GetValue<int>("level"))
                {
                    currentLevelMultiplierNumeratorInfoBB = levelMultiplierNumeratorTable[i];

                    if(levelMultiplierNumeratorTable.Count > i+1)
                        nextLevelMultiplierNumeratorInfoBB = levelMultiplierNumeratorTable[i+1];
                    else
                        nextLevelMultiplierNumeratorInfoBB = null;

                    if(i > 0)
                        previousLevelMultiplierNumeratorInfoBB = levelMultiplierNumeratorTable[i-1];
                    else
                        previousLevelMultiplierNumeratorInfoBB = null;
                }
                else
                    break;
            }
        }

        public static Blackboard GetLevelMultiplierNumeratorInfoBB(int level)
        {
            if(currentLevelMultiplierNumeratorInfoBB == null)
                CalculateLevelMultiplierNumeratorInfoBB(level);
            else if(nextLevelMultiplierNumeratorInfoBB != null && level >= nextLevelMultiplierNumeratorInfoBB.GetValue<int>("level"))
                CalculateLevelMultiplierNumeratorInfoBB(level);

            return currentLevelMultiplierNumeratorInfoBB;
        }

        public static long GetLevelMultiplierNumerator(string typeValue)
        {
            return GetLevelMultiplierNumerator(BlackboardQueryUtils.GetMyLevel(), typeValue);
        }

        public static long GetLevelMultiplierNumerator(int level, string typeValue)
        {
            if (level < 0) return 0L;

            return GetLevelMultiplierNumeratorInfoBB(level)?.GetVariable<long>(typeValue)?.value ?? 0L;
        }

        public static long GetPriviousLevelMultiplierNumerator(string typeValue)
        {
            if(previousLevelMultiplierNumeratorInfoBB == null)
                CalculateLevelMultiplierNumeratorInfoBB(BlackboardQueryUtils.GetMyLevel());

            return previousLevelMultiplierNumeratorInfoBB?.GetVariable<long>(typeValue)?.value ?? 0L;
        }

        public static long GetLevelMultiplierNumeratorFromType(string typeValue)
        {
            return LevelUtils.GetLevelMultiplierNumeratorInfoBB(BlackboardQueryUtils.GetMyLevel())?.GetVariable<long>(typeValue)?.value ?? 0L;
        }

        public static long GetLevelMultiplierNumeratorFromPriviousSection(string typeValue)
        {
            if(previousLevelMultiplierNumeratorInfoBB == null)
                CalculateLevelMultiplierNumeratorInfoBB(BlackboardQueryUtils.GetMyLevel());

            long currentNumerator = LevelUtils.GetLevelMultiplierNumeratorInfoBB(BlackboardQueryUtils.GetMyLevel())?.GetVariable<long>(typeValue)?.value ?? 0L;
            if(currentNumerator <= 0L) return 0L;

            if( previousLevelMultiplierNumeratorInfoBB == null)
                return currentNumerator;

            long priviousNumerator = previousLevelMultiplierNumeratorInfoBB?.GetVariable<long>(typeValue)?.value ?? 0L;

            if( currentNumerator <= 0L)
                return currentNumerator;

            return NumberUtils.GetDevideNumeratorValue(currentNumerator, priviousNumerator);
        }

        public static double GetLevelMultiplierFromPreviousSection(string typeValue)
        {
            return NumberUtils.GetMultiplierFromNumerator( GetLevelMultiplierNumeratorFromPriviousSection(typeValue) );
        }

        public static string GetLevelMultiplierStringFromPreviousSection(string typeValue)
        {
            return string.Format("{0:0.#}", GetLevelMultiplierFromPreviousSection(typeValue) - 0.05);
        }

        public static string GetLevelMultiplierPromotionFormatStringFromPreviousSection(string typeValue)
        {
            var format = LevelUtils.GetLevelMultiplierNumeratorInfoBB(BlackboardQueryUtils.GetMyLevel())?.GetValue<PromotionFormat>("promotionFormat");

            switch (format)
            {
                case PromotionFormat.NUM:
                    return $"{GetLevelMultiplierStringFromPreviousSection(typeValue)}X";
                case PromotionFormat.PERCENTAGE:
                    return $"+{(long)(System.Math.Max((GetLevelMultiplierFromPreviousSection(typeValue) - 1.0), 0.0) * 100)}%";
                default:
                case PromotionFormat.UNKNOWN:
                    Debug.LogError("UNKNOWN PROMOTION FORMAT");
                    return $"{GetLevelMultiplierStringFromPreviousSection(typeValue)}X";
            }
        }

        public static long GetCurrentLevelMultiplierNumerator(string typeValue)
        {
            long multiplierNumerator = 0L;

            int currentLevel = BlackboardQueryUtils.GetMyLevel();
            if (currentLevel < 0 || !CheckLevelMultiplier(typeValue)) return multiplierNumerator;

            if (CheckPrefsLevelMultiplier())
                multiplierNumerator = GetLevelMultiplierNumerator(typeValue);

            return multiplierNumerator;
        }

        public static bool CheckLevelMultiplier(string typeValue)
        {
            int currentLevel = BlackboardQueryUtils.GetMyLevel();
            if (currentLevel < 1) return false;

            long currentNumerator = GetLevelMultiplierNumerator(currentLevel, typeValue);
            if (previousLevelMultiplierNumeratorInfoBB == null)
                return false;
            else if (currentLevelMultiplierNumeratorInfoBB.GetValue<int>("level") > currentLevel - 1)
                return currentNumerator > previousLevelMultiplierNumeratorInfoBB?.GetValue<long>(typeValue);
            return false;
        }

        public static bool CheckPrefsLevelMultiplier()
        {
            if(previousLevelMultiplierNumeratorInfoBB == null) return false;

            string userID = BlackboardQueryUtils.GetMyUserId();
            if( string.IsNullOrEmpty(userID) ) return false;

            bool isActiveLoginCount = false;
            bool isActiveCheckTime = false;
            if (PlayerPrefs.HasKey(SHOP_MULTIPLIER_LOGIN_COUNT + userID))
            {
                int loginCount = PlayerPrefs.GetInt(SHOP_MULTIPLIER_LOGIN_COUNT + userID);
                if (BlackboardUtils.FindVariable<int>(null, "/me/loginCount").value - loginCount < 5)
                    isActiveLoginCount = true;
            }
            if (PlayerPrefs.HasKey(SHOP_MULTIPLIER_CHECK_TIME + userID))
            {
                long checkTime = PlayerPrefsUtils.GetInt64(SHOP_MULTIPLIER_CHECK_TIME + userID);
                long threeDayMs = TimeUtils.ONE_DAY_MS * 3;
                if (checkTime + threeDayMs > TimeUtils.GetTimeStamp())
                    isActiveCheckTime = true;
            }

            return isActiveLoginCount && isActiveCheckTime;
        }

        public static void UpdatePrefsLevelMultiplier()
        {
            string userID = BlackboardQueryUtils.GetMyUserId();
            if( string.IsNullOrEmpty(userID) ) return;

            PlayerPrefs.SetInt(LevelUtils.SHOP_MULTIPLIER_LOGIN_COUNT + userID, BlackboardUtils.FindVariable<int>(null, "/me/loginCount").value);
            PlayerPrefsUtils.SetInt64(LevelUtils.SHOP_MULTIPLIER_CHECK_TIME + userID, TimeUtils.GetTimeStamp());
        }

        public static long GetLevelMultiplierNumeratorValue(long baseValue, string typeValue)
        {
            long levelMultiplierNumerator = GetLevelMultiplierNumerator(typeValue);
            if (levelMultiplierNumerator > 0L)
                return NumberUtils.GetMultiplierNumeratorValue(baseValue, levelMultiplierNumerator);
            return baseValue;
        }

        public static long GetLevelMultiplierDailyBoostCoinNumeratorValue(long baseValue, Blackboard bb, string typeValue)
        {
            if (bb == null) return baseValue;
            Variable<bool> isApplyLevelMultiplier = BlackboardUtils.FindVariable<bool>(bb, "applyLevelMultiplier");
            if (isApplyLevelMultiplier != null && isApplyLevelMultiplier.value == true)
                return GetLevelMultiplierNumeratorValue(baseValue, typeValue);
            return baseValue;
        }
    }
}
