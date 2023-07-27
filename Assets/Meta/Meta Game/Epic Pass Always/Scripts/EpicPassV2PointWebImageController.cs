using System.Collections;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class EpicPassV2PointWebImageController : EarningMetaGameItemController
    {
        private ContextElement webImageElement;

        protected override void Start()
        {
            base.Start();
            webImageElement = ContextUtils.FindElement(root, "Image", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetWebImage(webImageElement, EpicPassUtilsV2.PointIconImageUrl);
        }
    }
}