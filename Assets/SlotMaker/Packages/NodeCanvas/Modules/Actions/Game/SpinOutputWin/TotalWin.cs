using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Win
{
    [Category("✶ Slots/Win")]
    public class TotalWin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;

        protected override string info
        {
            get { return string.Format("{0}.TotalWin()", target); }
        }

        protected override void OnExecute()
        {
            SpinOutputWinningSubset win = target.value as SpinOutputWinningSubset;
            win.TotalWin();
            EndAction();
        }
    }
}