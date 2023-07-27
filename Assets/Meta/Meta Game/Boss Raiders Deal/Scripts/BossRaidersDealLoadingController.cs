using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using NodeCanvas.Framework;

namespace BagelCode.BossRaiders.Deal
{
    public class BossRaidersDealLoadingController : MonoBehaviour
    {
        public float waitLeaveTime = 1.0f;

        private ContextElement rootElement;
        private Blackboard rootBB;
        private Animator rootAnimator;

        private ContextElement progressBarElement;
        private ContextElement progressBarIconElement;

        private string contextId;
        private string bundleName = "";
        private string sharedBundleName = "";
        private string characterBundleName = "";
        private string dealUuid = "";
        private int themeId = 0;
        private int dealSpinCount = 0;
        private long dealMultiplier = 0L;

        public void OnInit()
        {
            InitProperty();
            InitBlackboard();
        }

        private void InitProperty()
        {
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);
            rootBB = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            progressBarElement = ContextUtils.FindElement(rootElement, "Progress Bar", ContextSearchingType.ChildrenSearch);
            progressBarIconElement = ContextUtils.FindElement(rootElement, "Boss Raiders Deal Progress Bar Icon", ContextSearchingType.ChildrenSearch);
        }

        private void InitBlackboard()
        {
            // Blackboard bundles(List) Set
            themeId = BlackboardUtils.FindValue<int>(rootBB, "themeId");
            dealSpinCount = BlackboardUtils.FindVariable<int>(rootBB, "dealSpinCount")?.value ?? 0;
            dealMultiplier = BlackboardUtils.FindVariable<long>(rootBB, "dealMultiplier")?.value ?? 0L;
            dealUuid = BlackboardUtils.FindVariable<string>(rootBB, "dealUuid")?.value ?? "";

            BossRaidersUtils.GetBossRaidersDealBundles(themeId, true, ref bundleName, ref sharedBundleName, ref characterBundleName);

            Variable<List<string>> bundles = GetDealBundles();
            if (bundles != null)
            {
                bundles.value.Add(bundleName);
                bundles.value.Add(sharedBundleName);
                bundles.value.Add(characterBundleName);
            }

            contextId = rootBB.GetValue<string>("_biContextID");
        }

        public IEnumerator LeaveDealCoroutine()
        {
            SetAnimationBool("MetaGameLogo", false);
            SetAnimationBool("Active", true);

            yield return new WaitForSeconds(waitLeaveTime);
        }

        public IEnumerator LoadAssetBundleCoroutine()
        {
            SetAnimationBool("Active", true);
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_start");

            bool isSuccess = false;
            bool isFail = false;

            yield return StartCoroutine(MetaAssetBundleUtils.UpdateAssetBundleLoadingProgressCoroutine(
                GetDealBundles().value, 1.7f, contextId,
                rootElement, progressBarElement, progressBarIconElement,
                () => isSuccess = true,
                () => isFail = true
                ));

            if (isSuccess)
            {
                // Send BI
                MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_end");
                // Enter Request
                bool requestSuccess = false;
                bool requestFail = false;
                BagelCodeClientAPI.RequestBossRaidersDealEnter(dealUuid,
                    (response) =>
                    {
                        ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), BossRaidersUtils.BOSS_RAIDERS_DEAL_INFO), response);

                        requestSuccess = true;
                    },
                    (error) =>
                    {
                        switch (error.errorCode)
                        {
                            case ClientModels.Error.BOSS_RAIDERS_DEAL_UUID_MISMATCH_ERROR:
                            case ClientModels.Error.BOSS_RAIDERS_DEAL_NOT_EXIST_ERROR:
                                GlobalErrorHandler.GlobalError(error);
                                break;
                            default:
                                GlobalErrorHandler.GlobalError(error);
                                break;
                        }

                        requestFail = true;
                    });
                yield return new WaitUntil(() => requestSuccess || requestFail);

                if (requestSuccess)
                    EventSender.SendEvent(gameObject, "OnSuccess");
                else
                    EventSender.SendEvent(gameObject, "OnFailed");
            }
            else if (isFail)
                EventSender.SendEvent(gameObject, "OnFailed");
        }

        public IEnumerator UnloadAssetBundlesAndCloseCoroutine()
        {
            // Unload Asset
            Variable<List<string>> bundles = GetDealBundles();
            if (bundles != null)
            {
                for (int i = 0; i < bundles.value.Count; ++i)
                    AssetBundleManager.UnloadAssetBundle(bundles.value[i], false);

                var operation = Resources.UnloadUnusedAssets();
                yield return new WaitUntil(() => operation.isDone);
            }
            // Close
            CloseLoadingScene();
            yield return new WaitForSeconds(0.1f);
        }

        public IEnumerator EnterMainScene()
        {
            string mainSceneAssetName = "Boss Raiders Deal Scene";
            string sharedSoundAssetName = "Boss Raiders Contents Shared Sounds";
            bool completeLoad = false;
            SceneLoadOperation sceneOperation = null;
            if (!string.IsNullOrEmpty(bundleName))
                BossRaidersUtils.GetBossRaidersDealBundles(themeId, true, ref bundleName, ref sharedBundleName, ref characterBundleName);
            MetaPopupUtils.OpenPopupAsync(this, bundleName, mainSceneAssetName, MetaObjectUtils.MainCanvasAreaTransform, (scene) =>
                            {
                                sceneOperation = scene;
                                completeLoad = true;
                            });
            yield return new WaitUntil(() => completeLoad == true);
            GameObject mainScene = sceneOperation.GetScene();
            Blackboard mainSceneBB = mainScene.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "_biContextID", contextId);
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "caller", gameObject);
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "rootCaller", rootBB.GetValue<GameObject>("caller"));    // inbox Cell or Lobby?
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "themeId", themeId);
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "dealSpinCount", dealSpinCount);
            BlackboardUtils.SetOrCreateValue(mainSceneBB, "dealUuid", dealUuid);

            PlayerPrefs.SetString(BossRaidersUtils.NOT_FINISHED_BOSS_RAIDERS_DEAL, dealUuid);

            MetaObjectUtils.MakePrefab(sharedBundleName, sharedSoundAssetName, mainScene.transform);
            mainScene.SetActive(true);

            yield return new WaitForSeconds(0.1f);
            GSManager.Instance.GetHandler(BossRaidersUtils.Sounds.BOSS_RAIDERS_START).Play();

            // BI deal enter
            BIClientBossRaidersEnter();
            var snapshot = GSManager.Instance.GetAudioMixerSnapshot("Content_Main");
            snapshot.TransitionTo(0.0f);
        }

        public void CloseLoadingScene()
        {
            EventSender.SendCalleeCallback(gameObject);
            MetaPopupUtils.ClosePopup(gameObject);
        }

        private void SetAnimationBool(string key, bool isActive)
        {
            rootAnimator?.SetBool(key, isActive);
        }

        private Variable<List<string>> GetDealBundles()
        {
            var bundles = BlackboardUtils.FindVariable<List<string>>(rootBB, "bundles");
            if (bundles != null && bundles.value == null)
                bundles.value = new List<string>();

            return bundles;
        }

        private void BIClientBossRaidersEnter()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextId;
            customData["type"] = "inbox";
            customData["energy"] = null;
            customData["theme_id"] = themeId;
            customData["boss_raiders_type"] = "boss_raiders_deal";

            Analytics.CustomEvent("client_boss_raiders_enter", customData);
        }
    }
}