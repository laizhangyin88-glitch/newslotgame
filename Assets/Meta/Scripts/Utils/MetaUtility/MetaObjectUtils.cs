using UnityEngine;
using System;
using System.Collections;
using NodeCanvas.Framework;
using SlotMaker;
using BSS.Utils;

using Object = UnityEngine.Object;
using ParadoxNotion;

namespace BagelCode
{
    public static class MetaObjectUtils
    {
        private static Transform mainCanvasAreaTransform;
        public static Transform MainCanvasAreaTransform
        {
            get
            {
                if (mainCanvasAreaTransform is null)
                    mainCanvasAreaTransform = GameObject.Find("Main Canvas/Area").transform;

                return mainCanvasAreaTransform;
            }
        }

        public static bool ExistPrefab(string bundleName, string assetName)
        {
            if (string.IsNullOrEmpty(assetName)) return false;

            var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, assetName);
            if (prefab != null)
                return true;

            return false;
        }

        public static GameObject MakePrefab(string bundleName, string assetName, Transform root, string parentName = null, string objectName = null)
        {
            if (string.IsNullOrEmpty(assetName)) return null;

            var prefab = AssetBundleManager.LoadAsset<GameObject>(bundleName, assetName);
            if (prefab == null)
            {
                Debug.LogWarning("MakePrefab failure. bundleName: " + bundleName + ", assetName: " + assetName);
                return null;
            }

            GameObject go = GameObject.Instantiate(prefab) as GameObject;

            if (string.IsNullOrEmpty(objectName))
                go.name = assetName;
            else
                go.name = objectName;

            Transform parent = root;

            if (!string.IsNullOrEmpty(parentName))
            {
                if (parent != null)
                {
                    parent = parent.Find(parentName);
                }
                else
                {
                    GameObject parentObj = GameObject.Find(parentName);
                    if (parentObj != null)
                        parent = parentObj.transform;
                }
            }

            if (parent != null)
                go.transform.SetParent(parent, false);

            return go;
        }

        public static IEnumerator MakePrefabCoroutine(string bundleName, string assetName, Transform root, Action<GameObject> onLoadAction = null)
        {
            bool fail = false;
            bool success = false;

            GameObject obj = null;

            if (root == null || !root.gameObject.activeSelf) yield break;

            var op = AssetBundleManager.LoadAssetAsync<GameObject>(bundleName, assetName);
            if (op == null) yield break;

            CoroutineUtility.ExcuteAfterCondition(root.GetComponent<MonoBehaviour>(), () => op.IsDone(),
                () =>
                {
                    var prefab = op.GetAsset<GameObject>();
                    if (prefab == null)
                    {
                        fail = true;
                        return;
                    }

                    obj = GameObject.Instantiate(prefab) as GameObject;
                    obj.name = assetName;

                    Transform parent = root;

                    if (parent != null)
                        obj.transform.SetParent(parent, false);

                    onLoadAction?.Invoke(obj);
                    success = true;
                }
            );

            yield return new WaitUntil(() => success || fail);

            if (fail)
            {
                Debug.LogWarning(string.Format("MakePrefabCoroutine failure. bundle:{0}, asset:{1}", bundleName, assetName));
            }
        }

        public static void MakePrefabAsync(string bundleName, string assetName, Transform root, Action<GameObject> OnResultAct = null)
        {
            GameObject go = null;

            if (!string.IsNullOrEmpty(assetName))
            {
                var op = AssetBundleManager.LoadAssetAsync<GameObject>(bundleName, assetName);
                CoroutineUtility.ExcuteAfterCondition(root.GetComponent<MonoBehaviour>(), () => root == null || op.IsDone(),
                    () =>
                    {
                        if (root == null) return;
                        var prefab = op.GetAsset<GameObject>();
                        go = GameObject.Instantiate(prefab) as GameObject;
                        go.name = assetName;
                        Transform parent = root;
                        if (parent != null)
                        {
                            go.transform.SetParent(parent, false);
                        }
                        OnResultAct?.Invoke(go);
                    }
                );
            }
        }

        public static GameObject MakeScene(string bundleName, string assetName, Transform root, string parentName = "", bool useAssetName = true)
        {
            if (string.IsNullOrEmpty(assetName)) return null;

            var sceneInfo = AssetBundleManager.LoadAsset<SceneInfoObject>(bundleName, assetName).GetSceneInfo();

            Transform parent = root;

            if (!string.IsNullOrEmpty(parentName))
            {
                if (parent != null)
                {
                    parent = parent.Find(parentName);
                }
                else
                {
                    GameObject parentObj = GameObject.Find(parentName);
                    if (parentObj != null)
                        parent = parentObj.transform;
                }
            }

            GameObject go = SceneManager.LoadScene(parent, sceneInfo);
            if(useAssetName) go.name = assetName;

            return go;
        }

        public static GameObject MakePrefab(string assetName, Transform parent)
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName);
            if (prefab == null)
                Debug.LogError($"({assetName}) Not Exist.");

            var copy = Object.Instantiate(prefab, parent);
            copy.name = prefab.name;
            var element = copy.GetComponent<ContextElement>();

            if (element != null)
            {
                if (element.Parent != null)
                {
                    element.Parent.UpdateContext(false);
                }
                else
                {
                    element.UpdateContext(false);
                }
            }

            return copy;
        }

        public static T MakePrefab<T>(string assetName, Transform parent) where T : Component
        {
            var prefab = AssetBundleManager.LoadAsset<GameObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName);

            if (prefab == null)
                Debug.LogError($"({assetName}) Not Exist.");

            var copy = Object.Instantiate(prefab, parent);
            copy.name = prefab.name;

            var element = copy.GetComponent<ContextElement>();
            if (element != null)
            {
                if (element.Parent != null)
                {
                    element.Parent.UpdateContext(false);
                }
                else
                {
                    element.UpdateContext(false);
                }
            }

            var component = copy.GetComponent<T>();
            if (component == null)
                Debug.LogError($"({typeof(T).ToString()}) Not attached to the ({assetName}).");
            return component;
        }

        public static GameObject MakeScene(string sceneName, Transform parent)
        {
            var sceneInfoObj = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, sceneName);
            if (sceneInfoObj == null)
                Debug.LogError($"({sceneName}) Not Exist.");

            var copy = SceneManager.LoadScene(parent, sceneInfoObj.sceneInfo);
            var element = copy.GetComponent<ContextElement>();

            if (element != null)
            {
                element.UpdateContext(false);
            }

            return copy;
        }

        public static T MakeScene<T>(string sceneName, Transform parent) where T : Component
        {
            var sceneInfoObj = AssetBundleManager.LoadAsset<SceneInfoObject>(MetaStringDefine.LOBBY_BUNDLE_NAME, sceneName);
            if (sceneInfoObj == null)
                Debug.LogError($"({sceneName}) Not Exist.");

            var copy = SceneManager.LoadScene(parent, sceneInfoObj.sceneInfo);
            var element = copy.GetComponent<ContextElement>();
            if (element != null)
            {
                element.UpdateContext(false);
            }

            var component = copy.GetComponent<T>();
            if (component == null)
                Debug.LogError($"({typeof(T).ToString()}) Not attached to the ({sceneName}).");

            return component;
        }

        public static IEnumerator MakeSceneCoroutine(string bundle, string asset, Transform root, System.Action<GameObject> onLoadScene = null)
        {
            AssetBundleLoadAssetOperation bundleLoadOperation = null;
            SceneLoadOperation sceneLoadOperation = null;

            GameObject sceneObj = null;

            while (sceneObj == null)
            {
                if (bundleLoadOperation == null)
                    bundleLoadOperation = AssetBundleManager.LoadAssetAsync<SceneInfoObject>(bundle, asset);

                if (bundleLoadOperation.IsDone())
                {
                    if (sceneLoadOperation == null)
                    {
                        var sceneInfo = bundleLoadOperation.GetAsset<SceneInfoObject>().GetSceneInfo();
                        sceneLoadOperation = SceneManager.LoadSceneAsync(root, sceneInfo, true);
                    }

                    if (sceneLoadOperation.IsDone())
                    {
                        sceneObj = sceneLoadOperation.GetScene();
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            if (sceneObj != null)
            {
                onLoadScene?.Invoke(sceneObj);
            }
        }

        public static void UpdateBadge(ContextElement badgeAreaElement, bool isNewBadge, int count = 0)
        {
            if (badgeAreaElement.transform.childCount == 0)
            {
                MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Badge", badgeAreaElement.transform, "");
                badgeAreaElement.UpdateContext(true);
            }
            else
            {
                badgeAreaElement.UpdateContext(false);
            }

            ContextElement badgeElement = ContextUtils.FindElement(badgeAreaElement, "Badge", ContextSearchingType.ChildrenSearch);
            ContextElement badgeTextElement = ContextUtils.FindElement(badgeElement, "Text", ContextSearchingType.ChildrenSearch);
            ContextAnimator badgeAnimator = badgeElement as ContextAnimator;
            badgeAnimator.isPreserve = true;
            badgeAnimator.propertyName = "value";

            if (isNewBadge)
            {
                badgeAnimator.SetIntProperty(1);
                MetaContextElementUtils.SetText(badgeTextElement, "N");
            }
            else
            {
                badgeAnimator.SetIntProperty(count);
                if (count > 99)
                    MetaContextElementUtils.SetText(badgeTextElement, "99+");
                else
                    MetaContextElementUtils.SetText(badgeTextElement, count.ToString());
            }
        }

        public static string GetBundleName(bool combineApplicationType, string bundleName)
        {
            return combineApplicationType ? ApplicationSettings.MakeApplicationBundleName(bundleName) : bundleName;
        }

        public static Sprite MakeSprite(string bundleName, string assetName)
        {
#if UNITY_EDITOR
            var tex2D = AssetBundleManager.LoadAsset<Texture2D>(bundleName, assetName);
            if (tex2D != null)
            {
                Rect rec = new Rect(0, 0, tex2D.width, tex2D.height);
                return Sprite.Create(tex2D, rec, new Vector2(0, 0), 1);
            }

            return null;
#else
            return AssetBundleManager.LoadAsset<Sprite>(bundleName, assetName);
#endif
        }

        public static GameObject GetCaller(GameObject callee, string callerName = "caller")
        {
            Blackboard bb = callee.GetComponent<Blackboard>();
            if (bb != null) return BlackboardUtils.FindVariable<GameObject>(bb, callerName)?.value;
            return null;
        }

        public static void SetCalleeCaller(GameObject target, GameObject caller, string callerName = "caller")
        {
            Blackboard bb = target.GetComponent<Blackboard>();
            if (bb != null) bb.AddVariable(callerName, caller);
        }

        public static void EnableBackButton(bool isEnable)
        {
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT,
                new EventData(isEnable ? "OnEnableBackButton" : "OnDisableBackButton"));
        }
    }
}
