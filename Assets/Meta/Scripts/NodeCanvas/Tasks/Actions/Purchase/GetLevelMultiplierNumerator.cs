using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/Purchase")]
    public class GetLevelMultiplierNumerator : ActionTask<Blackboard>
    {
        public BBParameter<long> valueA;
        public BBParameter<string> valueB;
        public BBParameter<long> saveValue;

        protected override string info
        {
            get { return string.Format("{0} = Get Level Multiplier Numerator({1}, LM type:{2})", saveValue, valueA, valueB); }
        }

        protected override void OnExecute()
        {
            saveValue.value = LevelUtils.GetLevelMultiplierNumeratorValue(valueA.value, valueB.value);
            EndAction();
        }
    }
}