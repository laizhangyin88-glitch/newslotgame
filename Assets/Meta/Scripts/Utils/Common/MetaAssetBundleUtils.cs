using NodeCanvas.Framework;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BagelCode
{
    public static class MetaAssetBundleUtils
    {
        public static IEnumerator LoadAssetBundleCoroutine(string bundle, bool forceDlc = true,
            System.Action<float> onUpdate = null, System.Action onSuccess = null, System.Action onFail = null)
        {
            return LoadAssetBundleCoroutine(new List<string>() { bundle },
                forceDlc, onUpdate, onSuccess, onFail);
        }

        public static IEnumerator LoadAssetBundleCoroutine(List<string> bundles,
            bool forceDlc = true,
            System.Action<float> onUpdate = null,
            System.Action onSuccess = null,
            System.Action onFail = null)
        {
            // make load operations
            var loadOperationList = new List<AssetBundleLoadOperation>();
            for (int i = 0; i < bundles.Count; ++i)
            {
                string bundle = bundles[i];

                if (forceDlc) AssetBundleManager.AddDLC(bundle);
                loadOperationList.AddRange(AssetBundleManager.LoadDependencies(bundle));
                loadOperationList.Add(AssetBundleManager.LoadAssetBundle(bundle));
            }

            bool allDone = false;
            bool error = false;
            string errorString = "";

            bool useProgress = onUpdate != null;
            float progressValue = 0f;

            // wait bundles loaded
            do
            {
                yield return new WaitForEndOfFrame();

                allDone = loadOperationList.All(l => l.IsDone());

                errorString = loadOperationList.FirstOrDefault(l => !string.IsNullOrEmpty(l.GetError()))?.GetError();
                error = !string.IsNullOrEmpty(errorString);

                if (useProgress)
                {
                    progressValue = loadOperationList.Sum(l => l.Progress());
                    onUpdate?.Invoke(progressValue / loadOperationList.Count);
                }

            // Until (allDone || error)
            } while (!allDone && !error);

            if (error)
            {
                Debug.LogError("error: " + errorString);
                Debug.LogError(string.Format("Bundle loading failure: {0}", string.Join(", ", bundles)));
                onFail?.Invoke();
                yield break;
            }

            if (ApplicationSettings.LogTest())
                Debug.Log(string.Format("Bundle loading complete: {0}", string.Join(", ", bundles)));

            onSuccess?.Invoke();
        }

        public static IEnumerator LoadAssetBundleCoroutine(List<string> bundles,
            WeightProgress weightProgress, string progressKey = "bundle", bool forceDlc = true,
            System.Action onSuccess = null, System.Action onFail = null)
        {
            var loadOperations = new List<AssetBundleLoadOperation>();
            for (int i = 0; i < bundles.Count; ++i)
            {
                if (forceDlc) AssetBundleManager.AddDLC(bundles[i]);

                loadOperations.AddRange(AssetBundleManager.LoadDependencies(bundles[i]));
                loadOperations.Add(AssetBundleManager.LoadAssetBundle(bundles[i]));
            }

            bool allDone;
            while (true)
            {
                allDone = true;

                float progressValue = 0f;
                for (int i = 0; i < loadOperations.Count; ++i)
                {
                    progressValue += loadOperations[i].Progress();
                    string error = loadOperations[i].GetError();
                    if (!loadOperations[i].IsDone())
                    {
                        allDone = false;
                    }
                    else if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError("error: " + error);
                        Debug.LogError(string.Format("Bundle loading failure: {0}", string.Join(", ", bundles)));

                        onFail?.Invoke();
                        yield break;
                    }
                }

                // Update Progress
                if (weightProgress != null && !string.IsNullOrEmpty(progressKey))
                {
                    if (allDone)
                    {
                        weightProgress.UpdateProgress(progressKey, 1f);
                    }
                    else
                    {
                        progressValue /= loadOperations.Count;
                        weightProgress.UpdateProgress(progressKey, progressValue);
                    }
                }

                if (allDone) break;

                yield return new WaitForEndOfFrame();
            }

            if (ApplicationSettings.LogTest())
                Debug.Log(string.Format("Bundle loading complete: {0}", string.Join(", ", bundles)));

            onSuccess?.Invoke();
        }

        public static IEnumerator UpdateAssetBundleLoadingProgressCoroutine(
            List<string> bundles, float loadingSpeed, string contextId,
            ContextElement root, ContextElement progressBarElement, ContextElement progressBarIconElement,
            System.Action onSuccess, System.Action onFail)
        {
            // Make Weight Progress
            var weightProgress = BlackboardQueryUtils.GetWeightProgress();
            weightProgress.Clear();

            weightProgress.AddProgress("bundle", 6f);
            weightProgress.AddProgress("info", 6f);

            progressBarElement.GetComponent<Slider>().handleRect = progressBarIconElement.GetComponent<RectTransform>();
            var progressProperty = progressBarElement as IContextFloatProperty;

            weightProgress.UpdateProgress("bundle", 0.7f);

            // Update Progress Bar
            var cancel = new Variable<bool>();
            cancel.value = false;
            root.StartCoroutine(UpdateProgressCoroutine(root, weightProgress, loadingSpeed, progressProperty, cancel));

            // Retry Enabled?
            bool isRetryDownloadEnabled = BlackboardUtils.FindValue<bool>("/values/misc/CLIENT_ASSET_DOWNLOAD_RETRY_ENABLED");

            // Load Asset Bundle
            bool isSuccess = false;
            bool isFail = false;
            root.StartCoroutine(LoadAssetBundleCoroutine(bundles, weightProgress, "bundle", true,
                () => isSuccess = true,
                () => isFail = true));

            while (true)
            {
                if (isSuccess)
                {
                    if (ApplicationSettings.LogTest())
                    {
                        Debug.Log("Assets loading complete.");
                    }

                    weightProgress.UpdateProgress("bundle", 1f);

                    weightProgress.UpdateProgress("info", 1f);

                    yield return new WaitForSeconds(1f);

                    onSuccess?.Invoke();
                    yield break;
                }
                else if (isFail)
                {
                    if (isRetryDownloadEnabled)
                    {
                        if (ApplicationSettings.LogTest())
                        {
                            Debug.Log("Assets loading failure. Try download again.");
                        }

                        isFail = false;

                        // Make Retry Popup
                        yield return root.StartCoroutine(MakeRetryPopupCoroutine(root));

                        var retryTrigger = new EventTrigger(root.gameObject, "RetryLoadBundles");
                        yield return new WaitUntilTrigger(retryTrigger);

                        //// Send BI todo shk
                        //SendMetaGameLoadingBIEvent(contextId, "step");

                        root.StartCoroutine(LoadAssetBundleCoroutine(bundles, weightProgress, "bundle", true,
                            () => isSuccess = true,
                            () => isFail = true));
                    }
                    else
                    {
                        if (ApplicationSettings.LogTest())
                        {
                            Debug.Log("Assets loading failure. Back to lobby.");
                        }

                        onFail?.Invoke();
                        yield break;
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private static IEnumerator UpdateProgressCoroutine(ContextElement root, WeightProgress weightProgress,
            float loadingSpeed, IContextFloatProperty progressProperty, Variable<bool> cancel)
        {
            float progress = 0f;
            float barProgress = 0f;
            while (progress < 1f)
            {
                progress = weightProgress.GetTotalProgress();

                float time = 1f / loadingSpeed;
                float delta = progress - barProgress;
                if (delta > 0)
                {
                    // Update Progress Bar
                    float startBarProgress = barProgress;
                    yield return root.StartCoroutine(AsyncActionUtils.ProgressiveActionCoroutine(time, null,
                        (float t) =>
                        {
                            barProgress = startBarProgress + t * delta;
                            progressProperty.SetFloatProperty(barProgress);
                        }, null));
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }

                if (cancel.value)
                {
                    yield break;
                }
            }
        }

        public static IEnumerator MakeRetryPopupCoroutine(MonoBehaviour agent)
        {
            string bundle = ApplicationSettings.MakeApplicationBundleName("system");
            string asset = "Popup Common Ok Scene";
            Transform parent = MetaPopupUtils.PopupManagerAreaTransform;

            GameObject popupObj = null;
            yield return agent.StartCoroutine(MetaPopupUtils.OpenPopupCoroutine(bundle, asset, parent,
                (SceneLoadOperation sceneLoadOperation) => popupObj = sceneLoadOperation.GetScene()));

            MetaObjectUtils.SetCalleeCaller(popupObj, agent.gameObject);

            string errorText = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_RETRY");

            MetaPopupUtils.SetCommonPopupData(popupObj, agent.transform, errorText, "", "RetryLoadBundles", "OK",
                "", "", "", "", true, true, true, false, false);

            MetaPopupUtils.OpenPopup(popupObj);
        }

        public static void SendMetaGameLoadingBIEvent(string contextId, string step)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["type"] = "meta_game_content";
            customData["step"] = step;
            customData["context_id"] = contextId;
            Analytics.CustomEvent("client_loading_funnel", customData);
        }
    }
}
