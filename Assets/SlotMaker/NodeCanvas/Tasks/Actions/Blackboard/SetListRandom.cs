using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("★ SlotMaker/Blackboard")]
	[Description("Set a blackboard integer list variable at random between min and max value")]
	public class SetListRandom : ActionTask
    {
		public BBParameter<int> minValue;
		public BBParameter<int> maxValue;
        public BBParameter<int> count;

        // optional
        public BBParameter<List<int>> exceptionList = null;

		[BlackboardOnly]
		public BBParameter<List<int>> saveAs;

		protected override string info{
			get {return "Set " + saveAs + " Random(" + minValue + ", " + maxValue + ")";}
		}

        private int GetRangeRandom(int begin, int end)
        {
            int ret = (int)Mathf.Floor(UnityEngine.Random.Range(begin, end + 1));
            if (ret > end) ret = end;
            return ret;
        }

		protected override void OnExecute()
        {
            List<int> list = new List<int>();

            for (int i = 0; i < count.value; ++i)
            {
                while (true)
                {
                    int ret = GetRangeRandom(minValue.value, maxValue.value);
                    if (!list.Contains(ret))
                    {
                        list.Add(ret);
                        break;
                    }
                }
            }

            if (!exceptionList.isNone)
            {
                for (int i = 0; i < exceptionList.value.Count; ++i)
                {
                    int val = exceptionList.value[i];
                    if (list.Contains(val))
                    {
                        list.Remove(val);
                    }
                }
            }

            saveAs.value = list;
			EndAction();
		}
	}
}
