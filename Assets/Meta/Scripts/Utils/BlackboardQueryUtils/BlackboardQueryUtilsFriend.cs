using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;
using System.Linq;

namespace BagelCode
{
    public static partial class BlackboardQueryUtils
    {
        public static void AppendMuteUserList(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var variable = BlackboardUtils.GetOrCreateVariable<List<string>>(
                MainBlackboard.Get(), "muteUserIdList");

            if (variable.value == null)
            {
                variable.value = new List<string>() { userId };
            }
            else if (!variable.value.Contains(userId))
            {
                variable.value.Add(userId);
            }
        }

        public static List<string> GetMuteUserIDList()
        {
            return BlackboardUtils.GetOrCreateVariable<List<string>>(
                MainBlackboard.Get(), "muteUserIdList")?.value ?? new List<string>();
        }

        public static bool IsBlockedUserId(string userId)
        {
            var blockedUserIdList = GetBlockedUserIDList();
            if (blockedUserIdList == null || blockedUserIdList.Count == 0) return false;

            return blockedUserIdList.Contains(userId);
        }

        public static List<string> GetBlockedUserIDList()
        {
            return BlackboardUtils.GetOrCreateVariable<List<string>>(
                MainBlackboard.Get(), "blockedUserIdList")?.value ?? new List<string>();
        }

        public static List<string> GetIgnoreUserIdAll()
        {
            var list = new List<string>();
            list.AddRange(GetBlockedUserIDList());
            list.AddRange(GetMuteUserIDList());
            return list;
        }

        public static void RemoveBlockedUserID(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var variable = BlackboardUtils.GetOrCreateVariable<List<string>>(
                MainBlackboard.Get(), "blockedUserIdList");

            if (variable.value != null &&
                variable.value.Contains(userId))
            {
                variable.value.Remove(userId);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BLOCKED_USER_CHANGED);
            }
        }

        public static void AppendBlockedUserID(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var variable = BlackboardUtils.GetOrCreateVariable<List<string>>(
                MainBlackboard.Get(), "blockedUserIdList");

            if (variable.value == null)
            {
                variable.value = new List<string>() { userId };
            }
            else if (!variable.value.Contains(userId))
            {
                variable.value.Add(userId);
            }

            RemoveSuggest(userId);

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BLOCKED_USER_CHANGED);
        }

        public static string GetFriendNameById(string userId)
        {
            var friendList = GetFriendList();

            for (int i = 0; i < friendList.Count; ++i)
            {
                if (friendList[i].GetValue<string>("userId").Equals(userId))
                {
                    return friendList[i].GetValue<string>("name");
                }
            }

            return "";
        }

        public static void AddFriend(FriendInfo friendInfo)
        {
            if (friendInfo == null) return;

            // var friendList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "friendList");

            var friendInfoBB = BlackboardUtils.CreateBlackboard("FriendInfo");
            ClientAPI2Blackboard.Serialize(friendInfoBB, friendInfo);

            BlackboardUtils.AddToBlackboardList(MainBlackboard.Get(), "friendList", friendInfoBB);
        }

        public static void RemoveFriend(string userId)
        {
            var friendList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "friendList");

            for (int i = 0; i < friendList.value.Count; ++i)
            {
                if (friendList.value[i].GetValue<string>("userId").Equals(userId))
                {
                    GameObject.Destroy(friendList.value[i].gameObject);
                    friendList.value.RemoveAt(i);
                    return;
                }
            }
        }

        public static void RejectAllRequests()
        {
            var friendList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "friendList");

            for (int i = 0; i < friendList.value.Count;)
            {
                if (friendList.value[i].GetValue<bool>("accepted") == false)
                {
                    friendList.value.RemoveAt(i);
                }
                else
                    ++i;
            }
        }

        public static bool UpdateFriendItemAsAccepted(string userId)
        {
            if (userId == null) return false;

            RemoveSuggest(userId);
            {
                var friendList = GetFriendList();

                for (int i = 0; i < friendList.Count; ++i)
                {
                    if (friendList[i].GetValue<string>("userId").Equals(userId)
                     && friendList[i].GetValue<bool>("accepted") == false)
                    {
                        friendList[i].SetValue("accepted", true);
                        return true;
                    }
                }
            }
            return false;
        }

        public static void RemoveSuggest(string userId)
        {
            if (userId == null) return;
            {
                var recommendedFriendList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "friendRecommendationList");

                for (int i = 0; i < recommendedFriendList.value.Count; ++i)
                {
                    if (recommendedFriendList.value[i].GetValue<string>("userId").Equals(userId))
                    {
                        GameObject.Destroy(recommendedFriendList.value[i].gameObject);
                        recommendedFriendList.value.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public static void UpdateCelebInfo()
        {
            var recommendedFriendList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/friendRecommendationList")?.value ?? new List<Blackboard>();
            var celebList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/celebrityList")?.value ?? new List<Blackboard>();
            string myId = BlackboardUtils.FindVariable<string>(null, "/me/userId")?.value ?? "";

            for (int i = recommendedFriendList.Count - 1; i >= 0; --i)
            {
                string userId = recommendedFriendList[i].GetValue<string>("userId");

                if (myId.Equals(userId) || IsMyFriend(userId) || IsBlockedUserId(userId))
                {
                    BlackboardUtils.RemoveAtBlackboardList(MainBlackboard.Get(), "friendRecommendationList", i);
                }
            }

            // List<string> onlineFriendUserIdList = BlackboardUtils.FindVariable<List<string>>(null, "/onlineFriendUserIdList").value ?? new List<string>();
            for (int i = celebList.Count - 1; i >= 0; --i)
            {
                string userId = celebList[i].GetValue<string>("userId");
                if (myId.Equals(userId))
                {
                    BlackboardUtils.RemoveAtBlackboardList(MainBlackboard.Get(), "celebrityList", i);
                }
            }

            UpdateChallengeFriendRecommendationList();
        }

        public static bool IsContainsUser(string userID, List<Blackboard> targetList)
        {
            for (int i = 0; i < targetList.Count; ++i)
            {
                if (userID.Equals(targetList[i].GetValue<string>("userId")))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsMyFriend(string userID)
        {
            return IsContainsUser(userID, GetFriendList(true));
        }

        public static bool IsFacebookFriend(string userID)
        {
            var allFriendList = GetFriendList();

            if (allFriendList == null || allFriendList == null) return false;

            Blackboard friendInfo = BlackboardQueryUtils.GetFriendInfo(userID, allFriendList);

            if (friendInfo != null && friendInfo.GetValue<FriendType>("type") == FriendType.FACEBOOK)
                return true;

            return false;
        }

        public static List<Blackboard> GetFriendList()
        {
            var list = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/friendList")?.value;
            if (list == null || list.Count == 0) return new List<Blackboard>();

            // todo shk friend list 복사해서 관리 최적화
            return list.Where(
                f => !IsBlockedUserId(BlackboardUtils.GetOrCreateVariable<string>(f, "userId")?.value)).ToList();
        }

        public static List<Blackboard> GetFriendList(bool isAccepted)
        {
            var allFriendList = GetFriendList();
            List<Blackboard> filteredFriendList = new List<Blackboard>();

            if (allFriendList != null && allFriendList != null)
            {
                for (int i = 0; i < allFriendList.Count; ++i)
                {
                    if (allFriendList[i].GetValue<bool>("accepted") == isAccepted)
                    {
                        filteredFriendList.Add(allFriendList[i]);
                    }
                }
            }

            return filteredFriendList;
        }

        public static List<Blackboard> GetMyOnlineFriendList()
        {
            List<Blackboard> myFriends = GetFriendList(true);
            List<Blackboard> onlineFriendList = new List<Blackboard>();
            List<string> onlineFriendUserIdList = BlackboardUtils.FindVariable<List<string>>(null, "/onlineFriendUserIdList").value;

            var myClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");

            for (int i = 0; i < myFriends.Count; ++i)
            {

                if ((myClubID.value == 0 || myClubID.value != myFriends[i].GetValue<long>("clubId"))
                     && onlineFriendUserIdList.Contains(myFriends[i].GetValue<string>("userId")))
                {
                    onlineFriendList.Add(myFriends[i]);
                }
            }

            return onlineFriendList;
        }

        public static List<Blackboard> GetRecommendationList()
        {
            List<Blackboard> recommendedFriendList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/friendRecommendationList").value;
            return recommendedFriendList;
        }

        public static List<Blackboard> GetCelebrityList()
        {
            List<Blackboard> celebList = new List<Blackboard>();
            celebList.AddRange(BlackboardUtils.FindVariable<List<Blackboard>>(null, "/celebrityList").value);

            List<string> onlineFriendUserIdList = BlackboardUtils.FindVariable<List<string>>(null, "/onlineFriendUserIdList").value;

            for (int i = 0; i < celebList.Count;)
            {
                string userId = celebList[i].GetValue<string>("userId");
                if (onlineFriendUserIdList.Contains(userId) || onlineClubUserIDList.Contains(userId))
                {
                    celebList.RemoveAt(i);
                }
                else
                    ++i;
            }

            return celebList;
        }

        public static Blackboard GetFriendInfo(string userId, List<Blackboard> targetList)
        {
            for (int i = 0; i < targetList.Count; ++i)
            {
                if (userId.Equals(targetList[i].GetValue<string>("userId")))
                {
                    return targetList[i];
                }
            }
            return null;
        }

        public static bool IsIgnoredUser(string userId, int reportCount = 0)
        {
            var maxReportCount = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/HIDE_PROFILE_REPORT_THRESHOLD");

            if (maxReportCount != null && reportCount >= maxReportCount.value)
            {
                return true;
            }
            else
            {
                string userPrefs = PlayerPrefs.GetString("ignore_list", string.Empty);

                List<string> ignoredList = new List<string>(userPrefs.Split(":".ToCharArray()));

                for (int i = 0; i < ignoredList.Count; ++i)
                {
                    if (ignoredList[i] == userId) return true;
                }
            }
            return false;
        }

        public static void RemoveOnlineFriendUserID(string userID)
        {
            var onlineFriendUserIdList = BlackboardUtils.FindVariable<List<string>>(null, "/onlineFriendUserIdList");

            if (onlineFriendUserIdList.value.Contains(userID))
            {
                onlineFriendUserIdList.value.Remove(userID);
            }
        }

        public static int GetFriendCollect()
        {
            var friendList = GetFriendList();
            int giftCount = 0;

            if (friendList != null)
            {
                for (int i = 0; i < friendList.Count; i++)
                {
                    if (friendList[i].GetValue<bool>("accepted"))
                    {
                        int friendGift = friendList[i].GetValue<int>("receivedGiftCount");
                        if (friendGift > 0)
                        {
                            giftCount += friendGift;
                        }
                    }
                }
            }
            return giftCount;
        }

        public static int GetFriendCoinRequest()
        {
            var friendList = GetFriendList();
            int requestCoinFriendCount = 0;

            if (friendList != null && friendList != null)
            {
                for (int i = 0; i < friendList.Count; i++)
                {
                    var accepted = friendList[i].GetValue<bool>("accepted");
                    if (!accepted)
                    {
                        int friendGift = friendList[i].GetValue<int>("receivedGiftCount");
                        if (friendGift > 0)
                        {
                            requestCoinFriendCount++;
                        }
                    }
                }
            }
            return requestCoinFriendCount;
        }

        public static List<Blackboard> GetChallengeFriendRecommendationList()
        {
            return BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "challengeFriendRecommendationList");
        }

        public static void UpdateChallengeFriendRecommendationList()
        {
            var recommendedFriendList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/friendRecommendationList");
            var challengeRecommendedFriendList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/challengeFriendRecommendationList");
            string myId = BlackboardUtils.FindVariable<string>(null, "/me/userId").value;

            for (int i = challengeRecommendedFriendList.value.Count - 1; i >= 0; --i)
            {
                string userId = challengeRecommendedFriendList.value[i].GetValue<string>("userId");
                if (myId.Equals(userId) || IsMyFriend(userId))
                    BlackboardUtils.RemoveAtBlackboardList(MainBlackboard.Get(), "challengeFriendRecommendationList", i);
            }
        }

        public static bool GetFriendInviteLinkVersion()
        {
#if UNITY_IOS || UNITY_ANDROID
            var enableInviteLink = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_INVITE_INSTALL");
            if (enableInviteLink != null && enableInviteLink.value)
            {
                int prefsValue = PlayerPrefs.GetInt("ShowFriendInviteLinkBadge", 1);
                return prefsValue > 0;
            }
#endif
            return false;
        }

        public static void SetFriendInviteLinkVersion()
        {
#if UNITY_IOS || UNITY_ANDROID
            var enableInviteLink = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_INVITE_INSTALL");
            if (enableInviteLink != null && enableInviteLink.value)
            {
                PlayerPrefs.SetInt("ShowFriendInviteLinkBadge", 0);
            }
#endif
        }
    }
}
