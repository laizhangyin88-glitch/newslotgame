using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ BagelCode/Utils")]
    public class CheckOrCreateVariable<T> : ConditionTask<Blackboard>
    {
        [BlackboardOnly]
        public string key;
        public BBParameter<T> value;

        protected override string info
        {
            get { return "\"" + key + "\" == " + value.value; }
        }

        protected override bool OnCheck()
        {
            T currentValue = BlackboardUtils.GetOrCreateVariable<T>(agent, key).value;
            return ParadoxNotion.ObjectUtils.TrueEquals(currentValue, value.value);
        }
    }
}
