using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupAccountJoin : MonoBehaviour
    {
        public ContextElement agent;
        public Blackboard rootBlackboard;
        public Animator rootAni;

        public void OnUpdate()
        {
            rootBlackboard = GetComponent<Blackboard>();
            rootAni = GetComponent<Animator>();
            
            agent = GetComponent<ContextElement>();
            agent.UpdateContext();

            MetaContextElementUtils.SimpleSetTextGlobal(agent, "Title Area/Text", "POPUP_JOIN_TITLE_TEXT", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(agent, "Text Sync", "POPUP_JOIN_SYNC_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(agent, "Text Get", "POPUP_JOIN_GET_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(agent, "Text Combine", "POPUP_JOIN_COMBINE_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(agent, "Text Share", "POPUP_JOIN_SHARE_TEXT", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SimpleSetTextGlobal(agent, "Text Flag", "POPUP_JOIN_SHARE_FLAG_TEXT", ContextSearchingType.ChildrenSearch);

            var buttonElement = ContextUtils.FindElement(agent, "Button Close", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                buttonElement,
                "OnClose",
                agent,
                null
            );

            var questionMarkButton = ContextUtils.FindElement(agent, "Button Question Mark", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                questionMarkButton,
                "OnReward",
                agent,
                null
            );

            // Find 2 buttons
            var textInfoElement = ContextUtils.FindElement(agent, "Text Info", ContextSearchingType.ChildrenSearch);
            var facebookButtonElement = ContextUtils.FindElement(agent, "Button Account Join", ContextSearchingType.ChildrenSearch);
            var emailButtonElement = ContextUtils.FindElement(agent, "Button Email Join", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SetClickable(
                facebookButtonElement,
                "OnFacebookJoin",
                agent,
                null
            );

            MetaContextElementUtils.SetClickable(
                emailButtonElement,
                "OnEmailJoin",
                agent,
                null
            );

            long facebookReward = BlackboardUtils.GetOrCreateVariable<long>(null, "/values/reward/FACEBOOK_CONNECT/credit").value;
            //long emailReward = BlackboardUtils.GetOrCreateVariable<long>(null, "/values/reward/EMAIL_CONNECT/credit").value;
            //long appleReward = BlackboardUtils.GetOrCreateVariable<long>(null, "/values/reward/APPLE_CONNECT/credit").value;

            bool isAppleJoined = false;
            bool isFacebookJoined = false;
            bool isEmailJoined = false;

            var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(null, "/ssoAccountInfo");

            if(ssoAccountInfo != null)
            {
#if UNITY_IOS && !UNITY_EDITOR
                var appleID = BlackboardUtils.GetOrCreateVariable<string>(null, "/ssoAccountInfo/appleId");
                if(appleID != null && !string.IsNullOrEmpty(appleID.value))
                    isAppleJoined = true;
#endif

                var facebookID = BlackboardUtils.GetOrCreateVariable<string>(null, "/ssoAccountInfo/facebookId");
                if(facebookID != null && !string.IsNullOrEmpty(facebookID.value))
                    isFacebookJoined = true;

                var email = BlackboardUtils.GetOrCreateVariable<string>(null, "/ssoAccountInfo/email");
                if(email != null && !string.IsNullOrEmpty(email.value))
                    isEmailJoined = true;

                if(isFacebookJoined)
                {
                    MetaContextElementUtils.SetTextGlobal(textInfoElement, "POPUP_JOIN_BOTTOM_JOIN_TEXT", facebookReward);
                    facebookButtonElement.gameObject.SetActive(false);
                }
                else
                {
                    MetaContextElementUtils.SetTextGlobal(textInfoElement, "POPUP_JOIN_BOTTOM_CONNECT_TEXT", facebookReward);
                }
            }
            else
            {
                MetaContextElementUtils.SetTextGlobal(textInfoElement, "POPUP_JOIN_BOTTOM_JOIN_TEXT", facebookReward);
            }

            MakeAppleLoginButton(isAppleJoined, emailButtonElement.transform.parent);

            MetaContextElementUtils.SimpleSetTextGlobal(facebookButtonElement, "Text", "BUTTON_JOIN_FACEBOOK", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(emailButtonElement, "Text", isEmailJoined ? "BUTTON_JOIN_JOINED" : "BUTTON_JOIN_EMAIL", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetBooleanProperty(emailButtonElement, !isEmailJoined);

            rootAni.SetBool("Active", true);
        }

        private void MakeAppleLoginButton(bool isAppleJoined, Transform parent)
        {
#if UNITY_IOS && !UNITY_EDITOR
            if(!isAppleJoined)
            {
                var appleLoginButton = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button White", parent);
                var appleLoginButtonElement = appleLoginButton.GetComponent<ContextElement>();
                appleLoginButtonElement.UpdateContext();

                MetaContextElementUtils.SetClickable(
                    appleLoginButtonElement,
                    "OnAppleJoin",
                    agent,
                    null
                );

                MetaContextElementUtils.SimpleSetTextGlobal(appleLoginButtonElement, "Text", "BUTTON_SIGN_WITH_APPLE", ContextSearchingType.ChildrenSearch);
            }
#endif
        }
    }
}