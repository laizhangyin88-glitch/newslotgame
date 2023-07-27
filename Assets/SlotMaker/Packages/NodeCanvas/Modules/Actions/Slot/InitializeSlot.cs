using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.Slot
{
    [Category("✶ Slots/Slot")]
    public class InitializeSlot : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;

        protected override string info
        {
            get { return string.Format("{0}.Init()", mediator); }
        }

        protected override void OnUpdate()
        {
            if (mediator.value.Instance)
            {
                mediator.value.Initialize();
                EndAction();
            }
        }
    }
}