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
    public class ClubArenaClubRankBaseData
    {
        protected ContextElement rootElement;
        protected ContextElement myClubOnElement;
        protected ContextElement myClubOffElement;
        protected ContextElement rankTextElement;
        protected ContextElement pointTextElement;
        protected ContextElement nameTextElement;
        protected ContextElement iconRankAreaElement;
        protected ContextElement iconRankTextElement;
        protected ContextElement[] iconRankElements;

        protected const int RANK_ICON_COUNT = 3;

        public virtual void OnInit(ContextElement _rootElement)
        {
            rootElement = _rootElement;

            myClubOnElement = ContextUtils.FindElement(rootElement, "Base My Club", ContextSearchingType.ChildrenSearch);
            myClubOffElement = ContextUtils.FindElement(rootElement, "Base Common", ContextSearchingType.ChildrenSearch);
            rankTextElement = ContextUtils.FindElement(rootElement, "Text Rank", ContextSearchingType.ChildrenSearch);
            pointTextElement = ContextUtils.FindElement(rootElement, "Text Point", ContextSearchingType.ChildrenSearch);
            nameTextElement = ContextUtils.FindElement(rootElement, "Text Club Name", ContextSearchingType.ChildrenSearch);

            iconRankAreaElement = ContextUtils.FindElement(rootElement, "Icon Rank Area", ContextSearchingType.ChildrenSearch);
            iconRankTextElement = ContextUtils.FindElement(iconRankAreaElement, "Text Icon Rank", ContextSearchingType.ChildrenSearch);
            iconRankElements = new ContextElement[RANK_ICON_COUNT];

            for (int i = 0; i < RANK_ICON_COUNT; ++i)
                iconRankElements[i] = ContextUtils.FindElement(iconRankAreaElement, string.Format("Icon Rank {0}", i + 1), ContextSearchingType.ChildrenSearch);
        }

        public virtual void SetData(Blackboard bb = null) { }

        protected void ActiveMyClub(bool isMyClub)
        {
            myClubOnElement.gameObject.SetActive(isMyClub);
            myClubOffElement.gameObject.SetActive(!isMyClub);

            nameTextElement.GetComponent<Text>().color = isMyClub ? Color.white : Color.black;
            MetaContextElementUtils.SetColor(pointTextElement, isMyClub ? Color.white : Color.black);
        }

        protected virtual void ClickItem() { }

        protected void SetTextName(string name)
        {
            if (nameTextElement != null)
                MetaContextElementUtils.SetText(nameTextElement, name);
        }

        protected void SetTextPoint(long point)
        {
            if (pointTextElement != null)
                MetaContextElementUtils.SetTextGlobal(pointTextElement, "SIMPLE_NUMBER", point);
        }

        protected void SetTextRank(int rank)
        {
            if (rank <= 0)
            {
                iconRankAreaElement.gameObject.SetActive(false);
                MetaContextElementUtils.SetText(rankTextElement, "-");
            }
            else if (rank > RANK_ICON_COUNT)
            {
                iconRankAreaElement.gameObject.SetActive(false);
                MetaContextElementUtils.SetTextGlobal(rankTextElement, "CLUB_ARENA_CLUB_RANK_TEXT", rank, rank);
            }
            else
            {
                iconRankAreaElement.gameObject.SetActive(true);
                MetaContextElementUtils.SetTextGlobal(iconRankTextElement, "CLUB_ARENA_CLUB_RANK_TEXT", rank, rank);
                for (int i = 0; i < RANK_ICON_COUNT; ++i)
                    iconRankElements[i].gameObject.SetActive(i + 1 == rank);
                rankTextElement.gameObject.SetActive(false);
            }
        }
    }
}