using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicAlbum
{
    [Category("★ BagelCode/EpicAlbum")]
    public class GetEpicAlbumNew : ActionTask
    {
        public BBParameter<bool> newExist;
        public BBParameter<int> newCount;
        public BBParameter<string> newCountText;
        
        protected override string info
        {
            get { return "Get Epic Album New"; }
        }

        protected override void OnExecute()
        {
            var enabledEpicAlbumRewardCount = BlackboardUtils.GetOrCreateVariable<int>( MainBlackboard.Get(), "enabledEpicAlbumRewardCount");

            if(enabledEpicAlbumRewardCount != null && enabledEpicAlbumRewardCount.value > 0)
            {
                newCount.value = enabledEpicAlbumRewardCount.value;
                newCountText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMA_NUMBER", enabledEpicAlbumRewardCount.value);
                newExist.value = true;
            }
            else
            {
                newCount.value = 1;
                newCountText.value = "N";
                newExist.value = BlackboardQueryUtils.CheckIfNewExistInEpicAlbum();
            }
            
            EndAction(true);
        }
    }
}