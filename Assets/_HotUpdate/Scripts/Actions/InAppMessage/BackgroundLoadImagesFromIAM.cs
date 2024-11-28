using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/IAM")]
    public class BackgroundLoadImagesFromIAM : ActionTask<Blackboard>
    {
        public BBParameter<string> iamInfoValue;

        protected override string info
        {
            get { return string.Format("Background Load IAM Images({0})", iamInfoValue); }
        }

        protected override void OnExecute()
        {
            var iamInfo = BlackboardUtils.FindVariable<Blackboard>(agent, iamInfoValue.value);
            List<string> imageUrlList = IAMUtils.GetImageUrlList(iamInfo.value);

            if(imageUrlList.Count > 0)
            {
                for (int i = 0; i < imageUrlList.Count; ++i)
                {
                    WebImageDownloadManager.Instance.LoadWebImage( 
                        imageUrlList[i],
                        CacheType.FileCache,
                        true
                    );
                }
            }
            
            EndAction();
        }
    }

}
