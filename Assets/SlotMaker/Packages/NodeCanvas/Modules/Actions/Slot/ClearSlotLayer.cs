using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Slot
{
    [Category("✶ Slots/Slot")]
    public class ClearSlotLayer : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;
        public BBParameter<int> layer;

        protected override void OnExecute()
        {
            mediator.value.ClearLayer(layer.value);
            EndAction();
        }
    }
}