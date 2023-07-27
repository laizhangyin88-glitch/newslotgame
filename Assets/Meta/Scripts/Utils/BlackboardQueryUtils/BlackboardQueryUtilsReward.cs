using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;

using static BagelCode.MysteryTest;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        public static void ApplyRewardResult(List<Blackboard> rewardResultBBList)
        {
            for (int i = 0; i < rewardResultBBList.Count; ++i)
            {
                ApplyRewardResult(rewardResultBBList[i]);
            }
        }

        public static void ApplyRewardResult(Blackboard rewardResultBB, bool isInbox = false)
        {
            // Apply Credits, RP, Spin, Boost, Collecting Game Pack
            var isSendToInbox = BlackboardUtils.FindVariable<bool>(rewardResultBB, "isSentToInbox");
            if (isSendToInbox.value) return;

            var rewardType = BlackboardUtils.FindVariable<RewardType>(rewardResultBB, "rewardType");

            switch (rewardType.value)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        var credit = BlackboardUtils.FindVariable<long>(rewardResultBB, "credit");
                        if (credit != null)
                            AddCoins(credit.value);
                    }
                    break;
                case RewardType.RP:
                    {
                        var rp = BlackboardUtils.FindVariable<long>(rewardResultBB, "rp");
                        if (rp != null)
                            AddRP(rp.value);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        var totalSpinCount = BlackboardUtils.FindVariable<int>(rewardResultBB, "totalSpinCount");
                        if (totalSpinCount != null)
                            SetDailySpinCount(totalSpinCount.value, MetaJackpotType.DAILY_BONUS);
                    }
                    break;
                case RewardType.PURCHASE_COUPON:
                    {

                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        var gameID = BlackboardUtils.FindVariable<int>(rewardResultBB, "gameId");
                        var addedSpinCount = BlackboardUtils.FindVariable<int>(rewardResultBB, "addedSpinCount");

                        if (gameID != null && addedSpinCount != null)
                            AddGameSpinCount(gameID.value, addedSpinCount.value);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        var gameID = BlackboardUtils.FindVariable<int>(rewardResultBB, "gameId");
                        var addedSpinCount = BlackboardUtils.FindVariable<int>(rewardResultBB, "addedCount");

                        if (gameID != null && addedSpinCount != null)
                            AddGameSpinCount(gameID.value, addedSpinCount.value);
                    }
                    break;
                case RewardType.GAME_PLAY:
                    {
                        var gameID = BlackboardUtils.FindVariable<int>(rewardResultBB, "gameId");
                        var addedSpinCount = BlackboardUtils.FindVariable<int>(rewardResultBB, "addedCount");

                        if (gameID != null && addedSpinCount != null)
                            AddGameSpinCount(gameID.value, addedSpinCount.value);
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        var earnCredit = BlackboardUtils.FindVariable<long>(rewardResultBB, "earnCredit");
                        var earnGem = BlackboardUtils.FindVariable<long>(rewardResultBB, "earnGem");
                        var dailyBoostInfo = BlackboardUtils.FindVariable<Blackboard>(rewardResultBB, "dailyBoost");

                        if (earnCredit != null && dailyBoostInfo != null)
                        {
                            AddCoins(earnCredit.value);
                            AddGems(earnGem.value);
                            ApplyDailyBoostInfo(dailyBoostInfo.value);
                            UpdateDailyBoostState();
                        }
                    }
                    break;
                case RewardType.TIER_UPGRADE:
                    {
                        int tier = TierUtils.GetMeTier();
                        int rewardTier = BlackboardUtils.FindVariable<int>(rewardResultBB, "rewardInfo/targetTier")?.value ?? 0;
                        int targetTier = Mathf.Max(tier, rewardTier);
                        TierUtils.SetTier(targetTier);
                    }
                    break;
                case RewardType.SCRATCHER:
                    {
                        var credit = BlackboardUtils.FindVariable<long>(rewardResultBB, "credit");
                        AddCoins(credit.value);
                    }
                    break;
                case RewardType.COLLECTING_GAME_PACK:
                    {
                        var rewardInfo = rewardResultBB.GetValue<Blackboard>("rewardInfo");
                        var packId = rewardInfo.GetValue<int>("packId");
                        var count = rewardInfo.GetValue<int>("count");
                        var collectingGameId = rewardInfo.GetValue<int>("collectingGameId");

                        EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME, true);
                        if (eventInfo != null)
                        {
                            int eventId = ((EventDataCollectingGame)eventInfo.constraints).collectingGameId;

                            bool isLockedFeature = IsLockedFeature(LockedFeatureType.META_GAME);
                            if (!isLockedFeature && collectingGameId == eventId)
                                AddPackPossessions(packId, count);
                        }
                    }
                    break;
                case RewardType.GEM:
                    {
                        var gem = BlackboardUtils.FindVariable<long>(rewardResultBB, "gem");
                        if (gem != null)
                            AddGems(gem.value);

                        break;
                    }
                case RewardType.MEGA_WHEEL_SPIN:
                    {
                        var totalSpinCount = BlackboardUtils.FindVariable<int>(rewardResultBB, "totalSpinCount");
                        if (totalSpinCount != null)
                            SetDailySpinCount(totalSpinCount.value, MetaJackpotType.DAILY_MEGA_WHEEL);
                    }
                    break;
                case RewardType.BOSS_RAIDERS_ENERGY:
                    var energy = BlackboardUtils.FindVariable<int>(rewardResultBB, "energy");
                    BossRaidersUtils.AddEnergys(energy.value);
                    break;
                case RewardType.CLUB_ARENA_ENERGY:
                    var clubArenaEnergy = BlackboardUtils.FindVariable<long>(rewardResultBB, "energy");
                    ClubArenaUtils.AddEnergys(clubArenaEnergy.value);
                    break;
                case RewardType.HIDDEN_UNIVERSE_FINDER:
                    var earnFinder = BlackboardUtils.FindVariable<int>(rewardResultBB, "earnFinder");
                    AddFinders(earnFinder.value);
                    break;
                case RewardType.VIP_LOUNGE_OPEN_TICKET:
                    Blackboard vipLoungeInfoBB = BlackboardUtils.FindVariable<Blackboard>(rewardResultBB, "vipLoungeInfo")?.value ?? null;
                    UpdateVIPLoungeInfo(vipLoungeInfoBB);
                    break;
                case RewardType.DEPOT:
                    Blackboard vegasDreamsInfoBB = BlackboardUtils.FindVariable<Blackboard>(rewardResultBB, "buildDreamInfo")?.value ?? null;
                    UpdateVegasDreamsInfo(vegasDreamsInfoBB);
                    UpdateDepotCount(rewardResultBB);
                    break;
                case RewardType.WILD_PUZZLE:
                    UpdateWildPuzzleCount(rewardResultBB);
                    break;
                case RewardType.EXP_MULTIPLY_EXTENDABLE:
                    {
                        var rewardInfo = rewardResultBB.GetValue<Blackboard>("rewardInfo");
                        if (rewardInfo != null)
                        {
                            int durationSec = BlackboardUtils.FindVariable<int>(rewardResultBB, "durationSec")?.value ?? 0;
                            bool isExtended = BlackboardUtils.FindVariable<bool>(rewardResultBB, "isExtended")?.value ?? false;

                            if (isExtended)
                            {
                                // Event Id가 바뀌지 않아서 갱신 안되므로 수동 연장
                                var extendableEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.EXP_MULTIPLY_EXTENDABLE);
                                if (extendableEventInfo != null)
                                {
                                    long current = TimeUtils.GetTimeStamp();
                                    extendableEventInfo.endTimestamp = current + durationSec * 1000L;

                                    var eventData = new EventData<int>(MetaEventDefine.REFRESH_PASSIVE_EVENT, extendableEventInfo.id);
                                    EventSender.SendGlobalEvent(MetaEventDefine.ON_PASSIVE_EVENT, eventData);
                                }
                            }
                        }
                    }
                    break;
                case RewardType.SCRATCHER_FOR_INBOX:
                case RewardType.PROGRAMMED_WIN:
                case RewardType.EXP_MULTIPLY:
                case RewardType.RANDOM:
                case RewardType.SOCIAL_CREDIT:
                case RewardType.TICKETED_BONUS_TICKET:
                case RewardType.CLUB_CREDIT:
                case RewardType.DAILY_DELIVERY:
                case RewardType.SEASON_PASS_POINT:
                case RewardType.SPIN_DEAL:
                case RewardType.TICKETED_BONUS_TICKET_FOR_BOOST:
                    // Nothing to do.
                    break;
#if DEV
                default:
                    Debug.LogError(string.Format("RewardType({0}) has not been processed.", rewardType.value));
                    break;
#endif
            }

        }

        public static long GetRewardCoin(List<Blackboard> rewardResultBBList)
        {
            long rewardCoins = 0;
            for (int i = 0; i < rewardResultBBList.Count; ++i)
            {
                rewardCoins += GetRewardCoin(rewardResultBBList[i], false);
            }

            return rewardCoins;
        }

        public static long GetRewardCoin(Blackboard rewardBB, bool isRewardInfo)
        {
            // Apply Credits, RP, Spin, Boost

            RewardType rewardType;
            if (isRewardInfo)
                rewardType = BlackboardUtils.FindValue<RewardType>(rewardBB, "type");
            else
                rewardType = BlackboardUtils.FindValue<RewardType>(rewardBB, "rewardType");

            var viewAd = BlackboardUtils.FindVariable<bool>(rewardBB, "viewAd");
            if (viewAd != null && viewAd.value == true)
                return 0;

            switch (rewardType)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        var credit = BlackboardUtils.FindVariable<long>(rewardBB, "credit");
                        var applyTierMultiplier = BlackboardUtils.FindVariable<bool>(rewardBB, "applyTierMultiplier");
                        var applyLevelMultiplier = BlackboardUtils.FindVariable<bool>(rewardBB, "applyLevelMultiplier");

                        if (credit != null)
                        {
                            long creditCoin = credit.value;
                            if (applyTierMultiplier != null && applyTierMultiplier.value == true)
                                creditCoin = TierUtils.GetTierFractionCoin(creditCoin, TierUtils.GetMeTier());
                            if (applyLevelMultiplier != null && applyLevelMultiplier.value == true)
                                creditCoin = LevelUtils.GetLevelMultiplierNumeratorValue(creditCoin, "coin");
                            return creditCoin;
                        }
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        if (!isRewardInfo)
                        {
                            var earnCredit = BlackboardUtils.FindVariable<long>(rewardBB, "earnCredit");
                            var dailyBoostInfo = BlackboardUtils.FindVariable<Blackboard>(rewardBB, "dailyBoost");

                            if (earnCredit != null && dailyBoostInfo != null)
                            {
                                return earnCredit.value;
                            }
                        }
                    }
                    break;
            }

            return 0;
        }
    }
}
