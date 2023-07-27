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
    public class ClubArenaRevengeItemsController : MonoBehaviour
    {
        public GameObject caller;
        public int userListIndex;

        private ContextElement rootElement;

        private ContextElement[] baseCells;
        private ContextElement userNameTextElement;
        private ContextElement userNumberElement;
        private ContextElement userPointElement;
        private ContextElement profileArea;

        private ContextElement attackElement;
        private ContextElement stealElement;

        private GameObject profilePicture;

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
            userNumberElement = ContextUtils.FindElement(rootElement, "Text Number", ContextSearchingType.ChildrenSearch);
            userPointElement = ContextUtils.FindElement(rootElement, "Text Point", ContextSearchingType.ChildrenSearch);

            profileArea = ContextUtils.FindElement(rootElement, "Profile Area", ContextSearchingType.ChildrenSearch);
            profilePicture = MetaObjectUtils.MakePrefab("lobby0", "Profile Picture Small", profileArea.transform, null, "Profile Picture");

            attackElement = ContextUtils.FindElement(rootElement, "Icon Attack", ContextSearchingType.ChildrenSearch);
            stealElement = ContextUtils.FindElement(rootElement, "Icon Steal", ContextSearchingType.ChildrenSearch);

            ContextElement revengeButtonAreaElement = ContextUtils.FindElement(rootElement, "Revenge Button Area", ContextSearchingType.ChildrenSearch);
            ContextElement buttonRevengeElement = ContextUtils.FindElement(revengeButtonAreaElement, "Button Revenge", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonRevengeElement,
                OnClickRevenge
            );

            rootElement.UpdateContext(true);
            isInit = true;
        }

        public void UpdateVariables(Blackboard revengeBB, int index, GameObject _caller)
        {
            caller = _caller;
            InitProperty();

            userListIndex = index;
            ActiveBaseCell(userListIndex % BASE_CELL_COUNT);
            ActiveStealType(revengeBB.GetValue<bool>("isTakenBySteal"));

            Blackboard bb = profileArea.GetComponent<Blackboard>();
            if (bb != null)
            {
                bb.SetValue("userInfo", revengeBB);
                bb.SetValue("updateProfile", true);

                MetaContextElementUtils.SetText(userNumberElement, (index + 1).ToString());
                MetaContextElementUtils.SetText(userNameTextElement, revengeBB.GetValue<string>("name"));
                MetaContextElementUtils.SetTextGlobal(userPointElement, "CLUB_ARENA_POPUP_REVENGE_POINT_TEXT", ClubArenaUtils.RevengePointNumberFormat(revengeBB.GetValue<long>("takenPoint")));

                profileArea.gameObject.SetActive(true);
            }
        }

        private void ActiveBaseCell(int activeIndex)
        {
            for (int i = 0; i < BASE_CELL_COUNT; ++i)
                baseCells[i].gameObject.SetActive(i == activeIndex);
        }

        private void ActiveStealType(bool isSteal)
        {
            stealElement.gameObject.SetActive(isSteal);
            attackElement.gameObject.SetActive(!isSteal);
        }

        private void OnClickRevenge()
        {
            if (caller != null)
            {
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_REVENGE_BUTTON).Play();
                MessageRouter router = Common.GetOrAddMessageRouter(caller);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new ParadoxNotion.EventData<int>(ClubArenaUtils.CLUB_ARENA_REVENGE_USER_BUTTON_CLICK, userListIndex), caller);
            }
        }
    }
}
