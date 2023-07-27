using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("★ SlotMaker/Blackboard")]
	public class SetListRandomElements : ActionTask {

        public BBParameter<List<int>> values;
        public BBParameter<int> randomOffsetRange;

		[RequiredField] [BlackboardOnly]
		public BBParameter<List<int>> targetList;
		
		protected override void OnExecute()
        {
            for (int i = 0; i < targetList.value.Count; ++i)
            {
            	int pick = UnityEngine.Random.Range(0, values.value.Count);
            	int offset = (int)UnityEngine.Random.Range(0, randomOffsetRange.value);
                targetList.value[i] = values.value[pick] + offset;
            }
			EndAction(true);
		}
	}
}
