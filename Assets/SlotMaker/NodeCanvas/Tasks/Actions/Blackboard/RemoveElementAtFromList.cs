using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

    [Category("✫ Blackboard/Lists")]
    [Description("Remove an element from the target list")]
    public class RemoveElementAtFromList<T> : ActionTask{

        [RequiredField] [BlackboardOnly]
        public BBParameter<List<T>> targetList;
        public BBParameter<int> targetIndex;

        protected override string info{
             get {return string.Format("Remove At {0} From {1}", targetIndex, targetList);}
        }

        protected override void OnExecute(){
            targetList.value.RemoveAt(targetIndex.value);
            EndAction(true);
        }
    }
}