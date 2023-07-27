using UnityEngine;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode
{
    public static class FreebieLevelUtils
    {
        public enum FreebieType
        {
            TIME_BONUS,
            LUCKY_SPIN,
            DAILY_BONUS_SPIN, // Daily Bonus Wheel
            VIP_BONUS,
            EPIC_WIN_SHARE,
            WELCOME_BACK_BONUS,
            FRIENDS_BONUS,
            FRIENDS_DEAL_BONUS,
            CLUB_DEAL_BONUS
        }

        public static long GetLevelMultiplierNumerator(FreebieType type)
        {
            return GetLevelMultiplierNumerator(BlackboardQueryUtils.GetMyLevel(), type);
        }

        public static long GetLevelMultiplierNumerator(int level, FreebieType type)
        {
            string typeValue = GetFreebieTypeToString(type);
            return GetLevelMultiplierNumerator(level, typeValue);
        }

        public static long GetLevelMultiplierNumerator(int level, string typeValue)
        {
            if (level < 0) return 0L;

            return LevelUtils.GetLevelMultiplierNumeratorInfoBB(level)?.GetValue<long>(typeValue) ?? 0L;
        }

        public static long GetLevelMultiplierNumeratorValue(long baseValue, FreebieType type)
        {
            long levelMultiplierNumerator = GetLevelMultiplierNumerator(type);
            if (levelMultiplierNumerator > 0L)
                return NumberUtils.GetMultiplierNumeratorValue(baseValue, levelMultiplierNumerator);
            return baseValue;
        }

        public static string GetFreebieTypeToString(FreebieType type)
        {
            string typeToString = "";
            switch (type)
            {
                case FreebieType.TIME_BONUS:
                    typeToString = "timeBonus";
                    break;
                case FreebieType.LUCKY_SPIN:
                    typeToString = "luckySpin";
                    break;
                case FreebieType.DAILY_BONUS_SPIN:
                    typeToString = "dailySpin";
                    break;
                case FreebieType.VIP_BONUS:
                    typeToString = "vipBonus";
                    break;
                case FreebieType.EPIC_WIN_SHARE:
                    typeToString = "epicWinShare";
                    break;
                case FreebieType.WELCOME_BACK_BONUS:
                    typeToString = "welcomeBackBonus";
                    break;
                case FreebieType.FRIENDS_BONUS:
                    typeToString = "friendsBonus";
                    break;
                case FreebieType.FRIENDS_DEAL_BONUS:
                    typeToString = "friendsDealBonus";
                    break;
                case FreebieType.CLUB_DEAL_BONUS:
                    typeToString = "clubDealBonus";
                    break;
                default:
                    typeToString = "shop";
                    break;
            }

            return typeToString;
        }
    }
}
