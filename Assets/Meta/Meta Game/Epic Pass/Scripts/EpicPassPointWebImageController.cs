using SlotMaker;

namespace BagelCode
{
    public class EpicPassPointWebImageController : EarningMetaGameItemController
    {
        private ContextElement webImageElement;

        protected override void Start()
        {
            base.Start();
            webImageElement = ContextUtils.FindElement(root, "Image", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetWebImage(webImageElement, EpicPassUtils.PointImageUrl);
        }
    }
}
