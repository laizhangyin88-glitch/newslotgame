using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public static class MetaPopupUtils
    {
        private const string OWNER = "owner";
        private const string EVENT_BUTTON_YES = "eventButtonYes";
        private const string EVENT_BUTTON_NO = "eventButtonNo";
        private const string EVENT_BUTTON_X = "eventButtonX";
        private const string EVENT_BUTTON_BACK = "eventButtonBack";
        private const string TITLE = "title";
        private const string TEXT = "text";
        private const string BUTTON_YES_TEXT = "buttonYesText";
        private const string BUTTON_NO_TEXT = "buttonNoText";
        private const string AUTO_CLOSE_YES = "autoCloseYes";
        private const string AUTO_CLOSE_NO = "autoCloseNo";
        private const string AUTO_CLOSE_X = "autoCloseX";
        private const string USE_CLOSE_BUTTON = "useCloseButton";
        private const string USE_BACK_BUTTON = "useBackButton";

        private static Transform popupManagerAreaTransform;
        public static Transform PopupManagerAreaTransform
        {
            get
            {
                if (popupManagerAreaTransform is null)
                    popupManagerAreaTransform = GameObject.Find("Popup Manager/Area").transform;

                return popupManagerAreaTransform;
            }
        }

        private static Transform popupManagerInteractionTransform;
        public static Transform PopupManagerInteractionTransform
        {
            get
            {
                if (popupManagerInteractionTransform is null)
                    popupManagerInteractionTransform = GameObject.Find("Popup Manager/Interaction").transform;

                return popupManagerInteractionTransform;
            }
        }

        public static bool OpenDealIam(GameObject caller, InAppMessageTriggerType type, bool useCache)
        {
            int iamId = 0;
            if (useCache) // Check Deal IAM
            {
                var iamIdValue = PlayerPrefs.GetString("IAMDeal_ID", "0");
                if (iamIdValue != "0")
                {
                    int id = System.Convert.ToInt32(iamIdValue);

                    var iamBB = BlackboardQueryUtils.GetIAMBlackboard(id);

                    if (iamBB != null && IAMRouter.Instance.IsValidIAMasDeal(iamBB))
                    {
                        long currentTimestamp = TimeUtils.GetTimeStamp();
                        long endTimestamp = GetDealIamEndTimestamp(iamBB);

                        if (currentTimestamp < endTimestamp)
                        {
                            iamId = id;
                        }
                    }
                    else
                    {
                        PlayerPrefs.DeleteKey("IAMDeal_ID");
                    }
                }
            }

            bool triggered;
            if (iamId != 0)
            {
                triggered = IAMRouter.Instance.TriggerIAMByID(
                    InAppMessageTriggerType.UNKNOWN,
                    iamId,
                    caller,
                    "");
            }
            else
            {
                triggered = IAMRouter.Instance.TriggerIAM(
                    type,
                    caller,
                    "");
            }

            return triggered;
        }

        public static GameObject OpenShop(GameObject caller, string biContextID = null)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Shop Scene";
            Transform parent = PopupManagerAreaTransform;

            GameObject shopObj = MetaObjectUtils.MakeScene(bundle, asset, parent);

            shopObj.SetActive(false);

            var shopBB = shopObj.GetComponent<Blackboard>();
            if (!string.IsNullOrEmpty(biContextID))
                BlackboardUtils.SetOrCreateValue(shopBB, "_biContextID", biContextID);
            BlackboardUtils.SetOrCreateValue(shopBB, "coinShopType", ShopType.COIN);
            BlackboardUtils.SetOrCreateValue(shopBB, "gemShopType", ShopType.GEM);
            BlackboardUtils.SetOrCreateValue(shopBB, "_openTabIndex", 0);
            
            OpenPopup(shopObj);

            if (caller != null)
                MetaObjectUtils.SetCalleeCaller(shopObj, caller);

            return shopObj;
        }

        public static GameObject OpenYesNo(string content, System.Action yesAct)
        {
            var sceneInfoObj = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Common Yes No Scene");

            var copy = SceneManager.LoadScene(PopupManager.Instance.transform, sceneInfoObj.sceneInfo);
            var element = copy.GetComponent<ContextElement>();
            element.UpdateContext(true);
            var bb = copy.GetComponent<Blackboard>();
            bb.SetValue(OWNER, copy.transform);
            bb.SetValue(TEXT, content);
            bb.SetValue(BUTTON_YES_TEXT, "Yes");
            bb.SetValue(BUTTON_NO_TEXT, "No");
            bb.SetValue(AUTO_CLOSE_YES, true);
            bb.SetValue(AUTO_CLOSE_NO, true);

            ContextButton yesButton = element.FindElement<ContextButton>("Button Yes");
            yesButton.AddListenerOnClick(
                (context) =>
                {
                    yesAct?.Invoke();
                }
            );
            //ContextButton noButton = element.FindElement<ContextButton>("Button No");
            //noButton.AddListenerOnClick((context) => {
            //    noAct?.Invoke();
            //});

            PopupManager.Instance.Open(copy);
            return copy;
        }

        public static GameObject OpenOKPopup()
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            string asset = "Popup Common OK Scene";
            Transform parent = PopupManagerAreaTransform;

            var popupObj = MetaObjectUtils.MakeScene(bundle, asset, parent);
            OpenPopup(popupObj);

            return popupObj;
        }

        public static IEnumerator OpenOKPopupCoroutine(Transform parent, Action<GameObject> onOpenPopup)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;

            return OpenPopupCoroutine(bundle, "Popup Common Ok Scene", parent,
                (SceneLoadOperation scene) => onOpenPopup?.Invoke(scene.GetScene()));
        }

        public static Coroutine OpenLoadingPopupAsync(MonoBehaviour behaviour, Action<GameObject> onOpenLoadingPopup = null)
        {
            if (behaviour == null) return null;
            return behaviour.StartCoroutine(OpenLoadingPopupCoroutine(onOpenLoadingPopup));
        }

        public static IEnumerator OpenLoadingPopupCoroutine(Transform parent, Action<GameObject> onOpenLoadingPopup, bool openManually = false)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;

            return OpenPopupCoroutine(bundle, "Loading Full Scene", parent,
                (SceneLoadOperation sceneLoadOperation) =>
                {
                    if (!openManually)
                        OnLoadLoadingPopup(sceneLoadOperation);

                    var obj = sceneLoadOperation.GetScene();
                    onOpenLoadingPopup?.Invoke(obj);
                });
        }

        public static IEnumerator OpenLoadingPopupCoroutine(Action<GameObject> onOpenLoadingPopup = null)
        {
            var root = PopupManagerAreaTransform;
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;

            return OpenPopupCoroutine(bundle, "Loading Full Scene", root,
                (SceneLoadOperation sceneLoadOperation) =>
                {
                    var obj = OnLoadLoadingPopup(sceneLoadOperation);
                    onOpenLoadingPopup?.Invoke(obj);
                });
        }

        public static GameObject OpenLoadingPopup()
        {
            var root = PopupManagerAreaTransform;
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;

            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundle, "Loading Full Scene").GetSceneInfo();
            var sceneObj = SceneManager.LoadScene(root, sceneInfo);
            PopupManager.Instance.Open(sceneObj);
            sceneObj.SetActive(true);
            return sceneObj;
        }

        private static GameObject OnLoadLoadingPopup(SceneLoadOperation sceneLoadOperation)
        {
            if(sceneLoadOperation != null)
            {
                var sceneObj = sceneLoadOperation.GetScene();
                PopupManager.Instance.Open(sceneObj);
                sceneObj.SetActive(true);
                return sceneObj;
            }
            else
            {
                Debug.LogWarning("MetaPopupUtils.OnLoadLoadingPopup Failure: The sceneLoadOperation is null.");
                return null;
            }
        }

        public static void SetInformationPopupData(Blackboard infoBB, int pageCount, string bundleName, string dotPrefabFormat, string infoPrefabFormat, string infoTextFormat)
        {
            infoBB.AddVariable("pageCount", pageCount);
            infoBB.AddVariable("bundle", bundleName);
            infoBB.AddVariable("dotPrefabFormat", dotPrefabFormat);
            infoBB.AddVariable("infoPrefabFormat", infoPrefabFormat);
            infoBB.AddVariable("infoTextFormat", infoTextFormat);
        }

        public static void SetCommonOKPopupData(GameObject popup, Transform owner, string content, string buttonText)
        {
            SetCommonPopupData(popup, owner,
                content, "", "", buttonText,
                "", "", "", "", true, true, true, true, true);
        }

        // todo 현재는 owner에 graph 필요로 함. router 직접 참조하는 방식으로 수정.
        public static void SetCommonPopupData(GameObject popup, Transform owner, string title, string text,
            string eventButtonYes, string buttonYesText, string eventButtonNo, string buttonNoText,
            string eventButtonX, string eventButtonBack, bool autoCloseYes, bool autoCloseNo, bool autoCloseX,
            bool useCloseButton, bool useBackButton)
        {
            Blackboard bb = popup.GetComponent<Blackboard>();

            if(owner != null)
            {
                bb.AddVariable("owner", owner);
            }

            bb.AddVariable("title", title);
            bb.AddVariable("text", text);
            bb.AddVariable("eventButtonYes", eventButtonYes);
            bb.AddVariable("buttonYesText", buttonYesText);
            bb.AddVariable("eventButtonNo", eventButtonNo);
            bb.AddVariable("buttonNoText", buttonNoText);
            bb.AddVariable("eventButtonX", eventButtonX);
            bb.AddVariable("eventButtonBack", eventButtonBack);
            bb.AddVariable("autoCloseYes", autoCloseYes);
            bb.AddVariable("autoCloseNo", autoCloseNo);
            bb.AddVariable("autoCloseX", autoCloseX);
            bb.AddVariable("useCloseButton", useCloseButton);
            bb.AddVariable("useBackButton", useBackButton);
        }

        public static Coroutine OpenPopupAsync(MonoBehaviour agent, string bundleName, string assetName, Transform root, Action<SceneLoadOperation> onCompleteLoadPopup)
        {
            return agent.StartCoroutine(OpenPopupCoroutine(bundleName, assetName, root, onCompleteLoadPopup));
        }

        public static IEnumerator OpenPopupCoroutine(string bundleName, string assetName, Transform root, Action<SceneLoadOperation> actionCompleteLoadPopup = null, bool constraintSceneActivation = true)
        {
            AssetBundleLoadAssetOperation loadPopupInfoOperation = null;
            loadPopupInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(bundleName, assetName);
            yield return new WaitUntil(() => loadPopupInfoOperation.IsDone());

            var sceneInfoObj = loadPopupInfoOperation.GetAsset<SceneInfoObject>();
            if (sceneInfoObj == null)
            {
                Debug.LogError("Popup loading failure. bundle: " + bundleName + ", asset: " + assetName);
                yield break;
            }

            SceneInfo sceneInfo = sceneInfoObj.GetSceneInfo();
            SceneLoadOperation loadPopupOperation = null;
            loadPopupOperation = SceneManager.LoadSceneAsync(root, sceneInfo, constraintSceneActivation);
            yield return new WaitUntil(() => loadPopupOperation.IsDone());

            actionCompleteLoadPopup?.Invoke(loadPopupOperation);
        }

        public static IEnumerator OpenPopupCoroutine(string bundleName, string assetName, Transform root, Action<GameObject> actionCompleteLoadPopup = null, bool constraintSceneActivation = true)
        {
            AssetBundleLoadAssetOperation loadPopupInfoOperation = null;
            loadPopupInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(bundleName, assetName);
            yield return new WaitUntil(() => loadPopupInfoOperation.IsDone());

            var sceneInfoObj = loadPopupInfoOperation.GetAsset<SceneInfoObject>();
            if (sceneInfoObj == null)
            {
                Debug.LogError("Popup loading failure. bundle: " + bundleName + ", asset: " + assetName);
                yield break;
            }

            SceneInfo sceneInfo = sceneInfoObj.GetSceneInfo();
            SceneLoadOperation loadPopupOperation = null;
            loadPopupOperation = SceneManager.LoadSceneAsync(root, sceneInfo, constraintSceneActivation);
            yield return new WaitUntil(() => loadPopupOperation.IsDone());

            actionCompleteLoadPopup?.Invoke(loadPopupOperation.GetScene());
        }

        public static IEnumerator SimpleOpenPopupCoroutine(MonoBehaviour agent, string assetName, Action<SceneLoadOperation> actionCompleteLoadPopup = null)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            Transform parent = PopupManagerAreaTransform;

            yield return agent.StartCoroutine(OpenPopupCoroutine(bundle, assetName, parent, actionCompleteLoadPopup));
        }

        // bundle null = lobby0
        public static IEnumerator OpenCommonInformationPopupCoroutine(MonoBehaviour agent, int pageCount, string infoPrefabFormat, string infoTextFormat, string bundle = null, Action<GameObject> informationPopupObj = null)
        {
            string lobbyBundle = MetaStringDefine.LOBBY_BUNDLE_NAME;
            if (string.IsNullOrEmpty(bundle)) bundle = lobbyBundle;

            string asset = "Popup Information Scene";
            Transform parent = PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return agent.StartCoroutine(OpenPopupCoroutine(lobbyBundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            MetaObjectUtils.SetCalleeCaller(popupObj, agent.gameObject);

            string dotPrefabName = "Information Dots";

            var popupBB = popupObj.GetComponent<Blackboard>();
            SetInformationPopupData(popupBB, pageCount, bundle, dotPrefabName, infoPrefabFormat, infoTextFormat);

            informationPopupObj?.Invoke(popupObj);

            OpenPopup(popupObj);
        }

        public static void OpenPopup(GameObject popup)
        {
            if (popup == null) return;

            PopupManager.Instance.Open(popup);
            popup.SetActive(true);
        }

        public static void ClosePopup(GameObject popup)
        {
            if (popup == null) return;

            PopupManager.Instance.Close(popup);
            GameObject.Destroy(popup);
        }

        private static long GetDealIamEndTimestamp(Blackboard iamInfoBB)
        {
            long currentTimestamp = TimeUtils.GetTimeStamp();

            long endTimestamp = BlackboardUtils.FindVariable<long>(iamInfoBB, "endTimestamp").value;
            bool useUserTimer = BlackboardUtils.FindVariable<bool>(iamInfoBB, "useUserTimer").value;
            int userTimerMin = BlackboardUtils.FindVariable<int>(iamInfoBB, "userTimerMin").value;

            string id = BlackboardUtils.FindVariable<int>(iamInfoBB, "id").value.ToString();
            string iamKey = string.Format("IAMTimer:{0}", id);
            string userStartTimeText = PlayerPrefs.GetString(iamKey, "0");
            long userStartTimestamp = System.Convert.ToInt64(userStartTimeText);
            long userEndTimestamp = userStartTimestamp + (System.Convert.ToInt64(userTimerMin) * 60000L);

            if (useUserTimer && userTimerMin > 0)
            {
                if (endTimestamp == 0L)
                {
                    endTimestamp = userEndTimestamp;
                }
                else
                {
                    if (userEndTimestamp < endTimestamp)
                    {
                        endTimestamp = userEndTimestamp;
                    }
                }
            }

            if (endTimestamp == 0L || (endTimestamp > 0 && currentTimestamp < endTimestamp))
            {
                return endTimestamp;
            }
            else
            {
                return -1L;
            }
        }
    }
}
