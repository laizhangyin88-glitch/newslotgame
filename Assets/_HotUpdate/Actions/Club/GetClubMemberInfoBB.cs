using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Club")]

public class GetClubMemberInfoBB : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;
    public BBParameter<int>     cellIndex;
    public BBParameter<int>     rankOffset;
    public BBParameter<List<GameObject>> bgList; // 0, 1 : member, 2 : Me
    public BBParameter<ClubAuthority> myAuthority;
    public BBParameter<bool>    enableShare;

    public BBParameter<string>  clubRankText;
    public BBParameter<string>  userNameText;
    public BBParameter<string>  authorityText;
    public BBParameter<string>  donationPointText;
    public BBParameter<string>  contributionPointText;
    public BBParameter<string>  leaguePointText;
    public BBParameter<string>  giftCountText;

    public BBParameter<string>  profileURL;
    public BBParameter<string>  offlineText;
    public BBParameter<int>     tier;
    public BBParameter<bool>    isOnline;
    public BBParameter<bool>    isOffline;
    public BBParameter<bool>    isMyFriend;

    public BBParameter<bool>    isMe;
    public BBParameter<bool>    isActiveMenuButton;
    private int typeIndex = -1;

    private string[] RANK_TYPE_FORMAT_LIST = new string[] {"CLUB_MEMBER_RANK_LEADER", "CLUB_MEMBER_RANK_MEMBER", "CLUB_MEMBER_RANK_ME"};
    private string[] NAME_TYPE_FORMAT_LIST = new string[] {"CLUB_MEMBER_NAME_LEADER", "CLUB_MEMBER_NAME_MEMBER", "CLUB_MEMBER_NAME_ME"};
    private string[] AUTHORITY_TYPE_FORMAT_LIST = new string[] {"CLUB_MEMBER_AUTHORITY_LEADER", "CLUB_MEMBER_AUTHORITY_MEMBER", "CLUB_MEMBER_AUTHORITY_ME"};
    private string[] DONATION_TYPE_FORMAT_LIST = new string[] {"CLUB_MEMBER_DONATION_LEADER", "CLUB_MEMBER_DONATION_MEMBER", "CLUB_MEMBER_DONATION_ME"};
    private string[] CONTRIBUTION_TYPE_FORMAT_LIST = new string[] {"CLUB_MEMBER_CONTRIBUTION_LEADER", "CLUB_MEMBER_CONTRIBUTION_MEMBER", "CLUB_MEMBER_CONTRIBUTION_ME"};
    private string[] LEAGUE_POINT_TYPE_FORMAT_LIST = new string [] {"CLUB_MEMBER_LEAGUE_POINT_LEADER", "CLUB_MEMBER_LEAGUE_POINT", "CLUB_MEMBER_LEAGUE_POINT_ME"};
    private string[] GIFT_COUNT_TYPE_FORMAT_LIST = new string [] {"CLUB_MEMBER_LEAGUE_POINT_LEADER", "CLUB_MEMBER_LEAGUE_POINT", "CLUB_MEMBER_LEAGUE_POINT_ME"};

    private const string AUTHORITY_LEADER = "CLUB_MEMBER_LEADER_TEXT";
    private const string AUTHORITY_COLEADER = "CLUB_MEMBER_COLEADER_TEXT";
    private const string AUTHORITY_MEMBER = "CLUB_MEMBER_MEMBER_TEXT";
    private const int maxDisplayDay = 9;

    protected override string info
    {
        get { return "Get Club Member Cell Info BB"; }
    }

    protected override void OnExecute()
    {
        if (agent == null) return;

        var memberInfo = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);

        if(memberInfo != null && memberInfo.value != null)
        {
            var name = memberInfo.value.GetValue<string>("name");
            var userID = memberInfo.value.GetValue<string>("userId");
            var authority = memberInfo.value.GetValue<ClubAuthority>("authority");
            var donation = memberInfo.value.GetValue<long>("donation");
            var contribution = memberInfo.value.GetValue<long>("contribution");
            var leaguePoint = memberInfo.value.GetValue<long>("leaguePoint");
            var lastOnlineTimestamp = memberInfo.value.GetValue<long>("lastOnlineTimestamp");
            profileURL.value = memberInfo.value.GetValue<string>("profileUrl");
            tier.value = memberInfo.value.GetValue<int>("tier");
            isOnline.value = memberInfo.value.GetValue<bool>("isOnline");
            isOffline.value = !isOnline.value;
            isMyFriend.value = BlackboardQueryUtils.IsMyFriend(userID);
            offlineText.value = GetOfflineText(lastOnlineTimestamp);

            long clubGiving = 0;
            if(enableShare.value)
                clubGiving = memberInfo.value.GetValue<long>("clubGiving");

            UpdateCellType(userID, authority);
            UpdateBG();

            bool isError = false;

            clubRankText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, RANK_TYPE_FORMAT_LIST[typeIndex], GetRankText(authority, cellIndex.value - rankOffset.value), out isError);
            userNameText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, NAME_TYPE_FORMAT_LIST[typeIndex], name, out isError);
            authorityText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, AUTHORITY_TYPE_FORMAT_LIST[typeIndex], GetAuthorityText(authority), out isError);
            donationPointText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, DONATION_TYPE_FORMAT_LIST[typeIndex], donation, out isError);
            contributionPointText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, CONTRIBUTION_TYPE_FORMAT_LIST[typeIndex], contribution, out isError);
            leaguePointText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, LEAGUE_POINT_TYPE_FORMAT_LIST[typeIndex], leaguePoint, out isError);
            giftCountText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, GIFT_COUNT_TYPE_FORMAT_LIST[typeIndex], clubGiving, out isError);
        }

        EndAction();
    }

    private void UpdateCellType(string userID, ClubAuthority authority)
    {
        string meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId").value;
        isMe.value = userID == meID;

        if(isMe.value)
        {
            typeIndex = 2;
            isActiveMenuButton.value = false;
        }
        else
        {
            switch(myAuthority.value)
            {
                case ClubAuthority.LEADER:
                    isActiveMenuButton.value = true;
                    break;
                case ClubAuthority.COLEADER:
                    isActiveMenuButton.value = authority == ClubAuthority.MEMBER;
                    break;
                default:
                    isActiveMenuButton.value = false;
                    break;
            }

            if(authority == ClubAuthority.LEADER)
                typeIndex = 0;
            else
                typeIndex = 1;
        }

    }

    private string GetRankText(ClubAuthority authority, int rankIndex)
    {
        string rankText = "";

        switch(authority)
        {
            case ClubAuthority.LEADER:
                rankText = "C";
                break;
            default:
                rankText = rankIndex.ToString();
                break;
        }

        return rankText;
    }

    private string GetOfflineText(long lastOnlineTimestamp)
    {
        TimeSpan timeOffset = BagelCode.TimeUtils.GetLastLoginTimeOffset(lastOnlineTimestamp);
        bool isError = false;
        string offlineText = "";

        if (lastOnlineTimestamp < 1)
        {
            offlineText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_MEMBER_LAST_ONLINE_MIN_TEXT", 0, out isError);

            return offlineText;
        }

        if (timeOffset.Days > maxDisplayDay)
        {
            offlineText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_MEMBER_LAST_ONLINE_DAY_OVER_TEXT", maxDisplayDay, out isError);
        }
        else if (timeOffset.Days > 0)
        {
            offlineText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_MEMBER_LAST_ONLINE_DAY_TEXT", timeOffset.Days, out isError);
        }
        else if (timeOffset.Hours > 0)
        {
            offlineText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_MEMBER_LAST_ONLINE_HOUR_TEXT", timeOffset.Hours, out isError);
        }
        else
        {
            offlineText = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_MEMBER_LAST_ONLINE_MIN_TEXT", timeOffset.Minutes, out isError);
        }

        return offlineText;
    }

    private void UpdateBG()
    {
        int cellStyle = cellIndex.value%2;
        if(isMe.value) cellStyle = 2;

        for(int i=0; i<bgList.value.Count; ++i)
        {
            bgList.value[i].SetActive(i==cellStyle);
        }
    }

    private string GetAuthorityText(ClubAuthority authority)
    {
        switch(authority)
        {
            case ClubAuthority.LEADER:
                return StringTableUtils.GetString(StringTable.StringTableType.Global, AUTHORITY_LEADER);
            case ClubAuthority.COLEADER:
                return StringTableUtils.GetString(StringTable.StringTableType.Global, AUTHORITY_COLEADER);
        }

        return StringTableUtils.GetString(StringTable.StringTableType.Global, AUTHORITY_MEMBER);
    }
}

}
