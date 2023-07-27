using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ BagelCode/Utils")]
    public class GetOrCreateVariable<T> : ActionTask<Blackboard>
    {
        [BlackboardOnly]
        public string key;
        public BBParameter<T> saveAs;

        protected override string info
        {
            get { return "GetOrCreate " + key; }
        }

        protected override void OnExecute()
        {
            saveAs.value = BlackboardUtils.GetOrCreateVariable<T>(agent, key).value;
            EndAction();
        }
    }
}
