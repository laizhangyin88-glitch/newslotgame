using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode
{
    public class ClubUtils
    {
        public enum ResultPopupMetaGameName
        {
            BOSS_RAIDERS = 0,
            CLUB_ARENA = 1
        }

        private static int maxLevel = -1;

        private static List<int> clubMemberTable = new List<int>();
        private static List<int> clubCoCaptainTable = new List<int>();
        private static List<long> clubDonateTable = new List<long>();
        private static List<long> clubDonateCostTable = new List<long>();

        private const int HOUR_SECONDS = 3600;
        private const int MINUTE_SECONDS = 60;

        public static IEnumerator ClaimClubFeedCoroutine(Blackboard agent, long feedId, Variable<int> bonusType, Variable<long> bonusCredit, Variable<Blackboard> claimReward, bool withoutAdd = false)
        {
            if (ApplicationSettings.LogTest())
                Debug.Log(string.Format("ClaimClubFeedCoroutine({0},{1})", agent.gameObject.name, feedId));

            bool success = false;
            bool failure = false;

            BagelCodeClientAPI.SendNewsFeedClaim(feedId,
            (response) =>
            {
                long winBonusCoins = 0;
                if (response.claimReward != null)
                {
                    switch (response.claimReward.rewardType)
                    {
                        case RewardType.CREDIT:
                            {
                                RewardResultCredit creditResult = response.claimReward.rewardResult as RewardResultCredit;
                                if (creditResult != null)
                                {
                                    winBonusCoins = creditResult.credit;
                                    if (!withoutAdd)
                                    {
                                        BlackboardQueryUtils.AddCoins(creditResult.credit);
                                    }
                                }
                            }
                            break;
                        case RewardType.CREDIT_WITH_MULTIPLIER:
                            {
                                RewardResultCreditWithMultiplier creditResult = response.claimReward.rewardResult as RewardResultCreditWithMultiplier;
                                if (creditResult != null)
                                {
                                    winBonusCoins = creditResult.credit;
                                    if (!withoutAdd)
                                    {
                                        BlackboardQueryUtils.AddCoins(creditResult.credit);
                                    }
                                }
                            }
                            break;
                        case RewardType.GEM:
                            {
                                RewardResultGem gemResult = response.claimReward.rewardResult as RewardResultGem;
                                if (gemResult != null)
                                {
                                    if (!withoutAdd)
                                    {
                                        BlackboardQueryUtils.AddGems(gemResult.gem);
                                    }
                                }
                            }
                            break;
                    }
                }

                if (agent != null && claimReward != null)
                {
                    var rewardBB = BlackboardUtils.GetOrCreateBlackboard(agent, "feedReward");
                    ClientAPI2Blackboard.Serialize(rewardBB, response.claimReward);
                    claimReward.value = (Blackboard)rewardBB;
                }

                if (agent != null && bonusType != null)
                {
                    if (winBonusCoins > 0)
                    {
                        bonusType.value = 1;
                        if (bonusCredit != null)
                            bonusCredit.value = winBonusCoins;
                    }
                    else
                    {
                        bonusType.value = 0;
                    }
                }

                success = true;
            },
            (error) =>
            {
                switch (error.errorCode)
                {
                    case ClientModels.Error.SOMEONE_UPDATING_CLUB_ERROR:
                        GlobalErrorHandler.OpenAlertPopup("ERROR_SOMETHING_WRONG");
                        if (agent != null)
                            success = true;
                        break;
                    case ClientModels.Error.NOT_IN_CLUB_ERROR:
                        {
                            if (agent != null)
                                failure = true;
                            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                            if (meClubID.value > 0)
                            {
                                bool stringError = false;
                                ErrorPopupInfo info = new ErrorPopupInfo();
                                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CLUB_REMOVED_ALERT", out stringError);
                                info.type = ErrorPopupType.OK;
                                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

                                ErrorPopupHandler.Instance.OpenError(info);
                            }

                            BlackboardQueryUtils.SetMyClubId(0);
                            MessageDispatcher.Dispatch("OnMetaUIEvent", new EventData("OnRemovedClub"));
                        }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
                failure = true;
            });

            yield return new WaitUntil(() => success || failure);
        }

        public static float GetLeaderPushSendCoolTimeRemaining(Blackboard leaderPushInfo)
        {
            if (leaderPushInfo == null) return 0;

            long lastPushTimestamp = leaderPushInfo.GetValue<long>("lastPushTimestamp");
            long cooltimeMs = leaderPushInfo.GetValue<long>("cooltimeMs");

            long elapsed = TimeUtils.GetTimeStamp() - lastPushTimestamp;
            long remaining = cooltimeMs - elapsed;
            if (remaining < 0L) remaining = 0L;
            return remaining / 1000f;
        }

        public static long GetLeaderPushSendCoolTimeRemainingLong(Blackboard leaderPushInfo)
        {
            if (leaderPushInfo == null) return 0;

            long lastPushTimestamp = leaderPushInfo.GetValue<long>("lastPushTimestamp");
            long cooltimeMs = leaderPushInfo.GetValue<long>("cooltimeMs");

            long elapsed = TimeUtils.GetTimeStamp() - lastPushTimestamp;
            long remaining = cooltimeMs - elapsed;
            if (remaining < 0L) remaining = 0L;
            return remaining;
        }

        public static void Clear()
        {
            maxLevel = -1;

            clubMemberTable.Clear();
            clubDonateTable.Clear();
            clubDonateCostTable.Clear();
            clubCoCaptainTable.Clear();
        }

        public static int GetMaxLevel()
        {
            if(maxLevel < 1)
                maxLevel = BlackboardUtils.FindVariable<int>(null, "/values/club/LEVEL/MAX_LEVEL").value;

            return maxLevel;
        }

        public static int GetClubMaxMemberCount(int level)
        {
            if(level > GetMaxLevel()) level = GetMaxLevel();

            if(clubMemberTable.Count == 0)
                clubMemberTable = BlackboardUtils.FindVariable<List<int>>(null, "/values/club/MAX_MEMBERS").value;

            if(clubMemberTable.Count <= level) return 0;

            return clubMemberTable[level];
        }

        public static int GetClubMaxCoCaptainCount(int level)
        {
            if(level > GetMaxLevel()) level = GetMaxLevel();

            if(clubCoCaptainTable.Count == 0)
                clubCoCaptainTable = BlackboardUtils.FindVariable<List<int>>(null, "/values/club/COLEADER_LIMIT").value;

            if(clubCoCaptainTable.Count <= level) return 0;

            return clubCoCaptainTable[level];
        }

        public static long GetClubNextExp(int level)
        {
            if(level > GetMaxLevel()) level = GetMaxLevel();

            if(clubDonateTable.Count == 0)
                clubDonateTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/club/LEVEL/REQUIRED_EXP_TABLE").value;

            return clubDonateTable[level];
        }

        public static long GetDonateUnitCost(int level)
        {
            if(level > GetMaxLevel()) level = GetMaxLevel();

            if(clubDonateCostTable.Count == 0)
                clubDonateCostTable = BlackboardUtils.FindVariable<List<long>>(null, "/values/club/DONATE_UNIT_CREDIT_LIST").value;

            return clubDonateCostTable[level];
        }

        public static int GetMaxDonateCount()
        {
            return BlackboardUtils.FindVariable<int>(null, "/values/club/DAILY_MAX_DONATION_COUNT").value;
        }

        public static string GetClubFeedLeftTimeText(long leftTimestamp)
        {
            string result = null;

            long leftTime = leftTimestamp / 1000;

            bool error = false;

            long hour = leftTime / HOUR_SECONDS;
            if (hour >= 1)
            {
                long leftSec = leftTime % HOUR_SECONDS;

                if(hour > 1)
                {
                    result = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEFT_TIME_TEXT_2", out error, hour);
                }
                else
                {
                    if(leftSec < MINUTE_SECONDS)
                    {
                        result = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEFT_TIME_TEXT_2", out error, hour);
                    }
                    else
                    {
                        long minute = leftSec / MINUTE_SECONDS;
                        result = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEFT_TIME_TEXT_3", out error, hour, minute);
                    }
                }
            }
            else
            {
                long minute = leftTime / MINUTE_SECONDS;
                if (minute >= 1)
                    result = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEFT_TIME_TEXT_1", out error, minute);
                else
                    result = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_NEWS_FEED_LEFT_TIME_TEXT_0", out error);
            }

            return result;
        }

        public static int GetClubTierGroup(int clubTier)
        {
            switch(clubTier)
            {
                case 0:  // Mini
                    return 0;
                case 1:  // Minor
                    return 1;
                case 2:  // Major 1
                case 3:  // Major 2
                case 4:  // Major 3
                    return 2;
                case 5:  // Mega 1
                case 6:  // Mega 2
                case 7:  // Mega 3
                    return 3;
                case 8: // Grand 1
                case 9: // Grand 2
                case 10: // Grand 3
                    return 4;
                case 11: // Epic
                    return 5;
            }

            return 0;
        }

        public static void SetIsClubber(bool isClubber)
        {
            MainBlackboard.Get().AddVariable("isClubber", isClubber);
        }

        public static bool IsClubber()
        {
            var clubId = BlackboardUtils.FindVariable<long>("/clubId");
            if(clubId == null || clubId.value <= 0L)
            {
                return false;
            }
            else
            {
                var isClubber = BlackboardUtils.FindVariable<bool>("/isClubber");
                if(isClubber == null || isClubber.value == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static int GetClubPlayerPrefs(string key, long clubId, int def = 0)
        {
            return PlayerPrefs.GetInt(string.Format("{0}:{1}", key, clubId), def);
        }

        public static void SetClubPlayerPrefs(string key, long clubId, int value)
        {
            PlayerPrefs.SetInt(string.Format("{0}:{1}", key, clubId), value);
        }

        public static int GetClubMetaGamePlayerPrefs(ResultPopupMetaGameName metaGameName, int def = 0)
        {
            return PlayerPrefs.GetInt(GetClubMetaGamePlayerPrefsName(metaGameName), def);
        }

        public static void SetClubMetaGamePlayerPrefs(ResultPopupMetaGameName metaGameName, int value)
        {
            PlayerPrefs.SetInt(GetClubMetaGamePlayerPrefsName(metaGameName), value);
        }

        public static void ResetClubMetaGamePlayerPrefs(int value = 0)
        {
            int maxCount = 2;
            for (int i = 0; i < maxCount; ++i)
                SetClubMetaGamePlayerPrefs((ResultPopupMetaGameName)i, value);
        }

        public static string GetClubMetaGamePlayerPrefsName(ResultPopupMetaGameName metaGameName)
        {
            return string.Format("{0}:{1}", ClubDefine.PLAYER_PREFS_LAST_META_GAME_CHECKED, metaGameName.ToString());
        }

        public static long GetClubRewardMultiplierNumerator()
        {
            return LevelUtils.GetLevelMultiplierNumeratorFromType("clubReward");
        }
    }
}
