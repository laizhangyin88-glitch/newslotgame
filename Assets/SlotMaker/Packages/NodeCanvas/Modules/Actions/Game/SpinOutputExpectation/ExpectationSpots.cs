using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Expectation
{
    [Category("✶ Slots/Expectation")]
    public class ExpectationSpots : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;

        protected override string info
        {
            get { return string.Format("{0}.ExpectationSpots()", target); }
        }

        protected override void OnExecute()
        {
            SpinOutputExpectationSubset expectation = target.value as SpinOutputExpectationSubset;
            expectation.ExpectationSpots();
            EndAction();
        }
    }
}