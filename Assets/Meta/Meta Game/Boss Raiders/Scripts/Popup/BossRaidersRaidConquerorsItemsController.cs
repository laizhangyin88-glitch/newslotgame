using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders
{
    public class BossRaidersRaidConquerorsItemsController : MonoBehaviour
    {
        public int userListIndex;
        public List<GameObject> profileIcons;
        public ContextElement caller;

        private ContextElement rootElement;

        private ContextElement[] baseCells;
        private ContextElement userNameTextElement;
        private ContextElement userRankElement;
        private ContextElement userPointElement;
        private ContextElement profileArea;
        private ContextElement myRankingElement;

        private GameObject profilePicture;

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

            userNameTextElement = ContextUtils.FindElement(rootElement, "Text Name", ContextSearchingType.ChildrenSearch);
            userRankElement = ContextUtils.FindElement(rootElement, "Text Rank", ContextSearchingType.ChildrenSearch);
            userPointElement = ContextUtils.FindElement(rootElement, "Text Point", ContextSearchingType.ChildrenSearch);
            myRankingElement = ContextUtils.FindElement(rootElement, "Icon My", ContextSearchingType.ChildrenSearch);

            profileArea = ContextUtils.FindElement(rootElement, "Profile Area", ContextSearchingType.ChildrenSearch);
            profilePicture = MetaObjectUtils.MakePrefab("lobby0", "Profile Picture Small", profileArea.transform, null, "Profile Picture");

            rootElement.UpdateContext(true);
            isInit = true;
        }

        public void UpdateVariables(Blackboard rewardsBB, int index, ContextElement _caller)
        {
            InitProperty();

            userListIndex = index;
            caller = _caller;
            ActiveBaseCell(userListIndex % BASE_CELL_COUNT);
            ActiveProfileIcon();

            Blackboard bb = profileArea.GetComponent<Blackboard>();
            if (bb != null)
            {
                bb.SetValue("userInfo", rewardsBB);
                bb.SetValue("updateProfile", true);

                MetaContextElementUtils.SetText(userRankElement, (index + 1).ToString());
                MetaContextElementUtils.SetText(userNameTextElement, rewardsBB.GetValue<string>("userName"));
                MetaContextElementUtils.SetTextGlobal(userPointElement, "SIMPLE_NUMBER", rewardsBB.GetValue<int>("totalAttack"));

                userId = rewardsBB.GetValue<string>("userId");
                myRankingElement.gameObject.SetActive(userId.Equals(BlackboardQueryUtils.GetMyUserId()));
                profileArea.gameObject.SetActive(true);
            }
        }

        private void ActiveBaseCell(int activeIndex)
        {
            for (int i = 0; i < BASE_CELL_COUNT; ++i)
                baseCells[i].gameObject.SetActive(i == activeIndex);
        }

        private void ActiveProfileIcon()
        {
            if (profileIcons == null) return;

            for (int i = 0; i < profileIcons.Count; ++i)
                profileIcons[i].SetActive(i == userListIndex);
        }

        public void OnClickCellItem()
        {
            if (!string.IsNullOrEmpty(userId))
                MetaContextElementUtils.SendEvent(rootElement, "OnClickClubProfile", userId, caller, null);
        }
    }
}