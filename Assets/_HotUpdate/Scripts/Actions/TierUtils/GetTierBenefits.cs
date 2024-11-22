using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TierUtils")]
    public class GetTierBenefits : ActionTask
    {
        public BBParameter<int> tier;

        [BlackboardOnly]
        public BBParameter<double> dailyBonusMultiplier;

        [BlackboardOnly]
        public BBParameter<long> vipCoins;

        [BlackboardOnly]
        public BBParameter<double> purchaseMultiplier;

        [BlackboardOnly]
        public BBParameter<long> timeBonusCoins;

        protected override string info
        {
            get { return string.Format("GetTierBenefits({0})", tier); }
        }

        protected override void OnExecute()
        {
            dailyBonusMultiplier.value  = TierUtils.GetTierMultiplier(tier.value);
            vipCoins.value              = TierUtils.GetVipCoins(tier.value);
            purchaseMultiplier.value    = TierUtils.GetTierMultiplier(tier.value);
            timeBonusCoins.value        = TierUtils.GetTimeBonusCoins(tier.value);

            EndAction();
        }
    }
}


