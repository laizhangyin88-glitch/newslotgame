using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("★ SlotMaker/Blackboard")]
	[Description("Set a blackboard integer list variable at random between min and max value")]
	public class SetListShuffle<T> : ActionTask
    {
        public BBParameter<List<T>> valueA;
        public bool clone;

        public BBParameter<List<T>> saveAs;

        protected override string info
        {
            get { return string.Format("Shuffle {0}", valueA); }
        }

        private int GetRangeRandom(int begin, int end)
        {
            int ret = (int)Mathf.Floor(UnityEngine.Random.Range(begin, end + 1));
            if (ret > end) ret = end;
            return ret;
        }

        private List<int> GetUniqueRandomList(int minValue, int maxValue, int count)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                while (true)
                {
                    int ret = GetRangeRandom(minValue, maxValue);
                    if (!list.Contains(ret))
                    {
                        list.Add(ret);
                        break;
                    }
                }
            }

            return list;
        }

        protected override void OnExecute()
        {
            List<T> dst;
            List<int> rnd = GetUniqueRandomList(0, valueA.value.Count - 1, valueA.value.Count);
            if (clone)
            {
                dst = new List<T>(valueA.value.Count);
                for (int i = 0; i < valueA.value.Count; ++i)
                {
                    dst.Add(valueA.value[rnd[i]]);
                }
                valueA.value = dst;
            }
            else
            {
                dst = valueA.value;
                for (int i = 0; i < dst.Count; ++i)
                {
                    T tempA = dst[i];
                    dst[i] = dst[rnd[i]];
                    dst[rnd[i]] = tempA;
                }
            }
            saveAs.value = dst;
            EndAction();
        }
    }
}
