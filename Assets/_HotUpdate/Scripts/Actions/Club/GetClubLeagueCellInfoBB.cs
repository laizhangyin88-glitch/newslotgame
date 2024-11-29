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

    public class GetClubLeagueCellInfoBB : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        public BBParameter<int> cellIndex;
        public BBParameter<List<GameObject>> bgList; // other, myclub
        public BBParameter<List<GameObject>> memberIconList; // other, myclub
        public BBParameter<List<GameObject>> rankStateList; // 0 Promote, 1 Stay, 2 Demote
        public BBParameter<int> indexPromote;
        public BBParameter<int> indexDemote;
        public BBParameter<int> maxOpenedTier;

        public BBParameter<long> clubID;
        public BBParameter<int> maxMemberCount;
        public BBParameter<string> rankText;
        public BBParameter<string> clubNameText;
        public BBParameter<string> clubMessageText;
        public BBParameter<string> clubMemberCountText;
        public BBParameter<string> clubLeaguePointText;

        public BBParameter<bool> isPrivateClub;
        public BBParameter<bool> isRequiredLevel;
        public BBParameter<int> requiredLevel;

        private Blackboard clubInfo = null;
        private long meClubID = 0;

        protected override string info
        {
            get { return "Get Club League Cell Info BB"; }
        }

        protected override void OnExecute()
        {
            var cellInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);

            if (cellInfoBB != null && cellInfoBB.value != null)
            {
                bool isFromLeaguePopup = agent.GetVariable<bool>("isFromLeaguePopup")?.value ?? false;
                bool showRequirement = !isFromLeaguePopup;
                bool showClubMember = !isFromLeaguePopup;

                var root = agent.GetComponent<ContextElement>();
                var requirementElement = ContextUtils.FindElement(root, "Requirement", ContextSearchingType.ChildrenSearch);
                requirementElement.gameObject.SetActive(showRequirement);

                var clubMemberElement = ContextUtils.FindElement(root, "Club Member Area", ContextSearchingType.ChildrenSearch);
                clubMemberElement.gameObject.SetActive(showClubMember);

                meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId").value;
                clubInfo = cellInfoBB.value;
                clubID.value = cellInfoBB.value.GetValue<long>("id");
                var level = cellInfoBB.value.GetValue<int>("level");
                // maxMemberCount.value = ClubUtils.GetClubMaxMemberCount(level);
                var minPlayerLevel = cellInfoBB.value.GetValue<int>("minPlayerLevel");
                var joinType = cellInfoBB.value.GetValue<ClubJoinType>("joinType");

                bool error = false;
                rankText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CELL_RANK_TEXT", out error, cellIndex.value + 1);

                clubNameText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_CELL_NAME", out error, cellInfoBB.value.GetValue<string>("name"));
                clubMessageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_CELL_DESCRIPTION", out error, cellInfoBB.value.GetValue<string>("motd"));
                clubMemberCountText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_CELL_MEMBER_COUNT", out error, cellInfoBB.value.GetValue<int>("members"), ClubUtils.GetClubMaxMemberCount(level));
                clubLeaguePointText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CELL_POINT_TEXT", out error, cellInfoBB.value.GetValue<long>("leaguePoint"));

                if (meClubID == clubID.value)
                {
                    clubNameText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CELL_MY_CLUB_FORMAT", out error, clubNameText.value);
                    clubMessageText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CELL_MY_CLUB_FORMAT", out error, clubMessageText.value);
                    clubMemberCountText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CELL_MY_CLUB_FORMAT", out error, clubMemberCountText.value);
                    clubLeaguePointText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "CLUB_LEAGUE_CELL_MY_CLUB_FORMAT", out error, clubLeaguePointText.value);
                }

                isPrivateClub.value = joinType == ClubJoinType.PRIVATE ? true : false;
                isRequiredLevel.value = minPlayerLevel > 1 ? true : false;
                requiredLevel.value = minPlayerLevel;

                UpdateBG();
                UpdateRankState();
            }

            EndAction();
        }

        private void UpdateBG()
        {
            int cellStyle = 0;

            if (meClubID == clubID.value)
                cellStyle = 1;

            for (int i = 0; i < bgList.value.Count; ++i)
            {
                bgList.value[i].SetActive(i == cellStyle);
            }

            for (int i = 0; i < memberIconList.value.Count; ++i)
            {
                memberIconList.value[i].SetActive(i == cellStyle);
            }
        }

        private void UpdateRankState()
        {
            int rankState = 1;

            var leagueMaxTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEAGUE/MAX_TIER");
            var leagueTier = clubInfo.GetValue<int>("leagueTier");

            if (leagueTier == 0)
            {
                if (cellIndex.value <= indexPromote.value)
                    rankState = 0;
            }
            else if (leagueTier == leagueMaxTier.value || leagueTier >= maxOpenedTier.value)
            {
                if (cellIndex.value >= indexDemote.value)
                    rankState = 2;
            }
            else
            {
                if (cellIndex.value <= indexPromote.value)
                    rankState = 0;
                else if (cellIndex.value >= indexDemote.value)
                    rankState = 2;
            }

            for (int i = 0; i < rankStateList.value.Count; ++i)
            {
                rankStateList.value[i].SetActive(i == rankState);
            }
        }
    }
}