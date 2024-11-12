using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ Blackboard")]
    public class AddVariable<T> : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<Blackboard> bb;
        public string key;
        public BBParameter<T> value;

        protected override string info
        {
            get { return "Add " + key + " " + value; }
        }

        protected override void OnExecute()
        {
            bb.value.AddVariable(key, value.value);
            EndAction();
        }
    }
}