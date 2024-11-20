using System.Collections;
using System.Collections.Generic;
using BagelCode;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace SlotMaker.Tasks.Actions
{

[Category("★ BagelCode/Utils")]
public class DownloadImages : ActionTask
{
    public BBParameter<List<string>> imageUrlList;
    public BBParameter<bool> alwaysSuccess;

    public BBParameter<int> requests;
    public BBParameter<int> downloadCompleteCount;

    public BBParameter<float> saveAsProgress;
    public BBParameter<bool> saveAsSuccess;

    private bool isRequest = false;

    protected override string info
    {
        get
        {
            return string.Format("DownloadImages({0}), Always Success {1}", imageUrlList, alwaysSuccess.value);
        }
    }

    protected override void OnExecute()
    {
        isRequest = false;
        requests.value = 0;
        downloadCompleteCount.value = 0;
        saveAsProgress.value = 0f;
        saveAsSuccess.value = false;

        if(imageUrlList.value != null)
        {
            for (int i = 0; i < imageUrlList.value.Count; ++i)
            {
                if(!string.IsNullOrEmpty(imageUrlList.value[i]))
                {
                    isRequest = true;
                    requests.value++;
                    WebImageDownloader.Instance.LoadWebImage( 
                        imageUrlList.value[i],
                        CacheType.FileCache,
                        true,
                        imageDownloaded,
#if !UNITY_EDITOR && UNITY_WEBGL
                        imageLoaded,
#else
                        null,
#endif
                        null,
                        failed
                    );
                }
            }
        }
        

        if(!isRequest)
        {
            saveAsProgress.value = 1f;
            saveAsSuccess.value = true;
            EndAction(true);
        }
    }

    protected override void OnUpdate()
    {
        if(isRequest && requests.value == 0)
        {
            if(imageUrlList.value.Count == downloadCompleteCount.value)
            {
                saveAsProgress.value = 1f;
                saveAsSuccess.value = true;
            }
            else
            {
                saveAsSuccess.value = false;
            }

            if(alwaysSuccess.value)
                EndAction();
            else
                EndAction(saveAsSuccess.value);
        }
    }

    private void imageLoaded(Sprite imageSprite)
    {
        // For web-memory cache.
    }

    private void imageDownloaded(string url)
    {
        downloadCompleteCount.value++;
        requests.value--;

        saveAsProgress.value = (float)downloadCompleteCount.value / (float)imageUrlList.value.Count;
    }

    private void failed(WebImageDownloader.WebImageDownloadError error)
    {
        requests.value--;
    }
}

}
