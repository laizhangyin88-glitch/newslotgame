using System.Collections.Generic;
using BagelCode;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class PreloadImages : ActionTask
    {
        public BBParameter<int> requests = 0;

        private PerformanceAnalyzer.TimeSample timeSample = new PerformanceAnalyzer.TimeSample("preload", "webimage");

        protected override void OnExecute()
        {
            timeSample.BeginSample();

            List<string> imageUrlList = new List<string>();

            // caching shop images
            var shopList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "shopList");
            if (shopList.value != null)
            {
                for (int i = 0; i < shopList.value.Count; ++i)
                {
                    string url = BlackboardUtils.FindVariable<string>(shopList.value[i], "imageUrl").value;
                    if (!string.IsNullOrEmpty(url))
                    {
                        imageUrlList.Add(url);
                    }
                }
            }

            // caching notice images
            var noticeList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "noticeList");

            if (noticeList.value != null)
            {
                for (int i = 0; i < noticeList.value.Count; ++i)
                {
                    string url = BlackboardUtils.FindVariable<string>(noticeList.value[i], "imageUrl").value;
                    if (!string.IsNullOrEmpty(url))
                    {
                        imageUrlList.Add(url);
                    }
                }
            }

            // slot Banner Images
            var slotBannerImageList = BagelCode.BlackboardQueryUtils.GetSlotBannerImageList();
            if (slotBannerImageList != null)
            {
                for (int i = 0; i < slotBannerImageList.Count; ++i)
                {
                    if (!string.IsNullOrEmpty(slotBannerImageList[i]))
                    {
                        imageUrlList.Add(slotBannerImageList[i]);
                    }
                }
            }

            // caching IAM images
            imageUrlList.AddRange(BlackboardQueryUtils.GetInAppMessageWebImageUrlList());

            var bingoInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "/dailyBingoInfo");

            if (bingoInfo != null)
            {
                string url = BlackboardUtils.FindVariable<string>(bingoInfo.value, "board/backgroundUrl").value;
                if (!string.IsNullOrEmpty(url))
                {
                    imageUrlList.Add(url);
                }
            }

            requests.value = imageUrlList.Count;

            for (int i = 0; i < imageUrlList.Count; ++i)
            {
                WebImageDownloader.Instance.LoadWebImage(
                    imageUrlList[i],
                    CacheType.FileCache,
                    true,
                    imageDownloaded,
                    null,
                    null,
                    failed
                );
            }
        }

        protected override void OnUpdate()
        {
            if (requests.value > 0) return;

            timeSample.EndSample();

            EndAction();
        }

        private void imageDownloaded(string url)
        {
            requests.value--;
        }

        private void failed(WebImageDownloader.WebImageDownloadError error)
        {
            requests.value--;
        }
    }
}