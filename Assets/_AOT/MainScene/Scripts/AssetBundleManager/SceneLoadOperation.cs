using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace SlotMaker
{
    public abstract class SceneLoadOperation : IEnumerator
    {
    	public object Current { get { return null; } }
    	public bool MoveNext() { return !IsDone(); }
    	public void Reset() {}

    	public abstract bool Update(float deltaTime);
    	public abstract bool IsDone();
    	public abstract void Abort();
    	public abstract GameObject GetScene();

        public virtual void Print() {}
    }

    public class SceneLoad : SceneLoadOperation
    {
    	protected Transform root;
    	protected SceneInfo sceneInfo;
    	protected GameObject scene;
    	protected bool constraintSceneActivation;
        protected enum SceneLoadProgress
        {
            LoadAssets,
            InstantiateAssets,
            InstantiatingAssets,
            Done
        };
        protected SceneLoadProgress progress = SceneLoadProgress.LoadAssets;
        
        private PerformanceAnalyzer.TimeSample timeSample;
        public SceneLoad(Transform root, SceneInfo sceneInfo, bool constraintSceneActivation)
        {
            this.root = root;
            this.sceneInfo = sceneInfo;
    		this.constraintSceneActivation = constraintSceneActivation;

            timeSample = new PerformanceAnalyzer.TimeSample("scene", sceneInfo.assetName);
        }
        
        public override GameObject GetScene()
        {
            return scene;
        }
        
        public override  bool IsDone()
        {
            return progress == SceneLoadProgress.Done;
        }
        
        public override void Abort()
        {
        }
        
        public override bool Update(float deltaTime)
        {
            if (progress == SceneLoadProgress.LoadAssets)
            {
                timeSample.BeginSample();
                
                LoadAsset(sceneInfo);
                progress = SceneLoadProgress.InstantiateAssets;
            }
            
            if (progress == SceneLoadProgress.InstantiateAssets)
            {
                progress = SceneLoadProgress.InstantiatingAssets;
                SceneManager.Instance.StartCoroutine(InstantiateAsset(root, sceneInfo, true));
            }
            
            if (progress == SceneLoadProgress.Done)
            {
                timeSample.EndSample();
                return false;
            }
            
            return true;
        }
        
        protected void LoadAsset(SceneInfo sceneInfo)
        {
            AssetBundleManager.LoadAssetAsync(sceneInfo.bundleName, sceneInfo.assetName, typeof(GameObject));
            
            int count = sceneInfo.children.Count;
            for (int i = 0; i < count; ++i)
            {
                LoadAsset(sceneInfo.children[i]);
            }
        }
        
        protected IEnumerator InstantiateAsset(Transform root, SceneInfo sceneInfo, bool isRoot = false)
        {
            LoadedAsset asset = null;
            while (true)
            {
                asset = AssetBundleManager.GetLoadedAsset(sceneInfo.bundleName, sceneInfo.assetName);
                
                if (asset != null) break;

                yield return null;
            }

            Transform parent = root;
            if (!string.IsNullOrEmpty(sceneInfo.parentName))
            {
                if (parent == null)
                    parent = GameObject.Find(sceneInfo.parentName).transform;
                else 
                    parent = parent.Find(sceneInfo.parentName);
            }
            if (parent == null) yield break;
            
            var prefab = asset.asset as GameObject;
            var go = GameObject.Instantiate(prefab) as GameObject;
            go.name = sceneInfo.uniqueName;
            go.transform.SetParent(parent, false);
            go.transform.SetAsLastSibling();
            
            if (isRoot)
            {
                go.SetActive(false);
                scene = go;
            }
            else 
            {
                go.SetActive(sceneInfo.activate);
            }
            
            int count = sceneInfo.children.Count;
            for (int i = 0; i < count; ++i)
            {
                if (go == null) yield break;

                yield return SceneManager.Instance.StartCoroutine(InstantiateAsset(go.transform, sceneInfo.children[i]));
            }
            
            if (isRoot)
    		{
    			if (!constraintSceneActivation)
    				go.SetActive(true);
    				
    			progress = SceneLoadProgress.Done;
    		}
        }
    }
}
