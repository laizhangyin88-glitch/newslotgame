using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;

namespace SlotMaker
{
    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        public object Current { get { return null; } }
        public bool MoveNext() { return !IsDone(); }
        public void Reset() {}

        public abstract bool Update();
        public abstract bool IsDone();
        public abstract float Progress();
        public abstract string GetError();
    }

    public class AssetBundleLoadBundleSimulation : AssetBundleLoadOperation
    {
        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }

        public override float Progress()
        {
            return 1f;
        }

        public override string GetError()
        {
            return null;
        }
    }

    public class AssetBundleLoadFileBundle : AssetBundleLoadOperation
    {
        protected string bundleName;
        protected AssetBundleCreateRequest request = null;
        protected bool isDone = false;

        private PerformanceAnalyzer.TimeSample timeSample;

        public AssetBundleLoadFileBundle(string bundleName)
        {
            this.bundleName = bundleName;

            timeSample = new PerformanceAnalyzer.TimeSample("bundle", bundleName);
        }

        public override bool Update()
        {
            if (request == null)
            {
                string bundlePath = Path.Combine(AssetBundleManager.BaseFilePath, bundleName);
#if UNITY_ANDROID
                bundlePath += ".assetbundle";
#endif
                request = AssetBundle.LoadFromFileAsync(bundlePath);

                timeSample.BeginSample();
            }

            if (request.isDone)
            {
                var bundle = new LoadedAssetBundle(request.assetBundle);
                AssetBundleManager.SetLoadedAssetBundle(bundleName, bundle);
                isDone = true;

                timeSample.EndSample();

                Debug.Log("[AssetBundleManager] Loaded AssetBundle(LoadFromFileAsync): " + bundleName);

                return false;
            }

            return true;
        }

        public override bool IsDone()
        {
            return isDone;
        }

        public override float Progress()
        {
            if (request != null)
                return request.progress;

            return 0f;
        }

        public override string GetError()
        {
            return null;
        }
    }

    public class AssetBundleLoadWWWBundle : AssetBundleLoadOperation
    {
        protected string bundleName;
        protected bool checkHash;
        protected UnityWebRequest request = null;

        protected bool isDone = false;
        protected string error = null;

        private float beginTime;
        private float lastProgress;

        private PerformanceAnalyzer.TimeSample timeSample;

        public AssetBundleLoadWWWBundle(string bundleName, bool checkHash = true)
        {
            this.bundleName = bundleName;
            this.checkHash = checkHash;

            timeSample = new PerformanceAnalyzer.TimeSample("bundle", bundleName);
        }

        public override bool Update()
        {
            if (request == null)
            {
                if (checkHash)
                {
                    request = UnityWebRequestAssetBundle.GetAssetBundle(
                       Path.Combine(AssetBundleManager.BaseUrl, bundleName),
                       AssetBundleManager.Manifest.GetAssetBundleHash(bundleName));
                }
                else
                {
#if UNITY_WEBGL
                    request = UnityWebRequestAssetBundle.GetAssetBundle(
                        Path.Combine(AssetBundleManager.BaseFilePath, bundleName));
#elif UNITY_ANDROID
                    request = UnityWebRequestAssetBundle.GetAssetBundle(
                        Path.Combine(AssetBundleManager.BaseFilePath, bundleName + ".assetbundle"));
#else
                    request = UnityWebRequestAssetBundle.GetAssetBundle(
                        "file://" + Path.Combine(AssetBundleManager.BaseFilePath, bundleName));
#endif
                }
                request.SendWebRequest();

                beginTime = Time.realtimeSinceStartup;
                lastProgress = 0f;

                timeSample.BeginSample();
            }

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(string.Format("[AssetBundleManager] Failed downloading bundle from {0}: {1}", request.url, request.error));

                isDone = true;
                error = request.error;
                lastProgress = request.downloadProgress;
                request.Dispose();

                return false;
            }

            if (request.isDone)
            {
                var bundle = new LoadedAssetBundle(DownloadHandlerAssetBundle.GetContent(request));
                AssetBundleManager.SetLoadedAssetBundle(bundleName, bundle);

                isDone = true;
                lastProgress = request.downloadProgress;
                request.Dispose();

                timeSample.EndSample();

                Debug.Log("[AssetBundleManager] Loaded AssetBundle(UnityWebRequest): " + bundleName);

                return false;
            }

            if (ApplicationSettings.Instance.asyncLoadBundleTimeoutEnabled)
            {
                float endTime = Time.realtimeSinceStartup;
                float timeSpan = endTime - beginTime;
                if (timeSpan > ApplicationSettings.Instance.asyncLoadBundleTimeout && lastProgress == request.downloadProgress)
                {
                    Debug.LogError(string.Format("[AssetBundleManager] Failed downloading bundle from {0}: Timeout", request.url));

                    isDone = true;
                    error = "Timeout";
                    lastProgress = request.downloadProgress;
                    request.Dispose();

                    return false;
                }
            }

            if (lastProgress != request.downloadProgress)
                beginTime = Time.realtimeSinceStartup;
            lastProgress = request.downloadProgress;

            return true;
        }

        public override bool IsDone()
        {
            return isDone;
        }

        public override float Progress()
        {
            return Mathf.Max(lastProgress, 0f);
        }

        public override string GetError()
        {
            return error;
        }
    }

    public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
    {
        public string bundleName;
        public string assetName;

        public abstract T GetAsset<T>() where T : UnityEngine.Object;

        public override string GetError()
        {
            return null;
        }
    }

    public class AssetBundleLoadAssetSimulation : AssetBundleLoadAssetOperation
    {
        protected UnityEngine.Object asset = null;

        public AssetBundleLoadAssetSimulation(string bundleName, string assetName, System.Type type)
        {
            this.bundleName = bundleName;
            this.assetName = assetName;

            asset = AssetBundleManager.LoadAsset(bundleName, assetName, type);
        }

        public override T GetAsset<T>()
        {
            return asset as T;
        }

        public override bool Update()
        {
            return false;
        }

        public override bool IsDone()
        {
            return true;
        }

        public override float Progress()
        {
            return 1f;
        }
    }

    public class AssetBundleLoadAsset : AssetBundleLoadAssetOperation
    {
        protected string downloadingError;
        protected System.Type type;
        protected LoadedAssetBundle bundle = null;
        protected AssetBundleRequest request = null;
        protected bool isDone = false;

        public AssetBundleLoadAsset(string bundleName, string assetName, System.Type type)
        {
            this.bundleName = bundleName;
            this.assetName = assetName;
            this.type = type;
            this.bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName);
        }

        public override T GetAsset<T>()
        {
            return request.asset as T;
        }

        public override bool Update()
        {
            if (request == null)
            {
                /// CRASH REPORT
                if (bundle == null || bundle.assetBundle == null)
                {
                    try
                    {
                        throw new System.NullReferenceException(bundleName);
                    }
                    catch(System.Exception e)
                    {
                        Debug.LogException(e);
                        return false;
                    }
                }
                /// CRASH REPORT
                request = bundle.assetBundle.LoadAssetAsync(assetName, type);
            }

            if (request.isDone)
            {
                if(request.asset != null)
                {
                    var asset = new LoadedAsset(bundleName, assetName, type, request.asset);
                    AssetBundleManager.SetLoadedAsset(bundleName, assetName, asset);
                }

                isDone = true;

                return false;
            }

            return true;
        }

        public override bool IsDone()
        {
            return isDone;
        }

        public override float Progress()
        {
            if (request != null)
            {
                return request.progress;
            }

            return 0f;
        }
    }

    public class AssetBundleLoadManifest : AssetBundleLoadAsset
    {
        protected UnityWebRequest www = null;
        protected AssetBundle manifestBundle = null;

        public AssetBundleLoadManifest() : base(ApplicationSettings.GetPlatformName(), "AssetBundleManifest", typeof(AssetBundleManifest)) {}

        public override bool Update()
        {
            if (www == null)
            {
                www = UnityWebRequestAssetBundle.GetAssetBundle(Path.Combine(AssetBundleManager.BaseUrl, bundleName));
                www.SendWebRequest();
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(string.Format("[AssetBundleManager] Failed downloading bundle AssetBundleManifest from {0}: {1}", www.url, www.error));
                return false;
            }

            if (request == null)
            {
                if (www.isDone)
                {
                    manifestBundle = DownloadHandlerAssetBundle.GetContent(www);
                    request = manifestBundle.LoadAssetAsync(assetName, type);
                    return true;
                }
            }
            else
            {
                if (request.isDone)
                {
                    AssetBundleManager.Manifest = GetAsset<AssetBundleManifest>();
                    isDone = true;

                    manifestBundle.Unload(false);
                    www.Dispose();

                    if (ApplicationSettings.LogBundle())
                        Debug.Log("[AssetBundleManager] Loaded AssetBundleManifest");

                    return false;
                }
            }

            return true;
        }
    }
}
