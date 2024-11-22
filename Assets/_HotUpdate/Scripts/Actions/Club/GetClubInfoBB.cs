using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]

    public class GetClubInfoBB : ActionTask<Blackboard>
    {
        //public BBParameter<string> fromType;

        public BBParameter<string> clubInfoValue;
        public BBParameter<string> clubTierValue;
        public BBParameter<string> memberListValue;
        public BBParameter<string> myClubInfoValue;
        public BBParameter<string> leagueResultInfoValue;
        public BBParameter<string> clubLpBoostValue;

        public BBParameter<long> clubID;
        //public BBParameter<ClubJoinType> clubJoinType;

        public BBParameter<string> nameText;
        public BBParameter<string> levelText;
        public BBParameter<string> donationText;
        public BBParameter<string> contributionText;
        public BBParameter<string> leaguePointText;
        public BBParameter<string> noticeText;
        public BBParameter<float> expGaugeRate;

        public BBParameter<string> informationBalloonText;
        public BBParameter<string> donationBalloonText;
        public BBParameter<string> contributionBalloonText;
        public BBParameter<string> leaguePointBalloonText;
        public BBParameter<string> giftsSendBalloonText;

        public BBParameter<string> leaguePointBalloonText1;
        public BBParameter<string> leaguePointBalloonText2;
        public BBParameter<bool> leaguePointBalloonChart1Enabled;
        public BBParameter<bool> leaguePointBalloonChart1Applicabled;
        public BBParameter<string> leaguePointBalloonChart1Text;
        public BBParameter<string> leaguePointBalloonChart1Percent;
        public BBParameter<bool> leaguePointBalloonChart2Enabled;
        public BBParameter<bool> leaguePointBalloonChart2Applicabled;
        public BBParameter<string> leaguePointBalloonChart2Text;
        public BBParameter<string> leaguePointBalloonChart2Percent;
        public BBParameter<bool> leaguePointBalloonChart3Enabled;
        public BBParameter<bool> leaguePointBalloonChart3Applicabled;
        public BBParameter<string> leaguePointBalloonChart3Text;
        public BBParameter<string> leaguePointBalloonChart3Percent;
        public BBParameter<string> leaguePointBalloonChart3CoverDisabledText;
        public BBParameter<bool> leaguePointAllDisabled;

        public BBParameter<int> maxMemberCount;
        public BBParameter<bool> isMyClub;
        public BBParameter<bool> isFull;
        public BBParameter<bool> useJoinButton;
        //public BBParameter<string> joinButtonText;
        //public BBParameter<string> uiJoinType;
        //public BBParameter<string> restrictionText;
        public BBParameter<ClubAuthority> authority;

        public BBParameter<bool> isLeagueResult;

        //const string FROM_TYPE_INVITE = "FROM_INVITE";

        protected override string info
        {
            get { return "Get Club Info BB"; }
        }

        protected override void OnExecute()
        {
            var clubInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubInfoValue.value);
            var clubTierBB = BlackboardUtils.FindVariable<Blackboard>(agent, clubTierValue.value);
            var memberList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, memberListValue.value);
            var myClubInfo = BlackboardUtils.FindVariable<Blackboard>(agent, myClubInfoValue.value);
            var lpBoostInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubLpBoostValue.value);

            isMyClub.value = false;
            isLeagueResult.value = false;
            authority.value = ClubAuthority.UNKNOWN;
            donationBalloonText.value = "";
            contributionBalloonText.value = "";
            leaguePointBalloonText.value = "";
            useJoinButton.value = false;
            //joinButtonText.value = "";
            //restrictionText.value = "";
            //uiJoinType.value = "";

            leaguePointBalloonText1.value = "";
            leaguePointBalloonText2.value = "";
            leaguePointBalloonChart1Enabled.value = false;
            leaguePointBalloonChart1Applicabled.value = false;
            leaguePointBalloonChart1Text.value = "";
            leaguePointBalloonChart1Percent.value = "";
            leaguePointBalloonChart2Enabled.value = false;
            leaguePointBalloonChart2Applicabled.value = false;
            leaguePointBalloonChart2Text.value = "";
            leaguePointBalloonChart2Percent.value = "";
            leaguePointBalloonChart3Enabled.value = false;
            leaguePointBalloonChart3Applicabled.value = false;
            leaguePointBalloonChart3Text.value = "";
            leaguePointBalloonChart3Percent.value = "";
            leaguePointBalloonChart3CoverDisabledText.value = "";
            leaguePointAllDisabled.value = false;

            if (clubInfoBB != null)
            {
                int memberCount = memberList == null ? 0 : memberList.value.Count;
                int onlineMemberCount = 0;

                for (int i = 0; i < memberCount; ++i)
                {
                    bool isOnline = memberList.value[i].GetValue<bool>("isOnline");
                    if (isOnline)
                    {
                        onlineMemberCount++;
                    }
                }

                clubID.value = BlackboardUtils.FindVariable<long>(clubInfoBB.value, "id").value;
                //clubJoinType.value = BlackboardUtils.FindVariable<ClubJoinType>(clubInfoBB.value, "joinType").value;

                var clubName = clubInfoBB.value.GetValue<string>("name");
                var level = clubInfoBB.value.GetValue<int>("level");
                var exp = clubInfoBB.value.GetValue<long>("exp");
                var baseExp = clubInfoBB.value.GetValue<long>("baseExp");
                var notice = clubInfoBB.value.GetValue<string>("notice");
                var minPlayerLevel = clubInfoBB.value.GetValue<int>("minPlayerLevel");
                //var joinType = clubInfoBB.value.GetValue<ClubJoinType>("joinType");
                var leaguePointUnitCoins = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "values/club/LEAGUE/LEAGUE_POINT_UNIT_WIN_CREDIT");

                maxMemberCount.value = ClubUtils.GetClubMaxMemberCount(level);

                long leaguePoint = 0;
                if (clubTierBB != null)
                    leaguePoint = clubTierBB.value.GetValue<long>("leaguePoint");

                int maxLevel = ClubUtils.GetMaxLevel();
                long currentExp = exp - baseExp;

                int watcher = clubInfoBB.value.GetValue<int>("watcher");

                var totalContribution = clubInfoBB.value.GetValue<long>("totalContribution");
                // TODO: Add now playing parameter

                // informationBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_INFORMATION_BALLOON", memberCount, maxMemberCount.value, onlineMemberCount, watcher, minPlayerLevel, coCaptainMemberCount, totalCoCaptain);

                nameText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_CLUB_NAME", clubName);
                contributionBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_CONTRIBUTION_BALLOON");
                donationText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_DONATION_POINT", exp);
                contributionText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_CONTRIBUTION_POINT", totalContribution);
                leaguePointText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT", leaguePoint);
                giftsSendBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_GIFTS_SENT_BALLOON");

                if (lpBoostInfo != null && lpBoostInfo.value != null)
                {
                    // leaguePointBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_TEXT", leaguePointUnitCoins.value);
                    leaguePointBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_TEXT");

                    int totalPercent = lpBoostInfo.value.GetValue<int>("totalPercent");
                    leaguePointBalloonText1.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_TEXT_1");
                    // leaguePointBalloonText2.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_TEXT_2", leaguePointUnitCoins.value, totalPercent / 100.0);
                    leaguePointBalloonText2.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_TEXT_2");

                    //Debug.LogError("Enter");

                    var leaguePointTextElement = ContextUtils.FindElement(agent.GetComponent<ContextElement>(), "Member/Text League Point", ContextSearchingType.FullNameSearch);
                    leaguePointTextElement.GetComponent<Blackboard>().SetValue("_totalRatio", totalPercent / 100.0);

                    leaguePointBalloonChart1Text.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_1_TEXT");
                    leaguePointBalloonChart2Text.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_2_TEXT");
                    leaguePointBalloonChart3Text.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_3_TEXT");

                    bool isGlobalEnabled = lpBoostInfo.value.GetValue<bool>("isGlobalEnabled");
                    bool isTierBoostEnabled = lpBoostInfo.value.GetValue<bool>("isTierBoostEnabled");
                    bool isPurchaseBoostEnabled = lpBoostInfo.value.GetValue<bool>("isPurchaseBoostEnabled");

                    int eventPercent = lpBoostInfo.value.GetValue<int>("eventPercent");
                    int tierBoostPercent = lpBoostInfo.value.GetValue<int>("tierBoostPercent");
                    int purchasePercent = lpBoostInfo.value.GetValue<int>("purchasePercent");
                    int purchasePercentMax = lpBoostInfo.value.GetValue<int>("purchasePercentMax");

                    if (isGlobalEnabled)
                    {
                        leaguePointBalloonChart1Enabled.value = true;
                        if (eventPercent != 0)
                        {
                            leaguePointBalloonChart1Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_PERCENT", eventPercent / 100.0);
                            leaguePointBalloonChart1Applicabled.value = true;
                        }
                        else
                        {
                            leaguePointBalloonChart1Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_HYPHEN");
                            leaguePointBalloonChart1Applicabled.value = false;
                        }
                    }
                    else
                    {
                        leaguePointBalloonChart1Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_HYPHEN");
                        leaguePointBalloonChart1Enabled.value = false;
                    }

                    if (isTierBoostEnabled)
                    {
                        leaguePointBalloonChart2Enabled.value = true;

                        if (tierBoostPercent != 0)
                        {
                            leaguePointBalloonChart2Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_PERCENT", tierBoostPercent / 100.0);
                            leaguePointBalloonChart2Applicabled.value = true;
                        }
                        else
                        {
                            leaguePointBalloonChart2Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_HYPHEN");
                            leaguePointBalloonChart2Applicabled.value = false;
                        }
                    }
                    else
                    {
                        leaguePointBalloonChart2Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_HYPHEN");
                        leaguePointBalloonChart2Enabled.value = false;
                    }

                    if (isPurchaseBoostEnabled)
                    {
                        leaguePointBalloonChart3Enabled.value = true;

                        if (purchasePercent != 0)
                        {
                            leaguePointBalloonChart3Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_PERCENT", purchasePercent / 100.0);
                            leaguePointBalloonChart3Applicabled.value = true;
                        }
                        else
                        {
                            leaguePointBalloonChart3Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_HYPHEN");
                            leaguePointBalloonChart3Applicabled.value = false;
                            leaguePointBalloonChart3CoverDisabledText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_PURCHASE_DISABLED", purchasePercentMax / 100.0);
                        }
                    }
                    else
                    {
                        leaguePointBalloonChart3Percent.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_LEAGUE_POINT_BALLOON_CHART_HYPHEN");
                        leaguePointBalloonChart3Enabled.value = false;
                    }

                    if (!isGlobalEnabled && !isTierBoostEnabled && !isPurchaseBoostEnabled)
                    {
                        leaguePointAllDisabled.value = true;
                    }
                }


                if (string.IsNullOrEmpty(notice))
                {
                    noticeText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_NOTICE_DEFAULT");
                }
                else
                {
                    noticeText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_NOTICE", notice);
                }

                if (maxLevel > level)
                {
                    long nextExp = ClubUtils.GetClubNextExp(level);
                    long remainExp = nextExp - currentExp;
                    expGaugeRate.value = (float)currentExp / (float)nextExp;

                    int currentMemberCount = ClubUtils.GetClubMaxMemberCount(level);
                    int nextMemberCount = ClubUtils.GetClubMaxMemberCount(level + 1);

                    donationBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_DONATION_BALLOON", remainExp, nextMemberCount - currentMemberCount);
                }
                else
                {
                    expGaugeRate.value = 1f;

                    donationBalloonText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_DONATION_BALLOON_MAX");
                }

                if (myClubInfo != null)
                {
                    levelText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_YOUR_CLUB_LEVEL", level);

                    isMyClub.value = true;

                    authority.value = myClubInfo.value.GetValue<ClubAuthority>("authority");

                    var leagueResultInfo = BlackboardUtils.FindVariable<Blackboard>(agent, leagueResultInfoValue.value);

                    if (leagueResultInfo != null)
                    {
                        var myClubID = myClubInfo.value.GetValue<long>("clubId");
                        var leagueClubID = leagueResultInfo.value.GetValue<long>("clubId");

                        if (leagueClubID == myClubID)
                        {
                            isLeagueResult.value = true;
                        }
                    }
                }
                else
                {
                    var myClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId");
                    if (myClubID != null && clubID.value == myClubID.value)
                    {
                        levelText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_YOUR_CLUB_LEVEL", level);
                    }
                    else
                    {
                        levelText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_INFO_CLUB_LEVEL", level);
                    }
                }

                EndAction();
            }
        }
    }
}
