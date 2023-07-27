using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Slot
{
    [Category("✶ Slots/Slot")]
    public class SendEventToSlot : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;
        public BBParameter<string> eventName;

        protected override string info
        {
            get { return string.Format("{0}.SendEvent({1})", mediator, eventName); }
        }

        protected override void OnExecute()
        {
            mediator.value.SendEvent(eventName.value);
            EndAction();
        }
    }
}