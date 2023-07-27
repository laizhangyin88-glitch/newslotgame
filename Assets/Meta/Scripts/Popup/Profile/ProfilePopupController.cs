using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using BagelCode.Chat;
using ParadoxNotion;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class ProfilePopupController : EventMonoBehaviour
    {
        private Blackboard bb;
        private ContextElement rootElement;

        private ContextElement profileElement;

        private ContextElement chatToggleContextElement;
        private Animator chatToggleAnimator;

        private bool isMe;
        private string userId;
        private GameObject countryIconObj = null;

        private ContextElement buttonVIPJoinBigElement;
        private ContextElement buttonVIPJoinSmallElement;
        private ContextElement buttonEnterCodeBigElement;
        private ContextElement buttonEnterCodeSmallElement;
        private ContextElement buttonFacebookConnectBigElement;
        private ContextElement buttonFacebookConnectSmallElement;
        private ContextElement buttonActivateBigElement;
        private ContextElement buttonActivateSmallElement;
        private ContextElement buttonEmailConnectElement;
        private ContextElement buttonTierElement;
        private ContextElement buttonReportElement;

        private ContextElement blockButtonTextElement;

        private ContextElement facebookFriendTextElement;

        public void InitProperty()
        {
            bb = GetComponent<Blackboard>();
            rootElement = GetComponent<ContextElement>();
            rootElement.UpdateContext();

            profileElement = ContextUtils.FindElement(rootElement, "Profile", ContextSearchingType.ChildrenSearch);

            chatToggleContextElement = ContextUtils.FindElement(profileElement, "Button Chat On Off", ContextSearchingType.ChildrenSearch);
            chatToggleAnimator = chatToggleContextElement.GetComponent<Animator>();

            buttonVIPJoinBigElement = ContextUtils.FindElement(profileElement, "Button VIP Join Big", ContextSearchingType.ChildrenSearch);
            buttonVIPJoinSmallElement = ContextUtils.FindElement(profileElement, "Button VIP Join Small", ContextSearchingType.ChildrenSearch);
            buttonEnterCodeBigElement = ContextUtils.FindElement(profileElement, "Button Enter Code Big", ContextSearchingType.ChildrenSearch);
            buttonEnterCodeSmallElement = ContextUtils.FindElement(profileElement, "Button Enter Code Small", ContextSearchingType.ChildrenSearch);
            buttonFacebookConnectBigElement = ContextUtils.FindElement(profileElement, "Button Facebook Connect Big", ContextSearchingType.ChildrenSearch);
            buttonFacebookConnectSmallElement = ContextUtils.FindElement(profileElement, "Button Facebook Connect Small", ContextSearchingType.ChildrenSearch);
            buttonActivateBigElement = ContextUtils.FindElement(profileElement, "Button Activate Big", ContextSearchingType.ChildrenSearch);
            buttonActivateSmallElement = ContextUtils.FindElement(profileElement, "Button Activate Small", ContextSearchingType.ChildrenSearch);
            buttonEmailConnectElement = ContextUtils.FindElement(profileElement, "Button Email Connect", ContextSearchingType.ChildrenSearch);
            buttonTierElement = ContextUtils.FindElement(profileElement, "Button Tier", ContextSearchingType.ChildrenSearch);
            buttonReportElement = ContextUtils.FindElement(profileElement, "Button Report", ContextSearchingType.ChildrenSearch);

            facebookFriendTextElement = ContextUtils.FindElement(profileElement, "Text Facebook Friend", ContextSearchingType.ChildrenSearch);

            blockButtonTextElement = ContextUtils.FindElement(profileElement, "Button Report/Text", ContextSearchingType.FullNameSearch);

            isMe = bb.GetValue<bool>("isMe");
            userId = bb.GetValue<string>("_userId");

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_BLOCK, OnBlock);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_UNBLOCK, OnUnblock);
        }

        public void UpdateBlockButton()
        {
            bool isBlocked = BlackboardUtils.GetOrCreateVariable<bool>(bb, "isBlocked")?.value ?? false;
            MetaContextElementUtils.SetTextGlobal(blockButtonTextElement,
                isBlocked ? "BUTTON_UNBLOCK" : "BUTTON_BLOCK_PHOTO");

            MetaContextElementUtils.SimpleSetActive(profileElement,
                "Profile Picture Area/Profile Picture/Image", !isBlocked, ContextSearchingType.FullNameSearch);
        }

        private void OnBlock(EventData eventData)
        {
            string targetUserId = (string)eventData.value;
            if(userId == targetUserId)
            {
                BlackboardUtils.SetOrCreateValue(bb, "isBlocked", true);
                MetaContextElementUtils.SetTextGlobal(blockButtonTextElement, "BUTTON_UNBLOCK");
            }
        }

        private void OnUnblock(EventData eventData)
        {
            string targetUserId = (string)eventData.value;
            if (userId == targetUserId)
            {
                BlackboardUtils.SetOrCreateValue(bb, "isBlocked", false);
                MetaContextElementUtils.SetTextGlobal(blockButtonTextElement, "BUTTON_BLOCK_PHOTO");
            }
        }

        public IEnumerator BlockRequestCoroutine()
        {
            // Loading
            GameObject loadingPopup = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingPopup = popupObj));

            bool isSuccess = false;
            bool isFail = false;

            var blockType = BlackboardUtils.GetOrCreateVariable<BlockType>(bb, "blockType")?.value ?? BlockType.USER;
            BagelCodeClientAPI.UserBlock(userId, blockType,
            (response) =>
            {
                isSuccess = true;
                BlackboardQueryUtils.AppendBlockedUserID(userId);
            },
            (error) =>
            {
                isFail = true;
                Debug.LogError(error.errorCode);
            });

            yield return new WaitUntil(() => isSuccess || isFail);

            if (isSuccess)
            {
                var eventData = new EventData<string>(MetaEventDefine.ON_BLOCK, userId);
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);

                // Disable photo
                MetaContextElementUtils.SimpleSetActive(profileElement, "Profile Picture Area/Profile Picture/Image", false, ContextSearchingType.FullNameSearch);
            }

            MetaPopupUtils.ClosePopup(loadingPopup);

            EventSender.SendCalleeCallback(gameObject, "OnRefreshProfile");
        }

        public IEnumerator UnblockRequestCoroutine()
        {
            // Loading
            GameObject loadingPopup = null;
            yield return StartCoroutine(MetaPopupUtils.OpenLoadingPopupCoroutine(
                (GameObject popupObj) => loadingPopup = popupObj));

            bool isSuccess = false;
            bool isFail = false;

            BagelCodeClientAPI.UserUnblock(userId,
                (response) =>
                {
                    isSuccess = true;
                    BlackboardQueryUtils.RemoveBlockedUserID(userId);

                    var eventData = new EventData<string>(MetaEventDefine.ON_UNBLOCK, userId);
                    EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
                },
                (error) =>
                {
                    isFail = true;
                    Debug.Log("User Unblock failure: " + error.errorCode.ToString());
                });

            yield return new WaitUntil(() => isSuccess || isFail);

            if (isSuccess)
            {
                // Enable photo
                MetaContextElementUtils.SimpleSetActive(profileElement, "Profile Picture Area/Profile Picture/Image", true, ContextSearchingType.FullNameSearch);
            }

            MetaPopupUtils.ClosePopup(loadingPopup);

            // todo interactable bug
            buttonReportElement.GetComponent<PIDButton>().interactable = false;
            buttonReportElement.GetComponent<PIDButton>().interactable = true;
        }

        public void UpdateStaticVariables()
        {

        }

        public void OpenBlockPopup()
        {
            // profile report bt todo
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Friend Request Block Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent, "");

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void UpdateAcountStatus()
        {
            string validateEmail = PlayerPrefs.GetString("VALIDATE_EMAIL", "");

            buttonVIPJoinBigElement.gameObject.SetActive(false);
            buttonVIPJoinSmallElement.gameObject.SetActive(false);
            buttonEnterCodeBigElement.gameObject.SetActive(false);
            buttonEnterCodeSmallElement.gameObject.SetActive(false);
            buttonFacebookConnectBigElement.gameObject.SetActive(false);
            buttonFacebookConnectSmallElement.gameObject.SetActive(false);
            buttonActivateBigElement.gameObject.SetActive(false);
            buttonActivateSmallElement.gameObject.SetActive(false);
            buttonEmailConnectElement.gameObject.SetActive(false);

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
                    if(!isEmailJoined)
                        UpdateEmailJoin();
                }
                else
                {
                    if(isEmailJoined)
                        UpdateFacebookJoin();
                    else
                        UpdateJoinPopup();
                }
            }
            else
            {
                UpdateJoinPopup();
            }

            if(isFacebookJoined)
                UpdateFacebookCount();

            UpdateButtonTier();
        }

        private void UpdateJoinPopup()
        {
            MetaContextElementUtils.SetClickable(
                buttonVIPJoinBigElement,
                "OnClickJoinVIP",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonVIPJoinSmallElement,
                "OnClickJoinVIP",
                rootElement,
                null
            );

            long facebookReward = BlackboardUtils.GetOrCreateVariable<long>(null, "/values/reward/FACEBOOK_CONNECT/credit").value;

            MetaContextElementUtils.SimpleSetTextGlobal(buttonVIPJoinBigElement, "Text 01", "BUTTON_PROFILE_BIG_VIPCLUB", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonVIPJoinBigElement, "Text 02", "BUTTON_PROFILE_BIG_REWARD_TEXT", ContextSearchingType.ChildrenSearch, facebookReward);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonVIPJoinSmallElement, "Text", "BUTTON_PROFILE_SMALL_JOIN", ContextSearchingType.ChildrenSearch);

            buttonVIPJoinBigElement.gameObject.SetActive(true);
            buttonVIPJoinSmallElement.gameObject.SetActive(true);
        }

        private void UpdateFacebookJoin()
        {
            MetaContextElementUtils.SetClickable(
                buttonFacebookConnectBigElement,
                "OnClickFacebook",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                buttonActivateSmallElement,
                "OnClickFacebook",
                rootElement,
                null
            );

            long facebookReward = BlackboardUtils.GetOrCreateVariable<long>(null, "/values/reward/FACEBOOK_CONNECT/credit").value;

            MetaContextElementUtils.SimpleSetTextGlobal(buttonFacebookConnectBigElement, "Text 01", "BUTTON_PROFILE_BIG_FACEBOOK", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonFacebookConnectBigElement, "Text 02", "BUTTON_PROFILE_BIG_REWARD_TEXT", ContextSearchingType.ChildrenSearch, facebookReward);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonActivateSmallElement, "Text", "BUTTON_PROFILE_SMALL_FACEBOOK", ContextSearchingType.ChildrenSearch);
            
            buttonFacebookConnectBigElement.gameObject.SetActive(true);
            buttonActivateSmallElement.gameObject.SetActive(true);
        }

        private void UpdateEmailJoin()
        {
            MetaContextElementUtils.SetClickable(
                buttonEmailConnectElement,
                "OnClickConnectEmail",
                rootElement,
                null
            );

            long emailReward = BlackboardUtils.GetOrCreateVariable<long>(null, "/values/reward/EMAIL_CONNECT/credit").value;

            MetaContextElementUtils.SimpleSetTextGlobal(buttonEmailConnectElement, "Text 01", "BUTTON_PROFILE_BIG_CONNECT_EMAIL", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SimpleSetTextGlobal(buttonEmailConnectElement, "Text 02", "BUTTON_PROFILE_BIG_REWARD_TEXT", ContextSearchingType.ChildrenSearch, emailReward);
            
            buttonEmailConnectElement.gameObject.SetActive(true);
        }

        private void UpdateFacebookCount()
        {
            facebookFriendTextElement.gameObject.SetActive(true);
            int fbFriendCount = BlackboardUtils.GetOrCreateVariable<int>(bb, "facebookFriendCount").value;
            MetaContextElementUtils.SetText(facebookFriendTextElement, fbFriendCount.ToString());
        }

        public void InitChatToggle()
        {
            if(chatToggleContextElement == null) return;

            // Hotfix CBN Google Issue
            chatToggleContextElement.gameObject.SetActive(false);

            // if (isMe || userId == BlackboardQueryUtils.GetMyUserId())
            // {
            //     chatToggleContextElement.gameObject.SetActive(false);
            //     rootBB.SetValue("_userId", BlackboardQueryUtils.GetMyUserId());
            // }
            // else
            // {
            //     chatToggleContextElement.gameObject.SetActive(true);
            //     bool isMute = ChatMessenger.Instance.IsMute(userId);
            //     if(chatToggleAnimator != null)
            //         chatToggleAnimator.SetBool("Active", !isMute);
            // }
        }

        public void RequestMute()
        {
            if(chatToggleAnimator != null)
                chatToggleAnimator.SetBool("Active", false);

            ChatMessenger.Instance.RequestMuteAsync(true, userId, null, null);
        }

        public void RequestMuteCancel()
        {
            if(chatToggleAnimator != null)
                chatToggleAnimator.SetBool("Active", true);
            ChatMessenger.Instance.RequestMuteAsync(false, userId,null,null);
        }

        public void UpdateCountry(string countryCode)
        {
            var countryIconAreaElement = ContextUtils.FindElement(rootElement, "Profile/Icon Country Area", ContextSearchingType.FullNameSearch);
            if(countryIconObj == null)
                countryIconObj = MetaObjectUtils.MakePrefab("Icon Image", countryIconAreaElement.transform);
            var countryIconElement = countryIconObj.GetComponent<ContextElement>();

            var countryNameTextElement = ContextUtils.FindElement(rootElement, "Profile/Text Country", ContextSearchingType.FullNameSearch);

            if(CountryUtils.ExistCountryCode(countryCode))
            {
                countryIconElement.gameObject.SetActive(true);

                string countryName = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_PROFILE_COUNTRY_TEXT", CountryUtils.GetCountryName(countryCode) );
                MetaContextElementUtils.SetText(countryNameTextElement, countryName);

                MetaContextElementUtils.SetContextCountryImage(countryIconElement, countryCode);
            }
            else
            {
                countryIconElement.gameObject.SetActive(false);
                MetaContextElementUtils.SetText(countryNameTextElement, "");
            }
            
        }

        public void UpdateButtonTier()
        {
            if (isMe)
            {
                MetaContextElementUtils.SetClickable(
                    buttonTierElement,
                    "OnClickTier",
                    rootElement,
                    null
                );
            }

            buttonTierElement.gameObject.SetActive(isMe);
        }

        public void PopupOpenVIPReward()
        {
            string bundleName = ApplicationSettings.MakeApplicationBundleName("lobby");
            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundleName, "VIP Rewards Scene").GetSceneInfo();

            var go = GameObject.Find("Popup Manager/Area");
            if (go != null && sceneInfo != null)
            {
                var popupVIPRewrad = SceneManager.LoadScene(go.transform, sceneInfo);

                BIClientClickButton(BiEventUtils.GetPopupContextId(popupVIPRewrad));

                PopupManager.Instance.Open(popupVIPRewrad);
                popupVIPRewrad.SetActive(true);
            }
        }

        private string GetContextID()
        {
            Variable<string> contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "_biContextID");
            if (string.IsNullOrEmpty(contextId.value))
                contextId.value = BiEventUtils.GenerateContextID();
            return contextId.value;
        }

        private void BIClientClickButton(string contextId)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["button_name"] = "profile_vip_button";
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_click_button", customData);
        }
    }
}
