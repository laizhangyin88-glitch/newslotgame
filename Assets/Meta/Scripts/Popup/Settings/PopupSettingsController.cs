using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;
using BagelCode.Chat;
using BagelCode.ClientModels;
using ParadoxNotion;
using System.Linq;

namespace BagelCode
{
    public class PopupSettingsController : EventMonoBehaviour
    {
        private ContextElement root;
        private Blackboard bb;

        private ContextElement pushNotificationButtonElement;
        private ContextElement deleteAccountButtonAreaElement;
        private ContextElement deleteAccountButtonElement;

        private Variable<bool> userOptionsPushNotification;
        private Variable<bool> userOptionsKudoJackpot;
        private Variable<bool> userOptionsKudoTournament;
        private Variable<bool> userOptionsGlobalChat;
        private Variable<bool> userOptionsKudoNewUserWelcome;
        private Variable<bool> userOptionsPipMode;

        private string platform = "";

        private string biContextID;
        private const string ON_CLOSE_EVENT = "OnClose";
        private const string ON_SET_PUSH_OFF_AT_SETTINGS = "OnSetPushOffTriggerIAM";

        private bool isInit = false;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;
        private const ContextSearchingType FULL = ContextSearchingType.FullNameSearch;

        private void OnDestroy()
        {
            MetaSystem.UnSubscribeBackButton(gameObject.GetHashCode());
        }

        public void InitProperty()
        {
            if (isInit) return;

            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();

            root.UpdateContext();

            platform = ApplicationSettings.GetPlatformName().ToUpper();

            userOptionsPushNotification = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/pushNotification");
            userOptionsKudoJackpot = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/kudoJackpot");
            userOptionsKudoTournament = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/kudoTournament");
            userOptionsGlobalChat = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/globalChat");
            userOptionsKudoNewUserWelcome = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/kudoNewUserWelcome");
            userOptionsPipMode = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "/userOptions/enablePipMode");

            // Toggles
            InitToggleButton(GetState("MUTE_MUSIC"), "Settings Cell BGM", "POPUP_SETTINGS_TEXT_BGM", "OnSettingBGM");
            InitToggleButton(GetState("MUTE_SFX"), "Settings Cell SFX", "POPUP_SETTINGS_TEXT_SFX", "OnSettingSFX");
            InitToggleButton(GetState("WALL_OF_EPIC_AUTO_UPDATE"), "Settings Cell WOE", "POPUP_SETTINGS_TEXT_WOE", "OnSettingWOE");

            // is available pip?
            if (BlackboardQueryUtils.IsPipModeEnabled() &&
                NativeHelper.Instance.GetAppSettingsPipModeAvailable())
            {
                InitToggleButton(userOptionsPipMode.value, "Settings Cell PIP", "POPUP_SETTINGS_TEXT_PIP", "OnSettingPIP");
            }
            else
            {
                var cellElement = ContextUtils.FindElement(root, "Settings Cell PIP", CHILDREN);
                MetaContextElementUtils.SetActive(cellElement, false);
            }

            var titleTextElement = ContextUtils.FindElement(root, "Title Area/Text", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetTextGlobal(titleTextElement, "POPUP_SETTINGS_TITLE");

            pushNotificationButtonElement = ContextUtils.FindElement(root, "Settings Cell PUSH/Base/On Off/Tab Settings On Off", ContextSearchingType.FullNameSearch);

            ContextElement closeButtonElement = ContextUtils.FindElement(root, "Button Close", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                ON_CLOSE_EVENT,
                root,
                null
            );

            // Delete Account
            deleteAccountButtonAreaElement = ContextUtils.FindElement(root, "Settings Cell Delete Account", ContextSearchingType.ChildrenSearch);
            deleteAccountButtonElement = ContextUtils.FindElement(deleteAccountButtonAreaElement, "Base/Button Delete", ContextSearchingType.FullNameSearch);
            bool isAvailableDeleteAccount = BlackboardQueryUtils.IsAvailableDeleteAccount();
            if (isAvailableDeleteAccount)
            {
                MetaContextElementUtils.SetActive(deleteAccountButtonAreaElement, true);
                MetaContextElementUtils.SimpleSetActive(deleteAccountButtonAreaElement, "On Off", false);
                MetaContextElementUtils.SimpleSetTextGlobal(deleteAccountButtonAreaElement, "Text", "POPUP_SETTINGS_TEXT_DELETE_ACCOUNT", ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SimpleSetTextGlobal(deleteAccountButtonElement, "Text", "POPUP_SETTINGS_BUTTON_DELETE", ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SetClickable(deleteAccountButtonElement, OnClickDeleteAccount);
            }
            else
            {
                MetaContextElementUtils.SetActive(deleteAccountButtonAreaElement, false);
            }

            // Back Button Event.
            MetaSystem.SubscribeBackButton(gameObject.GetHashCode(),
                () => { EventSender.SendEvent(gameObject, ON_CLOSE_EVENT); });

            biContextID = BlackboardUtils.GetOrCreateVariable<string>(bb, "_biContextID", true).value;
            if (string.IsNullOrEmpty(biContextID))
                BlackboardUtils.SetOrCreateValue(bb, "_biContextID", GetBIContextID());

            Register("OnSettingBGM", OnToggleBgm);
            Register("OnSettingSFX", OnToggleSfx);
            Register("OnSettingWOE", OnToggleWoe);
            Register("OnSettingPIP", OnTogglePip);

            isInit = true;




#if NEW_NET
            // 关掉设置界面的部分按钮
            var cellElement01 = ContextUtils.FindElement(root, "Settings Cell Join", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell FPS", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell PUSH", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell KUDO", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell WOE", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell Logout", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell Delete Account", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
            cellElement01 = ContextUtils.FindElement(root, "Settings Cell PIP", CHILDREN);
            MetaContextElementUtils.SetActive(cellElement01, false);
#endif


        }

        private bool GetState(string playerPrefsKey, int defaultValue = 1)
        {
            return PlayerPrefs.GetInt(playerPrefsKey, 1) > 0;
        }

        private void InitToggleButton(bool state, string elementName, string textKey, string clickEvent)
        {
            var cellElement = ContextUtils.FindElement(root, elementName, CHILDREN);

            MetaContextElementUtils.SimpleSetTextGlobal(cellElement, "Text", textKey, CHILDREN);

            var onOffElement = ContextUtils.FindElement(cellElement, "Base/On Off/Tab Settings On Off", FULL);
            MetaContextElementUtils.SetBooleanProperty(onOffElement, state);

            var cellAnim = onOffElement.GetComponent<Animator>();
            cellAnim.SetBool("Active", state);

            MetaContextElementUtils.SimpleSetClickable(onOffElement,
                "Button On", gameObject, EventSender.ON_CUSTOM_EVENT, clickEvent, false, false);
            MetaContextElementUtils.SimpleSetTextGlobal(onOffElement, "Button On/Text", "BUTTON_ON", FULL);

            MetaContextElementUtils.SimpleSetClickable(onOffElement,
                "Button Off", gameObject, EventSender.ON_CUSTOM_EVENT, clickEvent, false, false);
            MetaContextElementUtils.SimpleSetTextGlobal(onOffElement, "Button Off/Text", "BUTTON_OFF", FULL);

            MetaContextElementUtils.SimpleSetClickable(cellElement,
                "Base", gameObject, EventSender.ON_CUSTOM_EVENT, clickEvent, false, false);
        }

        private void OnToggleBgm()
        {
            OnToggle("Settings Cell BGM", "MUTE_MUSIC");
            GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
        }

        private void OnToggleSfx()
        {
            OnToggle("Settings Cell SFX", "MUTE_SFX");
            GSManager.Instance.MusicVolume = PlayerPrefs.GetFloat("MUTE_MUSIC", 1);
            GSManager.Instance.SfxVolume = PlayerPrefs.GetFloat("MUTE_SFX", 1);
        }

        private void OnTogglePip()
        {
            bool targetState = !userOptionsPipMode.value;

            bool setState = true;
            if (targetState)
            {
                // Check device settings
                if (NativeHelper.Instance.GetAppSettingsPipModeAvailable() &&
                    NativeHelper.Instance.GetAppSettingsPipModeEnabled() == false)
                {
                    // Go ApplicationSettings
                    NativeHelper.Instance.OpenAppSettingsPipMode();

                    setState = false;
                }
#if UNITY_EDITOR
                setState = true;
#endif
            }

            if (setState)
            {
                string stateText = targetState ? "on" : "off";
                AEUtils.SendAE("client_pip_mode_option", ("action", stateText));

                SetToggleState("Settings Cell PIP", targetState);
                userOptionsPipMode.value = targetState;
                PIPManager.Instance.UpdatePipState();

                BagelCodeClientAPI.SystemOptionsUpdateRequest(userOptionsPushNotification.value,
                                                               userOptionsKudoJackpot.value,
                                                               userOptionsKudoTournament.value,
                                                               userOptionsGlobalChat.value,
                                                               userOptionsKudoNewUserWelcome.value,
                                                               targetState, (response) => { }, (error) => { });
            }
        }

        private void OnToggleWoe()
        {
            bool state = OnToggle("Settings Cell WOE", "WALL_OF_EPIC_AUTO_UPDATE");
            Analytics.CustomEvent("client_woe_upload_option", new Dictionary<string, object>
            {
                { "action", state ? "on" : "off" }
            });
        }

        private void SetToggleState(string elementKey, bool state)
        {
            var cellElement = ContextUtils.FindElement(root, elementKey, CHILDREN);
            var toggleElement = ContextUtils.FindElement(cellElement, "Base/On Off/Tab Settings On Off", FULL);

            MetaContextElementUtils.SetBooleanProperty(toggleElement, state);
            toggleElement.GetComponent<Animator>().SetBool("Active", state);
        }

        private bool OnToggle(string elementKey, string playerPrefsKey)
        {
            var cellElement = ContextUtils.FindElement(root, elementKey, CHILDREN);
            var toggleElement = ContextUtils.FindElement(cellElement, "Base/On Off/Tab Settings On Off", FULL);

            bool state = MetaContextElementUtils.ToggleBooleanProperty(toggleElement);
            toggleElement.GetComponent<Animator>().SetBool("Active", state);

            if (playerPrefsKey != null)
                PlayerPrefs.SetInt(playerPrefsKey, state ? 1 : 0);

            return state;
        }

        private void OnClickDeleteAccount()
        {
            EventSender.SendEvent(gameObject, "OnDeleteAccount");
        }

        public void RefreshInteraction()
        {
            deleteAccountButtonElement.GetComponent<UnityEngine.UI.Button>().interactable = false;
            deleteAccountButtonElement.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }

        public IEnumerator RequestDeleteAccountCodeCoroutine(string email)
        {
            var loadingObj = MetaPopupUtils.OpenLoadingPopup();

            bool isSuccess = false;
            bool isFail = false;

            string code = "";

            BagelCodeClientAPI.RequestDeleteAccountSendVerificationCode(email,
                (response) =>
                {
                    isSuccess = true;
                    code = response.verificationCode;
                },
                (error) =>
                {
                    isFail = true;
                    Debug.LogError(error.errorCode);
                });

            yield return new WaitUntil(() => isSuccess || isFail);

            MetaPopupUtils.ClosePopup(loadingObj);

            if (isSuccess)
            {
                var eventData = new EventData<string>("OnSuccess", code);
                EventSender.SendEventInCoroutine(gameObject, eventData);
            }
            else if (isFail)
            {
                EventSender.SendEventInCoroutine(gameObject, "OnCancel");
            }
        }

        public IEnumerator RequestDeleteAccountCoroutine()
        {
            var loadingObj = MetaPopupUtils.OpenLoadingPopup();

            bool isSuccess = false;
            bool isFail = false;

            BagelCodeClientAPI.RequestDeleteAccountRequest(
                (response) =>
                {
                    isSuccess = true;
                },
                (error) =>
                {
                    isFail = true;
                    Debug.LogError(error.errorCode);
                });

            yield return new WaitUntil(() => isSuccess || isFail);

            MetaPopupUtils.ClosePopup(loadingObj);

            if (isSuccess)
            {
                EventSender.SendEventInCoroutine(gameObject, "OnRequested");
            }
            else if (isFail)
            {
                EventSender.SendEventInCoroutine(gameObject, "OnCancel");
            }
        }

        public void OpenPopupDeleteAccountInfo()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Delete Account Info Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            var popupBB = popupObj.GetComponent<Blackboard>();

            string contextId = BiEventUtils.GenerateContextID();
            BlackboardUtils.SetOrCreateValue(bb, "contextId", contextId);

            BlackboardUtils.SetOrCreateValue(popupBB, "isInfo", true);
            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenPopupDeleteAccountVerification()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Delete Account Verification Email Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            var popupBB = popupObj.GetComponent<Blackboard>();

            string contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextId")?.value ?? "";

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenPopupDeleteAccountInputCode(string email, string verificationCode)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Delete Account Verification Email Code Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            var popupBB = popupObj.GetComponent<Blackboard>();

            string contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextId")?.value ?? "";

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);
            BlackboardUtils.SetOrCreateValue(popupBB, "email", email);
            BlackboardUtils.SetOrCreateValue(popupBB, "verificationCode", verificationCode);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenPopupDeleteAccountConfirm()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Delete Account Info Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            var popupBB = popupObj.GetComponent<Blackboard>();

            string contextId = BlackboardUtils.GetOrCreateVariable<string>(bb, "contextId")?.value ?? "";

            BlackboardUtils.SetOrCreateValue(popupBB, "contextId", contextId);
            BlackboardUtils.SetOrCreateValue(popupBB, "isInfo", false);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OpenPopupDeleteAccountRequested()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Delete Account Removal Requested Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void TogglePushNotification()
        {
            bool setToggle = true;
            bool targetState = !userOptionsPushNotification.value;

            if (targetState)
            {
                // Check TriggerIAM. 
                if (NativeHelper.Instance.GetPushNotificationSubscribed() == false)
                {
                    // AE - client_device_push_setting
                    BlackboardQueryUtils.BI_Device_Push_Setting(biContextID);
                    // Go ApplicationSettings
                    NativeHelper.Instance.OpenAppSettings();
                }
            }
            else
            {
                if (NativeHelper.Instance.GetPushNotificationSubscribed())
                {
                    if (IAMRouter.Instance.CheckTriggerIAM(InAppMessageTriggerType.SET_PUSH_OFF_AT_SETTINGS))
                    {
                        // Skip toggle. 
                        setToggle = false;
                        EventSender.SendEvent(gameObject, ON_SET_PUSH_OFF_AT_SETTINGS);
                    }
                }
            }
            if (setToggle)
                SetPushNotification(!userOptionsPushNotification.value);
        }

        public void SetPushNotification(bool targetState)
        {
            userOptionsPushNotification.value = targetState;
            MetaContextElementUtils.SetBooleanProperty(pushNotificationButtonElement, targetState);

            var animator = pushNotificationButtonElement.GetComponent<Animator>();
            animator.SetBool("Active", targetState);

            BagelCodeClientAPI.SystemOptionsUpdateRequest(targetState,
                                                           userOptionsKudoJackpot.value,
                                                           userOptionsKudoTournament.value,
                                                           userOptionsGlobalChat.value,
                                                           userOptionsKudoNewUserWelcome.value,
                                                           userOptionsPipMode.value,
            (response) =>
            {

            },
            (error) =>
            {

            });

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["action"] = targetState ? "on" : "off";
            customData["context_id"] = GetBIContextID();
            customData["device_push_setting"] = BlackboardQueryUtils.GetDevicePushSetting();

            Analytics.CustomEvent("client_pn_option", customData);
        }

        private string GetBIContextID()
        {
            if (string.IsNullOrEmpty(biContextID))
                biContextID = BiEventUtils.GenerateContextID();
            return biContextID;
        }
    }
}
