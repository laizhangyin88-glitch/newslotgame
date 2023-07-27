using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{

[Category("★ SlotMaker/Blackboard")]
[Description("Will sort the Blackboard in the target list by Blackboard's certain key and save that list to the blackboard")]
    public class SortBlackboardList : ActionTask<Blackboard> {
        public enum SortType {
            Ascending,
            Descending
        };
        public BBParameter<string> targetList;
        public BBParameter<List<string>> keys;
        public BBParameter<List<Type>> types;// Not Used
        public BBParameter<List<SortType>> sortType;
        public bool deepSearch = false;
        [BlackboardOnly]
        public BBParameter<List<Blackboard>> saveAs;

        private int findAndCompare(Blackboard xB, Blackboard yB, string key) {
            IComparable x = null;
            IComparable y = null;

            if (deepSearch)
            {
                x = (IComparable)BlackboardUtils.FindVariable(xB, key).value;
                y = (IComparable)BlackboardUtils.FindVariable(yB, key).value;
                return x.CompareTo(y);
            }

            x = (IComparable)xB.GetVariable(key).value;
            y = (IComparable)yB.GetVariable(key).value;
            return x.CompareTo(y);
        }

        protected override string info{
            get {
                return "Sort " + targetList + " by the multiplie criteria as " + saveAs;
            }
        }

        protected override void OnExecute() {

            var listBB = BlackboardUtils.FindVariable<List<Blackboard>>(agent, targetList.value);

            if (listBB == null)
            {
                Debug.LogError("[Blackboard] Null blackboard list founded in " + targetList.value);
                EndAction(false);
            }
            else
            {
                List<Blackboard> originalList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, targetList.value).value;

                var sortedList = new List<Blackboard>();
                for (int i = 0; i < originalList.Count; i++)
                {
                    sortedList.Add(originalList[i]);
                }

                sortedList.Sort(delegate(Blackboard xB, Blackboard yB) {
                        int result = 0;
                        for (int i = 0; i < keys.value.Count; i++) {
                            string key = keys.value[i];
                            // System.Type type = types.value[i];
                            SortType sort = sortType.value[i];
                            result = findAndCompare(xB, yB, key);
                            if (sort == SortType.Descending) result = -result;
                            if (result != 0) break;
                        }
                        return result;
                        });
                saveAs.value = sortedList;
            }
            EndAction();
        }
    }
}
