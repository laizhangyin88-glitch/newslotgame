using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Utils")]
    public class IsSlotAvailable : ConditionTask
    {
        public BBParameter<int> gameId;

        protected override string info
        {
            get { return "Is Slot Available:" + (gameId?.value ?? -1); }
        }

        protected override bool OnCheck()
        {
            int targetGameId = gameId?.value ?? -1;
            var slotInfo = BlackboardQueryUtils.GetEarlyAccessSlotInfo(targetGameId);
            if (slotInfo == null)
            {
                slotInfo = BlackboardQueryUtils.GetSlotInfoBB(targetGameId);
            }

            if (slotInfo != null)
            {
                int flags = BlackboardUtils.FindVariable<int>(slotInfo, "flags/status")?.value ?? 0;
                if (flags != 4) return true;
            }

            return false;
        }
    }
}