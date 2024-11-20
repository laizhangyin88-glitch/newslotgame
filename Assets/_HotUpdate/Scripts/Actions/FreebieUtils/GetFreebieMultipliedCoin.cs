using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/FreebieUtils")]
    public class GetFreebieMultipliedCoin : ActionTask
    {
        public BBParameter<long> coin;
        public BBParameter<FreebieLevelUtils.FreebieType> freebieType;
        public BBParameter<long> saveValue;

        protected override string info
        {
            get { return string.Format("{0} = Get Freebie({1}) Multiplied Coin({2})", saveValue, freebieType, coin); }
        }

        protected override void OnExecute()
        {
            saveValue.value = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(coin.value, freebieType.value);

            EndAction();
        }
    }
}