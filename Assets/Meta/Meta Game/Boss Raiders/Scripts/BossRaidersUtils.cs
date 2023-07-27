using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class BossRaidersUtils
    {
        public class Sounds
        {
            public const string BOSS_RAIDERS_START                  = "Meta_BossRaiders_Start";
            public const string BOSS_RAIDERS_POINTS_FLY             = "Meta_BossRaiders_PointsFly";
            public const string BOSS_RAIDERS_POINTS_GET             = "Meta_BossRaiders_PointsGet";
            public const string BOSS_RAIDERS_GAUGE                  = "Meta_BossRaiders_Gauge";
            public const string BOSS_RAIDERS_COLLECT                = "Meta_BossRaiders_Collect";
            public const string BOSS_RAIDERS_BGM                    = "Meta_BossRaiders_BGM";
            public const string BOSS_RAIDERS_URGENT_BGM             = "Meta_BossRaiders_UrgentBGM";
            public const string BOSS_RAIDERS_WHEEL_SPIN             = "Meta_BossRaiders_WheelSpin";
            public const string BOSS_RAIDERS_WHEEL_STOP             = "Meta_BossRaiders_WheelStop";
            public const string BOSS_RAIDERS_BATTLE_WALK            = "Meta_BossRaiders_Battle_Walk";
            public const string BOSS_RAIDERS_BATTLE_ATTACK          = "Meta_BossRaiders_Battle_Attack";
            public const string BOSS_RAIDERS_BATTLE_INJURED_NORMAL  = "Meta_BossRaiders_Battle_Injured_Normal";
            public const string BOSS_RAIDERS_BATTLE_INJURED_BIGHIT  = "Meta_BossRaiders_Battle_Injured_BigHit";
            public const string BOSS_RAIDERS_BATTLE_INJURED_MEGAHIT = "Meta_BossRaiders_Battle_Injured_MegaHit";
            public const string BOSS_RAIDERS_BATTLE_INJURED_EPICHIT = "Meta_BossRaiders_Battle_Injured_EpicHit";
            public const string BOSS_RAIDERS_BATTLE_INJURED_MULTI   = "Meta_BossRaiders_Battle_Injured_Multi";
            public const string BOSS_RAIDERS_BIG_HIT                = "Meta_BossRaiders_BigHit";
            public const string BOSS_RAIDERS_MEGA_HIT               = "Meta_BossRaiders_MegaHit";
            public const string BOSS_RAIDERS_EPIC_HIT               = "Meta_BossRaiders_EpicHit";
            public const string BOSS_RAIDERS_MULTI                  = "Meta_BossRaiders_Multi";
            public const string BOSS_RAIDERS_BONUS                  = "Meta_BossRaiders_Bonus";
            public const string BOSS_RAIDERS_BONUS_CARD             = "Meta_BossRaiders_BonusCard";
            public const string BOSS_RAIDERS_POPUP                  = "Meta_BossRaiders_Popup";
            public const string BOSS_RAIDERS_NEW_SMALL_BOSS         = "Meta_BossRaiders_NewSmallBoss";
            public const string BOSS_RAIDERS_NEW_BIG_BOSS           = "Meta_BossRaiders_NewBigBoss";
            public const string BOSS_RAIDERS_RESULTS                = "Meta_BossRaiders_Results";
            public const string BOSS_RAIDERS_HIT_SMALL_BOSS         = "Meta_BossRaiders_Hit_Small_Boss";
            public const string BOSS_RAIDERS_HIT_BIG_BOSS           = "Meta_BossRaiders_Hit_Big_Boss";
            public const string BOSS_RAIDERS_DISAPPEAR_SMALL_BOSS   = "Meta_BossRaiders_Disappear_Small_Boss";
            public const string BOSS_RAIDERS_DISAPPEAR_BIG_BOSS     = "Meta_BossRaiders_Disappear_Big_Boss";
            public const string BOSS_RAIDERS_BONUS_REWARD_CARD      = "Meta_BossRaiders_Bonus_RewardCard";
            public const string BOSS_RAIDERS_BONUS_COLLECT_COIN     = "Meta_BossRaiders_Bonus_CollectCoin";
            public const string BOSS_RAIDERS_BONUS_COLLECT_GEM      = "Meta_BossRaiders_Bonus_CollectGem";
            public const string BOSS_RAIDERS_BONUS_COLLECT_ENERGY   = "Meta_BossRaiders_Bonus_CollectEnergy";
            public const string BOSS_RAIDERS_POPUP_OUT_OF_ENERGY    = "Meta_BossRaiders_Popup_OutOfEnergy";
            public const string BOSS_RAIDERS_WHEEL_SPIN_BUTTON      = "Meta_BossRaiders_WheelSpin_Button";
            public const string BOSS_RAIDERS_WHEEL_START            = "Meta_BossRaiders_WheelStart";
            // Deal Sound
            public const string BOSS_RAIDERS_DEAL_SCORE_COIN        = "Meta_BossRaiders_Deal_Score_Coin";
            public const string BOSS_RAIDERS_DEAL_SCORE_GEM         = "Meta_BossRaiders_Deal_Score_Gem";
            public const string BOSS_RAIDERS_DEAL_POPUP_DEFAULT     = "Meta_BossRaiders_Deal_Popup_Default";
            public const string BOSS_RAIDERS_DEAL_POPUP_BIG         = "Meta_BossRaiders_Deal_Popup_Big";
            public const string BOSS_RAIDERS_DEAL_POPUP_SUPER_BIG   = "Meta_BossRaiders_Deal_Popup_SuperBig";
            public const string BOSS_RAIDERS_DEAL_POPUP_MEGA        = "Meta_BossRaiders_Deal_Popup_Mega";
            public const string BOSS_RAIDERS_DEAL_POPUP_SUPER_MEGA  = "Meta_BossRaiders_Deal_Popup_SuperMega";
            public const string BOSS_RAIDERS_DEAL_POPUP_EPIC        = "Meta_BossRaiders_Deal_Popup_Epic";
            public const string BOSS_RAIDERS_DEAL_POPUP_EFFECT      = "Meta_BossRaiders_Deal_Popup_Effect";
        }

        private const string BOSS_RAIDERS_GAME_INFO = "metaGameEnterInfo";

        public const string BOSS_RAIDERS_DEAL_INFO = "bossRaidersDealInfo";

        public static readonly string ON_LEVEL_UP_EVENT = "OnBossRaidersLevelUp";
        public static readonly string ON_REFRESH_EVENT = "OnBossRaidersRefresh";

        public static readonly string BOSS_RAIDERS_DEBUG_SPIN_TYPE = "debugSpinType";

        public static readonly string BOSS_RAIDERS_LOCAL_PUSH = "BOSS_RAIDERS_LOCAL_PUSH";
        public static readonly string BOSS_RAIDERS_FIRST_VISIT = "BOSS_RAIDERS_FIRST_VISIT";

        public static readonly string NOT_FINISHED_BOSS_RAIDERS_DEAL = "NOT_FINISHED_BOSS_RAIDERS_DEAL";

        private static Blackboard bossRaidersInfo = null;
        public static Blackboard BossRaidersInfo
        {
            get
            {
                if (bossRaidersInfo == null)
                {
                    var info = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), BOSS_RAIDERS_GAME_INFO);
                    if (info != null)
                    {
                        var type = info.value.GetValue<EventInfoType>("type");
                        if(type == EventInfoType.BOSS_RAIDERS)
                        {
                            bossRaidersInfo = info.value;
                        }
                    }
                }

                return bossRaidersInfo;
            }
        }

        private static List<Blackboard> energyBundleInfoList;
        public static List<Blackboard> EnergyBundleInfoList
        {
            get
            {
                if (energyBundleInfoList != null && energyBundleInfoList.Count > 0)
                {
                    if (energyBundleInfoList[0] == null)
                        energyBundleInfoList.Clear();
                }
                if (energyBundleInfoList == null || energyBundleInfoList.Count == 0)
                {
                    if (BossRaidersInfo != null)
                    {
                        var list = BossRaidersInfo.GetVariable<List<Blackboard>>("energyBundleInfoList");
                        if (list != null) energyBundleInfoList = list.value;
                    }
                }
                return energyBundleInfoList;
            }
        }

        public static long SpinPossibleCount
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetVariable<long>("spinPossibleCount").value;
                return 0L;
            }

            set
            {
                if (BossRaidersInfo != null && value > -1)
                {
                    BlackboardUtils.SetOrCreateValue<long>(BossRaidersInfo, "spinPossibleCount", value);
                }
            }
        }

        public static long PrevEnergy
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("prevEnergy");
                return 0L;
            }
        }

        public static long PrevCurrentEnergy
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("prevCurrentEnergy");
                return 0L;
            }
        }

        public static long PrevSpinPossibleCount
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("prevSpinPossibleCount");
                return 0L;
            }
        }

        public static long Energy
        {
            get
            {
                if (BossRaidersInfo != null && RequiredEnergy > 0L)
                    return CurrentEnergy % RequiredEnergy;
                return 0;
            }
        }

        public static long CurrentEnergy
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("energy");
                return 0L;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "energy", (long)value);
            }
        }

        public static long RequiredEnergy
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("energyForSpin");
                return 0L;
            }
        }

        public static long EarnEnergy
        {
            get
            {
                if (BossRaidersInfo != null)
                {
                    var earnEnergy = BossRaidersInfo.GetVariable<long>("energyEarning");
                    if (earnEnergy != null)
                        return earnEnergy.value;
                }

                return 0L;
            }
        }

        public static Blackboard RoundBalanceInfo
        {
            get
            {
                if (BossRaidersInfo != null)
                {
                    Blackboard bb = BossRaidersInfo.GetValue<Blackboard>("roundBalanceInfo");
                    if (bb != null)
                        return bb;
                }
                return null;
            }
            set
            {
                Blackboard bb = RoundBalanceInfo;
                BlackboardUtils.SetOrCreateValue(bb, "round", value.GetValue<int>("round"));
                BlackboardUtils.SetOrCreateValue(bb, "hp", value.GetValue<int>("hp"));
                BlackboardUtils.SetOrCreateValue(bb, "leaguePoint", value.GetValue<int>("leaguePoint"));
                CurrentReward = value.GetValue<Blackboard>("reward");
            }
        }

        public static Blackboard RoundBossInfo
        {
            get
            {
                if (BossRaidersInfo != null)
                {
                    Blackboard bb = BossRaidersInfo.GetValue<Blackboard>("roundBossInfo");
                    if (bb != null)
                        return bb;
                }
                return null;
            }
            set
            {
                CurrentBossHP = value.GetValue<int>("hp");
                RoundBossVariationInfo = value.GetValue<Blackboard>("variationInfo");
            }
        }

        private static Blackboard RoundBossVariationInfo
        {
            get
            {
                if (RoundBossInfo != null)
                {
                    Blackboard bb = RoundBossInfo.GetValue<Blackboard>("variationInfo");
                    if (bb != null)
                        return bb;
                }
                return null;
            }
            set
            {
                Blackboard bb = RoundBossVariationInfo;
                BlackboardUtils.SetOrCreateValue(bb, "type", value.GetValue<BossRaidersBossType>("type"));
                BlackboardUtils.SetOrCreateValue(bb, "color", value.GetValue<BossRaidersBossColorType>("color"));
                BlackboardUtils.SetOrCreateValue(bb, "scale", value.GetValue<double>("scale"));
                BlackboardUtils.SetOrCreateValue(bb, "bossIndex", value.GetValue<int>("bossIndex"));
            }
        }

        public static int CurrentRound
        {
            get
            {
                if (RoundBalanceInfo != null)
                    return RoundBalanceInfo.GetValue<int>("round");
                return 0;
            }
        }

        public static int CurrentTotalBossHP
        {
            get
            {
                if (RoundBalanceInfo != null)
                    return RoundBalanceInfo.GetValue<int>("hp");
                return 0;
            }
        }

        public static int CurrentLeaguePoint
        {
            get
            {
                if (RoundBalanceInfo != null)
                    return RoundBalanceInfo.GetValue<int>("leaguePoint");
                return 0;
            }
        }

        public static Blackboard CurrentReward
        {
            get
            {
                if (RoundBalanceInfo != null)
                    return RoundBalanceInfo.GetValue<Blackboard>("reward");
                return null;
            }
            set
            {
                Blackboard rewardBB = CurrentReward;
                rewardBB.SetValue("type", value.GetValue<RewardType>("type"));
                rewardBB.SetValue("gem", value.GetValue<long>("gem"));
            }
        }

        public static int CurrentBossHP
        {
            get
            {
                if (RoundBossInfo != null)
                    return RoundBossInfo.GetValue<int>("hp");
                return 0;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(RoundBossInfo, "hp", value);
            }
        }

        public static Blackboard PrevRoundBalanceInfo
        {
            get
            {
                if (BossRaidersInfo != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(BossRaidersInfo, "prevRoundBalanceInfo");
                    if (bb != null)
                        return (Blackboard)bb;
                }
                return null;
            }
        }

        public static int PrevLeaguePoint
        {
            get
            {
                if (PrevRoundBalanceInfo != null)
                    return PrevRoundBalanceInfo.GetValue<int>("prevLeaguePoint");
                return CurrentLeaguePoint;
            }
        }

        public static int PrevRound
        {
            get
            {
                if (PrevRoundBalanceInfo != null)
                    return PrevRoundBalanceInfo.GetValue<int>("prevRound");
                return CurrentRound;
            }
        }

        public static Blackboard PrevReward
        {
            get
            {
                if (PrevRoundBalanceInfo != null)
                    return (Blackboard)BlackboardUtils.GetOrCreateBlackboard(PrevRoundBalanceInfo, "reward");
                return null;
            }
            set
            {
                Blackboard rewardBB = PrevReward;
                BlackboardUtils.SetOrCreateValue(rewardBB, "type", value.GetValue<RewardType>("type"));
                BlackboardUtils.SetOrCreateValue(rewardBB, "gem", value.GetValue<long>("gem"));
            }
        }

        public static BossRaidersBossType CurrentBossType
        {
            get
            {
                if (RoundBossVariationInfo != null)
                    return RoundBossVariationInfo.GetValue<BossRaidersBossType>("type");
                return BossRaidersBossType.UNKNOWN;
            }
        }

        public static BossRaidersBossColorType CurrentBossColorType
        {
            get
            {
                if (RoundBossVariationInfo != null)
                    return RoundBossVariationInfo.GetValue<BossRaidersBossColorType>("color");
                return BossRaidersBossColorType.UNKNOWN;
            }
        }

        public static double CurrentBossScale
        {
            get
            {
                if (RoundBossVariationInfo != null)
                    return RoundBossVariationInfo.GetValue<double>("scale");
                return 1;
            }
        }

        public static int CurrentBossIndex
        {
            get
            {
                if (RoundBossVariationInfo != null)
                    return RoundBossVariationInfo.GetVariable<int>("bossIndex")?.value ?? 0;
                return 0;
            }
        }

        public static int ClubRank
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<int>("clubRank");
                return 0;
            }
        }

        public static int Percentile
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<int>("percentile");
                return 0;
            }
        }

        public static List<Blackboard> ClubRankInfoList
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<List<Blackboard>>("clubRankInfoList");
                return null;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(RoundBossInfo, "clubRankInfoList", value);
            }
        }

        public static List<Blackboard> ClubLeadersContributionInfoList
        {
            get
            {
                if (BossRaidersInfo != null)
                    return bossRaidersInfo.GetValue<List<Blackboard>>("contributionInfoList");
                return null;
            }
        }

        public static List<Blackboard> WheelCandidateList
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<List<Blackboard>>("wheelCandidateList");
                return null;
            }
        }

        public static Dictionary<int, List<int>> MaxBetMultiplyNumeratorList
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<Dictionary<int, List<int>>>("maxBetMultiplyNumeratorByEnergyList");
                return null;
            }
        }

        public static Dictionary<int, int> DefaultBetMultiplyNumeratorList
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<Dictionary<int, int>>("defaultBetMultiplyNumeratorByEnergy");
                return null;
            }
        }

        public static long BaseBetMultiplyNumerator
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("baseBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "baseBetMultiplyNumerator", value);
            }
        }

        public static long CurrentBetMultiplyNumerator
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("curentBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "curentBetMultiplyNumerator", value);
            }
        }

        public static long MaxBetMultiplyNumerator
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("maxBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "maxBetMultiplyNumerator", value);
            }
        }

        public static long PreviousBetMultiplyNumerator
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("previousBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "previousBetMultiplyNumerator", value);
            }
        }

        public static int SaveBetMultiplyNumerator
        {
            get { return PlayerPrefs.GetInt("BOSS_RAIDERS_SAVE_BET", (int)NumberUtils.GetGlobalDenominator()); }
            set { PlayerPrefs.SetInt("BOSS_RAIDERS_SAVE_BET", value); }
        }

        public static long BossRaidersCooltime
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("bossRaidersCooltime");
                return 0L;
            }
        }

        public static long LastVideoAdsClaimTimestamp
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<long>("lastVideoAdsClaimTimestamp");
                return TimeUtils.GetTimeStamp();
            }
        }

        public static BossRaidersDebugSpinType DebugSpinType
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<BossRaidersDebugSpinType>(BOSS_RAIDERS_DEBUG_SPIN_TYPE);
                return BossRaidersDebugSpinType.NONE;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, BOSS_RAIDERS_DEBUG_SPIN_TYPE, value);
            }
        }

        public static int WheelResultIndex
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<int>("wheelResultIndex");
                return 0;
            }
        }

        public static Blackboard WheelResultInfo
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<Blackboard>("wheelResultInfo");
                return null;
            }
        }

        public static List<int> BetMultiplyNumeratorList
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<List<int>>("betMultiplyNumeratorList");
                return null;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "betMultiplyNumeratorList", value);
            }
        }

        public static Dictionary<int, long> FinalRewardRankPayTypeInfo
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<Dictionary<int, long>>("finalRewardRankPayTypeInfo");
                return null;
            }
        }

        public static Dictionary<int, long> FinalRewardPercentilePayTypeInfo
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<Dictionary<int, long>>("finalRewardPercentilePayTypeInfo");
                return null;
            }
        }

        public static int UpdatedTotalCompletedRound
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetValue<int>("updatedTotalCompletedRound");
                return 0;
            }
        }

        public static int ThemeId
        {
            get
            {
                if (BossRaidersInfo != null)
                    return BossRaidersInfo.GetVariable<int>("themeId")?.value ?? 1;
                return 1;
            }
            set
            {
                if (BossRaidersInfo != null)
                    BlackboardUtils.SetOrCreateValue<int>(BossRaidersInfo, "themeId", value);
            }
        }

        public static void InitBossRaiders()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.BOSS_RAIDERS);
            if (eventInfo != null)
            {
                var eventID = BlackboardUtils.GetOrCreateVariable<int>(BossRaidersInfo, "id");
                if (eventID.value > 0 && eventID.value != eventInfo.id)
                    ClearBlackboardData();
                eventID.value = eventInfo.id;
                ThemeId = ((EventDataBossRaiders)eventInfo.constraints).themeId;
            }
            UpdateBossRaidersData();
        }

        public static void InitEnterResponse()
        {
            BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "baseBetMultiplyNumerator", NumberUtils.GetGlobalDenominator());
            BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "curentBetMultiplyNumerator", NumberUtils.GetGlobalDenominator());
            BlackboardUtils.SetOrCreateValue(BossRaidersInfo, "maxBetMultiplyNumerator", NumberUtils.GetGlobalDenominator());

            UpdateBetMultiplyNumerator();

            DebugSpinType = BossRaidersDebugSpinType.NONE;
        }

        public static void UpdateBossRaidersData()
        {
            SpinPossibleCount = CurrentEnergy / RequiredEnergy;
        }

        public static void BackUpClubPoint()
        {
            Blackboard bb = PrevRoundBalanceInfo;
            BlackboardUtils.SetOrCreateValue<int>(bb, "prevLeaguePoint", CurrentLeaguePoint);
            BlackboardUtils.SetOrCreateValue<int>(bb, "prevRound", CurrentRound);
            PrevReward = CurrentReward;
        }

        public static void BackUpInfo()
        {
            if (bossRaidersInfo == null) return;

            BlackboardUtils.SetOrCreateValue<long>(bossRaidersInfo, "prevEnergy", Energy);
            BlackboardUtils.SetOrCreateValue<long>(bossRaidersInfo, "prevCurrentEnergy", CurrentEnergy);
            BlackboardUtils.SetOrCreateValue<long>(bossRaidersInfo, "prevSpinPossibleCount", SpinPossibleCount);
        }

        public static List<SlotMaker.TestSuite.DebugSpin> GetDebugSpins()
        {
            List<SlotMaker.TestSuite.DebugSpin> debugSpins = new List<SlotMaker.TestSuite.DebugSpin>();

            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)BossRaidersDebugSpinType.NONE, description = BossRaidersDebugSpinType.NONE.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_DEFAULT, description = BossRaidersDebugSpinType.ATTACK_DEFAULT.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_BIG, description = BossRaidersDebugSpinType.ATTACK_BIG.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_MEGA, description = BossRaidersDebugSpinType.ATTACK_MEGA.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)BossRaidersDebugSpinType.ATTACK_EPIC, description = BossRaidersDebugSpinType.ATTACK_EPIC.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)BossRaidersDebugSpinType.BONUS, description = BossRaidersDebugSpinType.BONUS.ToString(), DebugSequenceList = null });

            return debugSpins;
        }

        public static Blackboard GetEnergyBundleInfoBB(int idx)
        {
            if (BossRaidersInfo == null || EnergyBundleInfoList == null) return null;

            List<Blackboard> energyBundles = EnergyBundleInfoList;
            Blackboard retBB = null;

            if (idx < energyBundles.Count)
            {
                if (energyBundles[idx].GetValue<long>("totalEnergyBundleEarning") > 0L)
                    retBB = energyBundles[idx];
            }

            return retBB;
        }

        public static Blackboard GetEnergyBundleInfoBB(long betCredit)
        {
            if (BossRaidersInfo == null || EnergyBundleInfoList == null) return null;

            List<Blackboard> energyBundles = EnergyBundleInfoList;
            Blackboard retBB = null;

            for(int i = 0; i < energyBundles.Count; ++i)
            {
                long unitBet = energyBundles[i].GetValue<long>("bet");
                if (betCredit < unitBet) continue;
                retBB = energyBundles[i];
            }

            return retBB;
        }

        public static string GetClubRank()
        {
            int clubRank = ClubRank;
            return clubRank > 0 ? clubRank.ToString() : "--";
        }

        public static float GetCurrentGaugeEnergyAsFloat()
        {
            var betCredit = BlackboardUtils.FindVariable<long>("./betCredit");

            List<Blackboard> energyBundles = EnergyBundleInfoList;

            int indexOfCurrentGauge = 0;
            int currentGaugeLevel = 0;

            for (int i = 0; i < energyBundles.Count; ++i)
            {
                int gaugeTypeToInt = GetEnergyBundleToInt(energyBundles[i]);
                if (i == 0 || currentGaugeLevel != gaugeTypeToInt)
                {
                    currentGaugeLevel = gaugeTypeToInt;
                    indexOfCurrentGauge = 0;
                }
                else
                    ++indexOfCurrentGauge;

                if (betCredit.value == energyBundles[i].GetValue<long>("bet"))
                    break;
            }

            int countOfCurrentGauge = GetCountOfGaugeEnergy(currentGaugeLevel);
            if (countOfCurrentGauge > 0)
                return currentGaugeLevel + (float)indexOfCurrentGauge / (float)countOfCurrentGauge;
            return 0;
        }

        public static int GetCountOfGaugeEnergy(int gaugeLevel)
        {
            int count = 0;
            List<Blackboard> energyBundles = EnergyBundleInfoList;

            for (int i = 0; i < energyBundles.Count; ++i)
            {
                if (GetEnergyBundleToInt(energyBundles[i]) == gaugeLevel)
                    ++count;
            }

            return count;
        }

        public static int GetEnergyBundleToInt(Blackboard bb)
        {
            int gaugeLevel = 0;

            if (bb != null)
            {
                var type = bb?.GetValue<BossRaidersEnergyBundleGaugeType>("gaugeType");
                if (type != null && type.Value != BossRaidersEnergyBundleGaugeType.UNKNOWN)
                {
                    gaugeLevel = (int)type.Value;
                    if (type.Value == BossRaidersEnergyBundleGaugeType.SMALL && bb.GetValue<long>("totalEnergyBundleEarning") == 0L)
                        gaugeLevel = 0;
                }
            }

            return gaugeLevel;
        }

        public static long GetResultGem(Blackboard bb)
        {
            long resultValue = 0;

            if (bb != null)
            {
                RewardType type = bb.GetValue<RewardType>("type");
                switch (type)
                {
                    case RewardType.CREDIT:
                        resultValue = bb.GetValue<long>("credit");
                        break;
                    case RewardType.GEM:
                        resultValue = bb.GetValue<long>("gem");
                        break;
                    case RewardType.BOSS_RAIDERS_ENERGY:
                        resultValue = bb.GetValue<int>("energy");
                        break;
                }
            }

            return resultValue;
        }

        public static BossRaidersHitType GetHitType()
        {
            Blackboard bb = WheelResultInfo;
            if (bb != null && bb.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.ATTACK)
                return bb.GetValue<BossRaidersHitType>("hitType");
            return BossRaidersHitType.UNKNOWN;
        }

        public static string GetPercentile()
        {
            int percentile = Percentile;
            return percentile > 0 ? percentile.ToString() : "--";
        }

        public static void SetDebugSpins(int debugIndex)
        {
            DebugSpinType = (BossRaidersDebugSpinType)debugIndex;
        }

        public static void UpdateBetMultiplyNumerator()
        {
            long currentEnergy = CurrentEnergy;
            List<int> betMultiplyNumeratorList = new List<int>();
            Dictionary<int, List<int>> maxBetMultiply = MaxBetMultiplyNumeratorList;

            betMultiplyNumeratorList.Add((int)NumberUtils.GetGlobalDenominator());

            if (maxBetMultiply != null)
            {
                foreach (var betMultiply in maxBetMultiply)
                {
                    if (betMultiply.Key <= currentEnergy)
                    {
                        for (int i = 0; i < betMultiply.Value.Count; ++i)
                            betMultiplyNumeratorList.Add(betMultiply.Value[i]);
                    }
                    else
                        break;
                }
            }

            BetMultiplyNumeratorList = betMultiplyNumeratorList;
            MaxBetMultiplyNumerator = betMultiplyNumeratorList[betMultiplyNumeratorList.Count - 1];
        }

        public static int GetDefaultBetMultiplyNumerator()
        {
            long currentEnergy = CurrentEnergy;
            Dictionary<int, int> defaultBetMultiply = DefaultBetMultiplyNumeratorList;

            int returnMultiply = (int)NumberUtils.GetGlobalDenominator();

            if (defaultBetMultiply != null)
            {
                foreach (var defaultMultiply in defaultBetMultiply)
                {
                    if (defaultMultiply.Key <= currentEnergy)
                        returnMultiply = defaultMultiply.Value;
                    else
                        break;
                }
            }

            return returnMultiply;
        }

        public static void CheckMetaStart()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BOSS_RAIDERS);
            if (eventInfo == null) return;

            long checkTime = eventInfo.startTimestamp + (TimeUtils.ONE_MIN_MS * 2);
            if (checkTime > TimeUtils.GetTimeStamp())
            {
                // Open after feed refactoring
                //if (PlayerPrefsUtils.GetInt64(BOSS_RAIDERS_LOCAL_PUSH, 0) < eventInfo.startTimestamp)
                //    MetaFeedUtils.SendBossRaidersFeed(StringTableUtils.GetString(StringTable.StringTableType.Global, "BOSS_RAIDERS_KUDO_META_GAME_START"));

                PlayerPrefsUtils.SetInt64(BOSS_RAIDERS_LOCAL_PUSH, eventInfo.startTimestamp);
            }
        }

        public static void ClearBlackboard()
        {
            ClearBlackboardData();
            InitBossRaiders();
        }

        public static void AddEnergys(int addEnergy)
        {
            Blackboard bb = BossRaidersInfo;
            if (bb == null || bb.GetValue<EventInfoType>("type") != EventInfoType.BOSS_RAIDERS) return;
            CurrentEnergy += addEnergy;
        }

        private static void ClearBlackboardData()
        {
            CurrentEnergy = 0;
        }

        public static bool IsClubber()
        {
            long clubId = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId")?.value ?? -1;
            return clubId > 0L;
        }
        // Use bossRaiders deal bundles
        public static bool GetBossRaidersDealBundles(int dealThemeId, bool isContents, ref string bundleName, ref string sharedBundleName, ref string characterBundleName)
        {
            bundleName = GetBossRaidersDealBundleName(dealThemeId, isContents);
            sharedBundleName = GetBossRaidersSharedBundleName(isContents);
            characterBundleName = GetBossRaidersCharacterBundleName(dealThemeId);
            return true;
        }

        public static string GetBossRaidersDealBundleName(int dealThemeId, bool isContents)
        {
            return StringTableUtils.GetString(StringTable.StringTableType.Global, isContents ? "BOSS_RAIDERS_ASSET_CONTENTS_DEAL" : "BOSS_RAIDERS_ASSET_COMMON_DEAL") + dealThemeId;
        }

        public static string GetBossRaidersSharedBundleName(bool isContents)
        {
            return StringTableUtils.GetString(StringTable.StringTableType.Global, isContents ? "BOSS_RAIDERS_ASSET_CONTENTS_SHARED" : "BOSS_RAIDERS_ASSET_COMMON_SHARED");
        }

        public static string GetBossRaidersCharacterBundleName(int themeId)
        {
            return string.Format("mgbossraiderscharacter{0}", themeId);
        }

        public static string GetBossRaidersMetaBundleName(int themeId, bool isContents)
        {
            return StringTableUtils.GetString(StringTable.StringTableType.Global, isContents ? "BOSS_RAIDERS_ASSET_CONTENTS" : "BOSS_RAIDERS_ASSET_COMMON") + themeId;
        }

        public static BossRaidersHitType GetDealHitType()
        {
            Blackboard dealBB = BlackboardUtils.FindValue<Blackboard>(MainBlackboard.Get(), BOSS_RAIDERS_DEAL_INFO);
            if (dealBB == null)
                return BossRaidersHitType.UNKNOWN;

            Blackboard bb = dealBB.GetValue<Blackboard>("wheelResultInfo");
            if (bb != null && bb.GetValue<BossRaidersWheelType>("type") == BossRaidersWheelType.ATTACK)
                return bb.GetValue<BossRaidersHitType>("hitType");
            return BossRaidersHitType.UNKNOWN;
        }

        public static long GetCurrentTotalBossHP()
        {
            Blackboard dealBB = BlackboardUtils.FindValue<Blackboard>(MainBlackboard.Get(), BOSS_RAIDERS_DEAL_INFO);
            if (dealBB != null)
            {
                Blackboard bb = dealBB.GetValue<Blackboard>("roundBalanceInfo");
                if (bb != null)
                    return bb.GetValue<long>("hpCoin");
            }
            return 1L;
        }

        public static void BIClientClickBossRaidersIcon(string contextId, string type)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextId;
            customData["type"] = type;
            customData["energy"] = CurrentEnergy;
            customData["theme_id"] = ThemeId;

            Analytics.CustomEvent("client_click_boss_raiders_icon", customData);
        }

        public static void BIClientBossRaidersPopup(string contextId, string type)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = type;
            customData["context_id"] = contextId;
            customData["energy"] = CurrentEnergy;
            customData["theme_id"] = ThemeId;

            Analytics.CustomEvent("client_boss_raiders_popup", customData);
        }

        public static void BIClientClickBossRaidersPopup(string contextId, string type)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = type;
            customData["context_id"] = contextId;
            customData["energy"] = CurrentEnergy;
            customData["theme_id"] = ThemeId;

            Analytics.CustomEvent("client_click_boss_raiders_popup", customData);
        }

        // todo : Deal AE
        public static void BIClientBossRaidersDealPopup(string contextId, string type)
        {
            //Dictionary<string, object> customData = new Dictionary<string, object>();
            //customData["type"] = type;
            //customData["context_id"] = contextId;
            //customData["energy"] = CurrentEnergy;
            //customData["theme_id"] = ThemeId;

            //Analytics.CustomEvent("client_boss_raiders_popup", customData);
        }

        public static void BIClientClickBossRaidersDealPopup(string contextId, string type)
        {
            //Dictionary<string, object> customData = new Dictionary<string, object>();

            //customData["type"] = type;
            //customData["context_id"] = contextId;
            //customData["energy"] = CurrentEnergy;
            //customData["theme_id"] = ThemeId;

            //Analytics.CustomEvent("client_click_boss_raiders_popup", customData);
        }
    }
}
