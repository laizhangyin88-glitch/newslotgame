using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using ParadoxNotion;
using NodeCanvas.Framework;

namespace BagelCode.ClubArena
{
    public class ClubArenaSceneClubLeadersController
    {
        private ContextElement clubListElement;

        private ContextElement[] leadersProfileElements;
        private ClubArenaClubLeadersData[] clubRankDatas;

        private ContextElement buttonClubReaders;

        private bool isInit = false;

        private const int CLUB_LIST_COUNT = 3;

        public void OnInit(ContextElement rootElement)
        {
            if (isInit) return;

            ContextElement clubLeadersElement = ContextUtils.FindElement(rootElement, "Club Rank", ContextSearchingType.ChildrenSearch);

            clubListElement = ContextUtils.FindElement(clubLeadersElement, "All Club List", ContextSearchingType.ChildrenSearch);

            leadersProfileElements = new ContextElement[CLUB_LIST_COUNT];
            clubRankDatas = new ClubArenaClubLeadersData[CLUB_LIST_COUNT];
            for (int i = 0; i < CLUB_LIST_COUNT; ++i)
            {
                clubRankDatas[i] = new ClubArenaClubLeadersData();
                clubRankDatas[i].OnInit(ContextUtils.FindElement(clubListElement, string.Format("Club Cell {0}", i + 1), ContextSearchingType.ChildrenSearch));
                clubRankDatas[i].SetCaller(rootElement);
                leadersProfileElements[i] = ContextUtils.FindElement(clubLeadersElement, string.Format("{0:00}", i + 1), ContextSearchingType.ChildrenSearch);
            }

            buttonClubReaders = ContextUtils.FindElement(clubLeadersElement, "Button Primary", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonClubReaders,
                "OnClickClubReaders",
                rootElement,
                null
            );

            UpdateClubText();

            isInit = true;
        }

        public void UpdateClubText()
        {
            MetaContextElementUtils.SimpleSetTextGlobal(buttonClubReaders, "Text", "CLUB_ARENA_CLUB_LEADERS", ContextSearchingType.ChildrenSearch);
            long clubId = ClubArenaUtils.ClubInfo.GetValue<long>("id");

            List<Blackboard> contributionList = ClubArenaUtils.ClubMemberContributionList;

            int contributionIndex = GetContributionIndex(contributionList);
            for (int i = 0; i < CLUB_LIST_COUNT; ++i)
            {
                // set leaders profile
                if (contributionList != null && contributionList.Count > i)
                {
                    Blackboard bb = leadersProfileElements[i].GetComponent<Blackboard>();
                    BlackboardUtils.SetOrCreateValue(bb, "userInfo", contributionList[i]);
                    BlackboardUtils.SetOrCreateValue(bb, "updateProfile", true);
                }
                // set myclub data
                int idx = i + contributionIndex;
                if (contributionList != null && contributionList.Count > idx)
                {
                    clubRankDatas[i].myRank = idx + 1;
                    clubRankDatas[i].SetData(contributionList[idx]);
                }
                else
                {
                    clubRankDatas[i].SetData();
                }
            }

        }

        private int GetContributionIndex(List<Blackboard> contributionList)
        {
            int contributionIndex = 0;
            if (contributionList != null && contributionList.Count > CLUB_LIST_COUNT)
            {
                int count = contributionList.Count;

                if (ClubArenaUtils.GetEqualsUserId(contributionList[count - 1]))
                    contributionIndex = count - 2;
                else
                {
                    for (int i = 0; i < count; ++i)
                    {
                        if (ClubArenaUtils.GetEqualsUserId(contributionList[i]))
                        {
                            contributionIndex = i - 1;
                            break;
                        }
                    }
                }
            }
            return contributionIndex < 0 ? 0 : contributionIndex;
        }
    }

    public class ClubArenaClubLeadersData : ClubArenaClubRankBaseData
    {
        private Blackboard rootBB;
        private ContextElement caller;
        private ContextElement clickElement;

        private string userId = "";

        public int myRank = 0;

        private ContextElement profileAreaElement;

        public override void OnInit(ContextElement _rootElement)
        {
            base.OnInit(_rootElement);
            profileAreaElement = ContextUtils.FindElement(rootElement, "Profile Picture Area", ContextSearchingType.ChildrenSearch);
            clickElement = ContextUtils.FindElement(rootElement, "Cell Click Area", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(clickElement, ClickItem);
        }

        public override void SetData(Blackboard bb = null)
        {
            rootBB = bb;
            if (bb == null)
            {
                ActiveMyClub(false);
                SetTextName("-");
                SetTextPoint(0);
            }
            else
            {
                ActiveMyClub(ClubArenaUtils.GetEqualsUserId(bb));
                SetTextName(bb.GetValue<string>("name"));
                SetTextPoint(bb.GetValue<long>("point"));
                userId = bb.GetValue<string>("userId");

                Blackboard profileBB = profileAreaElement.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue(profileBB, "userInfo", bb);
                BlackboardUtils.SetOrCreateValue(profileBB, "updateProfile", true);
            }
            SetTextRank(myRank);
            profileAreaElement.gameObject.SetActive(true);
        }

        public void SetCaller(ContextElement _caller)
        {
            caller = _caller;
        }

        protected override void ClickItem()
        {
            if (!string.IsNullOrEmpty(userId))
                MetaContextElementUtils.SendEvent(clickElement, "OnClickClubProfile", userId, caller, null);
        }
    }
}