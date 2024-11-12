using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/VIP Lounge")]
    public class GetVIPLoungeBenefitBonusMultiplierNumerator : ActionTask
    {
        public BBParameter<long> coin;
        public BBParameter<string> benefitBonusName;
        public BBParameter<long> saveValue;

        protected override string info
        {
            get { return string.Format("{0} = Get VIP Lounge Bonus({1}) * Coin({2})", saveValue, benefitBonusName, coin); }
        }

        protected override void OnExecute()
        {
            if (VipLounge.VipLounge.Utils.BadgeCount > 0)
                saveValue.value = NumberUtils.GetMultiplierNumeratorValue(coin.value, BlackboardQueryUtils.GetVIPLoungeClubVegasRewardActiveNumerator(benefitBonusName.value));
            else
                saveValue.value = coin.value;

            EndAction();
        }
    }
}