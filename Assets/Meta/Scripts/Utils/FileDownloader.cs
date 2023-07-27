using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using SlotMaker;

namespace BagelCode
{
    public class FileDownloader : MonoWeakSingleton<FileDownloader>
    {
        public class FileDownloadedInfo
        {
            public string url;
            public string path;
            public byte[] data;
        }

        public class FileDownloadError
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
            public Action<FileDownloadedInfo> onLoaded;
            public Action<float>  onProgress;
            public Action<FileDownloadError> onFailed;

            private string _extension;
            public string Extension
            {
                get
                {
                    if(string.IsNullOrEmpty(_extension))
                        _extension = System.IO.Path.GetExtension(url);

                    return _extension;
                }
            }
        }
        
        public Dictionary<string, FileDownloadedInfo> memCache = new Dictionary<string, FileDownloadedInfo>();
        
        public List<DownloadRequest> priorityQueue  = new List<DownloadRequest>();
        public List<DownloadRequest> queue = new List<DownloadRequest>();
        public List<DownloadRequest> inProgressQueue = new List<DownloadRequest>();
        
        private string CACHED_PATH;
        private string NOT_CACHED_PATH;
        
        private bool ignoreFileCache = false;
        
        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ignoreFileCache = true;
#else
            CACHED_PATH = ApplicationSettings.GetCachePath() + "/CachedFiles/";
            NOT_CACHED_PATH = ApplicationSettings.GetCachePath() + "/NotCachedFiles/";
            
            FileUtils.DeleteFolder(NOT_CACHED_PATH);
            FileUtils.CreateFolder(NOT_CACHED_PATH);
            FileUtils.CreateFolder(CACHED_PATH);
            
            if (!FileUtils.ExistDirectory(NOT_CACHED_PATH) || !FileUtils.ExistDirectory(CACHED_PATH))
                ignoreFileCache = true;
#endif
        }

#if UNITY_EDITOR
        public static void ClearFiles()
        {
            string CACHED_PATH = ApplicationSettings.GetCachePath() + "/CachedFiles/";
            string NOT_CACHED_PATH = ApplicationSettings.GetCachePath() + "/NotCachedFiles/";
            
            FileUtils.DeleteFolder(CACHED_PATH);
            FileUtils.DeleteFolder(NOT_CACHED_PATH);
        }
#endif
        
        public void ClearMemCache()
        {
            // foreach (var pair in memCache)
            // {
            //     UnityEngine.Object.Destroy(pair.Value);
            // }
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

        public bool CheckCachedFile(string url, CacheType cacheType)
        {
            if(string.IsNullOrEmpty(url)) return false;
            
            string hashCode = GetHash(url);
            
            FileDownloadedInfo fileInfo = null;
            if (memCache.TryGetValue(hashCode, out fileInfo))
                return true;
            
            if (!ignoreFileCache)
            {
                string path = GetCachedPath(hashCode, cacheType);
                if (FileUtils.Exist(path))
                    return true;
            }
            
            return false;
        }

        public bool CheckRequestedFileByURL(string url)
        {
            if(string.IsNullOrEmpty(url)) return false;

            string hashCode = GetHash(url);
            return CheckRequestedFileByHash(hashCode);
        }

        public bool CheckRequestedFileByHash(string hashCode)
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
        
        public void LoadFile(string url, CacheType cacheType = CacheType.None, bool priority = false, 
            Action<string> onSuccess = null, Action<FileDownloadedInfo> onLoaded = null,
            Action<float> onProgress = null, Action<FileDownloadError> onFailed = null)
        {
            string hashCode = GetHash(url);

            FileDownloadedInfo fileInfo = null;
            if (memCache.TryGetValue(hashCode, out fileInfo))
            {
                if (onLoaded != null)
                    onLoaded.Invoke(fileInfo);
                
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
                        fileInfo = new FileDownloadedInfo();
                        fileInfo.url = url;
                        fileInfo.path = path;
                        fileInfo.data = FileUtils.Read(path);
                        memCache[hashCode] = fileInfo;
                        onLoaded.Invoke(fileInfo);
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
                    downloadRequest.request = new UnityWebRequest(downloadRequest.url);
                    downloadRequest.request.downloadHandler = new DownloadHandlerBuffer();
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
                            FileDownloadError failedInfo = new FileDownloadError
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
                        string path = "";

                        if (!ignoreFileCache)
                        {
                            CheckCachedFolder(downloadRequest.cacheType);
                            
                            path = GetCachedPath(downloadRequest.hashCode, downloadRequest.cacheType);

                            if (!FileUtils.Exist(path))
                            {
                                byte[] bytes = downloadRequest.request.downloadHandler.data;
                                FileUtils.Write(path, bytes);
                            }
                        }
                        
                        if (downloadRequest.onLoaded != null)
                        {
                            FileDownloadedInfo fileData = null;
                            if (!memCache.TryGetValue(downloadRequest.hashCode, out fileData))
                            {
                                fileData = new FileDownloadedInfo();
                                fileData.url = downloadRequest.url;
                                fileData.path = path;
                                fileData.data = downloadRequest.request.downloadHandler.data;

                                memCache[downloadRequest.hashCode] = fileData;
                            }
                            
                            downloadRequest.onLoaded.Invoke(fileData);
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

            string extension = System.IO.Path.GetExtension(source);
            if(!string.IsNullOrEmpty(extension))
                return sBuilder.ToString() + extension;

            return sBuilder.ToString();
        }
    }
}

