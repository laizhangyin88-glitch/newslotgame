using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Chat
{
    public class ChatUserProfileInfo
    {
        public string userId = "";
        public string name = "";
        public string profileUrl = "";
        public string profileHighResolutionUrl = "";
        // public int level = 0;
        public int tier = 0;
        public int tierGroup = 0;
        public int reportCount = 0;
        public long clubId = 0;
        public long lastOnlineTimestamp = 0;
        public bool isOnline;
        public bool useOnlieMark;
        public bool useClubMark;
        public bool isMe;
        public int clubListIndex;
        public long leaguePoint;
        public ClubAuthority clubAuthority;
        public string countryCode;

        public ChatUserProfileInfo(UserProfile userProfile)
        {
            userId                   = userProfile.userId;
            name                     = userProfile.name;
            profileUrl               = userProfile.profileUrl;
            profileHighResolutionUrl = userProfile.profileHighResolutionUrl;
            tier                     = userProfile.tier;
            tierGroup                = TierUtils.GetTierGroup(tier);
            reportCount              = userProfile.reportCount;
            clubId                   = userProfile.clubId;
            lastOnlineTimestamp      = userProfile.lastOnlineTimestamp;
            isOnline                 = true;
            useOnlieMark             = false;
            useClubMark              = true;
            isMe                     = (userId == BlackboardQueryUtils.GetMyUserId());
            countryCode              = userProfile.countrySelected;
        }

        public ChatUserProfileInfo(ClubMemberListInfo userProfile, int index)
        {
            userId                   = userProfile.userId;
            name                     = userProfile.name;
            profileUrl               = userProfile.profileUrl;
            profileHighResolutionUrl = userProfile.profileUrl; // not exist profileHighResolutionUrl 
            tier                     = userProfile.tier;
            tierGroup                = TierUtils.GetTierGroup(tier);
            reportCount              = userProfile.reportCount;
            // clubId                   = userProfile.clubId; not exist.
            lastOnlineTimestamp      = userProfile.lastOnlineTimestamp;
            isOnline                 = userProfile.isOnline;
            useOnlieMark             = true;
            useClubMark              = false;
            isMe                     = (userId == BlackboardQueryUtils.GetMyUserId());
            clubListIndex            = index;
            leaguePoint              = userProfile.leaguePoint;
            clubAuthority            = userProfile.authority;
            countryCode              = userProfile.countrySelected;
        }

        public bool IsClub()
        {
            if(!useClubMark) return false;
            if(clubId == 0) return false;

            var myClubID  = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
            if(myClubID == null || myClubID.value == 0) return false;

            return myClubID.value == clubId;
        }

        public bool IsFacebook()
        {
            return BlackboardQueryUtils.IsFacebookFriend(userId);
        }
        
        public bool IsFriend()
        {
            return BlackboardQueryUtils.IsMyFriend(userId);
        }
    }
}
