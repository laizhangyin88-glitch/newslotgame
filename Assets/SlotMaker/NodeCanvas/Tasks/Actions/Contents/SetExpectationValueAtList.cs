using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class SetExpectationValueAtList : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> begin;
        public BBParameter<int> end;

        public BBParameter<bool> isExpectation;
        public BBParameter<List<bool>> masks;

        protected override string info { get { return string.Format("SetExpectationValueAtList({0}, {1}(include))", begin, end); } }

        public bool NeedExpectation(int reelIndex)
        {
            if (isExpectation.value)
                return isExpectation.value;
            else
            {
                if (masks.isNone)
                    return false;
                else
                    return masks.value[reelIndex];
            }
        }

        protected override void OnExecute()
    	{
            var expectation = ContentCustomData.GetSlotData(slotIndex.value).expectation;

            for (int i = begin.value; i <= end.value; ++i)
            {
                expectation.expectations[i] = NeedExpectation(i);
            }

            EndAction();
        }
    }
}
