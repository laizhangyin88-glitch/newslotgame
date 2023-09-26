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
            if (bundleList.value[0] == "fishing")
            {
                bundleList.value.Add("fishingaudio");
                bundleList.value.Add("fishingpanel");
                bundleList.value.Add("fishingeffect");
                bundleList.value.Add("fishinggold");
                bundleList.value.Add("fishinglighteffect");
                bundleList.value.Add("fishingnet");
                bundleList.value.Add("fishingouttips");
                bundleList.value.Add("fishingscore");
                bundleList.value.Add("fishingskill");
                bundleList.value.Add("fishingspecialdeclare");
                bundleList.value.Add("fishingtips");
                bundleList.value.Add("fishingbg");
                bundleList.value.Add("fishingplustips");
            }
            for (int i = 0; i < bundleList.value.Count; ++i)
            {
                if (forceDLC) AssetBundleManager.AddDLC(bundleList.value[i]);
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
