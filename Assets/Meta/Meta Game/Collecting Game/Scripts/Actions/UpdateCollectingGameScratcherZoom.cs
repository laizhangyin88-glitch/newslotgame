using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion.Design;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class UpdateCollectingGameScratcherZoom : ActionTask<ContextElement>
    {
        public BBParameter<int> currentScratcherId;
        public BBParameter<bool> isLoaded;
        public BBParameter<ContextElement> scratcherZoomInElement;
        
        protected override string info
        {
            get { return "Update Collecting Game Scratcher Zoom"; }
        }

        protected override void OnExecute()
        {
            scratcherZoomInElement.value = ContextUtils.FindElement(agent, "Scratcher Zoom In", ContextSearchingType.ChildrenSearch);
            
            ContextElement scratcherAreaElement = ContextUtils.FindElement(scratcherZoomInElement.value, "Scratcher Area", ContextSearchingType.ChildrenSearch);
            if (scratcherAreaElement.ChildCount == 0)
            {
                MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Image Scratcher", scratcherAreaElement.transform);
                scratcherAreaElement.UpdateContext(true);
            }

            ContextElement scratcherImageElement = ContextUtils.FindElement(scratcherAreaElement, "Base", ContextSearchingType.ChildrenSearch);
            Blackboard currentScratcher = BlackboardQueryUtils.GetScratcher(currentScratcherId.value);
            string sampleImageUrl = currentScratcher.GetValue<Blackboard>("reward").GetValue<string>("sampleImageUrl");
            MetaContextElementUtils.SetWebImage(
                scratcherImageElement,
                sampleImageUrl,
                CacheType.FileCache,
                false,
                () => { isLoaded.value = true; }
            );
            
            EndAction();
        }
    }
}