using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode
{
    public class ClubLeagueRankCellController : MonoBehaviour
    {
        public Transform symbolParent;
        public List<GameObject> bgList; // other, myclub
        public List<GameObject> memberIconList; // other, myclub
        public List<GameObject> rankStateList; // 0 Promote, 1 St

        private ContextElement rootElement;
        private Blackboard rootBB;

        private ContextElement rankElement;
        private ContextElement nameElement;
        private ContextElement descTextElement;
        private ContextElement memberAreaElement;
        private ContextElement memberTextElement;
        private ContextElement friendCountElement;
        private ContextElement leaguePointElement;
        private ContextElement requirementElement;
        private ContextElement requiredLevelElement;
        private ContextElement requiredLevelTextElement;
        private ContextElement privateElement;

        private long meClubID;

        private Blackboard clubInfoBB;
        private long clubID;
        private int rank;
        private int indexPromote;
        private int indexDemote;
        private int maxOpenedTier;

        private GameObject symbolObj;

        private bool isInit = false;

        private const string RANK_TEXT_KEY = "CLUB_LEAGUE_CELL_RANK_TEXT";
        private const string NAME_TEXT_KEY = "CLUB_CELL_NAME";
        private const string DESCRIPTION_KEY = "CLUB_CELL_DESCRIPTION";
        private const string MEMBER_COUNT_KEY = "CLUB_CELL_MEMBER_COUNT";
        private const string POINT_TEXT_KEY = "CLUB_LEAGUE_CELL_POINT_TEXT";
        private const string MY_CLUB_FORMAT_KEY = "CLUB_LEAGUE_CELL_MY_CLUB_FORMAT";

        private const string FRIEND_COUNT_TEXT_KEY = "CLUB_CELL_FRIENDS_COUNT";

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private void InitProperty()
        {
            if(isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();

            rankElement = ContextUtils.FindElement(rootElement, "Text Club Rank", ContextSearchingType.ChildrenSearch);
            nameElement = ContextUtils.FindElement(rootElement, "Text Club Name", ContextSearchingType.ChildrenSearch);
            descTextElement = ContextUtils.FindElement(rootElement, "Text Club Description", ContextSearchingType.ChildrenSearch);

            memberAreaElement = ContextUtils.FindElement(rootElement, "Club Member Area", ContextSearchingType.ChildrenSearch);
            memberTextElement = ContextUtils.FindElement(memberAreaElement, "Text Member", ContextSearchingType.ChildrenSearch);
            friendCountElement = ContextUtils.FindElement(memberTextElement, "Text Friends", ContextSearchingType.ChildrenSearch);

            leaguePointElement = ContextUtils.FindElement(rootElement, "Text Club League Point", ContextSearchingType.ChildrenSearch);

            requirementElement = ContextUtils.FindElement(rootElement, "Requirement", ContextSearchingType.ChildrenSearch);
            requiredLevelElement = ContextUtils.FindElement(requirementElement, "Required Level", ContextSearchingType.ChildrenSearch);
            requiredLevelTextElement = ContextUtils.FindElement(requiredLevelElement, "Text Required Level", ContextSearchingType.ChildrenSearch);

            privateElement = ContextUtils.FindElement(requirementElement, "Private", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        public void UpdateVariables(OSA_Scroll.ClubLeagueRankModel_Cell model)
        {
            InitProperty();

            BlackboardUtils.SetOrCreateValue<GameObject>(rootBB, "caller", model.caller);

            clubInfoBB = model.infoBB;

            if (clubInfoBB != null)
            {
                clubID = model.infoBB.GetValue<long>("id");
                clubInfoBB = model.infoBB;
                rank = model.rank;
                indexPromote = model.indexPromote;
                indexDemote = model.indexDemote;
                maxOpenedTier = model.maxOpenedTier;

                // use fsm
                BlackboardUtils.SetOrCreateValue<long>(rootBB, "_clubID", clubID);

                // bool isFromLeaguePopup = rootBB.GetVariable<bool>("isFromLeaguePopup")?.value ?? false;
                bool showRequirement = !model.isFromLeaguePopup;
                bool showClubMember = !model.isFromLeaguePopup;

                requirementElement.gameObject.SetActive(showRequirement);
                memberAreaElement.gameObject.SetActive(showClubMember);

                meClubID = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "clubId").value;

                var level = clubInfoBB.GetValue<int>("level");
                var minPlayerLevel = clubInfoBB.GetValue<int>("minPlayerLevel");
                var joinType = clubInfoBB.GetValue<ClubJoinType>("joinType");

                string rankText = StringTableUtils.GetString(tableType, RANK_TEXT_KEY, rank + 1);
                string clubNameText = StringTableUtils.GetString(tableType, NAME_TEXT_KEY, clubInfoBB.GetValue<string>("name"));
                string clubMessageText = StringTableUtils.GetString(tableType, DESCRIPTION_KEY, clubInfoBB.GetValue<string>("motd"));
                string clubMemberCountText = StringTableUtils.GetString(tableType, MEMBER_COUNT_KEY, clubInfoBB.GetValue<int>("members"), ClubUtils.GetClubMaxMemberCount(level));
                string clubLeaguePointText = StringTableUtils.GetString(tableType, POINT_TEXT_KEY, clubInfoBB.GetValue<long>("leaguePoint"));

                if (meClubID == clubID)
                {
                    clubNameText = StringTableUtils.GetString(tableType, MY_CLUB_FORMAT_KEY, clubNameText);
                    clubMessageText = StringTableUtils.GetString(tableType, MY_CLUB_FORMAT_KEY, clubMessageText);
                    clubMemberCountText = StringTableUtils.GetString(tableType, MY_CLUB_FORMAT_KEY, clubMemberCountText);
                    clubLeaguePointText = StringTableUtils.GetString(tableType, MY_CLUB_FORMAT_KEY, clubLeaguePointText);
                }

                privateElement.gameObject.SetActive(joinType == ClubJoinType.PRIVATE ? true : false);

                MetaContextElementUtils.SetText(rankElement, rankText);
                MetaContextElementUtils.SetText(nameElement, clubNameText);
                MetaContextElementUtils.SetText(descTextElement, clubMessageText);
                MetaContextElementUtils.SetText(memberTextElement, clubMemberCountText);
                MetaContextElementUtils.SetText(leaguePointElement, clubLeaguePointText);

                bool isRequiredLevel = minPlayerLevel > 1 ? true : false;
                requiredLevelElement.gameObject.SetActive(isRequiredLevel);

                if(isRequiredLevel)
                {
                    int requiredLevel = minPlayerLevel;
                    MetaContextElementUtils.SetText(requiredLevelTextElement, StringTableUtils.GetString(tableType, "CLUB_CELL_LEVEL", requiredLevel));
                }

                int friendsCount = clubInfoBB.GetValue<int>("friends");
                if(friendsCount > 0)
                    MetaContextElementUtils.SetText(friendCountElement, StringTableUtils.GetString(tableType, "CLUB_CELL_FRIENDS_COUNT", friendsCount));
                else
                    MetaContextElementUtils.SetText(friendCountElement, "");


                MakeClubIcon();
                UpdateBG();
                UpdateRankState();
            }

            MetaContextElementUtils.SetClickable(
                rootElement,
                "OnSelectClub",
                rootElement,
                null
            );
        }

        private void MakeClubIcon()
        {
            if(symbolObj != null)
                GameObject.Destroy(symbolObj);

            var symbolName = clubInfoBB.GetValue<string>("symbol");

            if(!string.IsNullOrEmpty(symbolName))
            {
                symbolObj = MetaIconUtils.MakeClubSymbolIconObject(symbolName, symbolParent, null);
            }
        }

        private void UpdateBG()
        {
            int cellStyle = 0;

            if (meClubID == clubID)
                cellStyle = 1;

            for (int i = 0; i < bgList.Count; ++i)
            {
                bgList[i].SetActive(i == cellStyle);
            }

            for (int i = 0; i < memberIconList.Count; ++i)
            {
                memberIconList[i].SetActive(i == cellStyle);
            }
        }

        private void UpdateRankState()
        {
            int rankState = 1;

            var leagueMaxTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/club/LEAGUE/MAX_TIER");
            var leagueTier = clubInfoBB.GetValue<int>("leagueTier");

            if (leagueTier == 0)
            {
                if (rank <= indexPromote)
                    rankState = 0;
            }
            else if (leagueTier == leagueMaxTier.value || leagueTier >= maxOpenedTier)
            {
                if (rank >= indexDemote)
                    rankState = 2;
            }
            else
            {
                if (rank <= indexPromote)
                    rankState = 0;
                else if (rank >= indexDemote)
                    rankState = 2;
            }

            for (int i = 0; i < rankStateList.Count; ++i)
            {
                rankStateList[i].SetActive(i == rankState);
            }
        }
    }
}
