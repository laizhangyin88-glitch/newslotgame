using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/VIP Lounge")]
    public class GetVIPLoungeRewardMultiplierNumerator : ActionTask
    {
        public BBParameter<long> saveAsNumerator;
        public BBParameter<double> saveAsMultiplier;

        protected override string info
        {
            get { return "Get VIP Lounge Reward Multiplier"; }
        }

        protected override void OnExecute()
        {
            saveAsNumerator.value = BlackboardQueryUtils.GetVIPLoungeClubVegasRewardNumerator();
            saveAsMultiplier.value = NumberUtils.GetMultiplierFromNumerator(saveAsNumerator.value);
            EndAction();
        }
    }
}