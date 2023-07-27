using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_SendSpotEventByIndex : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<string> eventName;
        public BBParameter<int> index;

        protected override string info { get { return $"spots[{index}].SendEvent({eventName})"; } }

        protected override void OnExecute()
        {
            mediator.value.KenoInstance.SendSpotEvent(index.value, eventName.value);

            EndAction();
        }
    }
}