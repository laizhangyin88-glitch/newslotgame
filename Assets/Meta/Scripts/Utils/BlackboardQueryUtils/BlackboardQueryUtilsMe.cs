using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        private const float MIN_EXP_RATIO = 0.03f;

        public static void AddCoins(long earnCoins)
        {
            if(earnCoins <= 0) return;
            BlackboardUtils.FindVariable<long>(null, "/me/credit").value += earnCoins;
        }

        public static void AddRP(long earnRP)
        {
            if(earnRP <= 0) return;
            BlackboardUtils.FindVariable<long>(null, "/me/rp").value += earnRP;
            BlackboardUtils.FindVariable<long>(null, "/me/accRp").value += earnRP;
        }

        public static void AddGems(long earnGems)
        {
            if(earnGems <= 0) return;
            BlackboardUtils.FindVariable<long>(null, "/me/gem").value += earnGems;
        }

        public static void AddFinders(int finder)
        {
            if (finder <= 0) return;
            HiddenObjects.HiddenObjects.Utils.AddFinderCount(finder);
        }

        public static void SpentGems(long spentGems)
        {
            if(spentGems <= 0) return;
            BlackboardUtils.FindVariable<long>(null, "/me/gem").value -= spentGems;
        }

        public static void SpentCredit(long spentCredit)
        {
            BlackboardUtils.FindVariable<long>(null, "/me/credit").value -= spentCredit;
            AddExp(spentCredit);
        }

        public static void AddExp(long spentCredit)
        {
            var level = BlackboardUtils.FindVariable<int>(null, "/me/level");
            int beforeLevel = level.value;

            // use value from GameLogic_Level_Up
            Blackboard meBB = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
            BlackboardUtils.SetOrCreateValue<int>(meBB, "beforeLevel", beforeLevel);

            if(level.value < LevelUtils.GetMaxLevel())
            {
                long totalMultiplierNumerator = 100L;

                // Exp Multi
                var expMultiEventList = PassiveEventManager.Instance.GetActiveEventInfoList(EventInfoType.EXP_MULTIPLY);
                if (expMultiEventList != null && expMultiEventList.Count > 0)
                {
                    foreach(var eventInfo in expMultiEventList)
                    {
                        long multiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                        if (multiplierNumerator > 100L) totalMultiplierNumerator += multiplierNumerator - 100L;
                    }
                }

                // Exp Multi (Extendable)
                var expMultiExtendableEventList = PassiveEventManager.Instance.GetActiveEventInfoList(EventInfoType.EXP_MULTIPLY_EXTENDABLE);
                if (expMultiExtendableEventList != null && expMultiExtendableEventList.Count > 0)
                {
                    long max = 0L;
                    foreach (var eventInfo in expMultiExtendableEventList)
                    {
                        long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                        if (eventMultiplierNumerator > max) max = eventMultiplierNumerator;
                    }
                    if (max > 100L) totalMultiplierNumerator += max - 100L;
                }

                // Vip Benefit
                if (IsVipLoungeEnabled() &&
                    HasVipLoungeBadge())
                {
                    long vipExpMultiNumerator = BlackboardUtils.FindVariable<long>("/values/misc/VIP_LOUNGE_MISC/VIP_LOUNGE_EXP_BOOST_MULTIPLIER_NUMERATOR")?.value ?? 0L;
                    if (vipExpMultiNumerator > 100L) totalMultiplierNumerator += vipExpMultiNumerator - 100L;
                }

                // Level Dash Purchase Boost
                if(LevelUpDash.LevelUpDash.Utils.PurchaseBoosterEndTimestamp >
                    TimeUtils.GetTimeStamp())
                {
                    long levelDashMultiNumerator = LevelUpDash.LevelUpDash.Utils.PurchaseBoosterMultiplierNumerator;
                    if (levelDashMultiNumerator > 100L) totalMultiplierNumerator += levelDashMultiNumerator - 100L;
                }

                if (totalMultiplierNumerator < 100L) totalMultiplierNumerator = 100L;
                double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(totalMultiplierNumerator);

                long earnExp = LevelUtils.GetEarnExp(spentCredit) + LevelUtils.GetAdditionalExp(level.value);
                earnExp = System.Convert.ToInt64(earnExp * eventMultiplier);
                earnExp = LevelUtils.GetMaxEarnExp(level.value, earnExp, eventMultiplier);

                var requiredExp = BlackboardUtils.FindVariable<long>(null, "/me/requiredExp");
                long calculatedExp = requiredExp.value;

                while(earnExp > 0)
                {
                    if (earnExp < calculatedExp)
                    {
                        requiredExp.value = calculatedExp - earnExp;
                        earnExp = 0;
                    }
                    else
                    {
                        LevelUp();

                        earnExp -= calculatedExp;
                        earnExp = LevelUtils.GetMaxEarnExp(level.value, earnExp, eventMultiplier);

                        if (level.value == LevelUtils.GetMaxLevel())
                        {
                            requiredExp.value = 0L;
                            earnExp = 0;
                        }
                        else
                        {
                            calculatedExp = LevelUtils.GetRequiredExp(level.value);
                            requiredExp.value = calculatedExp;
                        }
                    }
                }

                if(beforeLevel < level.value)
                {
                    BlackboardUtils.FindVariable<long>(null, "/me/requiredExpMax").value = LevelUtils.GetRequiredExp(level.value);
                }
            }
        }

        public static void UpdatePotOfGold(long updateCoin)
        {
            var meBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "me");
            meBB.value.SetValue("piggyCredit", updateCoin);
        }

        public static List<long> GetRequiredExpTable()
        {
            return BlackboardUtils.FindVariable<List<long>>("/values/level/REQUIRED_EXP_TABLE")?.value;
        }

        public static long GetRequiredExp(int level)
        {
            List<long> expList = GetRequiredExpTable();
            return expList.IsValidIndex(level) ? expList[level] : 0L;
        }

        public static long GetRequiredExp(int fromLevel, int toLevel) // from zero to zero
        {
            long total = 0L;
            for (; fromLevel < toLevel; ++fromLevel)
                total += GetRequiredExp(fromLevel);
            return total;
        }

        public static long GetCurrentExp()
        {
            long requiredExp = BlackboardUtils.FindVariable<long>(null, "/me/requiredExp").value;
            long requiredExpMax = BlackboardUtils.FindVariable<long>(null, "/me/requiredExpMax").value;
            
            return requiredExpMax - requiredExp;
        }

        public static float GetCurrentExpRatio()
        {
            long requiredExp    = BlackboardUtils.FindVariable<long>(null, "/me/requiredExp").value;
            long requiredExpMax = BlackboardUtils.FindVariable<long>(null, "/me/requiredExpMax").value;
            float expRatio = (float)(requiredExpMax - requiredExp) / (float)requiredExpMax;
            
            if(expRatio < MIN_EXP_RATIO)
                expRatio = MIN_EXP_RATIO;

            return expRatio;
        }

        public static Variable GetMetaSpinCountVariable(MetaJackpotType type)
        {
            string spinType = "dailyBonusSpinCount";

            if (type == MetaJackpotType.DAILY_MEGA_WHEEL)
                spinType = "dailyMegaWheelSpinCount";

            var spinCount = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), spinType);

            return spinCount;
        }

        public static void SetDailySpinCount(int count, MetaJackpotType type)
        {
            var spinCount = GetMetaSpinCountVariable(type);
            spinCount.value = count;
        }

        public static int GetDailySpinCount(MetaJackpotType type)
        {
            var spinCount = GetMetaSpinCountVariable(type);

            if (spinCount != null) return (int)spinCount.value;
            else return 0;
        }

        public static void BackUpMe()
        {
            var backupBB = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "backupMe");
            var meBB = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
            BlackboardUtils.SetOrCreateValue<string>(backupBB, "userId", meBB.GetValue<string>("userId"));
            BlackboardUtils.SetOrCreateValue<long>(backupBB, "registerTimestamp", meBB.GetValue<long>("registerTimestamp"));
            BlackboardUtils.SetOrCreateValue<int>(backupBB, "loginCount", meBB.GetValue<int>("loginCount"));
            BlackboardUtils.SetOrCreateValue<long>(backupBB, "credit", meBB.GetValue<long>("credit"));
            BlackboardUtils.SetOrCreateValue<long>(backupBB, "gem", meBB.GetValue<long>("gem"));
            BlackboardUtils.SetOrCreateValue<int>(backupBB, "tier", meBB.GetValue<int>("tier"));
            BlackboardUtils.SetOrCreateValue<int>(backupBB, "purchaseCount", meBB.GetValue<int>("purchaseCount"));
            BlackboardUtils.SetOrCreateValue<double>(backupBB, "lifetimeSpend", meBB.GetValue<double>("lifetimeSpend"));

            var beforeLevel = BlackboardUtils.FindVariable(meBB, "beforeLevel");
            if(beforeLevel == null || beforeLevel.value == null)
            {
                BlackboardUtils.SetOrCreateValue<int>(backupBB, "level", meBB.GetValue<int>("level"));
            }
            else
            {
                BlackboardUtils.SetOrCreateValue<int>(backupBB, "level", (int)beforeLevel.value);
            }
        }

        public static bool AppendBackUpMeInfo(Dictionary<string, object> dict)
        {
            var backupBB = BlackboardUtils.FindVariable<Blackboard>(null, "/backupMe");

            if (backupBB != null && backupBB.value != null)
            {
                dict["user_id"] = backupBB.value.GetValue<string>("userId");
                dict["reg_ts"] = backupBB.value.GetValue<long>("registerTimestamp");
                dict["login_ct"] = backupBB.value.GetValue<int>("loginCount");
                dict["coin"] = backupBB.value.GetValue<long>("credit");
                dict["gem"] = backupBB.value.GetValue<long>("gem");
                dict["level"] = backupBB.value.GetValue<int>("level");
                dict["tier"] = backupBB.value.GetValue<int>("tier");
                dict["purchase_ct"] = backupBB.value.GetValue<int>("purchaseCount");
                dict["ltv"] = Convert.ToInt32(backupBB.value.GetValue<double>("lifetimeSpend") * 100);

                return true;
            }
            else
            {
                backupBB = BlackboardUtils.FindVariable<Blackboard>(null, "/me");

                if (backupBB != null && backupBB.value != null)
                {
                    dict["user_id"] = backupBB.value.GetValue<string>("userId");
                    dict["reg_ts"] = backupBB.value.GetValue<long>("registerTimestamp");
                    dict["login_ct"] = backupBB.value.GetValue<int>("loginCount");
                    dict["coin"] = backupBB.value.GetValue<long>("credit");
                    dict["gem"] = backupBB.value.GetValue<long>("gem");
                    dict["level"] = backupBB.value.GetValue<int>("level");
                    dict["tier"] = backupBB.value.GetValue<int>("tier");
                    dict["purchase_ct"] = backupBB.value.GetValue<int>("purchaseCount");
                    dict["ltv"] = Convert.ToInt32(backupBB.value.GetValue<double>("lifetimeSpend") * 100);
                    backupBB = null;

                    return true;
                }
            }

            return false;
        }

        public static void UpdatePurchaseErrorCode(string errorCode)
        {
            var meBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "me");
            meBB.value.SetValue("purchaseErrorCode", errorCode);
        }

        public static string GetMyUserId()
        {
            var meBB = MainBlackboard.Get().GetValue<Blackboard>("me");
            if(meBB != null)
                return BlackboardUtils.GetOrCreateVariable<string>(meBB, "userId").value;

            return null;
        }

        public static int GetMyLevel()
        {
            var level = BlackboardUtils.FindVariable<int>(null, "/me/level");
            if (level != null)
                return level.value;

            return 0;
        }

        public static int GetMyTier() => TierUtils.GetMeTier();

        public static void LevelUp(int value = 1)
        {
            if (value < 1) return;

            var level = BlackboardUtils.FindVariable<int>(null, "/me/level");
            if (level != null) level.value += value;
        }

        public static long GetMyClubId()
        {
            var clubID = BlackboardUtils.FindVariable<long>(null, "/clubId");
            if(clubID != null)
                return clubID.value;

            return 0;
        }

        public static void SetMyClubId(long clubId)
        {
            var mainClubID = BlackboardUtils.FindVariable<long>(null, "/clubId");
            if (mainClubID != null)
            {
                mainClubID.value = clubId;
            }

            var meClubID = BlackboardUtils.FindVariable<long>("/me/clubId");
            if (meClubID != null) meClubID.value = clubId;
        }

        public static void SetMyCredit(long credit)
        {
            var meCredit = BlackboardUtils.FindVariable<long>("/me/credit");
            if (meCredit != null) meCredit.value = credit;
        }

        public static UserProfile GetMyProfile() {
            var myProfile = new UserProfile();
            var meBB = MainBlackboard.Get().GetValue<Blackboard>("me");
            myProfile.userId = BlackboardUtils.GetOrCreateVariable<string>(meBB, "userId").value;
            myProfile.name = BlackboardUtils.GetOrCreateVariable<string>(meBB,"name").value;
            myProfile.level = BlackboardUtils.GetOrCreateVariable<int>(meBB,"level").value;
            myProfile.clubId = BlackboardUtils.GetOrCreateVariable<long>(meBB,"clubId").value;
            myProfile.profileUrl = BlackboardUtils.GetOrCreateVariable<string>(meBB,"profileUrl").value;
            myProfile.profileHighResolutionUrl = BlackboardUtils.GetOrCreateVariable<string>(meBB,"profileHighResolutionUrl").value;
            myProfile.reportCount= BlackboardUtils.GetOrCreateVariable<int>(meBB,"reportCount").value;
            myProfile.tier= BlackboardUtils.GetOrCreateVariable<int>(meBB,"tier").value;
            return myProfile;
        }

        private static long speakerUpdateTimestamp = 0;
        public static void UpdateGlobalChatSpeaker(int speakerCount, long serverTimestamp)
        {
            if(speakerUpdateTimestamp < serverTimestamp)
            {
                BlackboardUtils.FindVariable<int>(null, "/me/speaker").value = speakerCount;
                speakerUpdateTimestamp = serverTimestamp;
            }
        }

        public static void SpendGlobalChatSpeaker(int count = 1)
        {
            UpdateGlobalChatSpeaker(GetGlobalChatSpeakerCount() - count, speakerUpdateTimestamp - 1);
        }

        public static int GetGlobalChatSpeakerCount()
        {
            return BlackboardUtils.FindVariable<int>(null, "/me/speaker").value;
        }

        public static bool UserOptionsPushNotification()
        {
            return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/pushNotification").value;
        }

        public static Blackboard GetUserBucksInfoBB()
        {
            return BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "userBucksInfo")?.value ?? null;
        }

        public static Blackboard GetUserBucksBB()
        {
            Blackboard userBucksInfoBB = GetUserBucksInfoBB();
            if (userBucksInfoBB == null)
                return null;
            return BlackboardUtils.FindVariable<Blackboard>(userBucksInfoBB, "bucks")?.value ?? null;
        }

        public static long GetUserTotalBucks()
        {
            Blackboard bucksBB = GetUserBucksBB();
            if (bucksBB == null)
                return 0;

            long paidBucks = BlackboardUtils.FindVariable<long>(bucksBB, "paidBucks")?.value ?? 0L;
            long bonusBucks = BlackboardUtils.FindVariable<long>(bucksBB, "bonusBucks")?.value ?? 0L;
            long freeBucks = BlackboardUtils.FindVariable<long>(bucksBB, "freeBucks")?.value ?? 0L;

            return paidBucks + bonusBucks + freeBucks;
        }

        public static void UpdateUserBucksInfo(UserBucksInfo userBucksInfo)
        {
            if (userBucksInfo == null)
                return;
            ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "userBucksInfo"), userBucksInfo);
        }

        public static void UpdateUserBucks(UserBucks userBucks)
        {
            if (userBucks == null)
                return;
            Blackboard bb = GetUserBucksBB();
            if (bb != null)
                ClientAPI2Blackboard.Serialize(bb, userBucks);
        }
    }

}

