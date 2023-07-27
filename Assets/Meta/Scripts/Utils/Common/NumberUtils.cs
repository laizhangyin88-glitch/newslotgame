using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class NumberUtils
    {
        private static long globalDenominator = 0;

        public static void Clear()
        {
            globalDenominator = 0;
        }

        public static long GetGlobalDenominator()
        {
            if(globalDenominator == 0)
                globalDenominator = BlackboardUtils.FindVariable<long>(null, "/values/misc/GLOBAL_DENOMINATOR").value;

            return globalDenominator;
        }

        public static long GetMultiplierNumeratorValue(long origValue, long multiplierNumerator)
        {
            if(multiplierNumerator <= 0L)
                return origValue;

            return origValue * multiplierNumerator / GetGlobalDenominator();
        }

        public static long GetAdditionalMultiplierNumeratorValue(long origValue, long additionalMultiplierNumerator)
        {
            return GetMultiplierNumeratorValue(origValue, (additionalMultiplierNumerator + GetGlobalDenominator()));
        }

        public static double GetMultiplierFromNumerator(long multiplierNumerator)
        {
            if(multiplierNumerator <= 0L)
                return 0.0;

            return (double)multiplierNumerator / (double)GetGlobalDenominator();
        }

        public static long GetPercentFromNumerator(long multiplierNumerator)
        {
            return multiplierNumerator * 100L / GetGlobalDenominator();
        }

        public static long GetAdditionalPercent(long multiplierNumerator)
        {
            if(multiplierNumerator < GetGlobalDenominator())
                return 100L;

            var additionalNumerator = multiplierNumerator - GetGlobalDenominator();

            return additionalNumerator / (GetGlobalDenominator()/100L) ;
        }

        public static long GetDevideNumeratorValue(long origValue, long devideMultiplierNumerator)
        {
            if(devideMultiplierNumerator <= 0)
                devideMultiplierNumerator = GetGlobalDenominator();

            return origValue * GetGlobalDenominator() / devideMultiplierNumerator;
        }

        public static long GetSaleNumeratorValue(long origValue, long salePercentNumerator)
        {
            if(salePercentNumerator <= 0 || salePercentNumerator > 100L)
                salePercentNumerator = GetGlobalDenominator();


            return origValue * salePercentNumerator / GetGlobalDenominator();
        }

        public static double GetDiscountNumeratorValue(double originValue, long discountNumerator)
        {
            double discountValue = GetMultiplierFromNumerator(discountNumerator);

            return originValue * discountValue;
        }

        public static long GetDiscountPercentValue(long discountNumerator)
        {
            if(discountNumerator <= GetGlobalDenominator())
                return 0;

            return (GetGlobalDenominator() * 100) / discountNumerator;
        }

        public static long GetShopInflationNumerator(ShopType type)
        {
            long inflationNumerator = 0;

            switch(type)
            {
                case ShopType.COIN:
                    inflationNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/INFLATION_NUMERATOR_SHOP").value;
                    break;
                // case ShopType.DAILY_BOOST:
                //     break;
                case ShopType.PIGGY_BANK:
                    inflationNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/INFLATION_NUMERATOR_POG").value;
                    break;
                case ShopType.DAILY_BONUS:
                    inflationNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/INFLATION_NUMERATOR_WHEEL").value;
                    break;
                // case ShopType.TIER_BOOST:
                //     break;
                // case ShopType.TIER_UP:
                //     break;
                // case ShopType.VOUCHER:
                //     break;
                // case ShopType.ACTION:
                //     break;
                // case ShopType.ALL_IN_BONUS:
                //     break;
                // case ShopType.MEGA_WHEEL:
                //     break;
                // case ShopType.VIP_DEAL:
                //     break;
                // case ShopType.POG_BOOSTER:
                //     break;
                case ShopType.GEM:
                    inflationNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/INFLATION_NUMERATOR_SHOP").value;
                    break;
                // case ShopType.COIN_BOOSTER:
                //     break;
                // case ShopType.COIN_WITH_META_GAME:
                //     break;
                // case ShopType.GEM_WITH_META_GAME:
                //     break;
                // case ShopType.GEM_BAB:
                //     break;
            }

            // inflationNumerator += NumberUtils.GetGlobalDenominator();

            return inflationNumerator;
        }

        public static double SecondDecimalCutting(long targetValue)
        {
            if(targetValue <= 0L) return 0.0;

            return SecondDecimalCutting(GetMultiplierFromNumerator(targetValue));
        }

        public static double SecondDecimalCutting(double targetValue)
        {
            if(targetValue <= 0.0) return 0.0;

            var cutStr = string.Format("{0:0.#}", targetValue - 0.05);
            return System.Convert.ToDouble(cutStr);
        }
    }
}
