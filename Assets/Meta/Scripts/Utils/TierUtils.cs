using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public enum TierMultiplierTableType
    {
        CoinMultiplier = 0,
        TimeBonusMultiplier,
        DailyBonusMultiplier,
        VipBonusMultiplier,
        FriendGiftMultiplier
    }

    public class TierUtils
    {
        private static List<long> rpTable = new List<long>();
        private static List<long> vipBonusCoinTable = new List<long>();
        private static List<long> tierMultiplierNumeratorTable = new List<long>();

        public static void Clear()
        {
            rpTable.Clear();
            vipBonusCoinTable.Clear();
            tierMultiplierNumeratorTable.Clear();
        }

        public static int GetMeTier()
        {
            return BlackboardUtils.FindVariable<int>(null, "/me/tier").value;
        }

        public static int GetMeTierGroup()
        {
            return GetTierGroup(GetMeTier());
        }

        public static List<long> GetRpTable()
        {
            if (rpTable.Count == 0)
                rpTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/tier/RP_TABLE").value;

            return rpTable;
        }

        public static List<long> GetVipBonusCoinsTable()
        {
            if (vipBonusCoinTable.Count == 0)
                vipBonusCoinTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/tier/VIP_DAILY_BONUS_CREDIT").value;

            return vipBonusCoinTable;
        }

        public static List<long> GetTierMultiplierNumeratorTable()
        {
            if (tierMultiplierNumeratorTable.Count == 0)
                tierMultiplierNumeratorTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/tier/TIER_MULTIPLIER_NUMERATOR_TABLE").value;

            return tierMultiplierNumeratorTable;
        }

        public static void SetTier(int tier)
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "me/tier", tier);
        }

        public static int GetTier(long accRp)
        {
            List<long> rpTable = GetRpTable();

            int tier = 0;

            for (int i = 0; i < rpTable.Count; ++i)
            {
                if (accRp >= rpTable[i])
                {
                    tier = i;

                }
                else
                {
                    break;
                }
            }

            return tier;
        }

        public static long GetTotalRP(int tier)
        {
            List<long> rpTable = GetRpTable();

            if (rpTable.Count > tier)
            {
                return rpTable[tier];
            }

            return 0;
        }

        public static long GetRequireRP(int tier)
        {
            List<long> rpTable = GetRpTable();

            if (rpTable.Count > tier && !IsMaxTier(tier))
            {
                return rpTable[tier + 1] - rpTable[tier];
            }

            return 0;
        }

        public static int GetTierGroup(int tier)
        {
            if (tier < 0) return 0;
            return tier / 3;
        }

        public static int GetMaxTier()
        {
            return BlackboardUtils.FindVariable<int>(null, "/values/tier/MAX_TIER").value;
        }

        public static bool IsMaxTier(int tier)
        {
            if (GetMaxTier() <= tier)
            {
                return true;
            }

            return false;
        }

        public static T GetTableValue<T>(int tier, List<T> table)
        {
            if (table.Count <= tier)
            {
                return table[table.Count - 1];
            }
            return table[tier];
        }

        public static double GetFractionMultiplier(int tier)
        {
            long numerator = GetTableValue<long>(tier, GetTierMultiplierNumeratorTable());

            return (double)numerator / (double)NumberUtils.GetGlobalDenominator();
        }

        public static string GetFrationMultiplierText(int tier)
        {
            return GetFractionMultiplier(tier).ToString();
        }

        public static double GetTierMultiplier(int tier)
        {
            return GetFractionMultiplier(tier);
        }

        public static long GetCurrentTierMultiplierValue(long originCredit)
        {
            int tier = BlackboardQueryUtils.GetMyTier();
            long multiplierNumerator = GetTableValue<long>(tier, GetTierMultiplierNumeratorTable());
            return NumberUtils.GetMultiplierNumeratorValue(originCredit, multiplierNumerator);
        }

        public static long GetVipCoins(int tier)
        {
            long baseCoin = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(GetTableValue(tier, GetVipBonusCoinsTable()), FreebieLevelUtils.FreebieType.VIP_BONUS);
            baseCoin = NumberUtils.GetMultiplierNumeratorValue(baseCoin, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator("SHOP_BONUS"));
            return baseCoin;
        }

        public static long GetDiffTimeBonusCoins(int fromTier, int targetTier)
        {
            long baseTimeBonusCoins = BlackboardUtils.FindVariable<long>(null, "/values/timeBonus/BASE_CREDIT").value;

            long fromTimeBonusCoins = GetTierFractionCoin(baseTimeBonusCoins, fromTier);
            long targetTimeBonusCoins = GetTierFractionCoin(baseTimeBonusCoins, targetTier);

            return targetTimeBonusCoins - fromTimeBonusCoins;
        }

        public static long GetTimeBonusCoins(int tier)
        {
            long baseTimeBonusCoins = BlackboardUtils.FindVariable<long>(null, "/values/timeBonus/BASE_CREDIT").value;
            return FreebieLevelUtils.GetLevelMultiplierNumeratorValue(GetTierFractionCoin(baseTimeBonusCoins, tier), FreebieLevelUtils.FreebieType.TIME_BONUS);
        }

        public static long GetTierFractionCoin(long coin, int tier)
        {
            long numerator = GetTableValue<long>(tier, GetTierMultiplierNumeratorTable());

            return NumberUtils.GetMultiplierNumeratorValue(coin, numerator);
        }

        public static long GetRewardCoin(long coin, double multiplier)
        {
            long numerator = System.Convert.ToInt64((multiplier + 0.00001) * NumberUtils.GetGlobalDenominator());
            return coin * numerator / NumberUtils.GetGlobalDenominator();
            // return System.Convert.ToInt64(System.Convert.ToDouble(coin) * RoundDouble(multiplier));
        }
    }
}
