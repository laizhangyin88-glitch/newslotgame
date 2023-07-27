using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("★ SlotMaker/Blackboard")]
	public class SetListElements<T> : ActionTask{

		[RequiredField] [BlackboardOnly]
		public BBParameter<List<T>> targetList;
        public BBParameter<T> newValue;

		protected override void OnExecute()
        {
            for (int i = 0; i < targetList.value.Count; ++i)
            {
                targetList.value[i] = newValue.value;
            }
			EndAction(true);
		}
	}
}
