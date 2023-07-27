using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

    [Category("★ SlotMaker/Blackboard/Generic")]
    public class SetBlackboardValueListAtDictionaryLongKey<T> : ActionTask<Blackboard>
    {
        public BBParameter<string> destination;
        public BBParameter<long> key;
        public BBParameter<List<T>> value;

        protected override string info
        {
            get { return string.Format("{0}[{1}] = {2}", destination, key, value); }
        }

        protected override void OnExecute()
        {
            var variable = BlackboardUtils.FindVariable<Dictionary<long, List<T>>>(agent, destination.value);
            if (variable == null)
            {
                Debug.LogError("[Blackboard](" + agent.name + ") Null variable founded in " + destination.value);
                EndAction(false);
            }
            else
            {
                variable.value[key.value] = value.value;
                EndAction();
            }
        }
    }

}
