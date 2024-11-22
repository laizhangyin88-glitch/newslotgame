using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ BagelCode/Utils")]
    public class SetOrCreateVariable<T> : ActionTask<Blackboard>
    {
        [BlackboardOnly]
        public string key;
        public BBParameter<T> value;

        protected override string info
        {
            get { return "SetOrCreateVariable " + key; }
        }

        protected override void OnExecute()
        {
            var variable = BlackboardUtils.GetOrCreateVariable<T>(agent, key);
            variable.value = value.value;
            EndAction();
        }
    }
}
