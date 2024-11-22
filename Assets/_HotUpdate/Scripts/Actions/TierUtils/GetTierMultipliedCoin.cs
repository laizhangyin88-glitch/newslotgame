using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TierUtils")]
    public class GetTierMultipliedCoin : ActionTask<Blackboard>
    {
        public BBParameter<long> coin;
        public BBParameter<int> tier;

        // Legacy Parameter. 
        public BBParameter<TierMultiplierTableType> multiplierType;
        
        public BBParameter<long> saveValue;
        
        protected override string info
        {
            get { return string.Format("{0} = Get Tier Multiplied Coin({1}, {2})", saveValue, coin, tier); }
        }

        protected override void OnExecute()
        {
            saveValue.value = TierUtils.GetTierFractionCoin(coin.value, tier.value);
            
            EndAction();
        }
    }
}


