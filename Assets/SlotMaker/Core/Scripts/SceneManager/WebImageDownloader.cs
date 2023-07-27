using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

namespace SlotMaker
{
    public enum CacheType : int
    {
        None        = 1,
        MemCache    = 2,
        FileCache   = 4,
        All         = 6,
    };
        
    public class WebImageDownloader : MonoWeakSingleton<WebImageDownloader>
    {
        public class WebImageDownloadError
        {
            public string url;
            public string hashCode;
            public long responseCode;
        }

        public class DownloadRequest
        {
            public string url;
            public string hashCode;
            public CacheType cacheType;
            public UnityWebRequest request;
            
            public Action<string> onSuccess;
            public Action<Sprite> onLoaded;
            public Action<float>  onProgress;
            public Action<WebImageDownloadError> onFailed;
        }
        
        public Dictionary<string, Sprite> memCache = new Dictionary<string, Sprite>();
        
        public List<DownloadRequest> priorityQueue  = new List<DownloadRequest>();
        public List<DownloadRequest> queue = new List<DownloadRequest>();
        public List<DownloadRequest> inProgressQueue = new List<DownloadRequest>();
        
        private string CACHED_PATH;
        private string NOT_CACHED_PATH;

        private string LEGACY_CACHED_PATH;
        
        private bool ignoreFileCache = false;
        
        private void Awake()
        {
    #if UNITY_WEBGL && !UNITY_EDITOR
            ignoreFileCache = true;
    #else
            LEGACY_CACHED_PATH = ApplicationSettings.GetCachePath() + "/CachedImages/";
            CACHED_PATH = ApplicationSettings.GetCachePath() + "/CachedImagesV2/";
            NOT_CACHED_PATH = ApplicationSettings.GetCachePath() + "/NotCachedImages/";
            
            FileUtils.DeleteFolder(LEGACY_CACHED_PATH);
            FileUtils.DeleteFolder(NOT_CACHED_PATH);
            FileUtils.CreateFolder(NOT_CACHED_PATH);
            FileUtils.CreateFolder(CACHED_PATH);
            
            if (!FileUtils.ExistDirectory(NOT_CACHED_PATH) || !FileUtils.ExistDirectory(CACHED_PATH))
                ignoreFileCache = true;
    #endif
        }

#if UNITY_EDITOR
        public static void ClearWebImages()
        {
            string CACHED_PATH = ApplicationSettings.GetCachePath() + "/CachedImagesV2/";
            string NOT_CACHED_PATH = ApplicationSettings.GetCachePath() + "/NotCachedImages/";
            
            FileUtils.DeleteFolder(CACHED_PATH);
            FileUtils.DeleteFolder(NOT_CACHED_PATH);
        }
#endif
        
        public void ClearMemCache()
        {
            foreach (var pair in memCache)
            {
                UnityEngine.Object.Destroy(pair.Value);
            }
            memCache.Clear();
        }
        
        private bool HasCacheType(CacheType source, CacheType mask)
        {
            return (int)(source & mask) == (int)mask;
        }
        
        private string GetCachedFolder(CacheType cacheType)
        {
            return (HasCacheType(cacheType, CacheType.MemCache) ? NOT_CACHED_PATH : CACHED_PATH);
        }

        private string GetCachedPath(string hashCode, CacheType cacheType)
        {
            return GetCachedFolder(cacheType) + hashCode;
        }

        private void CheckCachedFolder(CacheType cacheType)
        {
            string folderPath = GetCachedFolder(cacheType);

            if(!FileUtils.ExistDirectory(folderPath))
                FileUtils.CreateFolder(folderPath);
        }

        public bool CheckCachedImage(string url, CacheType cacheType)
        {
            if(string.IsNullOrEmpty(url)) return false;
            
            string hashCode = GetHash(url);
            
            Sprite sprite = null;
            if (memCache.TryGetValue(hashCode, out sprite))
                return true;
            
            if (!ignoreFileCache)
            {
                string path = GetCachedPath(hashCode, cacheType);
                if (FileUtils.Exist(path))
                    return true;
            }
            
            return false;
        }

        public bool CheckRequestedImageByURL(string url)
        {
            if(string.IsNullOrEmpty(url)) return false;

            string hashCode = GetHash(url);
            return CheckRequestedImageByHash(hashCode);
        }

        public bool CheckRequestedImageByHash(string hashCode)
        {
            if(string.IsNullOrEmpty(hashCode)) return false;

            var request = GetDownloadRequest(hashCode);

            return request != null;
        }

        public DownloadRequest GetDownloadRequest(string hashCode)
        {
            foreach(DownloadRequest request in inProgressQueue)
            {
                if(request.hashCode == hashCode) return request;
            }

            foreach(DownloadRequest request in priorityQueue)
            {
                if(request.hashCode == hashCode) return request;
            }

            foreach(DownloadRequest request in queue)
            {
                if(request.hashCode == hashCode) return request;
            }

            return null;
        }
        
        public void LoadWebImage(string url, CacheType cacheType = CacheType.None, bool priority = false, 
            Action<string> onSuccess = null, Action<Sprite> onLoaded = null, Action<float> onProgress = null, Action<WebImageDownloadError> onFailed = null)
        {
            string hashCode = GetHash(url);
            
            Sprite sprite = null;
            if (memCache.TryGetValue(hashCode, out sprite))
            {
                if (onLoaded != null)
                    onLoaded.Invoke(sprite);
                
                if (onSuccess != null)
                    onSuccess.Invoke(url);
                
                return;
            }
            
            if (!ignoreFileCache)
            {
                string path = GetCachedPath(hashCode, cacheType);

                if (FileUtils.Exist(path))
                {
                    if (onLoaded != null)
                    {
                        Texture2D tex2D = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                        byte[] bytes = FileUtils.Read(path);
                        ImageConversion.LoadImage(tex2D, bytes);
                        sprite = Sprite.Create(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), Vector2.zero, 100f);
                        memCache[hashCode] = sprite;
                        onLoaded.Invoke(sprite);
                    }
                    
                    if (onSuccess != null)
                        onSuccess.Invoke(url);
                    
                    return;
                }
            }

            var request = GetDownloadRequest(hashCode);
            if(request != null)
            {
                request.onSuccess += onSuccess;
                request.onLoaded += onLoaded;
                request.onProgress += onProgress;
                request.onFailed += onFailed;
            }
            else
            {
                request = new DownloadRequest
                {
                    url = url,
                    hashCode = hashCode,
                    cacheType = cacheType,
                    onSuccess = onSuccess,
                    onLoaded = onLoaded,
                    onProgress = onProgress,
                    onFailed = onFailed
                };

                if (priority)
                    priorityQueue.Add(request);
                else
                    queue.Add(request);
            }
        }

        private void Update()
        {
            int processingLimit = ApplicationSettings.Instance.asyncLoadWebImageLimit;

            while ((priorityQueue.Count > 0) && (inProgressQueue.Count < processingLimit))
            {
                inProgressQueue.Add(priorityQueue[0]);
                priorityQueue.RemoveAt(0);
            }

            while ((queue.Count > 0) && (inProgressQueue.Count < processingLimit))
            {
                inProgressQueue.Add(queue[0]);
                queue.RemoveAt(0);
            }

            for (int i = 0; i < inProgressQueue.Count;)
            {
                var downloadRequest = inProgressQueue[i];
                if (downloadRequest.request == null)
                {
                    downloadRequest.request = (downloadRequest.onLoaded != null) ? UnityWebRequestTexture.GetTexture(downloadRequest.url) : UnityWebRequest.Get(downloadRequest.url);
                    downloadRequest.request.SendWebRequest();
                }
                else
                {
                    if (downloadRequest.onProgress != null)
                        downloadRequest.onProgress.Invoke(downloadRequest.request.downloadProgress);

                    bool done = false;
#if UNITY_WEBGL && !UNITY_EDITOR
                    if (downloadRequest.request.responseCode <= 0)
                    {
                        /// If the UnityWebRequest has not yet processed a response, this property will return -1.
                    }
                    else if (downloadRequest.request.isNetworkError || downloadRequest.request.responseCode != 200)
#else
                    if (downloadRequest.request.isNetworkError || downloadRequest.request.isHttpError)
#endif
                    {
                        if (downloadRequest.onFailed != null)
                        {
                            WebImageDownloadError failedInfo = new WebImageDownloadError
                            {
                                url = downloadRequest.url,
                                hashCode = downloadRequest.hashCode,
                                responseCode = downloadRequest.request.responseCode
                            };

                            downloadRequest.onFailed.Invoke(failedInfo);
                        }
                        done = true;
                    }
                    else if (downloadRequest.request.isDone)
                    {
                        if (!ignoreFileCache)
                        {
                            CheckCachedFolder(downloadRequest.cacheType);
                            
                            string path = GetCachedPath(downloadRequest.hashCode, downloadRequest.cacheType);

                            if (!FileUtils.Exist(path))
                            {
                                byte[] bytes = downloadRequest.request.downloadHandler.data;
                                FileUtils.Write(path, bytes);
                            }
                        }
                        
                        if (downloadRequest.onLoaded != null)
                        {
                            Sprite sprite = null;
                            if (!memCache.TryGetValue(downloadRequest.hashCode, out sprite))
                            {
                                Texture2D tex2D = null;

                                DownloadHandlerTexture textureHandler = downloadRequest.request.downloadHandler as DownloadHandlerTexture;
                                if(textureHandler == null)
                                {
                                    tex2D = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                                    ImageConversion.LoadImage(tex2D, downloadRequest.request.downloadHandler.data);
                                }
                                else
                                {
                                    tex2D = textureHandler.texture;
                                }

                                if (tex2D != null)
                                {
                                    sprite = Sprite.Create(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), Vector2.zero, 100f);
                                    memCache[downloadRequest.hashCode] = sprite;
                                }
                                
                            }
                            downloadRequest.onLoaded.Invoke(sprite);
                        }
                        
                        if (downloadRequest.onSuccess != null)
                        {
                            downloadRequest.onSuccess.Invoke(downloadRequest.url);
                        }
                        
                        done = true;
                    }
                    
                    if (done)
                    {
                        downloadRequest.request.Dispose();
                        inProgressQueue.RemoveAt(i);
                        continue;
                    }
                }
                
                ++i;
            }
        }

        private static MD5 md5Hash = MD5.Create();

        private static string GetHash(string source)
        {
            if (string.IsNullOrEmpty(source)) return null;

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}

