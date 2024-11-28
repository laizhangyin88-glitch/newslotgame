using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TierUtils")]
    public class GetDiffTimeBonusCoins : ActionTask
    {
        public BBParameter<int> fromTier;
        public BBParameter<int> targetTier;

        [BlackboardOnly]
        public BBParameter<long> saveAs;

        protected override string info
        {
            get { return "Get Diff Time Bonus Coins"; }
        }

        protected override void OnExecute()
        {
            saveAs.value = TierUtils.GetDiffTimeBonusCoins(fromTier.value, targetTier.value);
            
            EndAction();
        }
    }
}


