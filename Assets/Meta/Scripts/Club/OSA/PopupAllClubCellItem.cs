using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.OSA_Scroll;

namespace BagelCode
{
    public class PopupAllClubCellItem : MonoBehaviour
    {
        private ContextElement rootElement;
        private ContextElement caller;

        // Rank
        private ContextElement clubRankTextElement;
        private List<GameObject> clubRankIconList = new List<GameObject>();
        // Symbol
        private ContextElement symbolArea;
        private GameObject clubSymbol = null;
        // Info
        private ContextElement clubNameTextElement;
        private ContextElement clubDescTextElement;
        private ContextElement privateElement;
        private ContextElement requiredLevelElement;
        private ContextElement requiredLevelTextElement;
        private ContextElement memberTextElement;
        private ContextElement friendsTextElement;
        private ContextElement clubLpTextElement;

        private long clubId = 0;
        private bool isInit = false;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            // Club Rank
            ContextElement clubRankAreaElement = ContextUtils.FindElement(rootElement, "Club Rank Area", ContextSearchingType.ChildrenSearch);
            clubRankTextElement = ContextUtils.FindElement(clubRankAreaElement, "Text Rank", ContextSearchingType.ChildrenSearch);

            clubRankIconList.Clear();
            clubRankIconList.Add(ContextUtils.FindElement(clubRankAreaElement, "Rank 1st", ContextSearchingType.ChildrenSearch).gameObject);
            clubRankIconList.Add(ContextUtils.FindElement(clubRankAreaElement, "Rank 2nd", ContextSearchingType.ChildrenSearch).gameObject);
            clubRankIconList.Add(ContextUtils.FindElement(clubRankAreaElement, "Rank 3rd", ContextSearchingType.ChildrenSearch).gameObject);

            // Club Symbol
            symbolArea = ContextUtils.FindElement(rootElement, "Club Symbol Area", ContextSearchingType.ChildrenSearch);
            // Club Info
            clubNameTextElement = ContextUtils.FindElement(rootElement, "Text Club Name", ContextSearchingType.ChildrenSearch);
            clubDescTextElement = ContextUtils.FindElement(rootElement, "Text Club Description", ContextSearchingType.ChildrenSearch);

            ContextElement requirementElement = ContextUtils.FindElement(rootElement, "Requirement", ContextSearchingType.ChildrenSearch);
            privateElement = ContextUtils.FindElement(requirementElement, "Private", ContextSearchingType.ChildrenSearch);
            requiredLevelElement = ContextUtils.FindElement(requirementElement, "Required Level", ContextSearchingType.ChildrenSearch);
            requiredLevelTextElement = ContextUtils.FindElement(requiredLevelElement, "Text Required Level", ContextSearchingType.ChildrenSearch);

            memberTextElement = ContextUtils.FindElement(rootElement, "Text Member", ContextSearchingType.ChildrenSearch);
            friendsTextElement = ContextUtils.FindElement(rootElement, "Text Friends", ContextSearchingType.ChildrenSearch);
            clubLpTextElement = ContextUtils.FindElement(rootElement, "Text Club Lp", ContextSearchingType.ChildrenSearch);

            isInit = true;
        }

        private void SetData(Blackboard clubInfoBB)
        {
            clubId = clubInfoBB.GetValue<long>("id");
            int rank = clubInfoBB.GetVariable<int>("rank")?.value ?? 0;
            MetaContextElementUtils.SetText(clubRankTextElement, rank == 0 ? "-" : rank.ToString());
            for (int i = 0; i < clubRankIconList.Count; ++i)
                clubRankIconList[i].SetActive(i == (rank - 1));

            MetaContextElementUtils.SetText(clubNameTextElement, clubInfoBB.GetValue<string>("name"));
            MetaContextElementUtils.SetText(clubDescTextElement, clubInfoBB.GetValue<string>("motd"));

            ClubJoinType joinType = clubInfoBB.GetValue<ClubJoinType>("joinType");
            privateElement.gameObject.SetActive(joinType == ClubJoinType.PRIVATE);

            int minPlayerLevel = clubInfoBB.GetValue<int>("minPlayerLevel");
            MetaContextElementUtils.SetTextGlobal(requiredLevelTextElement, "CLUB_CELL_LEVEL", minPlayerLevel);
            requiredLevelElement.gameObject.SetActive(1 < minPlayerLevel);

            int level = clubInfoBB.GetValue<int>("level");
            int memberCount = clubInfoBB.GetValue<int>("members");
            int maxMemberCount = ClubUtils.GetClubMaxMemberCount(level);

            MetaContextElementUtils.SetTextGlobal(memberTextElement, memberCount < maxMemberCount ? "CLUB_CELL_MEMBER_COUNT" : "CLUB_CELL_MEMBER_MAX_COUNT", memberCount, maxMemberCount);
            MetaContextElementUtils.SetTextGlobal(friendsTextElement, "CLUB_CELL_FRIENDS_COUNT", clubInfoBB.GetValue<int>("friends"));
            MetaContextElementUtils.SetTextGlobal(clubLpTextElement, "SIMPLE_LP", clubInfoBB.GetValue<long>("leaguePoint"));

            if (clubSymbol != null)
                Destroy(clubSymbol);
            clubSymbol = MetaIconUtils.MakeClubSymbolIconObject(clubInfoBB.GetValue<string>("symbol"), symbolArea.transform, null);
        }

        public void UpdateVariables(Blackboard clubInfoBB, ContextElement _caller)
        {
            caller = _caller;

            InitProperty();
            SetData(clubInfoBB);
        }

        public void OnClickClubInfo()
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnClickClubInfo", clubId, caller, null);
        }
    }
}