using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace BagelCode.VipLounge
{
    public static class VipLounge
    {
        public static class Utils
        {
            public static GameObject MainScene
            {
                get
                {
                    if (mainScene == null)
                        mainScene = GameObject.Find("Main Canvas/Area/VIP Epic Lounge Main");
                    return mainScene;
                }
            }
            private static GameObject mainScene = null;

            public static Blackboard VipLoungeInfo
            {
                get
                {
                    if (vipLoungeInfo == null)
                    {
                        var info = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "vipLoungeInfo");
                        vipLoungeInfo = (Blackboard)info;
                    }

                    return vipLoungeInfo;
                }
            }
            private static Blackboard vipLoungeInfo;

            public static bool IsVipLoungeInfoNull() => vipLoungeInfo == null;

            public static string GetExpireDate()
            {
                long delta = BenefitEndTimestamp - TimeUtils.GetTimeStamp();
                return BlackboardQueryUtils.GetTimeStampToTimeString(delta);
            }

            public static long GetCurrentLoungePoint()
            {
                if (BadgeCount == Defines.MAX_BADGE_COUNT)
                    return ExcessLoungePoint;
                else
                    return LoungePoint;
            }

            public static long GetLoungeJackpotMultiplierValue(long baseWinCredit, long multiplier)
            {
                if (multiplier <= 0L)
                    return baseWinCredit;

                return (long)(baseWinCredit * ((double)multiplier / NumberUtils.GetGlobalDenominator()));
            }

            public static long GetLoungeJackpotCreditFloor(long baseCredit, long floorNumber = 1000L)
            {
                return baseCredit / floorNumber * floorNumber;
            }

            public static int GetExtendedValue()
            {
                return BlackboardUtils.FindVariable<int>(VipLoungeInfo, "extendedBetIndex")?.value ?? 0;
            }

            public static bool IsLoungeBadgeEnabled()
            {
                return BlackboardQueryUtils.IsVipLoungeEnabled() && BlackboardQueryUtils.HasVipLoungeBadge();
            }


            public static bool IsEnded => TimeUtils.GetTimeStamp() > BenefitEndTimestamp;
            public static bool IsCashbackActive => TimeUtils.GetTimeStamp() < CashbackEndTimestamp;

            public static int PrevBadgeCount => BlackboardUtils.FindVariable<int>("/vipLoungeInfo/prevBadgeCount")?.value ?? BadgeCount;
            public static int BadgeCount => BlackboardUtils.FindVariable<int>("/vipLoungeInfo/badgeCount")?.value ?? 0;
            public static long MaxLoungePoint => BlackboardUtils.FindVariable<long>("/vipLoungeInfo/gaugeMax")?.value ?? 0;
            public static long LoungePoint => BlackboardUtils.FindVariable<long>("/vipLoungeInfo/loungePoint")?.value ?? 0;
            public static long ExcessLoungePoint => BlackboardUtils.FindVariable<long>("/vipLoungeInfo/excessLoungePoint")?.value ?? 0;
            public static long BenefitEndTimestamp => BlackboardUtils.FindVariable<long>("/vipLoungeInfo/benefitEndTimestamp")?.value ?? 0;

            public static int CumulatedSpinCount(Blackboard bb) => BlackboardUtils.FindVariable<int>(bb, "cumulatedSpinCount")?.value ?? 0;
            public static long QualifiedBetAmount(Blackboard bb) => BlackboardUtils.FindVariable<long>(bb, "qualifiedBetAmount")?.value ?? 0;

            public static long CashbackEndTimestamp => BlackboardUtils.FindVariable<long>("/cashbackInfo/endTimestamp")?.value ?? 0;
            public static long CashbackTotalCredit => BlackboardUtils.FindVariable<long>("/cashbackInfo/totalCredit")?.value ?? 0;

            public static string EVENT_NAME => StringTableUtils.GetString(StringTable.StringTableType.Global, "LOBBY_EVENT_NAME_VIP_LOUNGE");

            // Lounge Jackpot
            public static bool IsActiveJackpotPopup => TimeUtils.GetTimeStamp() > LoungeJackpotActiveTimestamp && LoungeJackpotActiveTimestamp != 0;
            public static bool IsLockLoungeJackpot => (LoungeJackpotActiveTimestamp == 0 || LoungeJackpotActiveTimestamp < TimeUtils.GetTimeStamp()) && !IsEnded;
            public static long LoungeJackpotActiveTimestamp => BlackboardUtils.FindVariable<long>("/vipLoungeInfo/loungeJackpotActivateTimestamp")?.value ?? 0;
            public static List<Blackboard> LoungeJackpotTableList(Blackboard bb) => BlackboardUtils.FindVariable<List<Blackboard>>(bb, "loungeJackpotInfo/jackpotTableList")?.value ?? null;
            public static List<Blackboard> LoungeJackpotWheelPreset(Blackboard bb) => BlackboardUtils.FindVariable<List<Blackboard>>(bb, "loungeJackpotInfo/wheelPreset")?.value ?? null;
            public static long LoungeJackpotBaseWinCredit(Blackboard bb) => BlackboardUtils.FindVariable<long>(bb, "loungeJackpotInfo/baseWinCredit")?.value ?? 0L;
            public static long LoungeJackpotGrandJackpotCommunityCredit(Blackboard bb) => BlackboardUtils.FindVariable<long>(bb, "loungeJackpotInfo/grandJackpotCommunityCredit")?.value ?? 0L;

            // MISC
            public static long QualifiedSpinsValue => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/CHALLENGE_VALUES/QUALIFIED_SPINS")?.value ?? 0;
            public static long CollectShopBonusValue => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/CHALLENGE_VALUES/COLLECT_SHOP_BONUS")?.value ?? 0;
            public static long ReachLevelValue => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/CHALLENGE_VALUES/REACH_LEVEL")?.value ?? 0;
            public static long CollectTimeBonusValue => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/CHALLENGE_VALUES/COLLECT_TIME_BONUS")?.value ?? 0;
            public static long CollectLuckySpinValue => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/CHALLENGE_VALUES/COLLECT_LUCKY_SPIN")?.value ?? 0;
            public static int CumulativeSpinMax => BlackboardUtils.FindVariable<int>("/values/misc/VIP_LOUNGE_MISC/CUMULATIVE_SPIN_MAX")?.value ?? 0;
            public static long LoungeOpenTimeMillisec => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/LOUNGE_OPEN_TIME_MILLISEC")?.value ?? 0;
            public static long LoungeJackpotTimeMillisec => BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/LOUNGE_JACKPOT_TIMER_MILLISEC")?.value ?? 0L;
        }

        public static class Defines
        {
            // Sounds
            public const string VIP_LOUNGE_LOADING = "Meta_VIPLounge_Loading";
            public const string VIP_LOUNGE_MAIN_BGM = "Meta_VIPLounge_MainBGM";
            public const string SPIN_REWARD_SHOP = "Spin_Reward_Shop";
            public const string BONUS_FEATURES_NOTICE_LOCK = "Bonus_Features_Notice_Lock";
            public const string BONUS_FEATURES_NOTICE_OPEN = "Bonus_Features_Notice_Open";
            public const string LOUNGE_JACKPOT_ENTER = "LoungeJackpot_Enter";
            public const string LOUNGE_JACKPOT_MAIN_BGM = "LoungeJackpot_Main_BGM";
            public const string LOUNGE_JACKPOT_EXTRA_VLP = "LoungeJackpot_ExtraVLP_Apply";
            public const string LOUNGE_JACKPOT_WHEEL = "LoungeJackpot_Wheel";
            public const string LOUNGE_JACKPOT_WHEEL_TICK = "LoungeJackpot_Wheel_Tick";
            public const string LOUNGE_JACKPOT_WHEEL_STOP = "LoungeJackpot_Wheel_Stop";
            public const string LOUNGE_JACKPOT_RESULT_COIN = "LoungeJackpot_Result_Coin";
            public const string LOUNGE_JACKPOT_RESULT_MINI = "LoungeJackpot_Result_MiniJackpot";
            public const string LOUNGE_JACKPOT_RESULT_MINOR = "LoungeJackpot_Result_MinorJackpot";
            public const string LOUNGE_JACKPOT_RESULT_MAJOR = "LoungeJackpot_Result_MajorJackpot";
            public const string LOUNGE_JACKPOT_RESULT_GRAND = "LoungeJackpot_Result_GrandJackpot";
            public const string LOUNGE_JACKPOT_RESULT_END_CREDIT = "LoungeJackpot_Result_End_Credit";

            public const int MAX_BADGE_COUNT = 2;
            public const int INFORMATION_PAGE_COUNT = 3;
            public const int EARNING_POINT_CHALLENGE_COUNT = 5;
            
            public const string COMMON_BUNDLE = "mgviploungecommon";
            public const string CONTENTS_BUNDLE = "mgviploungecontents";

            // main cell types
            public const string META_GAME = "MetaGame";
            public const string SPIN_REWARD = "SpinReward";
            public const string LEVEL_UP = "LevelUp";
            public const string VEGAS_CARD = "VegasCard";
            public const string REWARD = "Reward";
            public const string HIGH_ROLLER = "HighRoller";
            public const string CLUB_BONUS_BOOSTED = "ClubBonusBoosted";
            public const string LOUNGE_JACKPOT_INFO = "LoungeJackpotInfo";

            public const string PREV_BADGE_COUNT_KEY = "prevBadgeCount";

            //
            public const string PLAYER_PREFS_IS_FIRST_ENTER = "VIP_LOUNGE_IS_FIRST_ENTER";
            public const string PLAYER_PREFS_DEBUG_EARN_BADGE = "VIP_LOUNGE_DEBUG_EARN_BADGE";
            public const string PLAYER_PREFS_DEBUG_STATIC_BADGE = "VIP_LOUNGE_DEBUG_STATIC_BADGE";
            public const string PLAYER_PREFS_JACKPOT_INCREASE_TIME = "VIP_LOUNGE_JACKPOT_INCREASE_TIME";
            public const string PLAYER_PREFS_VIP_LOUNGE_JACKPOT_DEBUG = "VIP_LOUNGE_JACKPOT_DEBUG";
            // Lounge Jackpot
            public const string LOUNGE_JACKPOT_DEBUG_SPIN = "vipLoungeDebugSpin";
        }

        public static class Events
        {
            public const string ON_ENTER_VIP_LOUNGE = "OnEnterVipLounge";
            public const string ON_CLOSE_WELCOME_POPUP = "OnCloseWelcomePopup";

            // OnMetaUIEvent
            public const string CHECK_VIP_LOUNGE_OPEN = "CheckVipLoungeOpen";
            public const string ON_UPDATE_VIP_LOUNGE_INFO = "OnUpdateVipLoungeInfo";

            // Main Scene
            public const string ON_CLOSE = "OnClose";
            public const string ON_CLICK_INFORMATION = "OnClickInformation";
            public const string ON_CLICK_EARNING_POINT = "OnClickEarningPoint";
            public const string ON_BACK_BUTTON = "OnBackButton";
            public const string ON_OK_BUTTON = "OnOKButton";
            public const string ON_RETURN = "OnReturn";
            public const string ON_CLICK_SPIN_REWARD = "OnClickSpinReward";
            public const string ON_JACKPOT_END_TIME = "OnJackpotEndTime";
            public const string ON_CLICK_JACKPOT = "OnClickJackpot";
            public const string ON_CLICK_BIG_META_GAME = "OnClickBigMetaGame";
            public const string ON_ENDED_TIME = "OnEndedTime";

            // Information
            public const string ON_SELECT_LEFT_INFO = "OnSelectLeftInfo";
            public const string ON_SELECT_RIGHT_INFO = "OnSelectRightInfo";

            // Earning Point
            public const string ON_PLAY = "OnPlay";
            public const string ON_OTHERS = "OnOthers";
            public const string ON_CLICK_COIN_SHOP = "OnClickCoinShop";
            public const string ON_CLICK_GEM_SHOP = "OnClickGemShop";
            public const string ON_CLICK_DAILY_SPIN = "OnClickDailySpin";

            // Spin Reward
            public const string ON_CLICK_PURCHASE = "OnClickPurchase";
        }
    }
}
