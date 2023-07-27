using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.ScatterWin
{
    [Category("✶ Slots/Win/Scatter")]
    public class CalcScatterWin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public BBParameter<long> betCredit;
        public BBParameter<long> multiplier = 1L;

        [BlackboardOnly]
        public BBParameter<long> earnCredit;
        [BlackboardOnly]
        public BBParameter<int> scatterCount;

        protected override string info
        {
            get { return string.Format("{0}.CalcScatterWin()", target); }
        }

        protected override void OnExecute()
        {
            SpinOutputScatterWin scatterWin = target.value as SpinOutputScatterWin;

            scatterWin.earnCredit = 0L;
            scatterWin.multiplier = multiplier.value;
            scatterWin.betCredit = betCredit.value;

            scatterWin.CalcScatterWin();

            if (!earnCredit.isNone) 
                earnCredit.value = scatterWin.earnCredit;

            if (!scatterCount.isNone) 
                scatterCount.value = scatterWin.win.hitCount;

            EndAction();
        }
    }
}