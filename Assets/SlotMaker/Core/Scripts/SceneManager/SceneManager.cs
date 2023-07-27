using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker
{
    public class SceneManager : MonoWeakSingleton<SceneManager>
    {
        private static List<SceneLoadOperation> inProgresssOperations = new List<SceneLoadOperation>();

        public static void Abort()
        {
            foreach (var operation in inProgresssOperations)
            {
                operation.Abort();
            }
        }

        public static SceneLoadOperation LoadSceneAsync(Transform root, SceneInfo sceneInfo, bool constraintSceneActivation = false)
        {
            if (ApplicationSettings.LogScene())
                Debug.Log("[SceneManager] Loading " + sceneInfo.uniqueName + " scene");

            SceneLoad operation = new SceneLoad(root, sceneInfo, constraintSceneActivation);
            inProgresssOperations.Add(operation);
            return operation;
        }

        public static GameObject LoadScene(Transform root, SceneInfo sceneInfo)
        {
            if (ApplicationSettings.LogScene())
              Debug.Log("[SceneManager] Load " + sceneInfo.uniqueName + " scene");

            var prefab = (GameObject)AssetBundleManager.LoadAsset<GameObject>(sceneInfo.bundleName, sceneInfo.assetName);
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.name = sceneInfo.uniqueName;

            Transform parent = root;
            if (!string.IsNullOrEmpty(sceneInfo.parentName))
            {
                if (parent == null)
                    parent = GameObject.Find(sceneInfo.parentName).transform;
                else
                    parent = parent.Find(sceneInfo.parentName);
            }
            if (parent != null)
                go.transform.SetParent(parent, false);

            go.SetActive(sceneInfo.activate);

            if (sceneInfo.children != null)
            {
                for (int i = 0; i < sceneInfo.children.Count; ++i)
                {
                    LoadScene(go.transform, sceneInfo.children[i]);
                }
            }

            return go;
        }

#if UNITY_EDITOR
        public static void LoadSceneInEditor(Transform root, SceneInfo sceneInfo)
        {
            if (ApplicationSettings.LogScene())
                Debug.Log("[SceneManager] Load " + sceneInfo.uniqueName + " in Editor");

            var prefab = (GameObject)AssetBundleManager.LoadAsset<GameObject>(sceneInfo.bundleName, sceneInfo.assetName);
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.name = sceneInfo.uniqueName;

            if (!go.GetComponent<ManagedPrefab>())
                go.AddComponent<ManagedPrefab>();

            Transform parent = root;
            if (!string.IsNullOrEmpty(sceneInfo.parentName))
            {
                if (parent == null)
                    parent = GameObject.Find(sceneInfo.parentName).transform;
                else
                    parent = parent.Find(sceneInfo.parentName);
            }
            go.transform.SetParent(parent, false);
            go.transform.SetAsLastSibling();

            go.SetActive(sceneInfo.activate);

            if (sceneInfo.children != null)
            {
                for (int i = 0; i < sceneInfo.children.Count; ++i)
                {
                    LoadSceneInEditor(go.transform, sceneInfo.children[i]);
                }
            }
        }
#endif

        private void Update()
        {
            float startTime = Time.realtimeSinceStartup;

            for (int i = 0; i < inProgresssOperations.Count;)
            {
                if (!inProgresssOperations[i].Update(Time.deltaTime))
                {
                    if (ApplicationSettings.LogScene())
                        inProgresssOperations[i].Print();

                    inProgresssOperations.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }
    }
}
