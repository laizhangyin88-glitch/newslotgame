using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Blackboard")]
	public class CheckStringIsNullOrEmptyFromList : ConditionTask {

		[RequiredField] [BlackboardOnly]
		public BBParameter<List<string>> targetList;
		public BBParameter<int> index;
		[BlackboardOnly]
        public BBParameter<string> saveAs;

		protected override string info{
			get {return string.Format("{0}[{1}] == null or empty", targetList, index);}
		}

		protected override bool OnCheck(){
            if (index.value < 0 || index.value >= targetList.value.Count){
				return false;
			}

			saveAs.value = targetList.value[index.value];
			return string.IsNullOrEmpty(saveAs.value);
		}
	}
}
