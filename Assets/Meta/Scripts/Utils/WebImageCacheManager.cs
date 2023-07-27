using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using SlotMaker;

namespace BagelCode
{
    public class WebImageCacheManager : MonoWeakSingleton<WebImageCacheManager>
    {
        private string CACHED_PATH;
        
        private void Awake()
        {
    #if !UNITY_WEBGL
            CACHED_PATH = ApplicationSettings.GetCachePath() + "/CachedImagesV2/";

            if(ApplicationSettings.Instance.webImageExpireDays <=0 ) return;
            if(!FileUtils.ExistDirectory(CACHED_PATH)) return;

            RemoveOldCaches();
    #endif
        }

        private void RemoveOldCaches()
        {
    #if !UNITY_WEBGL
            var txtFiles = Directory.EnumerateFiles(CACHED_PATH, "*.*", SearchOption.AllDirectories);

            DateTime currentDate = TimeUtils.GetLocalDateTime();
            DateTime targetDate = currentDate.AddDays(-ApplicationSettings.Instance.webImageExpireDays);

            foreach (string currentFile in txtFiles)
            {
                DateTime accessDate = File.GetLastAccessTime(currentFile);

                int compareResult = DateTime.Compare(targetDate, accessDate);

                if(compareResult > 0)
                {
                    try
                    {
                        File.Delete(currentFile);
                    }
                    catch(IOException error)
                    {
                        Debug.Log(error);
                        // Nothing to do. 
                    }
                }
            }
    #endif
        }
    }
}