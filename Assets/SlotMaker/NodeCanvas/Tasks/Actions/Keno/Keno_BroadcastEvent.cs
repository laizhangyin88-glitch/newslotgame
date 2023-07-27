using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_BroadcastEvent : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<string> eventName;

        protected override string info { get { return $"BroadcastEvent({eventName})"; } }

        protected override void OnExecute()
        {
            mediator.value.KenoInstance.BroadCastSpotEvent(eventName.value);

            EndAction();
        }
    }
}