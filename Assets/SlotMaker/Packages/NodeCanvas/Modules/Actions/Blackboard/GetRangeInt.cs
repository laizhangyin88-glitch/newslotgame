using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker.IoC;

namespace SlotMaker.Slots.Tasks.Actions.Blackboards
{
    [Category("✶ Slots/Blackboard")]
    public class GetRangeInt : ActionTask<Blackboard>
    {
        public BBParameter<int> valueA;
        public BBParameter<List<int>> range = new List<int>{ 0, int.MaxValue };

        public enum RangeOperation
        {
            RangeIndex,
            RangeValue,
            InverseRangeValue
        };
        public RangeOperation rangeOperation = RangeOperation.RangeValue;

        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = GetRange({1}, {2}, {3})", saveAs, valueA, range, rangeOperation); }
        }

        protected override void OnExecute()
        {
            for (int i = range.value.Count - 1; i >= 0; --i)
            {
                if (valueA.value >= range.value[i])
                {
                    if (rangeOperation == RangeOperation.RangeIndex)
                        saveAs.value = i;
                    else if (rangeOperation == RangeOperation.RangeValue)
                        saveAs.value = valueA.value - range.value[i];
                    else if (rangeOperation == RangeOperation.InverseRangeValue)
                    {
                        if (i < (range.value.Count - 1))
                            saveAs.value = range.value[i + 1] - valueA.value;
                        else 
                            saveAs.value = 0;// Clamped
                    }
                    break;
                }
            }

            EndAction();
        }
    }
}