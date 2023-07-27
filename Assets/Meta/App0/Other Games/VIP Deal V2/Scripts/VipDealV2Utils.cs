using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using System.Collections.Generic;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static class VipDealV2
    {
        public static class Utils
        {
            public static bool GetIsViewed(bool isPayDeal)
            {
                var info = GetInfo();
                if (info == null) return false;

                return isPayDeal ?
                    BlackboardUtils.FindValue<bool>(info, "isViewedPaid") :
                    BlackboardUtils.FindValue<bool>(info, "isViewedFree");
            }

            public static void SetIsViewed(bool isViewed, bool isPayDeal)
            {
                var info = GetInfo();
                if (info == null) return;

                if (isPayDeal)
                    BlackboardUtils.SetOrCreateValue(info, "isViewedPaid", isViewed);
                else
                    BlackboardUtils.SetOrCreateValue(info, "isViewedFree", isViewed);
            }

            public static long GetFreeDealCreditMultiplierValue(long originCredit)
            {
                long resultCredit = originCredit;

                bool applyTierMulti = BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_VIP_DEAL_V2_FREEBIE_TIER_MULTI")?.value ?? false;
                if (applyTierMulti)
                    resultCredit = TierUtils.GetCurrentTierMultiplierValue(resultCredit);

                resultCredit = LevelUtils.GetLevelMultiplierNumeratorValue(resultCredit, "vipDealFreebie");
                return resultCredit;
            }

            public static bool GetVipFreeDealCompensationState()
            {
                return BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "vipDealV2FreebieCompensation").value;
            }

            public static void SetVipFreeDealCompensationState(bool state)
            {
                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "vipDealV2FreebieCompensation", state);
            }

            public static Blackboard GetInfo()
            {
                return BlackboardUtils.FindVariable<Blackboard>("/vipDealInfoV2")?.value;
            }

            public static int GetInfoID()
            {
                var info = GetInfo();
                return info != null ? BlackboardUtils.FindValue<int>(info, "vipDealInfoId") : 0;
            }

            public static bool IsEnabled()
            {
                return BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_VIP_DEAL_V2")?.value ?? false;
            }

            public static Blackboard GetActiveInfo()
            {
                var enableVipDeal = BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_VIP_DEAL")?.value ?? false;
                var enableVipDealV2 = IsEnabled();
                if (!enableVipDeal && enableVipDealV2)
                {
                    var vipDealInfoV2 = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "vipDealInfoV2");

                    if (vipDealInfoV2 != null && vipDealInfoV2.value != null)
                    {
                        var startTimestamp = vipDealInfoV2.value.GetValue<long>("startTimestamp");
                        var endTimestamp = vipDealInfoV2.value.GetValue<long>("endTimestamp");
                        var currentTimestamp = TimeUtils.GetTimeStamp();

                        if (currentTimestamp > startTimestamp && currentTimestamp <= endTimestamp)
                            return vipDealInfoV2.value;
                    }
                }

                return null;
            }

            public static int GetMinTier()
            {
                return BlackboardUtils.FindValue<int>("/values/tier/VIP_DEAL_V2_MIN_TIER");
            }

            public static bool IsTierLock()
            {
                var meTier = TierUtils.GetMeTier();
                var minTier = BlackboardUtils.FindValue<int>("/values/tier/VIP_DEAL_V2_MIN_TIER");

                if (meTier >= minTier)
                    return false;

                return true;
            }

            public static List<long> GetWheelMultiplierNumeratorList(bool isPayDeal)
            {
                var info = GetInfo();
                if (info == null) return new List<long>();

                return isPayDeal ?
                    BlackboardUtils.FindValue<List<long>>(info, "paidMultiplierNumeratorList") :
                    BlackboardUtils.FindValue<List<long>>(info, "freeMultiplierNumeratorList");
            }

            public static List<Blackboard> GetDealList(bool isPayDeal)
            {
                var info = GetInfo();
                if (info == null) return new List<Blackboard>();

                return isPayDeal ?
                    BlackboardUtils.FindValue<List<Blackboard>>(info, "paidDealList") :
                    BlackboardUtils.FindValue<List<Blackboard>>(info, "freeDealList");
            }

            public static string GetIconWebImageUrl()
            {
                var info = GetInfo();
                if (info == null) return null;

                var vipDealShopBB = BlackboardQueryUtils.GetShopBB(ShopType.VIP_DEAL);
                if (vipDealShopBB == null) return null;

                return BlackboardUtils.FindVariable<string>(vipDealShopBB, "imageUrl")?.value;
            }
        }

        public static class Defines
        {
            public const long DEFAULT_PAY_DEAL_MAX_MULTIPLIER_NUMERATOR = 1000L; // 10x

            public const long COIN_GRADE_SEPARATOR_1 = 9000000L;
            public const long COIN_GRADE_SEPARATOR_2 = 30000000L;

            // Sounds
            public const string SOUND_LOBBY_DEAL_BUTTON_CLICK = "VIP_DEAL_BUTTON_CLICK";

            public const string SOUND_WHEEL_LOOP = "UI_Coin_Booster_Wheel_Loop";
            public const string SOUND_WHEEL_STOP = "UI_Coin_Booster_Stop";
            public const string SOUND_MULTIPLIER_DECIED = "VIP_DEAL_MULTIPLIER_DECIDED";
            public const string SOUND_TRANSITION = "UI_Coin_Booster_Transition";

            public const string SOUND_FREE_ITEM_DECIDED = "VIP_DEAL_FREE_ITEM_DECIDED";
        }

        public static class Events
        {
            // main
            public const string ON_ENTER_VIP_DEAL_SHOP = "OnEnterVipDealShop";
            public const string ON_ENTER_VIP_DEAL = "OnEnterVipDeal"; // meta

            public const string ON_SELECT_ITEM = "OnSelectItem";
            public const string SET_MULTIPLIER = "SetMultiplier";
            public const string COMPLETE_SET_MULTIPLIER = "CompleteSetMultiplier";
            public const string COMPLETE_OPEN_CARD = "CompleteOpenCard";

            public const string ON_CHANGE_FREE_START = "OnChangeFreeStart";
            public const string ON_SET_FREE_COMPLETE = "OnSetFreeComplete";

            public const string ON_FREEBIE_REDEEM_SUCCESS = "OnFreebieRedeemSuccess";
            public const string ON_FREEBIE_REDEEM_FAILURE = "OnFreebieRedeemFailure";
            public const string ON_REDEEM = "OnRedeem";

            public const string ON_SKIP_PAY_DEAL = "OnSkipPayDeal";

            // global
            public const string READY_SELECT_CELL = "ReadySelectCell"; // meta
            public const string ON_OPEN_CARD = "OnOpenCard";
        }
    }
}
