using BagelCode;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.VipLounge;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("★ BagelCode/VIP Lounge")]
    public class CheckActiveVIPLoungeReward : ConditionTask<Blackboard>
    {
        protected override bool OnCheck()
        {
            // Tutorial & Vip Lounge data Check
            if (BlackboardQueryUtils.IsActiveTutorial() || !BlackboardQueryUtils.IsVipLoungeEnabled() || VipLounge.Utils.IsEnded)
                return false;

            return BlackboardQueryUtils.GetVIPLoungeClubVegasRewardAdditionalPercent() > 0L;
        }
    }
}