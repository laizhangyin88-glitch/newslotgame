using System.Collections.Generic;
using SlotMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using ParadoxNotion.Services;
using ParadoxNotion;

namespace BagelCode.ClubArena
{
    public class ClubArenaPopupAttackedController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement userInfoAreaElement;
        private ContextElement profileAreaElement;
        private ContextElement userNameTextElement;
        private ContextElement messageTextElement;

        private ContextElement buttonStartElement;
        private ContextElement buttonHelpElement;

        private string popupContextID = "";

        private bool isHelp;

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            userInfoAreaElement = ContextUtils.FindElement(rootElement, "User Info Area", ContextSearchingType.ChildrenSearch);
            profileAreaElement = ContextUtils.FindElement(userInfoAreaElement, "Profile Area", ContextSearchingType.ChildrenSearch);
            userNameTextElement = ContextUtils.FindElement(userInfoAreaElement, "Text Name", ContextSearchingType.ChildrenSearch);

            messageTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);

            buttonStartElement = ContextUtils.FindElement(rootElement, "Button Start", ContextSearchingType.ChildrenSearch);
            buttonHelpElement = ContextUtils.FindElement(rootElement, "Button Help", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonStartElement,
                OnClickStart
                );
            MetaContextElementUtils.SetClickable(
                buttonHelpElement,
                OnClickHelp
                );
        }

        private void InitData()
        {
            Blackboard bb = ClubArenaUtils.PointTaken;
            string popupTypeName = "";

            MetaContextElementUtils.SimpleSetTextGlobal(buttonStartElement, "Text", "CLUB_ARENA_POPUP_ATTACKED_START_BUTTON", ContextSearchingType.ChildrenSearch);
            if (isHelp)
            {
                MetaContextElementUtils.SimpleSetTextGlobal(buttonHelpElement, "Text", "CLUB_ARENA_POPUP_ATTACKED_HELP_BUTTON", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetTextGlobal(messageTextElement, "CLUB_ARENA_POPUP_ATTACKED_HELP_TEXT", bb.GetValue<long>("mostTakenAmount"), bb.GetValue<long>("takenAmountSum"));
                MetaContextElementUtils.SetText(userNameTextElement, bb.GetValue<string>("mostTakenUserName"));

                Blackboard profileBB = profileAreaElement.GetComponent<Blackboard>();

                if (profileBB != null)
                {
                    IBlackboard userBB = BlackboardUtils.GetOrCreateBlackboard(rootBB, "userInfo");
                    userBB.SetValue("profileUrl", bb.GetValue<string>("mostTakenUserProfileUrl"));
                    userBB.SetValue("userId", bb.GetValue<string>("mostTakenUserId"));

                    profileBB.SetValue("userInfo", userBB);
                    profileBB.SetValue("updateProfile", true);

                    userInfoAreaElement.gameObject.SetActive(true);
                    profileAreaElement.gameObject.SetActive(true);
                }
                GSManager.Instance.GetHandler(ClubArenaUtils.Sounds.CLUB_ARENA_POPUP_HELP).Play();
                buttonHelpElement.gameObject.SetActive(true);
                popupTypeName = "attacked_alarm_with_request_help";
            }
            else
            {
                MetaContextElementUtils.SetTextGlobal(messageTextElement, "CLUB_ARENA_POPUP_ATTACKED_START_TEXT", bb.GetValue<long>("takenAmountSum"));

                MetaContextElementUtils.SetText(userNameTextElement, "--");

                userInfoAreaElement.gameObject.SetActive(false);
                buttonHelpElement.gameObject.SetActive(false);
                popupTypeName = "attacked_alarm";
            }
            ClubArenaUtils.BIClientClubArenaPopup(popupTypeName, popupContextID);
        }

        public void OnInit(bool _isHelp, string contextID)
        {
            isHelp = _isHelp;
            popupContextID = contextID;

            InitProperty();
            InitData();
        }

        private void OnClickHelp()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("request_help", popupContextID);
            OnPopupClose("OnClickPopupHelp");
        }

        private void OnClickStart()
        {
            ClubArenaUtils.BIClientClickClubArenaPopup("start_club_arena", popupContextID);
            OnPopupClose("OnClickPopupClose");
        }

        private void OnPopupClose(string eventName)
        {
            Variable<GameObject> caller = BlackboardUtils.FindVariable<GameObject>(rootBB, "caller");
            if (caller != null && caller.value != null)
            {
                MessageRouter router = Common.GetOrAddMessageRouter(caller.value);
                router.Dispatch(MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName), caller.value);
            }

            PopupManager.Instance.Close(gameObject);
            rootAnimator?.SetTrigger("Close");
        }

        public void SetActive(bool isActive)
        {
            rootAnimator?.SetBool("Active", isActive);
        }
    }
}
