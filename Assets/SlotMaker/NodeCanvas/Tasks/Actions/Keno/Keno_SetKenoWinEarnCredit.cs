using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_SetKenoWinEarnCredit : ActionTask<Blackboard>
    {
        public BBParameter<KenoWin> kenoWin;

        // Save As
        public BBParameter<long> earnCredit;

        protected override void OnExecute()
        {
            kenoWin.value.earnCredit = earnCredit.value;

            EndAction();
        }
    }
}
