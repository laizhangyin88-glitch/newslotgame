using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/IAM")]
public class PreloadImagesFromIAMById : ActionTask<Blackboard>
{
    public BBParameter<int> iamId;

    public BBParameter<bool> saveAsIsSuccess;

    private int requestImageCount = 0;

    protected override string info
    {
        get { return string.Format("Preload IAM Images(id = {0})", iamId); }
    }

    protected override void OnExecute()
    {
        saveAsIsSuccess.value = true;

        var iamInfo = BlackboardQueryUtils.GetIAMBlackboard(iamId.value);
        List<string> imageUrlList = IAMUtils.GetImageUrlList(iamInfo);

        if(imageUrlList.Count > 0)
        {
            requestImageCount = imageUrlList.Count;

            for (int i = 0; i < imageUrlList.Count; ++i)
            {
                WebImageDownloader.Instance.LoadWebImage( 
                    imageUrlList[i],
                    CacheType.FileCache,
                    true,
                    ImageDownloaded,
                    null,
                    null,
                    ImageDownloadFailed
                );
            }
        }
        else
        {
            EndAction();
        }
    }

    private void ImageDownloaded(string url)
    {
        --requestImageCount;

        if(requestImageCount <= 0)
            EndAction();
    }

    private void ImageDownloadFailed(WebImageDownloader.WebImageDownloadError error)
    {
        --requestImageCount;

        saveAsIsSuccess.value = false;

        if(requestImageCount <= 0)
            EndAction();
    }
}

}
