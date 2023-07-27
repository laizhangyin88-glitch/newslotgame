using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.EpicAlbum
{
    [Category("★ BagelCode/EpicAlbum")]
    public class CheckIfNewEpicRecord : ConditionTask
    {
        public BBParameter<long> earnCredit;
        
        protected override string info
        {
            get
            {
                return "Check If New Epic Record";
            }
        }

        protected override bool OnCheck()
        {
            var gameId = BlackboardUtils.GetOrCreateVariable<int>(null, "./game/gameId");
            bool newEpicRecord = BlackboardQueryUtils.CheckIfNewEpicRecord(gameId.value, earnCredit.value);
            var earlyAccessInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(gameId.value);
            bool enableEpicAlbum = BlackboardUtils.GetOrCreateVariable<bool>(null, "/values/misc/ENABLE_EPIC_ALBUM").value;
            bool gameExistInEpicAlbum = BlackboardQueryUtils.CheckIfGameExistInEpicAlbum(gameId.value);
            
            return newEpicRecord && earlyAccessInfo == null && enableEpicAlbum && gameExistInEpicAlbum;
        }
    }
}