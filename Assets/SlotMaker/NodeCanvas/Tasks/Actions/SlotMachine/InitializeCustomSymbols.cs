using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace SlotMaker.Tasks.Actions.Contents
{
    [Category("★ SlotMaker/SlotMachine")]
    public class InitializeCustomSymbols : ActionTask
    {
        public BBParameter<GameObject> slotMachine;
        public BBParameter<List<int>> indices;

        protected override void OnExecute()
        {
            var sm = slotMachine.value.GetComponent<BaseSlotMachine>();
            sm.Shuffle(indices.value);

            EndAction();
        }
    }
}
