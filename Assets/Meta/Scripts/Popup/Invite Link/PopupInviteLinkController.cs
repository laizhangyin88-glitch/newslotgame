using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using Sirenix.OdinInspector;

namespace BagelCode
{
    public class PopupInviteLinkController : MonoBehaviour
    {
        private ContextElement rootElement;
        private Blackboard rootBlackboard;
        private Animator rootAnimator;

        private ContextElement titleTextElement;
        private ContextElement descTextElement;
        private ContextElement speechBalloonTextElement;
        private ContextElement bottomTextElement;

        private ContextElement invitePlusElement;
        private ContextElement inviteFBElement;

        private ContextElement buttonPlusElement;
        private ContextElement buttonFBElement;

        private long snsRewardCredit = 0L;
        private long fbRewardCredit = 0L;
        private string snsInviteInstallUrl = "";
        private string fbInviteInstallUrl = "";
        private string shareMessage = "";

        private string biContextID = "";

        private bool isInit = false;

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            rootAnimator = rootElement.GetComponent<Animator>();
            rootBlackboard = rootElement.GetComponent<Blackboard>();

            titleTextElement = ContextUtils.FindElement(rootElement, "Title Area/Title Anchor/Text", ContextSearchingType.FullNameSearch);
            descTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            speechBalloonTextElement = ContextUtils.FindElement(rootElement, "Tutorial Character/Speech Balloon Right Area/Text", ContextSearchingType.FullNameSearch);
            bottomTextElement = ContextUtils.FindElement(rootElement, "Bottom Area/Text Bottom", ContextSearchingType.FullNameSearch);

            invitePlusElement = ContextUtils.FindElement(rootElement, "Button Invite Plus", ContextSearchingType.ChildrenSearch);
            inviteFBElement = ContextUtils.FindElement(rootElement, "Button Invite Facebook", ContextSearchingType.ChildrenSearch);

            buttonPlusElement = ContextUtils.FindElement(invitePlusElement, "Button Shop", ContextSearchingType.ChildrenSearch);
            buttonFBElement = ContextUtils.FindElement(inviteFBElement, "Button Blue", ContextSearchingType.ChildrenSearch);

            ContextElement buttonClose = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            snsRewardCredit = BlackboardUtils.FindValue<long>(null, "/inviteInstallInfo/snsRewardCredit");
            fbRewardCredit = BlackboardUtils.FindValue<long>(null, "/inviteInstallInfo/fbMessageRewardCredit");
            snsInviteInstallUrl = BlackboardUtils.FindValue<string>(null, "/inviteInstallInfo/snsUrl");
            fbInviteInstallUrl = BlackboardUtils.FindValue<string>(null, "/inviteInstallInfo/fbMessageUrl");
            shareMessage = BlackboardUtils.FindValue<string>(null, "/inviteInstallInfo/shareMessage");
            biContextID = BlackboardUtils.FindValue<string>(rootBlackboard, "_biContextID");

            BlackboardUtils.SetOrCreateValue(rootBlackboard, "_snsRewardCredit", snsRewardCredit);
            BlackboardUtils.SetOrCreateValue(rootBlackboard, "_snsInstallUrl", snsInviteInstallUrl);
            BlackboardUtils.SetOrCreateValue(rootBlackboard, "_fbRewardCredit", fbRewardCredit);
            BlackboardUtils.SetOrCreateValue(rootBlackboard, "_fbInstallUrl", fbInviteInstallUrl);

            // Button Clickable Event
            MetaContextElementUtils.SetClickable(
                buttonPlusElement,
                "OnClickPlus",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonFBElement,
                "OnClickFacebook",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonClose,
                "OnClickClose",
                rootElement,
                null
            );
        }

        private void InitText()
        {
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "POPUP_INVITE_LINK_TITLE_TEXT");
            MetaContextElementUtils.SetTextGlobal(descTextElement, "POPUP_INVITE_LINK_DESC_TEXT", snsRewardCredit);
            MetaContextElementUtils.SetTextGlobal(speechBalloonTextElement, "POPUP_INVITE_LINK_SPEECH_BALLOON", snsRewardCredit);
            MetaContextElementUtils.SetTextGlobal(bottomTextElement, "POPUP_INVITE_LINK_BOTTOM_TEXT", snsRewardCredit);

            MetaContextElementUtils.SimpleSetTextGlobal(buttonPlusElement, "Text", "POPUP_INVITE_LINK_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonPlusElement, "Text 1M", "POPUP_INVITE_LINK_BUTTON_REWARD_TEXT", ContextSearchingType.ChildrenSearch, snsRewardCredit);

            MetaContextElementUtils.SimpleSetTextGlobal(buttonFBElement, "Text", "POPUP_INVITE_LINK_BUTTON_TEXT", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonFBElement, "Text 1M", "POPUP_INVITE_LINK_BUTTON_REWARD_TEXT", ContextSearchingType.ChildrenSearch, fbRewardCredit);
        }

        public void OnInit()
        {
            if (isInit) return;

            InitProperty();
            InitText();

            bool isFBActive = SocialManager.Instance.GetAvailableFacebookMessenger(fbInviteInstallUrl);
            SetActiveButtonFacebook(isFBActive);

            BIClientFriendsInvitePopup("trigger", RewardType.UNKNOWN, 0);

            isInit = true;
        }

        public void SetActiveButtonFacebook(bool isActive)
        {
            if (inviteFBElement != null)
                inviteFBElement.gameObject.SetActive(isActive);
        }

        public void SetActiveButtonPlus(bool isActive)
        {
            if (invitePlusElement != null)
                invitePlusElement.gameObject.SetActive(isActive);
        }

        public void OnClickInvitePlus()
        {
            BIClientFriendsInvitePopup("click_sns_invite", RewardType.CREDIT, fbRewardCredit);
            string snsInviteMessageUrl = snsInviteInstallUrl;
            if (!string.IsNullOrEmpty(shareMessage))
                snsInviteMessageUrl = shareMessage + "\n\n" + snsInviteMessageUrl;
            NativeHelper.Instance.ShareSNSUrl(snsInviteMessageUrl);
        }

        public void OnClickInviteFB()
        {
            BIClientFriendsInvitePopup("click_fb_message_invite", RewardType.CREDIT, fbRewardCredit);
            string FBMessengerInviteUrl = "fb-messenger://share?link=" + fbInviteInstallUrl;
#if UNITY_WEBGL && !UNITY_EDITOR
            NativeHelper.Instance.OpenUrl(FBMessengerInviteUrl);
#elif (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            Application.OpenURL(FBMessengerInviteUrl);
#endif
            //SocialManager.Instance.ShareMessengerFB(fbInviteInstallUrl,
            //    (string json) =>
            //    {
            //        if (json.Equals("Error"))
            //        {
            //            Debug.LogError("FB Share Failed : " + json);
            //            //SendEvent("OnShareError");
            //        }
            //        else
            //        {
            //            Debug.LogError("FB Share Success");
            //            //SendEvent("OnShareSuccess");
            //        }
            //    });
        }

        public void BIClientFriendsInvitePopup(string type, RewardType rewardType, long value)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = type;
            string reward = BiEventUtils.GetRewardJsonString(rewardType, value);
            
            customData["reward"] = string.IsNullOrEmpty(reward) ? null : reward;
            customData["context_id"] = biContextID;

            Analytics.CustomEvent("client_friends_invite_popup", customData);
        }
#if UNITY_EDITOR
        [Button]
        private void TestPendingActionOpenUrl(string url)
        {
            if (this.isActiveAndEnabled)
            {
                Debug.Log("Test Open PendingActionOpenUrl");
                PendingActionManager.Instance.OpenUrl(url);
            }
        }

        [Button]
        private void TestPushPendingAction(string url)
        {
            if (this.isActiveAndEnabled)
            {
                Debug.Log("Test Open PushPendingAction :: url : " + url);
                PendingActionManager.Instance.PushPendingAction(url);
            }
        }
#endif
    }
}