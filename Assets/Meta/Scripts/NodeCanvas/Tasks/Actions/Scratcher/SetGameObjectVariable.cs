using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Category("✫ Blackboard")]
    public class SetCallerVariable<T> : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<GameObject> obj;
        public string key;
        public BBParameter<T> value;

        protected override string info
        {
            get { return obj + "." + key + " = " + value; }
        }

        protected override void OnExecute()
        {
            var callerBB = obj.value.GetComponent<Blackboard>();
            if (callerBB == null)
            {
                EndAction(false);
                return;
            }

            callerBB.AddVariable(key, value.value);
            EndAction();
        }
    }
}
