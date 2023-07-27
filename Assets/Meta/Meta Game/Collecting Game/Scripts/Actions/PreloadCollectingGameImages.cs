using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class PreloadCollectingGameImages : ActionTask
    {
        private int requestImageCount;
        
        protected override string info
        {
            get { return "Preload Collecting Game Images"; }
        }

        protected override void OnExecute()
        {
            List<string> imageUrlList = BlackboardQueryUtils.GetCollectingGameImageUrlList();
            
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

            if(requestImageCount <= 0)
                EndAction();
        }
    }
}