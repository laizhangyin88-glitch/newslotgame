using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine.UI;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class ClubArenaUtils
    {
        public class Sounds
        {
            public const string CLUB_ARENA_ENERGY_FLY = "Meta_ClubArena_EnergyFly";
            public const string CLUB_ARENA_ENERGY_GET = "Meta_ClubArena_EnergyGet";
            public const string CLUB_ARENA_START_BGM = "Meta_ClubArena_StartBGM";
            public const string CLUB_ARENA_MAIN_BGM = "Meta_ClubArena_MainBGM";
            public const string CLUB_ARENA_BET_BUTTON = "Meta_ClubArena_BetButton";
            public const string CLUB_ARENA_SPIN_BUTTON = "Meta_ClubArena_SpinButton";
            public const string CLUB_ARENA_WHEEL_START = "Meta_ClubArena_WheelStart";
            public const string CLUB_ARENA_WHEEL_SPIN = "Meta_ClubArena_WheelSpin";
            public const string CLUB_ARENA_WHEEL_STOP = "Meta_ClubArena_WheelStop";
            public const string CLUB_ARENA_BET_MULTI_KUDO = "Meta_ClubArena_BetMulti_Kudo";
            public const string CLUB_ARENA_POINT_FLY = "Meta_ClubArena_PointFly";
            public const string CLUB_ARENA_POINT_GET = "Meta_ClubArena_PointGet";
            public const string CLUB_ARENA_SHIELD_FLY = "Meta_ClubArena_ShieldFly";
            public const string CLUB_ARENA_SHIELD_GET = "Meta_ClubArena_ShieldGet";
            public const string CLUB_ARENA_BONUS_POPUP = "Meta_ClubArena_BonusPopup";
            public const string CLUB_ARENA_BONUS_PICK_CARD = "Meta_ClubArena_Bonus_PickCard";
            public const string CLUB_ARENA_BONUS_OTHER_CARD = "Meta_ClubArena_Bonus_OtherCard";
            public const string CLUB_ARENA_BONUS_PICK_CARD_ZOOM = "Meta_ClubArena_Bonus_PickCard_Zoom";
            //public const string CLUB_ARENA_BONUS_GET_REWARD = "Meta_ClubArena_Bonus_GetReward";
            public const string CLUB_ARENA_BONUS_MULTI = "Meta_ClubArena_Bonus_Multi";
            public const string CLUB_ARENA_BONUS_COLLECT_COIN = "Meta_ClubArena_Bonus_CollectCoin";
            public const string CLUB_ARENA_BONUS_COLLECT_GEM = "Meta_ClubArena_Bonus_CollectGem";
            public const string CLUB_ARENA_BONUS_COLLECT_ENERGY = "Meta_ClubArena_Bonus_CollectEnergy";
            public const string CLUB_ARENA_STEAL_START = "Meta_ClubArena_Steal_Start";
            public const string CLUB_ARENA_STEAL_END = "Meta_ClubArena_Steal_End";
            public const string CLUB_ARENA_ATTACK_FAIL = "Meta_ClubArena_AttackFail";
            public const string CLUB_ARENA_ATTACK_BROKE_SHIELD = "Meta_ClubArena_Attack_BrokeShield";
            public const string CLUB_ARENA_ATTACK_SUCCESS = "Meta_ClubArena_AttackSuccess";
            public const string CLUB_ARENA_MATCH_START = "Meta_ClubArena_RandomMatching_Start";
            public const string CLUB_ARENA_MATCH_END = "Meta_ClubArena_RandomMatching_End";
            public const string CLUB_ARENA_UPGRADE_START = "Meta_ClubArena_Upgrade_Start";
            public const string CLUB_ARENA_UPGRADE_END = "Meta_ClubArena_Upgrade_End";
            public const string CLUB_ARENA_REVENGE_KUDO = "Meta_ClubArena_RevengeKudo";
            public const string CLUB_ARENA_REVENGE_BUTTON = "Meta_ClubArena_RevengeButton";
            public const string CLUB_ARENA_POPUP_HELP = "Meta_ClubArena_HelpPopup";
            //public const string CLUB_ARENA_POPUP_HELP_APPEAR = "Meta_ClubArena_HelpPopup_Appear";
            public const string CLUB_ARENA_POPUP_HELP_CHEST = "Meta_ClubArena_HelpPopup_Chest";
            public const string CLUB_ARENA_MY_WARRIOR = "Meta_ClubArena_My_Warrior";
            public const string CLUB_ARENA_OPPONENT_WARRIOR = "Meta_ClubArena_Opponent_Warrior";
            //public const string CLUB_ARENA_WHEEL_ATTACK = "Meta_ClubArena_Wheel_Attack";
            //public const string CLUB_ARENA_WHEEL_SHIELD = "Meta_ClubArena_Wheel_Shield";
            //public const string CLUB_ARENA_WHEEL_STEAL = "Meta_ClubArena_Wheel_Steal";
            //public const string CLUB_ARENA_GET_POINTS = "Meta_ClubArena_getPoints";
            //public const string CLUB_ARENA_BATTLE_ATTACK = "Meta_ClubArena_Battle_Attack";
            //public const string CLUB_ARENA_BATTLE_INJURED = "Meta_ClubArena_Battle_Injured";
            //public const string CLUB_ARENA_BET_RESULT = "Meta_ClubArena_BetResult";
        }

        private const string CLUB_ARENA_GAME_INFO = "metaGameEnterInfo";

        public static string ON_REFRESH_EVENT = "OnClubArenaRefresh";

        public static readonly string CLUB_ARENA_LOCAL_PUSH = "CLUB_ARENA_LOCAL_PUSH";
        public static readonly string CLUB_ARENA_FIRST_VISIT = "CLUB_ARENA_FIRST_VISIT";
        public static readonly string CLUB_ARENA_REVENGE_USER_BUTTON_CLICK = "OnClickRevengeUser";
        public static readonly string CLUB_ARENA_REVENGE_USER_INDEX = "revengeIndex";
        public static readonly string CLUB_ARENA_POINT_TAKEN_COUNT = "CLUB_ARENA_POINT_TAKEN_COUNT";

        public static readonly int CLUB_ARENA_MAX_SHIELD = 100;
        public static readonly int CLUB_ARENA_MAX_HELP_SHOW_USER = 10;
        public static readonly int CLUB_ARENA_MAX_HELP_COUNT = 5;

        // Battle Callback key
        public static readonly string BATTLE_ANIMATION_REWARD_ENERGY = "RewardEnergy";
        public static readonly string BATTLE_ANIMATION_CHANGED_CHEST = "ChangedChest";
        public static readonly string BATTLE_ANIMATION_STEAL = "steal";
        public static readonly string BATTLE_ANIMATION_ATTACK = "attack";
        public static readonly string BATTLE_ANIMATION_SHIELD = "shield";
        public static readonly string BATTLE_ANIMATION_HIT = "hit";
        public static readonly string BATTLE_ANIMATION_DEAD = "dead";
        public static readonly string BATTLE_ANIMATION_MY_CLUB = "myClub";
        public static readonly string BATTLE_ANIMATION_OPPONENT_CLUB = "opponentClub";
        public static readonly string BATTLE_ANIMATION_DAMAGE = "clubDamage";

        // Other Animation Callback Key
        public static readonly string OPPONENT_ANIMATION_APPEAR = "opponentAppear";

        private static Blackboard clubArenaInfo = null;
        public static Blackboard ClubArenaInfo
        {
            get
            {
                if (clubArenaInfo == null)
                {
                    var info = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), CLUB_ARENA_GAME_INFO);
                    if (info != null)
                    {
                        var type = info.value.GetValue<EventInfoType>("type");
                        if (type == EventInfoType.CLUB_ARENA)
                        {
                            clubArenaInfo = info.value;
                        }
                    }
                }

                return clubArenaInfo;
            }
        }

        private static List<Blackboard> energyBundleInfoList;
        private static List<Blackboard> EnergyBundleInfoList
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
                    if (ClubArenaInfo != null)
                    {
                        var list = ClubArenaInfo.GetVariable<List<Blackboard>>("energyBundleInfoList");
                        if (list != null) energyBundleInfoList = list.value;
                    }
                }
                return energyBundleInfoList;
            }
        }

        public static long Energy
        {
            get
            {
                if (ClubArenaInfo != null)
                    return CurrentEnergy % RequiredEnergy;
                return 0;
            }
        }

        public static long CurrentEnergy
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("energy");
                return 0;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "energy", value);
            }
        }

        public static long RequiredEnergy
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("energyForSpin");
                return 0;
            }
        }

        public static long EarnEnergy
        {
            get
            {
                if (ClubArenaInfo != null)
                {
                    var earnEnergy = ClubArenaInfo.GetVariable<long>("energyEarning");
                    if (earnEnergy != null)
                        return earnEnergy.value;
                }

                return 0;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue<long>(ClubArenaInfo, "energyEarning", value);
            }
        }

        public static long SpinPossibleCount
        {
            get
            {
                if (ClubArenaInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<long>(ClubArenaInfo, "spinPossibleCount").value;
                return 0L;
            }

            set
            {
                if (ClubArenaInfo != null && value > -1)
                {
                    BlackboardUtils.SetOrCreateValue<long>(ClubArenaInfo, "spinPossibleCount", value);
                }
            }
        }

        public static long PrevEnergy
        {
            get
            {
                if (ClubArenaInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<long>(ClubArenaInfo, "prevEnergy")?.value ?? 0L;
                return 0L;
            }
        }

        public static long PrevCurrentEnergy
        {
            get
            {
                if (ClubArenaInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<long>(ClubArenaInfo, "prevCurrentEnergy")?.value ?? 0L;
                return 0L;
            }
        }

        public static long PrevSpinPossibleCount
        {
            get
            {
                if (ClubArenaInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<long>(ClubArenaInfo, "prevSpinPossibleCount")?.value ?? 0L;
                return 0L;
            }
        }

        public static List<Blackboard> WheelCandidateList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<List<Blackboard>>("wheelCandidateList");
                return null;
            }
        }

        private static Dictionary<ClubArenaSpinResultType, long> wheelBaseCandidateList;
        public static Dictionary<ClubArenaSpinResultType, long> WheelBaseCandidateList
        {
            get { return wheelBaseCandidateList; }
        }

        public static Dictionary<long, List<long>> MaxBetMultiplyNumeratorList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Dictionary<long, List<long>>>("maxBetMultiplyNumeratorByEnergyList");
                return null;
            }
        }

        public static Dictionary<long, long> DefaultBetMultiplyNumeratorList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Dictionary<long, long>>("defaultBetMultiplyNumeratorByEnergy");
                return null;
            }
        }

        public static long BaseBetMultiplyNumerator
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("baseBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "baseBetMultiplyNumerator", value);
            }
        }

        public static long CurrentBetMultiplyNumerator
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("currentBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "currentBetMultiplyNumerator", value);
            }
        }

        public static long MaxBetMultiplyNumerator
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("maxBetMultiplyNumerator");
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "maxBetMultiplyNumerator", value);
            }
        }

        public static long PreviousBetMultiplyNumerator
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetVariable<long>("previousBetMultiplyNumerator")?.value ?? NumberUtils.GetGlobalDenominator();
                return NumberUtils.GetGlobalDenominator();
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "previousBetMultiplyNumerator", value);
            }
        }

        public static List<long> BetMultiplyNumeratorList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<List<long>>("betMultiplyNumeratorList");
                return null;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "betMultiplyNumeratorList", value);
            }
        }

        public static long ClubArenaCooltime
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("videoAdsCooltime");
                return 0L;
            }
        }

        public static long LastVideoAdsClaimTimestamp
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("lastVideoAdsClaimTimestamp");
                return TimeUtils.GetTimeStamp();
            }
        }

        public static ClubArenaDebugSpinResultType DebugSpinType
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<ClubArenaDebugSpinResultType>("debugSpinType");
                return ClubArenaDebugSpinResultType.NONE;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "debugSpinType", value);
            }
        }

        public static int WheelResultIndex
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<int>("wheelResultIndex");
                return 0;
            }
        }

        public static Blackboard WheelResultInfo
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Blackboard>("result");
                return null;
            }
        }

        public static Blackboard MyState
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Blackboard>("myState");
                return null;
            }
        }

        private static ClubArenaPersonal myStateClass = null;
        public static ClubArenaPersonal MyStateClass
        {
            get
            {
                if (ClubArenaInfo != null)
                {
                    if (myStateClass == null)
                        myStateClass = new ClubArenaPersonal();
                    Blackboard bb = MyState;

                    myStateClass.userId = bb.GetValue<string>("userId");
                    myStateClass.eventId = bb.GetValue<int>("eventId");
                    myStateClass.point = bb.GetValue<long>("point");
                    myStateClass.energy = bb.GetValue<long>("energy");
                    myStateClass.shield = bb.GetValue<long>("shield");
                    myStateClass.lastEnterTimestamp = bb.GetValue<long>("lastEnterTimestamp");
                    myStateClass.opponentUserId = bb.GetValue<string>("opponentUserId");
                    myStateClass.lastMatchingTimestamp = bb.GetValue<long>("lastMatchingTimestamp");
                    myStateClass.leftHelpPopupCount = bb.GetValue<int>("leftHelpPopupCount");
                    myStateClass.lastPushTimestamp = bb.GetValue<long>("lastPushTimestamp");
                    myStateClass.leftPushCount = bb.GetValue<int>("leftPushCount");

                    return myStateClass;
                }
                return null;
            }
        }

        public static Blackboard OpponentState
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Blackboard>("opponentState");
                return null;
            }
            set
            {
                Blackboard bb = OpponentState;
                BlackboardUtils.SetOrCreateValue(bb, "userId", value.GetValue<string>("userId"));
                BlackboardUtils.SetOrCreateValue(bb, "point", value.GetValue<long>("point"));
                BlackboardUtils.SetOrCreateValue(bb, "energy", value.GetValue<long>("energy"));
                BlackboardUtils.SetOrCreateValue(bb, "shield", value.GetValue<long>("shield"));
                BlackboardUtils.SetOrCreateValue(bb, "name", value.GetValue<string>("name"));
                BlackboardUtils.SetOrCreateValue(bb, "profileUrl", value.GetValue<string>("profileUrl"));
                BlackboardUtils.SetOrCreateValue(bb, "clubId", value.GetValue<long>("clubId"));
                BlackboardUtils.SetOrCreateValue(bb, "tier", value.GetValue<int>("tier"));
                BlackboardUtils.SetOrCreateValue(bb, "matchContextId", value.GetValue<string>("matchContextId"));
            }
        }

        private static ClubArenaPersonalSimpleWithProfile opponentStateClass = null;
        public static ClubArenaPersonalSimpleWithProfile OpponentStateClass
        {
            get
            {
                if (ClubArenaInfo != null)
                {
                    if (opponentStateClass == null)
                        opponentStateClass = new ClubArenaPersonalSimpleWithProfile();
                    Blackboard bb = OpponentState;

                    opponentStateClass.userId = bb.GetValue<string>("userId");
                    opponentStateClass.point = bb.GetValue<long>("point");
                    opponentStateClass.energy = bb.GetValue<long>("energy");
                    opponentStateClass.shield = bb.GetValue<long>("shield");
                    opponentStateClass.name = bb.GetValue<string>("name");
                    opponentStateClass.profileUrl = bb.GetValue<string>("profileUrl");
                    opponentStateClass.clubId = bb.GetValue<long>("clubId");
                    opponentStateClass.tier = bb.GetValue<int>("tier");

                    return opponentStateClass;
                }
                return null;
            }
        }

        public static Blackboard ChestChangePoint
        {
            get
            {
                if (ClubArenaInfo != null)
                {
                    return ClubArenaInfo.GetValue<Blackboard>("chestImageChangePoint");
                }
                return null;
            }
        }

        public static int ClubRank
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<int>("clubRank");
                return 0;
            }
        }

        public static int Percentile
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<int>("percentile");
                return 0;
            }
        }

        public static List<Blackboard> ClubMemberContributionList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<List<Blackboard>>("clubMemberContribution");
                return null;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "clubMemberContribution", value);
            }
        }

        public static List<Blackboard> ClubRankInfoList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<List<Blackboard>>("clubRankingList");
                return null;
            }
            set
            {
                BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "clubRankingList", value);
            }
        }

        public static Dictionary<int, long> FinalRewardRank
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Dictionary<int, long>>("finalRewardRank");
                return null;
            }
        }

        public static Dictionary<int, long> FinalRewardPercentile
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Dictionary<int, long>>("finalRewardPercentile");
                return null;
            }
        }

        public static List<Blackboard> RevengeList
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<List<Blackboard>>("revengeList");
                return null;
            }
        }

        public static long AddedPoint
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("addedPoint");
                return 0;
            }
            set { BlackboardUtils.SetOrCreateValue<long>(ClubArenaInfo, "addedPoint", value); }
        }

        public static long StealPercentage
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("stealPercentage");
                return 0;
            }
            set { BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "stealPercentage", value); }
        }

        public static long ShieldAttackPoint
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("shieldAttackPoint");
                return 5;
            }
            set { BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "shieldAttackPoint", value); }
        }

        public static Blackboard ClubInfo
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Blackboard>("clubInfo");
                return null;
            }
        }

        public static Blackboard PointTaken
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<Blackboard>("pointTaken");
                return null;
            }
        }

        public static string TargetUserId
        {
            get
            {
                if (ClubArenaInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<string>(ClubArenaInfo, "targetUserId").value;
                return null;
            }
            set { BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "targetUserId", value); }
        }

        public static long MaximumShield
        {
            get
            {
                if (ClubArenaInfo != null)
                    return ClubArenaInfo.GetValue<long>("maximumShield");
                return 0;
            }
        }

        public static bool OpponentHasShield
        {
            get
            {
                if (ClubArenaInfo != null)
                    return BlackboardUtils.GetOrCreateVariable<bool>(ClubArenaInfo, "opponentHasShield").value;
                return false;
            }
        }

        public static string MatchContextID
        {
            get
            {
                if (OpponentState != null)
                    return BlackboardUtils.GetOrCreateVariable<string>(OpponentState, "matchContextId").value;
                return "";
            }
        }

        private static long myClubId = 0;
        public static long MyClubId
        {
            get { return myClubId; }
            set { myClubId = value; }
        }

        private static string myUserId = "";
        public static string MyUserId
        {
            get { return myUserId; }
            set { myUserId = value; }
        }

        public static void InitClubArena()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.CLUB_ARENA);
            if (eventInfo != null)
            {
                var eventID = BlackboardUtils.GetOrCreateVariable<int>(ClubArenaInfo, "id");
                if (eventID.value > 0 && eventID.value != eventInfo.id)
                    ClearBlackboardData();
                eventID.value = eventInfo.id;
            }
            UpdateClubArenaData();
        }

        public static void InitEnterResponse()
        {
            BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "baseBetMultiplyNumerator", NumberUtils.GetGlobalDenominator());
            BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "currentBetMultiplyNumerator", NumberUtils.GetGlobalDenominator());
            BlackboardUtils.SetOrCreateValue(ClubArenaInfo, "maxBetMultiplyNumerator", NumberUtils.GetGlobalDenominator());
            CurrentEnergy = MyState.GetValue<long>("energy");
            UpdateBetMultiplyNumerator();

            DebugSpinType = ClubArenaDebugSpinResultType.NONE;

            MyClubId = ClubInfo.GetValue<long>("id");
            MyUserId = BlackboardUtils.FindVariable<string>(null, "/me/userId").value;

            InitWheelBaseCandidateList();
        }

        private static void InitWheelBaseCandidateList()
        {
            if (wheelBaseCandidateList == null)
                wheelBaseCandidateList = new Dictionary<ClubArenaSpinResultType, long>();
            wheelBaseCandidateList.Clear();

            List<Blackboard> wheelCandidateList = WheelCandidateList;

            for (int i = 0; i < wheelCandidateList.Count; ++i)
            {
                ClubArenaSpinResultType type = wheelCandidateList[i].GetValue<ClubArenaSpinResultType>("type");
                if (wheelBaseCandidateList.ContainsKey(type)) continue;

                long baseValue = 0;
                switch (type)
                {
                    case ClubArenaSpinResultType.POINT_10:
                    case ClubArenaSpinResultType.POINT_50:
                        baseValue = wheelCandidateList[i].GetValue<long>("point");
                        break;
                    case ClubArenaSpinResultType.SHIELD:
                        baseValue = 1;
                        break;
                    case ClubArenaSpinResultType.ATTACK_10:
                    case ClubArenaSpinResultType.ATTACK_20:
                    case ClubArenaSpinResultType.ATTACK_50:
                    case ClubArenaSpinResultType.ATTACK_100:
                        baseValue = wheelCandidateList[i].GetValue<long>("attack");
                        break;
                }
                wheelBaseCandidateList.Add(type, baseValue);
            }
        }

        public static void UpdateClubArenaData()
        {
            SpinPossibleCount = CurrentEnergy / RequiredEnergy;
        }

        public static void AddEnergys(long addEnergy)
        {
            Blackboard bb = ClubArenaInfo;
            if (bb == null || bb.GetValue<EventInfoType>("type") != EventInfoType.CLUB_ARENA) return;
            CurrentEnergy += addEnergy;
        }

        public static void BackUpInfo()
        {
            if (clubArenaInfo == null) return;

            BlackboardUtils.SetOrCreateValue<long>(clubArenaInfo, "prevEnergy", Energy);
            BlackboardUtils.SetOrCreateValue<long>(clubArenaInfo, "prevCurrentEnergy", CurrentEnergy);
            BlackboardUtils.SetOrCreateValue<long>(clubArenaInfo, "prevSpinPossibleCount", SpinPossibleCount);
        }

        public static void CheckMetaStart()
        {
            EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CLUB_ARENA);
            if (eventInfo == null)
                return;

            long checkTime = eventInfo.startTimestamp + (TimeUtils.ONE_MIN_MS * 2);
            if (checkTime > TimeUtils.GetTimeStamp())
            {
                if (PlayerPrefsUtils.GetInt64(CLUB_ARENA_LOCAL_PUSH, 0) < eventInfo.startTimestamp)
                    MetaFeedUtils.SendClubArenaStart();
                PlayerPrefsUtils.SetInt64(CLUB_ARENA_LOCAL_PUSH, eventInfo.startTimestamp);
            }
        }

        public static void CheckEnergyForSpinResult()
        {
            Blackboard bb = WheelResultInfo;
            ClubArenaSpinResultType resultType = bb.GetValue<ClubArenaSpinResultType>("type");

            if (resultType == ClubArenaSpinResultType.SHIELD)
            {
                long resultEnergy = bb.GetValue<long>("energy");
                if (resultEnergy > 0)
                    AddEnergys(resultEnergy * -1);
            }
            else if (resultType == ClubArenaSpinResultType.BONUS)
            {
                int bonusIndex = bb.GetValue<int>("bonusIndex");
                List<Blackboard> bonusInfoList = bb.GetValue<List<Blackboard>>("bonusInfoList");
                if (bonusInfoList[bonusIndex].GetValue<ClubArenaBonusType>("bonusType") == ClubArenaBonusType.ENERGY)
                {
                    long rewardResultAmount = NumberUtils.GetMultiplierNumeratorValue(bonusInfoList[bonusIndex].GetValue<long>("amount"), CurrentBetMultiplyNumerator);
                    AddEnergys(rewardResultAmount * -1);
                }
            }
        }

        public static bool IsUnlockedLevel()
        {
            return MetaGameUtils.IsMetaGameLevelLocked();
        }

        public static void ClearBlackboard()
        {
            ClearBlackboardData();
            InitClubArena();
        }

        private static void ClearBlackboardData()
        {
            CurrentEnergy = 0;
        }

        public static List<SlotMaker.TestSuite.DebugSpin> GetDebugSpins()
        {
            List<SlotMaker.TestSuite.DebugSpin> debugSpins = new List<SlotMaker.TestSuite.DebugSpin>();

            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.NONE, description = ClubArenaDebugSpinResultType.NONE.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.POINT, description = ClubArenaDebugSpinResultType.POINT.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.SHIELD, description = ClubArenaDebugSpinResultType.SHIELD.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.ATTACK, description = ClubArenaDebugSpinResultType.ATTACK.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.STEAL, description = ClubArenaDebugSpinResultType.STEAL.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.BONUS_COIN, description = ClubArenaDebugSpinResultType.BONUS_COIN.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.BONUS_GEM, description = ClubArenaDebugSpinResultType.BONUS_GEM.ToString(), DebugSequenceList = null });
            debugSpins.Add(new SlotMaker.TestSuite.DebugSpin() { code = (int)ClubArenaDebugSpinResultType.BONUS_ENERGY, description = ClubArenaDebugSpinResultType.BONUS_ENERGY.ToString(), DebugSequenceList = null });

            return debugSpins;
        }

        public static void SetDebugSpins(int debugIndex)
        {
            DebugSpinType = (ClubArenaDebugSpinResultType)debugIndex;
        }

        public static string GetClubRank()
        {
            int clubRank = ClubRank;
            return clubRank > 0 ? clubRank.ToString() : "--";
        }

        public static string GetClubPercentile()
        {
            int percentile = Percentile;
            return percentile > 0 ? percentile.ToString() : "--";
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
                var type = bb?.GetValue<ClubArenaEnergyBundleType>("bundleType");
                if (type != null && type.Value != ClubArenaEnergyBundleType.UNKNOWN)
                {
                    gaugeLevel = (int)type.Value;
                    if (type.Value == ClubArenaEnergyBundleType.SMALL && bb.GetValue<long>("maximumEnergy") == 0)
                        gaugeLevel = 0;
                }
            }

            return gaugeLevel;
        }

        public static Blackboard GetEnergyBundleInfoBB(int idx)
        {
            if (ClubArenaInfo == null || EnergyBundleInfoList == null) return null;

            List<Blackboard> energyBundles = EnergyBundleInfoList;
            Blackboard retBB = null;

            if (idx < energyBundles.Count)
            {
                if (energyBundles[idx].GetValue<long>("maximumEnergy") > 0)
                    retBB = energyBundles[idx];
            }

            return retBB;
        }

        public static Blackboard GetEnergyBundleInfoBB(long betCredit)
        {
            if (ClubArenaInfo == null || EnergyBundleInfoList == null) return null;

            List<Blackboard> energyBundles = EnergyBundleInfoList;
            Blackboard retBB = null;

            for (int i = 0; i < energyBundles.Count; ++i)
            {
                long unitBet = energyBundles[i].GetValue<long>("bet");
                if (betCredit < unitBet) continue;
                retBB = energyBundles[i];
            }

            return retBB;
        }

        public static bool GetEqualsClubId(Blackboard bb)
        {
            // bb => ClubRankInfoList data
            if (bb == null)
                return false;
            Variable<long> id = bb.GetVariable<long>("clubId");
            if (id == null || id.value == 0)
                return false;
            return MyClubId == id.value;
        }

        public static bool GetEqualsUserId(Blackboard bb)
        {
            // bb => ClubMemberContributionList data
            if (bb == null)
                return false;
            Variable<string> id = bb.GetVariable<string>("userId");
            if (id == null || id.value == null)
                return false;
            return string.Equals(MyUserId, id.value);
        }

        public static void UpdateBetMultiplyNumerator()
        {
            long currentEnergy = CurrentEnergy;
            List<long> betMultiplyNumeratorList = new List<long>();
            Dictionary<long, List<long>> maxBetMultiply = MaxBetMultiplyNumeratorList;

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

        public static long GetDefaultBetMultiplyNumerator()
        {
            long currentEnergy = CurrentEnergy;
            Dictionary<long, long> defaultBetMultiply = DefaultBetMultiplyNumeratorList;

            long returnMultiply = NumberUtils.GetGlobalDenominator();

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

        public static int GetBattleAnimationIndex()
        {
            List<long> betList = BetMultiplyNumeratorList;
            long currentBet = CurrentBetMultiplyNumerator;
            if (betList == null || betList.Count < 2)
                return 1;
            else if (betList.Count == 2) // Count 2
                return currentBet < betList[1] ? 1 : 2;
            else
            {
                int firstIndex = (int)(betList.Count * 0.33f);
                int secondIndex = (int)(betList.Count * 0.66f);
                if (currentBet <= betList[firstIndex])
                    return 1;
                else if (currentBet <= betList[secondIndex])
                    return 2;
                else
                    return 3;
            }
        }

        public static ClubArenaDebugSpinResultType GetResultDebugSpinType()
        {
            Blackboard bb = WheelResultInfo;
            if (bb != null)
            {
                switch (bb.GetValue<ClubArenaSpinResultType>("type"))
                {
                    case ClubArenaSpinResultType.POINT_10:
                    case ClubArenaSpinResultType.POINT_50:
                        return ClubArenaDebugSpinResultType.POINT;
                    case ClubArenaSpinResultType.SHIELD:
                        return ClubArenaDebugSpinResultType.SHIELD;
                    case ClubArenaSpinResultType.ATTACK_10:
                    case ClubArenaSpinResultType.ATTACK_20:
                    case ClubArenaSpinResultType.ATTACK_50:
                    case ClubArenaSpinResultType.ATTACK_100:
                        return ClubArenaDebugSpinResultType.ATTACK;
                    case ClubArenaSpinResultType.STEAL:
                        return ClubArenaDebugSpinResultType.STEAL;
                    case ClubArenaSpinResultType.BONUS:
                        List<Blackboard> bonusInfoList = bb.GetValue<List<Blackboard>>("bonusInfoList");
                        int bonusIndex = bb.GetValue<int>("bonusIndex");
                        if (bonusInfoList != null && bonusInfoList.Count > 0)
                        {
                            ClubArenaBonusType rewardType = bonusInfoList[bonusIndex].GetValue<ClubArenaBonusType>("bonusType");
                            if (rewardType == ClubArenaBonusType.COIN)
                                return ClubArenaDebugSpinResultType.BONUS_COIN;
                            else if (rewardType == ClubArenaBonusType.GEM)
                                return ClubArenaDebugSpinResultType.BONUS_GEM;
                            else if (rewardType == ClubArenaBonusType.ENERGY)
                                return ClubArenaDebugSpinResultType.BONUS_ENERGY;
                        }
                        break;
                }
            }

            return ClubArenaDebugSpinResultType.UNKNOWN;
        }

        public static ClubArenaSpinResultType GetResultSpinType()
        {
            Blackboard bb = WheelResultInfo;
            if (bb != null)
                return bb.GetValue<ClubArenaSpinResultType>("type");
            return ClubArenaSpinResultType.UNKNOWN;
        }

        public static Blackboard GetRevengeUser(int index)
        {
            List<Blackboard> revengeUsers = RevengeList;
            if (revengeUsers == null || revengeUsers.Count == 0 || revengeUsers.Count <= index) return null;
            return revengeUsers[index];
        }

        public static long GetWheelBaseCandidateData(ClubArenaSpinResultType type)
        {
            if (wheelBaseCandidateList == null || !wheelBaseCandidateList.ContainsKey(type)) return 0;
            return wheelBaseCandidateList[type];
        }

        public static void RemoveRevengeList(int index)
        {
            BlackboardUtils.RemoveAtBlackboardList(ClubArenaInfo, "revengeList", index);
        }

        public static bool IsFirstEnter()
        {
            bool isFirstEnter = false;

            if (ClubArenaInfo != null)
            {
                var checkValue = ClubArenaInfo.GetVariable<long>("maximumShield");
                isFirstEnter = (checkValue == null);
            }

            return isFirstEnter;
        }

        public static bool IsClubber()
        {
            long clubId = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId")?.value ?? -1;
            return clubId > 0L;
        }

        public static long GetOpponentAttackDamage(long damage)
        {
            Blackboard opponentBB = OpponentState;
            long opponentShield = opponentBB.GetValue<long>("shield");
            bool hasShield = OpponentHasShield;
            return hasShield ? ShieldAttackPoint : damage;
        }

        public static void SetOpponentAttackDamage(long damage)
        {
            Blackboard opponentBB = OpponentState;
            long opponentShield = opponentBB.GetValue<long>("shield");
            long opponentPoint = opponentBB.GetValue<long>("point");
            long attackBasePoint = ShieldAttackPoint;
            bool hasShield = OpponentHasShield;

            if (hasShield)
                attackBasePoint = ShieldAttackPoint;
            else
            {
                attackBasePoint = opponentPoint > damage ? damage : opponentPoint;
                opponentPoint -= damage;
                opponentBB.SetValue("point", opponentPoint > 0L ? opponentPoint : 0L);
            }
            BlackboardUtils.SetOrCreateValue(opponentBB, "basePoint", attackBasePoint);
        }

        public static long GetOpponentStealDamage()
        {
            Blackboard opponentBB = OpponentState;

            long opponentPoint = opponentBB.GetValue<long>("point");
            return (long)System.Math.Ceiling((double)(opponentPoint * StealPercentage) / NumberUtils.GetGlobalDenominator());
        }

        public static void SetOpponentStealDamage()
        {
            Blackboard opponentBB = OpponentState;

            long opponentPoint = opponentBB.GetValue<long>("point");
            long stealBasePoint = GetOpponentStealDamage();
            BlackboardUtils.SetOrCreateValue(opponentBB, "basePoint", stealBasePoint);

            opponentBB.SetValue("point", (opponentPoint - stealBasePoint) > 0L ? opponentPoint - stealBasePoint : 0L);
        }

        public static long GetOpponentBasePoint()
        {
            return OpponentState.GetValue<long>("basePoint");
        }

        public static string InfoPointNumberFormat(long number)
        {
            long _1M = 1000000;
            long _1000B = 1000000000000;
            string _100B_FORMAT = "#,000M";

            if (number < _1000B)
                return number.ToString("N0");
            else
                return (number / _1M).ToString(_100B_FORMAT);
        }

        public static string RevengePointNumberFormat(long number)
        {
            long _1B = 1000000000;
            long _10B = 10000000000;
            long _100B = 100000000000;
            string _1B_FORMAT = "0.00B";
            string _10B_FORMAT = "00.0B";
            string _100B_FORMAT = "#,000B";

            if (number < _1B)
                return number.ToString("N0");
            else if (number < _10B)
                return (((double)number / _1B) - 0.005).ToString(_1B_FORMAT);
            else if (number < _100B)
                return (((double)number / _1B) - 0.05).ToString(_10B_FORMAT);

            return (number / _1B).ToString(_100B_FORMAT);
        }

        public static int PointTakenCount()
        {
            string todayDate = GetTodayDateTime();
            string prefsValue = PlayerPrefs.GetString(CLUB_ARENA_POINT_TAKEN_COUNT, (todayDate + ":0"));
            string[] splitData = prefsValue.Split(':');
            // 0 : yyyyMMdd / 1 : Count Numer (max 5)
            if (splitData != null && splitData.Length == 2)
            {
                int todayTime = Convert.ToInt32(todayDate);
                int prefsTime = Convert.ToInt32(splitData[0]);
                if (todayTime > prefsTime)
                    return 0;
                else if (todayTime == prefsTime && Convert.ToInt32(splitData[1]) < CLUB_ARENA_MAX_HELP_COUNT)
                    return Convert.ToInt32(splitData[1]);
            }
            return 0;
        }

        public static void AddPointTakenCount()
        {
            string todayDate = GetTodayDateTime();
            string prefsValue = PlayerPrefs.GetString(CLUB_ARENA_POINT_TAKEN_COUNT, (todayDate + ":0"));
            string[] splitData = prefsValue.Split(':');
            int takenCount = 0;
            if (splitData != null && splitData.Length == 2)
            {
                int todayTime = Convert.ToInt32(todayDate);
                int prefsTime = Convert.ToInt32(splitData[0]);

                if (todayTime == prefsTime)
                    takenCount = Convert.ToInt32(splitData[1]);
            }
            PlayerPrefs.SetString(CLUB_ARENA_POINT_TAKEN_COUNT, (todayDate + ":" + ++takenCount));
        }

        public static string GetTodayDateTime()
        {
            return string.Format("{0:yyyyMMdd}", TimeUtils.GetCurrentDateTime());
        }

        public static void SetKudoActive(bool isActive)
        {
            BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "isKudoActive", isActive);
        }

        public static void BIClientClubArenaPopup(string popupType, string contextID, string userIdList = null)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["match_context_id"] = MatchContextID;
            customData["context_id"] = contextID;
            customData["type"] = popupType;
            customData["target_user_id_list"] = userIdList;
            customData["energy"] = CurrentEnergy;

            Analytics.CustomEvent("client_club_arena_popup", customData);
        }

        public static void BIClientClickClubArenaPopup(string popupType, string contextID)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["match_context_id"] = MatchContextID;
            customData["context_id"] = contextID;
            customData["type"] = popupType;
            customData["energy"] = CurrentEnergy;

            Analytics.CustomEvent("client_click_club_arena_popup", customData);
        }

        public static void BIClientClickClubArenaIcon(string contextId, string type)
        {
            var customData = new Dictionary<string, object>();

            customData["context_id"] = contextId;
            customData["type"] = type;
            customData["energy"] = CurrentEnergy;

            Analytics.CustomEvent("client_click_club_arena_icon", customData);
        }
    }
}
