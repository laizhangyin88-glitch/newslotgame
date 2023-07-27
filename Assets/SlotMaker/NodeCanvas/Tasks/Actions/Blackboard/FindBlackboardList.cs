using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard/Generic")]
[Description("Will sort the Blackboard in the target list by Blackboard's certain key and save that list to the blackboard")]
    public class FindBlackboardList<T> : ActionTask<Blackboard> where T : IComparable {
        public enum CompareType {
            LessThan = -1,
            EqualTo = 0,
            GreaterThan = 1
        };
        public BBParameter<string> targetList;
        public BBParameter<string> key;
        public BBParameter<CompareType> compareType;
        public BBParameter<T> compare;
        [BlackboardOnly]
        public BBParameter<Blackboard> saveAs;

        protected override string info{
            get {
                return "Find " + targetList + "[i]."+key+" "+compareType+" "+compare+" as " + saveAs;
            }
        }

        protected override void OnExecute(){
            var listBB = BlackboardUtils.FindVariable<List<Blackboard>>(agent, targetList.value);
            if (listBB == null) {
                Debug.LogError("[Blackboard] Null blackboard list founded in " + targetList.value);
                EndAction(false);
            } else {
                saveAs.value = listBB.value.Find(x => 
                        BlackboardUtils.FindVariable<T>(x, key.value).value.CompareTo(compare.value) == (int)compareType.value
                        );
            }
            EndAction();
        }
    }
}
