using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlotMaker
{
    public class LoadedAssetBundle
    {
        public AssetBundle assetBundle;

        public LoadedAssetBundle(AssetBundle assetBundle) : base()
        {
            this.assetBundle = assetBundle;
        }
    }

    public class LoadedAsset
    {
        public string bundleName;
        public string assetName;
        public System.Type assetType;

        public UnityEngine.Object asset;

        public LoadedAsset(string bundleName, string assetName, System.Type assetType, UnityEngine.Object asset) : base()
        {
            this.bundleName = bundleName;
            this.assetName = assetName;
            this.assetType = assetType;
            this.asset = asset;
        }
    }

    public class AssetBundleManager : MonoBehaviour
    {
        private static AssetBundleManifest manifest = null;
        private static string baseFilePath = "";
        private static string baseUrl = "";

        private static HashSet<string> DLCs = new HashSet<string>();
        private static Dictionary<string, LoadedAssetBundle> loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        private static Dictionary<string, Dictionary<string, LoadedAsset>> loadedAssets = new Dictionary<string, Dictionary<string, LoadedAsset>>();
        private static Dictionary<string, AssetBundleLoadOperation> inProgressBundleOperations = new Dictionary<string, AssetBundleLoadOperation>();
        private static List<AssetBundleLoadAssetOperation> inProgressAssetOperations = new List<AssetBundleLoadAssetOperation>();
        private static Dictionary<string, AssetBundleLoadAssetOperation> inProgressAssetOperationReferences = new Dictionary<string, AssetBundleLoadAssetOperation>();
        private static Dictionary<string, string[]> dependencies = new Dictionary<string, string[]>();

        public static AssetBundleManifest Manifest
        {
            get { return manifest; }
            set { manifest = value; }
        }

        public static string BaseFilePath
        {
            get { return baseFilePath; }
            set { baseFilePath = value; }
        }

        public static string BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        public static AssetBundleLoadManifest Initialize()
        {
            var operation = new AssetBundleLoadManifest();
            inProgressAssetOperations.Add(operation);
            return operation;
        }

        public static bool AddDLC(string dlc)
        {
            if (!DLCs.Contains(dlc))
                return DLCs.Add(dlc);

            return false;
        }

        public static LoadedAssetBundle GetLoadedAssetBundle(string bundleName)
        {
            LoadedAssetBundle bundle = null;
            loadedAssetBundles.TryGetValue(bundleName, out bundle);
            if (bundle == null)
                return null;

            string[] deps = null;
            if (!dependencies.TryGetValue(bundleName, out deps))
                return bundle;

            foreach (var dep in deps)
            {
                LoadedAssetBundle dependentBundle;
                loadedAssetBundles.TryGetValue(dep, out dependentBundle);
                if (dependentBundle == null)
                    return null;
            }

            return bundle;
        }

        public static void SetLoadedAssetBundle(string bundleName, LoadedAssetBundle bundle)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
            {
                Debug.LogError("[AssetBundleManager] Already loaded AssetBundle: " + bundleName);
                return;
            }

            loadedAssetBundles.Add(bundleName, bundle);
        }

        public static LoadedAsset GetLoadedAsset(string bundleName, string assetName)
        {
            Dictionary<string, LoadedAsset> loaded;
            if (loadedAssets.TryGetValue(bundleName, out loaded))
            {
                LoadedAsset asset;
                if (loaded.TryGetValue(assetName, out asset))
                    return asset;
            }

            return null;
        }

        public static void SetLoadedAsset(string bundleName, string assetName, LoadedAsset asset)
        {
            Dictionary<string, LoadedAsset> loaded;
            if (loadedAssets.ContainsKey(bundleName))
            {
                loaded = loadedAssets[bundleName];
            }
            else
            {
                loaded = new Dictionary<string, LoadedAsset>();
                loadedAssets.Add(bundleName, loaded);
            }

            if (loaded.ContainsKey(assetName))
            {
                Debug.LogWarning("[AssetBundleManager] Already loaded Asset: " + assetName + " in " + bundleName);
                return;
            }

            loaded.Add(assetName, asset);
        }

        public static void RemoveLoadedAssets(string bundleName)
        {
            loadedAssets.Remove(bundleName);

#if USE_ASSETBUNDLE
            string[] deps = Manifest.GetAllDependencies(bundleName);
#elif UNITY_EDITOR
            string[] deps = AssetDatabase.GetDependencies(bundleName, true);
#else
            string[] deps = null;
#endif

            if (deps.Length == 0)
                return;

            foreach (var dep in deps)
            {
                loadedAssets.Remove(dep);
            }

        }

        public static void UnloadLoadedAssets(string bundleName, bool unloadAllLoadedObjects)
        {
            LoadedAssetBundle bundle = GetLoadedAssetBundle(bundleName);
            if (bundle == null)
                return;

            bundle.assetBundle.Unload(unloadAllLoadedObjects);
        }

        public static AssetBundleLoadOperation LoadAssetBundle(string bundleName)
        {
            if (ApplicationSettings.LogBundle())
                Debug.Log("[AssetBundleManager] Loading AssetBundle: " + bundleName);

#if USE_ASSETBUNDLE
            if (Manifest == null)
            {
                Debug.LogError("[AssetBundleManager] Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return null;
            }
#endif

            var operation = LoadAssetBundleInternal(bundleName);
            return operation;
        }

        public static AssetBundleLoadOperation LoadAssetBundleInternal(string bundleName)
        {
            LoadedAssetBundle bundle = null;
            loadedAssetBundles.TryGetValue(bundleName, out bundle);
            if (bundle != null)
                return new AssetBundleLoadBundleSimulation();

            AssetBundleLoadOperation operation = null;

            if (inProgressBundleOperations.ContainsKey(bundleName))
                return inProgressBundleOperations[bundleName];

#if USE_ASSETBUNDLE
            if (!DLCs.Contains(bundleName))
#if UNITY_WEBGL || USE_ASSETBUNDLE_FILECACHE
                    operation = new AssetBundleLoadWWWBundle(bundleName, false);
#else
                operation = new AssetBundleLoadFileBundle(bundleName);
#endif
            else
            {
                if (bundleName == "fishing"
                    || bundleName == "fishingaudio"
                    || bundleName == "fishingpanel"
                    || bundleName == "fishingeffect"
                    || bundleName == "fishinggold"
                    || bundleName == "fishinglighteffect"
                    || bundleName == "fishingnet"
                    || bundleName == "fishingouttips"
                    || bundleName == "fishingscore"
                    || bundleName == "fishingspecialdeclare"
                    || bundleName == "fishingtips"
                    || bundleName == "fishingplustips"
                    || bundleName == "fishingbg")
                    operation = new AssetBundleLoadFileBundle(bundleName);
                else
                    operation = new AssetBundleLoadWWWBundle(bundleName);
            }
#else
            operation = new AssetBundleLoadBundleSimulation();
#endif

            inProgressBundleOperations.Add(bundleName, operation);
            return operation;
        }

        public static List<AssetBundleLoadOperation> LoadDependencies(string bundleName)
        {
            var depOps = new List<AssetBundleLoadOperation>();

#if USE_ASSETBUNDLE
            string[] deps = Manifest.GetAllDependencies(bundleName);
            if (deps.Length == 0)
                return depOps;

            bool isDLC = DLCs.Contains(bundleName);

            if (!dependencies.ContainsKey(bundleName))
                dependencies.Add(bundleName, deps);

            for (int i = 0; i < deps.Length; ++i)
            {
                if (isDLC) AddDLC(deps[i]);
                depOps.Add(LoadAssetBundleInternal(deps[i]));
            }
#endif
            return depOps;
        }

        public static void UnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects)
        {
#if USE_ASSETBUNDLE
            UnloadAssetBundleInternal(bundleName, unloadAllLoadedObjects);
            UnloadDependencies(bundleName, unloadAllLoadedObjects);
#endif
        }

        protected static void UnloadDependencies(string bundleName, bool unloadAllLoadedObjects)
        {
            string[] deps = null;
            if (!dependencies.TryGetValue(bundleName, out deps))
                return;

            foreach (var dep in deps)
            {
                UnloadAssetBundleInternal(dep, unloadAllLoadedObjects);
            }

            dependencies.Remove(bundleName);
        }

        protected static void UnloadAssetBundleInternal(string bundleName, bool unloadAllLoadedObjects)
        {
            LoadedAssetBundle bundle = GetLoadedAssetBundle(bundleName);
            if (bundle == null)
                return;

            bundle.assetBundle.Unload(unloadAllLoadedObjects);
            loadedAssetBundles.Remove(bundleName);

            if (ApplicationSettings.LogBundle())
                Debug.Log("[AssetBundleManager] " + bundleName + " has been unloaded");
        }

        public static AssetBundleLoadAssetOperation LoadAssetAsync(string bundleName, string assetName, System.Type type)
        {
#if USE_ASSETBUNDLE
            var key = bundleName + assetName;
#endif

            LoadedAsset asset = GetLoadedAsset(bundleName, assetName);
            if (asset != null)
                return new AssetBundleLoadAssetSimulation(bundleName, assetName, type);

            if (ApplicationSettings.LogBundle())
                Debug.Log("[AssetBundleManager] Loading " + assetName + " from " + bundleName + " bundle");

            AssetBundleLoadAssetOperation operation = null;
#if USE_ASSETBUNDLE
            inProgressAssetOperationReferences.TryGetValue(key, out operation);
            if (operation == null)
            {
                operation = new AssetBundleLoadAsset(bundleName, assetName, type);

                inProgressAssetOperations.Add(operation);
                inProgressAssetOperationReferences.Add(key, operation);
            }
#else
            operation = new AssetBundleLoadAssetSimulation(bundleName, assetName, type);
#endif
            return operation;
        }

        public static AssetBundleLoadAssetOperation LoadAssetAsync<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            return LoadAssetAsync(bundleName, assetName, typeof(T));
        }

        public static UnityEngine.Object LoadAsset(string bundleName, string assetName, System.Type type)
        {
            if (ApplicationSettings.LogBundle())
                Debug.Log("[AssetBundleManager] Load " + assetName + " from " + bundleName + " bundle");

            LoadedAsset asset = GetLoadedAsset(bundleName, assetName);
            if (asset != null)
                return asset.asset;

#if USE_ASSETBUNDLE
            if (bundleName == "Android")
                Debug.Log("[AssetBundleManager] There is no asset with name \"" + assetName + "\" in " + bundleName);
            LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName);
            if (bundle != null)
            {
                asset = new LoadedAsset(bundleName, assetName, type, bundle.assetBundle.LoadAsset(assetName, type));
                SetLoadedAsset(bundleName, assetName, asset);
                return asset.asset;
            }
            else
                return null;
#elif UNITY_EDITOR
            if ( bundleName == "Android")
                Debug.Log("[AssetBundleManager] There is no asset with name \"" + assetName + "\" in " + bundleName);
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            if (assetPaths.Length > 0)
            {
                asset = new LoadedAsset(bundleName, assetName, type, AssetDatabase.LoadMainAssetAtPath(assetPaths[0]));
                SetLoadedAsset(bundleName, assetName, asset);
                return asset.asset;
            }
            else
            {
                Debug.Log("[AssetBundleManager] There is no asset with name \"" + assetName + "\" in " + bundleName);
                return null;
            }
#else
            return null;
#endif
        }

        public static T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            return LoadAsset(bundleName, assetName, typeof(T)) as T;
        }

        public static void UnloadAsset(string bundleName, string assetName)
        {
            if (!loadedAssets.ContainsKey(bundleName))
                return;

            loadedAssets[bundleName].Remove(assetName);

            if (ApplicationSettings.LogBundle())
                Debug.Log("[AssetBundleManager] " + assetName + " in " + bundleName + " has been unloaded");
        }

        private void Update()
        {
            UpdateLoadBundleProcess();
            UpdateLoadAssetProcess();
        }

        private void UpdateLoadBundleProcess()
        {
            foreach (var bundleOperator in new Dictionary<string, AssetBundleLoadOperation>(inProgressBundleOperations))
            {
                if (!bundleOperator.Value.Update())
                    inProgressBundleOperations.Remove(bundleOperator.Key);
            }
        }

        private void UpdateLoadAssetProcess()
        {
            for (int i = 0; i < inProgressAssetOperations.Count;)
            {
                if (!inProgressAssetOperations[i].Update())
                {
                    var operation = inProgressAssetOperations[i];
                    var key = operation.bundleName + operation.assetName;

                    if (inProgressAssetOperationReferences.ContainsKey(key))
                        inProgressAssetOperationReferences.Remove(key);

                    inProgressAssetOperations.RemoveAt(i);
                }
                else
                    ++i;
            }
        }
    }
}
