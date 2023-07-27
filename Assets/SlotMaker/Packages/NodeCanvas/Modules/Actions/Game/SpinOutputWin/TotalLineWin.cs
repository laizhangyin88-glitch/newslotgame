using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Win
{
    [Category("✶ Slots/Win")]
    public class TotalLineWin : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;

        protected override string info
        {
            get { return string.Format("{0}.TotalLineWin()", target); }
        }

        protected override void OnExecute()
        {
            SpinOutputLineWin win = target.value as SpinOutputLineWin;
            win.TotalLineWin();
            EndAction();
        }
    }
}