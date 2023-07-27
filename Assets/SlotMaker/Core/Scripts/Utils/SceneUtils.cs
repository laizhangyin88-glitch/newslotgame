using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class SceneUtils : MonoBehaviour
    {
        public string bundleName;
        public bool combineApplicationType;
        public Transform parent;
        public string parentName;
        public bool isPopup;
        public bool isSingleThread;

        private bool isRunning;

        public void SendSceneEvent(string assetName)
        {
            if (isSingleThread && isRunning) return;

            isRunning = true;

            MonoManager.current.StartCoroutine(LoadSceneAsync(
                GetBundleName(), assetName, GetParent(), false, 
                (result) =>
                {
                    if (isPopup)
                    {
                        PopupManager.Instance.Open(result);
                        isRunning = false;
                    }
                }
            ));
        }
        
        protected string GetBundleName()
    	{
    		return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
    	}
        
        protected Transform GetParent()
        {
            var _parent = parent;
            if (!string.IsNullOrEmpty(parentName))
            {
                if (_parent == null)
                    _parent = GameObject.Find(parentName).transform;
                else 
                    _parent = _parent.Find(parentName);
            }
            return _parent;
        }
        
        public static IEnumerator LoadSceneAsync(string bundleName, string assetName, Transform parent, bool constraintSceneActivation, Action<GameObject> successCallback)
        {
            var loadSceneInfoOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(bundleName, assetName);
            while (!loadSceneInfoOperation.IsDone())
                yield return null;
            
            var sceneInfo = loadSceneInfoOperation.GetAsset<SceneInfoObject>().GetSceneInfo();
            var sceneLoadOperation = SceneManager.LoadSceneAsync(parent, sceneInfo, constraintSceneActivation);
            while (!sceneLoadOperation.IsDone())
                yield return null;
            
            successCallback(sceneLoadOperation.GetScene());
        }
    }
}
