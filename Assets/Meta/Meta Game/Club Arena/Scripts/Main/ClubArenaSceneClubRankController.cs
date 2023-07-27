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
    public class ClubArenaSceneClubRankController
    {
        private ContextElement buttonClubTap;

        private ContextElement clubRankTextElement;
        private ContextElement clubTapTextElement;

        private ClubArenaClubRankData[] clubRankDatas;

        private const int ALL_CLUB_LIST_COUNT = 4;

        private bool isInit = false;

        private List<ClubRankItemData> clubRankItemList;

        public class ClubRankItemData
        {
            public int index;
            public int key;
            public string rankText;
            public long reward;
            public bool isRanker;
            public bool isMyRank;
        }

        public void OnInit(ContextElement rootElement)
        {
            if (isInit) return;

            ContextElement infoElement = ContextUtils.FindElement(rootElement, "Club Info", ContextSearchingType.ChildrenSearch);
            buttonClubTap = ContextUtils.FindElement(infoElement, "Button Primary", ContextSearchingType.ChildrenSearch);

            ContextElement myClubElement = ContextUtils.FindElement(infoElement, "My Club", ContextSearchingType.ChildrenSearch);
            //ContextElement myClubNameEmelent = ContextUtils.FindElement(myClubElement, "Text Club Name", ContextSearchingType.ChildrenSearch);
            //ContextElement myClubSymbolAreaElement = ContextUtils.FindElement(myClubElement, "Club Symbol Area", ContextSearchingType.ChildrenSearch);
            clubRankTextElement = ContextUtils.FindElement(myClubElement, "Text Rank", ContextSearchingType.ChildrenSearch);
            clubTapTextElement = ContextUtils.FindElement(buttonClubTap, "Text", ContextSearchingType.ChildrenSearch);

            clubRankDatas = new ClubArenaClubRankData[ALL_CLUB_LIST_COUNT];
            for (int i = 0; i < ALL_CLUB_LIST_COUNT; ++i)
            {
                clubRankDatas[i] = new ClubArenaClubRankData();
                clubRankDatas[i].OnInit(ContextUtils.FindElement(infoElement, string.Format("Club Cell {0}", i + 1), ContextSearchingType.ChildrenSearch));
                clubRankDatas[i].SetCaller(rootElement);
            }

            clubRankItemList = new List<ClubRankItemData>();

            MetaContextElementUtils.SetTextGlobal(clubRankTextElement, "CLUB_ARENA_CLUB_RANK", "--", "--");
            MetaContextElementUtils.SetTextGlobal(clubTapTextElement, "CLUB_ARENA_CLUB_TAP");
            //MetaContextElementUtils.SetText(myClubNameEmelent, MetaContextElementUtils.GetMaximumLengthExceededChangeText(BlackboardUtils.FindValue<string>(ClubArenaUtils.ClubInfo, "name"), 11));
            //MetaIconUtils.MakeClubSymbolIconObject(BlackboardUtils.FindValue<string>(ClubArenaUtils.ClubInfo, "symbol"), myClubSymbolAreaElement.transform, null);

            MetaContextElementUtils.SetClickable(
                buttonClubTap,
                "OnClickClubDetails",
                rootElement,
                null
            );

            UpdateClubText();

            isInit = true;
        }

        public void UpdateClubText()
        {
            MetaContextElementUtils.SetTextGlobal(clubRankTextElement, "CLUB_ARENA_CLUB_RANK", ClubArenaUtils.GetClubRank(), ClubArenaUtils.GetClubPercentile());

            List<Blackboard> clubRankInfoList = ClubArenaUtils.ClubRankInfoList;

            for (int i = 0; i < ALL_CLUB_LIST_COUNT; ++i)
            {
                if (clubRankInfoList != null && clubRankInfoList.Count > i)
                    clubRankDatas[i].SetData(clubRankInfoList[i]);
                else
                    clubRankDatas[i].SetData();
            }
        }
    }

    public class ClubArenaClubRankData : ClubArenaClubRankBaseData
    {
        private ContextElement caller;
        private ContextElement clubSymbolAreaElement;
        private ContextElement clickElement;
        private GameObject clubSymbolObject = null;

        public long clubId = 0;

        public override void OnInit(ContextElement _rootElement)
        {
            base.OnInit(_rootElement);
            clubSymbolAreaElement = ContextUtils.FindElement(rootElement, "Club Symbol Area", ContextSearchingType.ChildrenSearch);
            clickElement = ContextUtils.FindElement(rootElement, "Cell Click Area", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(clickElement, ClickItem);
        }

        public override void SetData(Blackboard bb = null)
        {
            base.SetData(bb);
            if (clubSymbolObject != null)
            {
                GameObject.Destroy(clubSymbolObject);
                clubSymbolObject = null;
            }

            if (bb == null)
            {
                ActiveMyClub(false);
                SetTextName("-");
                SetTextPoint(0);
                SetTextRank(0);
            }
            else
            {
                ActiveMyClub(ClubArenaUtils.GetEqualsClubId(bb));
                SetTextName(bb.GetValue<string>("clubName"));
                SetTextPoint(bb.GetValue<long>("totalPoint"));
                SetTextRank(bb.GetValue<int>("ranking"));
                clubSymbolObject = MetaIconUtils.MakeClubSymbolIconObject(bb.GetValue<string>("clubSymbol"), clubSymbolAreaElement.transform, null);
                clubId = bb.GetValue<long>("clubId");
            }
        }

        public void SetCaller(ContextElement _caller)
        {
            caller = _caller;
        }

        protected override void ClickItem()
        {
            if (clubId > 0)
                MetaContextElementUtils.SendEvent(clickElement, "OnClickClubInfo", clubId, caller, null);
        }
    }
}