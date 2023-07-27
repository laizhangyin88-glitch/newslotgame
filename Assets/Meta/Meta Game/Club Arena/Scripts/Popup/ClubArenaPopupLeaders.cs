using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using ParadoxNotion.Services;
using ParadoxNotion;
using BagelCode.OSA_Scroll;

namespace BagelCode.ClubArena
{
    public class ClubArenaPopupLeaders : MonoBehaviour
    {
        private ContextElement rootElement;
        private Animator rootAnimator;
        private Blackboard rootBB;

        private ContextElement titleTextElement;
        private ContextElement symbolAreaElement;
        private ContextElement myClubNameTextElement;
        private ContextElement myClubPointTextElement;

        private OSA_ClubArenaLeadersController osaScrollContrller;

        public void OnInit()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();

            ContextElement titleGoldElement = ContextUtils.FindElement(rootElement, "Arena Title Gold", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(titleGoldElement, "Text", ContextSearchingType.ChildrenSearch);

            ContextElement contentAreaElement = ContextUtils.FindElement(rootElement, "Arena Leaders Contents Area", ContextSearchingType.ChildrenSearch);
            symbolAreaElement = ContextUtils.FindElement(contentAreaElement, "Club Symbol Area", ContextSearchingType.ChildrenSearch);
            myClubNameTextElement = ContextUtils.FindElement(contentAreaElement, "Text Name", ContextSearchingType.ChildrenSearch);
            myClubPointTextElement = ContextUtils.FindElement(contentAreaElement, "Text Point", ContextSearchingType.ChildrenSearch);

            ContextElement buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            ContextElement osaScrollElement = ContextUtils.FindElement(rootElement, "Arena Leaders Contents Area", ContextSearchingType.ChildrenSearch);
            osaScrollContrller = osaScrollElement.GetComponent<OSA_ClubArenaLeadersController>();
            osaScrollContrller.SetCaller(rootElement);

            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                "OnClose",
                rootElement,
                null
            );

            MetaSystem.SubscribeBackButton(this.GetHashCode(), OnClickClose);

            InitTexts();
        }

        public void OnClickClose()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());

            ClubArenaUtils.BIClientClickClubArenaPopup("close", rootBB.GetValue<string>("_biContextID"));

            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(caller.value);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData("OnPopupClose"), caller.value);
            }

            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        private void InitTexts()
        {
            Blackboard clubBB = ClubArenaUtils.ClubInfo;
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "CLUB_ARENA_POPUP_LEADERS_TITLE");
            MetaContextElementUtils.SetTextGlobal(myClubPointTextElement, "CLUB_ARENA_POPUP_LEADERS_POINT_TEXT", GetMyClubPoint());
            MetaContextElementUtils.SetText(myClubNameTextElement, clubBB.GetValue<string>("name"));
            MetaIconUtils.MakeClubSymbolIconObject(clubBB.GetValue<string>("symbol"), symbolAreaElement.transform, null);
        }

        private long GetMyClubPoint()
        {
            long clubPoint = 0;
            List<Blackboard> clubRankInfoList = ClubArenaUtils.ClubRankInfoList;
            if (clubRankInfoList != null && clubRankInfoList.Count > 0)
            {
                for(int i = 0; i < clubRankInfoList.Count; ++i)
                {
                    if (ClubArenaUtils.GetEqualsClubId(clubRankInfoList[i]))
                    {
                        clubPoint = clubRankInfoList[i].GetValue<long>("totalPoint");
                        break;
                    }
                }
            }
            return clubPoint;
        }

        public void SetActive(bool isActive)
        {
            rootAnimator?.SetBool("Active", isActive);
        }

        public void SendUserProfile(string userId)
        {
            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            MetaContextElementUtils.SendEvent(rootElement, "OnClickClubProfile", userId, caller.value.GetComponent<ContextElement>(), null);
        }
    }
}
