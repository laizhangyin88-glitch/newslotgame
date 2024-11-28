using System.Security.AccessControl;
using System.Collections;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using SboxSpace;
using System;

namespace BagelCode
{
    public class MainController : EventMonoSingleton<MainController>
    {
        private bool loadMachine;
        private const StringTable.StringTableType GLOBAL = StringTable.StringTableType.Global;

#if DEV
        [Sirenix.OdinInspector.Button]
        private void InvokeGlobalEvent(string eventType, string eventName)
        {
            EventSender.SendGlobalEvent(eventType, eventName);
        }
#endif

        private void Start()
        {
            RegisterHandleEventType(MetaEventDefine.ON_CONTENT_UI_EVENT);
            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);
            Register(MetaEventDefine.ON_CONTENT_UI_EVENT, "ShowUI", ShowUI);
            Register(MetaEventDefine.ON_CONTENT_UI_EVENT, "HideUI", HideUI);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_ENTER_META_GAME, OnEnterMetaGame);
            Register(MetaEventDefine.ON_META_UI_EVENT, MetaEventDefine.ON_LEAVE_META_GAME, OnLeaveMetaGame);
            if (ApplicationSettings.Instance.isMachine)
                SBoxInit.Instance.Init("192.168.3.70", OnHardCheck);
        }

        private void OnHardCheck()
        {
            SBoxSanboxController.Instance.ToString();
            loadMachine = true;
        }

        private bool IsMachineLoadComplete()
        {
            return loadMachine;
        }

        private void HideUI()
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "hideUI", true);
        }

        private void ShowUI()
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "hideUI", false);
        }

        private void OnEnterMetaGame()
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "isInMetaGame", true);
        }

        private void OnLeaveMetaGame()
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "isInMetaGame", false);
        }

        public IEnumerator PreloadMachine()
        {
            yield return new WaitUntil(() => IsMachineLoadComplete());
        }

        public IEnumerator MetaGameCompensation()
        {

            bool isHogDealCompensation = BlackboardUtils.GetOrCreateVariable<bool>("/metaGameCompensation/hogDeal")?.value ?? false;
            if (isHogDealCompensation)
            {
                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "metaGameCompensation/hogDeal", false);

                yield return StartCoroutine(OpenCommonCompensationPopupCoroutine(
                    "POPUP_HOG_DEAL_COMPENSATION_TITLE",
                    "POPUP_HOG_DEAL_COMPENSATION_CONTENT"
                ));
            }

            // VIP Lounge Compensation
            if (BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "loungeJackpotCompensation")?.value ?? false)
            {
                BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "loungeJackpotCompensation", false);

                yield return StartCoroutine(OpenCommonCompensationPopupCoroutine(
                    "POPUP_COMPENSATION_TITLE",
                    "POPUP_COMPENSATION_LOUNGE_JACKPOT_TEXT"
                ));
            }

            // Vip Deal V2
            if (VipDealV2.Utils.GetVipFreeDealCompensationState())
            {
                VipDealV2.Utils.SetVipFreeDealCompensationState(false);

                yield return StartCoroutine(OpenCommonCompensationPopupCoroutine(
                    "VIP_DEAL_V2_COMPENSATION_POPUP_TITLE",
                    "VIP_DEAL_V2_COMPENSATION_POPUP_CONTENT"
                ));
            }
        }

        private IEnumerator OpenCommonCompensationPopupCoroutine(string titleKey, string textKey)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Compensation Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject compensationObj = null;
            yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (GameObject popupObj) => compensationObj = popupObj));

            InitCompensationPopup(compensationObj,
                                  StringTableUtils.GetString(GLOBAL, titleKey),
                                  StringTableUtils.GetString(GLOBAL, textKey));

            MetaPopupUtils.OpenPopup(compensationObj);

            var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
            yield return new WaitUntilTrigger(callbackTrigger);
        }

        public IEnumerator MetaGameRewardPopup()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            // Vegas Dreams
            if (BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "gurusFinalReward")?.value ?? null != null)
            {
                GameObject compensationObj = null;
                yield return StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, "Popup Vegas Dreams Season End Scene", parent,
                    (GameObject popupObj) => compensationObj = popupObj));

                Blackboard compensationBB = compensationObj.GetComponent<Blackboard>();
                MetaObjectUtils.SetCalleeCaller(compensationObj, gameObject);
                MetaPopupUtils.OpenPopup(compensationObj);

                var callbackTrigger = new EventTrigger(gameObject, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);
            }
        }

        private void InitCompensationPopup(GameObject popupObj, string title, string content)
        {
            if (popupObj == null)
                return;

            Blackboard compensationBB = popupObj.GetComponent<Blackboard>();
            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            BlackboardUtils.SetOrCreateValue(compensationBB, "_title", title);
            BlackboardUtils.SetOrCreateValue(compensationBB, "_content", content);
        }

        #region Vip Deal

        public IEnumerator MakeVipDealEnterLoadingSceneCoroutine()
        {
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "isSimpleMenuButtons", true);

            bool isActiveV1 = BlackboardQueryUtils.GetActiveVipDealInfo();
            bool isActiveV2 = VipDealV2.Utils.GetActiveInfo();

            string asset = "";
            bool isShop = false;
            bool isVipDealV2 = false;
            bool isPayDeal = false;
            if (isActiveV1)
            {
                asset = "Loading VIP Deal Scene";
                isShop = BlackboardUtils.FindValue<bool>("/vipDealInfo/isViewed");
                isVipDealV2 = false;
            }
            else if (isActiveV2)
            {
                isVipDealV2 = true;

                bool isViewedFree = BlackboardUtils.FindValue<bool>("/vipDealInfoV2/isViewedFree");
                if (!isViewedFree)
                {
                    asset = "Loading VIP Free Deal Scene";
                    isShop = false;
                    isPayDeal = false;
                }
                else
                {
                    asset = "Loading VIP Pay Deal Scene";
                    isShop = BlackboardUtils.FindValue<bool>("/vipDealInfoV2/isViewedPaid") == true;
                    isPayDeal = true;
                }
            }
            else yield break;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "isEnter", true);
            BlackboardUtils.SetOrCreateValue(popupBB, "isVipDealShop", isShop);
            BlackboardUtils.SetOrCreateValue(popupBB, "isVipDealV2", isVipDealV2);
            BlackboardUtils.SetOrCreateValue(popupBB, "isPayDeal", isPayDeal);
            MetaPopupUtils.OpenPopup(popupObj);

            var eventTrigger = new EventTrigger(gameObject, "StartLoadScene");
            yield return new WaitUntilTrigger(eventTrigger);

            EventSender.SendGlobalEvent("OnLeavePrevScene");
            yield return new WaitForSeconds(0.1f);
        }

        public IEnumerator MakeVipDealLeaveLoadingSceneCoroutine()
        {
            bool enableV1 = BlackboardUtils.FindVariable<bool>("/values/misc/ENABLE_VIP_DEAL")?.value ?? false;
            bool enableV2 = VipDealV2.Utils.IsEnabled();

            string asset = "";
            bool isShop = false;
            bool isVipDealV2 = false;
            bool isPayDeal = false;
            if (enableV1)
            {
                asset = "Loading VIP Deal Scene";
                isVipDealV2 = false;
            }
            else if (enableV2)
            {
                isVipDealV2 = true;

                bool isViewedFree = BlackboardUtils.FindValue<bool>("/vipDealInfoV2/isViewedFree");
                var isLeavedFreeDealVar = BlackboardUtils.GetOrCreateVariable<bool>("/vipDealInfoV2/isLeaveFreeDeal");
                if (!isViewedFree || isLeavedFreeDealVar.value)
                {
                    isLeavedFreeDealVar.value = false;
                    asset = "Loading VIP Free Deal Scene";
                    isPayDeal = false;
                }
                else
                {
                    asset = "Loading VIP Pay Deal Scene";
                    isPayDeal = true;
                }
            }
            else yield break;

            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            var popupBB = popupObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "isEnter", false);
            BlackboardUtils.SetOrCreateValue(popupBB, "isVipDealShop", isShop);
            BlackboardUtils.SetOrCreateValue(popupBB, "isVipDealV2", isVipDealV2);
            BlackboardUtils.SetOrCreateValue(popupBB, "isPayDeal", isPayDeal);
            MetaPopupUtils.OpenPopup(popupObj);

            var eventTrigger = new EventTrigger(gameObject, "StartLoadScene");
            yield return new WaitUntilTrigger(eventTrigger);

            EventSender.SendGlobalEvent("OnLeavePrevScene");
            yield return new WaitForSeconds(0.1f);
        }

        #endregion
    }
}
