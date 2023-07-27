using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard")]
    public class SetListIndex<T> : ActionTask
    {
        public BBParameter<int> valueA;
        public OperationMethod Operation = OperationMethod.Set;
        public BBParameter<int> valueB;
        
        public BBParameter<List<T>> list;
        public bool repeat;

        protected override void OnExecute()
        {   
            valueA.value = OperationTools.Operate(valueA.value, valueB.value, Operation);

            int count = list.value.Count;
            if (valueA.value < 0)
                valueA.value = repeat ? (valueA.value + count) : 0;
            else if (valueA.value > (count - 1))
                valueA.value = repeat ? (valueA.value - count) : (count - 1);
            
            EndAction();
        }
    }
}
