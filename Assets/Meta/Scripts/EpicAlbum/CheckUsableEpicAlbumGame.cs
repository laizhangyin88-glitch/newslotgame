using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicAlbum
{
    [Category("★ BagelCode/EpicAlbum")]
    public class CheckUsableEpicAlbumGame : ConditionTask
    {
        protected override string info
        {
            get
            {
                return "Check Usable Epic Album Game";
            }
        }

        protected override bool OnCheck()
        {
            var gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId");
            // var earlyAccessInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId.value);
            bool enableEpicAlbum = BlackboardUtils.GetOrCreateVariable<bool>(null, "/values/misc/ENABLE_EPIC_ALBUM").value;
            bool gameExistInEpicAlbum = BlackboardQueryUtils.CheckIfGameExistInEpicAlbum(gameId.value);
            
            // return earlyAccessInfo == null && enableEpicAlbum && gameExistInEpicAlbum;
            return enableEpicAlbum && gameExistInEpicAlbum;
        }
    }
}