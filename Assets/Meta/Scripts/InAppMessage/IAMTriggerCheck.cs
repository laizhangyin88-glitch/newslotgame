using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public static class IAMTriggerCheck
    {
        public static bool TierCheck(Blackboard iamInfo)
        {
            var tier = TierUtils.GetMeTier();
            var triggerV2List = BlackboardUtils.GetOrCreateBlackboardList(iamInfo, "triggerV2List");

            for (int i = 0; i < triggerV2List.Count; i++)
            {
                var type = triggerV2List[i].GetValue<InAppMessageTriggerType>("type");
                if (type != InAppMessageTriggerType.TIER_UP) continue;
                
                var triggerTier = triggerV2List[i].GetValue<int>("tier");
                if (tier == triggerTier) return true;
            }

            return false;
        }
    }
}
