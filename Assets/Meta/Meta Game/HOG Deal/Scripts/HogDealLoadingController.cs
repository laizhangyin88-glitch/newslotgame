using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using UnityEngine.UI;
using ParadoxNotion;

namespace BagelCode
{
    public class HogDealLoadingController : MonoBehaviour
    {
        // Settings
        private const float LOADING_SPEED = 1.7f;
        //

        private ContextElement root;
        private Blackboard bb;
        private Animator anim;

        private string contextId;

        private const ContextSearchingType CHILDREN = ContextSearchingType.ChildrenSearch;

        public void InitProperty()
        {
            root = GetComponent<ContextElement>();
            bb = GetComponent<Blackboard>();
            anim = GetComponent<Animator>();

            root.UpdateContext(false);

            anim.SetBool("Active", true);
        }

        public IEnumerator LoadStageBundleCoroutine()
        {
            SetLogoTrigger("InGameLogo");

            contextId = bb.GetValue<string>("_biContextID");
            MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_start");

            string chapter = bb.GetValue<string>("targetSymbol");
            int stage = bb.GetValue<int>("targetStage");
            string stageBundle = HiddenObjects.HiddenObjects.Utils.GetStageBundleName(chapter, stage);

            // Enter Meta
            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, "OnEnterMetaGame");

            // Bundle Load Progress
            bool isSuccess = false;
            bool isFail = false;
            var bundleList = new List<string>()
            { HogDeal.Defines.CONTENTS_BUNDLE, stageBundle };
            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar UV", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "Hog Deal Progress Bar Icon", CHILDREN);
            yield return StartCoroutine(MetaAssetBundleUtils.UpdateAssetBundleLoadingProgressCoroutine(
                bundleList, LOADING_SPEED, contextId,
                root, progressBarElement, progressBarIconElement,
                () => isSuccess = true,
                () => isFail = true
                ));

            if (isSuccess)
            {
                // Send BI
                MetaAssetBundleUtils.SendMetaGameLoadingBIEvent(contextId, "content_download_end");

                EventSender.SendEvent(gameObject, "OnSuccess");
            }
            else if (isFail)
            {
                Debug.LogError("Hog Deal bundle loading failure: " + stageBundle);
                EventSender.SendEvent(gameObject, "OnFail");
            }
        }

        public IEnumerator MakeStageCoroutine()
        {
            string chapter = bb.GetValue<string>("targetSymbol");
            int stage = bb.GetValue<int>("targetStage");

            // Make Stage
            string bundle = HiddenObjects.HiddenObjects.Utils.GetStageBundleName(chapter, stage);
            string asset = HiddenObjects.HiddenObjects.Utils.GetStageAssetName(chapter, stage);

            if (string.IsNullOrEmpty(bundle) || string.IsNullOrEmpty(asset))
            {
                Debug.LogError(string.Format("HiddenObjectsLoadingController.MakeStageSceneCoroutine failure.\n" +
                    "Bundle loading failure. chapter: {0}, stage: {1}\n" +
                    "Check \"HiddenObjectsChapterData\" asset.", chapter, stage));

                Close();
                yield break;
            }

            Transform mainArea = MetaObjectUtils.MainCanvasAreaTransform;

            GameObject stageObj = null;
            StartCoroutine(MetaObjectUtils.MakePrefabCoroutine(bundle, asset, mainArea,
                (GameObject sceneObj) =>
                {
                    stageObj = sceneObj;
                    stageObj.SetActive(false);
                }));

            // Make In Game Scene
            bundle = HogDeal.Defines.CONTENTS_BUNDLE;
            asset = "Hog Deal In Game Scene";

            GameObject inGameObj = null;
            StartCoroutine(MetaObjectUtils.MakeSceneCoroutine(bundle, asset, mainArea,
                (GameObject sceneObj) => inGameObj = sceneObj));

            // Wait
            yield return new WaitUntil(() => stageObj != null && inGameObj != null);

            // Set Parents
            var inGameElement = inGameObj.GetComponent<ContextElement>();
            inGameElement.UpdateContext(true);
            var stageAreaElement = ContextUtils.FindElement(inGameElement, "Stage Area", CHILDREN);
            stageObj.transform.parent = stageAreaElement.transform;

            // Set Caller
            var caller = bb.GetValue<GameObject>("caller"); // inbox cell
            MetaObjectUtils.SetCalleeCaller(stageObj, caller);
            MetaObjectUtils.SetCalleeCaller(inGameObj, caller);

            var inGameBB = inGameObj.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(inGameBB, "remainingCount", bb.GetValue<int>("remainingCount"));

            EventSender.SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT,
                new EventData<GameObject>(MetaEventDefine.ON_MAKE_RESULT_POPUP, inGameObj));

            stageObj.SetActive(true);

            // Finish
            Close();
        }

        public IEnumerator UnloadStageBundleCoroutine()
        {
            SetLogoTrigger("InGameLogo");

            // Unload Bundles
            string chapter = bb.GetValue<string>("targetSymbol");
            int stage = bb.GetValue<int>("targetStage");
            string stageBundle = HiddenObjects.HiddenObjects.Utils.GetStageBundleName(chapter, stage);

            AssetBundleManager.UnloadAssetBundle(stageBundle, false);
            var operation = Resources.UnloadUnusedAssets();

            AssetBundleManager.UnloadAssetBundle(HogDeal.Defines.CONTENTS_BUNDLE, false);
            var operation2 = Resources.UnloadUnusedAssets();

            yield return new WaitUntil(() => operation.isDone && operation2.isDone);

            // Update Progress Bar
            var progressBarElement = ContextUtils.FindElement(root, "Progress Bar UV", CHILDREN);
            var progressBarIconElement = ContextUtils.FindElement(root, "Hog Deal Progress Bar Icon", CHILDREN);
            progressBarElement.GetComponent<Slider>().handleRect = progressBarIconElement.GetComponent<RectTransform>();
            var progressProperty = progressBarElement as IContextFloatProperty;

            float DURATION = 0.7f;
            yield return root.StartCoroutine(AsyncActionUtils.ProgressiveActionCoroutine(DURATION, null,
                (float t) => progressProperty.SetFloatProperty(t), null));

            yield return new WaitForSeconds(0.3f);
        }

        private void SetLogoTrigger(string key)
        {
            anim.SetBool("LobbyLogo", key == "LobbyLogo");
            anim.SetBool("InGameLogo", key == "InGameLogo");
        }

        public void Close()
        {
            EventSender.SendCalleeCallback(gameObject);
            MetaPopupUtils.ClosePopup(gameObject);
        }
    }
}
