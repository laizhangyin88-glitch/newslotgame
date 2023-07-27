using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using System.Collections;
using ParadoxNotion;

namespace BagelCode
{
    public class VIPLoungeController : EventMonoBehaviour
    {
        private Blackboard bb;

        private Coroutine checkWelcomePopupCoroutine = null;

        private void Start()
        {
            bb = GetComponent<Blackboard>();

            StartCoroutine(BadgeExpireCoroutine());

            RegisterHandleEventType(MetaEventDefine.ON_META_UI_EVENT);

            Register(MetaEventDefine.ON_META_UI_EVENT, VipLounge.VipLounge.Events.CHECK_VIP_LOUNGE_OPEN, CheckWelcomePopup);
        }

        public void SendOnUpdateVipLoungeinfo()
        {
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, VipLounge.VipLounge.Events.ON_UPDATE_VIP_LOUNGE_INFO);
        }

        private void CheckWelcomePopup(EventData eventData)
        {
            if (checkWelcomePopupCoroutine != null)
                StopCoroutine(checkWelcomePopupCoroutine);

            GameObject caller = null;
            if (eventData != null && eventData.value is GameObject go)
                caller = go;

            bool isOpen = BlackboardQueryUtils.IsVipLoungeBadgeEarned();
            if (isOpen)
            {
                checkWelcomePopupCoroutine = StartCoroutine(CheckWelcomePopupCoroutine(caller));
            }
            else if (caller != null)
            {
                EventSender.SendEvent(caller, VipLounge.VipLounge.Events.ON_CLOSE_WELCOME_POPUP);
            }
        }

        public IEnumerator CheckWelcomePopupCoroutine(GameObject caller)
        {
            string bundle = VipLounge.VipLounge.Defines.COMMON_BUNDLE;
#if USE_ASSETBUNDLE
            bool isBundleLoaded = AssetBundleManager.GetLoadedAssetBundle(bundle) != null;
            if (!isBundleLoaded)
            {
                bool isSuccess = false;
                bool isFail = false;

                yield return StartCoroutine(MetaAssetBundleUtils.LoadAssetBundleCoroutine(
                    bundle, true, null, () => isSuccess = true, () => isFail = true));

                if (!isSuccess)
                {
                    Debug.LogError("VIPLoungeController.CheckWelcomePopupCoroutine failure. Common bundle loading failure.");
                    if (caller != null)
                        EventSender.SendEvent(caller, VipLounge.VipLounge.Events.ON_CLOSE_WELCOME_POPUP);
                    yield break;
                }
            }
#endif

            string asset = "Popup VIP Lounge Welcome Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;
            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent, "");

            MetaObjectUtils.SetCalleeCaller(popupObj, gameObject);

            MetaPopupUtils.OpenPopup(popupObj);

            if (caller != null)
            {
                var callbackTrigger = new EventTrigger(this, EventSender.ON_CALLEE_CALLBACK);
                yield return new WaitUntilTrigger(callbackTrigger);

                if (caller != null)
                    EventSender.SendEvent(caller, VipLounge.VipLounge.Events.ON_CLOSE_WELCOME_POPUP);
            }
        }

        private IEnumerator BadgeExpireCoroutine()
        {
            long remaining = VipLounge.VipLounge.Utils.VipLoungeInfo.GetVariable<long>("benefitEndTimestamp")?.value ?? 0L;
            yield return new WaitForSeconds(remaining / 1000f);

            BlackboardQueryUtils.SetBadgeCount(0);
        }

        public void CreateLoungeRewardPopup(Transform to)
        {
            GameObject currentPopup = BlackboardUtils.GetOrCreateVariable<GameObject>("popupObj")?.value ?? null;
            if (currentPopup != null)
                CloseLoungeRewardPopup(currentPopup);

            MetaPopupUtils.OpenPopupAsync(this, MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup VIP Epic Lounge Reward Scene", MetaPopupUtils.PopupManagerAreaTransform,
                (s) => {
                    GameObject popupObj = s.GetScene();
                    Blackboard popupBB = popupObj.GetComponent<Blackboard>();

                    BlackboardUtils.SetOrCreateValue(popupBB, "caller", gameObject);
                    BlackboardUtils.SetOrCreateValue(popupBB, "endTransform", to);

                    popupObj.SetActive(true);
                    PopupManager.Instance.Open(popupObj);

                    BlackboardUtils.SetOrCreateValue(bb, "popupObj", popupObj);
                });
        }

        public void CloseLoungeRewardPopup(GameObject popupObj)
        {
            if (popupObj == null)
                return;
            PopupManager.Instance.Close(popupObj);
            Destroy(popupObj);
        }

        [Button]
        private void TestRewardPopup(Transform targetTransform)
        {
            Transform t = targetTransform != null ? targetTransform : GameObject.Find("Main Canvas/Area/Lobby/Anchor/Lobby Bottom/Layout/Center/Button Time Bonus Area/Button Time Bonus")?.transform ?? null;
            string eventName = "OnVIPLoungeReward";
            EventSender.SendGlobalEvent(ParadoxNotion.Services.MessageRouter.ON_CUSTOM_EVENT, new ParadoxNotion.EventData<Transform>(eventName, t));
        }
    }
}
