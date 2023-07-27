using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_Win : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<KenoWin> kenoWin;

        protected override string info { get { return $"{mediator}.Win"; } }

        protected override void OnExecute()
        {
            mediator.value.Win(kenoWin.value);
            EndAction();
        }
    }
}