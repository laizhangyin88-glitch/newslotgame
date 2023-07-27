using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using BagelCode.Tasks.Actions.BI;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class PopupInstallLinkController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBlackboard;
        private Animator rootAnimator;

        private ContextElement titleTextElement;
        private ContextElement descTextElement;
        private ContextElement userNameTextElement;

        private ContextElement buttonAccountElement;
        private ContextElement buttonEmailElement;
        private ContextElement buttonAppleElement;

        private ContextElement profileElement;

        private ActionType actionType;
        private InviteInstallType inviteType;
        private string inviterUserId;
        private string biContextID;

        private bool isInit = false;
        private bool isAccountFacebook = false;
        private bool isAccountEmail = false;
        private bool isAccountApple = false;

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            rootAnimator = rootElement.GetComponent<Animator>();
            rootBlackboard = rootElement.GetComponent<Blackboard>();

            titleTextElement = ContextUtils.FindElement(rootElement, "Title Area/Title Anchor/Text", ContextSearchingType.FullNameSearch);
            descTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            userNameTextElement = ContextUtils.FindElement(rootElement, "Text Username", ContextSearchingType.ChildrenSearch);

            profileElement = ContextUtils.FindElement(rootElement, "Center Profile Area", ContextSearchingType.ChildrenSearch);

            ContextElement buttonClose = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            ContextElement buttonArea = ContextUtils.FindElement(rootElement, "Button Area", ContextSearchingType.ChildrenSearch);
            buttonAccountElement = ContextUtils.FindElement(buttonArea, "Button Account Join", ContextSearchingType.ChildrenSearch);
            buttonEmailElement = ContextUtils.FindElement(buttonArea, "Button Email Join", ContextSearchingType.ChildrenSearch);
            buttonAppleElement = ContextUtils.FindElement(buttonArea, "Button Apple Join", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                buttonAccountElement,
                "OnClickAccount",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonEmailElement,
                "OnClickEmail",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonAppleElement,
                "OnClickApple",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonClose,
                "OnClickClose",
                rootElement,
                null
            );

            inviterUserId = BlackboardUtils.FindValue<string>(rootBlackboard, "_inviterUserId");
            actionType = BlackboardUtils.FindValue<ActionType>(rootBlackboard, "_actionType");

            switch (actionType)
            {
                case ActionType.SNS_INVITE:
                    inviteType = InviteInstallType.SNS;
                    break;
                case ActionType.FB_MESSAGE_INVITE:
                    inviteType = InviteInstallType.FB_MESSAGE;
                    break;
                default:
                    inviteType = InviteInstallType.UNKNOWN;
                    break;
            }

#if !UNITY_IPHONE && !UNITY_EDITOR
            SetActiveButtonApple(false);
#endif
        }

        private void InitText()
        {
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "POPUP_INSTALL_LINK_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(descTextElement, "POPUP_INSTALL_LINK_DESC_TEXT");

            MetaContextElementUtils.SimpleSetTextGlobal(buttonAccountElement, "Text", "POPUP_INSTALL_LINK_FB_JOIN_BUTTON", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonEmailElement, "Text", "POPUP_INSTALL_LINK_EMAIL_JOIN_BUTTON", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonAppleElement, "Text", "POPUP_INSTALL_LINK_APPLE_JOIN_BUTTON", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetText(userNameTextElement, "");
        }

        public void InitData()
        {
            Blackboard userInfoBB = rootBlackboard.GetValue<Blackboard>("_userInfo");
            Blackboard bb = profileElement.GetComponent<Blackboard>();

            BlackboardUtils.SetOrCreateValue(bb, "userInfo", userInfoBB);
            BlackboardUtils.SetOrCreateValue(bb, "updateProfile", true);

            MetaContextElementUtils.SetText(userNameTextElement, userInfoBB.GetValue<string>("name"));

            BlackboardUtils.SetOrCreateValue(rootBlackboard, "_installType", inviteType);

            var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "ssoAccountInfo");
            if (ssoAccountInfo != null)
            {
                isAccountFacebook = !string.IsNullOrEmpty(BlackboardUtils.FindValue<string>(ssoAccountInfo.value, "facebookId"));
                isAccountEmail = !string.IsNullOrEmpty(BlackboardUtils.FindValue<string>(ssoAccountInfo.value, "email"));
                isAccountApple = !string.IsNullOrEmpty(BlackboardUtils.FindValue<string>(ssoAccountInfo.value, "appleId"));
            }
        }

        public void OnInit()
        {
            if (isInit) return;

            InitProperty();
            InitText();
            BIClientFriendsInviteWelcomePopup("trigger");
            isInit = true;
        }

        public void SetActiveButtonApple(bool isActive)
        {
            if (buttonAppleElement != null)
                buttonAppleElement.gameObject.SetActive(isActive);
        }

        public bool CheckSsoAcount()
        {
            bool isChanged = false;

            var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "ssoAccountInfo");
            if (ssoAccountInfo != null)
            {
                bool checkAccountFacebook = !string.IsNullOrEmpty(BlackboardUtils.FindValue<string>(ssoAccountInfo.value, "facebookId"));
                bool checkAccountEmail = !string.IsNullOrEmpty(BlackboardUtils.FindValue<string>(ssoAccountInfo.value, "email"));
                bool checkAccountApple = !string.IsNullOrEmpty(BlackboardUtils.FindValue<string>(ssoAccountInfo.value, "appleId"));

                if (isAccountFacebook != checkAccountFacebook)
                {
                    isAccountFacebook = checkAccountFacebook;
                    isChanged = isAccountFacebook;
                }
                else if (isAccountEmail != checkAccountEmail)
                {
                    isAccountEmail = checkAccountEmail;
                    isChanged = isAccountEmail;
                }
                else if (isAccountApple != checkAccountApple)
                {
                    isAccountApple = checkAccountApple;
                    isChanged = isAccountApple;
                }
            }

            return isChanged;
        }

        public void AsyncInviterData()
        {
            StartCoroutine(RequestInviterInfo());
        }

        public IEnumerator RequestInviterInfo()
        {
            bool isSuccess = false;
            bool isFinished = false;
            BagelCodeClientAPI.GetUserProfile(inviterUserId,
                (response) =>
                {
                    if(this == null || this.gameObject == null) return;

                    var bb = BlackboardUtils.GetOrCreateBlackboard(rootBlackboard, "userInfoResponse");
                    ClientAPI2Blackboard.Serialize(bb, response);

                    //if(!isMe.value)
                    rootBlackboard.SetValue("_userInfo", BlackboardUtils.GetOrCreateBlackboard(bb, "user"));
                    isSuccess = true;
                    isFinished = true;
                },
                (error) =>
                {
                    if(this == null || this.gameObject == null) return;
                    
                    isFinished = true;
                    switch (error.errorCode)
                    {
                        default:
                            GlobalErrorHandler.GlobalError(error);
                            break;
                    }
                });

            yield return new WaitUntil(() => isFinished);
            if (isSuccess)
                InitData();
        }

        public string SetVIPClubFunnelType(BI_client_vip_club_funnel.BIVIPClubFunnelType type)
        {
            string contextID = BiEventUtils.GenerateContextID();
            PlayerPrefs.SetInt("VIP_CLUB_FUNNEL_TYPE", (int)type);
            PlayerPrefs.SetString("VIP_CLUB_FUNNEL_CONTEXT_ID", contextID);
            return contextID;
        }

        public void BIClientFriendsInviteWelcomePopup(string type, string contextID = null)
        {
            //PlayerPrefs.SetInt("VIP_CLUB_FUNNEL_TYPE", (int) type.value);
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["invite_user_id"] = inviterUserId;
            customData["type"] = type;
            customData["context_id"] = string.IsNullOrEmpty(contextID) ? null : contextID;

            Analytics.CustomEvent("client_friends_invite_welcome_popup", customData);
        }
    }
}