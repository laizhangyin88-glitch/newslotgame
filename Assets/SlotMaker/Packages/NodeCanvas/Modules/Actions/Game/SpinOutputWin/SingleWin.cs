using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Win
{
    [Category("✶ Slots/Win")]
    public class SingleWin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public BBParameter<int> index;

        protected override string info
        {
            get { return string.Format("{0}[{1}].SingleWin()", target, index); }
        }

        protected override void OnExecute()
        {
            SpinOutputWinningSubset win = target.value as SpinOutputWinningSubset;
            win.SingleWin(index.value);
            EndAction();
        }
    }
}