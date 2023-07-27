using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using BagelCode;
using ParadoxNotion;
using NodeCanvas.Framework;
using BagelCode.OSA_Scroll;

namespace BagelCode
{
    public class PopupAllClubLeagueTierItem : MonoBehaviour
    {
        private ContextElement rootElement;
        private ContextElement caller;

        private ContextElement tierRankTextElement;
        private ContextElement badgeMyElement;
        private ContextElement coverElement;
        private List<ContextElement> leagueBGList = new List<ContextElement>();

        private int clubTier;
        private int myClubTier;
        private bool isInit = false;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            tierRankTextElement = ContextUtils.FindElement(rootElement, "Text Rank", ContextSearchingType.ChildrenSearch);
            badgeMyElement = ContextUtils.FindElement(rootElement, "Badge My", ContextSearchingType.ChildrenSearch);
            coverElement = ContextUtils.FindElement(rootElement, "Base Cover", ContextSearchingType.ChildrenSearch);

            leagueBGList.Clear();
            leagueBGList.Add(ContextUtils.FindElement(rootElement, "Base Minor", ContextSearchingType.ChildrenSearch));
            leagueBGList.Add(ContextUtils.FindElement(rootElement, "Base Major", ContextSearchingType.ChildrenSearch));
            leagueBGList.Add(ContextUtils.FindElement(rootElement, "Base Mega", ContextSearchingType.ChildrenSearch));
            leagueBGList.Add(ContextUtils.FindElement(rootElement, "Base Grand", ContextSearchingType.ChildrenSearch));

            isInit = true;
        }

        private void InitText()
        {
            MetaContextElementUtils.SetTextGlobal(tierRankTextElement, "TEXT_CLUB_TIER_NAME", clubTier);
        }

        public void UpdateVariables(int myClubLeagueTier, int clubLeagueTier, int currentTier, int index, ContextElement _caller)
        {
            caller = _caller;
            clubTier = clubLeagueTier;
            InitProperty();
            InitText();
            SetMyClubTier(myClubLeagueTier);
            SetCover(currentTier);
        }

        private void SetLeagueTierBG()
        {
            int leagueTierIndex = 0;
            switch (clubTier)
            {
                case 0:     // MINI
                case 1:     // MINOR
                    leagueTierIndex = 0;
                    break;
                case 2:     // MAJOR 1
                case 3:     // MAJOR 2
                case 4:     // MAJOR 3
                    leagueTierIndex = 1;
                    break;
                case 5:     // MEGA 1
                case 6:     // MEGA 2
                case 7:     // MEGA 3
                    leagueTierIndex = 2;
                    break;
                case 8:     // GRAND 1
                case 9:     // GRAND 2
                case 10:    // GRAND 3
                case 11:    // EPIC
                    leagueTierIndex = 3;
                    break;
            }

            for (int i = 0; i < leagueBGList.Count; ++i)
                leagueBGList[i].gameObject.SetActive(i == leagueTierIndex);

            badgeMyElement.gameObject.SetActive(clubTier == myClubTier);
        }

        public void SetCover(int selectTier)
        {
            coverElement.gameObject.SetActive(clubTier != selectTier);
        }

        public void SetMyClubTier(int myClubLeagueTier)
        {
            myClubTier = myClubLeagueTier;
            SetLeagueTierBG();
        }

        public void OnClickTier()
        {
            MetaContextElementUtils.SendEvent(rootElement, "OnClickLeague", clubTier, caller, null);
        }
    }
}