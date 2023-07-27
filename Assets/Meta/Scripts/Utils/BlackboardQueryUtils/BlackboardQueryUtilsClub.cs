using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        public static List<string> onlineClubUserIDList = new List<string>();

        public static void UpdateOnlineClubIDList()
        {
            var clubOnlineList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/clubOnlineList").value;

            onlineClubUserIDList.Clear();
            for (int i = 0; i < onlineClubUserIDList.Count; ++i)
            {
                onlineClubUserIDList.Add(clubOnlineList[i].GetValue<string>("userId"));
            }
        }

        public static List<Blackboard> GetOnlineClubMemberList()
        {
            List<Blackboard> clubMemberList = new List<Blackboard>();
            clubMemberList.AddRange(BlackboardUtils.FindVariable<List<Blackboard>>(null, "/clubOnlineList").value);

            // List<string> onlineFriendUserIdList = BlackboardUtils.FindVariable<List<string>>(null, "/onlineFriendUserIdList").value;

            // for (int i=0; i<clubMemberList.Count;)
            // {
            //     string userId = clubMemberList[i].GetValue<string>("userId");
            //     if (onlineFriendUserIdList.Contains(userId))
            //     {
            //         clubMemberList.RemoveAt(i);
            //     }
            //     else
            //         ++i;
            // }

            return clubMemberList;
        }

        public static bool IsMyClubMember(long userClubId)
        {
            if (userClubId <= 0L) return false;

            var clubId = MainBlackboard.Get().GetValue<long>("clubId");
            if (clubId == userClubId)
            {
                return true;
            }
            return false;
        }

        public static void ClearClubBadgeInfo()
        {
            var latestFeedRecordID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestFeedRecordId");
            latestFeedRecordID.value = 0;

            var newStuffExists = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "clubBadgeInfo/newStuffExists");
            newStuffExists.value = false;

            var latestRewardFeedRecordID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestRewardFeedRecordId");
            latestRewardFeedRecordID.value = 0;

            var leagueEndTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/leagueEndTimestamp");
            leagueEndTimestamp.value = 0;
        }

        public static void UpdateClubBadgeLastFeed(long clubID, long lastFeedID, long lastRewardID)
        {
            PlayerPrefs.SetString("LAST_CLUB_NEWS_FEED_ID", lastFeedID.ToString());
            PlayerPrefs.SetString("LAST_CLUB_REWARD_FEED_ID", lastRewardID.ToString());
            PlayerPrefs.SetString("NEWS_FEED_CLUB_ID", clubID.ToString());
        }

        public static bool IsExistNewFeed()
        {
            var newStuffExists = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "clubBadgeInfo/newStuffExists");
            if (newStuffExists.value == true)
                return true;

            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");

            if (meClubID.value > 0)
            {
                var latestFeedRecordID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestFeedRecordId");

                if (latestFeedRecordID.value > 0)
                {
                    long lastShowClubID = System.Convert.ToInt64(PlayerPrefs.GetString("NEWS_FEED_CLUB_ID", "0"));
                    if (lastShowClubID == 0 || lastShowClubID != meClubID.value)
                    {
                        return true;
                    }
                    else
                    {
                        long lastShowNewsFeedBadgeID = System.Convert.ToInt64(PlayerPrefs.GetString("LAST_CLUB_NEWS_FEED_ID", "0"));
                        if (latestFeedRecordID.value > lastShowNewsFeedBadgeID)
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool IsExistNewRewardFeed()
        {
            var meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");

            if (meClubID.value > 0)
            {
                var latestRewardFeedRecordID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/latestRewardFeedRecordId");

                if (latestRewardFeedRecordID.value > 0)
                {
                    long lastShowClubID = System.Convert.ToInt64(PlayerPrefs.GetString("NEWS_FEED_CLUB_ID", "0"));
                    if (lastShowClubID == 0 || lastShowClubID != meClubID.value)
                    {
                        return true;
                    }
                    else
                    {
                        long lastShowRewardFeedBadgeID = System.Convert.ToInt64(PlayerPrefs.GetString("LAST_CLUB_REWARD_FEED_ID", "0"));
                        if (latestRewardFeedRecordID.value > lastShowRewardFeedBadgeID)
                            return true;
                    }
                }
            }

            return false;
        }

        public static long GetClubLeagueEndTimestamp()
        {
            var leagueEndTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubBadgeInfo/leagueEndTimestamp");
            return leagueEndTimestamp != null ? leagueEndTimestamp.value : 0L;
        }

        public static string GetTierGroupName(int clubTier)
        {
            switch (clubTier)
            {
                case 0:
                default:
                    return "Mini";
                    
                case 1:
                    return "Minor";

                case 2:
                case 3:
                case 4:
                    return "Major";

                case 5:
                case 6:
                case 7:
                    return "Mega";

                case 8:
                case 9:
                case 10:
                    return "Grand";

                case 11:
                    return "Epic";
            }
        }

        public static string GetTierGroupSpriteName(int clubTier)
        {
            switch (clubTier)
            {
                case 0:
                default:
                    return "Mini";
                    
                case 1:
                    return "Minor";

                case 2:
                    return "Major01";
                case 3:
                    return "Major02";
                case 4:
                    return "Major03";

                case 5:
                    return "Mega01";
                case 6:
                    return "Mega02";
                case 7:
                    return "Mega03";

                case 8:
                    return "Grand01";
                case 9:
                    return "Grand02";
                case 10:
                    return "Grand03";

                case 11:
                    return "Epic";
            }
        }
    }
}
