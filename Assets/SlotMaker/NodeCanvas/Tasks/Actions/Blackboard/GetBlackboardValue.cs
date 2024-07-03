using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Sirenix.Utilities;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Blackboard/Generic")]
    public class GetBlackboardValue<T> : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        [BlackboardOnly]
        public BBParameter<T> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = {1}", saveAs, valueA); }
        }

        protected override void OnExecute()
        {
            var variable = BlackboardUtils.FindVariable<T>(agent, valueA.value);           
            if (variable == null)
            {
                Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + valueA.value);
                EndAction(false);
            }
            else
            {
                saveAs.value = variable.value;
                EndAction();
            }
        }
    }

}
