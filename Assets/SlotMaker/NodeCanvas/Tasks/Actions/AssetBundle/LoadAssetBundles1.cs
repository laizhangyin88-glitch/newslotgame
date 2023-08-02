using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/AssetBundle")]
    public class LoadAssetBundles1 : ActionTask
    {
        public BBParameter<List<string>> bundleList;
        public bool forceDLC = false;

        public BBParameter<float> progress;

        private List<AssetBundleLoadOperation> loadOperations;

        protected override void OnExecute()
        {
            loadOperations = new List<AssetBundleLoadOperation>();
            for (int i = 0; i < bundleList.value.Count; ++i)
            {
                if (forceDLC) AssetBundleManager.AddDLC(bundleList.value[i]);

                loadOperations.AddRange( AssetBundleManager.LoadDependencies(bundleList.value[i]) );
                loadOperations.Add( AssetBundleManager.LoadAssetBundle(bundleList.value[i]) );
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

            progress.value = progressValue;
            if (allDone) progress.value = 1f;

            if(allDone)
                EndAction();
        }
    }
}
