using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using System.Linq;
using UnityEngine;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static string GetChallengeProgressText(Blackboard missionInfo, out float progress, out bool isComplete)
        {
            string result;

            long currentCount = BlackboardUtils.FindVariable<long>(missionInfo, "progress").value;
            long completeCount = BlackboardUtils.FindVariable<long>(missionInfo, "completeCount").value;

            progress = (float)((double)currentCount / (double)completeCount);

            isComplete = currentCount == completeCount;

            ChallengeMissionType missionType = BlackboardUtils.FindVariable<ChallengeMissionType>(missionInfo, "missionType").value;

            const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

            switch (missionType)
            {
                // Credit (more than)
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_ANY:
                case ChallengeMissionType.WIN_CREDIT_MORE_THAN_TARGETED:
                    {
                        long baseCredit = 0;
                        var targetCredit = BlackboardUtils.FindVariable<long>(missionInfo, "targetCredit");
                        if (currentCount == completeCount)
                            baseCredit = targetCredit.value;

                        result = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", baseCredit, targetCredit.value);
                    }
                    break;
                // Currency (more than)
                case ChallengeMissionType.PURCHASE_PRICE_MORE_THAN:
                    {
                        var targetPrice = BlackboardUtils.FindVariable<long>(missionInfo, "targetPrice");
                        double currentCurrency = 0.0;
                        double completeCurrency = (double)targetPrice.value / 100.0;
                        if (currentCount == completeCount)
                            currentCurrency = completeCurrency;

                        result = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_CURRENCY", currentCurrency, completeCurrency);
                    }
                    break;
                // Currency
                case ChallengeMissionType.ACHIEVE_TOTAL_PURCHASE_PRICE:
                    {
                        double currentCurrency = (double)currentCount / 100.0;
                        double completeCurrency = (double)completeCount / 100.0;
                        result = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_CURRENCY", currentCurrency, completeCurrency);
                    }
                    break;
                // Credit
                case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WAGER_CREDIT_ANY_WITH_BET_LIMIT:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY_WITH_BET_LIMIT:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_TARGETED:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_BIG_WIN_TARGETED:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_ANY:
                case ChallengeMissionType.ACHIEVE_TOTAL_WIN_CREDIT_BY_FREE_SPIN_TARGETED:
                    result = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_COIN", currentCount, completeCount);
                    break;
                default:
                    result = StringTableUtils.GetString(GLOBAL, "POPUP_CHALLENGE_MISSION_PROGRESS_DEFAULT", currentCount, completeCount);
                    break;
            }

            return result;
        }

        public static void UpdateChallengeSimpleInfo(Blackboard simpleInfo)
        {
            var personalChallengeType = simpleInfo.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
            var clubChallengeType = simpleInfo.GetVariable<ClubChallengeType>("type")?.value ?? ClubChallengeType.UNKNOWN;

            bool isPersonalChallenge = personalChallengeType != ChallengeType.UNKNOWN;

            if (isPersonalChallenge)
            {
                long startTimestamp = simpleInfo.GetValue<long>("startTimestamp");
                long endTimestamp = simpleInfo.GetValue<long>("endTimestamp");

                // Search Simple Info
                Blackboard targetInfo = null;
                var challengeInfoList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "challengeInfoList");
                foreach (var info in challengeInfoList)
                {
                    var infoChallengeType = info.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
                    long oldStartTimestamp = info.GetValue<long>("startTimestamp");
                    if (personalChallengeType == infoChallengeType)
                    {
                        if (startTimestamp < oldStartTimestamp) return;

                        targetInfo = info;
                        break;
                    }
                }

                // New Simple Info
                int prevProgress = 0, prevMaxProgress = 0;
                bool prevDone = false, prevClaimed = false;
                if (targetInfo == null)
                {
                    targetInfo = (Blackboard)BlackboardUtils.CreateBlackboard("ChallengeInfoSimple");
                    BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "challengeInfoList", targetInfo);

                    targetInfo.AddVariable("challengeType", personalChallengeType);
                    string challengeId = simpleInfo.GetValue<string>("challengeId");
                    targetInfo.AddVariable("challengeId", challengeId);

                    // Event
                    if(personalChallengeType == ChallengeType.EVENT)
                    {
                        bool isCompleteToUnlock = simpleInfo.GetValue<bool>("isCompleteToUnlock");
                        targetInfo.AddVariable("isCompleteToUnlock", isCompleteToUnlock);
                    }

                    // Copy Rewards
                    var rewardList = simpleInfo.GetValue<List<Blackboard>>("rewardList");
                    BlackboardUtils.GetOrCreateBlackboardList(targetInfo, "rewardList");
                    foreach (var reward in rewardList)
                    {
                        var newReward = (Blackboard)BlackboardUtils.CreateBlackboard("reward");
                        BlackboardUtils.CopyBlackboard(reward, newReward);
                        BlackboardUtils.AddToBlackboardList(targetInfo, "rewardList", newReward);
                    }
                }
                else
                {
                    prevProgress = targetInfo.GetValue<int>("challengeProgress");
                    prevMaxProgress = targetInfo.GetValue<int>("maxChallengeProgress");
                    prevDone = targetInfo.GetValue<bool>("done");
                    prevClaimed = targetInfo.GetValue<bool>("claimed");
                }

                int progress = simpleInfo.GetVariable<int>("challengeProgress")?.value ?? prevProgress;
                bool done = simpleInfo.GetVariable<bool>("done")?.value ?? prevDone;
                bool claimed = simpleInfo.GetVariable<bool>("claimed")?.value ?? prevClaimed;

                int maxProgress;
                var missionList = simpleInfo.GetVariable<List<Blackboard>>("missionList")?.value;
                if(missionList != null)
                {
                    maxProgress = missionList.Count;
                }
                else
                {
                    maxProgress = simpleInfo.GetVariable<int>("maxChallengeProgress")?.value ?? prevMaxProgress;
                }

                // Set SimpleInfo Variables
                targetInfo.AddVariable("startTimestamp", startTimestamp);
                targetInfo.AddVariable("endTimestamp", endTimestamp);
                targetInfo.AddVariable("challengeProgress", progress);
                targetInfo.AddVariable("maxChallengeProgress", maxProgress);
                targetInfo.AddVariable("done", done);
                targetInfo.AddVariable("claimed", claimed);
            }
            else // Club
            {
                var challengeInfoList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "clubChallengeInfoList");

                long startTimestamp = simpleInfo.GetValue<long>("startTimestamp");
                long endTimestamp = simpleInfo.GetValue<long>("endTimestamp");
                var missionList = simpleInfo.GetVariable<List<Blackboard>>("missionList")?.value;

                // Search Simple Info
                Blackboard targetInfo = null;
                foreach (var info in challengeInfoList)
                {
                    var infoChallengeType = info.GetVariable<ClubChallengeType>("type")?.value ?? ClubChallengeType.UNKNOWN;
                    long oldStartTimestamp = info.GetValue<long>("startTimestamp");
                    if (clubChallengeType == infoChallengeType)
                    {
                        if (startTimestamp < oldStartTimestamp) return;

                        targetInfo = info;
                        break;
                    }
                }

                // New Simple Info
                int prevProgress = 0, prevMaxProgress = 0;
                if (targetInfo == null)
                {
                    targetInfo = (Blackboard)BlackboardUtils.CreateBlackboard("ChallengeInfoSimple");
                    BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "clubChallengeInfoList", targetInfo);

                    targetInfo.AddVariable("type", clubChallengeType);
                    string challengeId = simpleInfo.GetValue<string>("challengeId");
                    targetInfo.AddVariable("challengeId", challengeId);
                    bool isCompleteToUnlock = simpleInfo.GetValue<bool>("isCompleteToUnlock");
                    targetInfo.AddVariable("isCompleteToUnlock", isCompleteToUnlock);

                    // Copy Rewards
                    var rewardList = simpleInfo.GetValue<List<Blackboard>>("rewardList");
                    BlackboardUtils.GetOrCreateBlackboardList(targetInfo, "rewardList");
                    foreach (var reward in rewardList)
                    {
                        var newReward = (Blackboard)BlackboardUtils.CreateBlackboard("reward");
                        BlackboardUtils.CopyBlackboard(reward, newReward);
                        BlackboardUtils.AddToBlackboardList(targetInfo, "rewardList", newReward);
                    }
                }
                else
                {
                    prevProgress = targetInfo.GetValue<int>("challengeProgress");
                    prevMaxProgress = targetInfo.GetValue<int>("maxChallengeProgress");
                }

                bool done = false;
                int maxProgress, progress;
                if (ClubUtils.IsClubber())
                {
                    if (missionList != null)
                    {
                        maxProgress = missionList.Count;
                        progress = GetChallengeMissionCompleteCount(missionList);
                    }
                    else
                    {
                        progress = simpleInfo.GetVariable<int>("challengeProgress")?.value ?? prevProgress;
                        maxProgress = simpleInfo.GetVariable<int>("maxChallengeProgress")?.value ?? prevMaxProgress;
                    }

                    done = progress >= maxProgress;
                }
                else
                {
                    maxProgress = progress = 0;
                }

                // Set SimpleInfo Variables
                targetInfo.AddVariable("startTimestamp", startTimestamp);
                targetInfo.AddVariable("endTimestamp", endTimestamp);
                targetInfo.AddVariable("challengeProgress", progress);
                targetInfo.AddVariable("maxChallengeProgress", maxProgress);
                targetInfo.AddVariable("done", done);
            }
        }

        public static Blackboard GetFocusedChallengeSimpleInfo(out MetaChallengeType type, bool ignoreEventChallenge = false)
        {
            // Incompleted, Most Important First

            // Event Challenge
            var eventInfo = ChallengeUtils.GetPreferredPassiveEventInfo();
            if(eventInfo != null && !ignoreEventChallenge)
            {
                Blackboard eventChallengeBB = GetEventChallengeSimpleInfo(eventInfo);
                if (eventChallengeBB != null)
                {
                    if (eventInfo.type == EventInfoType.PERSONAL_EVENT_CHALLENGE &&
                        !IsCompleteChallenge(eventChallengeBB, MetaChallengeType.EVENT_PERSONAL))
                    {
                        type = MetaChallengeType.EVENT_PERSONAL;
                        return eventChallengeBB;
                    }
                    else
                    if (eventInfo.type == EventInfoType.CLUB_EVENT_CHALLENGE &&
                        !IsCompleteChallenge(eventChallengeBB, MetaChallengeType.EVENT_CLUB))
                    {
                        type = MetaChallengeType.EVENT_CLUB;
                        return eventChallengeBB;
                    }
                }
                else
                {
                    if (ApplicationSettings.LogTest())
                        Debug.LogWarning("Failed to get challengeInfo about the target passive event.");
                }
            }

            var normalChallengeList = new List<MetaChallengeType>()
            {
                MetaChallengeType.DAILY,
                MetaChallengeType.EXPERT,
                MetaChallengeType.MASTER,
            };
            
            if (BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "clubChallengeInfoList").Count != 0)
            {
                normalChallengeList.Add(MetaChallengeType.CLUB);
            }

            foreach(var _type in normalChallengeList)
            {
                var info = GetChallengeSimpleInfo(_type);
                if (info != null && !IsCompleteChallenge(info, _type))
                {
                    type = _type;
                    return info;
                }
            }

            type = normalChallengeList.Last();
            return GetChallengeSimpleInfo(type);
        }

        public static bool IsNextChallenge(Blackboard simpleInfo)
        {
            if (simpleInfo == null) return false;

            long currentTimestamp = TimeUtils.GetTimeStamp();
            long startTimestamp = simpleInfo.GetValue<long>("startTimestamp");
            startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);

            return startTimestamp > currentTimestamp;
        }

        public static bool IsCompleteChallenge(Blackboard simpleInfo, MetaChallengeType challengeType)
        {
            if (simpleInfo == null) return false;

            switch (challengeType)
            {
                case MetaChallengeType.EVENT_PERSONAL:
                    return simpleInfo.GetValue<bool>("done");
                case MetaChallengeType.EVENT_CLUB:
                case MetaChallengeType.CLUB:
                    {
                        int progress = simpleInfo.GetValue<int>("challengeProgress");
                        int maxProgress = simpleInfo.GetValue<int>("maxChallengeProgress");
                        return progress >= maxProgress;
                    }
                case MetaChallengeType.DAILY:
                case MetaChallengeType.EXPERT:
                case MetaChallengeType.MASTER:
                    {
                        bool isNextChallenge = IsNextChallenge(simpleInfo);

                        int completeCount = simpleInfo.GetValue<int>("challengeProgress");
                        int minCount = GetChallengeMinCount(challengeType);

                        // Next Challenge || Completed
                        return isNextChallenge || completeCount >= minCount;
                    }
                default:
                    return false;
            }
        }

        public static Blackboard GetEventChallengeSimpleInfo(EventInfo eventInfo)
        {
            if (eventInfo == null) return null;

            var type = eventInfo.type;
            if(type == EventInfoType.PERSONAL_EVENT_CHALLENGE)
            {
                return GetChallengeSimpleInfo(MetaChallengeType.EVENT_PERSONAL);
            }
            else if(type == EventInfoType.CLUB_EVENT_CHALLENGE)
            {
                return GetChallengeSimpleInfo(MetaChallengeType.EVENT_CLUB);
            }

            return null;
        }

        public static bool IsChallengeSimpleInfoPersonal(Blackboard simpleInfo)
        {
            var personalChallengeType = simpleInfo.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
            return personalChallengeType != ChallengeType.UNKNOWN;
        }

        public static MetaChallengeType GetChallengeSimpleInfoChallengeType(Blackboard simpleInfo)
        {
            bool isPersonalChallenge = IsChallengeSimpleInfoPersonal(simpleInfo);
            if (isPersonalChallenge)
            {
                return ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(simpleInfo.GetValue<ChallengeType>("challengeType"));
            }
            else
            {
                return ChallengeUtils.ClubChallengeTypeToMetaChallengeType(simpleInfo.GetValue<ClubChallengeType>("type"));
            }
        }

        public static Blackboard GetChallengeSimpleInfo(MetaChallengeType challengeType)
        {
            bool isPersonalChallenge = ChallengeUtils.IsChallengeTypePersonal(challengeType);

            List<Blackboard> challengeInfoList;
            if (isPersonalChallenge)
            {
                challengeInfoList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "challengeInfoList");
            }
            else
            {
                challengeInfoList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "clubChallengeInfoList");
            }

            if (challengeInfoList != null)
            {
                foreach (var info in challengeInfoList)
                {
                    var infoType = GetChallengeSimpleInfoChallengeType(info);
                    if (infoType == challengeType) return info;
                }
            }

            return null;
        }

        public static Blackboard GetChallengeInfoBB(Blackboard response, MetaChallengeType challengeType)
        {
            if (response == null) return null;

            ChallengeUtils.MetaChallengeTypeToChallengeType(challengeType, out ChallengeType personalChallengeType, out _);

            if (challengeType == MetaChallengeType.EVENT_PERSONAL || challengeType == MetaChallengeType.EVENT_CLUB)
            {
                return GetEventChallengeInfoBB(response, challengeType == MetaChallengeType.EVENT_PERSONAL);
            }
            else if (challengeType == MetaChallengeType.CLUB)
            {
                return response.GetVariable<Blackboard>("clubChallengeInfo")?.value;
            }
            else // Daily, Expert, Master
            {
                var challengeInfoList = response.GetVariable<List<Blackboard>>("challengeInfoList")?.value;
                if(challengeInfoList != null)
                {
                    foreach(var info in challengeInfoList)
                    {
                        var infoType = info.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
                        if (infoType == personalChallengeType) return info;
                    }
                }
            }

            return null;
        }

        public static Blackboard GetEventChallengeInfoBB(Blackboard response, bool isPersonal)
        {
            if (response == null) return null;

            Blackboard challengeInfo = null;
            if (isPersonal)
            {
                var challengeInfoList = response.GetVariable<List<Blackboard>>("challengeInfoList")?.value;
                if (challengeInfoList != null)
                {
                    foreach (var info in challengeInfoList)
                    {
                        var cType = info.GetVariable<ChallengeType>("challengeType")?.value ?? ChallengeType.UNKNOWN;
                        if (cType == ChallengeType.EVENT)
                        {
                            challengeInfo = info;
                            break;
                        }
                    }
                }
            }
            else
            {
                challengeInfo = response.GetVariable<Blackboard>("clubEventChallengeInfo")?.value;
            }

            return challengeInfo;
        }

        public static bool HasNewChallenge()
        {
            foreach(var type in ChallengeEventManager.ChallengeTypeList)
            {
                Blackboard simpleInfo = GetChallengeSimpleInfo(type);
                if (simpleInfo == null) continue;

                long currentTimestamp = TimeUtils.GetTimeStamp();
                long startTimestamp = simpleInfo.GetValue<long>("startTimestamp");
                startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);
                long endTimestamp = simpleInfo.GetValue<long>("endTimestamp");
                endTimestamp = TimeUtils.ApplyTimeZoneOffset(endTimestamp);

                switch (type)
                {
                    case MetaChallengeType.CLUB:
                        continue;
                    case MetaChallengeType.EVENT_PERSONAL:
                    case MetaChallengeType.EVENT_CLUB:
                    case MetaChallengeType.DAILY:
                    case MetaChallengeType.EXPERT:
                    case MetaChallengeType.MASTER:
                        var checkTime = ChallengeUtils.GetChallengeCheckTime(type);
                        // Checked before the challenge started but it started
                        if ((checkTime <= startTimestamp && currentTimestamp >= startTimestamp) ||
                        // Checked before the challenge ended but it ended
                            (checkTime <= endTimestamp && currentTimestamp >= endTimestamp))
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public static int GetIncompleteMissionCountToComplete(Blackboard simpleInfo, MetaChallengeType challengeType)
        {
            if (simpleInfo == null || challengeType == MetaChallengeType.NONE)
                return 0;

            if (IsCompleteChallenge(simpleInfo, challengeType))
                return 0;

            int maxProgress = GetChallengeMinCount(challengeType);
            if(maxProgress == 0)
                maxProgress = simpleInfo.GetVariable<int>("maxChallengeProgress")?.value ?? 0;

            int progress = simpleInfo.GetVariable<int>("challengeProgress")?.value ?? 0;

            progress = Math.Min(progress, maxProgress);

            return maxProgress - progress;
        }

        public static long GetUnclaimedCompleteChallengesTotalCredit()
        {
            long defaultMultiplierNumerator = NumberUtils.GetGlobalDenominator();

            long multiplierNumerator = NumberUtils.GetGlobalDenominator();
            var claimMultiplierPassiveEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
            if (claimMultiplierPassiveEventInfo != null)
            {
                multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(claimMultiplierPassiveEventInfo);
            }

            long totalCredit = 0L;
            foreach (var type in ChallengeEventManager.ChallengeTypeList)
            {
                var simpleInfo = GetChallengeSimpleInfo(type);
                if (simpleInfo == null) continue;

                bool isPersonalChallenge = ChallengeUtils.IsChallengeTypePersonal(type);
                if (!isPersonalChallenge) continue;

                bool isComplete = IsCompleteChallenge(simpleInfo, type);
                bool claimed = simpleInfo.GetValue<bool>("claimed");
                bool isEventChallenge = type == MetaChallengeType.EVENT_PERSONAL || type == MetaChallengeType.EVENT_CLUB;
                bool isNextChallenge = IsNextChallenge(simpleInfo);

                if (isComplete && !claimed && (isEventChallenge || !isNextChallenge))
                {
                    var rewardList = simpleInfo.GetValue<List<Blackboard>>("rewardList");
                    long rewards = rewardList.Sum(r => r.GetVariable<long>("credit")?.value ?? 0L);
                    if (isEventChallenge)
                        rewards = NumberUtils.GetMultiplierNumeratorValue(rewards, defaultMultiplierNumerator);
                    else
                        rewards = NumberUtils.GetMultiplierNumeratorValue(rewards, multiplierNumerator);
                    totalCredit += rewards;
                }
            }

            return totalCredit;
        }

        public static int GetUnclaimedCompleteChallengeCount()
        {
            int count = 0;
            foreach (var type in ChallengeEventManager.ChallengeTypeList)
            {
                var simpleInfo = GetChallengeSimpleInfo(type);
                if (simpleInfo == null) continue;

                bool isPersonalChallenge = ChallengeUtils.IsChallengeTypePersonal(type);
                if (!isPersonalChallenge) continue;

                long currentTimestamp = TimeUtils.GetTimeStamp();
                long startTimestamp = simpleInfo.GetValue<long>("startTimestamp");
                startTimestamp = TimeUtils.ApplyTimeZoneOffset(startTimestamp);
                if (currentTimestamp < startTimestamp) continue;

                bool done = simpleInfo.GetValue<bool>("done");
                bool claimed = simpleInfo.GetValue<bool>("claimed");

                if (done && !claimed) ++count;
            }

            return count;
        }

        public static int GetChallengeMinCount(MetaChallengeType type)
        {
            ChallengeUtils.MetaChallengeTypeToChallengeType(type, out ChallengeType personalChallengeType, out _);
            return GetChallengeMinCount(personalChallengeType);
        }

        public static int GetChallengeMinCount(ChallengeType challengeType)
        {
            var challengeMinCountList = BlackboardUtils.FindVariable<Dictionary<string, int>>(MainBlackboard.Get(), "values/misc/CHALLENGE_MIN_COUNT");

            if (challengeMinCountList.value.ContainsKey(challengeType.ToString()))
                return challengeMinCountList.value[challengeType.ToString()];

            return 0;
        }

        public static int GetChallengeMissionCompleteCount(List<Blackboard> missionList)
        {
            return missionList?.Count(m => m.GetValue<bool>("done")) ?? 0;
        }

        public static string GetChallengeMissionRewardText(Blackboard missionInfo)
        {
            string rewardText = "";

            var rewardType = BlackboardUtils.FindVariable<RewardType>(missionInfo, "reward/type");

            bool isError = false;

            switch (rewardType.value)
            {
                case RewardType.CREDIT:
                    {
                        var rewardCoin = BlackboardUtils.FindVariable<long>(missionInfo, "reward/credit");
                        // long coin = System.Convert.ToInt64((double)rewardCoin.value * (double)rewardMultiplier);

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", out isError, rewardCoin.value);
                    }
                    break;
                case RewardType.RP:
                    {
                        var rp = BlackboardUtils.FindVariable<long>(missionInfo, "reward/rp");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_RP", out isError, rp.value);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/spinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/spinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_PLAY:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        var rewardCoin = BlackboardUtils.FindVariable<long>(missionInfo, "reward/earnCredit");
                        // long coin = System.Convert.ToInt64((double)rewardCoin.value * (double)rewardMultiplier);

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", out isError, rewardCoin.value);
                    }
                    break;
                case RewardType.GEM:
                    {
                        var gem = BlackboardUtils.FindVariable<long>(missionInfo, "reward/gem");

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_GEM", out isError, gem.value);
                    }
                    break;
            }

            return rewardText;
        }

        public static string GetChallengeMissionRewardMultiplierText(Blackboard missionInfo, long clubRewardMultiplier)
        {
            string rewardText = "";

            var rewardType = BlackboardUtils.FindVariable<RewardType>(missionInfo, "reward/type");

            bool isError = false;

            switch (rewardType.value)
            {
                case RewardType.CREDIT:
                    {
                        var rewardCoin = BlackboardUtils.FindVariable<long>(missionInfo, "reward/credit");
                        // long coin = System.Convert.ToInt64((double)rewardCoin.value * (double)rewardMultiplier);

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", out isError, NumberUtils.GetMultiplierNumeratorValue(rewardCoin.value, clubRewardMultiplier));
                    }
                    break;
                case RewardType.RP:
                    {
                        var rp = BlackboardUtils.FindVariable<long>(missionInfo, "reward/rp");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_RP", out isError, rp.value);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/spinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/spinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_PLAY:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(missionInfo, "reward/count");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        var rewardCoin = BlackboardUtils.FindVariable<long>(missionInfo, "reward/earnCredit");
                        // long coin = System.Convert.ToInt64((double)rewardCoin.value * (double)rewardMultiplier);

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", out isError, rewardCoin.value);
                    }
                    break;
                case RewardType.GEM:
                    {
                        var gem = BlackboardUtils.FindVariable<long>(missionInfo, "reward/gem");

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_GEM", out isError, gem.value);
                    }
                    break;
            }

            return rewardText;
        }

        public static string GetChallengeMissionRewardResultText(Blackboard rewardResult)
        {
            string rewardText = "";

            var rewardType = BlackboardUtils.FindVariable<RewardType>(rewardResult, "rewardType");

            bool isError = false;

            switch (rewardType.value)
            {
                case RewardType.CREDIT:
                case RewardType.CREDIT_WITH_MULTIPLIER:
                    {
                        var rewardCoin = BlackboardUtils.FindVariable<long>(rewardResult, "credit");
                        var clubMultiplier  = ClubUtils.GetClubRewardMultiplierNumerator();
                        rewardCoin.value = NumberUtils.GetMultiplierNumeratorValue(rewardCoin.value, clubMultiplier);

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", out isError, rewardCoin.value);
                    }
                    break;
                case RewardType.RP:
                    {
                        var rp = BlackboardUtils.FindVariable<long>(rewardResult, "rp");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_RP", out isError, rp.value);
                    }
                    break;
                case RewardType.DAILY_BONUS_WHEEL_SPIN:
                    {
                        var spinCount = BlackboardUtils.FindVariable<int>(rewardResult, "addedSpinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, spinCount.value);
                    }
                    break;
                case RewardType.GAME_SPIN:
                    {
                        var addedSpinCount = BlackboardUtils.FindVariable<int>(rewardResult, "addedSpinCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, addedSpinCount.value);
                    }
                    break;
                case RewardType.GAME_DEAL:
                    {
                        var addedSpinCount = BlackboardUtils.FindVariable<int>(rewardResult, "addedCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, addedSpinCount.value);
                    }
                    break;
                case RewardType.GAME_PLAY:
                    {
                        var addedSpinCount = BlackboardUtils.FindVariable<int>(rewardResult, "addedCount");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "DEFAULT_SPINS", out isError, addedSpinCount.value);
                    }
                    break;
                case RewardType.DAILY_BOOST:
                    {
                        var rewardCoin = BlackboardUtils.FindVariable<long>(rewardResult, "earnCredit");
                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_COIN", out isError, rewardCoin.value);
                    }
                    break;
                case RewardType.GEM:
                    {
                        var gem = BlackboardUtils.FindVariable<long>(rewardResult, "gem");

                        rewardText = StringTableUtils.GetString(StringTable.StringTableType.Global, "SIMPLE_GEM", out isError, gem.value);
                    }
                    break;
            }

            return rewardText;
        }

        public static string GetChallengeInfoRewardResultText(List<Blackboard> rewardListBB, long multiplierNumerator = 100, bool isTotal = true)
        {
            string resultText = null;
            if (rewardListBB != null && rewardListBB.Count > 0)
            {
                for (int i = 0; i < rewardListBB.Count; i++)
                {
                    string rewardText = GetChallengeInfoRewardResultText(rewardListBB[i], multiplierNumerator, isTotal);

                    if (!string.IsNullOrEmpty(resultText) && !string.IsNullOrEmpty(rewardText))
                    {
                        resultText += " + ";
                    }

                    if (!string.IsNullOrEmpty(rewardText))
                    {
                        resultText += rewardText;
                    }
                }
            }
            else
            {
                if(isTotal)
                {
                    resultText = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_TOTAL_REWARD_COIN", 0);
                }
                else
                {
                    resultText = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_REWARD_COIN", 0);
                }
            }

            return resultText;
        }

        public static string GetChallengeInfoRewardResultText(Blackboard rewardBB, long multiplierNumerator = 100, bool isTotal = true)
        {
            RewardType rewardType = rewardBB.GetVariable<RewardType>("rewardType") != null ? rewardBB.GetValue<RewardType>("rewardType") : rewardBB.GetValue<RewardType>("type");

            switch (rewardType)
            {
                case RewardType.CREDIT:
                    {
                        long totalCoin = rewardBB.GetValue<long>("credit");
                        if (totalCoin <= 0L) return null;

                        if (isTotal)
                        {
                            totalCoin = NumberUtils.GetMultiplierNumeratorValue(totalCoin, multiplierNumerator);
                            return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_TOTAL_REWARD_COIN", totalCoin);
                        }
                        else
                        {
                            return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_REWARD_COIN", totalCoin);
                        }
                    }
                case RewardType.GEM:
                    {
                        long totalGem = rewardBB.GetValue<long>("gem");
                        if (totalGem <= 0L) return null;

                        if (isTotal)
                        {
                            totalGem = NumberUtils.GetMultiplierNumeratorValue(totalGem, multiplierNumerator);
                            return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_TOTAL_REWARD_GEM", totalGem);
                        }
                        else
                        {
                            return StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_REWARD_GEM", totalGem);
                        }
                    }
            }

            return null;
        }

        public static List<Blackboard> GetClubChallengeContributionInfo(Blackboard response, Blackboard missionInfo, bool isEventClubChallenge)
        {
            if (missionInfo == null) return null;

            string missionId = missionInfo.GetVariable<string>("missionId")?.value;
            if (string.IsNullOrEmpty(missionId)) return null;

            string variableName = isEventClubChallenge ? "clubEventChallengeMissionContributionList" : "clubChallengeMissionContributionList";
            var allMissionContributionList = response.GetVariable<List<Blackboard>>(variableName)?.value;

            int missionIndex = -1;
            variableName = isEventClubChallenge ? "clubEventChallengeInfo" : "ClubChallengeInfo";
            var challengeInfo = response.GetValue<Blackboard>(variableName);
            var missionList = challengeInfo.GetValue<List<Blackboard>>("missionList");
            for(int i = 0; i < missionList.Count; ++i)
            {
                if (missionList[i].GetValue<string>("missionId") == missionId)
                {
                    missionIndex = i;
                    break;
                }
            }

            if(missionIndex < 0 || missionIndex >= allMissionContributionList.Count) return null;

            return allMissionContributionList[missionIndex].GetVariable<List<Blackboard>>("contributionList")?.value;
        }

        public static int GetClubChallengeMaxStage(Blackboard clubChallengeInfoBB)
        {
            int maxStage = clubChallengeInfoBB.GetVariable<int>("maxStage")?.value ?? -1;
            return maxStage + 1;
        }
    }
}
