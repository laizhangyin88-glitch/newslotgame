using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SlotMaker.Tasks.Actions.Contents
{
    public class SetBlackboardValue : ActionTask<Blackboard>
    {
        public BBParameter<string> valueA;
        [BlackboardOnly]
        public BBParameter<long> newValue;

        protected override string info 
        {
            get { return string.Format("{0} = {1}", newValue, valueA); }
        }

        protected override void OnExecute()
        {

            BlackboardUtils.GetOrCreateVariable<long>(agent, valueA.value).value = newValue.value;
            EndAction();

        }
    }
}
