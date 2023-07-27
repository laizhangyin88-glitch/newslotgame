using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions.Contents
{
    [Category("★ SlotMaker/SlotMachine")]
    public class InitializeSymbols : ActionTask
    {
        public BBParameter<GameObject> slotMachine;
        public BBParameter<bool> shuffle;

        protected override void OnExecute()
        {
            var sm = slotMachine.value.GetComponent<BaseSlotMachine>();
            if (shuffle.value)
                sm.Shuffle();
            else
                sm.InitializeSymbols();

            EndAction();
        }
    }
}
