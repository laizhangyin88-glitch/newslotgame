using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SlotMaker;
using UnityEngine;

namespace BagelCode
{
    public class WebImageDownloadManager : SlotMaker.MonoWeakSingleton<WebImageDownloadManager>
    {
        private class DownloadInfo
        {
            public int retryCount;
            public CacheType cacheType;
            public bool priority;
        }
        
        private Dictionary<string, DownloadInfo> downloadInfos = new Dictionary<string, DownloadInfo>();

        private static float WAIT_TIME = 1f;
        private static int MAX_RETRY_COUNT = 10;
        private bool isRunning = false;
        
        public void LoadWebImage(string url, CacheType cacheType = CacheType.None, bool priority = false)
        {
            DownloadInfo downloadInfo = new DownloadInfo
            {
                retryCount = MAX_RETRY_COUNT,
                cacheType = cacheType,
                priority = priority,
            };
            
            if (!downloadInfos.ContainsKey(url))
                downloadInfos.Add(url, downloadInfo);
            
            if(!isRunning)
            {
                isRunning = true;
                StartCoroutine("DownloadWebImage");
            }
        }
        
        private IEnumerator DownloadWebImage()
        {
            List<string> urlList;
            string url;
            
            while (true)
            {
                if (downloadInfos.Count == 0)
                    break;

                urlList = downloadInfos.Keys.ToList();
                
                for (int i = urlList.Count - 1; i >= 0; i--)
                {
                    url = urlList[i];
                    if (WebImageDownloader.Instance.CheckCachedImage(url, downloadInfos[url].cacheType))
                    {
                        OnSuccess(url);
                        continue;
                    }

                    if (WebImageDownloader.Instance.CheckRequestedImageByURL(url))
                    {
                        continue;
                    }
                    
                    WebImageDownloader.Instance.LoadWebImage(
                        url,
                        downloadInfos[url].cacheType,
                        downloadInfos[url].priority,
                        OnSuccess,
                        null,
                        null,
                        OnFailed
                    );
                    
                }
                
                yield return new WaitForSeconds(WAIT_TIME);
            }
            
            isRunning = false;
        }

        private void OnSuccess(string url)
        {
            downloadInfos.Remove(url);
        }

        private void OnFailed(WebImageDownloader.WebImageDownloadError error)
        {
            if (error.responseCode == 403 || error.responseCode == 404 || error.responseCode == 405)
            {
                downloadInfos.Remove(error.url);
                return;
            }

            int retryCount = downloadInfos[error.url].retryCount;
            downloadInfos[error.url].retryCount = retryCount - 1;
            
            if (downloadInfos[error.url].retryCount == 0)
                downloadInfos.Remove(error.url);
            
            if (!isRunning)
            {
                isRunning = true;
                StartCoroutine("DownloadWebImage");
            }
        }
    }
}