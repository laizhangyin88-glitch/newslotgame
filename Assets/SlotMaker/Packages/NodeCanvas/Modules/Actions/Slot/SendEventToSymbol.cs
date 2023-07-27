using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Slot
{
    [Category("✶ Slots/Slot")]
    public class SendEventToSymbol : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;
        public BBParameter<int> layer;
        public BBParameter<string> eventName;

        protected override string info
        {
            get { return string.Format("{0}[{1}].SendEvent({2})", mediator, layer, eventName); }
        }

        protected override void OnExecute()
        {
            mediator.value.SendSymbolEvent(eventName.value, layer.value);
            EndAction();
        }
    }
}