using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Expectation
{
    [Category("✶ Slots/Expectation")]
    public class GetExpectationSpotsCount : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;

        public BBParameter<int> saveAs;

        protected override string info
        {
            get { return string.Format("{0} = {1}.SpotsCount", saveAs, target); }
        }

        protected override void OnExecute()
        {
            SpinOutputExpectationSubset expectation = target.value as SpinOutputExpectationSubset;
            var list2 = expectation.GetExpectedSpots();

            int count = 0;
            foreach (var list in list2)
            {
                count += list.Count;
            }
            saveAs.value = count;

            EndAction();
        }
    }
}