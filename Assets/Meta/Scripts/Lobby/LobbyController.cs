using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SlotMaker;
using ParadoxNotion.Services;
using ParadoxNotion;

namespace BagelCode
{
    public class LobbyController : EventMonoBehaviour
    {
        private Animator anim;

        private const string EVENT_ON_CLOSE_NAVIGATION_MENU = "OnCloseNavigationMenu";

        private static GameObject tempScene = null;

        private void Start()
        {
            anim = GetComponent<Animator>();

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_CUSTOMER_SUPPORT, OnOpenCustomerSupport);

            Register(EVENT_ON_CLOSE_NAVIGATION_MENU, OnCloseScene);
        }

        public void OnOpenCoupon()
        {
            EventSender.SendGlobalEvent("OnDisableMenu");

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Coupon Enter Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        public void OnOpenOnlinePlayers()
        {
            anim.SetBool("Active", false);

            Dictionary<string, object> customData = new Dictionary<string, object>();
            Analytics.CustomEvent("client_click_online_players", customData);

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Online Players Scene";
            Transform parent = GameObject.Find("Main Canvas/Area").transform;

            // lobby inactive되는 상황 방지
            MonoManager.current.StartCoroutine(
                SceneUtils.LoadSceneAsync(bundle, asset, parent, true,
                (result) => { result.SetActive(true); tempScene = result; }));
        }

        public void OnWallOfEpic()
        {
            anim.SetBool("Active", false);

            Analytics.CustomEvent("client_woe_enter", new Dictionary<string, object>
            {
            });

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Wall Of Epic Scene";
            Transform parent = GameObject.Find("Main Canvas/Area").transform;

            MonoManager.current.StartCoroutine(
                SceneUtils.LoadSceneAsync(bundle, asset, parent, true,
                (result) => { result.SetActive(true); tempScene = result; }));
        }

        public void OnEnterLeaderBoard()
        {
            anim.SetBool("Active", false);

            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = "lobby_slide_button";
            Analytics.CustomEvent("client_click_ranking", customData);

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Leaderboard Scene";
            Transform parent = GameObject.Find("Main Canvas/Area").transform;

            MonoManager.current.StartCoroutine(
                SceneUtils.LoadSceneAsync(bundle, asset, parent, true,
                (result) => { result.SetActive(true); tempScene = result; }));
        }

        public void OnEnterRefundDialog()
        {
            EventSender.SendGlobalEvent("OnDisableMenu");
            ErrorPopupInfo info = new ErrorPopupInfo();
            info.text = $"<size=32>Confirm Refund</size>";
            info.type = ErrorPopupType.YesNo;
            info.buttonText1 = "Confirm";
            info.buttonText2 = "Cancle";
            info.callback1 = delegate
            {
                SBoxSanboxController.Instance.PrintMoneyOrder();
            };
            info.callback2 = delegate
            {
                EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, new EventData(MetaEventDefine.ON_LEAVE_REFUND_DIALOG));
                ErrorPopupHandler.Instance.ClosePopup(MetaEventDefine.ON_LEAVE_REFUND_DIALOG);
            };
            ErrorPopupHandler.Instance.OpenError(info);
        }

        public void OnEnterJackpotRecord()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "New Jackpot Record Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent, useAssetName: false);
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);
            MetaPopupUtils.OpenPopup(popupObj);
        }

        private void OnOpenCustomerSupport()
        {
            AEUtils.SendAE("client_click_customer_support", ("type", "default"));

            string supportPageUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "/values/misc/SUPPORT_PAGE_URL").value;

#if UNITY_WEBGL && !UNITY_EDITOR
            NativeHelper.Instance.OpenUrl(UrlBuildUtils.GetHelpCenterUrl(supportPageUrl));
#else
            Application.OpenURL(UrlBuildUtils.GetHelpCenterUrl(supportPageUrl));
#endif
        }

        private void OnCloseScene()
        {
            if (tempScene != null)
                GameObject.Destroy(tempScene);

            tempScene = null;
        }
    }
}
