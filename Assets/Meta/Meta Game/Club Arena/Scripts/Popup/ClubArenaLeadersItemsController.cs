using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace BagelCode.ClubArena
{
    public class ClubArenaLeadersItemsController : MonoBehaviour
    {
        public int userIndex;
        public List<GameObject> profileIcons;
        public ContextElement caller;

        private ContextElement rootElement;
        private ContextElement nameTextElement;
        private ContextElement pointTextElement;
        private ContextElement rankTextElement;

        private ContextElement[] baseCells;
        private ContextElement profileAreaElement;
        private ContextElement profileIconAreaElement;

        private string userId;
        private bool isInit = false;

        private readonly int BASE_CELL_COUNT = 2;

        private void InitProperty()
        {
            if (isInit) return;

            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            baseCells = new ContextElement[BASE_CELL_COUNT];
            for (int i = 0; i < BASE_CELL_COUNT; ++i)
                baseCells[i] = ContextUtils.FindElement(rootElement, string.Format("Base Cell {0}", i + 1), ContextSearchingType.ChildrenSearch);

            nameTextElement = ContextUtils.FindElement(rootElement, "Text Name", ContextSearchingType.ChildrenSearch);
            pointTextElement = ContextUtils.FindElement(rootElement, "Text Point", ContextSearchingType.ChildrenSearch);
            rankTextElement = ContextUtils.FindElement(rootElement, "Text Rank", ContextSearchingType.ChildrenSearch);

            profileAreaElement = ContextUtils.FindElement(rootElement, "Profile Area", ContextSearchingType.ChildrenSearch);
            MetaObjectUtils.MakePrefab("lobby0", "Profile Picture Chatting Balloon", profileAreaElement.transform, null, "Profile Picture");

            profileIconAreaElement = ContextUtils.FindElement(rootElement, "Profile Icon Area", ContextSearchingType.ChildrenSearch);

            rootElement.UpdateContext(true);

            isInit = true;
        }

        public void UpdateVariables(Blackboard contributionBB, int index, ContextElement _caller)
        {
            InitProperty();

            userIndex = index;
            caller = _caller;
            ActiveBaseCell(userIndex % BASE_CELL_COUNT);
            ActiveProfileIcon();

            Blackboard bb = profileAreaElement.GetComponent<Blackboard>();
            if (bb != null)
            {
                bb.SetValue("userInfo", contributionBB);
                bb.SetValue("updateProfile", true);
                SetTextName(contributionBB.GetValue<string>("name"));
                SetTextPoint(contributionBB.GetValue<long>("point"));

                userId = contributionBB.GetValue<string>("userId");
                profileAreaElement.gameObject.SetActive(true);
            }
            else
            {
                SetTextName("-");
                SetTextPoint(0);
            }
            SetTextRank();
        }

        private void ActiveBaseCell(int activeIndex)
        {
            for (int i = 0; i < BASE_CELL_COUNT; ++i)
                baseCells[i].gameObject.SetActive(i == activeIndex);
        }

        private void ActiveProfileIcon()
        {
            if (profileIcons == null) return;

            if (profileIcons.Count > userIndex)
            {
                profileIconAreaElement.gameObject.SetActive(true);
                for (int i = 0; i < profileIcons.Count; ++i)
                    profileIcons[i].SetActive(i == userIndex);
            }
            else
                profileIconAreaElement.gameObject.SetActive(false);
        }

        private void SetTextName(string name)
        {
            if (nameTextElement != null)
                MetaContextElementUtils.SetText(nameTextElement, name);
        }

        private void SetTextPoint(long point)
        {
            if (pointTextElement != null)
                MetaContextElementUtils.SetTextGlobal(pointTextElement, "TEXT_COMMA_NUMBER", point);
        }

        private void SetTextRank()
        {
            if (rankTextElement != null)
                MetaContextElementUtils.SetText(rankTextElement, (userIndex + 1).ToString());
        }

        public void OnClickCellItem()
        {
            if (!string.IsNullOrEmpty(userId))
                MetaContextElementUtils.SendEvent(rootElement, "OnClickClubProfile", userId, caller, null);
        }
    }
}