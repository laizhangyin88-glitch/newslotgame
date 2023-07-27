using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode;
using ParadoxNotion;
using ParadoxNotion.Services;
using BagelCode.OSA_Scroll;
using SlotMaker.Json;

namespace BagelCode.ClubArena
{
    public class ClubArenaPopupRevengeController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement titleTextElement;
        private ContextElement contentAreaElement;
        private ContextElement myProfileNameTextElement;
        private ContextElement myprofilePointTextElement;
        private ContextElement randomProfileNameTextElement;

        private Blackboard myProfileBB;
        private Blackboard randomProfileBB;

        private bool isInit = false;

        public int selectedIndex = -1;
        public string popupContextID = "";

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            ContextElement arenaTitleAreaElement = ContextUtils.FindElement(rootElement, "Arena Title Gold", ContextSearchingType.ChildrenSearch);
            titleTextElement = ContextUtils.FindElement(arenaTitleAreaElement, "Text", ContextSearchingType.ChildrenSearch);

            contentAreaElement = ContextUtils.FindElement(rootElement, "Revenge List Contents Area", ContextSearchingType.ChildrenSearch);
            ContextElement myInfoElement = ContextUtils.FindElement(contentAreaElement, "My Info Base", ContextSearchingType.ChildrenSearch);
            ContextElement profileAreaElement = ContextUtils.FindElement(myInfoElement, "Profile Area", ContextSearchingType.ChildrenSearch);
            myProfileBB = profileAreaElement.GetComponent<Blackboard>();
            myProfileNameTextElement = ContextUtils.FindElement(myInfoElement, "Text Name", ContextSearchingType.ChildrenSearch);
            myprofilePointTextElement = ContextUtils.FindElement(myInfoElement, "Text Point", ContextSearchingType.ChildrenSearch);

            ContextElement randomInfoElement = ContextUtils.FindElement(contentAreaElement, "Revenge Random Base", ContextSearchingType.ChildrenSearch);
            ContextElement randomProfileAreaElement = ContextUtils.FindElement(randomInfoElement, "Profile Area", ContextSearchingType.ChildrenSearch);
            randomProfileBB = randomProfileAreaElement.GetComponent<Blackboard>();
            randomProfileNameTextElement = ContextUtils.FindElement(randomInfoElement, "Text Name", ContextSearchingType.ChildrenDeepSearch);

            ContextElement revengeButtonArea = ContextUtils.FindElement(randomInfoElement, "Revenge Button Area", ContextSearchingType.ChildrenSearch);
            ContextElement buttonRandomElement = ContextUtils.FindElement(revengeButtonArea, "Button Purchase", ContextSearchingType.ChildrenSearch);

            ContextElement buttonCloseElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            OSA_ClubArenaRevengeController osa = contentAreaElement.GetComponent<OSA_ClubArenaRevengeController>();
            osa.caller = gameObject;
            contentAreaElement.gameObject.SetActive(true);

            MetaContextElementUtils.SetClickable(
                buttonRandomElement,
                OnClickRandomEvent
            );

            MetaContextElementUtils.SetClickable(
                buttonCloseElement,
                OnClickCloseEvent
            );

        }

        private void InitData()
        {
            Blackboard meBB = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
            Blackboard opponentBB = ClubArenaUtils.OpponentState;

            SetUserProfile(myProfileBB, meBB);
            SetUserProfile(randomProfileBB, opponentBB);

            MetaContextElementUtils.SetTextGlobal(titleTextElement, "CLUB_ARENA_POPUP_REVENGE_TITLE");

            MetaContextElementUtils.SetText(myProfileNameTextElement, BlackboardUtils.FindVariable<string>(meBB, "name").value);
            MetaContextElementUtils.SetTextGlobal(myprofilePointTextElement, "CLUB_ARENA_POPUP_REVENGE_POINT_TEXT", ClubArenaUtils.RevengePointNumberFormat(ClubArenaUtils.MyState.GetValue<long>("point")));

            MetaContextElementUtils.SetText(randomProfileNameTextElement, BlackboardUtils.FindVariable<string>(opponentBB, "name").value);

            myProfileBB.gameObject.SetActive(true);
            randomProfileBB.gameObject.SetActive(true);

            popupContextID = rootBB.GetValue<string>("_biContextID");
        }

        private void InitEvents()
        {
            MetaSystem.SubscribeBackButton(this.GetHashCode(), OnClickCloseEvent);
        }

        private void ClearEvents()
        {
            MetaSystem.UnSubscribeBackButton(this.GetHashCode());
        }

        public void OnInit()
        {
            if (isInit) return;

            InitProperty();
            InitData();
            InitEvents();

            isInit = true;
        }

        public void OnClickCloseRevengeChange(int index)
        {
            // OnClickClose -> caller Blackboard Setting
            ClubArenaUtils.BIClientClickClubArenaPopup("revenge", popupContextID);

            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                BlackboardUtils.SetOrCreateValue(caller.value.GetComponent<Blackboard>(), "_revengeContextID", popupContextID);
            }
        }

        public void OnClickClose()
        {
            ClearEvents();

            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                BlackboardUtils.SetOrCreateValue(caller.value.GetComponent<Blackboard>(), ClubArenaUtils.CLUB_ARENA_REVENGE_USER_INDEX, selectedIndex);

                MessageRouter router = Common.GetOrAddMessageRouter(caller.value);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData("OnPopupClose"), caller.value);
            }

            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        public void SetActive(bool isActive)
        {
            rootAnimator?.SetBool("Active", isActive);
        }

        protected void SetUserProfile(Blackboard baseBB, Blackboard dataBB)
        {
            BlackboardUtils.SetOrCreateValue(baseBB, "userInfo", dataBB);
            BlackboardUtils.SetOrCreateValue(baseBB, "updateProfile", true);
        }

        private void OnClickRandomEvent()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("random", popupContextID);
            SendCallerEvent("OnClose");
        }

        private void OnClickCloseEvent()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("close", popupContextID);
            SendCallerEvent("OnClose");
        }

        private void SendCallerEvent(string eventName)
        {
            if (rootElement != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(rootElement);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName), rootElement);
            }
        }

        public string GetTargetUserIdList()
        {
            string targetList = "";
            List<Blackboard> revengeList = ClubArenaUtils.RevengeList;
            if (revengeList != null && revengeList.Count > 0)
            {
                List<Dictionary<string, object>> customData = new List<Dictionary<string, object>>();
                for (int i = 0; i < revengeList.Count; ++i)
                {
                    Dictionary<string, object> listData = new Dictionary<string, object>();
                    listData["user_id"] = revengeList[i].GetValue<string>("userId");
                    listData["type"] = revengeList[i].GetValue<bool>("isTakenBySteal") ? "steal" : "attack";
                    listData["value"] = revengeList[i].GetValue<long>("takenPoint");
                    customData.Add(listData);
                }
                targetList = SlotSimpleJson.SerializeObject(customData);
            }
            return targetList;
        }
    }
}
