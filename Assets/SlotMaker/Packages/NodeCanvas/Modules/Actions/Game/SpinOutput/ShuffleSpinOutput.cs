using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker.Slots.Strategy;

namespace SlotMaker.Slots.Tasks.Actions.Output
{
    [Category("✶ Slots/Output")]
    public class ShuffleSpinOutput : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutput> target;

        protected override string info
        {
            get { return string.Format("{0}.Shffle()", target); }
        }

        protected override void OnExecute()
        {
            target.value.Shuffle();
            EndAction();
        }
    }
}