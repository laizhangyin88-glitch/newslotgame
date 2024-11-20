using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/IAM")]
    public class BackgroundLoadImages : ActionTask
    {
        protected override string info
        {
            get { return string.Format("Background Load IAM Images"); }
        }

        protected override void OnExecute()
        {
            BlackboardQueryUtils.LoadInAppMessageWebImages(CacheType.FileCache, true);

            EndAction();
        }
    }

}
