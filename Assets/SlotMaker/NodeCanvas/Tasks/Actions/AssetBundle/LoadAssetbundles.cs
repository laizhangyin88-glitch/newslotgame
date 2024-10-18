using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/AssetBundle")]
    public class LoadAssetBundles : ActionTask
    {
        public BBParameter<List<string>> bundleList;
        public bool forceDLC = false;

        public BBParameter<string> progressKey;
        public BBParameter<object> progress;

        private List<AssetBundleLoadOperation> loadOperations;

        protected override void OnExecute()
        {
            loadOperations = new List<AssetBundleLoadOperation>();
            List<string> strings = ApplicationSettings.GetMachineStreamingAssets();
            for (int i = 0; i < bundleList.value.Count; ++i)
            {
                if (forceDLC)
                {
                    if (ApplicationSettings.Instance.isMachine)
                    {
                        if (!strings.Contains(bundleList.value[i]))
                            AssetBundleManager.AddDLC(bundleList.value[i]);
                    }
                    else
                        AssetBundleManager.AddDLC(bundleList.value[i]);
                }
                loadOperations.AddRange(AssetBundleManager.LoadDependencies(bundleList.value[i]));
                loadOperations.Add(AssetBundleManager.LoadAssetBundle(bundleList.value[i]));
            }
        }

        protected override void OnUpdate()
        {
            bool allDone = true;
            float progressValue = 0f;
            for (int i = 0; i < loadOperations.Count; ++i)
            {
                progressValue += loadOperations[i].Progress();
                if (!loadOperations[i].IsDone())
                    allDone = false;
                else
                {
                    string error = loadOperations[i].GetError();
                    if (!string.IsNullOrEmpty(error))
                    {
                        EndAction(false);
                        return;
                    }
                }
            }

            UpdateProgress(allDone, progressValue);

            if (allDone)
                EndAction();
        }

        private void UpdateProgress(bool allDone, float progressValue)
        {
            if (progress != null && progress.value != null
                && progressKey != null && !string.IsNullOrEmpty(progressKey.value))
            {
                WeightProgress wp = progress.value as WeightProgress;

                if (allDone)
                {
                    wp.UpdateProgress(progressKey.value, 1f);
                }
                else
                {
                    progressValue /= loadOperations.Count;
                    wp.UpdateProgress(progressKey.value, progressValue);
                }
            }
        }
    }

}
