using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

    [Category("✫ Blackboard/Lists")]
    [Description("Remove an element from the target list")]
    public class RemoveElementRangeFromList<T> : ActionTask{

        [RequiredField] [BlackboardOnly]
        public BBParameter<List<T>> targetList;
        public BBParameter<int> startIndex;
        public BBParameter<int> count;

        protected override string info{
             get {return string.Format("Remove Range({0}, {1}) From {2}", startIndex, count, targetList);}
        }

        protected override void OnExecute(){
            targetList.value.RemoveRange(startIndex.value, count.value);
            EndAction(true);
        }
    }
}