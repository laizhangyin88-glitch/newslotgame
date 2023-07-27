using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/Blackboard/Generic")]
    public class AddListToList<T> : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<List<T>> listA;
        public BBParameter<List<T>> listB;

        protected override string info
        {
            get { return string.Format("{0}.AddRange({1})", listA, listB); }
        }

        protected override void OnExecute()
        {
            if (listA.value == null)
                listA.value = new List<T>(listB.value);
            else 
                listA.value.AddRange(listB.value);
                
            EndAction();
        }
    }
}
